/* ==============================================================================
* Description：SettingCollection  
* Author     ：litao
* Create Date：2017/11/22 11:01:20
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    class SettingCollection : ISetting
    {
        public SettingCollection()
        {
            Items = new List<ISetting>();
            var item = new SettingItem()
            {
                Name = "MD5(默认)",
                IsEnable = false
            };
            Items.Add(item);
             item = new SettingItem()
            {
                Name = "关键字(默认)",
                IsEnable = true
            };
            Items.Add(item);
            item = new SettingItem()
            {
                Name = "URL(默认)",
                IsEnable = true
            };
            Items.Add(item);
            item = new SettingItem()
            {
                Name = "电话(默认)",
                IsEnable = true
            };
            Items.Add(item);
        }

        public bool IsEnable { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 功能需要的数据所在的路径或目录
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 孩子数据
        /// </summary>
        public List<ISetting> Items { get; private set; }
    }
}
