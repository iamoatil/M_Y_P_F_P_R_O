/* ==============================================================================
* Description：SettingManager  
* Author     ：litao
* Create Date：2017/11/25 15:48:18
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    class SettingManager: ISetting
    {
        public SettingManager()
        {
            Items = new List<ISetting>();
            var item = new SettingCollection()
            {
                Name= "CountrySafety",
                Description = "涉及国安",
                IsEnable = true
            };
            CurrentSelected = item;

            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "PublicSafety",
                Description = "涉及治安",
                IsEnable = true
            };
            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "EconomySafety",
                Description = "涉及经济",
                IsEnable = true
            };
            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "Livehood",
                Description = "涉及民生",
                IsEnable = false
            };
            Items.Add(item);
            item = new SettingCollection()
            {
                Name = "Custom",
                Description = "自定义 ",
                IsEnable = false
            };
            Items.Add(item);


            IsEnable = true;
            Name = "默认开启智能检视";
        }        

        public bool IsEnable { get; set; }

        public string Name { get; set; }

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
            }
        }
        private SettingCollection _currentSelected;
    }
}
