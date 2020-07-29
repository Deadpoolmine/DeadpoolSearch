using DeadpoolSearch.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DeadpoolSearch.Helpers
{
    /// <summary>
    /// 例子：
    /// 
    /// 
    /// </summary>
    public static class SearchCoreHelper
    {
        public static Lucene.Net.Store.Directory Directory { get; set; }
        public static string StorePath { get; set; }
        public static void CreateDirectory(List<MarvelComic> comics)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(folder.Path + "\\MarvelComicsIndexes"));
            StorePath = folder.Path + "\\MarvelComicsIndexes";

            using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            using (var writer = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(1000)))
            {
                foreach (MarvelComic comic in comics)
                {
                    Document doc = new Document();
                    doc.Add(new Field(ConstantsHelper.Title, comic.Title,
                        Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field(ConstantsHelper.DateTime, comic.DateTime,
                        Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field(ConstantsHelper.RelatedHero, comic.RelatedHero,
                        Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field(ConstantsHelper.Description, comic.Description,
                        Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field(ConstantsHelper.HeroBg, comic.HeroBg,
                        Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field(ConstantsHelper.Detail, comic.Detail,
                        Field.Store.YES, Field.Index.ANALYZED));
                    writer.AddDocument(doc);
                }
                writer.Optimize();
                writer.Flush(true, true, true);
            }
            Directory = directory;
        }

        public static ObservableCollection<MarvelComic> Search(string query)
        {
            ObservableCollection<MarvelComic> comics = new ObservableCollection<MarvelComic>();

            using (var reader = IndexReader.Open(Directory, true))
            using (var searcher = new IndexSearcher(reader))
            {
                using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                {
                    /*需要更换*/
                    var queryParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new string[] {ConstantsHelper.DateTime,
                        ConstantsHelper.Description, ConstantsHelper.RelatedHero, ConstantsHelper.Title, ConstantsHelper.Detail }, analyzer);
                    //允许宽度查找
                    queryParser.AllowLeadingWildcard = true;

                    var parsedQuery = queryParser.Parse(query);

                    var collector = TopScoreDocCollector.Create(600, true);

                    searcher.Search(parsedQuery, collector);

                    var matches = collector.TopDocs().ScoreDocs;

                    foreach (var item in matches)
                    {
                        var id = item.Doc;
                        var doc = searcher.Doc(id);
                        MarvelComic comic = new MarvelComic
                        {
                            Title = doc.GetField(ConstantsHelper.Title).StringValue,
                            Description = doc.GetField(ConstantsHelper.Description).StringValue,
                            DateTime = doc.GetField(ConstantsHelper.DateTime).StringValue,
                            RelatedHero = doc.GetField(ConstantsHelper.RelatedHero).StringValue,
                            HeroBg = doc.GetField(ConstantsHelper.HeroBg).StringValue,
                            Detail = doc.GetField(ConstantsHelper.Detail).StringValue
                        };
                        comics.Add(comic);
                    }
                }
            }
            return comics;
        }
    }
}
