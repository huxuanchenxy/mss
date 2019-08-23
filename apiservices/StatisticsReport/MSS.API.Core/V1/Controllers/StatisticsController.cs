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

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly int _userId;
        private readonly IAuthHelper _authHelper;
        public StatisticsController(IStatisticsService statisticsService, IAuthHelper authHelper)

        {
            _statisticsService = statisticsService;
            _authHelper = authHelper;
            _userId = _authHelper.GetUserId();
        }

        [HttpGet("alarm")]
        public async Task<ActionResult<ApiResult>> Alarm([FromQuery]StatisticsParam param, string groupby, int dateType)
        {
            List<string> group = new List<string>();
            if (!string.IsNullOrWhiteSpace(groupby))
            {
                group.AddRange(groupby.Split(','));
            }
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
            var ret = await _statisticsService.ListStatisticsAlarm(param, group, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbyeqptype")]
        public async Task<ActionResult<ApiResult>> AlarmGroupByEqpType([FromQuery]StatisticsParam param, int dateType)
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
            var ret = await _statisticsService.ListStatisticsAlarmGroupByEqpType(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbysupplier")]
        public async Task<ActionResult<ApiResult>> AlarmGroupBySupplier([FromQuery]StatisticsParam param, int dateType)
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
            var ret = await _statisticsService.ListStatisticsAlarmGroupBySupplier(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbymanufacturer")]
        public async Task<ActionResult<ApiResult>> AlarmGroupByManufacturer([FromQuery]StatisticsParam param, int dateType)
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
            var ret = await _statisticsService.ListStatisticsAlarmGroupByManufacturer(param, dateType);
            return ret;
        }

        [HttpGet("alarm/groupbysubsystem")]
        public async Task<ActionResult<ApiResult>> AlarmGroupBySubSystem([FromQuery]StatisticsParam param, int dateType)
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
            var ret = await _statisticsService.ListStatisticsAlarmGroupBySubSystem(param, dateType);
            return ret;
        }
    }
}