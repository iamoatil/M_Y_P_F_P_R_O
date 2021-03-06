﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 17:43:20
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    public static partial class BaseTypeExtension
    {
        #region 获取说明信息

        /// <summary>
        /// 获取枚举的描述信息(Descripion)。
        /// 支持位域，如果是位域组合值，多个按分隔符组合。
        /// </summary>
        public static string[] GetEnumFlagDescription(this Enum enumValue)
        {
            var enumTmpValues = enumValue.ToString().Split(',');
            string[] result = new string[enumTmpValues.Length];
            var enumType = enumValue.GetType();
            for (int i = 0; i < result.Length; i++)
            {
                var fieldTmp = enumType.GetField(enumTmpValues[i].Trim());
                var att = System.Attribute.GetCustomAttribute(fieldTmp, typeof(DescriptionAttribute), false);
                result[i] = att == null ? string.Empty : ((DescriptionAttribute)att).Description;
            }
            return result;
        }

        /// <summary>
        /// 获取枚举的描述信息(Descripion)。
        /// </summary>
        public static string GetDescription(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var fieldTmp = enumType.GetField(enumValue.ToString());
            var att = System.Attribute.GetCustomAttribute(fieldTmp, typeof(DescriptionAttribute), false);
            return att == null ? string.Empty : ((DescriptionAttribute)att).Description;
        }
        #endregion

        public static string GetDescriptions(this Enum @this, string separator = ",")
        {
            var names = @this.ToString().Split(',');
            string[] res = new string[names.Length];
            var type = @this.GetType();
            for (int i = 0; i < names.Length; i++)
            {
                var field = type.GetField(names[i].Trim());
                if (field == null) continue;
                res[i] = GetDescription(field);
            }
            return string.Join(separator, res);
        }
        private static string GetDescription(FieldInfo field)
        {
            var att = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), false);
            return att == null ? string.Empty : ((DescriptionAttribute)att).Description;
        }

        #region [位域]（位域枚举是否包含指定的值）

        /// <summary>
        /// 位域枚举是否包含指定的值，true：包含。
        /// .net中可以直接使用HasFlag判定
        /// </summary>
        public static bool Has(this Enum value, Enum target)
        {
            Type valueType = value.GetType();
            Type targetType = target.GetType();
            if (Enum.IsDefined(valueType, value) &&
                Enum.IsDefined(targetType, target) &&
                valueType == targetType)
            {
                return (value.GetHashCode() & target.GetHashCode()) == target.GetHashCode();
            }
            return false;
        }
        #endregion

        #region [位域]除去位域枚举指定的一个枚举（此方法不应该为扩展方法）
        ///// <summary>
        ///// 除去位域枚举指定的一个枚举，返回处理后的枚举值
        ///// </summary>
        //public static T Remove<T>(this Enum value, Enum target)
        //    where T : Enum
        //{
        //    Type valueType = value.GetType();
        //    Type targetType = target.GetType();
        //    if (Enum.IsDefined(valueType, value) &&
        //        Enum.IsDefined(targetType, target) &&
        //        valueType == targetType)
        //    {

        //    }

        //    var a = value.GetHashCode() & (~target.GetHashCode());
        //    return a.ToEnum<T>();
        //}
        #endregion

        #region ToEnum
        /// <summary>
        /// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
        /// </summary>
        public static T ToEnum<T>(this string name)
        {
            Type type = typeof(T);
            if (type.IsEnum)
            {
                return (T)Enum.Parse(typeof(T), name, true);
            }
            throw new InvalidCastException("必须是枚举类型才能转换。");
        }

        /// <summary>
        /// 尝试将一个字符串转换为指定的枚举类型。
        /// </summary>
        /// <typeparam name="T">枚举类型。</typeparam>
        /// <param name="str">字符串。</param>
        /// <param name="value">转换结果。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public static Boolean TryToEnum<T>(this String str,out T value)
            where T : struct
        {
            value = default(T);
            if (!typeof(T).IsEnum) return false;
            return Enum.TryParse<T>(str, out value);
        }

        /// <summary>
        /// 测试一个枚举中是否包含指定的枚举值。
        /// </summary>
        /// <param name="souce">要测试的枚举值。</param>
        /// <param name="matchTo">要匹配到的枚举。</param>
        /// <returns>包含返回true；否则返回false。</returns>
        public static Boolean IsSet(this Enum souce, Enum matchTo)
        {
            return matchTo.ToString().Contains(souce.ToString());
        }

        /// <summary>
        /// 测试一个枚举中是否包含指定的字符串。
        /// </summary>
        /// <param name="souce">要测试的字符串。</param>
        /// <param name="matchTo">要匹配到的枚举。</param>
        /// <returns>包含返回true；否则返回false。</returns>
        public static Boolean IsSet(this String source, Enum matchTo)
        {
            return matchTo.ToString().Contains(source ?? String.Empty,StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
        /// </summary>
        public static T ToEnum<T>(this int value)
        {
            return value.ToString().ToEnum<T>();
        }

        #endregion

        #region ToSafeEnum
        /// <summary>
        /// 将枚举常数的名称或数字值的字符串表示安全的转换成等效的枚举对象。
        /// </summary>
        public static T ToSafeEnum<T>(this string value, T defaultValue)
        {
            try
            {
                return value.ToEnum<T>();
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region ToEnumByValue

        /// <summary>
        /// 转换为枚举对象(不适用于位域值)
        /// </summary>
        public static T ToEnumByValue<T>(this object value)
        {
            return value.GetEnumNameByValue<T>().ToEnum<T>();
        }

        #endregion

        #region GetEnumNameByValue：通过枚举的值获取对应的枚举名称
        /// <summary>
        /// 通过枚举的值获取对应的枚举名称
        /// </summary>
        public static string GetEnumNameByValue<T>(this object value)
        {
            Type type = typeof(T);
            if (type.IsEnum)
            {
                return Enum.GetName(type, value);
            }
            throw new InvalidCastException("必须是枚举类型才能获取枚举名称。");
        }
        #endregion

    }
}
