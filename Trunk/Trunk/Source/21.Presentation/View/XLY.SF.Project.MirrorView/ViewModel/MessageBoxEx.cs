using System.Windows;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;

/* ==============================================================================
* Description：MessageBoxX  
* Author     ：litao
* Create Date：2017/11/16 9:54:37
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    class MessageBoxX
    {
        IMessageBox _msgBox;

        public MessageBoxX()
        {
            bool isToolRun = Application.Current is App;
            if (!isToolRun)
            {
                _msgBox = IocManagerSingle.Instance.GetPart<IMessageBox>();
            }
        }

        public void ShowDialogErrorMsg(string errorText)
        {
            if (_msgBox != null)
            {
                _msgBox.ShowErrorMsg(errorText);
            }
            else
            {
                MessageBox.Show(errorText, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowDialogSuccessMsg(string text)
        {
            if (_msgBox != null)
            {
                _msgBox.ShowSuccessMsg(text);
            }
            else
            {
                MessageBox.Show(text, "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void ShowDialogWarningMsg(string text)
        {
            if (_msgBox != null)
            {
                _msgBox.ShowWarningMsg(text);
            }
            else
            {
                MessageBox.Show(text, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public bool ShowQuestionMsg(string text)
        {
            if (_msgBox != null)
            {
                return _msgBox.ShowDialogWarningMsg(text);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(text, "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                return result == MessageBoxResult.OK;
            }
        }
    }
}
