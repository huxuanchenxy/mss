using System;
using System.Collections.Generic;
using System.Data;
using System.API.DAO.Interface;
using System.API.Model;

namespace System.API.Service
{
  public  class ConfigMidAreaService : IConfigMidAreaService
    {
        private IConfigMidAreaRepo<TB_Config_MidArea> _MidAreaRepo;
        public ConfigMidAreaService(IConfigMidAreaRepo<TB_Config_MidArea> MidAreaRepo)
        {
            _MidAreaRepo = MidAreaRepo;
        }
        public int Add(TB_Config_MidArea model)
        {
            return _MidAreaRepo.Add(model);
        }

        public bool Delete(int WF_TemID)
        {
            return _MidAreaRepo.Delete(WF_TemID);
        }

        public bool DeleteList(string WF_TemIDlist)
        {
            return _MidAreaRepo.DeleteList(WF_TemIDlist);
        }

        public bool Exists(int WF_TemID)
        {
            return _MidAreaRepo.Exists(WF_TemID);
        }

        public List<TB_Config_MidArea> GetList(string strWhere)
        {
            return _MidAreaRepo.GetList(strWhere);
        }

        public List<TB_Config_MidArea> GetListByPid(int Pid)
        {
            return _MidAreaRepo.GetListByPid(Pid);
        }
        public List<TB_Config_MidArea> GetList(int Top, string strWhere, string filedOrder)
        {
            return _MidAreaRepo.GetList(Top, strWhere, filedOrder);
        }

        public List<TB_Config_MidArea> GetListByPage(string strWhere, string sort, string orderby, int page, int size)
        {
            return _MidAreaRepo.GetListByPage(strWhere, sort, orderby, page, size);
        }

        public int GetMaxId()
        {
            return _MidAreaRepo.GetMaxId();
        }

        public TB_Config_MidArea GetModel(int WF_TemID)
        {
            return _MidAreaRepo.GetModel(WF_TemID);
        }

        public int GetRecordCount(string strWhere)
        {
            return _MidAreaRepo.GetRecordCount(strWhere);
        }

        public bool Update(TB_Config_MidArea model)
        {
            return _MidAreaRepo.Update(model);
        }
    }
}
