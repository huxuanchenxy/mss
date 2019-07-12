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
    }
    #endregion

    #region 字典
    public static class MyDictionary
    {
        #region 子系统
        public const string STR_SUB_SYSTEM = "sub_system";
        public enum SUB_SYSTEM
        {
            ISCS,
            FAS,
            OCS,
            PIS
        }
        #endregion

        #region 区域类型（表和ID的关系）
        public const string STR_AREA_TYPE = "area_table_id";
        public enum AREA_TYPE
        {
            dictionary,
            tb_config_bigarea,
            tb_config_midarea
        }
        #endregion

        public const string STR_EQPTYPE_DRAWINGS = "eqp_type_drawings";
    }
    #endregion


}
