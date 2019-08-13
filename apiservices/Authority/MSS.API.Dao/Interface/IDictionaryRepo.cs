﻿using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MSS.API.Utility.Const;

namespace MSS.API.Dao.Interface
{
    public interface IDictionaryRepo<T> where T:BaseEntity
    {
        //Task<MSSResult<DictionaryView>> GetPageByParm(DictionaryQueryParm parm);
        //Task<Dictionary> GetByID(int Id);

        //Task<int> Add(Dictionary Dictionary);

        //Task<int> Update(Dictionary Dictionary);

        //Task<int> Delete(string[] ids);

        //Task<List<Dictionary>> GetSubByCode(string code);
        Task<List<DictionaryTree>> GetSubByCode(int pid);
        Task<List<DictionaryRelation>> GetByParent(int pid);
    }
}