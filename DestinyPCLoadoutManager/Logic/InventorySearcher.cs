using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Logic.Search;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DestinyPCLoadoutManager.Logic
{
    public class InventorySearcher
    {
        private LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        private Analyzer analyzer;
        private IndexWriter writer;

        private Dictionary<long, Item> storedItems = new Dictionary<long, Item>();

        public InventorySearcher()
        {
            var directory = new RAMDirectory();
            analyzer = new StandardAnalyzer(AppLuceneVersion);
            var config = new IndexWriterConfig(AppLuceneVersion, analyzer);
            writer = new IndexWriter(directory, config);
        }

        public void Index(Inventory inventory)
        {
            var items = inventory.EquippedItems.Union(inventory.InventoryItems);
            var documents = items.Select(i => i.ToDocument());

            writer.AddDocuments(documents);

            foreach (var item in items)
            {
                storedItems.TryAdd(item.Id, item);
            }

            writer.Flush(false, false);
        }

        public IEnumerable<Item> Search(string searchTerm)
        {
            var nameQuery = new FuzzyQuery(new Term("name", searchTerm));
            nameQuery.Boost = 2;
            var typeQuery = new FuzzyQuery(new Term("type", searchTerm));

            var query = new BooleanQuery
            {
                { nameQuery, Occur.SHOULD },
                { typeQuery, Occur.SHOULD }
            };

            var reader = writer.GetReader(true);
            var searcher = new IndexSearcher(reader);

            var otherResults = searcher.Search(new MatchAllDocsQuery(), 20);

            var results = searcher.Search(query, 20);
            return results.ScoreDocs.Select(d => searcher.Doc(d.Doc)).Select(d => d.GetField("id").GetInt64Value()).Select(id => id.HasValue ? storedItems.GetValueOrDefault((long)id) : null).Where(item => item != null);
        }
    }
}
