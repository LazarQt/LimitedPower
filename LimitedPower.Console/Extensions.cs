namespace LimitedPower.Console
{
    public static class Extensions
    {
        public static string GetParam(this string[] args, int index)
        {
            if (index >= args.Length) return string.Empty;
            return args[index];
        }
    }
}
