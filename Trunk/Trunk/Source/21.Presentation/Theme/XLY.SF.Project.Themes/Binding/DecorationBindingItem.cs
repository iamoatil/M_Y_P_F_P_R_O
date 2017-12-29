using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 数据装饰属性的绑定项
    /// </summary>
    public class DecorationBindingItem : INotifyPropertyChanged
    {
        public DecorationBindingItem(object obj, object dp)
        {
            Source = obj;
            DP = dp;
            DP.AddOnChangedEvent(new Action<object, dynamic>(DP_OnChanged));
        }

        private void DP_OnChanged(object sender, dynamic e)
        {
            if (sender == Source)
                OnPropertyChanged("BindProperty");
        }
        /// <summary>
        /// 源对象
        /// </summary>
        private dynamic Source { get; set; }
        /// <summary>
        /// 附加的属性DecorationProperty
        /// </summary>
        private dynamic DP { get; set; }
        //public object BindProperty
        //{
        //    get => Source.GetValue(DP);
        //    set { Source.SetValue(DP, value); OnPropertyChanged(); }
        //}

        /// <summary>
        /// 用于绑定的属性（比如IsChecked）
        /// </summary>
        public object BindProperty
        {
            get => GetValueMethod.Invoke(null, new object[] { Source, DP });
            set { SetValueMethod.Invoke(null, new object[] { Source, DP, value }); OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private static MethodInfo GetValueMethod = null;
        private static MethodInfo SetValueMethod = null;
        static DecorationBindingItem()
        {
            var type = Assembly.Load("XLY.SF.Project.Domains").GetType("XLY.SF.Project.Domains.DecorationExtesion");
            SetValueMethod = type.GetMethod("SetValue", BindingFlags.Public | BindingFlags.Static);
            GetValueMethod = type.GetMethod("GetValue", BindingFlags.Public | BindingFlags.Static);
        }
    }
}
