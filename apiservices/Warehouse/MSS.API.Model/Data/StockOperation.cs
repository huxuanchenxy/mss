using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class StockOperation
    {
        public int ID { get; set; }
        public string OperationID { get; set; }
        public int Type { get; set; }
        public int Reason { get; set; }
        public string ReasonName { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public string Remark { get; set; }
        public int? FromWarehouse { get; set; }
        public string FromWarehouseName { get; set; }
        public string PickerName { get; set; }
        public int? Picker { get; set; }
        public string PickerDeptName { get; set; }
        public string SupplierName { get; set; }
        public int? Supplier { get; set; }
        public string Agreement { get; set; }
        public string BudgetDeptName { get; set; }
        public int? BudgetDept { get; set; }
        public string BudgetItems { get; set; }
        public string CreatedName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string DetailList { get; set; }
    }

    public class StockOperationMap : EntityMap<StockOperation>
    {
        public StockOperationMap()
        {
            Map(o => o.OperationID).ToColumn("operation_id");
            Map(o => o.Type).ToColumn("type");

            Map(o => o.Reason).ToColumn("reason");
            Map(o => o.ReasonName).ToColumn("name");
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("wname");
            Map(o => o.FromWarehouse).ToColumn("from_warehouse");
            Map(o => o.FromWarehouseName).ToColumn("fromWName");
            Map(o => o.Picker).ToColumn("picker");
            Map(o => o.PickerName).ToColumn("pname");
            Map(o => o.PickerDeptName).ToColumn("pdname");
            Map(o => o.Supplier).ToColumn("supplier");
            Map(o => o.SupplierName).ToColumn("sname");
            Map(o => o.BudgetDept).ToColumn("budget_dept");
            Map(o => o.BudgetDeptName).ToColumn("bname");

            Map(o => o.Remark).ToColumn("remark");
            Map(o => o.Agreement).ToColumn("agreement");
            Map(o => o.BudgetItems).ToColumn("budget_items");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("user_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
        }
    }

    public class StockOperationQueryParm:BaseQueryParm
    {
        public int? SearchType { get; set; }
        public int? SearchReason { get; set; }
        public int? SearchFromWarehouse { get; set; }
        public int? SearchWarehouse { get; set; }
        public int? SearchSupplier { get; set; }
        public int? SearchPicker { get; set; }
        public DateTime? SearchStart { get; set; }
        public DateTime? SearchEnd { get; set; }
    }

    public class StockOperationDetail
    {
        public int ID { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public string SparePartsModel { get; set; }
        public string SparePartsUnit { get; set; }
        public int OrderNo { get; set; }
        public int CountNo { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public int Currency { get; set; }
        public string CurrencyName { get; set; }
        public string Invoice { get; set; }
        public string Purchase { get; set; }
        public string Repair { get; set; }
        public int Operation { get; set; }
        public double? ExchangeRate { get; set; }
        public double? TotalAmount { get; set; }
        public string Remark { get; set; }
    }

    public class StockOperationDetailMap : EntityMap<StockOperationDetail>
    {
        public StockOperationDetailMap()
        {
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("name");
            Map(o => o.SparePartsModel).ToColumn("model");
            Map(o => o.SparePartsUnit).ToColumn("unit");

            Map(o => o.OrderNo).ToColumn("order_no");
            Map(o => o.CountNo).ToColumn("count_no");
            Map(o => o.UnitPrice).ToColumn("unit_price");
            Map(o => o.Amount).ToColumn("amount");
            Map(o => o.Currency).ToColumn("currency");
            Map(o => o.CurrencyName).ToColumn("cname");
            Map(o => o.Invoice).ToColumn("invoice");
            Map(o => o.Purchase).ToColumn("purchase");
            Map(o => o.Repair).ToColumn("repair");
            Map(o => o.Operation).ToColumn("operation");
            Map(o => o.ExchangeRate).ToColumn("exchange_rate");
            Map(o => o.TotalAmount).ToColumn("total_amount");
            Map(o => o.Remark).ToColumn("remark");
        }
    }


}
