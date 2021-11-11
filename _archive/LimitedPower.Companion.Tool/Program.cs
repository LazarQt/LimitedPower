using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BetterConsoleTables;
using LimitedPower.Companion.Extensions;
using LimitedPower.Companion.Model;
using LimitedPower.Remote;
using Newtonsoft.Json;

namespace LimitedPower.Companion.Tool
{
    public class DraftPick
    {
        public object PlayerId { get; set; }
        public object ClientPlatform { get; set; }
        public string DraftId { get; set; }
        public string EventId { get; set; }
        public int SeatNumber { get; set; }
        public int PackNumber { get; set; }
        public int PickNumber { get; set; }
        public int PickGrpId { get; set; }
        public List<int> CardsInPack { get; set; }
        public bool AutoPick { get; set; }
        public double TimeRemainingOnPick { get; set; }
        public int EventType { get; set; }
        public DateTime EventTime { get; set; }
    }

    public class CardInfo
    {
        public string Card { get; set; }
        public decimal TakenAt { get; set; }
        public decimal WinRate { get; set; }

        public decimal Iwd { get; set; }
        public string BetterWinIn { get; set; }

        public string BetterIwdIn { get; set; }

    }

    class Program
    {
        private static PlayerCardData _playerCardData;
        private static string LogLoc = @"C:\Users\Jerry\AppData\LocalLow\Wizards Of The Coast\MTGA";
        private static string TryRead(string path)
        {
            try
            {
                var lines = readAllLines(path);

                var line = lines.Last();

                //var line = File.ReadAllLines(path).Last();
                //Console.WriteLine("line is " + line);
                return line;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        private static List<string> readAllLines(String i_FileNameAndPath)
        {
            List<string> o_Lines = new List<string>();
            using (FileStream fileStream = File.Open(i_FileNameAndPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    var x = streamReader.ReadToEnd();
                    o_Lines = x.Split("\r\n").ToList();
                }
            }

            return o_Lines;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            // if nothing changed, do nothing
            if (e.ChangeType != WatcherChangeTypes.Changed) return;

            // only look for lines with CardsInPack string

            ShowRatings(e.FullPath);

        }

        private static void ShowRatings(string p)
        {
            var lines = new List<string>();

            File.Copy(p, "CopiedLog.log");

            while (true)
            {
                try
                {
                    lines = readAllLines(p);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("retry!");
                    Thread.Sleep(200);
                }
            }



            if (lines == null) return;
            var line = "";
            var i = lines.Count - 1;

            while (i >= 0)
            {
                if (lines[i].Contains("CardsInPack"))
                {
                    line = lines[i];
                    break;
                }
                else
                {
                    i--;
                }
            }

            //var reading = true;
            //while (reading)
            //{
            //    Console.WriteLine("tryint to read!");
            //    Thread.Sleep(100);
            //    line = TryRead(e.FullPath);
            //    reading = line == string.Empty;
            //}

            //try
            //{
            //    line = File.ReadAllLines(e.FullPath).Last();
            //}
            //catch (Exception ex)
            //{
            //    return;
            //}

            //Console.WriteLine("line is " + line);

            if (!line.Contains("CardsInPack")) return;

            // clear console for new output
            Console.Clear();

            // check if player data has been loaded properly
            if (_playerCardData == null) throw new Exception("player data not loaded!");

            // retrieve json from line in log file
            var cardInfos = new List<CardInfo>();
            var start = line.IndexOf("Payload", StringComparison.Ordinal);
            var parsedLine = line.Substring(start + "Payload".Length + 5);
            parsedLine = parsedLine.Substring(0, parsedLine.Length - 5);
            parsedLine = parsedLine.Replace("\\", "");
            var pack = JsonConvert.DeserializeObject<DraftPick>(parsedLine);
            if (pack == null) throw new Exception("could not get pack data from log file");

            // set colors to retrieve data from
            var colors = new[] { "WU", "WB", "WR", "WG", "UB", "UR", "UG", "BR", "BG", "RG" };

            foreach (var c in pack.CardsInPack)
            {
                var dbCard = _playerCardData.ColorData.FirstOrDefault(u => u.Color == "")
                    ?.CardData
                    .FirstOrDefault(a => a.ArenaId == c);

                if (dbCard == null) throw new Exception($"can't find card {c} in local db!");

                var ci = new CardInfo()
                {
                    Card = dbCard.Name,
                    BetterIwdIn = "",
                    BetterWinIn = ""
                };

                if (dbCard.WinRate != null) ci.WinRate = Math.Round((decimal)dbCard.WinRate * 100, 0, MidpointRounding.AwayFromZero);
                if (dbCard.AvgPick != null) ci.TakenAt = Math.Round((decimal)dbCard.AvgPick, 1, MidpointRounding.AwayFromZero);
                if (dbCard.DrawnImprovementWinRate != null)
                    ci.Iwd = Math.Round((decimal)dbCard.DrawnImprovementWinRate * 100, 2, MidpointRounding.AwayFromZero);


                foreach (var color in colors)
                {
                    var atCard = _playerCardData.ColorData.FirstOrDefault(u => u.Color == color)
                        ?.CardData
                        .FirstOrDefault(a => a.ArenaId == c);
                    if (atCard == null) continue;

                    if (atCard.Color != color && !color.Contains(atCard.Color)) continue;

                    if (atCard.WinRate != null)
                    {
                        var win = Math.Round((decimal)atCard.WinRate * 100, 0, MidpointRounding.AwayFromZero);
                        if (win > ci.WinRate + 1) ci.BetterWinIn += $"{color}({win}),";
                    }

                    if (atCard.DrawnImprovementWinRate != null)
                    {
                        var iwd = Math.Round((decimal)atCard.DrawnImprovementWinRate * 100, 2,
                            MidpointRounding.AwayFromZero);
                        if (iwd > ci.Iwd + 1) ci.BetterIwdIn += $"{color}({iwd}),";
                    }

                    //Console.WriteLine($"rating for card {x.Name} in colors {color}: {x.WinRate}");
                }

                if (ci.BetterIwdIn.Length > 0) ci.BetterIwdIn = ci.BetterIwdIn.Substring(0, ci.BetterIwdIn.Length - 1);
                if (ci.BetterWinIn.Length > 0) ci.BetterWinIn = ci.BetterWinIn.Substring(0, ci.BetterWinIn.Length - 1);
                cardInfos.Add(ci);
            }

            cardInfos = cardInfos.OrderBy(c => c.TakenAt).ToList();
            var t = new Table(TableConfiguration.Default()).From(cardInfos);
            Console.Write(t.ToString());
            //File.AppendAllText(@"C:\Users\Jerry\Downloads\draft2.log", parsedLine + Environment.NewLine);
        }

        static void Main()
        {
            Console.WriteLine("LimitedPower Companion");

            var landsDataFile = "17lands.json";
            if (!File.Exists(landsDataFile))
            {
                File.WriteAllText(landsDataFile, JsonConvert.SerializeObject(new PlayerCardData()
                {
                    ColorData = new List<ColorData>(),
                    LastUpdated = DateTime.MinValue
                }));
            }

            _playerCardData = JsonConvert.DeserializeObject<PlayerCardData>(File.ReadAllText(landsDataFile));
            if (_playerCardData.LastUpdated <= DateTime.Now.AddDays(-1))
            {
                _playerCardData = new PlayerCardData()
                {
                    ColorData = new List<ColorData>(),
                    LastUpdated = DateTime.Now
                };
                var colors = new[] { "", "WU", "WB", "WR", "WG", "UB", "UR", "UG", "BR", "BG", "RG" };
                var sfCards = new ScryfallApi().GetSourceCards(new[] { "MID" });
                foreach (var color in colors)
                {

                    // get 17lands stuff
                    var today = DateTime.Now;
                    var last = DateTime.Now.AddDays(-5);
                    var url =
                        $"https://www.17lands.com/card_ratings/data?expansion=MID&format=PremierDraft&start_date={last.Year}-{last.Month:00}-{last.Day:00}&end_date={today.Year}-{today.Month:00}-{today.Day:00}";
                    if (!string.IsNullOrEmpty(color)) url += "&colors=" + color;
                    var doc = new System.Net.WebClient().DownloadString(Uri.EscapeUriString(url));
                    var cardRatings = JsonConvert.DeserializeObject<List<SeventeenLandsExtended>>(doc);

                    foreach (var cr in cardRatings)
                    {
                        if (cr.Name == "Candlelit Cavalry")
                        {
                            cr.ArenaId = 78532;
                            continue;
                        }
                        var sfCard = sfCards.FirstOrDefault(x => x.Name.ToLower().Contains(cr.Name.ToLower()));
                        if (sfCard == null) throw new Exception("card not found");
                        cr.ArenaId = sfCard.ArenaId;
                    }

                    _playerCardData.ColorData.Add(new ColorData()
                    {
                        CardData = cardRatings,
                        Color = color
                    });
                }

                File.WriteAllText(landsDataFile, JsonConvert.SerializeObject(_playerCardData));
            }

            // Console.WriteLine("log file location?");
            //Console.WriteLine(@"C:\Users\Jerry\AppData\LocalLow\Wizards Of The Coast\MTGA");
            //var loc = Console.ReadLine();

            using var watcher = new FileSystemWatcher(LogLoc)
            {
                Filter = "Player.log",
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite
            };


            watcher.Changed += OnChanged;
            Console.WriteLine("read");
            /*
            foreach (var line in lines)
            {
                if (!line.Contains("CardsInPack")) continue;
                if(line.Contains("{"))
                {
                    Console.WriteLine(line);
                    var start = line.IndexOf("Payload");

                    var parsedLine = line.Substring(start+ "Payload".Length + 5);
                    parsedLine = parsedLine.Substring(0, parsedLine.Length - 5);
                    parsedLine = parsedLine.Replace("\\","");
                    var x = JsonConvert.DeserializeObject<DraftPick>(parsedLine);
                    File.AppendAllText(@"C:\Users\Jerry\Downloads\draft2.log", parsedLine + Environment.NewLine);
                }
                

            }
            */

            var sheetDb = new SheetDb();

            var d = sheetDb.Drafts.Last();
            if (d.NewPackStatus == null)
            {
                Console.WriteLine($"Current {d.Set} {d.Event} Draft going on with deck {d.Deck}");
            }

            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == null) continue;
                if (cmd == "show")
                {
                    ShowRatings(LogLoc);
                }  else if (cmd.IsCommand(Commands.StartDraft))
                {
                    var update = Commands.StartDraft.GetUpdateObject(cmd, sheetDb.Drafts.Count);
                    sheetDb.AddRow(update.Row, update.Position);
                    Console.WriteLine("Started draft");
                }
                else if (cmd.IsCommand(Commands.ReportDeck))
                {
                    var update = Commands.ReportDeck.GetUpdateObject(cmd, sheetDb.Drafts.Count);
                    sheetDb.UpdateRow(update.Row, update.Position);
                    Console.WriteLine("Added deck");
                }
                else if (cmd.IsCommand(Commands.ReportMatch))
                {
                    var update = Commands.ReportMatch.GetUpdateObject(cmd, sheetDb.Matches.Count);
                    var row = update.Row.ToList();
                    row.Insert(0, sheetDb.CurrentDraft.Matches.Count + 1);
                    row.Insert(0, sheetDb.CurrentDraft.Id);
                    sheetDb.AddRow(row.ToArray(), update.Position);
                    Console.WriteLine("Reported match outcome");
                }
                else if (cmd.IsCommand(Commands.FinishDraft))
                {
                    var update = Commands.FinishDraft.GetUpdateObject(cmd, sheetDb.Drafts.Count);
                    sheetDb.UpdateRow(update.Row, update.Position);
                    Console.WriteLine("Finished draft");
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        //private static void ShowStuff()
        //{
        //    var sheetDb = new SheetDb();

        //    Console.Write(new Table(TableConfiguration.MySql()).From(sheetDb.Sets).ToString());
        //    Console.Write(new Table(TableConfiguration.MySql()).From(sheetDb.Drafts).ToString());
        //    Console.Write(new Table(TableConfiguration.MySql()).From(sheetDb.Matches).ToString());
        //}

        //private static Table GenerateTable<T>(List<T> data)
        //{
        //    var table = new Table(TableConfiguration.MySql());
        //    table.From(data);

        //    var newTable = new Table();

        //    foreach (var col in table.Columns)
        //    {
        //        var t = typeof(T);
        //        var prop = t.GetProperty(col.Title);
        //        if (prop == null) continue;
        //        if (!Attribute.IsDefined(prop, typeof(GeneratedAttribute)))
        //        {
        //            newTable.AddColumn(col.Title);
        //        }
        //    }

        //    newTable.From(data);

        //    return newTable;
        //}
    }
}
