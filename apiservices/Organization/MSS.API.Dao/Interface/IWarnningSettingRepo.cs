using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IWarnningSettingRepo<T> where T:BaseEntity
    {
        Task<EarlyWarnningSetting> SaveWarnningSetting(EarlyWarnningSetting warnSet);
        Task<bool> CheckWarnExist(EarlyWarnningSetting warnSet);
        Task<int> SaveWarnningSettingEx(List<EarlyWarnningSettingEx> settingEx);

        Task<EarlyWarnningSetting> UpdateWarnningSetting(EarlyWarnningSetting warnSet);
    }
}
