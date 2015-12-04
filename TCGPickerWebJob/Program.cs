using System.Collections.Generic;
using TCGPicker.Areas.Yugioh.Models;

namespace TCGPickerWebJob
{
    class Program
    {
        static void Main(string[] args)
        {   
            //using (var context = new TCGPickerContext())
            //{
            //    var cards = context.YugiohCards.ToList();
            //}

            SortedSet<string> cardNames = new SortedSet<string>();

            YugiohPricesApiAccess yugioh = new YugiohPricesApiAccess();
            var sets = yugioh.GetAllSetNames().Result;

            foreach (var s in sets)
            {
                var cards = yugioh.GetAllCardNamesForSet(s).Result;

                foreach (var c in cards)
                {
                    cardNames.Add(c);
                }
            }
            
            //var card = yugioh.GetCardDetails("B. Skull Dragon").Result;
        }
    }
}
