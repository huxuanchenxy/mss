using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Model.DTO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;
using MSS.API.Core.Common;
using Microsoft.Extensions.Caching.Distributed;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using Microsoft.Extensions.Configuration;

namespace MSS.API.Core.V1.Business
{
    public class StatisticsService : IStatisticsService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IStatisticsRepo _statisticsRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly IConfiguration _config;

        public StatisticsService(IStatisticsRepo statisticsRepo, IServiceDiscoveryProvider consulServiceProvider, IConfiguration config)
        {
            _statisticsRepo = statisticsRepo;
            _consulServiceProvider = consulServiceProvider;
            _config = config;
        }

        public async Task<ApiResult> ListStatisticsAlarm(StatisticsParam param,
            List<string> groupby, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    groupby, dateType);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListStatisticsAlarmGroupByEqpType(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "eqp_type_id" }, dateType, false);
                
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListStatisticsAlarmGroupBySupplier(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "supplier_id" }, dateType, false);

                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListStatisticsAlarmGroupByManufacturer(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "manufacturer_id" }, dateType, false);

                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListStatisticsAlarmGroupBySubSystem(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "sub_system_id" }, dateType, false);

                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListStatisticsAlarmGroupByLocation(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                string groupby = "";
                string groupbyName = "";
                if (!string.IsNullOrEmpty(param.LocationLevel1s))
                {
                    groupby = "location_level2";
                    groupbyName = "locationLevel2Name";
                }
                if (!string.IsNullOrEmpty(param.LocationLevel2s))
                {
                    groupby = "location_level3";
                    groupbyName = "locationLevel3Name";
                }
                if (!string.IsNullOrEmpty(param.LocationLevel3s))
                {
                    groupby = "location_level3";
                    groupbyName = "locationLevel3Name";
                }
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { groupby }, dateType, false);

                // 统计当前位置 子级各站数据时，如子级id为0，标识此设备属于当前位置。
                if (groupby.Equals("location_level3"))
                {
                    foreach (StatisticsAlarm alarm in data)
                    {
                        if (alarm.dimension.LocationLevel3 == 0)
                        {
                            alarm.dimension.LocationLevel3Name = alarm.dimension.LocationLevel2Name;
                        }
                    }
                }
                if (groupby.Equals("location_level2"))
                {
                    // data = data.Where(c => c.dimension.LocationLevel2 != 0).ToList();
                    foreach (StatisticsAlarm alarm in data)
                    {
                        if (alarm.dimension.LocationLevel2 == 0)
                        {
                            alarm.dimension.LocationLevel2Name = alarm.dimension.LocationLevel1Name;
                        }
                    }
                }
                   
                ret.code = Code.Success;
                ret.data = new {
                    data = data,
                    groupby = groupbyName
                };
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListStatisticsAlarmGroupByOrg(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmDetail(param);
                List<StatisticsAlarm> groupResult = new List<StatisticsAlarm>();

                var services = await _consulServiceProvider.GetServiceAsync("OrgService");
                string[] path = param.OrgPath.Split(',');
                string orgid = path[path.Count() - 1];
                ApiResult result = HttpClientHelper.GetResponse<ApiResult>(services
                    + "/api/v1/org/" + orgid);
                if (result.code == Code.Success)
                {
                    List<OrgNodeView> org = JsonConvert.DeserializeObject<List<OrgNodeView>>(
                        result.data.ToString());
                    // 循环设备，定位设备属于哪个分组
                    foreach (StatisticsAlarm eqp in data)
                    {
                        OrgNodeView parent = _getParentNode(eqp.dimension.TeamID, org[0].children);
                        eqp.dimension.TeamID = parent.id;
                        eqp.dimension.TeamName = parent.label;
                    }

                    
                    IEnumerable<IGrouping<int, StatisticsAlarm>> groups = data.GroupBy(c => c.dimension.TeamID);
                    foreach (IGrouping<int, StatisticsAlarm> group in groups)
                    {
                        StatisticsAlarm first = group.First();
                        first.num = group.Count().ToString();
                        first.avgtime = (group.Sum(item => item.ElapsedTime)/group.Count()).ToString();
                        groupResult.Add(first);
                    }
                }

                

                ret.code = Code.Success;
                ret.data = new
                {
                    data = groupResult,
                    groupby = "teamName"
                };
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        private OrgNodeView _getParentNode(int teamId, List<OrgNodeView> nodes)
        {
            OrgNodeView parentNode = null;
            foreach (OrgNodeView node in nodes)
            {
                if (node.id == teamId)
                {
                    parentNode = node;
                    break;
                }
                else if (node.children != null && node.children.Count > 0)
                {
                    bool ret = _isInChild(teamId, node.children);
                    if (ret)
                    {
                        parentNode = node;
                        break;
                    }
                }
            }
            return parentNode;
        }

        private bool _isInChild (int teamId, List<OrgNodeView> nodes)
        {
            bool ret = false;
            foreach (OrgNodeView node in nodes)
            {
                if (node.id == teamId)
                {
                    ret = true;
                    break;
                }
                else if (node.children != null && node.children.Count > 0)
                {
                    ret = _isInChild(teamId, node.children);
                    if (ret)
                    {
                        break;
                    }
                }
            }
            return ret;
        }

        public async Task<ApiResult> GetStatisticsTroubleRank()
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _statisticsRepo.GetStatisticsTroubleRank();
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetRunningtime()
        {
            ApiResult ret = new ApiResult();
            try
            {
                string starttime = _config["statisticsStartTime"];
                double rettime = (DateTime.Now - Convert.ToDateTime(starttime)).TotalMinutes;
                var data = Math.Round(rettime,0);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> GetNow()
        {
            ApiResult ret = new ApiResult();
            try
            {
                RangeTime rt = new RangeTime();
                string d = _config["statisticsNow"];
                if (string.IsNullOrEmpty(d))
                {
                    d = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else
                {
                    d = d + " 23:59:59";
                }

                string startdate = Convert.ToDateTime(d).AddMonths(-6).ToString("yyyy-MM-01");
                rt.startTime = startdate;
                rt.endTime = d;
                ret.code = Code.Success;
                ret.data = rt;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }



        public async Task<ApiResult> GetIndexProcess()
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsResult> result = new List<StatisticsResult>();
                float d1 =  (float)DateTime.Now.Month / (float)12;
                int _count = (int)(Math.Round(d1, 2) * 100);
                result.Add(new StatisticsResult() { count = _count, name = "本年时间进度" });
                //年进度
                int _countyear = 0;
                var services = await _consulServiceProvider.GetServiceAsync("WorkflowWebApiService");
                ApiResult r1 = HttpClientHelper.GetResponse<ApiResult>(services
                    + "/api/v1/ConstructionPlanMonthChart/GetMonthChart?xAxisType=3&startYear=" + DateTime.Now.Year + "&endYear=" + DateTime.Now.Year);
                if (r1.code == Code.Success && r1.data != null)
                {
                    dynamic dd = r1.data;
                    try
                    {
                        dynamic series = dd.series[0];
                        _countyear = series.data[0];
                    }
                    catch (Exception ex) { }
                    result.Add(new StatisticsResult() { count = _countyear, name = "年计划完成率" });
                }
                //月进度
                int _countmonth = 0;
                ApiResult r2 = HttpClientHelper.GetResponse<ApiResult>(services
                    + "/api/v1/ConstructionPlanMonthChart/GetMonthChart?xAxisType=2&year=" + DateTime.Now.Year+"&startMonth="+ DateTime.Now.Month + "&endMonth="+ DateTime.Now.Month);
                if (r2.code == Code.Success && r2.data != null)
                {
                    dynamic dd = r2.data;
                    try
                    {
                        dynamic series = dd.series[0];
                        _countmonth = series.data[0];
                    }
                    catch (Exception ex) { }
                    result.Add(new StatisticsResult() { count = _countmonth, name = "月计划完成率" });
                }
                ret.code = Code.Success;
                ret.data = result;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

    }
}
