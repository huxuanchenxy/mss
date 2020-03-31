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
    public interface IEquipmentRepairHistoryRepo<T> where T : BaseEntity
    {
        Task<EquipmentRepairHistory> Save(EquipmentRepairHistory equipmentRepairHistory);
        Task<int> Update(EquipmentRepairHistory equipmentRepairHistory);
        Task<int> Delete(string[] ids);
        Task<EquipmentRepairHistoryView> GetPageByParm(EquipmentRepairHistoryQueryParm parm);
        Task<EquipmentRepairHistory> GetByID(int id);
    }

    public class EquipmentRepairHistoryRepo : BaseRepo, IEquipmentRepairHistoryRepo<EquipmentRepairHistory>
    {
        public EquipmentRepairHistoryRepo(DapperOptions options) : base(options) { }

        public async Task<EquipmentRepairHistory> Save(EquipmentRepairHistory equipmentRepairHistory)
        {
            return await WithConnection(async c =>
            {
                string sql;
                //IDbTransaction trans = c.BeginTransaction();
                //try
                {
                    sql = " insert into equipment_repair_history " +
                        " values (0,@Trouble,@Eqp,@EqpPath,@Desc,@PMType,@ReplaceType, " +
                        " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy); ";
                    sql += "SELECT LAST_INSERT_ID()";
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, equipmentRepairHistory);
                    equipmentRepairHistory.ID = newid;
                    //if (!string.IsNullOrWhiteSpace(equipmentRepairHistory.UploadFiles))
                    //{
                    //    List<object> objs = new List<object>();
                    //    JArray jobj = JsonConvert.DeserializeObject<JArray>(equipmentRepairHistory.UploadFiles);
                    //    foreach (var obj in jobj)
                    //    {
                    //        foreach (var item in obj["ids"].ToString().Split(','))
                    //        {
                    //            objs.Add(new
                    //            {
                    //                eqpRepairID = newid,
                    //                fileID = Convert.ToInt32(item),
                    //                type = Convert.ToInt32(obj["type"]),
                    //                systemResource = (int)SystemResource.EqpRepair
                    //            });
                    //        }
                    //    }
                    //    sql = "insert into upload_file_relation values (0,@eqpRepairID,@fileID,@type,@systemResource)";
                    //    int ret = await c.ExecuteAsync(sql, objs, trans);
                    //}
                    //trans.Commit();
                    return equipmentRepairHistory;
                }
                //catch (Exception ex)
                //{
                //    trans.Rollback();
                //    throw new Exception(ex.ToString());
                //}
            });
        }

        public async Task<int> Update(EquipmentRepairHistory equipmentRepairHistory)
        {
            return await WithConnection(async c =>
            {
                string sql;
                IDbTransaction trans = c.BeginTransaction();
                try
                {
                    sql = " update equipment_repair_history " +
                        " set trouble=@Trouble,eqp=@Eqp,eqp_path=@EqpPath,`desc`=@Desc,pm_type=@PMType, " +
                        " replace_type=@ReplaceType,updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id";
                    int result = await c.ExecuteAsync(sql, equipmentRepairHistory,trans);
                    if (!string.IsNullOrWhiteSpace(equipmentRepairHistory.UploadFiles))
                    {
                        sql = "delete from upload_file_relation where entity_id=@id and system_resource=@sr";
                        int ret = await c.ExecuteAsync(sql, new { id = equipmentRepairHistory.ID, sr = SystemResource.EmergencyPlan }, trans);
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(equipmentRepairHistory.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                objs.Add(new
                                {
                                    eqpRepairID = equipmentRepairHistory.ID,
                                    fileID = Convert.ToInt32(item),
                                    type = Convert.ToInt32(obj["type"]),
                                    systemResource = (int)SystemResource.EqpRepair
                                });
                            }
                        }
                        sql = "insert into upload_file_relation values (0,@eqpRepairID,@fileID,@type,@systemResource)";
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
                var result = await c.ExecuteAsync(" delete from equipment_repair_history " +
                " WHERE id in @ids ",new { ids});
                return result;
            });
        }

        public async Task<EquipmentRepairHistoryView> GetPageByParm(EquipmentRepairHistoryQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                EquipmentRepairHistoryView erhv= new EquipmentRepairHistoryView();
                erhv.rows = new List<EquipmentRepairHistory>();
                erhv.total = 0;
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,e.eqp_name,u1.user_name as created_name,")
                .Append("e.eqp_code,u2.user_name as updated_name,dt.name as pmName,dt1.name as rName ")
                .Append(" FROM equipment_repair_history a ")
                .Append(" left join equipment e on a.eqp=e.id ")
                .Append(" left join dictionary_tree dt on dt.id=a.pm_type ")
                .Append(" left join dictionary_tree dt1 on dt1.id=a.replace_type ")
                //.Append(" left join trouble_report t on a.trouble=t.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                //whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (parm.PMType!=null)
                {
                    whereSql.Append(" and a.pm_type =" + parm.PMType);
                }
                if (parm.ReplaceType != null)
                {
                    whereSql.Append(" and a.replace_type =" + parm.ReplaceType);
                }
                if (parm.Eqp != null)
                {
                    whereSql.Append(" and a.eqp=" + parm.Eqp);
                }
                if (!string.IsNullOrWhiteSpace(parm.Desc))
                {
                    whereSql.Append(" and a.desc like '%" + parm.Desc + "%' ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                erhv.total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from equipment_repair_history a where 1=1 " + whereSql.ToString());
                if (erhv.total>0)
                {
                    erhv.rows= (await c.QueryAsync<EquipmentRepairHistory>(sql.ToString())).ToList();
                }
                return erhv;
            });
        }

        public async Task<EquipmentRepairHistory> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<EquipmentRepairHistory>(
                    "SELECT * FROM equipment_repair_history WHERE id = @id", new { id = id });
                return result;
            });
        }
    }
}
