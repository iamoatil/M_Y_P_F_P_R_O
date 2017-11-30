using System;
using System.Collections.Generic;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataPump.Android;
using XLY.SF.Project.DataPump.IOS;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 与数据泵有关的扩展方法。
    /// </summary>
    public static class DataPumpExtensions
    {
        #region Methods

        #region Public

        /// <summary>
        /// 根据提取方式和系统类型获取指定的数据泵，并完成数据泵的初始化。
        /// </summary>
        /// <param name="pump">元数据。</param>
        /// <returns>数据泵。</returns>
        public static DataPumpBase GetDataPump(this Pump pump)
        {
            DataPumpBase dp = null;
            switch (pump.Type)
            {
                case EnumPump.USB:
                    dp = GetUsbDataDataPump(pump);
                    break;
                case EnumPump.Mirror:
                    dp = GetMirrorDataPump(pump);
                    break;
                default:
                    break;
            }
            dp?.Initialize();
            return dp;
        }

        /// <summary>
        /// 执行数据泵。
        /// </summary>
        /// <param name="dataPump">元数据。</param>
        /// <param name="source">数据源。</param>
        /// <param name="reporter">异步通知器。</param>
        /// <param name="items">提取项列表。</param>
        /// <returns>数据泵任务执行上下文。</returns>
        public static DataPumpExecutionContext Execute(this DataPumpBase dataPump, SourceFileItem source, MultiTaskReporterBase reporter, params ExtractItem[] items)
        {
            DataPumpExecutionContext context = dataPump.CreateContext(source);
            context.ExtractItems = items;
            DataPumpControllableExecutionContext contextEx = context as DataPumpControllableExecutionContext;
            if (contextEx != null) contextEx.Reporter = reporter;
            dataPump.Execute(context);
            return context;
        }

        /// <summary>
        /// 取消指定执行上下文关联的任务。
        /// </summary>
        /// <param name="context">执行上下文关联。</param>
        public static void Cancel(this DataPumpControllableExecutionContext context)
        {
            if (context == null) return;
            ControllableDataPumpBase dataPump = context.Owner as ControllableDataPumpBase;
            dataPump.Cancel(context);
        }

        /// <summary>
        /// 取消指定执行上下文关联的任务。
        /// </summary>
        /// <param name="context">执行上下文关联。</param>
        public static void Cancel(this DataPumpExecutionContext context)
        {
            DataPumpControllableExecutionContext contextEx = context as DataPumpControllableExecutionContext;
            if (contextEx == null) return;
            Cancel(contextEx);
        }

        /// <summary>
        /// 获取上下文的自定义数据。
        /// </summary>
        /// <param name="context"上下文。</param>
        /// <param name="name">数据名称。</param>
        /// <returns>数据值。</returns>
        public static Object GetContextData(this DataPumpExecutionContext context, String name)
        {
            return context[name];
        }

        /// <summary>
        /// 获取上下文的自定义数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="context">上下文。</param>
        /// <param name="name">数据名称。</param>
        /// <returns>数据值。</returns>
        public static T GetContextData<T>(this DataPumpExecutionContext context, String name)
        {
            if (context[name] == null) return default(T);
            return (T)context[name];
        }

        /// <summary>
        /// 设置上下文的自定义数据。
        /// </summary>
        /// <param name="context">上下文。</param>
        /// <param name="name">数据名称。</param>
        /// <param name="value">数据值。</param>
        public static void SetContextData(this DataPumpExecutionContext context, String name, Object value)
        {
            context[name] = value;
        }

        #endregion

        #region Internal

        /// <summary>
        /// 创建执行上下文。
        /// </summary>
        /// <param name="dataPump">数据泵。</param>
        /// <param name="metaData">元数据。</param>
        /// <param name="rootSavePath">保存路径。</param>
        /// <param name="source">数据源。</param>
        /// <param name="extractItems">提取项列表。</param>
        /// <param name="asyn">异步通知器。</param>
        /// <returns>执行上下文。</returns>
        internal static DataPumpExecutionContext CreateContext(this DataPumpBase dataPump, Pump metaData, String rootSavePath, SourceFileItem source, IEnumerable<ExtractItem> extractItems, DefaultMultiTaskReporter asyn = null)
        {
            DataPumpExecutionContext context = dataPump.CreateContext(source);
            context.ExtractItems = extractItems;
            DataPumpControllableExecutionContext contextEx = context as DataPumpControllableExecutionContext;
            if (contextEx != null) contextEx.Reporter = asyn;
            return contextEx;
        }

        #endregion

        #region Private

        private static DataPumpBase GetUsbDataDataPump(Pump pump)
        {
            switch (pump.OSType)
            {
                case EnumOSType.Android when pump.Source is Device device:
                    if (device.IsRoot || device.Status == EnumDeviceStatus.Recovery)
                    {
                        return new AndroidUsbDataPump(pump);
                    }
                    else if (device.Brand.ToSafeString().ToLower() == "vivo" || device.Manufacture.ToSafeString().ToLower() == "vivo")
                    {
                        return new AndroidVivoBackupDataPump(pump);
                    }
                    else
                    {
                        return new AndroidUsbUnrootDataPump(pump);
                    }
                case EnumOSType.IOS when pump.Source is Device:
                    return new IOSUsbDataPump(pump);
                default:
                    return null;
            }
        }

        private static DataPumpBase GetMirrorDataPump(Pump pump)
        {
            switch (pump.OSType)
            {
                case EnumOSType.Android:
                    return new AndroidMirrorDataPump(pump);
                case EnumOSType.IOS:
                    return new IOSMirrorDataPump(pump);
                default:
                    return null;
            }
        }

        #endregion

        #endregion
    }
}
