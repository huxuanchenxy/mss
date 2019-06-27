/**  版本信息模板在安装目录下，可自行修改。
* TB_Config_MidArea.cs
*
* 功 能： N/A
* 类 名： TB_Config_MidArea
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2019/6/21 10:12:45   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
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

        //[DataMember]
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Updated_Time
		{
            get;
            set;
        }

       // [DataMember]
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

