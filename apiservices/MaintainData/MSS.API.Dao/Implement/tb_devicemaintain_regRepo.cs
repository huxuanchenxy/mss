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
    public class tb_devicemaintain_regRepo : BaseRepo, Itb_devicemaintain_regRepo<tb_devicemaintain_reg>
    {
        public tb_devicemaintain_regRepo(DapperOptions options) : base(options)
        { 
        }
        public async Task<int> Add(tb_devicemaintain_reg model)
        {
            return await WithConnection(
                async c =>
                {
                    string sql = "INSERT INTO tb_devicemaintain_reg (device_type_id, device_id, team_group_id, director_id, maintain_date,attch_file,file_type,detail_desc,Is_deleted, created_time, created_by,updated_time,updated_by)"
                                                  + " Values (@device_type_id, @device_id, @team_group_id, @director_id, @maintain_date,@attch_file,@file_type,@detail_desc,@Is_deleted, @CreatedTime, @CreatedBy,@UpdatedTime,@UpdatedBy) "; 
                    return await c.QueryFirstOrDefaultAsync<int>(sql, model);
                });
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Delete(tb_devicemaintain_reg model)
        {
            return await WithConnection(
               async c =>
               {
                   string sql = "UPDATE tb_devicemaintain_reg SET Is_deleted = 1,updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID ";
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

        public async Task<bool> Exists(string title)
        {
            return await WithConnection(
                async c =>
                {
                    string sql = " SELECT* FROM tb_devicemaintain_reg WHERE  id = @id and Is_deleted!= 1 ";
                    int ret = await c.ExecuteAsync(sql, new
                    {
                        id = title
                    });
                    if (ret < 1)
                    {
                        return false;
                    }
                    return true;

                });

        }

        public Task<List<tb_devicemaintain_reg>> GetList(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<tb_devicemaintain_reg>> GetList(string strWhere)
        {
            throw new NotImplementedException();
        }

        public async Task<List<tb_devicemaintain_reg>> GetListByPage(string strWhere, string orderby, string sort, int page, int size)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM tb_devicemaintain_reg where   Is_deleted!= 1 and " + strWhere);
                if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(orderby))
                {
                    sql.Append(" order by " + sort + " " + orderby);
                }
                sql.Append(" limit " + (page - 1) * size + "," + size);

                var list = await c.QueryAsync<tb_devicemaintain_reg>(sql.ToString());
                return list.ToList();
            });
        }

        public Task<int> GetMaxId()
        {
            throw new NotImplementedException();
        }

        public async Task<tb_devicemaintain_reg> GetModel(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM tb_devicemaintain_reg WHERE ID = @ID  and Is_deleted!= 1";
                tb_devicemaintain_reg model = await c.QueryFirstOrDefaultAsync<tb_devicemaintain_reg>(sql,
                new
                {
                    ID = id
                });
                return model;
            });
        }

        public async Task<int> GetRecordCount(string where)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT count(*) FROM tb_devicemaintain_reg where Is_deleted!= 1");
                var count = await c.QueryFirstAsync<int>(sql.ToString());
                return count;
            });
        }

        public async Task<int> Update(tb_devicemaintain_reg model)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE tb_devicemaintain_reg SET device_type_id=@device_type_id, device_id=@device_id, attch_file=@attch_file,file_type=@file_type, team_group_id=@team_group_id, director_id=@director_id, maintain_date=@maintain_date, detail_desc=@detail_desc,Is_deleted=@Is_deleted, updated_by = @UpdatedBy, updated_time = @UpdatedTime  WHERE id = @id  ";
                return await c.ExecuteAsync(sql, model); ;
            });
        } 
    }
}
