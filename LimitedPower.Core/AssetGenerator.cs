using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LimitedPower.ScryfallLib;
using LimitedPower.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using TinyPng;

namespace LimitedPower.Core
{
    public class AssetGenerator
    {
        private readonly ScryfallApi _scryfallApi = new();
        private readonly TinyPngClient _tinyPng = new("Mmq8dtn4Q9YCsz5zMHncy202h2Lpnjsh");

        public void GenerateSetJson(string[] setCodes)
        {
            var result = new List<Card>();

            var sourceCards = _scryfallApi.GetSourceCards(setCodes);
            foreach (var sourceCard in sourceCards)
            {
                var card = new Card()
                {
                    CollectorNumber = sourceCard.CollectorNumber,
                    ColorIdentity = sourceCard.ColorIdentity.CreateColorWheel(),
                    Name = sourceCard.Name,
                    Rarity = sourceCard.Rarity.GetRarity(),
                    SetCode = sourceCard.Set,
                    ArenaId = sourceCard.ArenaId,
                    Keywords = sourceCard.Keywords,
                    ProducedMana = sourceCard.ProducedMana?.CreateColorWheel() ?? ColorWheel.None
                };

                if (sourceCard.CardFaces != null)
                {
                    foreach (var cardFace in sourceCard.CardFaces)
                    {
                        card.CardFaces.Add(new CardFace()
                        {
                            Colors = cardFace.Colors.CreateColorWheel(),
                            ManaValue = cardFace.ManaCost.GetManaValue(),
                            Name = cardFace.Name,
                            OracleText = cardFace.OracleText,
                            Power = Convert.ToInt32(cardFace.Power),
                            Toughness = Convert.ToInt32(cardFace.Toughness),
                            TypeLine = cardFace.TypeLine
                        });
                    }
                }
                else
                {
                    card.CardFaces.Add(new CardFace()
                    {
                        Colors = sourceCard.ColorIdentity.CreateColorWheel(),
                        ManaValue = sourceCard.Cmc,
                        Name = sourceCard.Name,
                        OracleText = sourceCard.OracleText,
                        Power = Convert.ToInt32(sourceCard.Power),
                        Toughness = Convert.ToInt32(sourceCard.Toughness),
                        TypeLine = sourceCard.TypeLine
                    });
                }

                result.Add(card);
            }

            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"{setCodes.First()}.json"), JsonConvert.SerializeObject(result));
        }

        public void DownloadSetImages(string[] setCodes)
        {
            var sourceCards = _scryfallApi.GetSourceCards(setCodes);
            foreach (var sourceCard in sourceCards)
            {
                if (sourceCard.CardFaces != null)
                {
                    for (var i = 0; i < sourceCard.CardFaces.Count; i++)
                    {
                        DownloadImage(sourceCard.CardFaces[i].ImageUris.Png, $"{sourceCard.ArenaId}-{i}.png", setCodes.First());
                    }
                }
                else
                {
                    DownloadImage(sourceCard.ImageUris.Png, $"{sourceCard.ArenaId}-0.png", setCodes.First());
                }
            }

        }

        private async void DownloadImage(string imgUrl, string fileName, string setcode)
        {
            var imgDirectory = Path.Combine(Environment.CurrentDirectory, setcode);
            if (!Directory.Exists(imgDirectory)) Directory.CreateDirectory(imgDirectory);
            var imageStream = new RestClient(imgUrl).DownloadData(new RestRequest("#", Method.GET));
            await File.WriteAllBytesAsync(Path.Combine(imgDirectory, fileName), await _tinyPng.Compress(imageStream).Download().GetImageByteData());
        }
    }
}
