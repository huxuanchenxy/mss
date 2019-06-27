/**  版本信息模板在安装目录下，可自行修改。
* TB_Config_BigArea.cs
*
* 功 能： N/A
* 类 名： TB_Config_BigArea
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
namespace System.API.Model
{
	/// <summary>
	/// TB_Config_BigArea:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_Config_BigArea
	{
		public TB_Config_BigArea()
		{}
		#region Model
		private int _id;
		private string _areaname;
        private int? _configType;
        private int? _sort;
		private int? _is_used;
		private int? _is_deleted;
		private DateTime? _created_time;
		private int? _created_by;
		private DateTime? _updated_time;
		private int? _updated_by;
		private string _remark;
		/// <summary>
		/// 
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string AreaName
		{
			set{ _areaname=value;}
			get{return _areaname;}
		}


        public int? ConfigType
        {
            set { _configType = value; }
            get { return _configType; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? sort
		{
			set{ _sort=value;}
			get{return _sort;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Is_Used
		{
			set{ _is_used=value;}
			get{return _is_used;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Is_Deleted
		{
			set{ _is_deleted=value;}
			get{return _is_deleted;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? Created_Time
		{
			set{ _created_time=value;}
			get{return _created_time;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Created_By
		{
			set{ _created_by=value;}
			get{return _created_by;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? Updated_Time
		{
			set{ _updated_time=value;}
			get{return _updated_time;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Updated_By
		{
			set{ _updated_by=value;}
			get{return _updated_by;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		#endregion Model

	}
}

