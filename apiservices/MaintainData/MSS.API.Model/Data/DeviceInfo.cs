using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MSS.API.Model.Data
{
    [DataContract]
    public  class DeviceInfo
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string deviceName { get; set; }
    }
}