using System;
using System.Collections.Generic;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataPump;
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
                case EnumPump.LocalData:
                    dp = new LocalFileDataPump(pump);
                    break;
                case EnumPump.MTP:
                    dp = new MtpDataPump(pump);
                    break;
                default:
                    break;
            }
            dp?.Initialize();
            return dp;
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

        #region Private

        private static DataPumpBase GetUsbDataDataPump(Pump pump)
        {
            switch (pump.OSType)
            {
                case EnumOSType.Android when pump.Source is Device device:
                    if ((pump.Solution & PumpSolution.Downgrading) == PumpSolution.Downgrading)
                    {
                        return new AndroidDowngradingDataPump(pump);
                    }
                    if (device.IsRoot || device.Status == EnumDeviceStatus.Recovery)
                    {
                        return new AndroidUsbDataPump(pump);
                    }
                    return new AndroidUsbUnrootDataPump(pump);
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
