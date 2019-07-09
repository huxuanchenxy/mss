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
using static MSS.API.Common.Const;
using static MSS.API.Common.FilePath;
using Microsoft.AspNetCore.Http;
using MSS.API.Common.Utility;
using System.IO;

namespace MSS.API.Core.V1.Business
{
    public class EquipmentService : IEquipmentService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IEquipmentRepo<Equipment> _eqpRepo;
        private readonly IUploadFileRepo<UploadFile> _uploadFileRepo;

        public EquipmentService(IEquipmentRepo<Equipment> eqpRepo, IUploadFileRepo<UploadFile> uploadFileRepo)
        {
            //_logger = logger;
            _eqpRepo = eqpRepo;
            _uploadFileRepo = uploadFileRepo;
        }

        public async Task<ApiResult> Save(Equipment eqp)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                eqp.UpdatedTime = dt;
                eqp.CreatedTime = dt;
                ret.data = await _eqpRepo.Save(eqp);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(Equipment eqp)
        {
            ApiResult ret = new ApiResult();
            try
            {
                Equipment et = await _eqpRepo.GetByID(eqp.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    eqp.UpdatedTime = dt;
                    ret.data = await _eqpRepo.Update(eqp);
                }
                else
                {
                    ret.code = Code.DataIsnotExist;
                    ret.msg = "所要修改的数据不存在";
                }
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Delete(string ids,int userID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _eqpRepo.Delete(ids.Split(','),userID);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetPageByParm(EqpQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                EqpView ev = await _eqpRepo.GetPageByParm(parm);
                List<Equipment> eqps=ev.rows;
                List<AllArea> laa = await _eqpRepo.GetAllArea();
                foreach (var item in eqps)
                {
                    item.LocationName = laa.Where(a => a.Tablename == item.LocationBy && a.ID == item.Location)
                        .FirstOrDefault().AreaName;
                }
                ret.data = ev;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                Equipment e = await _eqpRepo.GetByID(id);
                e.Drawings =await _uploadFileRepo.ListByForeignAndType(e.ID, (int)FileType.Eqp_Drawings);
                ret.data = e;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetAll()
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _eqpRepo.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
    }
}
