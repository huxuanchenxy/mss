using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public class AlarmParam : BasePageParam
    {
        public int? orgID { get; set; }
        public int? eqpTypeID { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public int? status { get; set; }
    }
}
