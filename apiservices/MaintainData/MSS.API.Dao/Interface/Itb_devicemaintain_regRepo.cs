using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface Itb_devicemaintain_regRepo<T> where T : BaseEntity
    {

        Task<int> Add(tb_devicemaintain_reg model);
        Task<int> Update(tb_devicemaintain_reg node);
        Task<bool> Delete(tb_devicemaintain_reg node);

        Task<bool> DeleteList(string ids);

        Task<int> GetMaxId();

        Task<bool> Exists(string keyword);
        Task<tb_devicemaintain_reg> GetModel(int id);

        Task<int> GetRecordCount(string where);

        Task<List<tb_devicemaintain_reg>> GetList(int id);

        /// <summary>
        /// 组合条件查询所有
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<List<tb_devicemaintain_reg>> GetList(string strWhere);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <param name="orderby">asc：desc</param>
        /// <param name="sort">分类字段</param>
        /// <param name="pagesize">每页大小</param>
        /// <returns></returns>
        Task<List<tb_devicemaintain_reg>> GetListByPage(string strWhere, string sort, string orderby, int page, int size);
    }
}
