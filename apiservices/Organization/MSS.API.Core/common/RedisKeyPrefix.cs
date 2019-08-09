using System.ComponentModel;
namespace MSS.API.Core.Common
{
    public class RedisKeyPrefix
    {
        public const string Eqp = "eqp";
        public const string Org = "org";

        // 报警
        public const string Alarm = "alarm";
        // 预警
        public const string Warn = "warn";
        // 通知
        public const string Notification = "notification";
    }

    public enum UpdateEventType
    {
        [Description("设备配置")]
        InitEqpConfig = 0,

        [Description("所有设备")]
        InitEquipment = 1,

        [Description("顶级公司下所有用户")]
        InitTopOrg = 2,

        [Description("预警设置")]
        InitWarnSetting = 3,
    }
}
