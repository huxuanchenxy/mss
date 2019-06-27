using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.API.Core.Function
{
    public class Helper
    {
        /// <summary>
        /// 获取枚举描述值
        /// </summary>
        /// <returns></returns>
        public static string GetEnumDescription(System.Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            if (field == null)
            {
                return str;
            }
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }

        #region 类型转换
        /// <summary>
        /// List类型转换
        /// </summary>
        /// <typeparam name="T">源对象</typeparam>
        /// <typeparam name="T1">目标对象</typeparam>
        /// <param name="list">源数据</param>
        /// <param name="ListDTO">目标数据</param>
        public static void ModelToDTO<T, T1>(List<T> list, List<T1> ListDTO)
        {
            foreach (var item in list)
            {
                T1 ModelDTO = (T1)Activator.CreateInstance(typeof(T1));
                ModelCtrl.CopyModel(ModelDTO, item);
                ListDTO.Add(ModelDTO);
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T">源对象</typeparam>
        /// <typeparam name="T1">目标对象</typeparam>
        /// <param name="list"></param>
        /// <param name="ListDTO"></param>
        public static void ModelToDTOBySingle<T, T1>(T list, T1 ListDTO)
        {
            T1 ModelDTO = (T1)Activator.CreateInstance(typeof(T1));
            ModelCtrl.CopyModel(ModelDTO, list);
            ListDTO = ModelDTO;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T">源对象</typeparam>
        /// <typeparam name="T1">目标对象</typeparam>
        /// <param name="list"></param>
        /// <param name="ListDTO"></param>
        public static T1 ModelToDTOByone<T, T1>(T list, T1 ListDTO)
        {
            T1 ModelDTO = (T1)Activator.CreateInstance(typeof(T1));
            ModelCtrl.CopyModel(ModelDTO, list);
            return ModelDTO;
        }

        /// <summary>
        /// 计算两个日期的天数
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endStartDate"></param>
        /// <returns></returns>
        public static int DiffDates(DateTime startDate, DateTime endStartDate)
        {
            DateTime start = Convert.ToDateTime(startDate.ToShortDateString());
            DateTime end = Convert.ToDateTime(endStartDate.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            int days = sp.Days;
            return days;
        }
        #endregion

       
    }
}