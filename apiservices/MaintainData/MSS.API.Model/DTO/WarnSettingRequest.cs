﻿using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public class WarnSettingQueryParm : BasePageParm
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipmentTypeID { get; set; }
    }
}
