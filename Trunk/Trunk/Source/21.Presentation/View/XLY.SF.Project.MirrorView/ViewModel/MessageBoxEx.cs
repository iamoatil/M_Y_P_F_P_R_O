using System.Windows;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;

/* ==============================================================================
* Description：MessageBoxEx  
* Author     ：litao
* Create Date：2017/11/16 9:54:37
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    class MessageBoxEx: IMessageBox
    {
        IMessageBox _msgBox;
        public MessageBoxEx()
        {
            bool isToolRun = Application.Current is App;
            if(!isToolRun)
            {
                _msgBox= IocManagerSingle.Instance.GetPart<IMessageBox>();
            }
        }

        public bool ShowDialogErrorMsg(string errorText)
        {
            if (_msgBox != null)
            {
                return _msgBox.ShowDialogErrorMsg(errorText);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(errorText, "提示");
                return result == MessageBoxResult.OK;
            }
        }

        public bool ShowDialogNoticeMsg(string text)
        {
            if (_msgBox != null)
            {
                return _msgBox.ShowDialogNoticeMsg(text);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(text, "提示");
                return result == MessageBoxResult.OK;
            }
        }
        public void ShowErrorMsg(string errorText)
        {
            if (_msgBox != null)
            {
                _msgBox.ShowErrorMsg(errorText);
            }
            else
            {
                MessageBox.Show(errorText, "提示");
            }
        }

        public bool ShowMutualMsg(string title, string text)
        {
            if (_msgBox != null)
            {
                return _msgBox.ShowMutualMsg(title, text);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(text, title);
                return result == MessageBoxResult.OK;
            }
        }

        public void ShowNoticeMsg(string text)
        {
            if (_msgBox != null)
            {
                _msgBox.ShowNoticeMsg(text);
            }
            else
            {
                MessageBox.Show(text, "提示");
            }
        }

        public void ShowOtherMsg(string title, string text)
        {
            if (_msgBox != null)
            {
                _msgBox.ShowOtherMsg(title, text);
            }
            else
            {
                MessageBox.Show(text, title);
            }
        }
    }
}
