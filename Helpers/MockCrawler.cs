using DeadpoolSearch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadpoolSearch.Helpers
{
    public static class MockCrawler
    {
        public static ObservableCollection<MarvelComic> CrawlComics()
        {
            ObservableCollection<MarvelComic> comics = new ObservableCollection<MarvelComic>();
            MarvelComic comic1 = new MarvelComic
            {
                Title = "IronMan",
                Description = "IronMan Never Die",
                DateTime = "2020/6/20",
                RelatedHero = "IronMan"
            };
            MarvelComic comic2 = new MarvelComic
            {
                Title = "BatMan",
                Description = "I'm BatMan",
                DateTime = "2000/6/20",
                RelatedHero = "BatMan"
            };
            MarvelComic comic3 = new MarvelComic
            {
                Title = "AntMan",
                Description = "Become Bigger and Smaller",
                DateTime = "2010/6/10",
                RelatedHero = "AntMan"
            };
            MarvelComic comic4 = new MarvelComic
            {
                Title = "Spider Man: Legendary",
                Description = "Your friendly Neighbor",
                DateTime = "2020/6/20",
                RelatedHero = "Spider Man"
            };
            MarvelComic comic5 = new MarvelComic
            {
                Title = "Unstoppable Hulk",
                Description = "Hulk Smash",
                DateTime = "2020/6/20",
                RelatedHero = "Hulk"
            };
            comics.Add(comic1);
            comics.Add(comic2);
            comics.Add(comic3);
            comics.Add(comic4);
            comics.Add(comic5);
            return comics;
        } 
    }
}
