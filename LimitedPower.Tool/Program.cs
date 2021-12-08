using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ImageMagick;
using LimitedPower.Core;
using LimitedPower.Core.RatingSources.Deathsie;
using LimitedPower.Core.RatingSources.DraftaholicsAnonymous;
using LimitedPower.Core.RatingSources.DraftSim;
using LimitedPower.Core.RatingSources.InfiniteMythicEdition;
using LimitedPower.Remote;
using Newtonsoft.Json;
using System.Configuration;

namespace LimitedPower.Tool
{
    class Program
    {
        // fixed values
        private static readonly string RootPath = ConfigurationManager.AppSettings[Const.Settings.OutPath];

        static void Main()
        {
            // default settings
            Console.WriteLine($"default out path: {RootPath}");
            Console.WriteLine();

            // commands
            Console.WriteLine("--- commands ---");
            foreach (var c in typeof(Commands).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                Console.WriteLine(c.GetValue(null)?.ToString());
            }
            Console.WriteLine("----------------");

            // load config
            var lpConfigs = JsonConvert.DeserializeObject<List<LimitedPowerConfig>>(File.ReadAllText(ConfigurationManager.AppSettings[Const.Settings.SetConfig]));

            // process commands
            while (true)
            {
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                var splitInput = input.Split(' ');
                var command = splitInput[0];
                var userArgs = splitInput[1];

                switch (command)
                {
                    case Commands.LoadImages:
                        LoadImages(lpConfigs.GetSet(userArgs));
                        Console.WriteLine($"{Commands.LoadImages} done");
                        break;
                    case Commands.LoadCards:
                        LoadCards(lpConfigs.GetSet(userArgs));
                        Console.WriteLine($"{Commands.LoadCards} done");
                        break;
                    case Commands.LoadRatings:
                        LoadRatings(lpConfigs.GetSet(userArgs));
                        Console.WriteLine($"{Commands.LoadRatings} done");
                        break;
                    case "Exit":
                        return;
                    default:
                        Console.Write("invalid input");
                        break;
                }
            }
        }

        private static void LoadImages(LimitedPowerConfig lpConfig)
        {
            var imgDirectory = Path.Combine(RootPath, lpConfig.Set.PrimarySet());
            if (!Directory.Exists(imgDirectory)) Directory.CreateDirectory(imgDirectory);
            foreach (var img in new AssetGenerator(new ScryfallApi(lpConfig.ScryfallApiArgs)).DownloadSetImagesByName(lpConfig.Set.ToArray()))
            {
                // save
                var file = Path.Combine(imgDirectory, img.Key);
                File.WriteAllBytes(file, img.Value);

                // resize
                using var image = new MagickImage(file);
                image.Resize(new Percentage(50));

                // save again
                image.Write(file);
            }
        }

        private static void LoadCards(LimitedPowerConfig lpConfig) => File.WriteAllText(Path.Combine(RootPath, $"{lpConfig.Set.PrimarySet()}.json"),
            JsonConvert.SerializeObject(new AssetGenerator(new ScryfallApi(lpConfig.ScryfallApiArgs)).GenerateSetJson(lpConfig.Set.ToArray())));

        private static void LoadRatings(LimitedPowerConfig lpConfig)
        {
            foreach (var p in lpConfig.ParserConfigurations)
            {
                switch (p.GeneratorName)
                {
                    case nameof(DraftaholicsAnonymousGenerator):
                        //new DraftaholicsAnonymousGenerator(RootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions).Process();
                        break;
                    case nameof(DraftSimGenerator):
                        //new DraftSimGenerator(RootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions, p.Args).Process();
                        break;
                    case nameof(InfiniteMythicEditionGenerator):
                        //new InfiniteMythicEditionGenerator(RootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions, p.Args).Process();
                        break;
                    case nameof(DeathsieGenerator):
                        new DeathsieGenerator(RootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions, p.Args).Process();
                        break;
                }
            }
        }
    }
}
