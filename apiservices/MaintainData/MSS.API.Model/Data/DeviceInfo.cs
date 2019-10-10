using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MSS.API.Model.Data
{
    [DataContract]
    public class DeviceInfo
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string deviceName { get; set; }
 
    }


    public class LocationDeviceInfo
    {

        public int ConfigTypeID { get; set; }

        public int MetroLineID { get; set; }

        public string ConfigType { get; set; }

        public int StationID { get; set; }

        public string StationName { get; set; }

        public int LocationID { get; set; }

        public string LocationName { get; set; }

    }
}