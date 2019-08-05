using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Common;

namespace MSS.API.Core.Models.Ex
{
    public class EquipmentConfig
    {
        public int reminder { get; set; }
        public int beforeDead { get; set; }
        public int beforeMaintainMiddle { get; set; }
        public int beforeMaintainBig { get; set; }
        public string textTemplate { get; set; }
    }

    public class EqpHistory
    {
        public int ID { get; set; }
        public int EqpID { get; set; }
        public MyDictionary.EqpHistoryType Type { get; set; }
        public string TypeName { get; set; }
        public int WorkingOrder { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
// {"eTime":"2019-08-01T13:21:32+00:00","eTime_MS":966,"pid":"r_s01_t10_a_0001","eLevel":1
// ,"ack":0,"originTime":"2019-08-01T13:11:28","originTime_MS":179,"restoreTime":"2019-08-01T13:11:18"
// ,"restoreTime_MS":43,"ackTime":"2019-08-01T13:21:32","ackTime_MS":966,"nodeID":"OCC","user":"RDB_TST_RegWrite"
// ,"src":"S01SVRA","type":5,"eqDes":"EQDes","pidDes":"甲烷含量1浓度"
// ,"valueDisplay":"32.7 ppm (u)","des":"越上上限 10 ppm","stnNo":23,"stnName":"中心01","specialtyNo":1
// ,"eqType":"EQType","pushGraph":"EQType"}
    public class Alarm
    {
        public DateTime ETime { get; set;}
        public string pid { get; set; }
        public int eLevel { get; set; }
        public int Ack { get; set; }
        public DateTime OriginTime { get; set; }
        public DateTime? RestoreTime { get; set; }
        public DateTime? AckTime { get; set; }
        public string NodeID { get; set; }
        public string PidDes { get; set; }
        public string Des { get; set; }

        // ex
        public string EqpCode  { get; set; }
        public string EqpName { get; set; }
        public string EqpTypeName { get; set; }
    }
}
