using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Language;
using XLY.SF.Shell.CommWindow;
using ProjectExtend.Context;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:17:41
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Shell.MessageBox
{
    [Export(typeof(IMessageBox))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public class MessageBoxEx : IMessageBox
    {
        /// <summary>
        /// 创建消息窗口
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        private MessageBoxWin CreateMsgWindow(MessageBoxType msgType, string msg)
        {
            MessageBoxWin winResult = new MessageBoxWin();
            winResult.SetMsgBox(msg, msgType);
            if (Application.Current.MainWindow.GetType() == typeof(Shell)&&
                Application.Current.MainWindow.IsVisible)
                winResult.Owner = Application.Current.MainWindow;
            return winResult;
        }

        #region 非模式对话框

        /// <summary>
        /// 显示警告消息
        /// </summary>
        /// <param name="warningText">内容</param>
        public void ShowWarningMsg(string warningText)
        {
            var msgWin = CreateMsgWindow( MessageBoxType.Warning, warningText);
            msgWin.Show();
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="errorText">内容</param>
        public void ShowErrorMsg(string errorText)
        {
            var msgWin = CreateMsgWindow( MessageBoxType.Error, errorText);
            msgWin.Show();
        }

        /// <summary>
        /// 显示成功消息
        /// </summary>
        /// <param name="successText">消息内容</param>
        public void ShowSuccessMsg(string successText)
        {
            var msgWin = CreateMsgWindow(MessageBoxType.Success, successText);
            msgWin.Show();
        }

        #endregion

        #region 模式对话框

        /// <summary>
        /// 显示错误消息（模式对话框）
        /// </summary>
        public bool ShowDialogErrorMsg(string errorText)
        {
            var msgWin = CreateMsgWindow( MessageBoxType.Error,errorText);
            return ShowDialogMsg(msgWin);
        }

        /// <summary>
        /// 显示成功消息【模式对话框】
        /// </summary>
        /// <param name="successText">消息内容</param>
        public bool ShowDialogSuccessMsg(string successText)
        {
            var msgWin = CreateMsgWindow(MessageBoxType.Success, successText);
            return ShowDialogMsg(msgWin);
        }

        /// <summary>
        /// 显示警告消息（模式对话框）
        /// </summary>
        /// <param name="warningText">消息内容</param>
        public bool ShowDialogWarningMsg(string warningText)
        {
            var msgWin = CreateMsgWindow(MessageBoxType.Warning, warningText);
            return ShowDialogMsg(msgWin);
        }

        #endregion

        /// <summary>
        /// 显示模式对话框
        /// </summary>
        /// <param name="win"></param>
        /// <returns></returns>
        private bool ShowDialogMsg(MessageBoxWin win)
        {
            var result = win.ShowDialog();
            return result.HasValue && result.Value;
        }
    }
}
