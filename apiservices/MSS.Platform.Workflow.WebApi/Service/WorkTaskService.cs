using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.Common.Consul;
using MSS.Platform.Workflow.WebApi.Data;
using MSS.Platform.Workflow.WebApi.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MSS.Platform.Workflow.WebApi.Service
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IWorkTaskRepo<TaskViewModel> _repo;
        private readonly IAuthHelper _authhelper;
        private readonly IServiceDiscoveryProvider _consulclient;
        private readonly IWfprocessRepo<Wfprocess> _wfprocessRepo;
        public WorkTaskService(IWorkTaskRepo<TaskViewModel> repo, IAuthHelper authhelper, IServiceDiscoveryProvider consulclient, IWfprocessRepo<Wfprocess> wfprocessRepo)
        {
            _repo = repo;
            _authhelper = authhelper;
            _consulclient = consulclient;
            _wfprocessRepo = wfprocessRepo;
        }

        /// <summary>
        /// 我的待办
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult> GetReadyTasks(WorkTaskQueryParm parm)
        {
            ApiResult ret = new ApiResult();

            try
            {
                parm.ActivityState = 1;
                parm.AssignedToUserID = _authhelper.GetUserId();
                //parm.AssignedToUserID = 40;
                var data = await _repo.GetPageList(parm);
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

        /// <summary>
        /// 我的申请
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult> GetPageMyApply(WorkTaskQueryParm parm)
        {
            ApiResult ret = new ApiResult();

            try
            {
                parm.AssignedToUserID = _authhelper.GetUserId();
                //parm.AssignedToUserID = 40;
                var data = await _repo.GetPageMyApply(parm);
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

        /// <summary>
        /// 流转日志
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult> GetPageActivityInstance(WorkQueryParm parm)
        {
            ApiResult ret = new ApiResult();

            try
            {
                parm.UserID = _authhelper.GetUserId();
                //parm.UserID = 40;
                var data = await _repo.GetPageActivityInstance(parm);
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



        public async Task<WfRet> StartProcess(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);

                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/StartProcess";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }

            return ret;
        }


        public async Task<WfRet> GetNextStepRoleUserTree(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);

                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/GetNextStepRoleUserTree";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }

            return ret;
        }

        public async Task<WfRet> NextProcess(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);

                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/RunProcessApp";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }

            return ret;
        }

        public async Task<WfRet> WithdrawProcess(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);
                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/WithdrawProcess";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }
            return ret;
        }

        public async Task<WfRet> SendBackProcess(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);
                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/SendBackProcess";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }
            return ret;
        }

        public async Task<WfRet> GetProcessListSimple()
        {
            WfRet ret = new WfRet();

            try
            {
                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/GetProcessListSimple";
                var wfresponse = HttpClientHelper.GetResponse<WfRet>(url2);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }
            return ret;
        }

        public async Task<WfRet> ReverseProcess(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);
                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/ReverseProcess";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }
            return ret;
        }
        public async Task<WfRet> CancelProcess(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);
                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/CancelProcess";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }
            return ret;
        }

        public async Task<WfRet> QueryReadyActivityInstance(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);
                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/QueryReadyActivityInstance";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }
            return ret;
        }

        public async Task<WfRet> QueryCompletedTasks(WfReq parm)
        {
            WfRet ret = new WfRet();

            try
            {
                parm = await getWfCommonReq(parm);
                string url2 = await _consulclient.GetServiceAsync("WorkFlowProcessService");
                url2 += "/api/v1/WfProcess/QueryCompletedTasks";
                var wfresponse = HttpClientHelper.PostResponse<WfRet>(url2, parm);
                ret = wfresponse;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Status = -999;
                ret.Message = ex.Message.ToString();
            }
            return ret;
        }

        private async Task<WfReq> getWfCommonReq(WfReq parm)
        {
            parm.UserID = _authhelper.GetUserId();
            string url1 = await _consulclient.GetServiceAsync("AuthService");
            url1 += "/api/v1/User/" + parm.UserID;
            var userresponse = HttpClientHelper.GetResponse<ApiResult>(url1);
            dynamic dyuser = userresponse.data;
            string username = dyuser.user_name;

            var wfprocessobj = await _wfprocessRepo.GetByID(parm.ProcessID);
            parm.ProcessGUID = wfprocessobj.ProcessGUID;
            parm.AppName = wfprocessobj.ProcessName;//TODO 待需求确认写流程名称
            parm.UserName = username;
            return parm;
        }

    }

    public interface IWorkTaskService
    {
        Task<ApiResult> GetReadyTasks(WorkTaskQueryParm parm);
        Task<ApiResult> GetPageMyApply(WorkTaskQueryParm parm);
        Task<ApiResult> GetPageActivityInstance(WorkQueryParm parm);
        Task<WfRet> StartProcess(WfReq parm);
        Task<WfRet> GetNextStepRoleUserTree(WfReq parm);
        Task<WfRet> NextProcess(WfReq parm);
        Task<WfRet> WithdrawProcess(WfReq parm);
        Task<WfRet> SendBackProcess(WfReq parm);
        Task<WfRet> GetProcessListSimple();
        Task<WfRet> ReverseProcess(WfReq parm);
        Task<WfRet> CancelProcess(WfReq parm);
        Task<WfRet> QueryReadyActivityInstance(WfReq parm);
        Task<WfRet> QueryCompletedTasks(WfReq parm);
    }


}
