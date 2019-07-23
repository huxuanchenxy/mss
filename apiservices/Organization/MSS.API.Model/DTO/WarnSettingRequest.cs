using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public class WarnSettingQueryParm : BasePageParam
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipmentTypeID { get; set; }
        public string ParamID { get; set; }
    }
}
