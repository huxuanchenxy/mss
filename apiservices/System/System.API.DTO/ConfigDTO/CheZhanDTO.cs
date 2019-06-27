using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace System.API.DTO
{

    [DataContract]
   public class CheZhanDTO
    {
        [DataMember]
        /// <summary>
        /// id
        /// </summary>
        public int Id
        {
            get;
            set;

        }

        [DataMember]
        /// <summary>
        /// 车站名称
        /// </summary>
        public string AreaName
        {
            get;
            set;
        }
    }
}
