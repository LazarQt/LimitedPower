using ImageMagick;
using LimitedPower.Core;
using LimitedPower.Core.RatingSources;
using LimitedPower.Remote;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using LimitedPower.Core.RatingSources.Deathsie;
using LimitedPower.Core.RatingSources.DraftaholicsAnonymous;
using LimitedPower.Core.RatingSources.DraftSim;
using LimitedPower.Core.RatingSources.InfiniteMythicEdition;

namespace LimitedPower.Tool
{

    public class LpTool
    {
        // fixed values
        private static string _rootPath = Const.Settings.SavePath;

        public void Run(string input)
        {
            // default settings
            if (string.IsNullOrEmpty(_rootPath)) _rootPath = Directory.GetCurrentDirectory();
            Console.WriteLine($"default out path: {_rootPath}");
            Console.WriteLine();

            // load config
            var lpConfigs = JsonConvert.DeserializeObject<List<LimitedPowerConfig>>(File.ReadAllText(Const.Settings.SetConfiguration));

            // process commands
            if (string.IsNullOrEmpty(input)) return;

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
        private static void LoadImages(LimitedPowerConfig lpConfig)
        {
            var imgDirectory = Path.Combine(_rootPath, lpConfig.Set.PrimarySet());
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

        private static void LoadCards(LimitedPowerConfig lpConfig) => File.WriteAllText(Path.Combine(_rootPath, $"{lpConfig.Set.PrimarySet()}.json"),
            JsonConvert.SerializeObject(new AssetGenerator(new ScryfallApi(lpConfig.ScryfallApiArgs)).GenerateSetJson(lpConfig.Set.ToArray())));

        private static void LoadRatings(LimitedPowerConfig lpConfig)
        {
            foreach (var p in lpConfig.ParserConfigurations)
            {
                switch (p.GeneratorName)
                {
                    //case nameof(DraftaholicsAnonymousGenerator):
                    //    new DraftaholicsAnonymousGenerator(_rootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions).Process();
                    //    break;
                    //case nameof(DraftSimGenerator):
                    //    new DraftSimGenerator(_rootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions, p.Args).Process();
                    //    break;
                    //case nameof(InfiniteMythicEditionGenerator):
                    //    new InfiniteMythicEditionGenerator(_rootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions, p.Args).Process();
                    //    break;
                    //case nameof(DeathsieGenerator):
                    //    new DeathsieGenerator(_rootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions, p.Args).Process();
                    //    break;
                    case nameof(MtgdsGenerator):
                        new MtgdsGenerator(_rootPath, lpConfig.Set.PrimarySet(), p.CardNameSubstitutions, p.Args).Process();
                        break;
                }
            }
        }
    }
}
