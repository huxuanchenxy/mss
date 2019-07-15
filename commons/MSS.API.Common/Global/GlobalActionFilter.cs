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
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }



        

    }
}
