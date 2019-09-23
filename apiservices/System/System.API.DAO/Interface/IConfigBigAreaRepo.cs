using System;
using System.API.Model;
using System.Collections.Generic;
using System.Text;

namespace System.API.DAO.Interface
{
   public interface IConfigBigAreaRepo<T> //where T : BaseEntity
    {


        #region  成员方法
        /// <summary>
        /// 得到最大ID
        /// </summary>
        int GetMaxId();
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        bool Exists(int ID);
        /// <summary>
        /// 增加一条数据
        /// </summary>
        int Add(TB_Config_BigArea model);
        /// <summary>
        /// 更新一条数据
        /// </summary>
        bool Update(TB_Config_BigArea model);
        /// <summary>
        /// 删除一条数据
        /// </summary>
        bool Delete(int ID);
        bool DeleteList(string IDlist);
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        TB_Config_BigArea GetModel(int ID);

        /// <summary>
        /// 获得数据列表
        /// </summary>
        List<TB_Config_BigArea> GetList(string strWhere);

        List<TB_Config_BigArea> GetListByConfigType(string filedOrder);
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        List<TB_Config_BigArea> GetList(int Top, string strWhere, string filedOrder);
        int GetRecordCount(string strWhere);
        List<TB_Config_BigArea> GetListByPage(string strWhere, string sort, string orderby, int page, int size);

        /// <summary>
        /// 获得所有数据
        /// </summary>
        List<TB_Config_BigArea> ListAll();

        #endregion  成员方法
        #region  MethodEx

        #endregion  MethodEx
    }
}
