

namespace MSS.API.Operlog.Model.DTO
{
    public class UserOperationLogParm : BaseQueryParm
    {
        /// <summary>
        /// group_name模糊查询
        /// </summary>
        public string actionName { get; set; }
        public string userName { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string methodName { get; set; }
    }
}
