using System.ComponentModel;
using MSS.API.Model.Data;
namespace MSS.API.Core.EventServer
{
    public enum MssEventType {
        [Description("设备报警消息")]
        Alarm = 0,

        [Description("设备预警消息")]
        Warnning = 1,

        [Description("通知消息")]
        Notification = 2,
    }

    public class MssEventMsg
    {
        public Equipment eqp { get; set; }
        public MssEventType type { get; set; }
        public string msg { get; set; }
        public object detail { get; set; }
    }
}