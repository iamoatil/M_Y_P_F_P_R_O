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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;
using ProjectExtend.Context;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Project.ViewDomain.Model.MessageElement;

namespace XLY.SF.Project.Views.Main
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.ModuleMainUcView, typeof(UcViewBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class MainView : UcViewBase
    {
        /// <summary>
        /// 主界面容器
        /// </summary>
        private Window _winContainer;
        /// <summary>
        /// 界面时间更新
        /// </summary>
        private DispatcherTimer updateTime;
        /// <summary>
        /// 展开创建案例动画
        /// </summary>
        private Storyboard _sbExpandCreateCase;
        /// <summary>
        /// 折叠创建案例动画
        /// </summary>
        private Storyboard _sbOnExpandCreateCaseBack;

        public MainView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.ModuleMainViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }

        #region 加载

        private void UcViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            Binding s = new Binding("ActualHeight") { ElementName = "gd_ViewContent" };
            rt.SetBinding(Rectangle.HeightProperty, s);

            _sbExpandCreateCase = this.Resources["OnExpandCreateCase"] as Storyboard;
            _sbOnExpandCreateCaseBack = this.Resources["OnExpandCreateCaseBack"] as Storyboard;

            _winContainer = Window.GetWindow(this);
            ////注册主界面导航消息
            //MsgAggregation.Instance.RegisterNaviagtionMsg(this, SystemKeys.MainUcNavigation, MainNavigationCallback);
            //监听子界面展开消息
            MsgAggregation.Instance.RegisterSysMsg<SubViewMsgModel>(this, SystemKeys.SetSubViewStatus, OpenSubViewCallback);

            StartUpdateTimer();
        }

        private void StartUpdateTimer()
        {
            updateTime = new DispatcherTimer();
            updateTime.Interval = TimeSpan.FromSeconds(1);
            updateTime.Tick += UpdateTime_Tick;
            updateTime.Start();
        }

        #endregion

        #region 子界面操作

        /// <summary>
        /// 打开子界面回调
        /// </summary>
        /// <param name="args"></param>
        private void OpenSubViewCallback(SysCommonMsgArgs<SubViewMsgModel> args)
        {
            //gd_CaseName.Visibility = Visibility.Visible;
            ExecuteStoryboard(args.Parameters.IsExpandSubView, args.Parameters.NeedStoryboard);
        }

        /*
         * 当前案例名称不再显示的时候，表示回到了首页。
         * 重置设备列表，并不需要刷新主界面导航内容
         */
        private void gd_Devices_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue && _sbOnExpandCreateCaseBack != null)
            {
                //执行还原
                _sbOnExpandCreateCaseBack.Begin();
            }
            btn_Expand.IsEnabled = (bool)e.NewValue;
        }

        #endregion

        #region 时间更新

        private void UpdateTime_Tick(object sender, EventArgs e)
        {
            updateTime.Dispatcher.Invoke(() =>
            {
                tb_CurTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            });
        }

        #endregion

        #region Tools

        /// <summary>
        /// 执行动画
        /// </summary>
        /// <param name="isExpandSb">是否为展开</param>
        /// <param name="needStoryboard">是否需要动画</param>
        private void ExecuteStoryboard(bool isExpandSb, bool needStoryboard)
        {
            if (isExpandSb)
            {
                _sbExpandCreateCase.Begin();
            }
            else
            {
                _sbOnExpandCreateCaseBack.Begin();
            }
        }

        #endregion

        #region 界面操作

        //最小化
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        //界面大小改变【改变案例创建View大小】
        private void UcViewBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //由于除首页外，所有界面都会有这两行，所以固定减去这两行高度
            //br_CreateCaseView.Height = gd_ViewContent.ActualHeight - 122;
        }

        #endregion

        #region 主界面拖动

        //主界面拖动效果
        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _winContainer.DragMove();
        }

        #endregion

        private void cb_Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cb_Menu.SelectedIndex = -1;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _winContainer.WindowState = _winContainer.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}
