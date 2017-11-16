using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 提取数据项
    /// </summary>
    public class ExtractItem
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// App名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string Icon { get; set; }

        public ExtractItem()
        {
            Name = string.Empty;
            GUID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        public ExtractItem DeepClone()
        {
            ExtractItem item = new ExtractItem();
            item.Name = Name;
            item.AppName = AppName;
            item.GroupName = GroupName;
            item.Icon = Icon;
            return item;
        }

        public override string ToString()
        {
            return $"{GroupName}-{AppName}";
        }

    }
}
