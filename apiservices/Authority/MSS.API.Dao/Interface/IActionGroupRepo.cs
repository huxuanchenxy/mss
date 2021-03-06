﻿using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MSS.API.Utility.Const;

namespace MSS.API.Dao.Interface
{
    public interface IActionGroupRepo<T> where T:BaseEntity
    {
        Task<MSSResult<ActionGroupView>> GetPageByParm(ActionGroupQueryParm parm);
        Task<ActionGroup> GetByID(int Id);

        Task<int> Add(ActionGroup actionGroup);

        Task<int> Update(ActionGroup actionGroup);

        Task<int> Delete(string[] ids);

        Task<List<ActionGroup>> GetAll();
    }
}
