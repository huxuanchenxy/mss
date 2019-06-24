using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Core.Common
{
    public enum RESULT
    {
        FAIL,
        OK,
        REINSERT,
        NOTFOUNT
    }
    public class DataResult
    {
        public RESULT Result { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
