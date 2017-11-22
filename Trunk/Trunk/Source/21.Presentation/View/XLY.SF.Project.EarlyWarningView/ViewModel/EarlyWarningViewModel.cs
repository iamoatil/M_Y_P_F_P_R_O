/* ==============================================================================
* Description：EarlyWarningViewModel  
* Author     ：litao
* Create Date：2017/11/22 10:32:57
* ==============================================================================*/


using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XLY.SF.Project.EarlyWarningView
{
    class EarlyWarningViewModel : IEnable, INotifyPropertyChanged
    {
        public EarlyWarningViewModel()
        {
            Items = new List<IEnable>();
            var item = new EarlyWarningCollection()
            {
                Name = "涉及国安",
                IsEnable = true
            };
            CurrentSelected = item;
            Items.Add(item);
            item = new EarlyWarningCollection()
            {
                Name = "涉及治安",
                IsEnable = true
            };
            Items.Add(item);
            item = new EarlyWarningCollection()
            {
                Name = "涉及经济",
                IsEnable = true
            };
            Items.Add(item);
            item = new EarlyWarningCollection()
            {
                Name = "涉及民生",
                IsEnable = false
            };
            Items.Add(item);
            item = new EarlyWarningCollection()
            {
                Name = "自定义",
                IsEnable = false
            };
            Items.Add(item);


            IsEnable = true;
            Name = "默认开启智能检视";
        }

        public bool IsEnable { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 功能需要的数据所在的路径或目录
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 孩子数据
        /// </summary>
        public List<IEnable> Items { get; private set; }

        /// <summary>
        /// 当前选择的项目
        /// </summary>
        public EarlyWarningCollection CurrentSelected
        {
            get
            {
                return _currentSelected;

            }
            set
            {
                _currentSelected = value;
                OnPropertyChanged();
            }
        }
        private EarlyWarningCollection _currentSelected;

       
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

    }
}
