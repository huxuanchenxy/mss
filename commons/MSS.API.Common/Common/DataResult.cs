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
        public static string BASEFILE = "D:/bin/eqp/";//"F:/mssAll/MSS/apiservices/Equipment/MSS.API.Core/V1/"; 
        public static string EQPTYPE = "File/EqpType/";
    }
    #endregion

}
