using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
   public interface Itb_expert_dataRepo<T> where T : BaseEntity
    {
        
        Task<int> Add(tb_expert_data model);
        Task<int> Update(tb_expert_data node);
        Task<bool> Delete(tb_expert_data node);

        Task<bool> DeleteList(string ids);

        Task<int> GetMaxId();

        Task<bool> Exists(string keyword);
        Task<tb_expert_data> GetModel(int id); 

        Task<int> GetRecordCount(string where);  

        Task<List<tb_expert_data>> GetList(int id);

        /// <summary>
        /// 组合条件查询所有
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<List<tb_expert_data>> GetList(string strWhere);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <param name="orderby">asc：desc</param>
        /// <param name="sort">分类字段</param>
        /// <param name="pagesize">每页大小</param>
        /// <returns></returns>
        Task<List<tb_expert_data>> GetListByPage(string strWhere,  string sort, string orderby, int page, int size);       
    }
}
