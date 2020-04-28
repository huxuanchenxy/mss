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
                sql.Append(", 'split', b.*,d.id major_id,d.name major_name ");
                sql.Append(" FROM statistics_alarm a");
                sql.Append(" JOIN statistics_dimension b ON a.eqp_id = b.eqp_id ");
                sql.Append(" left join dictionary_tree c on c.id = b.sub_system_id ");
                sql.Append(" left join dictionary_tree d on c.ext = d.id ");
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
                    // string[] local = param.OrgPath.Split(',');
                    // for (int i = 0; i < local.Length; ++i)
                    // {
                    //     sql.Append(" AND FIND_IN_SET(" + local[i] + ",b.team_path)="
                    //     + (i + 1));
                    // }
                    string[] local = param.OrgPath.Split(',');
                    string id = local[local.Length - 1];
                    sql.Append(" AND FIND_IN_SET(" + id + ",b.team_path) > 0");
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

        public async Task<List<StatisticsAlarm>> ListStatisticsAlarmDetail(StatisticsParam param)
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
                    string id = local[local.Length - 1];
                    sql.Append(" AND FIND_IN_SET(" + id + ",b.team_path) > 0");
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

        // 故障统计

        public async Task<List<StatisticsTrouble>> ListStatisticsTrouble(StatisticsTroubleParam param,
            List<string> groupby, int dateType)
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

                sql.Append(",c.name as troublename, 'split', b.*");
                sql.Append(" FROM statistics_trouble a");
                sql.Append(" JOIN statistics_dimension b ON a.eqp_id = b.eqp_id");
                sql.Append(" JOIN dictionary_tree c ON a.trouble_type = c.id");
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
                    string id = local[local.Length -1 ];
                    // for (int i = 0; i < local.Length; ++i)
                    // {
                    //     sql.Append(" AND FIND_IN_SET(" + local[i] + ",b.team_path)="
                    //     + (i + 1));
                    // }
                    sql.Append(" AND FIND_IN_SET(" + id + ",b.team_path) > 0");
                }

                if (!string.IsNullOrWhiteSpace(param.TroubleTypes))
                {
                    sql.Append(" AND a.trouble_type IN (" + param.TroubleTypes + ")");
                }

                for (int i = 0; i < groupby.Count; ++i)
                {
                    string by = "b." + groupby[i];
                    if (groupby[i].Equals("date"))
                    {
                        by = "date";
                    }
                    if (groupby[i].Equals("trouble_type"))
                    {
                        by = "a.trouble_type";
                    }
                    if (i == 0)
                    {
                        sql.Append(" GROUP BY " + by);
                        
                    }
                    else
                    {
                        sql.Append(", " + by);
                    }
                }


                var data = await c.QueryAsync<StatisticsTrouble, StatisticsDimension, StatisticsTrouble>(
                        sql.ToString(), (trouble, dimension) =>
                        {
                            trouble.dimension = dimension;
                            return trouble;
                        },
                        splitOn: "split");
                return data.ToList();
            });
        }

        public async Task<List<StatisticsTrouble>> ListStatisticsTroubleDetail(StatisticsTroubleParam param)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("SELECT a.*, c.name as troublename, 'split', b.*");
                sql.Append(" FROM statistics_trouble a");
                sql.Append(" JOIN statistics_dimension b ON a.eqp_id = b.eqp_id");
                sql.Append(" JOIN dictionary_tree c ON a.trouble_type = c.id");
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
                    string id = local[local.Length - 1];
                    sql.Append(" AND FIND_IN_SET(" + id + ",b.team_path) > 0");
                }

                if (!string.IsNullOrWhiteSpace(param.TroubleTypes))
                {
                    sql.Append(" AND a.trouble_type IN (" + param.TroubleTypes + ")");
                }

                // var data = await c.QueryAsync<StatisticsAlarm>(sql.ToString());
                var data = await c.QueryAsync<StatisticsTrouble, StatisticsDimension, StatisticsTrouble>(
                        sql.ToString(), (trouble, dimension) =>
                        {
                            trouble.dimension = dimension;
                            return trouble;
                        },
                        splitOn: "split");
                return data.ToList();
            });
        }

        public async Task<StatisticsTrouble> AddTrouble(StatisticsTrouble trouble)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO statistics_trouble (eqp_id, occur_time, recover_time, elapsed_time, trouble_id, trouble_type)"
                            + " Values (@EqpID, @OccurTime, @RecoverTime, @ElapsedTime, @TroubleID, @TroubleType);";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, trouble);
                trouble.ID = newid;
                return trouble;
            });
        }

        public async Task<List<StatisticsTroubleRank>> GetStatisticsTroubleRank()
        {
            return await WithConnection(async c =>
            {
                string sql = $@"SELECT count(b.AreaName) troublecount,b.AreaName name
                                FROM trouble_report a
                                left join tb_config_bigarea b
                                on a.start_location = b.id and b.ConfigType = 9
                                GROUP BY b.AreaName" ;
                var data = await c.QueryAsync<StatisticsTroubleRank>(sql);
                return data.ToList().OrderByDescending(c1=>c1.troublecount).ToList();
            });
        }

        public async Task<List<CostChart>> GetCostChart(CostChartParm parm)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" SELECT cost_name cost_name,cost_type cost_type,sum(value) value,year
                                    FROM `cost_chart`
                                    WHERE year >= '{parm.startYear}' and year <= '{parm.endYear}'
                                    GROUP BY year,cost_type,cost_name ";
                var data = await c.QueryAsync<CostChart>(sql);
                return data.ToList().OrderBy(c1 => c1.Year).ToList();
            });
        }
    }    
}
