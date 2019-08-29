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
                Dictionary<int, string> eqptype = new Dictionary<int, string>();
                eqptype.Add(1, "小系统风机");
                eqptype.Add(3, "水泵");
                eqptype.Add(5, "照明");
                eqptype.Add(6, "双波长");
                eqptype.Add(13, "事故风机");
                eqptype.Add(14, "射流风机");
                eqptype.Add(15, "排热风机");
                eqptype.Add(16, "回排风机");
                eqptype.Add(17, "组合式空调");
                eqptype.Add(18, "压差开关");
                eqptype.Add(19, "防烟防火阀");
                eqptype.Add(20, "排烟防火阀");
                eqptype.Add(21, "冷冻水泵");
                eqptype.Add(22, "冷却水泵");
                eqptype.Add(23, "冷却塔");
                eqptype.Add(24, "液位开关");
                eqptype.Add(25, "二通调节阀");
                eqptype.Add(26, "压差旁通阀");
                eqptype.Add(27, "温度,湿度");
                eqptype.Add(28, "二氧化碳");
                eqptype.Add(29, "电磁");
                eqptype.Add(30, "空气流量");
                eqptype.Add(31, "水管温度");


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
                                // Equipment eqp = list.Where(c => c.EqpCode.Equals(eqpCode))
                                //     .FirstOrDefault();
                                // if (eqp != null)
                                // {
                                //     newdata.EqpID = eqp.ID;
                                // }
                                
                                // 设备属性
                                newdata.prop = pid_seg[4];
                                newdata.Des = item_pid["Des"].ToString();
                                // 判断是否为故障点
                                if (newdata.Des.IndexOf("故障") > 0)
                                {
                                    newdata.PidType = 1;
                                }

                                // 模拟数据
                                int eqptypeid = 0;
                                foreach (var item_eqptype in eqptype)
                                {
                                    bool isThistype = false;
                                    string[] deskeys = item_eqptype.Value.Split(',');
                                    for (int i = 0; i < deskeys.Count(); ++i)
                                    {
                                        if (newdata.Des.IndexOf(deskeys[i]) > 0)
                                        {
                                            isThistype = true;
                                            break;
                                        }
                                    }
                                    if (isThistype)
                                    {
                                        eqptypeid = item_eqptype.Key;
                                    }
                                }
                                if (eqptypeid > 0)
                                {
                                    List<Equipment> eqps = list.Where(c => c.EqpType.Equals(eqptypeid)).ToList();
                                    Random random = new Random();
                                    int idx = random.Next(eqps.Count - 1);
                                    newdata.EqpID = eqps[idx].ID;
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
                // 发送通知 更新pid内存表
                _globalDataManager.postUpdateEvent(UpdateEventType.InitAllPid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.Message);
            }
        }

    }
}