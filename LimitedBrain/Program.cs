using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace LimitedBrain
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");

            var csv = File.ReadAllLines("snc.csv")
                .Skip(1)
                .Select(v => v)
                .ToList();
            csv.RemoveAll(c => c == string.Empty);

            var cardProjections = new List<CardProjection>();
            foreach (var csvLine in csv)
            {
                var csvSplit = csvLine.Split(";");
                cardProjections.Add(new CardProjection()
                {
                    Name = csvSplit[0],
                    Power = Convert.ToDouble(csvSplit[4])
                });
            }
        }
    }
}
