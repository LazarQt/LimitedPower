namespace LimitedPower.Companion.Extensions
{
    public static class CommandExtensions
    {
        public static bool IsCommand(this string commandString, Command command) =>
            commandString.StartsWith(command.Name);
    }
}
