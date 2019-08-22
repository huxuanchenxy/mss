using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public class StatisticsParam
    {
        public string EqpTypeIDs { get; set; }

        public string SupplierIDs { get; set; }

        public string ManufacturerIDs { get; set; }

        public string SubSystemIDs { get; set; }

        // 位置ids ，号分割
        public string LocationLevel1s { get; set; }

        public string LocationLevel2s { get; set; }

        public string LocationLevel3s { get; set; }

        public string TopOrgIDs { get; set; }

        public string TeamIDs { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
        
    }
}
