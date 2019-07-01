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

namespace MSS.API.Core.V1.Business
{
    public class ExpertDataService : IExpertDataService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly Itb_expert_dataRepo<tb_expert_data> _expertRepo;

        public ExpertDataService(Itb_expert_dataRepo<tb_expert_data> expertRepo)
        {
            //_logger = logger;
            _expertRepo = expertRepo;
        }


        public async Task<ApiResult> Add(tb_expert_data model)
        {
            ApiResult result = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _expertRepo.Exists(model.keyword);
                    if (!isExist)
                    {
                        var ret = await _expertRepo.Add(model);
                        //if (ret<1)
                        //{
                        //    result.code = Code.Failure;
                        //    result.data = null;
                        //}
                        result.code = Code.Success;
                        result.data = true;
                    }
                    else
                    {
                        result.code = Code.DataIsnotExist;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                result.code = Code.Failure;
                result.msg = ex.Message;
            }

            return result;
        }

        public async Task<ApiResult> Update(tb_expert_data model)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _expertRepo.Exists(model.keyword);
                    if (!isExist)
                    {
                        var data = await _expertRepo.Update(model);
                        ret.code = Code.Success;
                        ret.data = true;
                    }
                    else
                    {
                        ret.code = Code.DataIsnotExist;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            { 
                ret.code = Code.Failure;
                ret.data = ex.Message;
            }

            return ret;
        }


        public async Task<ApiResult> Delete(tb_expert_data model)
        {
            ApiResult ret = new ApiResult();
            try
            {
                await _expertRepo.Delete(model);
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> DeleteList(string ids)
        {

            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    string[] strs = ids.Split(",");
                    foreach (var v in strs)
                    {
                        bool isExist = await _expertRepo.Delete(new tb_expert_data { ID = int.Parse(v),UpdatedTime=DateTime.Now, UpdatedBy=101 });
                    }
                    ret.code = Code.Success;
                    ret.data = true;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;  
        }

        Task<ApiResult> IExpertDataService.Exists(int id)
        {
            throw new NotImplementedException();
        }

        //Task<ApiResult> IExpertDataService.GetList(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<ApiResult> IExpertDataService.GetList(string strWhere)
        //{
        //    throw new NotImplementedException();
        //}

       public async Task<ApiResult>  GetListByPage(string strWhere, string orderby, string sort, int page, int size)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int count = await _expertRepo.GetRecordCount(strWhere);
                    var list = await _expertRepo.GetListByPage(strWhere , sort, orderby, page, size);
                    ret.code = Code.Success;
                    ret.data = new
                    {
                        total = count,
                        list = list
                    };
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        //Task<ApiResult> IExpertDataService.GetMaxId()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ApiResult>  GetModel(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var model = await _expertRepo.GetModel(id);                   
                    ret.code = Code.Success;
                    ret.data = model;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        //Task<ApiResult> IExpertDataService.GetRecordCount(string where)
        //{
        //    throw new NotImplementedException();
        //}
 
    }
}
