using DestinyPCLoadoutManager.API.Models;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace DestinyPCLoadoutManager.Logic.Search
{
    public static class SearchExtensions
    {
        public static Document ToDocument(this Item item)
        {
            return new Document
            {
                new Int64Field("id", item.Id, Field.Store.YES),
                new TextField("name", item.Name, Field.Store.YES),
                new TextField("type", item.Type.ToString(), Field.Store.YES),
                new TextField("subtype", item.SubType.ToString(), Field.Store.YES)
            };
        }
    }
}
