using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XLY.SF.Shell.MessageBox;

namespace XLY.SF.Shell.CommWindow
{
    /// <summary>
    /// MessageBoxWin.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxWin : Window
    {
        #region Properties

        /// <summary>
        /// 是否需要反馈
        /// </summary>
        public bool HasFeedback { get; private set; }

        /// <summary>
        /// 消息窗状态
        /// </summary>
        public MessageBoxType MsgType
        {
            get; private set;
        }

        public string ConfirmText { get; private set; }
        public string CancelText { get; private set; }

        #endregion

        public MessageBoxWin()
        {
            InitializeComponent();
        }

        #region 内容与标题定义

        public string MsgContent { get; private set; }

        /// <summary>
        /// 设置消息框
        /// </summary>
        /// <param name="content">内容</param>
        public void SetMsgBox(string content, MessageBoxType msgType, string confirmText, string cancelText)
        {
            this.MsgContent = content;
            MsgType = msgType;
            ConfirmText = confirmText;
            CancelText = cancelText;
            //var a = ProjectExtend.Context.SystemContext.LanguageProvider;
            HasFeedback = !string.IsNullOrWhiteSpace(cancelText);
        }

        #endregion

        #region 界面操作

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (HasFeedback)
                this.DialogResult = true;
            else
                this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
