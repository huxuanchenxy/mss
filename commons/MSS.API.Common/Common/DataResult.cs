using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace MSS.API.Common
{
    public enum Code
    {
        [Description("接口调用成功")]
        Success = 0,
        [Description("接口调用失败")]
        Failure = 1,
        [Description("数据已存在")]
        DataIsExist = 2,
        [Description("数据不存在")]
        DataIsnotExist = 3,
        // 向不可添加子节点的节点添加节点
        [Description("数据校验失败")]
        CheckDataRulesFail= 4,
        [Description("绑定用户存在冲突")]
        BindUserConflict = 5
    }
    public class ApiResult
    {
        public Code code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }

    #region 常量
    public enum IsDeleted
    {
        no,
        yes
    }
    public class Const
    {
        public static int PAGESIZE = 10;

        //public static string REDISConn_AUTH;
        public static string REDIS_AUTH_KEY_ACTIONINFO = "Auth_ActionInfo";
        public static string REDIS_AUTH_KEY_ROLEACTION = "Auth_RoleAction";
        public static string REDIS_AUTH_KEY_USER = "Auth_User";
    }

    public static class FilePath
    {
        /// <summary>
        /// 基础绝对路径
        /// </summary>
        public static string BASEFILE = "D:/bin/eqp/";
        public static string SHAREFILE = "File/";
    }

    public enum FileType
    {
        //设备类型中的作业指导书
        EqpType_Working_Instruction = 0,
        //设备类型中的技术图纸
        EqpType_Technical_Drawings = 1,
        //设备类型中的安装手册
        EqpType_Installation_Manual = 2,
        //设备类型中的使用手册
        EqpType_User_Guide = 3,
        //设备类型中的维护手册
        EqpType_Regulations = 4,
        //设备中的设备图纸
        Eqp_Drawings = 5,
        //设备生命周期维护的附件
        DeviceMaintain_attach = 6,
        
        //专家库视频资料
        ExpertData_vedio = 7,

        //专家库附件资料
        ExpertData_attach = 8
    }
    #endregion

    #region 字典
    public static class MyDictionary
    {
        public enum SystemResource
        {
            EqpType = 25,
            Eqp=26,
            Expert=27,
            MaintainReg=28
        }

        public enum EqpHistoryType
        {
            Install = 39,
            MediumMaintenance = 40,
            MajorMaintenance = 41,
            TroubleMaintenance = 42,
            FirstMajorConstructio=43,
            SecondaryMajorConstructio=44,
            Expiration=45
        }
    }
    #endregion


}
