using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Core.Models.Ex
{
    public class EquipmentConfig
    {
        public int reminder { get; set; }
        public int beforeDead { get; set; }
        public int beforeMaintainMiddle { get; set; }
        public int beforeMaintainBig { get; set; }
        public string textTemplate { get; set; }
    }
}
