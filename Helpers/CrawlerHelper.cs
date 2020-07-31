using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.IO;
using DeadpoolSearch.Models;
using Windows.Storage;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace DeadpoolSearch.Helpers
{
    public enum CrwalerOptions
    {
        Fandom,
        MarvelHQ,
        Comixology
    }
    public static class CrawlerHelper
    {
        public delegate void OnProgressChangedEvent(int progress, CrwalerOptions options);
        public static event OnProgressChangedEvent OnProgressChanged;

        private static int startYear = 1939;
        private static int endYear = 2020;
        public static async Task GetFromFandom()
        {
            int year = startYear;
            HttpClient httpClient = new HttpClient();

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("MarvelComics", CreationCollisionOption.ReplaceExisting);
            //C:\Users\Administrator\AppData\Local\Packages\951e17e0-73ce-478f-98e0-195a12edd6b2_75cr2b68sm664\LocalState
            List<string> Buffer = new List<string>();
            while (year <= endYear)
            {
                string categoryUrl = ConstantsHelper.fandomUrl + "/wiki/Category:" + year;
                try
                {
                    string html = await httpClient.GetStringAsync(categoryUrl);
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    //System.Diagnostics.Debug.WriteLine(html);
                    var divs = htmlDocument.DocumentNode.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "").Contains("category-page__trending-page")).ToList();
                    foreach (var div in divs)
                    {
                        var img = div.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
                        var title = div.Descendants("figcaption").FirstOrDefault().InnerText;
                        var detailUrl = div.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
                        detailUrl = ConstantsHelper.fandomUrl + detailUrl;
                        try
                        {
                            string detailHtml = await httpClient.GetStringAsync(detailUrl);
                            HtmlDocument detailHtmlDocument = new HtmlDocument();
                            detailHtmlDocument.LoadHtml(detailHtml);

                            //Get Title And Fearture Heros
                            string heroList = "";
                            var h2s = detailHtmlDocument.DocumentNode.Descendants("h2")
                                 .Where(node => node.GetAttributeValue("id", "").Contains("AppearingHeader")).ToList();
                            foreach (var h2 in h2s)
                            {
                                var ul = h2.NextSibling;
                                while (ul.Name != "ul")
                                {
                                    ul = ul.NextSibling;
                                }
                                var lis = ul.Descendants("li");
                                foreach (var li in lis)
                                {
                                    var hero = li.Descendants("a")?.FirstOrDefault()?.InnerText;
                                    if (hero != null && hero.Trim() != "")
                                    {
                                        heroList += hero + ". ";
                                    }

                                }
                            }

                            //Get Details
                            var article = detailHtmlDocument.DocumentNode.Descendants("div")
                                .Where(node => node.GetAttributeValue("id", "").Equals("WikiaArticle")).FirstOrDefault();
                            string detail = article.InnerHtml;

                            //Get Description
                            var span = detailHtmlDocument.DocumentNode.Descendants("span")
                                .Where(node => node.GetAttributeValue("id", "").Equals("Notes")).FirstOrDefault();
                            var wrpper = span?.ParentNode?.NextSibling?.NextSibling;
                            var descriptions = "";
                            if (wrpper != null)
                            {
                                if (wrpper.Name == "ul")
                                {
                                    var wrapperLis = wrpper.Descendants("li").ToList();
                                    foreach (var li in wrapperLis)
                                    {
                                        var note = li.InnerText;
                                        descriptions += note;
                                    }
                                }

                            }

                            if (heroList == "")
                            {
                                heroList = "Unknown";
                            }
                            if (descriptions == "")
                            {
                                descriptions = "Unknown";
                            }
                            MarvelComic marvelComic = new MarvelComic
                            {
                                DateTime = year.ToString(),
                                Title = title,
                                RelatedHero = heroList,
                                Description = descriptions,
                                HeroBg = img,
                                Detail = detail
                            };
                            if (year % 20 != 0)
                            {
                                Buffer.Add(JsonConvert.SerializeObject(marvelComic));
                            }
                            else
                            {
                                await FileIO.AppendLinesAsync(sampleFile, Buffer);
                                Buffer.Clear();
                            }

                            System.Diagnostics.Debug.WriteLine("-----------------");
                            System.Diagnostics.Debug.WriteLine(img);
                            System.Diagnostics.Debug.WriteLine(title);
                            System.Diagnostics.Debug.WriteLine(descriptions);
                            System.Diagnostics.Debug.WriteLine(heroList);
                            System.Diagnostics.Debug.WriteLine(year);
                            System.Diagnostics.Debug.WriteLine("-----------------");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                        }

                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }

                year++;
                OnProgressChanged?.Invoke(year, CrwalerOptions.Fandom);
                await Task.Delay(10);
            }
        }

        public static async Task GetFromMarvelHq()
        {
            HttpClient httpClient = new HttpClient();

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("MarvelComics", CreationCollisionOption.OpenIfExists);
            
            List<string> Buffer = new List<string>();
            try
            {
                string html = await httpClient.GetStringAsync(ConstantsHelper.marvelHqUrl);
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var lis = htmlDocument.DocumentNode.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "").Equals("col")).ToList();
                int currentIndex = 0;
                foreach (var li in lis)
                {
                    var img = li.Descendants("img")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("_currentItem")).FirstOrDefault().GetAttributeValue("src","");
                    var title = li.Descendants("a")
                        .FirstOrDefault().GetAttributeValue("data-title", "");
                    var descriptions = "";
                    var detail = "";
                    var heroList = "";
                    var year = "Unkown";
                    var detailUrl = li.Descendants("a")
                        .FirstOrDefault().GetAttributeValue("href", "");
                    try
                    {
                        string detailHtml = await httpClient.GetStringAsync(detailUrl);
                        HtmlDocument detailHtmlDocument = new HtmlDocument();
                        detailHtmlDocument.LoadHtml(detailHtml);

                        descriptions = detailHtmlDocument.DocumentNode.Descendants("p")
                            .Where(node => node.GetAttributeValue("id", "").Equals("comicDetailDesc"))
                            .FirstOrDefault().InnerText;

                        detail = detailHtmlDocument.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("itemprop","").Equals("comic"))
                            .FirstOrDefault().InnerHtml;

                        var heros = detailHtmlDocument.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("data-modulename", "").Equals("Related Characters"))
                            ?.FirstOrDefault()?.Descendants("li")
                            ?.Where(node => node.GetAttributeValue("class","").Equals("col"))?.ToList();
                        
                        if(heros != null && heros.Count != 0)
                        {
                            foreach (var hero in heros)
                            {
                                heroList += hero.Descendants("a").FirstOrDefault().GetAttributeValue("data-title", "") + ".";
                            }
                        }
                        
                        
                        if(heroList == "")
                        {
                            heroList = "Unknown";
                        }

                        if(img == "")
                        {
                            img = "../Pictures/DefaultDetailBg.jpg";
                        }
                        System.Diagnostics.Debug.WriteLine("-----------------");
                        System.Diagnostics.Debug.WriteLine(img);
                        System.Diagnostics.Debug.WriteLine(title);
                        System.Diagnostics.Debug.WriteLine(descriptions);
                        System.Diagnostics.Debug.WriteLine(heroList);
                        System.Diagnostics.Debug.WriteLine(year);
                        System.Diagnostics.Debug.WriteLine("-----------------");

                        MarvelComic marvelComic = new MarvelComic
                        {
                            DateTime = year.ToString(),
                            Title = title,
                            RelatedHero = heroList,
                            Description = descriptions,
                            HeroBg = img,
                            Detail = detail
                        };

                        if ((currentIndex + 30 + 1) % 30 != 0)
                        {
                            Buffer.Add(JsonConvert.SerializeObject(marvelComic));
                        }
                        else
                        {
                            await FileIO.AppendLinesAsync(sampleFile, Buffer);
                            Buffer.Clear();
                        }
                        OnProgressChanged?.Invoke(currentIndex, CrwalerOptions.MarvelHQ);
                        currentIndex++;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }
                if (Buffer.Count != 0)
                {
                    await FileIO.AppendLinesAsync(sampleFile, Buffer);
                    Buffer.Clear();
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        public static List<string> HeroPool = new List<string>();

        public static async Task GetHeroPool()
        {
            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(ConstantsHelper.marvelHerosPoolUrl);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var heroes = htmlDocument.DocumentNode.Descendants("li")
                .ToList();
            foreach (var hero in heroes)
            {
                var heroName = hero.Descendants("a").FirstOrDefault().InnerText;
                HeroPool.Add(heroName);
            }
        }
        private static int totalPage = 1;
        public static async Task GetFromComixology()
        {
            await GetHeroPool();
            //44+9
            foreach (var url in ConstantsHelper.comixologyUrl)
            {
                await GetFromComixologyPart(url);
            }
        }
        

        public static async Task GetFromComixologyPart(KeyValuePair<string,int> url)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("MarvelComics", CreationCollisionOption.OpenIfExists);

            List<string> Buffer = new List<string>();
            int currentPage = 1;
            string currentUrl = url.Key + currentPage;
            while (currentPage <= url.Value)
            {
                try
                {
                    HttpClient httpClient = new HttpClient();
                    var html = await httpClient.GetStringAsync(currentUrl);
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    var items = htmlDocument.DocumentNode.Descendants("li")
                        .Where(node => node.GetAttributeValue("class", "").Equals("content-item"))
                        .ToList();
                    foreach (var item in items)
                    {
                        var img = item.Descendants("img").FirstOrDefault()
                            .GetAttributeValue("src", "");
                        var title = item.Descendants("h5").FirstOrDefault()
                            .InnerText;
                        var detail = item.Descendants("a").FirstOrDefault()
                            .GetAttributeValue("href", "");

                        var heroList = "";
                        var dateTime = "";
                        var descriptions = "";
                        

                        var detailUrl = item.Descendants("a")
                            .Where(node => node.GetAttributeValue("class", "").Equals("content-details"))
                            .FirstOrDefault().GetAttributeValue("href","");
                        try
                        {
                            var detailHtml = await httpClient.GetStringAsync(detailUrl);
                            HtmlDocument detailHtmlDocument = new HtmlDocument();
                            detailHtmlDocument.LoadHtml(detailHtml);

                            descriptions = detailHtmlDocument.DocumentNode.Descendants("section")
                                .Where(node => node.GetAttributeValue("class", "").Equals("item-description"))
                                .FirstOrDefault().InnerText;

                            dateTime = detailHtmlDocument.DocumentNode.Descendants("div")
                                .Where(node => node.GetAttributeValue("class", "").Equals("aboutText"))
                                .ToList()[1].InnerText;
                            foreach (var hero in HeroPool)
                            {
                                if (descriptions.Contains(hero))
                                {
                                    heroList += hero + ".";
                                }
                            }
                            System.Diagnostics.Debug.WriteLine("-----------------");
                            System.Diagnostics.Debug.WriteLine(img);
                            System.Diagnostics.Debug.WriteLine(title);
                            System.Diagnostics.Debug.WriteLine(descriptions);
                            System.Diagnostics.Debug.WriteLine(heroList);
                            System.Diagnostics.Debug.WriteLine(dateTime);
                            System.Diagnostics.Debug.WriteLine("-----------------");

                            MarvelComic marvelComic = new MarvelComic
                            {
                                DateTime = dateTime.ToString(),
                                Title = title,
                                RelatedHero = heroList,
                                Description = descriptions,
                                HeroBg = img,
                                Detail = detail
                            };

                            if ((currentPage + 1) % 5 != 0)
                            {
                                Buffer.Add(JsonConvert.SerializeObject(marvelComic));
                            }
                            else
                            {
                                await FileIO.AppendLinesAsync(sampleFile, Buffer);
                                Buffer.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                        }

                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                currentPage++;
                totalPage++;
                OnProgressChanged?.Invoke(totalPage, CrwalerOptions.Comixology);
                currentUrl = url.Key + currentPage;
            }
            if(Buffer.Count != 0)
            {
                await FileIO.AppendLinesAsync(sampleFile, Buffer);
                Buffer.Clear();
            }
        }
        public static async Task CrawlComics(CrwalerOptions options)
        {
            switch (options)
            {
                case CrwalerOptions.Fandom:
                    await GetFromFandom();
                    break;
                case CrwalerOptions.MarvelHQ:
                    await GetFromMarvelHq();
                    break;
                case CrwalerOptions.Comixology:
                    await GetFromComixology();
                    break;
                default:
                    break;
            }
        }


        public static async Task<List<MarvelComic>> GetComics()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("MarvelComics");

            IList<string> comicsJson = await FileIO.ReadLinesAsync(file);
            List<MarvelComic> comics = new List<MarvelComic>();
            foreach (var comicJson in comicsJson)
            {
                MarvelComic comic = JsonConvert.DeserializeObject<MarvelComic>(comicJson);
                comics.Add(comic);
            }

            return comics;
        }
    }
}
