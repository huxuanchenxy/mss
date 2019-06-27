using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace System.API.DTO 
{
    [DataContract]
    public  class BaseDTO
    {
        [DataMember]
        public int ret { get; set; }
        
    }
}
