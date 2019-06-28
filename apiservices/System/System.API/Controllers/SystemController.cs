using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.API.Core.Request;
using System.API.Core.Response;
using System.API.DTO;
using System.API.Service;
using System.API.Model;
using System.API.Core.Function;

namespace System.API.Core.Controllers
{
    /// <summary>
    /// 工作流模块
    /// </summary>
    [Route("api/System")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        //注入进来
         private readonly IService _SystemService; 
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="SystemService"></param>
        public SystemController(IService SystemService)
        {
            _SystemService = SystemService;

        }


        #region 站区模块

        /// <summary>
        /// 保存大区域
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        // [HttpPost]
        //  [Route("SaveConfigBigArea")]
        [HttpPost("SaveConfigBigArea")]
        public ResponseContext SaveConfigBigArea(TB_Config_BigAreaDTO data) //(RequestContext<TB_Config_BigAreaDTO> req)
        {
            ResponseContext result = new ResponseContext(); 
            var obj = data;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var Content = data; //req.data;
                TB_Config_BigArea model = new TB_Config_BigArea();
                ModelCtrl.CopyModel(model, Content, null, null);
                model.Created_Time = DateTime.Now;
                if (_SystemService._IConfigBigAreaService.Add(model) < 1)
                {
                    result.code = ErrCode.Failure;
                    result.ret = -1;
                }
                ts.Complete();
            } 
            return result;
        }

        /// <summary>
        /// 保存大区域
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateConfigBigArea")]
        public ResponseContext UpdateConfigBigAre(RequestContext<TB_Config_BigAreaDTO> req)
        {
            ResponseContext result = new ResponseContext();
           result.data = true;
            var obj = req.data;
            //if (obj == null)
            //{
            //    result.Head.Ret = -1;
            //    result.Head.Code = ErrCode.ParameterError;
            //    return result;
            //}
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                //校验传入参数
              //  var reqHeader = req.Head;
                var Content = req.data;
                TB_Config_BigArea model = new TB_Config_BigArea();
                ModelCtrl.CopyModel(model, Content, null, null);
                if (!_SystemService._IConfigBigAreaService.Update(model))
                {
                   // result.Head = new ResponseHead { Code = ErrCode.Failure, Ret = -1 };
                }
                ts.Complete();
            }
            //result.Head.Msg = "修改成功";
            
            return result;
        }


        /// <summary>
        /// 获取站区数据
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBigAreaQueryPageByParm")]
        public ResponseContext GetBigAreaQueryPageByParm([FromQuery] BigAreaQueryParm paras)
        { 
            ResponseContext result = new ResponseContext();
            TB_Config_BigAreaDTO model = new TB_Config_BigAreaDTO();
            string where = "  1=1 ";
            if (!string.IsNullOrEmpty(paras.SearchName))
            {
                where += " and AreaName like '%" + paras.SearchName + "%'";
            }
            else if(!string.IsNullOrEmpty(paras.searchType))
            {
                where += " and  ConfigType= ' " + paras.searchType + "'";
            }
            List<TB_Config_BigArea> list = _SystemService._IConfigBigAreaService.GetList(where);
            List<TB_Config_BigAreaDTO> results = new List<TB_Config_BigAreaDTO>();
            Helper.ModelToDTO<TB_Config_BigArea, TB_Config_BigAreaDTO>(list, results);
            foreach(var v in results)
            {
                if(v.ConfigType==1)
                {
                    v.ConfigTypeName = "车站";
                    continue;
                }
                if (v.ConfigType == 2)
                {
                    v.ConfigTypeName = "正线轨行区";
                    continue;
                }
                if (v.ConfigType == 3)
                {
                    v.ConfigTypeName = "保护区";
                    continue;
                }
                if (v.ConfigType ==4)
                {
                    v.ConfigTypeName = "车场生产区";
                    continue;
                }
            }
            result.data = results;
            result.total = results.Count();
            return result; 
        }

       

        /// <summary>
        /// 获取单个实体类
        /// </summary>
        /// <param name="id">查询参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetConfigBigAreaId")]
        public ResponseContext GetConfigBigAreaId(string id)
        {
            ResponseContext result = new ResponseContext();
            TB_Config_BigAreaDTO model = new TB_Config_BigAreaDTO();
            string where = " id='" + id + "'";
           result.data = _SystemService._IConfigBigAreaService.GetList(where);
            return result;
        }



        /// <summary>
        /// 删除单个实体类
        /// </summary>
        /// <param name="id">单个对象的ID</param>
        /// <returns></returns>
        [HttpGet("DelConfigBigAreaId/{id}")]
        // [Route("DelConfigBigAreaId")]
        public ResponseContext DelConfigBigAreaId(string id)
        {
            ResponseContext result = new ResponseContext();
            TB_Config_BigAreaDTO model = new TB_Config_BigAreaDTO();
            string[] strs = id.Split(',');
            foreach (string str in strs)
            {
               _SystemService._IConfigBigAreaService.Delete(int.Parse(str));
            }
            result.data = true;
            return result;
        }



        /// <summary>
        /// 删除多个实体类
        /// </summary>
        /// <param name="Ids">多个对象的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("MutilDelConfigBigAreaId")]
        public ResponseContext MutilDelConfigBigAreaId(string Ids)
        {
            ResponseContext result = new ResponseContext();
            TB_Config_BigAreaDTO model = new TB_Config_BigAreaDTO();
           result.data = _SystemService._IConfigBigAreaService.DeleteList(Ids);
            return result;
        }


        /// <summary>
        /// 获取地铁站数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSubWayStation")]
        //[Route("GetChezhanData")]
        public ResponseContext GetSubWayStation()
        {
            ResponseContext result = new ResponseContext();
            string where = "1";
            List<CheZhanDTO> lists = new List<CheZhanDTO>(); 
            Helper.ModelToDTO<TB_Config_BigArea, CheZhanDTO>(_SystemService._IConfigBigAreaService.GetListByConfigType(where), lists);
           result.data = lists;
            return result;
        }

        ///// <summary>
        ///// 查询数据(get,把参数散开：string para1,string para2)
        ///// </summary>
        ///// <param name="req">查询参数</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("SelectConfigBigArea")]
        //public ResponseContext SelectConfigBigArea() 
        //{
        //    ResponseContext result = new ResponseContext();

        //    TB_Config_BigAreaDTO model = new TB_Config_BigAreaDTO();
        //    string where = " 1=1 ";
        //   result.data= _SystemService._IConfigBigAreaService.GetList(where);
        //    return result;
        //}



        ///// <summary>
        ///// 查询数据(get,把参数散开：string para1,string para2)
        ///// </summary>
        ///// <param name="obj">查询参数</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("SelectConfigBigAreaByCondition")]
        //public ResponseContext SelectConfigBigAreaByCondition(string  obj)
        //{
        //    ResponseContext result = new ResponseContext();

        //    TB_Config_BigAreaDTO model = new TB_Config_BigAreaDTO();
        //    string where = " 1=1 ";
        //   result.data = _SystemService._IConfigBigAreaService.GetList(where);
        //    return result;
        //} 


        #endregion



        #region 位置模块

        /// <summary>
        /// 保存二级区域
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SaveConfigMidArea")]
        // [Route("addSmallArea")]
        public ResponseContext SaveConfigMidArea(TB_Config_MidAreaDTO data)
        {
            ResponseContext result = new ResponseContext();
            var obj = data;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var Content = data; 
                TB_Config_MidArea model = new TB_Config_MidArea();
                ModelCtrl.CopyModel(model, Content, null, null);
                model.Created_Time = DateTime.Now;
                if (_SystemService._IConfigMidAreaService.Add(model) < 1)
                {
                    result.code = ErrCode.Failure;
                    result.ret = -1;
                }
                ts.Complete();
            }
            return result;
        }
        /// <summary>
        /// 获取单个实体类
        /// </summary>
        /// <param name="id">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetConfigMidAreaId/{id}")]
        //[Route("GetConfigMidAreaId")]
        public ResponseContext GetConfigMidAreaId(string id)
        {
            ResponseContext result = new ResponseContext();
            TB_Config_MidAreaDTO model = new TB_Config_MidAreaDTO();
            string where = " id='" + id + "'";
            result.data = _SystemService._IConfigMidAreaService.GetList(where);
            return result;
        }

        /// <summary>
        /// 查询二级区域
        /// </summary>
        /// <param name="req">查询参数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("SelectConfigMidArea")]
        public ResponseContext SelectConfigMidArea()
        {
            ResponseContext result = new ResponseContext();

            TB_Config_MidAreaDTO model = new TB_Config_MidAreaDTO();
            string where = " 1=1 ";
           result.data = _SystemService._IConfigBigAreaService.GetList(where);
            return result;
        }




        /// <summary>
        /// 查询配置区域所有数据(不考虑分页)
        /// </summary> 
        /// Areacode
        /// <returns></returns>
        [HttpGet("SelectDicAreaData/{Areacode}")]
       // [Route("SelectDicArea")]
        public ResponseContext SelectDicAreaData(string Areacode)
        {
            ResponseContext result = new ResponseContext();  
            List<ChangQuDTO> DicAreaList = new List<ChangQuDTO>();
            DicAreaList.Add(new ChangQuDTO() { AreaName = "车站", Id = 1});
            DicAreaList.Add(new ChangQuDTO() { AreaName = "正线轨行区", Id = 2});
            DicAreaList.Add(new ChangQuDTO() { AreaName = "保护区", Id = 3});
            DicAreaList.Add(new ChangQuDTO() { AreaName = "车场生产区", Id = 4 }); 
           result.data = DicAreaList;
            return result;
        }


        /// <summary>
        /// 获取位置数据
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMidAreaQueryPageByParm")]
        public ResponseContext GetMidAreaQueryPageByParm([FromQuery] MidAreaQueryParm paras)
        {
             ResponseContext result = new ResponseContext();
            TB_Config_BigArea  model = null;
            // string where = " AreaName like '% " + paras.SearchName + "%'";
            //result.data = _SystemService._IConfigMidAreaService.GetList(where);
            // return result; 
            string where = "  1=1 ";
            if (!string.IsNullOrEmpty(paras.SearchName))
            {
                where += " and AreaName like '%" + paras.SearchName + "%'";
            }
            else if (!string.IsNullOrEmpty(paras.searchType))
            {
                where += " and  PID= ' " + paras.searchType + "'";
            }
            List<TB_Config_MidArea> list = _SystemService._IConfigMidAreaService.GetList(where);
            List<TB_Config_MidAreaDTO> results = new List<TB_Config_MidAreaDTO>();
            Helper.ModelToDTO<TB_Config_MidArea, TB_Config_MidAreaDTO>(list, results);
            foreach (var v in results)
            {
                model = _SystemService._IConfigBigAreaService.GetModel(v.PID);//.AreaName;
                if(model!=null)
                {
                    v.PName = model.AreaName;
                }
                v.PName = string.Empty;
            }
            result.data = results;
            result.total = results.Count();
            return result;
        }

        /// <summary>
        /// 删除单个实体类
        /// </summary>
        /// <param name="id">单个对象的ID</param>
        /// <returns></returns>
        [HttpGet("DelConfigMidAreaId/{id}")]
        //[Route("DelConfigMidAreaId")]
        public ResponseContext DelConfigMidAreaId(string id)
        {
             

            ResponseContext result = new ResponseContext();
            TB_Config_BigAreaDTO model = new TB_Config_BigAreaDTO();
            string[] strs = id.Split(',');
            foreach (string str in strs)
            {
                _SystemService._IConfigMidAreaService.Delete(int.Parse(str));
            }
            result.data = true;
            return result;
        }



        /// <summary>
        /// 删除位置多个实体类
        /// </summary>
        /// <param name="Ids">多个对象的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("MutilDelConfigMidAreaId")]
        public ResponseContext MutilDelConfigMidAreaId(string Ids)
        {
            ResponseContext result = new ResponseContext(); 
           result.data = _SystemService._IConfigMidAreaService.DeleteList(Ids);
            return result;
        }


        #endregion



        /// <summary>
        /// 查询配置区域所有数据(不考虑分页)
        /// </summary> 
        /// <returns></returns>
        [HttpGet]
        [Route("SelectConfigAreaDataDTO")]
        public ResponseContext SelectConfigAreaDataDTO()
        {
            ResponseContext result = new ResponseContext();
            ConfigAreaDataDTO model = new ConfigAreaDataDTO();

            List<DicAreaDTO> DicAreaList = new List<DicAreaDTO>();
            DicAreaList.Add(new DicAreaDTO() { AreaName = "车站", Id = 1, sort = 1 });
            DicAreaList.Add(new DicAreaDTO() { AreaName = "正线轨行区", Id = 2, sort = 2 });
            DicAreaList.Add(new DicAreaDTO() { AreaName = "保护区", Id = 3, sort = 2 });
            DicAreaList.Add(new DicAreaDTO() { AreaName = "车场生产区", Id = 4, sort = 4 });
            model.DicAreaList = DicAreaList;
            foreach (var v in model.DicAreaList)
            {
                v.BigAreaList = new List<BigAreaDTO>();
                Helper.ModelToDTO<TB_Config_BigArea, BigAreaDTO>(_SystemService._IConfigBigAreaService.GetListByConfigType(v.Id.ToString()), v.BigAreaList);
                if (v.Id == 1)
                {
                    foreach (var m in v.BigAreaList)
                    {
                        m.MidAreaList = new List<MidAreaDTO>();
                        Helper.ModelToDTO<TB_Config_MidArea, MidAreaDTO>(_SystemService._IConfigMidAreaService.GetListByPid(m.Id), m.MidAreaList);
                    }
                }
            }
           result.data = model;
            return result;
        }
    }
}