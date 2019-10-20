using DestinyPCLoadoutManager.API;
using DestinyPCLoadoutManager.Auth.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DestinyPCLoadoutManager.Auth
{
    class OAuthManager
    {
        private static string authorizationEndpoint = "https://www.bungie.net/en/OAuth/Authorize";
        private static string tokenEndpoint = "https://www.bungie.net/Platform/App/OAuth/token/";
        private string clientId;
        private string clientSecret;
        private string port;

        public TokenResponse currentToken;
        public bool IsAuthorized
        {
            get
            {
                return currentToken != null;
            }
        }

        public event EventHandler<bool> AuthEvent;

        public OAuthManager(string clientId, string clientSecret, string port)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.port = port;
        }

        public async Task StartAuth()
        {
            var savedToken = Properties.Settings.Default.AuthToken;

            if (savedToken != null)
            {
                // Attempt to use auth code
                currentToken = savedToken;

                var accountManager = App.provider.GetService(typeof(AccountManager)) as AccountManager;

                var result = false;

                try
                {
                    result = await accountManager.GetAccount(true) != null;
                }
                catch (UnauthorizedAccessException _)
                {
                    // Do nothing
                }

                if (result)
                {
                    // Token success
                    AuthEvent.Invoke(this, true);
                    return;
                }
                else
                {
                    AuthEvent.Invoke(this, false);
                }

                try
                {
                    result = await RefreshToken() != null && await accountManager.GetAccount(true) != null;
                }
                catch (UnauthorizedAccessException _)
                {
                    // Do nothing
                }

                if (result)
                {
                    // Refresh succeeded and token success
                    AuthEvent.Invoke(this, true);
                    return;
                }

                currentToken = null;
            }
        }

        public string GenerateAuthorizationUrl()
        {
            // Generates state and PKCE values.
            string state = randomDataBase64url(32);

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&redirect_uri={1}&client_id={2}&state={3}",
                authorizationEndpoint,
                System.Uri.EscapeDataString(RedirectUri()),
                clientId,
                state);

            return authorizationRequest.Replace("&", "^&");
        }

        public async void ProcessUriAndExchangeCode(string[] args)
        {
            var code = ParseUriResponse(args);

            if (code == null)
            {
                return;
            }

            await ExchangeCode(code);
        }

        public string ParseUriResponse(string[] args)
        {
            if (args.Length < 1)
            {
                return null;
            }

            var firstSegment = args[0];

            var splitCode = firstSegment.Split("?");

            if (splitCode.Length < 2)
            {
                // No query params
                return null;
            }

            var parsedQuery = HttpUtility.ParseQueryString(splitCode[1]);

            return parsedQuery.Get("code");
        }

        public async Task<TokenResponse> ExchangeCode(string code)
        {
            // builds the request
            string tokenRequestBody = string.Format("grant_type=authorization_code&code={0}&client_id={1}&client_secret={2}", code, clientId, clientSecret);

            return await RequestToken(tokenRequestBody);
        }

        public async Task<TokenResponse> RefreshToken()
        {
            var refreshToken = currentToken?.RefreshToken;

            if (refreshToken == null)
            {
                return null;
            }

            // builds the request
            string tokenRequestBody = string.Format("grant_type=refresh_token&refresh_token={0}&client_id={1}&client_secret={2}", refreshToken, clientId, clientSecret);

            return await RequestToken(tokenRequestBody);
        }

        private async Task<TokenResponse> RequestToken(string tokenRequestBody)
        {
            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            tokenRequest.Method = "POST";
            // tokenRequest.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", clientSecret));
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            //tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();

                    var token = JsonConvert.DeserializeObject<TokenResponse>(responseText);
                    currentToken = token;

                    AuthEvent.Invoke(this, true);

                    SaveToken();

                    return token;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        Console.WriteLine("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            Console.WriteLine(responseText);
                        }
                    }

                }
            }

            AuthEvent.Invoke(this, false);

            return null;
        }

        public void VisitUrl(string url)
        {
            // Opens request in the browser.
            System.Diagnostics.Process.Start("cmd", String.Format("/C start {0}", url));
        }

        private string RedirectUri()
        {
            return string.Format("http://{0}:{1}/", IPAddress.Loopback, port);
        }

        private void SaveToken()
        {
            Properties.Settings.Default.AuthToken = currentToken;

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        private static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}
