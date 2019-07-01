using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    /// <summary>
    /// 查询条件
    /// </summary>
  public class ExpertDataQurey : BasePageParm
    {
        public string keyword { get; set; }

        public string titile { get; set; }      

        public string deviceType { get; set; }

        public string dept { get; set; }
    }
}
