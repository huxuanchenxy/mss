using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace System.API.Core.Response
{
    [Serializable]
    [DataContract]
    public class ResponseContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseContext()
        {
            this.ret = 0;
            this.data = null;
        }

        #region 
        /// <summary>
        /// 响应头
        /// </summary>
        //[DataMember]
        // public ResponseHead Head { get; set; }

        /// <summary>
        /// 返回值
        /// 0   :成功
        /// -1  :失败
        /// </summary>
        [DataMember]
        public int ret { get; set; }

        /// <summary>
        /// 错误码
        /// 1000,1001
        /// </summary>
       // [DataMember]
        public ErrCode code { get; set; }
        /// <summary>
        /// 响应体
        /// </summary>
        [DataMember]
        public object data { get; set; }

        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public int total { get; set; }
        #endregion
    }
}