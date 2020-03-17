using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using MSS.API.Common;
using static MSS.API.Common.Const;
using MSS.API.Common.Utility;

namespace MSS.API.Core.V1.Business
{
    public class EquipmentTypeService : IEquipmentTypeService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IEquipmentTypeRepo<EquipmentType> _eqpTypeRepo;
        private readonly IEquipmentRepo<Equipment> _eqpRepo;
        private readonly IUploadFileRepo<UploadFile> _uploadFileRepo;
        private readonly int userID;

        public EquipmentTypeService(IEquipmentTypeRepo<EquipmentType> eqpTypeRepo,
            IEquipmentRepo<Equipment> eqpRepo, IAuthHelper auth, IUploadFileRepo<UploadFile> uploadFileRepo)
        {
            //_logger = logger;
            _eqpTypeRepo = eqpTypeRepo;
            _eqpRepo = eqpRepo;
            _uploadFileRepo = uploadFileRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(EquipmentType eqpType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                eqpType.UpdatedTime = dt;
                eqpType.CreatedTime = dt;
                eqpType.UpdatedBy = userID;
                eqpType.CreatedBy = userID;
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
                    eqpType.UpdatedBy = userID;
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

        public async Task<ApiResult> Delete(string ids)
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
                else if (await _eqpTypeRepo.UseEqpType(ids))
                {
                    ret.code = Code.DataIsExist;
                    ret.msg = "此设备类型被用于物资定义中，不允许删除";
                }
                // 判断设备类型下有没有挂设备，有的话不允许删除
                else ret.data = await _eqpTypeRepo.Delete(ids.Split(','),userID);
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
                List<UploadFile> ufs = await _uploadFileRepo.ListByEntity(
                    etv.rows.Select(a=>a.ID).ToArray(),MyDictionary.SystemResource.EqpType);
                if (ufs!=null && ufs.Count()>0)
                {
                    foreach (var item in etv.rows)
                    {
                        var tmp = ufs.Where(a => a.Entity == item.ID);
                        if (tmp != null)
                        {
                            item.UploadFiles = JsonConvert.SerializeObject(UploadFileHelper.CascaderShow(tmp.ToList()));
                        }
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
                List<UploadFile> ufs = await _uploadFileRepo.ListByEntity(new int[] { id}, MyDictionary.SystemResource.EqpType);
                if (ufs != null && ufs.Count()>0)
                {
                    et.UploadFiles = JsonConvert.SerializeObject(UploadFileHelper.ListShow(ufs));
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

        public async Task<ApiResult> MockData()
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<int> subsystems = new List<int>{
                    18, 19, 20, 21
                };
                // 部门
                Dictionary<int, string> org = new Dictionary<int, string>();
                org.Add(4, "1,2,4");
                org.Add(6, "1,2,6");
                org.Add(7, "1,3,7");
                org.Add(16, "1,16");
                org.Add(17, "8,17");

                Dictionary<int, string> location = new Dictionary<int, string>();
                location.Add(17, "2,17");
                location.Add(18, "1,18");
                location.Add(22, "1,22");
                location.Add(23, "1,23");
                location.Add(24, "1,24");
                location.Add(25, "2,25");
                location.Add(27, "2,27");
                location.Add(28, "3,28");
                location.Add(29, "3,29");
                location.Add(34, "1,34");
                location.Add(35, "1,35");
                location.Add(37, "1,37");
                location.Add(43, "4,43");
                location.Add(44, "4,44");

                List<EquipmentType> types = await _eqpTypeRepo.GetAll();
                
                foreach (EquipmentType type in types)
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        Equipment eqp = new Equipment();
                        eqp.Code = type.ID + "_" + i;
                        eqp.Name = type.TName + i;
                        eqp.Type = type.ID;
                        eqp.SubSystem = subsystems[i%4];
                        int radomKey = org.Keys.ToArray()[i%5];
                        eqp.Team = radomKey;
                        eqp.TeamPath = org[radomKey];
                        eqp.TopOrg = Convert.ToInt32(eqp.TeamPath.Split(',')[0]);

                        radomKey = location.Keys.ToArray()[i % 14];
                        eqp.Location = radomKey;
                        eqp.LocationBy = 1;
                        eqp.LocationPath = location[radomKey];

                        eqp.MediumRepair = 30;
                        eqp.LargeRepair = 90;
                        eqp.CreatedBy = 1;
                        eqp.CreatedTime = DateTime.Now;
                        eqp.UpdatedBy = 1;
                        eqp.UpdatedTime = DateTime.Now;
                        eqp.IsDel = 0;

                        await _eqpRepo.Save(eqp);
                    }
                    
                }
                ret.data = null;
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
