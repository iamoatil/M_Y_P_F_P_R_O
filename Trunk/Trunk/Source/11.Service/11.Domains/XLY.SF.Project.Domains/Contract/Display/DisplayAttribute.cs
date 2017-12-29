using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Language;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    #region DisplayAttribute（属性的显示设置特性）

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DisplayDataType
    {
        TEXT,
        INTEGER,
        REAL,
        BLOB
    }

    /// <summary>
    /// 属性的显示设置特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayAttribute : Attribute
    {
        private string _Key = null;
        /// <summary>
        /// 文本的语言资源Key，如果不设置，则默认为"类名_属性名"
        /// </summary>
        public string Key
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Key))
                {
                    _Key = $"{ Owner.DeclaringType.Name}_{Owner.Name}";
                }
                return _Key;
            }
            set
            {
                _Key = value;
            }
        }


        /// <summary>
        /// 对齐方式，默认左对齐
        /// </summary>
        public EnumAlignment Alignment { get; set; }

        /// <summary>
        /// 列的数据类型
        /// </summary>
        public EnumColumnType ColumnType { get; set; }

        private string _text = null;
        /// <summary>
        /// 列标题，支持多语言显示
        /// </summary>
        public String Text
        {
            get { return _text ?? LanguageManager.Current[$"LanguageResource/DataEntityLanguage/{Key}"] ?? Owner.Name; }
            set { _text = value; }
        }

        /// <summary>
        /// 列宽
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 列序号，越小则排在越前面
        /// </summary>
        public int ColumnIndex { get; set; }

        public Type type { get; set; }

        /// <summary>
        /// 显示格式化
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 数据库中存储的列类型
        /// </summary>
        public DisplayDataType DataType
        {
            get
            {
                switch (ColumnType)
                {
                    case EnumColumnType.DateTime:
                        return DisplayDataType.REAL;
                    case EnumColumnType.Double:
                        return DisplayDataType.REAL;
                    case EnumColumnType.Int:
                        return DisplayDataType.INTEGER;
                    default:
                        return DisplayDataType.TEXT;
                }
            }
        }

        /// <summary>
        /// 列可见性
        /// </summary>
        public EnumDisplayVisibility Visibility { get; set; } = EnumDisplayVisibility.Visible;

        ///// <summary>
        ///// 是否支持检索，默认支持true
        ///// </summary>
        //public bool FullTextSearchEnable { get; set; }

        public PropertyInfo Owner { get; set; }

        /// <summary>
        /// 对应的属性名称
        /// </summary>
        public string PropertyName => Owner?.Name;


        /// <summary>
        /// 获取该特性对应属性的值
        /// </summary>
        public object GetValue(object instance)
        {
            if (this.Owner == null || instance == null)
            {
                return null;
            }

            return this.Owner.GetValue(instance);
        }

    }
    #endregion
}
