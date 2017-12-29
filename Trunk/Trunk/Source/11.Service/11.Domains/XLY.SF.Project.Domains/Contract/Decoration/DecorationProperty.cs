using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据装饰属性基本类型
    /// </summary>
    [Serializable]
    public abstract class DecorationProperty
    {
        public DecorationProperty(object defaultValue = null, object metaData = null, EventHandler<DecorationArgs> onChanged = null)
        {
            DefaultValue = defaultValue;
            MetaData = metaData;
            if (onChanged != null)
            {
                OnChanged += onChanged;
            }
        }
        /// <summary>
        /// 该属性的默认值
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// 附加的数据
        /// </summary>
        public object MetaData { get; set; }
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event EventHandler<DecorationArgs> OnChanged;
        /// <summary>
        /// 设置该属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <param name="isTriggerByChild"></param>
        public virtual void SetValue(object obj, object value, bool isTriggerByChild)
        {
        }
        /// <summary>
        /// 批量设置该属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public virtual void SetListValue(IEnumerable<object> obj, object value)
        {
        }
        /// <summary>
        /// 获取该属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual object GetValue(object obj)
        {
            return DefaultValue;
        }
        /// <summary>
        /// 触发改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        protected void TriggerEvent(object sender, DecorationArgs arg)
        {
            OnChanged?.Invoke(sender, arg);
        }

        public virtual void AddOnChangedEvent(Action<object, dynamic> action)
        {
            OnChanged += (s,arg)=> action(s, arg);
        }
    }

    public class DecorationArgs : EventArgs
    {
        public object Value { get; set; }
        public bool TriggerByChild { get; set; } = false;
    }

    /// <summary>
    /// 数据支持自定义的Key和附加数据
    /// </summary>
    public interface IDecoration
    {
        object GetMetaData(DecorationProperty dp);
        string GetKey(DecorationProperty dp);
    }
}
