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

namespace MSS.API.Dao.Implement
{
    public class EquipmentRepo : BaseRepo, IEquipmentRepo<Equipment>
    {
        public EquipmentRepo(DapperOptions options) : base(options) { }

        public async Task<Equipment> Save(Equipment eqpType)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into equipment " +
                    " values (0,@Code,@Name,@Type,@AssetNo,@Model, " +
                    " @SubSystem,@Team,@TeamPath,@BarCode,@Desc,@Supplier,@Manufacturer, " +
                    " @SerialNo,@RatedVoltage,@RatedCurrent,@RatedPower, " +
                    " @Location,@LocationBy,@LocationPath,@Online,@Life,@PathPic, " +
                    " @MediumRepair,@LargeRepair,@OnlineAgain, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.ExecuteAsync(sql, eqpType);
                eqpType.ID = newid;
                return eqpType;
            });
        }

        public async Task<int> Update(Equipment eqpType)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update equipment " +
                    " set eqp_code=@Code,eqp_name=@Name,eqp_type=@Type,eqp_asset_number=@AssetNo, " +
                    " eqp_model=@Model,sub_system=@SubSystem,team=@Team,team_path=@TeamPath,bar_code=@BarCode, " +
                    " discription=@Desc,supplier=@Supplier,manufacturer=@Manufacturer,serial_number=@SerialNo, " +
                    " rated_voltage=@RatedVoltage,rated_current=@RatedCurrent,team=@Team,rated_power=@RatedPower, " +
                    " location=@Location,location_by=@LocationBy,location_path=@LocationPath,online_date=@Online,life=@Life, " +
                    " eqp_pic=@PathPic,medium_repair=@MediumRepair,large_repair=@LargeRepair,online_again=@OnlineAgain, " +
                    " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", eqpType);
                return result;
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
                List<Dictionary> dics = (await c.QueryAsync<Dictionary>("select * from Dictionary where code=@code", new { code = STR_AREA_TYPE })).ToList();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT distinct a.*,u1.user_name as created_name,u2.user_name as updated_name, ")
                .Append(" et.type_name,d.sub_code_name,ot.name,f1.name as sname,f2.name as mname ");
                if (parm.SearchLocationBy == (int)AREA_TYPE.dictionary)
                {
                    sql.Append(",area.sub_code_name as AreaName ")
                    .Append(" FROM (equipment a,dictionary d,dictionary area) ");
                }
                else if (parm.SearchLocationBy !=null)
                {
                    sql.Append(",area.AreaName ")
                    .Append(" FROM (equipment a,dictionary d) ");
                    string tbName = dics.Where(a => a.sub_code == parm.SearchLocationBy).FirstOrDefault().sub_code_name;
                    sql.Append(" left join "+ tbName + " area on area.id=a.location ");
                }
                else
                {
                    sql.Append(" FROM (equipment a,dictionary d) ");
                }
                sql.Append(" left join user u1 on a.created_by=u1.id ")
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
                if (parm.SearchLocation != null && parm.SearchLocationBy==(int)AREA_TYPE.dictionary)
                {
                    whereSql.Append(" and area.sub_code=a.location and area.code='" + STR_AREA_TYPE + "'")
                    .Append(" and area.sub_code =" + parm.SearchLocation);
                }
                else if (parm.SearchLocation != null && parm.SearchLocationBy != (int)AREA_TYPE.dictionary)
                {
                    whereSql.Append(" and area.id =" + parm.SearchLocation);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< Equipment > ets= (await c.QueryAsync<Equipment>(sql.ToString())).ToList();
                sql.Clear();
                sql.Append("select count(*) ");
                if (parm.SearchLocation != null && parm.SearchLocationBy == (int)AREA_TYPE.dictionary)
                {
                    sql.Append(" FROM (equipment a,dictionary area) ");
                }
                else if (parm.SearchLocationBy != null && parm.SearchLocationBy != (int)AREA_TYPE.dictionary)
                {
                    sql.Append("FROM equipment a ");
                    string tbName = dics.Where(a => a.sub_code == parm.SearchLocationBy).FirstOrDefault().sub_code_name;
                    sql.Append(" left join " + tbName + " area on area.id=a.location ");
                }
                else
                {
                    sql.Append(" FROM equipment a ");
                }
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    sql.ToString() + " where " + whereSql.ToString());
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

        public async Task<List<Equipment>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<Equipment>(
                    "SELECT * FROM equipment")).ToList();
                return result;
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
                var result = (await c.QueryAsync<AllArea>(sql)).ToList();
                return result;
            });
        }
    }
}
