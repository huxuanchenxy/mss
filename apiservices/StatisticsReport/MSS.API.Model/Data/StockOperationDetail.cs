
using Dapper.FluentMap.Mapping;
using MSS.API.Model.Data;
using System.Collections.Generic;

// Coded by admin 2019/11/29 17:02:07
namespace MSS.API.Model.Data
{
    public class StockOperationDetailParm
    {
        public int StockOperationType { get; set; }
        public int SparePartsType { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string legendData { get; set; }
    }
    public class StockOperationDetailPageView
    {
        public List<StockOperationDetail> rows { get; set; }
        public int total { get; set; }
    }

    public class StockOperationDetail : BaseEntity
    {
        public int Id { get; set; }
        public string Entity { get; set; }
        public int SpareParts { get; set; }
        public int OrderNo { get; set; }
        public int CountNo { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public int StorageLocation { get; set; }
        public int FromStorageLocation { get; set; }
        public string Invoice { get; set; }
        public System.DateTime LifeDate { get; set; }
        public int Supplier { get; set; }
        public int WorkingOrder { get; set; }
        public string Purchase { get; set; }
        public string Repair { get; set; }
        public string SomeOrder { get; set; }
        public int Operation { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remark { get; set; }
        public int StockDetail { get; set; }
        //public int StockOperationDetail { get; set; }
        public int ReturnNo { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public int EqpType { get; set; }
        public int SparePartsId { get; set; }
    }

    public class StockOperationDetailMap : EntityMap<StockOperationDetail>
    {
        public StockOperationDetailMap()
        {
            Map(o => o.Id).ToColumn("id");
            Map(o => o.Entity).ToColumn("entity");
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.OrderNo).ToColumn("order_no");
            Map(o => o.CountNo).ToColumn("count_no");
            Map(o => o.UnitPrice).ToColumn("unit_price");
            Map(o => o.Amount).ToColumn("amount");
            Map(o => o.Currency).ToColumn("currency");
            Map(o => o.StorageLocation).ToColumn("storage_location");
            Map(o => o.FromStorageLocation).ToColumn("from_storage_location");
            Map(o => o.Invoice).ToColumn("invoice");
            Map(o => o.LifeDate).ToColumn("life_date");
            Map(o => o.Supplier).ToColumn("supplier");
            Map(o => o.WorkingOrder).ToColumn("working_order");
            Map(o => o.Purchase).ToColumn("purchase");
            Map(o => o.Repair).ToColumn("repair");
            Map(o => o.SomeOrder).ToColumn("some_order");
            Map(o => o.Operation).ToColumn("operation");
            Map(o => o.ExchangeRate).ToColumn("exchange_rate");
            Map(o => o.TotalAmount).ToColumn("total_amount");
            Map(o => o.Remark).ToColumn("remark");
            Map(o => o.StockDetail).ToColumn("stock_detail");
            //Map(o => o.StockOperationDetail).ToColumn("stock_operation_detail");
            Map(o => o.ReturnNo).ToColumn("return_no");
            Map(o => o.Status).ToColumn("status");
            Map(o => o.Name).ToColumn("name");
            Map(o => o.EqpType).ToColumn("eqp_type");
            Map(o => o.SparePartsId).ToColumn("spare_parts_id");
        }
    }

    public class SpareParts : BaseEntity
    {
        public int id { get; set; }
        public string name { get; set; }

    }

}