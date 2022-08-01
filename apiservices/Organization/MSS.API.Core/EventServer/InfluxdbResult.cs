using System.Collections.Generic;
namespace MSS.API.Core.EventServer
{
    public class Series
    {
        public string name { get; set; }
        public List<string> columns { get; set; }
        public List<List<object>> values { get; set; }
    }
    public class Statement
    {
        public int statement_id { get; set; }
        public List<Series> series { get; set; }
    }
    public class InfluxdbResult
    {
        public List<Statement> results { get; set; }
    }
}