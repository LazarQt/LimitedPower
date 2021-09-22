using System;
using System.Collections.Generic;
using LimitedPower.Model;
using LimitedPower.ScryfallLib;
using RestSharp;

namespace LimitedPower.Core
{
    public class AssetGenerator
    {
        private ScryfallApi ScryfallApi { get; set; }

        public AssetGenerator(ScryfallApi scryfallApi)
        {
            ScryfallApi = scryfallApi;
        }

        public List<Card> GenerateSetJson(string[] setCodes)
        {
            var result = new List<Card>();

            var sourceCards = ScryfallApi.GetSourceCards(setCodes);
            foreach (var sourceCard in sourceCards)
            {
                // skip basics
                if (sourceCard.TypeLine.Contains("Basic")) continue;
                var card = new Card
                {
                    CollectorNumber = sourceCard.CollectorNumber,
                    Colors = sourceCard.Colors,
                    ColorIdentity = sourceCard.ColorIdentity,
                    Name = sourceCard.Name,
                    Rarity = sourceCard.Rarity,
                    SetCode = sourceCard.Set,
                    ArenaId = sourceCard.MtgoId,
                    Keywords = sourceCard.Keywords,
                    ProducedMana = sourceCard.ProducedMana ?? new List<string>(),
                    Layout = sourceCard.Layout,
                    ManaCost = sourceCard.ManaCost
                };

                if (sourceCard.CardFaces != null)
                {
                    foreach (var cardFace in sourceCard.CardFaces)
                    {
                        card.CardFaces.Add(new CardFace
                        {
                            ManaCost = cardFace.ManaCost,
                            Name = cardFace.Name,
                            OracleText = cardFace.OracleText,
                            //Power = Convert.ToInt32(cardFace.Power),
                            //Toughness = Convert.ToInt32(cardFace.Toughness),
                            TypeLine = cardFace.TypeLine
                        });
                    }
                }
                else
                {
                    card.CardFaces.Add(new CardFace
                    {
                        ManaCost = sourceCard.ManaCost,
                        Name = sourceCard.Name,
                        OracleText = sourceCard.OracleText,
                        // todo : int -> string
                        Power = sourceCard.Power == null || sourceCard.Power.Contains("*") ? 0 : Convert.ToInt32(sourceCard.Power),
                        Toughness = sourceCard.Toughness == null || sourceCard.Toughness.Contains("*") ? 0 : Convert.ToInt32(sourceCard.Toughness),
                        TypeLine = sourceCard.TypeLine
                    });
                }

                result.Add(card);
            }

            return result;
        }

        public Dictionary<string, byte[]> DownloadSetImages(string[] setCodes)
        {
            var sourceCards = ScryfallApi.GetSourceCards(setCodes);
            var results = new Dictionary<string, byte[]>();
            foreach (var sourceCard in sourceCards)
            {
                if (sourceCard.CardFaces != null)
                {
                    for (var i = 0; i < sourceCard.CardFaces.Count; i++)
                    {
                        if (sourceCard.CardFaces[i].ImageUris != null)
                        {
                            results.Add($"{sourceCard.MtgoId}-{i}.jpg", GetImageBytes(sourceCard.CardFaces[i].ImageUris.Normal));
                        }
                        else
                        {
                            results.Add($"{sourceCard.MtgoId}-{i}.jpg", GetImageBytes(sourceCard.ImageUris.Normal));
                            break;
                        }

                    }
                }
                else
                {
                    results.Add($"{sourceCard.MtgoId}-0.jpg", GetImageBytes(sourceCard.ImageUris.Normal));
                }
            }

            return results;
        }

        private byte[] GetImageBytes(string imgUrl) => new RestClient(imgUrl).DownloadData(new RestRequest("#", Method.GET));
    }
}
