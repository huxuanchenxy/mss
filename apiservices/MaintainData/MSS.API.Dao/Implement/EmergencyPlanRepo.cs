using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using MSS.API.Common;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static MSS.API.Common.MyDictionary;

namespace MSS.API.Dao.Implement
{
    public class EmergencyPlanRepo : BaseRepo, IEmergencyPlanRepo<EmergencyPlan>
    {
        public EmergencyPlanRepo(DapperOptions options) : base(options) { }

        public async Task<EmergencyPlan> Save(EmergencyPlan ePlan)
        {
            return await WithConnection(async c =>
            {
                string sql;
                IDbTransaction trans = c.BeginTransaction();
                try
                {
                    sql = " insert into emergency_plan " +
                        " values (0,@Scene,@Dept,@DeptPath,@Keyword,@Type,@Is_deleted, " +
                        " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy); ";
                    sql += "SELECT LAST_INSERT_ID()";
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, ePlan, trans);
                    ePlan.ID = newid;
                    if (!string.IsNullOrWhiteSpace(ePlan.UploadFiles))
                    {
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(ePlan.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                objs.Add(new
                                {
                                    ePlanID = newid,
                                    fileID = Convert.ToInt32(item),
                                    type = Convert.ToInt32(obj["type"]),
                                    systemResource = (int)SystemResource.EmergencyPlan
                                });
                            }
                        }
                        sql = "insert into upload_file_relation values (0,@ePlanID,@fileID,@type,@systemResource)";
                        int ret = await c.ExecuteAsync(sql, objs, trans);
                    }
                    trans.Commit();
                    return ePlan;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            });
        }

        public async Task<int> Update(EmergencyPlan ePlan)
        {
            return await WithConnection(async c =>
            {
            string sql;
            IDbTransaction trans = c.BeginTransaction();
                try
                {
                    var result = await c.ExecuteAsync(" update emergency_plan " +
                        " set emergency_scene=@Scene,dept_id=@Dept, " +
                        " dept_path=@DeptPath,Keyword=@keyword, " +
                        " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", ePlan,trans);
                    if (!string.IsNullOrWhiteSpace(ePlan.UploadFiles))
                    {
                        sql = "delete from upload_file_relation where entity_id=@id";
                        int ret = await c.ExecuteAsync(sql, new { id = ePlan.ID }, trans);
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(ePlan.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                objs.Add(new
                                {
                                    ePlanID = ePlan.ID,
                                    fileID = Convert.ToInt32(item),
                                    type = Convert.ToInt32(obj["type"]),
                                    systemResource = (int)SystemResource.EmergencyPlan
                                });
                            }
                        }
                        sql = "insert into upload_file_relation values (0,@ePlanID,@fileID,@type,@systemResource)";
                        ret = await c.ExecuteAsync(sql, objs, trans);
                    }
                    trans.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            });
        }

        public async Task<int> Delete(string[] ids)
        {
            return await WithConnection(async c =>
            {
                //var result = await c.ExecuteAsync(" Update emergency_plan set is_del=" + (int)IsDeleted.yes+
                //",updated_time=@updated_time,updated_by=@updated_by" +
                //" WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                var result = await c.ExecuteAsync(" Delete from emergency_plan"+
                " WHERE id in @ids ", new { ids});
                return result;
            });
        }

        public async Task<EPlanView> GetPageByParm(EPlanQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as created_name,u2.user_name as updated_name,o.name ")
                .Append(" FROM emergency_plan a ")
                .Append(" left join org_tree o on a.dept_id=o.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.type=" + parm.Type);
                if (!string.IsNullOrWhiteSpace(parm.SearchName))
                {
                    whereSql.Append(" and a.emergency_scene like '%" + parm.SearchName + "%' ");
                }
                if (!string.IsNullOrWhiteSpace(parm.SearchDesc))
                {
                    whereSql.Append(" and MATCH(keyword) AGAINST ('"+parm.SearchDesc+"' IN BOOLEAN MODE)");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                EPlanView ret = new EPlanView();
                ret.rows= (await c.QueryAsync<EmergencyPlan>(sql.ToString())).ToList();
                ret.total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from emergency_plan a " + whereSql.ToString());
                return ret;
            });
        }

        public async Task<EmergencyPlan> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<EmergencyPlan>(
                    "SELECT * FROM emergency_plan WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<EmergencyPlan>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<EmergencyPlan>(
                    "SELECT * FROM emergency_plan where is_del =" +(int)IsDeleted.no)).ToList();
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
    }
}
