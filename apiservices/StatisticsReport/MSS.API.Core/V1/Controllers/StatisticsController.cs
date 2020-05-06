using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using MSS.API.Core.Common;
using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.Common.Consul;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IStatisticsTroubleService _statisticsTroubleService;
        private readonly int _userId;
        private readonly IAuthHelper _authHelper;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        public StatisticsController(IStatisticsService statisticsService, 
            IStatisticsTroubleService statisticsTroubleService, IAuthHelper authHelper,
            IServiceDiscoveryProvider consulServiceProvider)

        {
            _statisticsService = statisticsService;
            _statisticsTroubleService = statisticsTroubleService;
            _consulServiceProvider = consulServiceProvider;
            _authHelper = authHelper;
            _userId = _authHelper.GetUserId();
        }

        private async Task<string> getTopOrgByUser()
        {
            var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
            IHttpClientHelper<ApiResult> h = new HttpClientHelper<ApiResult>();
            ApiResult r = await h.GetSingleItemRequest(_services + "/api/v1/user/" + _userId);
            JObject jobj = JsonConvert.DeserializeObject<JObject>(r.data.ToString());
            if ((bool)jobj["is_super"])
            {
                return null;
            }
            else
            {
                var _services1 = await _consulServiceProvider.GetServiceAsync("OrgService");
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.GetSingleItemRequest(_services1 + "/api/v1/org/topnode/" + _userId);
                if (result.data != null)
                {
                    JObject obj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    return obj["id"].ToString();
                }
                return "";
            }
        }

        private void SplitLocation(StatisticsParam param)
        {
            if (!string.IsNullOrEmpty(param.LocationPath))
            {
                string[] levels = param.LocationPath.Split(',');
                for (int i = 0; i < levels.Length; ++i)
                {
                    if (i == 0)
                    {
                        param.LocationLevel1s = levels[i];
                    }
                    if (i == 1)
                    {
                        param.LocationLevel2s = levels[i];
                    }
                    if (i == 2)
                    {
                        param.LocationLevel3s = levels[i];
                    }
                }
            }
        }

        [HttpGet("alarm")]
        public async Task<ActionResult<ApiResult>> Alarm([FromQuery]StatisticsParam param, string groupby, int dateType)
        {
            List<string> group = new List<string>();
            if (!string.IsNullOrWhiteSpace(groupby))
            {
                group.AddRange(groupby.Split(','));
            }
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsService.ListStatisticsAlarm(param, group, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbyeqptype")]
        public async Task<ActionResult<ApiResult>> AlarmGroupByEqpType([FromQuery]StatisticsParam param, int dateType)
        {
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsService.ListStatisticsAlarmGroupByEqpType(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbysupplier")]
        public async Task<ActionResult<ApiResult>> AlarmGroupBySupplier([FromQuery]StatisticsParam param, int dateType)
        {
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsService.ListStatisticsAlarmGroupBySupplier(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbymanufacturer")]
        public async Task<ActionResult<ApiResult>> AlarmGroupByManufacturer([FromQuery]StatisticsParam param, int dateType)
        {
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsService.ListStatisticsAlarmGroupByManufacturer(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbysubsystem")]
        public async Task<ActionResult<ApiResult>> AlarmGroupBySubSystem([FromQuery]StatisticsParam param, int dateType)
        {
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsService.ListStatisticsAlarmGroupBySubSystem(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbylocation")]
        public async Task<ActionResult<ApiResult>> AlarmGroupByLocation([FromQuery]StatisticsParam param, int dateType)
        {
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsService.ListStatisticsAlarmGroupByLocation(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbyorg")]
        public async Task<ActionResult<ApiResult>> AlarmGroupByOrg([FromQuery]StatisticsParam param, int dateType)
        {
            if (!string.IsNullOrEmpty(param.LocationPath))
            {
                string[] levels = param.LocationPath.Split(',');
                for (int i = 0; i < levels.Length; ++i)
                {
                    if (i == 0)
                    {
                        param.LocationLevel1s = levels[i];
                    }
                    if (i == 1)
                    {
                        param.LocationLevel2s = levels[i];
                    }
                    if (i == 2)
                    {
                        param.LocationLevel3s = levels[i];
                    }
                }
            }
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsService.ListStatisticsAlarmGroupByOrg(param, dateType);
            return ret;
        }

        [HttpGet("trouble/date")]
        public async Task<ActionResult<ApiResult>> TroubleGroupByDate([FromQuery]StatisticsTroubleParam param, string groupby, int dateType)
        {
            List<string> group = new List<string>();
            if (!string.IsNullOrWhiteSpace(groupby))
            {
                group.AddRange(groupby.Split(','));
            }
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsTroubleService.ListStatisticsTroubleGroupByDate(param, group, dateType);
            return ret;
        }

        [HttpGet("trouble/other")]
        public async Task<ActionResult<ApiResult>> TroubleGroupByOther([FromQuery]StatisticsTroubleParam param, string groupby, int dateType)
        {
            List<string> group = new List<string>();
            if (!string.IsNullOrWhiteSpace(groupby))
            {
                group.AddRange(groupby.Split(','));
            }
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsTroubleService.ListStatisticsTroubleGroupByOther(param, group, dateType);
            return ret;
        }

        [HttpGet("trouble/groupbylocation")]
        public async Task<ActionResult<ApiResult>> TroubleGroupByLocation([FromQuery]StatisticsTroubleParam param, int dateType)
        {
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsTroubleService.ListStatisticsTroubleGroupByLocation(param, dateType);
            return ret;
        }

        [HttpGet("trouble/groupbyorg")]
        public async Task<ActionResult<ApiResult>> TroubleGroupByOrg([FromQuery]StatisticsTroubleParam param, int dateType)
        {
            SplitLocation(param);
            param.TopOrgIDs = await getTopOrgByUser();
            var ret = await _statisticsTroubleService.ListStatisticsTroubleGroupByOrg(param, dateType);
            return ret;
        }

        [HttpPost("trouble")]
        public async Task<ActionResult<ApiResult>> AddTrouble(StatisticsTrouble trouble)
        {
            var ret = await _statisticsTroubleService.AddTrouble(trouble);
            return ret;
        }

        [HttpGet("trouble/rankbystation")]
        public async Task<ActionResult<ApiResult>> GetRankByStation()
        {
            var ret = await _statisticsService.GetStatisticsTroubleRank();
            return ret;
        }

        [HttpGet("trouble/runningtime")]
        public async Task<ActionResult<ApiResult>> GetRunningtime()
        {
            var ret = await _statisticsService.GetRunningtime();
            return ret;
        }

        [HttpGet("trouble/getnow")]
        public async Task<ActionResult<ApiResult>> GetNow()
        {
            var ret = await _statisticsService.GetNow();
            return ret;
        }

        [HttpGet("trouble/planchart")]
        public async Task<ActionResult<ApiResult>> GetIndexProcess()
        {
            var ret = await _statisticsService.GetIndexProcess();
            return ret;
        }

        [HttpGet("trouble/runningcost")]
        public async Task<ActionResult<ApiResult>> GetRunningCost()
        {
            var ret = await _statisticsService.GetRunningCost();
            return ret;
        }

        [HttpGet("trouble/pidchart")]
        public async Task<ActionResult<ApiResult>> GetPidChart()
        {
            var ret = await _statisticsService.GetPidChart();
            return ret;
        }

        [HttpGet("trouble/costchart")]
        public async Task<ActionResult<ApiResult>> GetCostChart()
        {
            var ret = await _statisticsService.GetCostChart();
            return ret;
        }
    }
}