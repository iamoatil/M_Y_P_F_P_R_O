/* ==============================================================================
* Description：SettingViewModel  
* Author     ：litao
* Create Date：2017/11/22 10:32:57
* ==============================================================================*/


using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XLY.SF.Project.EarlyWarningView
{
    class SettingViewModel : ISetting, INotifyPropertyChanged
    {
        public SettingViewModel()
        {
            Items = new List<ISetting>();
            var item = new SettingCollection()
            {
                Name = "涉及国安",
                IsEnable = true
            };
            CurrentSelected = item;
            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "涉及治安",
                IsEnable = true
            };
            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "涉及经济",
                IsEnable = true
            };
            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "涉及民生",
                IsEnable = false
            };
            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "自定义",
                IsEnable = false
            };
            Items.Add(item);


            IsEnable = true;
            Name = "默认开启智能检视";
        }

        /// <summary>
        /// 智能预警
        /// </summary>
        EarlyWarning _earlyWarning { get { return EarlyWarning.Instance; } }

        public bool IsEnable { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 功能需要的数据所在的路径或目录
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 孩子数据
        /// </summary>
        public List<ISetting> Items { get; private set; }

        /// <summary>
        /// 当前选择的项目
        /// </summary>
        public SettingCollection CurrentSelected
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
        private SettingCollection _currentSelected;

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
