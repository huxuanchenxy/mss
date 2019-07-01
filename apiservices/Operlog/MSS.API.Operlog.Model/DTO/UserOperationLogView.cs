

using MSS.API.Operlog.Model.Data;

namespace MSS.API.Operlog.Model.DTO
{
    public class UserOperationLogView : UserOperationLog
    {
        public string created_name { get; set; }
        public string updated_name { get; set; }
    }
}
