﻿using System;
using System.API.Model;
using System.Collections.Generic;
using System.Text;

namespace System.API.Service 
{
  public  interface IConfigMidAreaService
    { /// <summary>
      /// 得到最大ID
      /// </summary>
        int GetMaxId();
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        bool Exists(int WF_TemID);
        /// <summary>
        /// 增加一条数据
        /// </summary>
        int Add(TB_Config_MidArea model);
        /// <summary>
        /// 更新一条数据
        /// </summary>
        bool Update(TB_Config_MidArea model);
        /// <summary>
        /// 删除一条数据
        /// </summary>
        bool Delete(int WF_TemID);
        bool DeleteList(string WF_TemIDlist);
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        TB_Config_MidArea GetModel(int WF_TemID);

        /// <summary>
        /// 获得数据列表
        /// </summary>
        List<TB_Config_MidArea> GetList(string strWhere);

        List<TB_Config_MidArea> GetListByPid(int Pid);
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        List<TB_Config_MidArea> GetList(int Top, string strWhere, string filedOrder);
        int GetRecordCount(string strWhere);
        List<TB_Config_MidArea> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex);
      

    }
}