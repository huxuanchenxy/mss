using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
   public interface IWorkingApplicationRepo<T>
    {
        Task<WorkingApplication> GetByID(int id);


    }
}
