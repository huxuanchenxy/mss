using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.Data
{
    public class DictionaryTree:BaseEntity
    {
        public string parent_id { get; set; }
        public string name { get; set; }
        public int node_type { get; set; }
    }
}
