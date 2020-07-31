using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadpoolSearch.Helpers
{
    public static class ConstantsHelper
    {
        public static string Title = nameof(Title);
        public static string Description = nameof(Description);
        public static string RelatedHero = nameof(RelatedHero);
        public static string DateTime = nameof(DateTime);
        public static string HeroBg = nameof(HeroBg);
        public static string Detail = nameof(Detail);

        public static string fandomUrl = "https://marvel.fandom.com";

        public static string marvelHqUrl = "https://www.marvelhq.com/comics";

        public static KeyValuePair<string, int>[] comixologyUrl = new KeyValuePair<string, int>[]
        {
            new KeyValuePair<string, int>("https://www.comixology.com/search/items?search=Marvel&subType=SINGLE_ISSUES&itemsSearchDetailList_pg=",44),
            new KeyValuePair<string, int>("https://www.comixology.com/search/items?search=Marvel&subType=COLLECTIONS&itemsSearchDetailList_pg=",9),
        };

        public static string marvelHerosPoolUrl = "https://www.marvel.com/comics/characters";


    }
}
