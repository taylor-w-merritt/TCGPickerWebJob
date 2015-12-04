using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TCGPicker.Areas.Yugioh.Models;

namespace TCGPickerWebJob
{
    public class YugiohPricesApiAccess : IDisposable
    {
        protected readonly Uri baseAddress = new Uri("http://yugiohprices.com/api/");
        protected HttpClient httpClient = null;

        public YugiohPricesApiAccess()
        {
            httpClient = new HttpClient { BaseAddress = baseAddress };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<string>> GetAllSetNames()
        {
            using (var response = await httpClient.GetAsync("card_sets"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var deserializedResponse = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<string>>(responseData));
                    return deserializedResponse;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<string>> GetAllCardNamesForSet(string setName)
        {
            var cardNames = new List<string>();

            using (var response = await httpClient.GetAsync("set_data/" + setName))
            {
                if (response.IsSuccessStatusCode)
                {
                    dynamic responseData = JObject.Parse(await response.Content.ReadAsStringAsync());
                    dynamic data = responseData.data;
                    dynamic cards = data.cards;

                    foreach (dynamic card in cards)
                    {
                        cardNames.Add(Convert.ToString(card.name));
                    }
                }
                else
                {
                    return null;
                }
            }

            return cardNames;
        }

        public async Task<YugiohCard> GetCardDetails(string name)
        {
            using (var response = await httpClient.GetAsync("card_data/" + name))
            {
                if (response.IsSuccessStatusCode)
                {
                    dynamic responseData = JObject.Parse(await response.Content.ReadAsStringAsync());
                    dynamic data = responseData.data;

                    var card = new YugiohCard
                    {
                        Attack = data.atk,
                        CardType = data.card_type,
                        Defense = data.def,
                        Family = data.family,
                        Level = data.level,
                        Name = data.name,
                        Property = data.property,
                        Text = data.text,
                        Type = data.type
                    };

                    return card;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Dispose()
        {
            if (httpClient != null)
                httpClient.Dispose();
        }
    }
}
