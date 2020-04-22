using Dapper;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Implement
{
    public class LifeTimeKeyMaintainRepo : BaseRepo, ILifeTimeKeyMaintainRepo<LifeTimeKeyMaintainInfo>
    {
        public LifeTimeKeyMaintainRepo(DapperOptions options) : base(options)
        {
        }
        public async Task<List<LifeTimeKeyMaintainInfo>> GetListByPage(string strWhere, string orderby, string sort, int page, int size)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.id as device_id ,a.eqp_type as device_type, a.eqp_code as device_code, a.eqp_name as device_name,a.team as team_group_id ," +
                        " o.name as team_group_name," +
                        " a.online_date as setup_date, DATE_ADD(a.online_date, INTERVAL life year) as life_time," +
                        " '正常' as cur_state," +
                        " a.updated_time," +
                        " u.user_name as updated_name," +
                        " a.Updated_By," +
                        " IsDaXiuOrZhongXiu(a.large_repair, a.medium_repair, a.online_date, a.online_again, a.life) as next_maintain_type," +
                        " Next_Maintain_Date(a.large_repair, a.medium_repair, a.online_date, a.online_again, a.life) as next_maintain_date" +
                        " FROM  equipment a join user u on a.Updated_By = u.id" +
                        " JOIN org_tree o on a.team = o.ID " +
                        " where    a.Is_del!= 1 and " + strWhere);
                //join user u on a.updated_by=u.id  
                if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(orderby))
                {
                    sql.Append(" order by a." + sort + " " + orderby);
                }
                sql.Append(" limit " + (page - 1) * size + "," + size);

                var list = await c.QueryAsync<LifeTimeKeyMaintainInfo>(sql.ToString());
                return list.ToList();
            });
        }

        public async Task<List<LocationDeviceInfo>>  GetlocationList()
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select  " +
                          "  case a.ConfigType  when 9 then '车站'" +
                          "    when 10 then '正线轨行区'" +
                          "    when 11 then '保护区'" +
                          "    when 12 then '车场生产区'" +
                          "    end as ConfigType, " +
                          "    a.ConfigType as ConfigTypeID,a.MetroLineID, " +
                          "    a.AreaName as StationName,  a.id as StationID, b.AreaName as LocationName, b.id as LocationID  from tb_config_bigarea a left" +
                          "    join tb_config_midarea b on a.id = b.pid   order by ConfigTypeID asc "); 
                var list = await c.QueryAsync<LocationDeviceInfo>(sql.ToString());
                return list.ToList();
            });
        }

        public async Task<List<LocatioInfo>> GetdeviceList(int level,int location,string strWhere)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(" select id as ID, eqp_name as Name  from equipment where getLevel(location_path) = {0} and location = {1} " + strWhere, level, location); 
                var list = await c.QueryAsync<LocatioInfo>(sql.ToString());
                return list.ToList();
            });
        }

        public async Task<List<Equipment>> ListEqpAllByCond(int? topOrg, int eqpType, int line)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select * from equipment where is_del=0 ");
                object o = new { eqpType, line };
                if (topOrg != null)
                {
                    sql.Append(" and top_org="+topOrg);
                }
                if (eqpType != 0)
                {
                    sql.Append(" and eqp_type=" + eqpType);
                }
                if (line != 0)
                {
                    sql.Append(" and line=" + line);
                }
                var list = await c.QueryAsync<Equipment>(sql.ToString());
                return list.ToList();
            });
        }

        public async Task<EquipmntView> GetListByPage(LifeTimeKeyMaintainQurey parm)
        {
            return await WithConnection(async c =>
            {
                EquipmntView ret = new EquipmntView();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,ot.name ")
                .Append(" FROM equipment a ")
                .Append(" left join org_tree ot on ot.id=a.team where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                //whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (parm.device_type != null)
                {
                    whereSql.Append(" and a.eqp_type =" + parm.device_type);
                }
                if (parm.device_id != null)
                {
                    whereSql.Append(" and a.id =" + parm.device_id);
                }
                if (parm.StartDate != null)
                {
                    whereSql.Append(" and (a.next_medium_repair_date >=" + parm.StartDate)
                    .Append(" or a.next_large_repair_date >=" + parm.StartDate+") ");
                }
                if (parm.EndDate != null)
                {
                    whereSql.Append(" and (a.next_medium_repair_date <=" + parm.EndDate)
                    .Append(" or a.next_large_repair_date <=" + parm.EndDate + ") ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                ret.Total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from equipment a where 1=1 " + whereSql.ToString());
                if (ret.Total > 0)
                {
                    ret.Rows = (await c.QueryAsync<Equipment>(sql.ToString())).ToList();
                }
                return ret;
            });
        }
    }
}