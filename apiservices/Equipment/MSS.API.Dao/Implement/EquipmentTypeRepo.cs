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
    public class EquipmentTypeRepo : BaseRepo, IEquipmentTypeRepo<EquipmentType>
    {
        public EquipmentTypeRepo(DapperOptions options) : base(options) { }

        public async Task<EquipmentType> Save(EquipmentType eqpType)
        {
            return await WithConnection(async c =>
            {
                string sql;
                IDbTransaction trans = c.BeginTransaction();
                try
                {
                    sql = " insert into equipment_type " +
                        " values (0,@TName,@Model,@Desc, " +
                        " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                    sql += "SELECT LAST_INSERT_ID()";
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, eqpType, trans);
                    eqpType.ID = newid;
                    if (!string.IsNullOrWhiteSpace(eqpType.UploadFiles))
                    {
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(eqpType.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                objs.Add(new
                                {
                                    eqpTypeID = newid,
                                    fileID = Convert.ToInt32(item),
                                    type = Convert.ToInt32(obj["type"]),
                                    systemResource = (int)SystemResource.EqpType
                                });
                            }
                        }
                        sql = "insert into upload_file_relation values (0,@eqpTypeID,@fileID,@type,@systemResource)";
                        int ret = await c.ExecuteAsync(sql, objs, trans);
                    }
                    trans.Commit();
                    return eqpType;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            });
        }

        public async Task<int> Update(EquipmentType eqpType)
        {
            return await WithConnection(async c =>
            {
            string sql;
            IDbTransaction trans = c.BeginTransaction();
                try
                {
                    var result = await c.ExecuteAsync(" update equipment_type " +
                        " set type_name=@TName,model=@Model,description=@Desc, " +
                        " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", eqpType,trans);
                    if (!string.IsNullOrWhiteSpace(eqpType.UploadFiles))
                    {
                        sql = "delete from upload_file_relation where entity_id=@id";
                        int ret = await c.ExecuteAsync(sql, new { id = eqpType.ID }, trans);
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(eqpType.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                objs.Add(new
                                {
                                    eqpTypeID = eqpType.ID,
                                    fileID = Convert.ToInt32(item),
                                    type = Convert.ToInt32(obj["type"]),
                                    systemResource = (int)SystemResource.EqpType
                                });
                            }
                        }
                        sql = "insert into upload_file_relation values (0,@eqpTypeID,@fileID,@type,@systemResource)";
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

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update equipment_type set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<EqpTypeView> GetPageByParm(EqpTypeQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM equipment_type a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (!string.IsNullOrWhiteSpace(parm.SearchName))
                {
                    whereSql.Append(" and a.type_name like '%" + parm.SearchName + "%' ");
                }
                if (!string.IsNullOrWhiteSpace(parm.SearchDesc))
                {
                    whereSql.Append(" and a.description like '%" + parm.SearchDesc + "%' ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                EqpTypeView ret = new EqpTypeView();
                ret.rows= (await c.QueryAsync<EquipmentType>(sql.ToString())).ToList();
                ret.total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from equipment_type a " + whereSql.ToString());
                return ret;
            });
        }

        public async Task<EquipmentType> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<EquipmentType>(
                    "SELECT * FROM equipment_type WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<EquipmentType>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<EquipmentType>(
                    "SELECT * FROM equipment_type where is_del =" +(int)IsDeleted.no)).ToList();
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
