using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
   public class DeviceMainRegQuery: BasePageParm
    {  
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 负责班组
        /// </summary>
        public string TeamGroup { get; set; }

        /// <summary>
        /// 维护负责人
        /// </summary>
        public string Director { get; set; } 

        /// <summary>
        /// 维护日期
        /// </summary>
        public string maintain_date { get; set; }
    }
}
