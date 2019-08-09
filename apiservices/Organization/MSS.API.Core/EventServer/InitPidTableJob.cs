using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System.Collections.Generic;
using MSS.API.Core.Models.Ex;
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.Common;
using System.Threading;
using MSS.API.Common;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Transactions;
namespace MSS.API.Core.EventServer
{
    public class InitPidTableJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly IWarnningSettingRepo<EarlyWarnningSetting> _warnSettingRepo;
        private readonly GlobalDataManager _globalDataManager;
        public InitPidTableJob(ILogger<InitPidTableJob> logger, IDistributedCache cache,
            IServiceDiscoveryProvider consulServiceProvider, IWarnningSettingRepo<EarlyWarnningSetting> warnSettingRepo,
            GlobalDataManager globalDataManager)
        {
            _logger = logger;
            _cache = cache;
            _consulServiceProvider = consulServiceProvider;
            _warnSettingRepo = warnSettingRepo;
            _globalDataManager = globalDataManager;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("开始同步pid表");
                var _services = await _consulServiceProvider.GetServiceAsync("RDBRTDBService");
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.
                    GetSingleItemRequest(_services + "/api/v1/TableInfo");
                if (result.code == Code.Success)
                {
                    // 获取所有设备
                    List<Equipment> list = await _warnSettingRepo.ListAllEquipment();

                    JArray data_table = JsonConvert.DeserializeObject<JArray>(result.data.ToString());
                    foreach (JObject item in data_table)
                    {
                        string table = item["TableName"].ToString();
                        result = await httpHelper.
                            GetSingleItemRequest(_services + "/api/v1/TableInfo/" + table);
                        // 获取每张表中pid
                        if (result.code == Code.Success)
                        {
                            List<PidTable> pidTableData = new List<PidTable>();
                            JArray data_pids = JsonConvert.DeserializeObject<JArray>(result.data.ToString());
                            foreach (JObject item_pid in data_pids)
                            {
                                string pid = item_pid["PID"].ToString();
                                string[] pid_seg = pid.Split('_');
                                if (pid_seg.Length != 5)
                                {
                                    _logger.LogInformation("pid:" + pid + "格式不正确");
                                    continue;
                                }
                                // 
                                PidTable newdata = new PidTable();
                                newdata.pid = pid;
                                // 根据设备code查mss中设备id
                                string eqpCode = pid_seg[3];
                                newdata.EqpID = null;
                                Equipment eqp = list.Where(c => c.EqpCode.Equals(eqpCode))
                                    .FirstOrDefault();
                                if (eqp != null)
                                {
                                    newdata.EqpID = eqp.ID;
                                }
                                // 设备属性
                                newdata.prop = pid_seg[4];
                                newdata.Des = item_pid["Des"].ToString();
                                // 判断是否为故障点
                                if (newdata.Des.IndexOf("故障") > 0)
                                {
                                    newdata.PidType = 1;
                                }
                                if (item_pid["UP"] != null && item_pid["UP"].Type != JTokenType.Null)
                                {
                                    newdata.UP = Convert.ToDouble(item_pid["UP"]);
                                }
                                if (item_pid["DW"] != null && item_pid["DW"].Type != JTokenType.Null)
                                {
                                    newdata.DW = Convert.ToDouble(item_pid["DW"]);
                                }
                                if (item_pid["UUP"] != null && item_pid["UUP"].Type != JTokenType.Null)
                                {
                                    newdata.UUP = Convert.ToDouble(item_pid["UUP"]);
                                }
                                if (item_pid["DDW"] != null && item_pid["DDW"].Type != JTokenType.Null)
                                {
                                    newdata.DDW = Convert.ToDouble(item_pid["DDW"]);
                                }
                                pidTableData.Add(newdata);
                            }
                            // 删除表中原有记录然后添加
                            if (pidTableData.Count > 0)
                            {
                                using (TransactionScope scope = new TransactionScope())
                                {
                                    await _warnSettingRepo.DeletePidTable(pidTableData);
                                    await _warnSettingRepo.SavePidTable(pidTableData);
                                    scope.Complete();
                                }
                            }
                        }
                        else
                        {
                            _logger.LogError("/api/v1/TableInfo/" + table + "接口调用失败");
                        }
                    }
                }
                else
                {
                    _logger.LogError("/api/v1/TableInfo 接口调用失败");
                }
                _logger.LogInformation("同步pid表结束");
                // 发送通知 更新预警设定，预警设定中会牵涉到pid列表
                _globalDataManager.postUpdateEvent(UpdateEventType.InitWarnSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.Message);
            }
        }

    }
}