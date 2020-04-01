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

        // 点位预警
        public const string NotificationPidcount = "notificationpidcount";

        // 报警分析
        public const string AlarmStatistic = "alarm-statistic";

        // 事件中心，管理员用户组
        public const string SuperGroup = "superGroup";
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

        [Description("所有pid列表")]
        InitAllPid = 4,

        [Description("所有pid列表")]
        InitStatisticsDimention = 5,
    }
}
