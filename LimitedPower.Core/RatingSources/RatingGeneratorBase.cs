using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources
{
    public interface IRatingGenerator
    {
        void LoadFile();
        void WriteFile();
        void RateCards();
    }

    public abstract class RatingGeneratorBase : IRatingGenerator
    {
        public List<ViewModel.Card> Cards { get; set; }
        public List<RatingFailure> Fails { get; set; } = new();
        public string BasePath { get; set; }
        public string Set { get; set; }

        public string SetFile => Path.Combine(BasePath, $"{Set}.json");

        protected RatingGeneratorBase(string basePath, string set)
        {
            BasePath = basePath;
            Set = set;
        }

        public void LoadFile()
        {
            Cards = JsonConvert.DeserializeObject<List<ViewModel.Card>>(File.ReadAllText(SetFile));
            if (Cards == null)
            {
                throw new Exception($"could not load file {SetFile}");
            }
        }

        public void WriteFile()
        {
            File.WriteAllText(SetFile, JsonConvert.SerializeObject(Cards));
        }

        public abstract void RateCards();
    }
}
