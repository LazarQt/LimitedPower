using System.Collections.Generic;
using LimitedPower.Core;
using LimitedPower.Core.RatingSources;
using LimitedPower.Core.RatingSources.Deathsie;
using LimitedPower.Core.RatingSources.DraftaholicsAnonymous;
using LimitedPower.Core.RatingSources.DraftSim;
using LimitedPower.Core.RatingSources.Drifter;
using LimitedPower.Core.RatingSources.InfiniteMythicEdition;
using static System.Console;

namespace LimitedPower.Console
{
    class Program
    {
        public static class Commands
        {
            public static string LoadCards = "LoadCards";
            public static string LoadImages = "LoadImages";
            public static string LoadRatings = "LoadRatings";
        }

        static void Main()
        {
            WriteLine("*** Commands:                         ***");
            WriteLine("*** --------------------------------- ***");
            WriteLine("*** Example: LoadCards stx            ***");
            WriteLine("*** Example: LoadCards stx,sta        ***");
            WriteLine("*** --------------------------------- ***");
            WriteLine("*** Example: LoadImages stx           ***");
            WriteLine("*** Example: LoadImages stx,sta       ***");
            WriteLine("*** --------------------------------- ***");
            WriteLine("*** Example: LoadRatings stx          ***");
            WriteLine("*** --------------------------------- ***");

            while (true)
            {
                // Command Input handling
                WriteLine("Please enter command...");
                var input = ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    WriteLine("invalid input");
                    continue;
                }

                var splitInput = input.Split(' ');
                var command = splitInput[0];

                if (command.Equals(Commands.LoadCards) || command.Equals("1"))
                {
                    new AssetGenerator().GenerateSetJson(splitInput[1].Split(','));
                    WriteLine("Generated json");
                }

                if (command.Equals(Commands.LoadImages) || command.Equals("2"))
                {
                    new AssetGenerator().DownloadSetImages(splitInput[1].Split(','));
                    WriteLine("Downloaded images");
                }

                if (command.Equals(Commands.LoadRatings) || command.Equals("3"))
                {
                    var set = splitInput[1];
                    var root = @"C:\dev\";
                    var ratingGenerators = new List<IRatingGenerator>
                    {
                        // ReSharper disable StringLiteralTypo
                        new DraftaholicsAnonymousGenerator(root, set),
                        new DraftSimGenerator(root, set, new[] {"STX", "STX_mys", "STX_lesson"}),
                        new InfiniteMythicEditionGenerator(root, set, "1KGjmlYSENE5M3lNCKwvtmMnDw_Bq9d9Hw5nk1_12o0w"),
                        new DeathsieGenerator(root, set, "1KGjmlYSENE5M3lNCKwvtmMnDw_Bq9d9Hw5nk1_12o0w"),
                        new DrifterGenerator(root, set, "1KGjmlYSENE5M3lNCKwvtmMnDw_Bq9d9Hw5nk1_12o0w"),
                        // ReSharper restore StringLiteralTypo
                    };
                    ratingGenerators.ForEach(rg => rg.RateCards());

                    WriteLine("rated cards");
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
