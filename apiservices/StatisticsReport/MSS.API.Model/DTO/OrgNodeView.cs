using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public class OrgNodeType
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public bool HasChildren { get; set; }
        public bool HasUsers { get; set; }
        public bool HasUsersLeafOnly { get; set; }
    }
    public class OrgNodeView
    {
        public int id { get; set; }

        public string label { get; set; }

        public string node_type { get; set; }

        public OrgNodeType type { get; set; }

        public List<OrgNodeView> children { get; set; }
        
    }
}
