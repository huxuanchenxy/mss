
using Dapper.FluentMap.Mapping;
using MSS.Platform.Workflow.WebApi.Data;
using System;
using System.Collections.Generic;
using System.Data;

// Coded by admin 2019/9/26 16:46:46
namespace MSS.Platform.Workflow.WebApi.Model
{
    public static class Common
    {
        public const int WORK_TYPE = 111;
        public const int PM_TYPE = 116;

        public static string GetLastDay(int month, int year)
        {
            switch (month)
            {
                case 4:
                case 6:
                case 9:
                case 11:
                    return "30";
                case 2:
                    if (year % 4 == 0) return "29";
                    else return "28";
                default:
                    return "31";
            }
        }

    }

}