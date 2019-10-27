using DestinyPCLoadoutManager.API.Models;
using Fastenshtein;
using System.Text.RegularExpressions;

namespace DestinyPCLoadoutManager.Logic.Search
{
    public class SearchScorer
    {
        private static string namePattern = "^.*{0}.*$";

        private Levenshtein levenshtein;
        private Regex nameRegex;

        public SearchScorer(string searchTerm)
        {
            levenshtein = new Levenshtein(searchTerm);
            nameRegex = new Regex(string.Format(namePattern, searchTerm), RegexOptions.IgnoreCase);
        }

        public float Score(Item item)
        {
            float levenshteinScore = levenshtein.DistanceFrom(item.Name);
            var name = nameRegex.Match(item.Name);
            float nameScore = name.Success ? 1 : 0;

            var score = nameScore + 1 / levenshteinScore;

            return score;
        }
    }
}
