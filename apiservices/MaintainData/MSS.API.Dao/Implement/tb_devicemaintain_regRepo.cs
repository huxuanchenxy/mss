using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Dapper.FastCrud;
using System.Transactions;
using static MSS.API.Common.MyDictionary;

namespace MSS.API.Dao.Implement
{
    public class tb_devicemaintain_regRepo : BaseRepo, Itb_devicemaintain_regRepo<tb_devicemaintain_reg>
    {
        public tb_devicemaintain_regRepo(DapperOptions options) : base(options)
        {
        }
        public async Task<int> Add(tb_devicemaintain_reg model)
        {
            List<string> list = new List<string>();
            int ret = 0;
            StringBuilder sqlBuild = new StringBuilder();
            string sql = "INSERT INTO tb_devicemaintain_reg (device_type_id, device_id,device_id_path, team_group_id,team_group_path, director_id, maintain_date,attch_file,file_type,origin_file,detail_desc,Is_deleted, created_time, created_by,updated_time,updated_by)"
                                                + " Values (@device_type_id, @device_id,@device_id_path, @team_group_id,@team_group_path, @director_id, @maintain_date,@attch_file,@file_type,@origin_file,@detail_desc,@Is_deleted, @CreatedTime, @CreatedBy,@UpdatedTime,@UpdatedBy);" +
                                                " SELECT LAST_INSERT_ID() ";
            return await WithConnection(
                async c =>
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        int newid = await c.QueryFirstOrDefaultAsync<int>(sql, model);
                        if (!string.IsNullOrEmpty(model.attch_file.Trim()))
                        {
                            list = ((IEnumerable<string>)model.attch_file.Split(',')).ToList<string>();
                            foreach (var v in list)
                            {
                                sqlBuild.AppendFormat("insert into upload_file_relation(entity_id, file_id, type, system_resource) values({0},{1},{2},{3})", newid, v, model.file_type, 28);
                                sqlBuild.AppendLine(";");
                            }
                            ret = await c.QueryFirstOrDefaultAsync<int>(sqlBuild.ToString());
                        }
                        scope.Complete();
                    }
                    return ret;
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
                sql.Append("SELECT a.*,b.type_name as device_type_name,c.eqp_name as device_name ,u.user_name as updated_name,o.name as team_group_name " +
                    " FROM tb_devicemaintain_reg a " +
                    " JOIN equipment_type b on a.device_type_id=b.id " +
                    " JOIN equipment c ON a.device_id=c.id " +
                    " join user u on a.updated_by = u.id " +
                    " JOIN org_tree o on a.team_group_id=o.ID " +
                    " where   a.Is_deleted!= 1 and " + strWhere);
                //join user u on a.updated_by=u.id  
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
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT  a.*,b.type_name as device_type_name,c.eqp_name as device_name ,u.user_name as updated_name,o.name as team_group_name " +
                   " FROM tb_devicemaintain_reg a " +
                   " JOIN equipment_type b on a.device_type_id=b.id " +
                   " JOIN equipment c ON a.device_id=c.id " +
                   " join user u on a.updated_by = u.id " +
                   " JOIN org_tree o on a.team_group_id=o.ID " +
                    "WHERE a.ID = @ID  and a.Is_deleted != 1");
                tb_devicemaintain_reg model = await c.QueryFirstOrDefaultAsync<tb_devicemaintain_reg>(sql.ToString(),
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


        public async Task<List<UploadFile>> ListByEntity(int[] ids, SystemResource systemResource)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<UploadFile>(
                    "SELECT uf.*,ufr.type,dt.name as TypeName,dt1.name as SystemResourceName," +
                    "ufr.system_resource,ufr.entity_id FROM upload_file uf " +
                    "left join upload_file_relation ufr on ufr.file_id=uf.id " +
                    "left join dictionary_tree dt on ufr.type=dt.id " +
                    "left join dictionary_tree dt1 on ufr.system_resource=dt1.id " +
                    "WHERE ufr.system_resource=@systemResource and ufr.entity_id in @ids",
                    new { systemResource = (int)systemResource, ids });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return null;
                }
            });
        }

        public async Task<int> Update(tb_devicemaintain_reg model)
        {
            //return await WithConnection(async c =>
            //{
            string sql = "UPDATE tb_devicemaintain_reg SET device_type_id=@device_type_id,team_group_path=@team_group_path, device_id=@device_id,device_id_path=@device_id_path, attch_file=@attch_file,file_type=@file_type,origin_file=@origin_file, team_group_id=@team_group_id, director_id=@director_id, maintain_date=@maintain_date, detail_desc=@detail_desc,Is_deleted=@Is_deleted, updated_by = @UpdatedBy, updated_time = @UpdatedTime  WHERE id = @id  ";
            //    return await c.ExecuteAsync(sql, model); ;
            //});
            List<string> list = new List<string>();
            int ret = 0;
            StringBuilder sqlBuild = new StringBuilder();
            return await WithConnection(
                async c =>
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        int newid = model.ID;
                        await c.ExecuteAsync(sql, model);
                        await c.ExecuteAsync("delete from upload_file_relation where entity_id=@entity_id ", new { entity_id = newid });
                        if (!string.IsNullOrEmpty(model.attch_file.Trim()))
                        {
                            list = ((IEnumerable<string>)model.attch_file.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
                            foreach (var v in list)
                            {
                                string[] strs = v.ToString().Split(':');
                                string[] arr = strs[1].Split(',');
                                foreach (var u in arr)
                                {
                                    sqlBuild.AppendFormat("insert into upload_file_relation(entity_id, file_id, type, system_resource) values({0},{1},{2},{3})", newid, u, strs[0], 27);
                                    sqlBuild.AppendLine(";");
                                }

                            }
                            ret = await c.QueryFirstOrDefaultAsync<int>(sqlBuild.ToString());
                        }
                        scope.Complete();
                    }
                    return ret;
                });
        }
    }
}
