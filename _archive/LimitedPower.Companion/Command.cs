using System;
using LimitedPower.Companion.Extensions;

namespace LimitedPower.Companion
{
    public abstract class Command
    {
        public string Name { get; set; }
        public string Help { get; set; }

        protected Command(string name, string help)
        {
            Name = name;
            Help = help;
        }

        public abstract SheetRow GetUpdateObject(string input, int pos);
        public virtual string[] Parts(string input) => input.Split(' ');
    }

    public class SheetRow
    {
        public object[] Row { get; set; }
        public string Position { get; set; }
    }

    public class StartDraftCommand : Command
    {
        public StartDraftCommand() : base("startdraft", "start draft with parameters set, event & entry fee (e.g. 'startdraft MID BO3 1500 Gems')")
        {
        }

        public override SheetRow GetUpdateObject(string input, int pos)
        {
            var p = Parts(input);
            return new SheetRow()
            {
                Row = new object[] { pos + 1, p[1], p[2], $"{p[3]} {p[4]}", DateTime.Now.ToShortDateString() },
                Position = $"Drafts!A{pos + 2}:E{pos + 2}"
            };
        }
    }

    public class ReportDeckCommand : Command
    {
        public ReportDeckCommand() : base("reportdeck", "report deck for current draft (e.g. 'reportdeck WB')")
        {
        }

        public override SheetRow GetUpdateObject(string input, int pos)
        {
            var p = Parts(input);
            return new SheetRow()
            {
                Row = new object[] { p[1] },
                Position = $"Drafts!f{pos + 1}:f{pos + 1}"
            };
        }
    }

    public class ReportMatchCommand : Command
    {
        public ReportMatchCommand() : base("reportmatch", "report match outcome (e.g. 'reportmatch GR Win')")
        {
        }

        public override SheetRow GetUpdateObject(string input, int pos)
        {
            var p = Parts(input);
            return new SheetRow()
            {
                Row = new object[] { p[1], p[2] },
                Position = $"Matches!A{pos + 2}:D{pos + 2}"
            };
        }
    }

    public class FinishDraftCommand : Command
    {
        public FinishDraftCommand() : base("finishdraft", "finish draft with new status for gold, gems and packs (e.g. 'finishdraft 7500 15000 50')")
        {
        }

        public override SheetRow GetUpdateObject(string input, int pos)
        {
            var p = Parts(input);
            return new SheetRow()
            {
                Row = new object[] { p[1].ToNumber(), p[2].ToNumber(), p[3].ToNumber() },
                Position = $"Drafts!G{pos + 1}:I{pos + 1}"
            };
        }
    }

    public static class Commands
    {
        public static Command StartDraft => new StartDraftCommand();
        public static Command ReportDeck => new ReportDeckCommand();
        public static Command ReportMatch => new ReportMatchCommand();
        public static Command FinishDraft => new FinishDraftCommand();
    }
}
