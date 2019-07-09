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
using System.IO;

namespace MSS.API.Core.V1.Business
{
    public class EquipmentTypeService : IEquipmentTypeService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IEquipmentTypeRepo<EquipmentType> _eqpTypeRepo;
        private readonly IEquipmentRepo<Equipment> _eqpRepo;

        public EquipmentTypeService(IEquipmentTypeRepo<EquipmentType> eqpTypeRepo,
            IEquipmentRepo<Equipment> eqpRepo)
        {
            //_logger = logger;
            _eqpTypeRepo = eqpTypeRepo;
            _eqpRepo = eqpRepo;
        }

        public async Task<ApiResult> Save(EquipmentType eqpType, List<IFormFile> file)
        {
            ApiResult ret = new ApiResult();
            try
            {
                //保存文件
                //SavePDF(file,ref eqpType);

                DateTime dt = DateTime.Now;
                eqpType.UpdatedTime = dt;
                eqpType.CreatedTime = dt;
                ret.data = await _eqpTypeRepo.Save(eqpType);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(EquipmentType eqpType, List<IFormFile> file)
        {
            ApiResult ret = new ApiResult();
            try
            {
                EquipmentType et = await _eqpTypeRepo.GetByID(eqpType.ID);
                if (et!=null)
                {
                    //保存文件
                    //SavePDF(file, ref eqpType);
                    //原来已上传的路径放进来
                    if (string.IsNullOrWhiteSpace(eqpType.PWorking) && !string.IsNullOrWhiteSpace(et.PWorking))
                    {
                        eqpType.PWorking = et.PWorking;
                    }
                    if (string.IsNullOrWhiteSpace(eqpType.PDrawings) && !string.IsNullOrWhiteSpace(et.PDrawings))
                    {
                        eqpType.PDrawings = et.PDrawings;
                    }
                    if (string.IsNullOrWhiteSpace(eqpType.PInstall) && !string.IsNullOrWhiteSpace(et.PInstall))
                    {
                        eqpType.PInstall = et.PInstall;
                    }
                    if (string.IsNullOrWhiteSpace(eqpType.PUser) && !string.IsNullOrWhiteSpace(et.PUser))
                    {
                        eqpType.PUser = et.PUser;
                    }
                    if (string.IsNullOrWhiteSpace(eqpType.PRegulations) && !string.IsNullOrWhiteSpace(et.PRegulations))
                    {
                        eqpType.PRegulations = et.PRegulations;
                    }

                    DateTime dt = DateTime.Now;
                    eqpType.UpdatedTime = dt;
                    ret.data = await _eqpTypeRepo.Update(eqpType);
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
                var eqps =await _eqpRepo.ListByEqpType(ids);
                if (eqps!=null && eqps.Count>0)
                {
                    ret.code = Code.DataIsExist;
                    ret.msg = "挂有设备的设备类型不允许删除";
                }
                // 判断设备类型下有没有挂设备，有的话不允许删除
                ret.data = await _eqpTypeRepo.Delete(ids.Split(','),userID);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> GetPageByParm(EqpTypeQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _eqpTypeRepo.GetPageByParm(parm);
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
                ret.data = await _eqpTypeRepo.GetByID(id);
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
                ret.data = await _eqpTypeRepo.GetAll();
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
