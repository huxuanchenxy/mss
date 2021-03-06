﻿using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
   public interface IEqpHistoryRepo<T>
    {
        Task<List<EqpHistory>> ListByEqp(int id,bool isHide);
        Task<List<EqpHistory>> ListByType(string[] types);

        Task<List<QueryItem>> ListAllLocations();
        Task<int> SaveEqpHistory(List<EqpHistory> eqps);
        Task<int> SaveEqpHistory(EqpHistory eh);

        Task<DateTime> OnLineByEqp(int id);
    }
}
