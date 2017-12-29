using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 应用数据解析插件配置信息
    /// </summary>
    [Serializable]
    [XmlRoot("plugin")]
    public class DataParsePluginInfo : AbstractZipPluginInfo
    {
        /// <summary>
        /// App名称
        /// </summary>
        [XmlElement("app")]
        public string AppName { get; set; }

        /// <summary>
        /// 数据泵类型
        /// </summary>
        [XmlElement("pump")]
        public EnumPump Pump { get; set; }

        /// <summary>
        /// 设备系统类型
        /// </summary>
        [XmlElement("devicetype")]
        public EnumOSType DeviceOSType { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        [XmlElement("manufacture")]
        public string Manufacture { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        [XmlIgnore]
        public EnumViewType ViewType { get; set; }

        /// <summary>
        /// 提取文件的路径
        /// </summary>
        [XmlIgnore]
        public SourceFileItems SourcePath { get; set; }

        /// <summary>
        /// 提取数据保存数据库路径
        /// </summary>
        [XmlIgnore]
        public string SaveDbPath { get; set; }

        [XmlArrayItem("value")]
        [XmlArray("source")]
        public List<string> SourcePathStr { get; set; }

        [XmlElement("data")]
        public List<DataView> DataView { get; set; }

        [XmlIgnore]
        [NonSerialized]
        private Device _Phone;

        /// <summary>
        /// 提取的手机，用于插件
        /// </summary>
        [XmlIgnore]
        public Device Phone { get => _Phone; set { _Phone = value; } }

        /// <summary>
        /// 插件文件名，相对路径，表示解析器调用的主插件
        /// </summary>
        public override string ScriptFile { get; set; }
        [XmlIgnore]
        public string FileFullPath { get; set; }

        public override void AfterReadConfigure()
        {
            base.AfterReadConfigure();
            if (SourcePathStr != null)
            {
                SourcePath = new SourceFileItems();
                SourcePath.AddItems(SourcePathStr);
            }
            if(DataView != null)
            {
                DataView.ForEach(dv => dv.Plugin = this);
            }
        }
    }
}
