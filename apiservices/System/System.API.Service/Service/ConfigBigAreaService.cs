using System;
using System.Collections.Generic;
using System.Data;
using System.API.DAO.Interface;
using System.API.Model;

namespace System.API.Service
{
  public  class ConfigBigAreaService : IConfigBigAreaService
    {
        private IConfigBigAreaRepo<TB_Config_BigArea> _BigAreaRepo;
        public ConfigBigAreaService(IConfigBigAreaRepo<TB_Config_BigArea> BigAreaRepo)
        {
            _BigAreaRepo = BigAreaRepo;
        }
        public int Add(TB_Config_BigArea model)
        {
            return _BigAreaRepo.Add(model);
        }

        public bool Delete(int WF_TemID)
        {
            return _BigAreaRepo.Delete(WF_TemID);
        }

        public bool DeleteList(string WF_TemIDlist)
        {
            return _BigAreaRepo.DeleteList(WF_TemIDlist);
        }

        public bool Exists(int WF_TemID)
        {
            return _BigAreaRepo.Exists(WF_TemID);
        }

        public List<TB_Config_BigArea> GetList(string strWhere)
        {
            return _BigAreaRepo.GetList(strWhere);
        }

        public List<TB_Config_BigArea> GetList(int Top, string strWhere, string filedOrder)
        {
            return _BigAreaRepo.GetList(Top, strWhere, filedOrder);
        }

        public List<TB_Config_BigArea> GetListByConfigType(string filedOrder)
        {
            return _BigAreaRepo.GetListByConfigType(filedOrder);
        }

        public List<TB_Config_BigArea> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            return _BigAreaRepo.GetListByPage(strWhere, orderby, startIndex, endIndex);
        }

        public int GetMaxId()
        {
            return _BigAreaRepo.GetMaxId();
        }

        public TB_Config_BigArea GetModel(int WF_TemID)
        {
            return _BigAreaRepo.GetModel(WF_TemID);
        }

        public int GetRecordCount(string strWhere)
        {
            return _BigAreaRepo.GetRecordCount(strWhere);
        }

        public bool Update(TB_Config_BigArea model)
        {
            return _BigAreaRepo.Update(model);
        }
    }
}
