﻿using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IWarnningSettingRepo<T> where T:BaseEntity
    {
        Task<EarlyWarnningSetting> SaveWarnningSetting(EarlyWarnningSetting setting);
        Task<bool> CheckWarnExist(EarlyWarnningSetting setting);
        Task<int> SaveWarnningSettingEx(List<EarlyWarnningSettingEx> settingEx);

        Task<EarlyWarnningSetting> UpdateWarnningSetting(EarlyWarnningSetting setting);

        Task<int> DeleteWarnningSettingEx(EarlyWarnningSetting setting);

        Task<bool> DeleteWarnningSetting(EarlyWarnningSetting setting);

        Task<List<EarlyWarnningSetting>> ListWarnningSettingByPage(int idx, int size, string sort, string order,
            int? eqpTypeID, string paramID);
        Task<int> Count(int? eqpTypeID, string paramID);

        Task<EarlyWarnningSetting> GetWarnningSettingByID(int id);

        Task<List<EarlyWarnningExType>> ListEarlyWarnningExType();
    }
}