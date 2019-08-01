using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.Data
{
  public  class LocatioInfo
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public List<LocatioInfo> Children { get; set; }// = new List<LocatioInfo>();
    } 
}
