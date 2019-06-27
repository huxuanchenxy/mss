using System;
using System.Collections.Generic;
using System.Text;

namespace System.API.Model
{
    public abstract class BaseEntity
    { 
        public DateTime Created_Time { get; set; }
        public int Created_By { get; set; }
        public DateTime Updated_Time { get; set; }
        public int Updated_By { get; set; }

        public int Is_Deleted { get; set; }
    }
}
