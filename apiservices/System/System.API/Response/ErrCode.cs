using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace System.API.Core.Response
{
    public enum ErrCode
    {
        [Description("接口调用失败")]
        Failure = 9000,

        [Description("无权限")]
        PermissionDenied = 9990, 

        [Description("数据已存在")]
        DataIsExist = 9992,

        [Description("数据不存在")]
        DataIsnotExist = 9993,

        [Description("数据查询错误")]
        QueryError = 9994,

        [Description("数据插入错误")]
        InsertError = 9995,

        [Description("数据删除错误")]
        DeleteError = 9996,

        [Description("数据修改错误")]
        UpdateError = 9997,

        [Description("参数错误")]
        ParameterError = 9998,

        [Description("内部错误")]
        InnerError = 9999,

        [Description("接口调用成功")]
        Sucess = 10000,

        #region 错误定义
       

        [Description("用户名或者密码不合法")]
        InvalidUsernameOrPwd = 10007,

      
        [Description("请求体不合法")]
        InvalidRequestBody = 10009,


        [Description("输入的参数不能为空")]
        ParametersIsNotAllowedEmpty_Code = 10010,
 
  

        [Description("输入的密码信息不能为空")]
        PasswordIsNotAllowedEmpty_Reg = 10019,

     

        [Description("帐号已存在")]
        AccountAlreadyExist_Reg = 10023,

        [Description("用户密码不正确")]
        PasswordIsNotCorrect_Reg = 10024,

        [Description("创建用户失败")]
        FaildCreateAccount_Reg = 10025,

        [Description("注册成功")]
        RegisterSuccess_Reg = 10026, 
   

        [Description("登录失败")]
        LoginFailed_Login = 10030,
 

        [Description("登录成功")]
        LoginSuccess_Login = 10032, 

        [Description("Token已过期")]
        TokenIsOutOfTime = 10046,


        [Description("获取用户信息成功")]
        GetUserProfileSuccess_GetUserProfile = 10047, 

        [Description("输入的字符串不允许为空")]
        ParametersIsNotAllowedEmpty_ApplyProduct = 10195,

        [Description("无法解析字符串, 请确认是否为Json格式")]
        ParametersIsNotValid_ApplyProduct = 10196, 
 
        [Description("Token已过期，请重新登录！")]
        TokenPastDue = 110080,

        [Description("授权认证失败401")]
        Unauthorized = 110081,

        #endregion
    }
}