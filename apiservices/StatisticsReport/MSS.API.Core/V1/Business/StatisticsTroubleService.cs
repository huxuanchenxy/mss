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
namespace MSS.API.Core.V1.Business
{
    public class StatisticsTroubleService : IStatisticsTroubleService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IStatisticsRepo _statisticsRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;

        public StatisticsTroubleService(IStatisticsRepo statisticsRepo, IServiceDiscoveryProvider consulServiceProvider)
        {
            _statisticsRepo = statisticsRepo;
            _consulServiceProvider = consulServiceProvider;
        }

        public async Task<ApiResult> ListStatisticsTroubleGroupByDate(StatisticsTroubleParam param,
            List<string> groupby, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                groupby.Insert(0, "date");
                List<StatisticsTrouble> data = await _statisticsRepo.ListStatisticsTrouble(param,
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

        public async Task<ApiResult> ListStatisticsTroubleGroupByOther(StatisticsTroubleParam param,
            List<string> groupby, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsTrouble> data = await _statisticsRepo.ListStatisticsTrouble(param,
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

        public async Task<ApiResult> ListStatisticsTroubleGroupByLocation(StatisticsTroubleParam param, int dateType)
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
                List<StatisticsTrouble> data = await _statisticsRepo.ListStatisticsTrouble(param,
                    new List<string> { groupby }, dateType);

                // 统计当前位置 子级各站数据时，如子级id为0，标识此设备属于当前位置。
                if (groupby.Equals("location_level3"))
                {
                    foreach (StatisticsTrouble alarm in data)
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
                    foreach (StatisticsTrouble alarm in data)
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

        public async Task<ApiResult> ListStatisticsTroubleGroupByOrg(StatisticsTroubleParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsTrouble> data = await _statisticsRepo.ListStatisticsTroubleDetail(param);
                List<StatisticsTrouble> groupResult = new List<StatisticsTrouble>();

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
                    foreach (StatisticsTrouble eqp in data)
                    {
                        OrgNodeView parent = _getParentNode(eqp.dimension.TeamID, org[0].children);
                        eqp.dimension.TeamID = parent.id;
                        eqp.dimension.TeamName = parent.label;
                    }

                    
                    IEnumerable<IGrouping<int, StatisticsTrouble>> groups = data.GroupBy(c => c.dimension.TeamID);
                    foreach (IGrouping<int, StatisticsTrouble> group in groups)
                    {
                        StatisticsTrouble first = group.First();
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

        public async Task<ApiResult> AddTrouble(StatisticsTrouble trouble)
        {
            ApiResult ret = new ApiResult();
            try
            {
                StatisticsTrouble topNode = await _statisticsRepo.AddTrouble(trouble);

                ret.code = Code.Success;
                ret.data = topNode;
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
