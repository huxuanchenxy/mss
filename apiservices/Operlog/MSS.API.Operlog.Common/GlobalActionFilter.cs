using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using MSS.API.Common;
using Newtonsoft.Json;
using System;
using MSS.API.Common.Utility;
using MSS.API.Operlog.Dao;
using MSS.API.Operlog.Dao.Implement;
using MSS.API.Operlog.Model.Data;

namespace MSS.Common.Web
{
    public class GlobalActionFilter: IActionFilter
    {
        private  IConfiguration Configuration { get; }
        public GlobalActionFilter(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            if (context.HttpContext.Response.StatusCode == 200)
            {
                string controllername = context.RouteData.Values["Controller"].ToString();
                string actionname = context.RouteData.Values["Action"].ToString();
                string methodname = context.HttpContext.Request.Method.ToString();
                string header = context.HttpContext.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(header) && (methodname != "GET"))
                {
                    string token = string.Empty;
                    if (header.IndexOf("Bearer") >= 0)
                    {
                        token = header.Replace("Bearer", "").Trim();
                    }
                    UserTokenResponse response = new UserTokenResponse();
                    AuthHelper au = new AuthHelper();
                    int userid = au.GetUserId(context.HttpContext);
                    RedisMSSHelper.Init(this.Configuration["redis:ConnectionString"]);
                    response = JsonConvert.DeserializeObject<UserTokenResponse>(RedisMSSHelper.Get(userid.ToString()));
                    string localIPAddress = NetworkHelper.LocalIPAddress;
                    string localMacAddress = NetworkHelper.LocalMacAddress;
                    //DapperOptions options = new DapperOptions();
                    //options.ConnectionString = this.Configuration.Item("Dapper:ConnectionString");
                    UserOperationLog obj = new UserOperationLog();
                    obj.controller_name = controllername;
                    obj.action_name = actionname;
                    obj.method_name = methodname;
                    obj.acc_name = response.acc_name;
                    obj.user_name = response.user_name;
                    obj.ip = localIPAddress;
                    obj.mac_add = localMacAddress;
                    obj.created_time = DateTime.Now;
                    obj.created_by = -999;
                    DapperOptions dapper = new DapperOptions();
                    dapper.ConnectionString = Configuration["Dapper:ConnectionString"];
                    UserOperationLogRepo repo = new UserOperationLogRepo(dapper);
                      repo.Add(obj);
                }
            }


        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

    }
}
