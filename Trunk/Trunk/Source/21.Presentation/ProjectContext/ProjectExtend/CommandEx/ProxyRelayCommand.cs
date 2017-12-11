using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Project.Extension.CommandEx;
using XLY.SF.Project.ViewDomain.Model;

namespace GalaSoft.MvvmLight.CommandWpf
{
    #region 基类

    /// <summary>
    /// 代理Command基类
    /// </summary>
    public class ProxyRelayCommandBase
    {
        protected ProxyRelayCommandBase()
        {

        }

        /// <summary>
        /// 操作日志
        /// </summary>
        protected SysCommonMsgArgs logArgs;

        /// <summary>
        /// 获取截图目标回调
        /// </summary>
        protected Func<object> _getContainerCallback;

        /// <summary>
        /// 当前命令对应的模块
        /// </summary>
        protected string _curCmdModelName;

        /// <summary>
        /// 界面绑定命令
        /// </summary>
        public ICommand ViewExecuteCmd { get; protected set; }

        /// <summary>
        /// 写入操作日志
        /// </summary>
        /// <param name="operationResult">操作结果内容</param>
        public void WriteOperationLog(CmdLogModel log)
        {
            if (log != null)
            {
                //屏幕截图
                string screenShotPath = string.Empty;
                if (_getContainerCallback != null)
                {
                    var container = _getContainerCallback();
                    if (container != null && container is FrameworkElement)
                        screenShotPath = SystemContext.Instance.SaveOperationImageByWindow(container as FrameworkElement);
                }

                //记录日志【此处保存全路径】
                SystemContext.Instance.AddOperationLog(new ObtainEvidenceLogModel()
                {
                    OperationModel = log.ModelName,
                    OpContent = log.OperationResult,
                    ImageNameForScreenShot = screenShotPath,
                    OpTime = DateTime.Now,
                });
            }
        }
    }

    #endregion

    /// <summary>
    /// RelayCommand代理，增加日志的输出.如不需要日志输出，可直接使用RelayCommand类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProxyRelayCommand<T> : ProxyRelayCommandBase
    {
        /// <summary>
        /// 内部使用回调
        /// </summary>
        private Func<T, string> _callback;

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="screenShotTarget">是否需要截图</param>
        public ProxyRelayCommand(Func<T, string> cmdByReportOpInfo, string modelName, Func<T, bool> canExecute = null)
        {
            _callback = cmdByReportOpInfo ??
                throw new ArgumentNullException(string.Format("功能方法【cmdByReportOpInfo】不能为NULL"));

            _curCmdModelName = modelName;
            logArgs = new SysCommonMsgArgs(XLY.SF.Framework.Core.Base.SystemKeys.WirteOperationLog);
            if (canExecute != null)
                ViewExecuteCmd = new RelayCommand<T>(ConcreateExecute, canExecute);
            else
                ViewExecuteCmd = new RelayCommand<T>(ConcreateExecute);
        }

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志同时截图保存
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="getViewContainerCallback">获取ViewContainer回调</param>
        /// <param name="canExecute"></param>
        public ProxyRelayCommand(Func<T, string> cmdByReportOpInfo, string modelName, Func<object> getViewContainerCallback, Func<T, bool> canExecute)
            : this(cmdByReportOpInfo, modelName, canExecute)
        {
            _getContainerCallback = getViewContainerCallback;
        }

        private void ConcreateExecute(T t)
        {
            var logResult = _callback(t);
#if !DEBUG
            if (!string.IsNullOrWhiteSpace(logResult))
                base.WriteOperationLog(new CmdLogModel() { ModelName = _curCmdModelName, OperationResult = logResult });
#endif
        }
    }

    /// <summary>
    /// RelayCommand代理，增加日志的输出.如不需要日志输出，可直接使用RelayCommand类
    /// </summary>
    public class ProxyRelayCommand : ProxyRelayCommandBase
    {
        /// <summary>
        /// 内部使用回调
        /// </summary>
        private Func<string> _callback;

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="canExecute"></param>
        public ProxyRelayCommand(Func<string> cmdByReportOpInfo, string modelName, Func<bool> canExecute = null)
        {
            _callback = cmdByReportOpInfo ??
                throw new ArgumentNullException(string.Format("功能方法【cmdByReportOpInfo】不能为NULL"));

            _curCmdModelName = modelName;
            logArgs = new SysCommonMsgArgs(XLY.SF.Framework.Core.Base.SystemKeys.WirteOperationLog);
            if (canExecute != null)
                ViewExecuteCmd = new RelayCommand(ConcreateExecute, canExecute);
            else
                ViewExecuteCmd = new RelayCommand(ConcreateExecute);
        }

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志同时截图保存
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="getViewContainerCallback">获取ViewContainer回调</param>
        /// <param name="canExecute"></param>
        public ProxyRelayCommand(Func<string> cmdByReportOpInfo, string modelName, Func<object> getViewContainerCallback, Func<bool> canExecute)
            : this(cmdByReportOpInfo, modelName, canExecute)
        {
            _getContainerCallback = getViewContainerCallback;
        }

        private void ConcreateExecute()
        {
            var logResult = _callback();
#if !DEBUG
            if (!string.IsNullOrWhiteSpace(logResult))
                base.WriteOperationLog(new CmdLogModel() { ModelName = _curCmdModelName, OperationResult = logResult });
#endif
        }
    }
}
