using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.API.Core
{
    /// <summary>
    /// 通用实体操作类
    /// </summary>
    public class ModelCtrl
    {
 

     

        #region 操作没有做表映射的实体(非表映射的实体)

        /// <summary>
        /// 通过DataRow获实体对象，不区分大小写
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dr">DataRow</param>
        /// <returns>实体</returns>
        public static T GetModel<T>(DataRow dr) where T : new()
        {
            T en = new T();
            Type enType = typeof(T);

            //PropertyInfo[] arrPi = enType.GetProperties();  2013.01.05：修复dr[pi.name]，当pi.name列不存在会报异常，不是等于null!

            foreach (DataColumn dcCol in dr.Table.Columns)
            {
                if (dr[dcCol.ColumnName] == null || dr[dcCol.ColumnName].GetType().ToString() == "System.DBNull") continue;

                if (dcCol.ColumnName.ToLower() == "db_option_action") continue; //兼容旧实体

                PropertyInfo pi = enType.GetProperty(dcCol.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    pi.SetValue(en, dr[dcCol.ColumnName], null);
                }
            }

            return en;
        }

        /// <summary>
        /// 通过DataTable获实体列表
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns>实体列表</returns>
        public static List<T> SelectModel<T>(DataTable dt) where T : new()
        {
            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T en = GetModel<T>(dr);
                lst.Add(en);
            }

            return lst;
        }
 

     
       
 

        #endregion

        #region 实体间的复制

        /// <summary>
        /// 复制实体对象的属性值，相同属性名，而且属性类型相同才会复制
        /// </summary>
        /// <param name="objTarget">目标对象</param>
        /// <param name="objSource">源对象</param>
        /// <param name="strFields">指定哪些属性,多个用,分开,null=全部</param>
        /// <param name="strExcludeFields">排除属性,多个用,分开,为空不排除</param>
        /// <returns></returns>
        public static void CopyModel(object objTarget, object objSource, string strFields, string strExcludeFields)
        {
            if (objTarget == null || objSource == null) return;

            Type typeTarget = objTarget.GetType();
            Type typeSource = objSource.GetType();

            PropertyInfo[] arrPi = typeTarget.GetProperties();

            List<string> lstExcludeFields = (string.IsNullOrEmpty(strExcludeFields)) ? new List<string>() : new List<string>(strExcludeFields.Split(','));

            if (string.IsNullOrEmpty(strFields))
            {
                foreach (PropertyInfo pi in arrPi)
                {
                    if (lstExcludeFields != null && lstExcludeFields.Contains(pi.Name)) continue;
                    PropertyInfo objPi = typeSource.GetProperty(pi.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (objPi == null || objPi.PropertyType != pi.PropertyType) continue;
                    pi.SetValue(objTarget, objPi.GetValue(objSource, null), null);
                }
            }
            else
            {
                List<string> lstCols = (string.IsNullOrEmpty(strExcludeFields)) ? new List<string>() : new List<string>(strExcludeFields.Split(','));

                foreach (string str in lstCols)
                {
                    if (lstExcludeFields != null && lstExcludeFields.Contains(str)) continue;
                    PropertyInfo pi = typeTarget.GetProperty(str, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo objPi = typeSource.GetProperty(str, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (objPi == null || pi == null || objPi.PropertyType != pi.PropertyType) continue;
                    pi.SetValue(objTarget, objPi.GetValue(objSource, null), null);
                }
            }
        }

        /// <summary>
        /// 复制实体对象的属性值，相同属性名，而且属性类型相同才会复制
        /// </summary>
        /// <param name="objTarget">目标对象</param>
        /// <param name="objSource">源对象</param>
        /// <param name="strFields">指定哪些属性,多个用,分开,null=全部</param>
        /// <returns></returns>
        public static void CopyModel(object objTarget, object objSource, string strFields)
        {
            CopyModel(objTarget, objSource, strFields, null);
        }

        /// <summary>
        /// 复制实体对象的属性值，相同属性名，而且属性类型相同才会复制
        /// </summary>
        /// <param name="objTarget">目标对象</param>
        /// <param name="objSource">源对象</param>
        /// <returns></returns>
        public static void CopyModel(object objTarget, object objSource)
        {
            CopyModel(objTarget, objSource, null, null);
        }

        /// <summary>
        /// 复制实体对象的属性值，相同属性名，而且属性类型相同[兼容?前缀]才会复制
        /// </summary>
        /// <param name="objTarget">目标对象</param>
        /// <param name="objSource">源对象</param>
        /// <returns></returns>
        public static void CopyCompatibleModel(object objTarget, object objSource, string strFields, string strExcludeFields)
        {
            if (objTarget == null || objSource == null) return;

            Type typeTarget = objTarget.GetType();
            Type typeSource = objSource.GetType();

            PropertyInfo[] arrPi = typeTarget.GetProperties();

            List<string> lstExcludeFields = (string.IsNullOrEmpty(strExcludeFields)) ? new List<string>() : new List<string>(strExcludeFields.Split(','));

            if (string.IsNullOrEmpty(strFields))
            {
                foreach (PropertyInfo pi in arrPi)
                {
                    if (lstExcludeFields != null && lstExcludeFields.Contains(pi.Name)) continue;
                    PropertyInfo objPi = typeSource.GetProperty(pi.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (objPi == null || !objPi.PropertyType.ToString().Contains(pi.PropertyType.ToString())) continue;
                    pi.SetValue(objTarget, objPi.GetValue(objSource, null), null);
                }
            }
            else
            {
                List<string> lstCols = (string.IsNullOrEmpty(strExcludeFields)) ? new List<string>() : new List<string>(strExcludeFields.Split(','));

                foreach (string str in lstCols)
                {
                    if (lstExcludeFields != null && lstExcludeFields.Contains(str)) continue;
                    PropertyInfo pi = typeTarget.GetProperty(str, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo objPi = typeSource.GetProperty(str, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (objPi == null || pi == null || !objPi.PropertyType.ToString().Contains(pi.PropertyType.ToString())) continue;
                    pi.SetValue(objTarget, objPi.GetValue(objSource, null), null);
                }
            }
        }
        #endregion

        #region 通过集合对给实体赋值

        /// <summary>
        /// 通过键值对集合对获得实体对象的属性，在表单请求参数是很实用，不必一个个赋值
        /// </summary>
        /// <param name="objTarget">目标对象</param>
        /// <param name="nvc">键值对集合</param>
        /// <param name="strFields">指定哪些属性,多个用,分开,null=全部</param>
        /// <param name="strExcludeFields">排除属性,多个用,分开,为空不排除</param>
        public static void GetFromCollection(object objTarget, NameValueCollection nvc, string strFields, string strExcludeFields)
        {
            if (objTarget == null || nvc == null || nvc.Count < 1) return;

            Type objType = objTarget.GetType();

            List<string> lstExcludeFields = (string.IsNullOrEmpty(strExcludeFields)) ? new List<string>() : new List<string>(strExcludeFields.Split(','));

            if (string.IsNullOrEmpty(strFields))
            {
                //没有指定属性，取全部
                PropertyInfo[] arrPi = objType.GetProperties();
                foreach (PropertyInfo pi in arrPi)
                {
                    if (lstExcludeFields != null && lstExcludeFields.Contains(pi.Name)) continue;
                    if (nvc[pi.Name] != null)
                    {
                        SetPropertyValue(objTarget, pi, nvc[pi.Name].ToString());
                    }
                }
            }
            else
            {
                //指定了属性
                List<string> lstCols = new List<string>(strFields.Split(','));
                foreach (string strCol in lstCols)
                {
                    if (lstExcludeFields != null && lstExcludeFields.Contains(strCol)) continue;
                    PropertyInfo pi = objType.GetProperty(strCol, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null && nvc[strCol] != null)
                    {
                        SetPropertyValue(objTarget, pi, nvc[strCol].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 通过键值对集合对获得实体对象的属性，在表单请求参数是很实用，不必一个个赋值
        /// </summary>
        /// <param name="objTarget">目标对象</param>
        /// <param name="nvc">键值对集合</param>
        /// <param name="strFields">指定哪些属性,多个用,分开,null=全部</param>
        public static void GetFromCollection(object objTarget, NameValueCollection nvc, string strFields)
        {
            GetFromCollection(objTarget, nvc, strFields, null);
        }

        /// <summary>
        /// 通过键值对集合对获得实体对象的属性，在表单请求参数是很实用，不必一个个赋值
        /// </summary>
        /// <param name="objTarget">目标对象</param>
        /// <param name="nvc">键值对集合</param>
        public static void GetFromCollection(object objTarget, NameValueCollection nvc)
        {
            GetFromCollection(objTarget, nvc, null, null);
        }

        /// <summary>
        /// 根据属性类型，获值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pi"></param>
        /// <param name="strValue"></param>
        private static void SetPropertyValue(object obj, PropertyInfo pi, string strValue)
        {
            string strType = pi.PropertyType.ToString().ToLower();
            bool bl = false;
            switch (strType)
            {
                case "system.boolean":
                    bool blTemp = false;
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        strValue = strValue.Trim().ToLower() + "";
                        blTemp = (strValue == "1" || strValue == "yes" || strValue == "true" || strValue == "on");
                    }
                    pi.SetValue(obj, blTemp, null);
                    break;
                case "system.byte":
                    byte btTemp = 0;
                    bl = byte.TryParse(strValue, out btTemp);
                    if (bl) pi.SetValue(obj, btTemp, null);
                    break;
                case "system.int16":
                    short shTemp = 0;
                    bl = short.TryParse(strValue, out shTemp);
                    if (bl) pi.SetValue(obj, shTemp, null);
                    break;
                case "system.int32":
                    int intTemp = 0;
                    bl = int.TryParse(strValue, out intTemp);
                    if (bl) pi.SetValue(obj, intTemp, null);
                    break;
                case "system.int64":
                    long lngTemp = 0;
                    bl = long.TryParse(strValue, out lngTemp);
                    if (bl) pi.SetValue(obj, lngTemp, null);
                    break;
                case "system.decimal":
                    decimal decTemp = 0;
                    bl = decimal.TryParse(strValue, out decTemp);
                    if (bl) pi.SetValue(obj, decTemp, null);
                    break;
                case "system.double":
                    double dblTemp = 0;
                    bl = double.TryParse(strValue, out dblTemp);
                    if (bl) pi.SetValue(obj, dblTemp, null);
                    break;
                case "system.datetime":
                    DateTime dtTemp = DateTime.Now;
                    bl = DateTime.TryParse(strValue, out dtTemp);
                    if (bl) pi.SetValue(obj, dtTemp, null);
                    break;
                default:
                    pi.SetValue(obj, strValue, null);
                    break;
            }

        }

        #endregion 
        private static bool IsIgnore(PropertyInfo pi)
        {
            object[] igAttr = pi.GetCustomAttributes(false);
            if (igAttr.Length > 0)
            {
                if (igAttr[0].GetType().Name == "IgnoreAttribute")
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Convert a List{T} to a DataTable.
        /// </summary>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// Convert a List{T} to a DataRow.
        /// </summary>
        public static DataTable ToDataTable<T>(T item)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            var values = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null);
            }

            tb.Rows.Add(values);


            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
    }
}
