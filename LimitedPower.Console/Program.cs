using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using LimitedPower.Core;
using LimitedPower.Core.RatingSources.Deathsie;
using LimitedPower.Core.RatingSources.DraftaholicsAnonymous;
using LimitedPower.Core.RatingSources.DraftSim;
using LimitedPower.Core.RatingSources.InfiniteMythicEdition;
using LimitedPower.Core.RatingSources.MtgaZone;
using LimitedPower.Core.RatingSources.SeventeenLands;
using LimitedPower.ScryfallLib;
using Newtonsoft.Json;

namespace LimitedPower.Console
{
    class Program
    {
        // ReSharper disable once RedundantAssignment
        static void Main(string[] args)
        {
#if DEBUG
            //args = new[] {Commands.LoadImages, "stx,sta",@"C:\dev\out"};
            //args = new[] { Commands.LoadCards, "stx,sta", @"C:\dev\out" };
            args = new[] { Commands.LoadRatings, "stx,sta", @"C:\dev\out", @"‪C:\dev\LimitedPower\.batches\stx.json" };

            //args = new[] {Commands.LoadRatings, "khm",@"C:\dev\out", File.ReadAllText("sample-configuration-khm.json")};
            //args = new[] {Commands.LoadImages, "stx,sta",@"C:\dev\out"};
            //args = new[] { Commands.LoadImages, "khm", @"C:\dev\out", "{\"ScryfallApi\":{\"PrintedSize\":285}}" };
#endif
            var command = args.GetParam(0);
            var sets = args.GetParam(1).Split(',');
            var primarySet = sets.First();
            var root = args.GetParam(2);

            if (command.Equals(Commands.LoadImages))
            {
                var customParameters = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(args.GetParam(3));
                var imgDirectory = Path.Combine(root, primarySet);
                if (!Directory.Exists(imgDirectory)) Directory.CreateDirectory(imgDirectory);
                foreach (var img in new AssetGenerator(new ScryfallApi(customParameters?["ScryfallApi"])).DownloadSetImages(sets))
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

            if (command.Equals(Commands.LoadCards))
            {
                var customParameters = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(args.GetParam(3));
                File.WriteAllText(Path.Combine(root, $"{primarySet}.json"),
                    JsonConvert.SerializeObject(new AssetGenerator(new ScryfallApi(customParameters?["ScryfallApi"])).GenerateSetJson(sets)));
            }

            if (command.Equals(Commands.LoadRatings))
            {
                var parserConfigurations = JsonConvert.DeserializeObject<List<ParserConfiguration>>(File.ReadAllText(args.GetParam(3)));
                if (parserConfigurations == null) return;
                foreach (var p in parserConfigurations)
                {
                    if (p.GeneratorName == nameof(SeventeenLandsGenerator))
                        new SeventeenLandsGenerator(root, primarySet, p.CardNameSubstitutions).Process();

                    if (p.GeneratorName == nameof(DraftaholicsAnonymousGenerator))
                        new DraftaholicsAnonymousGenerator(root, primarySet, p.CardNameSubstitutions).Process();

                    if (p.GeneratorName == nameof(DraftSimGenerator))
                        new DraftSimGenerator(root, primarySet, p.CardNameSubstitutions, p.Args).Process();

                    if (p.GeneratorName == nameof(InfiniteMythicEditionGenerator))
                        new InfiniteMythicEditionGenerator(root, primarySet, p.CardNameSubstitutions, p.Args).Process();

                    if (p.GeneratorName == nameof(DeathsieGenerator))
                        new DeathsieGenerator(root, primarySet, p.CardNameSubstitutions, p.Args).Process();

                    if (p.GeneratorName == nameof(MtgaZoneGenerator))
                        new MtgaZoneGenerator(root, primarySet, p.CardNameSubstitutions, p.Args).Process();
                }
            }
        }
    }
}
