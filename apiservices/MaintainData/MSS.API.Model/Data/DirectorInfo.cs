using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MSS.API.Model.Data
{
    [DataContract]
    public  class DirectorInfo
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string directorName { get; set; }
    }
}
