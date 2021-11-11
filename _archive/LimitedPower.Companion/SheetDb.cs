using System.Collections.Generic;
using System.Linq;
using LimitedPower.Companion.Model;
using LimitedPower.Remote;

namespace LimitedPower.Companion
{
    public class SheetDb
    {
        public List<Set> Sets;
        public List<Draft> Drafts;
        public List<Match> Matches;

        public Draft CurrentDraft => Drafts.Last();

        private string spreadsheetId = "???";

        private GoogleDocsHelper googleDocsHelper;

        static readonly int maxRows = 40000;

        public SheetDb()
        {
            googleDocsHelper = new GoogleDocsHelper(spreadsheetId);
            Init();
        }

        private void Init()
        {
            Sets = SheetTransformer<Set>.GetSheet(googleDocsHelper.GetRows(new[] { $"{nameof(Sets)}!A1:D{maxRows}" }));
            Drafts = SheetTransformer<Draft>.GetSheet(googleDocsHelper.GetRows(new[] { $"{nameof(Drafts)}!A1:I{maxRows}" }));
            Matches = SheetTransformer<Match>.GetSheet(googleDocsHelper.GetRows(new[] { $"{nameof(Matches)}!A1:D{maxRows}" }));

            foreach (var draft in Drafts)
            {
                draft.Matches = new List<Match>(Matches.Where(m => m.Draft == draft.Id));
            }

            foreach (var set in Sets)
            {
                set.Drafts = new List<Draft>(Drafts.Where(d => d.Set == set.Name));
            }
        }

        public void AddRow(object[] obj, string range)
        {
            googleDocsHelper.AppendRow(UpdateObject(obj), range);
            Init();
        }

        public void UpdateRow(object[] obj, string range)
        {
            googleDocsHelper.UpdateRow(UpdateObject(obj), range);
            Init();
        }

        public void ReportDeck(string deckName)
        {
            googleDocsHelper.UpdateRow(UpdateObject(new object[] { deckName }), $"Drafts!f{Drafts.Count + 1}");
            Init();
        }

        private IList<IList<object>> UpdateObject(object[] objects) => new List<IList<object>> { objects.ToList() };
    }
}
