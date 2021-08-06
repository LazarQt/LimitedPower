using System;
using System.Collections.Generic;

namespace LimitedPower.Core.Extensions
{
    public static class StringExtensions
    {
        public static string StripBacksideName(this string term) => term.Contains("//") ? term.Substring(0, term.IndexOf("//", StringComparison.Ordinal) - 1) : term;
    }
}
