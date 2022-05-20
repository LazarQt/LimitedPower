using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.Extensions
{
    public static class LevenshteinDistance
    {
        /// <summary>
        /// Get distance for a term in input based on Levenshtein Distance algorithm
        /// </summary>
        /// <param name="input">Input to be searched in</param>
        /// <param name="term">Term to search for</param>
        /// <returns>Count how far term is away from being identical to input</returns>
        public static int DistanceIndex(this IEnumerable<string> input, string term)
        {
            var distances = input.ToList().Select(s => s.ComputeDistance(term)).ToList();
            return distances.IndexOf(distances.Min());
        }

        private static int ComputeDistance(this string s, string t)
        {
            // source: https://www.dotnetperls.com/levenshtein

            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // Verify arguments.
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Initialize arrays.
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Begin looping.
            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    // Compute cost.
                    var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Return cost.
            return d[n, m];
        }
    }
}
