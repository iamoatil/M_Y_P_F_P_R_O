using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 11:38:32
 * 接口功能说明：
 *      1. 框架核心接口
 *      2. 实现此接口用于弹出消息对话框
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    public interface IMessageBox
    {
        /// <summary>
        /// 显示警告消息
        /// </summary>
        /// <param name="warningText">内容</param>
        /// <param name="confirmText">确认按钮显示文字</param>
        void ShowWarningMsg(string warningText, string confirmText = null);

        /// <summary>
        /// 显示成功消息
        /// </summary>
        /// <param name="successText">内容</param>
        /// <param name="confirmText">确认按钮显示文字</param>
        void ShowSuccessMsg(string successText, string confirmText = null);

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="errorText">消息内容</param>
        /// <param name="confirmText">确认按钮显示文字</param>
        void ShowErrorMsg(string errorText, string confirmText = null);

        /// <summary>
        /// 显示错误消息（模式对话框）
        /// </summary>
        /// <param name="confirmText">确认按钮显示文字</param>
        /// <param name="cancelText">取消按钮显示文字</param>
        bool ShowDialogErrorMsg(string errorText, string confirmText = null, string cancelText = null);

        /// <summary>
        /// 显示成功消息【模式对话框】
        /// </summary>
        /// <param name="successText">消息内容</param>
        /// <param name="confirmText">确认按钮显示文字</param>
        /// <param name="cancelText">取消按钮显示文字</param>
        bool ShowDialogSuccessMsg(string successText, string confirmText = null, string cancelText = null);

        /// <summary>
        /// 显示警告消息（模式对话框）
        /// </summary>
        /// <param name="warningText">消息内容</param>
        /// <param name="confirmText">确认按钮显示文字</param>
        /// <param name="cancelText">取消按钮显示文字</param>
        bool ShowDialogWarningMsg(string warningText, string confirmText = null, string cancelText = null);
    }
}
