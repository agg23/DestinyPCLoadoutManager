using Destiny2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API
{
    class ManifestManager
    {
        private IManifestSettings settings;
        private IManifestDownloader downloader;

        public void SetupServices()
        {
            if (settings != null && downloader != null)
            {
                return;
            }

            settings = App.provider.GetService(typeof(IManifestSettings)) as IManifestSettings;
            downloader = App.provider.GetService(typeof(IManifestDownloader)) as IManifestDownloader;
        }

        public async Task DownloadManifest()
        {
            // TODO: Add version number
            // Returns new version number
            await downloader.DownloadManifest(settings.DbPath.FullName, null);
        }
    }
}
