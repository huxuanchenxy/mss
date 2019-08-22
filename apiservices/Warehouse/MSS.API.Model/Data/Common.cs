using System;
using System.Collections.Generic;
using System.Text;

using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public static class Common
    {
        public static class SWarehouse
        {
            public const string ADJUSTNO = "Warehouse_AdjustNo";
            public const string DELIVERYNO = "Warehouse_DeliveryNo";
            public const string RECEIVENO = "Warehouse_ReceiveNo";
            public const string MOVENO = "Warehouse_MoveNo";

            public static string GetOperationID(long number,string type)
            {
                switch(type)
                {
                    case ADJUSTNO:return "A"+string.Format("{0:00000000000}", number);
                    case DELIVERYNO: return "D" + string.Format("{0:00000000000}", number);
                    case RECEIVENO: return "R" + string.Format("{0:00000000000}", number);
                    case MOVENO: return "M" + string.Format("{0:00000000000}", number);
                    default: return "";
                }
            }
        }
    }
}
