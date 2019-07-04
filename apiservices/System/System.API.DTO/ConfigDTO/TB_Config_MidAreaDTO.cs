using System;
using System.Runtime.Serialization;

namespace System.API.DTO
{
    /// <summary>
    /// TB_WF_Template:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [DataContract]
    public partial class TB_Config_MidAreaDTO
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
        public int PID
		{
            get;
            set;
        }


        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string PName
        {
            get;
            set;
        }


        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int ConfigType
        {
            get;
            set;
        }


        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string ConfigTypeName
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
        /// <summary>
        /// 
        /// </summary>
        public int? Is_Used
		{
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int? Is_Deleted
		{
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Created_Time
		{
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int? Created_By
		{
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Updated_Time
		{
            get;
            set;
        }

       [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int? Updated_By
		{
            get;
            set;
        }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string Remark
		{
            get;
            set;
        }
	 

	}
}

