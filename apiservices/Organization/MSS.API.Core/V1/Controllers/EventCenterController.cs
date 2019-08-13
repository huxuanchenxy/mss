using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.Common.Consul;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EventCenterController : ControllerBase
    {
        private readonly IWarnningService  _warnService;
        private readonly IAuthHelper _authHelper;
        private readonly IOrgService _orgService;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly int _userId;
        private readonly IConfiguration _config;
        public EventCenterController(IWarnningService warnService, IAuthHelper authHelper,
            IOrgService orgService, IServiceDiscoveryProvider consulServiceProvider,
            IConfiguration config)
        {
            _warnService = warnService;
            _authHelper = authHelper;
            _orgService = orgService;
            _config = config;
            _userId = _authHelper.GetUserId();
            _consulServiceProvider = consulServiceProvider;
        }

        [HttpGet("config")]
        public ActionResult<ApiResult> GetConfig()
        {
            ApiResult ret = new ApiResult { code = Code.Success };
            string hub = _config.GetValue<string>("EventCenter:hub");
            ret.data = new {
                hub = hub
            };
            return ret;
        }

        [HttpGet("warning")]
        public async Task<ActionResult<ApiResult>> GetWarnning()
        {
            ApiResult ret = new ApiResult{ code = Code.Failure };
            ApiResult topOrg = await _orgService.GetTopNodeByUserID(_userId);
            if (topOrg.code == Code.Success)
            {
                int topOrgID = 0;
                if (topOrg.data != null)
                {
                    topOrgID = (topOrg.data as OrgTree).ID;
                    ret = await _warnService.ListWarnningByOrg(topOrgID);
                }
                else
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services+"/api/v1/user/" + _userId);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    if ((bool)jobj["is_super"])
                    {
                        ret = await _warnService.ListWarnningByOrg(null);
                    }
                }
            }
            
            return ret;
        }

        [HttpGet("notification")]
        public async Task<ActionResult<ApiResult>> GetNotification()
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ApiResult topOrg = await _orgService.GetTopNodeByUserID(_userId);
            if (topOrg.code == Code.Success)
            {
                int topOrgID = 0;
                if (topOrg.data != null)
                {
                    topOrgID = (topOrg.data as OrgTree).ID;
                    ret = await _warnService.ListNotificationByOrg(topOrgID);
                }
                else
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/user/" + _userId);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    if ((bool)jobj["is_super"])
                    {
                        ret = await _warnService.ListNotificationByOrg(null);
                    }
                }
            }

            return ret;
        }

        [HttpGet("alarm")]
        public async Task<ActionResult<ApiResult>> GetAlarm()
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ApiResult topOrg = await _orgService.GetTopNodeByUserID(_userId);
            if (topOrg.code == Code.Success)
            {
                int topOrgID = 0;
                if (topOrg.data != null)
                {
                    topOrgID = (topOrg.data as OrgTree).ID;
                    ret = await _warnService.ListAlarmByOrg(topOrgID);
                }
                else
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/user/" + _userId);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    if ((bool)jobj["is_super"])
                    {
                        ret = await _warnService.ListAlarmByOrg(null);
                    }
                }
            }

            return ret;
        }

        [HttpGet("alarm/eqps")]
        public async Task<ActionResult<ApiResult>> GetAlarmEqp([FromQuery] AlarmEqpParam param)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ApiResult topOrg = await _orgService.GetTopNodeByUserID(_userId);
            if (topOrg.code == Code.Success)
            {
                int topOrgID = 0;
                if (topOrg.data != null)
                {
                    topOrgID = (topOrg.data as OrgTree).ID;
                    ret = await _warnService.ListAlarmEqpByOrg(topOrgID, param);
                }
                else
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/user/" + _userId);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    if ((bool)jobj["is_super"])
                    {
                        ret = await _warnService.ListAlarmEqpByOrg(null, param);
                    }
                }
            }

            return ret;
        }

        [HttpDelete("notification/{id}")]
        public async Task<ActionResult<ApiResult>> DeleteNotification(int id)
        {
            ApiResult ret = await _warnService.DeleteNotification(id);
            return ret;
        }

        [HttpGet("history/alarm")]
        public async Task<ActionResult<ApiResult>> GetAlarmHistoryByPage([FromQuery]AlarmParam query)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ApiResult topOrg = await _orgService.GetTopNodeByUserID(_userId);
            if (topOrg.code == Code.Success)
            {
                if (topOrg.data != null)
                {
                    query.orgID = (topOrg.data as OrgTree).ID;
                }
                else
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/user/" + _userId);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    if ((bool)jobj["is_super"])
                    {
                        query.orgID = null;
                    }
                }
                ret = await _warnService.ListAlarmHistory(query);
            }

            return ret;
        }

        [HttpGet("history/warning")]
        public async Task<ActionResult<ApiResult>> GetWarningHistoryByPage([FromQuery]WarningParam query)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ApiResult topOrg = await _orgService.GetTopNodeByUserID(_userId);
            if (topOrg.code == Code.Success)
            {
                if (topOrg.data != null)
                {
                    query.orgID = (topOrg.data as OrgTree).ID;
                }
                else
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/user/" + _userId);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    if ((bool)jobj["is_super"])
                    {
                        query.orgID = null;
                    }
                }
                ret = await _warnService.ListEarlyWarningHistory(query);
            }

            return ret;
        }

        [HttpGet("history/notification")]
        public async Task<ActionResult<ApiResult>> GetNotificationHistoryByPage([FromQuery]NotificationParam query)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ApiResult topOrg = await _orgService.GetTopNodeByUserID(_userId);
            if (topOrg.code == Code.Success)
            {
                if (topOrg.data != null)
                {
                    query.orgID = (topOrg.data as OrgTree).ID;
                }
                else
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/user/" + _userId);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    if ((bool)jobj["is_super"])
                    {
                        query.orgID = null;
                    }
                }
                ret = await _warnService.ListNotificationHistory(query);
            }

            return ret;
        }

    }
}