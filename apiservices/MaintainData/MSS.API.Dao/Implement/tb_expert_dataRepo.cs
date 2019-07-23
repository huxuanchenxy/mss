using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Dapper.FastCrud;

namespace MSS.API.Dao.Implement
{
    public class Tb_expert_dataRepo : BaseRepo, Itb_expert_dataRepo<tb_expert_data>
    {
        public Tb_expert_dataRepo(DapperOptions options) :base(options)
            {
            } 
       public async Task<int> Add(tb_expert_data model)
        { 
            return await WithConnection(
                async c =>
                {
                    string sql = "INSERT INTO tb_expert_data (device_type, deptID,dept_path, keyword, title, content, video_file, attch_file,Is_deleted, created_time, created_by,updated_time,updated_by, remark)"
                                                  + " Values (@device_type, @deptID,@dept_path, @keyword, @title, @content, @video_file, @attch_file,@Is_deleted, @CreatedTime, @CreatedBy,@UpdatedTime,@UpdatedBy, @remark) ";
                    // sql += "SELECT LAST_INSERT_ID()";
                    // int newid = await c.QueryFirstOrDefaultAsync<int>(sql, model);
                    // model.ID = newid; 
                    return await c.QueryFirstOrDefaultAsync<int>(sql, model);
                });
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
      public async  Task<bool>  Delete(tb_expert_data model)
        {
            return await WithConnection(
               async c =>
            {
                string sql = "UPDATE tb_expert_data SET Is_deleted = 1,updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID "; 
                await c.ExecuteAsync(sql, model);
                return true;
            });
        }

        /// <summary>
        /// 省略
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteList(string ids)
        {
            throw new NotImplementedException();
        }

     public  async Task<bool> Exists(string title)
        {
            return await WithConnection(
                async c =>
                {
                    string sql = " SELECT* FROM tb_expert_data WHERE  title = @title and Is_deleted!= 1 ";
                   int ret= await c.ExecuteAsync(sql, new
                    {
                       title = title
                   });
                    if(ret<1)
                    {
                        return false;
                    }
                    return true;

                });
            
        }

        public Task<List<tb_expert_data>> GetList(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<tb_expert_data>> GetList(string strWhere)
        {
            throw new NotImplementedException();
        }

       public async Task<List<tb_expert_data>> GetListByPage(string strWhere,  string orderby, string sort, int page, int size)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM tb_expert_data where   Is_deleted!= 1 and " + strWhere );
                if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(orderby))
                {
                    sql.Append(" order by " + sort  + " " + orderby);
                }
                sql.Append(" limit " + (page - 1) * size + "," + size);

                var list = await c.QueryAsync<tb_expert_data>(sql.ToString());
                return list.ToList();
            });
        }

       public Task<int>  GetMaxId()
        {
            throw new NotImplementedException();
        }

      public async  Task<tb_expert_data>  GetModel(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM tb_expert_data WHERE ID = @ID  and Is_deleted!= 1";
                tb_expert_data model = await c.QueryFirstOrDefaultAsync<tb_expert_data>(sql,
                new
                {
                    ID = id
                });
                return model;
            });
        }

       public async Task<int>  GetRecordCount(string where)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT count(*) FROM tb_expert_data where Is_deleted!= 1"); 
                var count = await c.QueryFirstAsync<int>(sql.ToString());
                return count;
            });
        }
 
      public async   Task<int> Update(tb_expert_data model)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE tb_expert_data SET device_type=@device_type, deptID=@deptID, dept_path=@dept_path,keyword=@keyword, title=@title, content=@content, video_file=@video_file, attch_file=@attch_file,Is_deleted=@Is_deleted, updated_by = @UpdatedBy, updated_time = @UpdatedTime,remark=@remark WHERE id = @id  ";
               return await c.ExecuteAsync(sql, model); ;
            });
        }
    }
}         
