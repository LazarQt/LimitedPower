using System;
using System.Collections.Generic;
using LimitedPower.Remote;
using LimitedPower.Remote.Model;
using RestSharp;

namespace LimitedPower.Core
{
    public class AssetGenerator
    {
        private ScryfallApi ScryfallApi { get; }

        public AssetGenerator(ScryfallApi scryfallApi)
        {
            ScryfallApi = scryfallApi;
        }

        public List<ScryfallCard> GenerateSetJson(string[] setCodes) => ScryfallApi.GetSourceCards(setCodes);

        public Dictionary<string, byte[]> DownloadSetImagesByName(string[] setCodes) =>
            DownloadImagesByName(ScryfallApi.GetSourceCards(setCodes));

        public Dictionary<string, byte[]> DownloadImagesByName(List<ScryfallCard> sourceCards)
        {
            var results = new Dictionary<string, byte[]>();
            foreach (var sourceCard in sourceCards)
            {
#if DEBUG
                Console.WriteLine($"Getting image for {sourceCard.Name}");
#endif
                if (sourceCard.CardFaces != null)
                {
                    foreach (var t in sourceCard.CardFaces)
                    {
                        if (t.ImageUris != null)
                        {
                            results.Add($"{t.Name}.jpg", GetImageBytes(t.ImageUris.Normal));
                        }
                        else
                        {
                            results.Add($"{t.Name}.jpg", GetImageBytes(sourceCard.ImageUris.Normal));
                            break;
                        }
                    }
                }
                else
                {
                    results.Add($"{sourceCard.Name}.jpg", GetImageBytes(sourceCard.ImageUris.Normal));
                }
            }

            return results;
        }

        private byte[] GetImageBytes(string imgUrl)
        {
#if DEBUG
            Console.WriteLine($"Downloading {imgUrl}");
#endif
            return new RestClient(imgUrl).DownloadData(new RestRequest("#", Method.GET));
        }

    }
}
