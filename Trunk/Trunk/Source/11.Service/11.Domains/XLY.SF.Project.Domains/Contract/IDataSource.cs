using System;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据源定义
    /// </summary>
    public interface IDataSource : ICheckedItem
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        Guid Key { get; set; }

        /// <summary>
        /// 插件定义信息
        /// </summary>
        DataParsePluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        IDataItems Items { get; set; }

        /// <summary>
        /// 构建数据
        /// </summary>
        void BuildParent();

        /// <summary>
        /// 数据提取时使用的任务路径 ，该属性用于修正任务包路径修改后导致的路径不正确的问题
        /// </summary>
        string DataExtractionTaskPath { get; set; }

        /// <summary>
        /// 当前任务路径
        /// </summary>
        string CurrentTaskPath { get; set; }

        new ICheckedItem Parent { get; set; }

        int Total { get; }

        int DeleteTotal { get; }

        void Filter<T>(params FilterArgs[] args);

        void SetCurrentPath(string path);
    }
}
