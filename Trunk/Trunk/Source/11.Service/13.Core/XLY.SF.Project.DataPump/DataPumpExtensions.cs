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
        /// 执行数据泵。
        /// </summary>
        /// <param name="pump">元数据。</param>
        /// <param name="source">数据源。</param>
        /// <param name="reporter">异步通知器。</param>
        /// <param name="items">提取项列表。</param>
        /// <returns>数据泵任务执行上下文。</returns>
        public static DataPumpExecutionContext Execute(this Pump pump, SourceFileItem source, MultiTaskReporterBase reporter, params ExtractItem[] items)
        {
            DataPumpBase dataPump = pump.GetDataPump();
            DataPumpExecutionContext context = dataPump.CreateContext(pump, source);
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
        /// 根据提取方式和系统类型获取指定的数据泵。
        /// </summary>
        /// <param name="key">元数据。</param>
        /// <returns>数据泵。</returns>
        internal static DataPumpBase GetDataPump(this Pump key)
        {
            switch (key.Type)
            {
                case EnumPump.USB:
                    return GetUsbDataDataPump(key, EnumPump.USB.GetHashCode() ^ key.OSType.GetHashCode());
                case EnumPump.Mirror:
                    return GetMirrorDataPump(key, EnumPump.Mirror.GetHashCode() ^ key.OSType.GetHashCode());
                default:
                    throw new NotSupportedException(key.Type.ToString());
            }
        }

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
            DataPumpExecutionContext context = dataPump.CreateContext(metaData, source);
            context.ExtractItems = extractItems;
            DataPumpControllableExecutionContext contextEx = context as DataPumpControllableExecutionContext;
            if (contextEx != null) contextEx.Reporter = asyn;
            return contextEx;
        }

        #endregion

        #region Private

        private static DataPumpBase GetUsbDataDataPump(Pump key, Int32 hash)
        {
            var device = (Device)key.Source;
            DataPumpBase dataPump = null;
            switch (key.OSType)
            {
                case EnumOSType.Android:
                    if (device.IsRoot || device.Status == EnumDeviceStatus.Recovery)
                    {
                        dataPump = new AndroidUsbDataPump();
                    }
                    else if (device.Brand.ToSafeString().ToLower() == "vivo" || device.Manufacture.ToSafeString().ToLower() == "vivo")
                    {
                        dataPump = new AndroidVivoBackupDataPump();
                    }
                    else
                    {
                        dataPump = new AndroidUsbUnrootDataPump();
                    }
                    break;
                case EnumOSType.IOS:
                    dataPump = new IOSUsbDataPump();
                    break;
                default:
                    throw new NotSupportedException(key.OSType.ToString());
            }
            return dataPump;
        }

        private static DataPumpBase GetMirrorDataPump(Pump key, Int32 hash)
        {
            DataPumpBase dataPump = null;
            switch (key.OSType)
            {
                case EnumOSType.Android:
                    dataPump = new AndroidMirrorDataPump();
                    break;
                case EnumOSType.IOS:
                    dataPump = new IOSMirrorDataPump();
                    break;
                default:
                    throw new NotSupportedException(key.OSType.ToString());
            }
            return dataPump;
        }

        #endregion

        #endregion
    }
}
