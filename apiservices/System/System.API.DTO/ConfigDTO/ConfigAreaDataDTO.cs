using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace System.API.DTO
{
    [DataContract]
    public  class ConfigAreaDataDTO
    {
        [DataMember]
        public List<DicAreaDTO> DicAreaList { get; set; }
    }

    [DataContract]
    public class DicAreaDTO
    {
         [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;

        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string AreaName
        {
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int? sort
        {
            get;
            set;
        }

        [DataMember]
        public List<BigAreaDTO> children { get; set; }
    }

    [DataContract]
    public class BigAreaDTO
    {
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;

        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string AreaName
        {
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int? sort
        {
            get;
            set;
        }

        [DataMember]
        public List<MidAreaDTO> children { get; set; }
    }

    [DataContract]
    public class MidAreaDTO
    {
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;

        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string AreaName
        {
            get;
            set;
        }

    }
}
