using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MSS.API.Common.Utility;
using static MSS.API.Common.Const;
using Newtonsoft.Json.Linq;

namespace MSS.API.Common.Global
{
    public class GlobalActionFilter: IActionFilter
    {
        IAuthHelper _authhelper;
        public GlobalActionFilter(IAuthHelper authhelper)
        {
            _authhelper = authhelper;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string controllername = context.RouteData.Values["Controller"].ToString();
            string actionname = context.RouteData.Values["Action"].ToString();
            string methodname = context.HttpContext.Request.Method.ToString();

            var userid = _authhelper.GetUserId();

            string url = "/" + controllername + "/" + actionname;
            //ApiResult api = HttpClientHelper.GetResponse<ApiResult>(AUTHSERVICE + "api/v1/user/" + userid);
            //JObject jobj = JsonConvert.DeserializeObject<JObject>(api.data.ToString());
            //if (!(bool)jobj["is_super"])
            //{
            //    //白名单
            //    ApiResult api = HttpClientHelper.GetResponse<ApiResult>(AUTHSERVICE + "api/v1/user/GetAction");
            //    JArray arr = JsonConvert.DeserializeObject<JArray>(api.data.ToString());
            //    //if (arr.SelectToken()
            //}

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }


        

    }
}
