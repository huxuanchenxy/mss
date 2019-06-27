using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.API.Core.Request
{
    public class RequestContext<T>
    {
        /// <summary>
        /// 请求头
        /// </summary>
        //public RequestHead Head { get; set; }

        /// <summary>
        /// 请求体
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public RequestContext()
        {
          //  this.Head = new RequestHead();
            this.data = default(T);
        }

        #region
        public bool CheckValid()
        {
            //if (Head == null || data == null)
            //{
            //    return false;
            //}

            return true;
        }
        #endregion
    }
}