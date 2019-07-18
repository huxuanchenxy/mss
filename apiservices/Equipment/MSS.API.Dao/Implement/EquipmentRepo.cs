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
using static MSS.API.Common.MyDictionary;
using System.Data;

namespace MSS.API.Dao.Implement
{
    public class EquipmentRepo : BaseRepo, IEquipmentRepo<Equipment>
    {
        public EquipmentRepo(DapperOptions options) : base(options) { }

        public async Task<Equipment> Save(Equipment eqp)
        {
            return await WithConnection(async c =>
            {
                string sql;
                IDbTransaction trans = c.BeginTransaction();
                try
                {
                    sql = " insert into equipment " +
                        " values (0,@Code,@Name,@Type,@AssetNo,@Model, " +
                        " @SubSystem,@Team,@TeamPath,@TopOrg,@BarCode,@Desc,@Supplier,@Manufacturer, " +
                        " @SerialNo,@RatedVoltage,@RatedCurrent,@RatedPower, " +
                        " @Location,@LocationBy,@LocationPath,@Online,@Life, " +
                        " @MediumRepair,@LargeRepair,@OnlineAgain, " +
                        " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                    sql += "SELECT LAST_INSERT_ID()";
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, eqp,trans);
                    eqp.ID = newid;
                    if (!string.IsNullOrWhiteSpace(eqp.FileIDs))
                    {
                        List<object> objs = new List<object>();
                        foreach (var item in eqp.FileIDs.Split(','))
                        {
                            objs.Add(new
                            {
                                eqpID = newid,
                                fileID = Convert.ToInt32(item)
                            });
                        }
                        sql = "insert into upload_file_eqp values (0,@eqpID,@fileID)";
                        int ret = await c.ExecuteAsync(sql, objs, trans);
                    }
                    trans.Commit();
                    return eqp;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            });
        }

        public async Task<int> Update(Equipment eqp)
        {
            return await WithConnection(async c =>
            {
                IDbTransaction trans = c.BeginTransaction();
                try
                {
                    var result = await c.ExecuteAsync(" update equipment " +
                        " set eqp_code=@Code,eqp_name=@Name,eqp_type=@Type,eqp_asset_number=@AssetNo, " +
                        " eqp_model=@Model,sub_system=@SubSystem,team=@Team,team_path=@TeamPath,bar_code=@BarCode, " +
                        " discription=@Desc,supplier=@Supplier,manufacturer=@Manufacturer,serial_number=@SerialNo, " +
                        " rated_voltage=@RatedVoltage,rated_current=@RatedCurrent,team=@Team,rated_power=@RatedPower, " +
                        " location=@Location,location_by=@LocationBy,location_path=@LocationPath,top_org@TopOrg, " +
                        " online_date=@Online,life=@Life,medium_repair=@MediumRepair,large_repair=@LargeRepair, " +
                        " online_again=@OnlineAgain,updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", eqp,trans);
                    if (!string.IsNullOrWhiteSpace(eqp.FileIDs))
                    {
                        string sql = "delete from upload_file_eqp where eqp_id=@id";
                        int ret = await c.ExecuteAsync(sql, new { id = eqp.ID }, trans);
                        List<object> objs = new List<object>();
                        foreach (var item in eqp.FileIDs.Split(','))
                        {
                            objs.Add(new
                            {
                                eqpID = eqp.ID,
                                fileID = Convert.ToInt32(item)
                            });
                        }
                        sql = "insert into upload_file_eqp values (0,@eqpID,@fileID)";
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
                var result = await c.ExecuteAsync(" Update equipment set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<EqpView> GetPageByParm(EqpQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT distinct a.*,u1.user_name as created_name,u2.user_name as updated_name, ")
                .Append(" et.type_name,d.sub_code_name,ot.name,f1.name as supplierName,f2.name as manufacturerName ")
                .Append(" FROM (equipment a,dictionary d) ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ")
                .Append(" left join equipment_type et on et.id=a.eqp_type ")
                .Append(" left join org_tree ot on ot.id=a.team ")
                .Append(" left join firm f1 on f1.id=a.Supplier ")
                .Append(" left join firm f2 on f2.id=a.Manufacturer ")
                .Append(" where d.sub_code=a.sub_system and d.code='"+ STR_SUB_SYSTEM + "' and ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" a.is_del=" + (int)IsDeleted.no);
                if (parm.SearchSubSystem!=null)
                {
                    whereSql.Append(" and a.sub_system =" + parm.SearchSubSystem);
                }
                if (parm.SearchType!=null)
                {
                    whereSql.Append(" and a.eqp_type =" + parm.SearchType);
                }
                if (!string.IsNullOrWhiteSpace(parm.SearchCode))
                {
                    whereSql.Append(" and a.eqp_code like '%" + parm.SearchType + "%' ");
                }
                if (parm.SearchLocation != null && parm.SearchLocationBy!=null)
                {
                    whereSql.Append(" and a.location=" + parm.SearchLocation)
                    .Append(" and a.location_by =" + parm.SearchLocationBy);
                }
                if (parm.SearchTopOrg!=null)
                {
                    whereSql.Append(" and a.top_org=" + parm.SearchType);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< Equipment > ets= (await c.QueryAsync<Equipment>(sql.ToString())).ToList();
                sql.Clear();
                sql.Append("select count(*) FROM equipment a where ");
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    sql.ToString() + whereSql.ToString());
                EqpView ret = new EqpView();
                ret.rows = ets;
                ret.total = total;
                return ret;
            });
        }

        public async Task<Equipment> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<Equipment>(
                    "SELECT * FROM equipment WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<Equipment> GetDetailByID(int id)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT distinct a.*,u1.user_name as created_name,u2.user_name as updated_name, ")
                .Append(" et.type_name,d.sub_code_name,ot.name,f1.name as supplierName,f2.name as manufacturerName ")
                .Append(" FROM (equipment a,dictionary d) ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ")
                .Append(" left join equipment_type et on et.id=a.eqp_type ")
                .Append(" left join org_tree ot on ot.id=a.team ")
                .Append(" left join firm f1 on f1.id=a.Supplier ")
                .Append(" left join firm f2 on f2.id=a.Manufacturer ")
                .Append(" where d.sub_code=a.sub_system and d.code='" + STR_SUB_SYSTEM + "' and ")
                .Append(" a.is_del=" + (int)IsDeleted.no+ " and a.id="+id);
                return await c.QueryFirstOrDefaultAsync<Equipment>(sql.ToString());
            });
        }


        public async Task<List<UploadFileEqp>> ListByEqp(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<UploadFileEqp>(
                    "SELECT * FROM upload_file_eqp WHERE eqp_id = @id", new { id = id });
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
        public async Task<List<Equipment>> ListByPosition(int location,int locationBy, int eqpType,int? topOrg)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<Equipment>(
                    "SELECT * FROM Equipment " +
                    "WHERE location = @location and location_by=@locationBy " +
                    "and eqp_type=@eqpType and top_org=@topOrg", 
                    new { location = location, locationBy= locationBy, eqpType= eqpType, topOrg= topOrg });
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


        public async Task<List<Equipment>> ListByEqpType(string ids)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<Equipment>(
                    "SELECT * FROM equipment WHERE eqp_type in @ids", new { ids = ids });
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

        public async Task<List<Equipment>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<Equipment>(
                    "SELECT * FROM equipment"));
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

        public async Task<List<AllArea>> GetAllArea()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sub_code_name as AreaName,sub_code as id,0 as TableName " +
                "from dictionary where code='metro_area' UNION " +
                "select AreaName,id,1 as TableName from tb_config_bigarea UNION " +
                "select AreaName,id,2 as TableName from tb_config_midarea";
                var result = await c.QueryAsync<AllArea>(sql);
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
