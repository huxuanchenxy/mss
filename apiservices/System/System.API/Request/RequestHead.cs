using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.API.Core.Request
{
    public class RequestHead
    {
        /// <summary>
        /// 版本信息
        /// </summary>
        public string Version { get; set; }


        /// <summary>
        /// Token信息
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// App类型
        /// 默认为0: pc
        /// 1:Android 2:IOS  3:微信 4：M站
        /// </summary>
        public int AppType { get; set; }

        /// <summary>
        /// Api类型
        /// </summary>
        public int ApiType { get; set; }

        public string ContextId { get; set; }

        //From Mi
        /// <summary>
        /// APP版本号(兼容IOS旧版本加的)
        /// </summary>
        public int AppVersion { get; set; }
    }
}