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
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
namespace MSS.API.Core.EventServer
{
    public class NotificationJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly GlobalDataManager _globalDataManager;
        private readonly IWarnningRepo<EarlyWarnning> _warnSetting;
        private readonly IDistributedCache _cache;
        private readonly EventQueues _queues;
        private readonly IConfiguration _config;
        private readonly IOrgRepo<OrgTree> _orgRepo;
        public NotificationJob(ILogger<WarningJob> logger, GlobalDataManager globalDataManager,
            IServiceDiscoveryProvider consulServiceProvider, IDistributedCache cache,
            IWarnningRepo<EarlyWarnning> warnSetting, EventQueues queues, IConfiguration config,
            IOrgRepo<OrgTree> orgRepo)
        {
            _logger = logger;
            _globalDataManager = globalDataManager;
            _consulServiceProvider = consulServiceProvider;
            _cache = cache;
            _warnSetting = warnSetting;
            _queues = queues;
            _config = config;
            _orgRepo = orgRepo;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            while(true)
            {
                try
                {
                    // 取大中修事件
                    List<EqpHistory> eqpHistory = new List<EqpHistory>();
                    var _services = await _consulServiceProvider.GetServiceAsync("ExpertDataAndDeviceService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    string types = (int)MyDictionary.EqpHistoryType.Install + "," 
                        + (int)MyDictionary.EqpHistoryType.MediumMaintenance + ","
                        + (int)MyDictionary.EqpHistoryType.MajorMaintenance;
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/EqpHistory/ListByType/" + types);
                    if (result.code == Code.Success)
                    {
                        eqpHistory = JsonConvert.DeserializeObject<List<EqpHistory>>(result.data.ToString());
                    }
                    // 循环设备判断寿命及大中修事件
                    List<Equipment> allEqp = _globalDataManager.AllEqp;
                    EquipmentConfig eqpConfig = _globalDataManager.EqpConfig;
                    foreach (Equipment eqp in allEqp)
                    {
                        // 寿命
                        if (eqp.LifeCycle != null)
                        {
                            await _checkLifeCycle(eqp, eqpConfig);
                        }
                        await _checkMediumMaintenance(eqp, eqpConfig, eqpHistory);
                        await _checkMajorMaintenance(eqp, eqpConfig, eqpHistory);
                    }
                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
                }
            }
        }

        private async Task _checkMediumMaintenance(Equipment eqp, EquipmentConfig eqpConfig, List<EqpHistory> eqpHistory)
        {
            DateTime? start = null;
            EqpHistory his_medium = eqpHistory.Where(c => c.Type ==
                MyDictionary.EqpHistoryType.MediumMaintenance && c.EqpID == eqp.ID).FirstOrDefault();
            if (his_medium != null)
            {
                start = his_medium.CreatedTime;
            }
            else
            {
                EqpHistory his_install = eqpHistory.Where(c => c.Type ==
                MyDictionary.EqpHistoryType.Install && c.EqpID == eqp.ID).FirstOrDefault();
                if (his_install != null)
                {
                    start = his_install.CreatedTime;
                }
            }

            if (start != null && eqpConfig != null)
            {
                DateTime dateRepair = ((DateTime)start).AddDays(eqp.MediumRepair);
                if (dateRepair < DateTime.Now.AddDays(eqpConfig.beforeMaintainMiddle))
                {
                    string prefix = RedisKeyPrefix.Notification;
                    string notiID = _cache.GetString(prefix + eqp.ID + "_" + 2);
                    if (notiID == null)
                    {
                        int remain = (int)(dateRepair - DateTime.Now).TotalDays;
                        string msg = String.Format("设备中修时间{0}{1}天",
                            (remain > 0) ? "还有" : "超过", Math.Abs(remain));
                        
                        _addNotification(eqp, msg, 2, "中修");
                        await _sendMailOrSMS(eqp, eqpConfig, msg);
                    }
                }
            }
        }

        private async Task _checkMajorMaintenance(Equipment eqp, EquipmentConfig eqpConfig, List<EqpHistory> eqpHistory)
        {
            DateTime? start = null;
            EqpHistory his_major = eqpHistory.Where(c => c.Type ==
                MyDictionary.EqpHistoryType.MajorMaintenance && c.EqpID == eqp.ID).FirstOrDefault();
            if (his_major != null)
            {
                start = his_major.CreatedTime;
            }
            else
            {
                EqpHistory his_install = eqpHistory.Where(c => c.Type ==
                MyDictionary.EqpHistoryType.Install && c.EqpID == eqp.ID).FirstOrDefault();
                if (his_install != null)
                {
                    start = his_install.CreatedTime;
                }
            }

            if (start != null && eqpConfig != null)
            {
                DateTime dateRepair = ((DateTime)start).AddDays(eqp.LargeRepair);
                if (dateRepair < DateTime.Now.AddDays(eqpConfig.beforeMaintainBig))
                {
                    string prefix = RedisKeyPrefix.Notification;
                    string notiID = _cache.GetString(prefix + eqp.ID + "_" + 1);
                    if (notiID == null)
                    {
                        int remain = (int)(dateRepair - DateTime.Now).TotalDays;
                        string msg = String.Format("设备大修时间{0}{1}天",
                            (remain > 0) ? "还有" : "超过", Math.Abs(remain));
                        _addNotification(eqp, msg, 1, "大修");
                        await _sendMailOrSMS(eqp, eqpConfig, msg);
                    }
                }
            }
        }

        private async Task _checkLifeCycle(Equipment eqp, EquipmentConfig eqpConfig)
        {
            DateTime? start = null;
            if (eqp.OnlineAgain != null && eqp.OnlineAgain > eqp.OnlineDate)
            {
                start = eqp.OnlineAgain;
            }
            else
            {
                start = eqp.OnlineDate;
            }
            if (start != null && eqpConfig != null )
            {
                DateTime datelife = ((DateTime)start).AddYears((int)eqp.LifeCycle);
                if (datelife < DateTime.Now.AddDays(eqpConfig.beforeDead))
                {
                    string prefix = RedisKeyPrefix.Notification;
                    string notiID = _cache.GetString(prefix + eqp.ID + "_" + 0);
                    if (notiID == null)
                    {
                        int remain = (int)(datelife - DateTime.Now).TotalDays;
                        string msg = String.Format("寿命{0}{1}天",
                            (remain > 0) ? "还有" : "超过", Math.Abs(remain));
                        _addNotification(eqp, msg, 0, "寿命");
                        await _sendMailOrSMS(eqp, eqpConfig, msg);
                    }
                }
            }
        }

        private async Task _sendMailOrSMS(Equipment eqp, EquipmentConfig eqpConfig, string content)
        {
            // 发送邮件及短信, 1短信、2邮件、3 同时
            // 获取此设备所属班组人员
            if (eqp.Team != null)
            {
                List<OrgUser> users = await _orgRepo.ListOrgNodeUsers((int)eqp.Team);
                List<string> mailto = new List<string>();
                foreach (OrgUser user in users)
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        mailto.Add(user.Email);
                    }
                }

                if (eqpConfig.reminder == 1)
                {

                }
                else if (eqpConfig.reminder == 2)
                {
                    if (mailto.Count > 0)
                    {
                        _sendMail(mailto, content);
                    }
                    
                }
                else if (eqpConfig.reminder == 3)
                {
                    if (mailto.Count > 0)
                    {
                        _sendMail(mailto, content);
                    }
                }
            }
        }

        private void _sendMail(List<string> mailto, string content)
        {
            
            string host = _config.GetValue<string>("message:mail:host");
            string username = _config.GetValue<string>("message:mail:username");
            string password = _config.GetValue<string>("message:mail:password");
            // string host = "smtp.163.com";
            // string username = "niefei1979815@163.com";
            // string password = "19861120";

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式    
            client.Host = host;//邮件服务器
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential(username, password);//用户名、密码

            //////////////////////////////////////
            string strfrom = username;
            // string strto = "nief@seari.com";
            // string strcc = "2605625733@qq.com";//抄送


            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.From = new MailAddress(strfrom, "mss系统");
            foreach (string addr in mailto)
            {
                msg.To.Add(addr);
            }
            
            // msg.CC.Add(strcc);

            msg.Subject = "设备通知";//邮件标题
            msg.Body = content;//邮件内容
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            msg.IsBodyHtml = true;//是否是HTML邮件
            msg.Priority = MailPriority.High;//邮件优先级

            try
            {
                client.Send(msg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                _logger.LogError("邮件发送失败:" + ex.Message);
            }
        }

        private async void _addNotification(Equipment eqp, string content, int notificationType,
            string notificationTypeName)
        {
            Notification noti = new Notification();
            noti.EqpID = eqp.ID;
            noti.Content = content;
            noti.NotificationType = notificationType;
            noti.NotificationTypeName = notificationTypeName;
            noti.Status = 0;
            noti.CreatedTime = DateTime.Now;
            Notification ret = await _warnSetting.SaveNotification(noti);
            // 插入redis
            string prefix = RedisKeyPrefix.Notification;
            // key为设备id_通知类型,value 为 此通知在通知表中的id
            _cache.SetString(prefix + eqp.ID + "_" + notificationType, ret.ID.ToString());

            _sendNotification(eqp.ID, "on");
        }

        private void _sendNotification(int eqpID, string content)
        {
            MssEventMsg msg = new MssEventMsg();
            msg.msg = content;
            msg.type = MssEventType.Notification;
            msg.eqp = new Equipment()
            {
                ID = eqpID
            };
            _queues.AlarmQueue.Enqueue(msg);
        }

    }
}