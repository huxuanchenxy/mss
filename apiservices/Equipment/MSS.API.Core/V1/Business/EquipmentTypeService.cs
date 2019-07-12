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

        public async Task<ApiResult> Save(EquipmentType eqpType)
        {
            ApiResult ret = new ApiResult();
            try
            {
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

        public async Task<ApiResult> Update(EquipmentType eqpType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                EquipmentType et = await _eqpTypeRepo.GetByID(eqpType.ID);
                if (et!=null)
                {
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
                EqpTypeView etv = await _eqpTypeRepo.GetPageByParm(parm);
                List<UploadFileEqpType> ufets= await _eqpTypeRepo.UploadFileListByEqpType((etv.rows.Select(a => a.ID)).ToArray());
                foreach (var item in etv.rows)
                {
                    var tmp = ufets.Where(a => a.EqpTypeID == item.ID);
                    if (tmp!=null)
                    {
                        item.UploadFiles = ListToTreeJson(tmp.ToList());
                    }
                }
                ret.data = etv;
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
                EquipmentType et = await _eqpTypeRepo.GetByID(id);
                List<object> list = await _eqpTypeRepo.RelationListByEqpType(id);
                if (list!=null)
                {
                    et.UploadFiles = JsonConvert.SerializeObject(list);
                }
                ret.data = et;
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

        private string ListToTreeJson(List<UploadFileEqpType> ufets)
        {
            List<object> objs = new List<object>();
            IEnumerable<IGrouping<int, UploadFileEqpType>> groups = ufets.GroupBy(a => a.Type);
            foreach (var group in groups.OrderBy(a=>a.Key))
            {
                string label="";
                List<object> children = new List<object>();
                foreach (var item in group)
                {
                    label = item.TName;
                    children.Add(new {
                        value=item.FilePath,
                        label=item.FileName
                    });
                }
                objs.Add(new {
                    value=group.Key,
                    label=label,
                    children=children
                });
            }
            return JsonConvert.SerializeObject(objs);
        }
    }
}
