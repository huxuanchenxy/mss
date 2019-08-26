using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Dapper.FastCrud;
using MSS.API.Model.DTO;
using MSS.API.Dao.Common;

namespace MSS.API.Dao.Implement
{
    public class StatisticsRepo : BaseRepo, IStatisticsRepo
    {
        public StatisticsRepo(DapperOptions options) : base(options) { }

        public async Task<List<StatisticsAlarm>> ListStatisticsAlarmByDate(StatisticsParam param,
            List<string> groupby, int dateType, bool defaultByDate = true)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                string datecol = "DATE_FORMAT(a.occur_time,'%Y-%m-%d')";
                if (dateType == 1)
                {
                    datecol = "DATE_FORMAT(a.occur_time,'%Y-%m')";
                }
                sql.Append("SELECT " + datecol + " as date, count(*) as num, avg(a.elapsed_time) avgtime");
                // foreach (string item in groupby)
                // {
                //     sql.Append(", b." + item);
                // }
                sql.Append(", 'split', b.*");
                sql.Append(" FROM statistics_alarm a");
                sql.Append(" JOIN statistics_dimension b ON a.eqp_id = b.eqp_id");
                sql.Append(" WHERE 1=1");
                if (param.StartTime != null)
                {
                    sql.Append(" AND a.occur_time >= '" + param.StartTime + "'");
                }
                if (param.EndTime != null)
                {
                    sql.Append(" AND a.occur_time <= '" + param.EndTime + "'");
                }
                if (!string.IsNullOrWhiteSpace(param.EqpTypeIDs))
                {
                    sql.Append(" AND b.eqp_type_id IN (" + param.EqpTypeIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.LocationLevel1s))
                {
                    sql.Append(" AND b.location_level1 IN (" + param.LocationLevel1s + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.LocationLevel2s))
                {
                    sql.Append(" AND b.location_level2 IN (" + param.LocationLevel2s + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.LocationLevel3s))
                {
                    sql.Append(" AND b.location_level3 IN (" + param.LocationLevel3s + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.ManufacturerIDs))
                {
                    sql.Append(" AND b.manufacturer_id IN (" + param.ManufacturerIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.SupplierIDs))
                {
                    sql.Append(" AND b.supplier_id IN (" + param.SupplierIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.SubSystemIDs))
                {
                    sql.Append(" AND b.sub_system_id IN (" + param.SubSystemIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.TeamIDs))
                {
                    sql.Append(" AND b.team_id IN (" + param.TeamIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.TopOrgIDs))
                {
                    sql.Append(" AND b.top_org_id IN (" + param.TopOrgIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.OrgPath))
                {
                    string[] local = param.OrgPath.Split(',');
                    for (int i = 0; i < local.Length; ++i)
                    {
                        sql.Append(" AND FIND_IN_SET(" + local[i] + ",b.team_path)="
                        + (i + 1));
                    }
                }
                if (defaultByDate)
                {
                    sql.Append(" GROUP BY date");
                    foreach (string item in groupby)
                    {
                        sql.Append(", b." + item);
                    }
                }
                else
                {
                    for (int i = 0; i < groupby.Count; ++i)
                    {
                        if (i == 0)
                        {
                            sql.Append(" GROUP BY b." + groupby[i]);
                        }
                        else
                        {
                            sql.Append(", b." + groupby[i]);
                        }
                    }
                }
                
                
                // var data = await c.QueryAsync<StatisticsAlarm>(sql.ToString());
                var data = await c.QueryAsync<StatisticsAlarm, StatisticsDimension, StatisticsAlarm>(
                        sql.ToString(), (alarm, dimension) =>
                        {
                            alarm.dimension = dimension;
                            return alarm;
                        },
                        splitOn: "split");
                return data.ToList();
            });
        }

        public async Task<List<StatisticsAlarm>> ListStatisticsAlarm(StatisticsParam param)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                
                sql.Append("SELECT a.*, 'split', b.*");
                sql.Append(" FROM statistics_alarm a");
                sql.Append(" JOIN statistics_dimension b ON a.eqp_id = b.eqp_id");
                sql.Append(" WHERE 1=1");
                if (param.StartTime != null)
                {
                    sql.Append(" AND a.occur_time >= '" + param.StartTime + "'");
                }
                if (param.EndTime != null)
                {
                    sql.Append(" AND a.occur_time <= '" + param.EndTime + "'");
                }
                if (!string.IsNullOrWhiteSpace(param.EqpTypeIDs))
                {
                    sql.Append(" AND b.eqp_type_id IN (" + param.EqpTypeIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.LocationLevel1s))
                {
                    sql.Append(" AND b.location_level1 IN (" + param.LocationLevel1s + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.LocationLevel2s))
                {
                    sql.Append(" AND b.location_level2 IN (" + param.LocationLevel2s + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.LocationLevel3s))
                {
                    sql.Append(" AND b.location_level3 IN (" + param.LocationLevel3s + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.ManufacturerIDs))
                {
                    sql.Append(" AND b.manufacturer_id IN (" + param.ManufacturerIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.SupplierIDs))
                {
                    sql.Append(" AND b.supplier_id IN (" + param.SupplierIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.SubSystemIDs))
                {
                    sql.Append(" AND b.sub_system_id IN (" + param.SubSystemIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.TeamIDs))
                {
                    sql.Append(" AND b.team_id IN (" + param.TeamIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.TopOrgIDs))
                {
                    sql.Append(" AND b.top_org_id IN (" + param.TopOrgIDs + ")");
                }
                if (!string.IsNullOrWhiteSpace(param.OrgPath))
                {
                    string[] local = param.OrgPath.Split(',');
                    for (int i = 0; i < local.Length; ++i)
                    {
                        sql.Append(" AND FIND_IN_SET(" + local[i] + ",b.team_path)="
                        + (i + 1));
                    }
                }

                // var data = await c.QueryAsync<StatisticsAlarm>(sql.ToString());
                var data = await c.QueryAsync<StatisticsAlarm, StatisticsDimension, StatisticsAlarm>(
                        sql.ToString(), (alarm, dimension) =>
                        {
                            alarm.dimension = dimension;
                            return alarm;
                        },
                        splitOn: "split");
                return data.ToList();
            });
        }
    }    
}
