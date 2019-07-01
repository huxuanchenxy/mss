using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MSS.API.Model.Data
{ 
    [DataContract]
   public class DeviceType
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string deviceTypeName { get; set; }
    }
}
