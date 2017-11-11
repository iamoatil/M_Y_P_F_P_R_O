using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataDisplayView.ViewModel.MainDisplayViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/30 13:27:09
* ==============================================================================*/

namespace XLY.SF.Project.DataDisplayView.ViewModel
{
    /// <summary>
    /// MainDisplayViewModel
    /// </summary>
    [Export(ExportKeys.DataDisplayViewModel, typeof(ViewModelBase))]
    public class MainDisplayViewModel : ViewModelBase
    {
        public MainDisplayViewModel()
        {
            SelecedAppChanged = new RelayCommand<object>(DoSelecedAppChanged);
            ExpandPreviewAreaCommond = new RelayCommand<object>(DoExpandPreviewAreaCommond);
        }

        #region 事件
        protected override void LoadCore(object parameters)
        {
            string devicePath = parameters?.ToString();
            LoadData(devicePath);
        }
        #endregion

        #region 属性

        #region 数据列表
        private ObservableCollection<DataExtactionTreeItem> _dataList;

        /// <summary>
        /// 数据列表
        /// </summary>	
        public ObservableCollection<DataExtactionTreeItem> DataList
        {
            get { return _dataList; }
            set
            {
                _dataList = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 布局视图集合
        private ObservableCollection<object> _layoutViewItems;

        /// <summary>
        /// 布局视图集合
        /// </summary>	
        public ObservableCollection<object> LayoutViewItems
        {
            get { return _layoutViewItems; }
            set
            {
                _layoutViewItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前选择的布局视图
        private object _selectedLayoutViewItem;

        /// <summary>
        /// 当前选择的布局视图
        /// </summary>	
        public object SelectedLayoutViewItem
        {
            get { return _selectedLayoutViewItem; }
            set
            {
                _selectedLayoutViewItem = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 数据预览的高度
        private double _previewAreaHeight = 40;

        /// <summary>
        /// 数据预览的高度
        /// </summary>	
        public double PreviewAreaHeight
        {
            get { return _previewAreaHeight; }
            set
            {
                _previewAreaHeight = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 是否展开数据预览区域
        private bool _isExpandPreviewArea = false;

        /// <summary>
        /// 是否展开数据预览区域
        /// </summary>	
        public bool IsExpandPreviewArea
        {
            get { return _isExpandPreviewArea; }
            set
            {
                _isExpandPreviewArea = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Commond
        #region 选择了某个APP的数据命令

        public RelayCommand<object> SelecedAppChanged { get; set; }

        /// <summary>
        /// 选择了某个APP的数据命令
        /// </summary>
        private void DoSelecedAppChanged(object app)
        {
            if (app == null)
                return;
            if(app is DataExtactionTreeItem treeItem && treeItem.Data != null)
            {
                LayoutViewItems = new ObservableCollection<object>();
                foreach (var item in DataViewPluginAdapter.Instance.GetView(treeItem.Text, AbstractDataViewPlugin.XLY_LAYOUT_KEY))
                {
                    LayoutViewItems.Add(item.ToControl(new DataViewPluginArgument() {  CurrentData = null, DataSource = treeItem.Data as IDataSource}));
                }
                SelectedLayoutViewItem = LayoutViewItems.FirstOrDefault();
            }
        }
        #endregion

        #region 展开或折叠预览区域

        public RelayCommand<object> ExpandPreviewAreaCommond { get; set; }

        /// <summary>
        /// 展开或折叠预览区域
        /// </summary>
        private void DoExpandPreviewAreaCommond(object isExpanded)
        {
            IsExpandPreviewArea = bool.Parse(isExpanded?.ToString());
            if (IsExpandPreviewArea)
            {
                PreviewAreaHeight = 200;
            }
            else
            {
                PreviewAreaHeight = 40;
            }
        }
        #endregion

        #endregion

        #region 方法
        private void LoadData(string devicePath)
        {
            string DB_PATH = @"C:\Users\fhjun\Desktop\test.db";

            var treeSource = new TreeDataSource();
            treeSource.TreeNodes = new List<TreeNode>();
            treeSource.PluginInfo = new DataParsePluginInfo() { Guid = "微信", Name = "微信" };

            for (int i = 0; i < 2; i++)
            {
                TreeNode t = new TreeNode();
                t.Text = "账号" + i;
                treeSource.TreeNodes.Add(t);

                TreeNode accouts = new TreeNode();
                accouts.Text = "好友列表";
                accouts.IsHideChildren = true;
                t.TreeNodes.Add(accouts);
                accouts.Type = typeof(WeChatFriendShow);
                accouts.Items = new DataItems<WeChatFriendShow>(DB_PATH);
                for (int j = 0; j < 10; j++)
                {
                    accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j, Remark="XLY_张三" + j });
                }

                TreeNode accouts2 = new TreeNode();
                accouts2.Text = "聊天记录";
                accouts2.IsHideChildren = i % 2 == 0;
                accouts2.Type = typeof(WeChatFriendShowX);
                accouts2.Items = new DataItems<WeChatFriendShowX>(DB_PATH);
                t.TreeNodes.Add(accouts2);
                for (int j = 0; j < 5; j += 2)
                {
                    accouts2.Items.Add(new WeChatFriendShowX() { Nick = "昵称" + j, WeChatId = "账号" + j, Remark = "XLY_李四" + j });
                    TreeNode friend = new TreeNode();
                    friend.Text = "昵称" + j;
                    friend.Type = typeof(MessageCore);
                    friend.Items = new DataItems<MessageCore>(DB_PATH);
                    accouts2.TreeNodes.Add(friend);

                    for (int k = 0; k < 100; k++)
                    {
                        MessageCore msg = new MessageCore() { SenderName = friend.Text, SenderImage = "images/zds.png", Receiver = t.Text, Content = "消息内容" + k, MessageType = k % 4 == 0 ? "图片" : "文本", SendState = EnumSendState.Send };
                        friend.Items.Add(msg);
                        MessageCore msg2 = new MessageCore() { Receiver = friend.Text, SenderImage = "images/zjq.png", SenderName = t.Text, Content = "返回消息内容" + k, MessageType = k % 5 == 0 ? "图片" : "文本", SendState = EnumSendState.Receive };
                        friend.Items.Add(msg2);
                    }
                }

                TreeNode accouts3 = new TreeNode();
                accouts3.Text = "群消息";
                accouts3.IsHideChildren = true;
                t.TreeNodes.Add(accouts3);

                TreeNode accouts4 = new TreeNode();
                accouts4.Text = "发现";
                accouts4.IsHideChildren = true;
                t.TreeNodes.Add(accouts4);
            }
            treeSource.BuildParent();

            var sms = new SimpleDataSource();
            sms.PluginInfo = new DataParsePluginInfo() { Guid = "短信", Name = "短信" };
            sms.Type = typeof(SMS);
            sms.Items = new DataItems<SMS>(DB_PATH);
            for (int i = 0; i < 10; i++)
            {
                sms.Items.Add(new SMS() { StartDate = DateTime.Now.AddDays(i), Content = "短信内容内容" + i });
            }
            sms.BuildParent();

            sms.Filter<AbstractDataItem>();
            treeSource.Filter<AbstractDataItem>();

            DataList = new ObservableCollection<DataExtactionTreeItem>() {
                new DataExtactionTreeItem(){Text = "自动提取", TreeNodes = new ObservableCollection<DataExtactionTreeItem>(){
                    new DataExtactionTreeItem(){Text = "基础信息", TreeNodes = new ObservableCollection<DataExtactionTreeItem>(){
                       new DataExtactionTreeItem(){Text = "短信",Data = sms, TreeNodes = new ObservableCollection<DataExtactionTreeItem>() }
                } }, new DataExtactionTreeItem(){Text = "社交聊天", TreeNodes = new ObservableCollection<DataExtactionTreeItem>(){
                       new DataExtactionTreeItem(){Text = "微信", Data = treeSource, TreeNodes = new ObservableCollection<DataExtactionTreeItem>() }
                }}}}
            };
        }
        #endregion
    }

    public class DataExtactionTreeItem
    {
        public string Text { get; set; }
        public object Data { get; set; }
        public bool IsHideChildren { get; set; }
        public ObservableCollection<DataExtactionTreeItem> TreeNodes { get; set; }
    }

    public class WeChatFriendShowX : WeChatFriendShow { }
}
