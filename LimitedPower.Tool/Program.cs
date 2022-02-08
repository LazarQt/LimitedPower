using System;
using System.Reflection;

namespace LimitedPower.Tool
{
    class Program
    {
        static void Main()
        {
            // commands
            Console.WriteLine("--- commands ---");
            foreach (var c in typeof(Commands).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                Console.WriteLine(c.GetValue(null)?.ToString());
            }
            Console.WriteLine("----------------");

            while (true)
            {
                var input = Console.ReadLine();
                new LpTool().Run(input);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
