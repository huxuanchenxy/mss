using System;
using System.Collections.Generic;
using System.Text;

using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public static class Common
    {
        public enum StockOperationType
        {
            adjust = 70,
            delivery = 69,
            receive = 68,
            move = 71
        }
        public static string GetRedisKey(StockOperationType type)
        {
            switch (type)
            {
                case StockOperationType.adjust: return "Warehouse_AdjustNo";
                case StockOperationType.delivery: return "Warehouse_DeliveryNo";
                case StockOperationType.receive: return "Warehouse_ReceiveNo";
                case StockOperationType.move: return "Warehouse_MoveNo";
                default: return "";
            }
        }

        public static string GetOperationID(long number, StockOperationType type)
        {
            switch (type)
            {
                case StockOperationType.adjust: return "A" + string.Format("{0:00000000000}", number);
                case StockOperationType.delivery: return "D" + string.Format("{0:00000000000}", number);
                case StockOperationType.receive: return "R" + string.Format("{0:00000000000}", number);
                case StockOperationType.move: return "M" + string.Format("{0:00000000000}", number);
                default: return "";
            }
        }
    }
}
