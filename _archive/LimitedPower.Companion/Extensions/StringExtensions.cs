using System;
namespace LimitedPower.Companion.Extensions
{
    public static class StringExtensions
    {
        public static int ToNumber(this string s) => Convert.ToInt32(s);
    }
}
