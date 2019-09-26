using System;
using System.Collections.Generic;
using System.Text;

using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public static class Common
    {
        /// <summary>
        /// 库存操作状态
        /// </summary>
        public enum StockOperationStatus
        {
            /// <summary>
            /// 其他
            /// </summary>
            Other = 0,
            /// <summary>
            /// 未归还
            /// </summary>
            NoReturn = 1,
        }

        /// <summary>
        /// 库存操作类型
        /// </summary>
        public enum StockOperationType
        {
            /// <summary>
            /// 调整
            /// </summary>
            Adjust = 70,
            /// <summary>
            /// 物资发放
            /// </summary>
            Delivery = 69,
            /// <summary>
            /// 物资接收
            /// </summary>
            Receive = 68,
            /// <summary>
            /// 移库
            /// </summary>
            Move = 71
        }

        /// <summary>
        /// 库存操作（事务）原因
        /// </summary>
        public enum StockOptDetailType
        {
            /// <summary>
            /// 采购退货
            /// </summary>
            PurchaseReturn = 72,
            /// <summary>
            /// 采购接收
            /// </summary>
            PurchaseReceive = 73,
            /// <summary>
            /// 物资领用
            /// </summary>
            Distribution = 74,
            /// <summary>
            /// 退料
            /// </summary>
            MaterialReturn = 75,
            /// <summary>
            /// 移库
            /// </summary>
            MoveTo = 76,
            /// <summary>
            /// 盘盈
            /// </summary>
            InventoryProfit = 77,
            /// <summary>
            /// 盘亏
            /// </summary>
            InventoryLoss = 78,
            /// <summary>
            /// 故障件退库
            /// </summary>
            TroubleReturn = 79,
            /// <summary>
            /// 故障件送修出库
            /// </summary>
            TroubleRepair = 80,
            /// <summary>
            /// 物资借用
            /// </summary>
            MaterialLend = 81,
            /// <summary>
            /// 故障件移库
            /// </summary>
            TroubleMoveTo = 82,
            /// <summary>
            /// 其他接收
            /// </summary>
            OtherReceive = 84,
            /// <summary>
            /// 正常件送检出库
            /// </summary>
            Inspection = 85,
            /// <summary>
            /// 正常件送检归还
            /// </summary>
            InspectionReturn = 86,
            /// <summary>
            /// 故障件报废
            /// </summary>
            TroubleScrap = 87,
            /// <summary>
            /// 存货报废
            /// </summary>
            InStockScrap = 88,
            /// <summary>
            /// 借用归还
            /// </summary>
            lendReturn = 92,
            /// <summary>
            /// 返修件接收入库
            /// </summary>
            RepairReceive = 93
        }
        /// <summary>
        /// 库存操作改变
        /// </summary>
        public enum StockChange
        {
            /// <summary>
            /// 保持不变
            /// </summary>
            NoChange = 0,
            /// <summary>
            /// 加法
            /// </summary>
            Plus = 1,
            /// <summary>
            /// 减法
            /// </summary>
            Subtract = 2
        }

        /// <summary>
        /// 物资状态
        /// </summary>
        public enum StockStatus
        {
            /// <summary>
            /// 多个物资的状态
            /// </summary>
            None = 0,
            /// <summary>
            /// 正常
            /// </summary>
            Normal = 99,
            /// <summary>
            /// 故障
            /// </summary>
            Trouble = 100,
            /// <summary>
            /// 报废
            /// </summary>
            Scrap = 101,
            /// <summary>
            /// 外借
            /// </summary>
            Lend = 102,
            /// <summary>
            /// 送修
            /// </summary>
            Repair = 103,
            /// <summary>
            /// 送检
            /// </summary>
            Inspection = 104,
            /// <summary>
            /// 耗尽
            /// </summary>
            Exhaust = 105,
            /// <summary>
            /// 已退货
            /// </summary>
            Returned = 106,
            /// <summary>
            /// 盘亏
            /// </summary>
            Loss = 107,
            /// <summary>
            /// 盘盈
            /// </summary>
            Profit = 108
        }

        public static string GetRedisKey(StockOperationType type)
        {
            switch (type)
            {
                case StockOperationType.Adjust: return "Warehouse_AdjustNo";
                case StockOperationType.Delivery: return "Warehouse_DeliveryNo";
                case StockOperationType.Receive: return "Warehouse_ReceiveNo";
                case StockOperationType.Move: return "Warehouse_MoveNo";
                default: return "";
            }
        }

        public static string GetOperationID(long number, StockOperationType type)
        {
            switch (type)
            {
                case StockOperationType.Adjust: return "A" + string.Format("{0:00000000000}", number);
                case StockOperationType.Delivery: return "D" + string.Format("{0:00000000000}", number);
                case StockOperationType.Receive: return "R" + string.Format("{0:00000000000}", number);
                case StockOperationType.Move: return "M" + string.Format("{0:00000000000}", number);
                default: return "";
            }
        }
    }
}
