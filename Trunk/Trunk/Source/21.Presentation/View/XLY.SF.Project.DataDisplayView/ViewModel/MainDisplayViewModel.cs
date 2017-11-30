using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.Plugin.DataView;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataView.ViewModel.MainDisplayViewModel
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
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainDisplayViewModel : ViewModelBase
    {
        public MainDisplayViewModel()
        {
            SelecedAppChanged = new RelayCommand<object>(DoSelecedAppChanged);
            DeleteDataCommond = new RelayCommand<object>(DoDeleteDataCommond);

            MessageAggregation.RegisterGeneralMsg<bool>(this, MessageKeys.StartFilterKey, StartFilter);
            _MessageBox = IocManagerSingle.Instance.GetPart<IMessageBox>();
        }

        private IMessageBox _MessageBox
        {
            get;
            set;
        }

        #region 事件
        protected override void InitLoad(object parameters)
        {
            _currentDevicePath = parameters?.ToString();
            LoadPlugin();
            LoadData(_currentDevicePath);
        }

        /// <summary>
        /// 接收到数据更新的请求
        /// </summary>
        /// <param name="parameters"></param>
        public override void ReceiveParameters(object parameters)
        {
            base.ReceiveParameters(parameters);
            LoadData(_currentDevicePath);
        }
        #endregion

        #region 属性

        #region 数据列表
        private ObservableCollection<DataExtactionItem> _dataList;

        /// <summary>
        /// 数据列表
        /// </summary>	
        public ObservableCollection<DataExtactionItem> DataList
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

        #region 当前数据是否为空，是则显示提示信息
        private bool _hasData = false;

        /// <summary>
        /// 当前数据是否为空，是则显示提示信息
        /// </summary>	
        public bool HasData
        {
            get { return _hasData; }
            set
            {
                _hasData = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前数据列表是否为空，是则显示提示信息
        private bool _HasDataList = false;

        /// <summary>
        /// 当前数据列表是否为空，是则显示提示信息
        /// </summary>	
        public bool HasDataList
        {
            get { return _HasDataList; }
            set
            {
                _HasDataList = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 是否正在查询数据
        private bool _IsFiltering = false;

        /// <summary>
        /// 是否正在查询数据
        /// </summary>	
        public bool IsFiltering
        {
            get { return _IsFiltering; }
            set
            {
                _IsFiltering = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前正在执行的操作
        private string _CurrentOperation = "";

        /// <summary>
        /// 当前正在执行的操作
        /// </summary>	
        public string CurrentOperation
        {
            get { return _CurrentOperation; }
            set
            {
                _CurrentOperation = value;
                OnPropertyChanged();
            }
        }
        #endregion


        private string _currentDevicePath = null;

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
            if (app is DataExtactionItem treeItem && treeItem.Data != null)
            {
                LayoutViewItems = new ObservableCollection<object>();
                foreach (var item in DataViewPluginAdapter.Instance.GetView(treeItem.Text, DataViewConfigure.XLY_LAYOUT_KEY))
                {
                    LayoutViewItems.Add(item.ToControl(new DataViewPluginArgument() { CurrentData = null, DataSource = treeItem.Data as IDataSource, OnSelectedItemChanged = OnDataViewSelectedItemChanged }));
                }
                SelectedLayoutViewItem = LayoutViewItems.FirstOrDefault();
            }
            HasData = SelectedLayoutViewItem != null;
        }

        /// <summary>
        /// 选了某项数据，此时更新数据预览
        /// </summary>
        /// <param name="data"></param>
        private void OnDataViewSelectedItemChanged(object data)
        {
            MessageAggregation.SendGeneralMsg<object>(new GeneralArgs<object>(MessageKeys.PreviewKey) { Parameters = data });
        }

        #endregion

        #region 删除某个提取项数据

        public RelayCommand<object> DeleteDataCommond { get; set; }

        /// <summary>
        /// 删除某个提取项数据
        /// </summary>
        private void DoDeleteDataCommond(object item)
        {
            if(item == null)
            {
                return;
            }
            if(ShowMessageBox(string.Format(Languagekeys.DeleteNotice, item)))
            {
                Task.Factory.StartNew(() =>
                {
                    AsyncOperator.Execute(() => { CurrentOperation = Languagekeys.DeletingData; IsFiltering = true; } );
                    string path = Path.Combine(_currentDevicePath, item.ToString());
                    BaseUtility.Helper.FileHelper.RemoveDirectoryReadOnly(path);
                    Directory.Delete(path, true);
                    AsyncOperator.Execute(() =>
                    {
                        IsFiltering = false;
                        var d = DataList.FirstOrDefault(e => e.Text == item.ToString());
                        DataList.Remove(d);
                        HasDataList = DataList != null && DataList.Count > 0;
                        if (!SelectDefaultNode(DataList))
                        {
                            LayoutViewItems = new ObservableCollection<object>();
                            SelectedLayoutViewItem = null;
                            HasData = SelectedLayoutViewItem != null;
                        }
                    });
                });
            }
        }
        #endregion

        #endregion

        #region 方法
        private bool ShowMessageBox(string message)
        {
            if(_MessageBox != null)
            {
                return _MessageBox.ShowDialogWarningMsg(message);
            }
            else
            {
                return System.Windows.MessageBox.Show(message, Languagekeys.DeleteNoticeTitle, System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes;
            }
        }
        private void LoadPlugin()
        {
            if (DataViewPluginAdapter.Instance.Plugins == null || DataViewPluginAdapter.Instance.Plugins.Count() == 0)
            {
                DataViewPluginAdapter.Instance.Plugins = PluginAdapter.Instance.GetPluginsByType<DataViewPluginInfo>(PluginType.SpfDataView).ToList().ConvertAll(p => (AbstractDataViewPlugin)p.Value);
            }
        }
        private void LoadData(string devicePath)
        {
            Task.Factory.StartNew(() =>
            {
                IsFiltering = true;
                //string DB_PATH = @"C:\Users\fhjun\Desktop\test.db";
                //string DBBMK_PATH = @"C:\Users\fhjun\Desktop\test_bmk.db";
                //if (File.Exists(DB_PATH))
                //{
                //    File.Delete(DB_PATH);
                //}
                //if (File.Exists(DBBMK_PATH))
                //{
                //    File.Delete(DBBMK_PATH);
                //}
                //var treeSource = new TreeDataSource();
                //treeSource.TreeNodes = new List<TreeNode>();
                //treeSource.PluginInfo = new DataParsePluginInfo() { Guid = "微信", Name = "微信" };

                //for (int i = 0; i < 2; i++)
                //{
                //    TreeNode t = new TreeNode();
                //    t.Text = "账号" + i;
                //    treeSource.TreeNodes.Add(t);

                //    TreeNode accouts = new TreeNode();
                //    accouts.Text = "好友列表";
                //    accouts.IsHideChildren = true;
                //    t.TreeNodes.Add(accouts);
                //    accouts.Type = typeof(WeChatFriendShow);
                //    accouts.Items = new DataItems<WeChatFriendShow>(DB_PATH);
                //    for (int j = 0; j < 10; j++)
                //    {
                //        accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j, Remark = "XLY_张三" + j });
                //    }

                //    TreeNode accouts2 = new TreeNode();
                //    accouts2.Text = "聊天记录";
                //    accouts2.IsHideChildren = i % 2 == 0;
                //    accouts2.Type = typeof(WeChatFriendShowX);
                //    accouts2.Items = new DataItems<WeChatFriendShowX>(DB_PATH);
                //    t.TreeNodes.Add(accouts2);
                //    for (int j = 0; j < 15; j += 2)
                //    {
                //        accouts2.Items.Add(new WeChatFriendShowX() { DataState = j % 3 == 0 ? EnumDataState.Deleted : EnumDataState.Normal, Nick = "昵称" + j, WeChatId = "账号" + j, Remark = "XLY_李四" + j });
                //        TreeNode friend = new TreeNode();
                //        friend.Text = "昵称" + j;
                //        friend.Type = typeof(MessageCore);
                //        friend.Items = new DataItems<MessageCore>(DB_PATH);
                //        accouts2.TreeNodes.Add(friend);

                //        friend.Items.Add(new MessageCore() { SenderName = friend.Text, SenderImage = "images/zds.png", Receiver = t.Text, Content = "http://www.sohu.com", SendState = EnumSendState.Send, Type = EnumColumnType.URL });
                //        for (int k = 0; k < 130; k++)
                //        {
                //            MessageCore msg = new MessageCore() { SenderName = friend.Text, SenderImage = "images/zds.png", Receiver = t.Text, Content = "消息内容" + k, MessageType = k % 4 == 0 ? "图片" : "文本", SendState = EnumSendState.Send, Date = DateTime.Now.AddHours(k * 3) };
                //            friend.Items.Add(msg);
                //            MessageCore msg2 = new MessageCore() { Receiver = friend.Text, SenderImage = "images/zjq.png", SenderName = t.Text, Content = "返回消息内容" + k, MessageType = k % 5 == 0 ? "图片" : "文本", SendState = EnumSendState.Receive, Date = DateTime.Now.AddHours(k * 3) };
                //            friend.Items.Add(msg2);
                //        }
                //    }

                //    TreeNode accouts3 = new TreeNode();
                //    accouts3.Text = "群消息";
                //    accouts3.IsHideChildren = true;
                //    t.TreeNodes.Add(accouts3);

                //    TreeNode accouts4 = new TreeNode();
                //    accouts4.Text = "发现";
                //    accouts4.IsHideChildren = true;
                //    t.TreeNodes.Add(accouts4);
                //}
                //treeSource.BuildParent();

                //var sms = new SimpleDataSource();
                //sms.PluginInfo = new DataParsePluginInfo() { Guid = "短信", Name = "短信" };
                //sms.Type = typeof(SMS);
                //sms.Items = new DataItems<SMS>(DB_PATH);
                //for (int i = 0; i < 10; i++)
                //{
                //    sms.Items.Add(new SMS() { StartDate = DateTime.Now.AddDays(i), Content = "短信内容内容" + i, DataState = i % 3 == 0 ? EnumDataState.None : EnumDataState.Deleted });
                //}
                //sms.Items.Add(new SMS() { StartDate = DateTime.Now.AddDays(99), Content = "http://www.234.com", DataState = EnumDataState.Deleted });
                //sms.BuildParent();

                //sms.Filter<AbstractDataItem>();
                //treeSource.Filter<AbstractDataItem>();

                //var dataList = new ObservableCollection<DataExtactionItem>() {
                //    new DataExtactionItem(){Text = "自动提取", Total = 511, IsItemStyle=true, TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //        new DataExtactionItem(){Text = "基础信息", IsItemStyle=false,TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //           new DataExtactionItem(){Text = "短信",Data = sms, IsItemStyle=false, TreeNodes = new ObservableCollection<DataExtactionItem>() }
                //    } }, new DataExtactionItem(){Text = "社交聊天", IsItemStyle=false, TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //           new DataExtactionItem(){Text = "微信", IsItemStyle=false, Data = treeSource, TreeNodes = new ObservableCollection<DataExtactionItem>() }
                //    }}}},
                //    new DataExtactionItem(){Text = "镜像提取", Total = 11, IsItemStyle=true, TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //        new DataExtactionItem(){Text = "基础信息", IsItemStyle=false,TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //           new DataExtactionItem(){Text = "短信",Data = sms, IsItemStyle=false, TreeNodes = new ObservableCollection<DataExtactionItem>() }
                //    } }, new DataExtactionItem(){Text = "社交聊天", IsItemStyle=false, TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //           new DataExtactionItem(){Text = "微信", IsItemStyle=false, Data = treeSource, TreeNodes = new ObservableCollection<DataExtactionItem>() }
                //    }}}},
                //     new DataExtactionItem(){Text = "APP降级提取",Total = 0, IsItemStyle=true, TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //        new DataExtactionItem(){Text = "基础信息", IsItemStyle=false,TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //           new DataExtactionItem(){Text = "短信",Data = sms, IsItemStyle=false, TreeNodes = new ObservableCollection<DataExtactionItem>() }
                //    } }, new DataExtactionItem(){Text = "社交聊天", IsItemStyle=false, TreeNodes = new ObservableCollection<DataExtactionItem>(){
                //           new DataExtactionItem(){Text = "微信", IsItemStyle=false, Data = treeSource, TreeNodes = new ObservableCollection<DataExtactionItem>() }
                //    }}}}
                //};

                //devicePath = @"C:\Users\fhjun\Desktop\默认案例_20171115[081055]\默认案例_20171115[081055]\R7007_20171115[081055]";
                _currentDevicePath = devicePath;
                var dataList = DeviceExternsion.LoadDeviceData(devicePath);
                foreach (var item in dataList)
                {
                    item.BuildParent();
                }
                AsyncOperator.Execute(() =>
                {
                    DataList = dataList;

                    //重置数据
                    MessageAggregation.SendGeneralMsg(new GeneralArgs<ObservableCollection<DataExtactionItem>>(MessageKeys.SetDataListKey) { Parameters = DataList });

                    IsFiltering = false;
                    HasDataList = DataList != null && DataList.Count > 0;

                    SelectDefaultNode(DataList);
                });
            });
        }

        /// <summary>
        /// 设置查询状态
        /// </summary>
        /// <param name="obj"></param>
        private void StartFilter(GeneralArgs<bool> obj)
        {
            AsyncOperator.Execute(() =>
            {
                CurrentOperation = Languagekeys.Searching;
                IsFiltering = obj.Parameters;

                if(!IsFiltering)
                {
                    SelectDefaultNode(DataList);
                }
            });
        }

        /// <summary>
        /// 选择默认的节点，当刷新数据后设置
        /// </summary>
        private bool SelectDefaultNode(ObservableCollection<DataExtactionItem> nodes)
        {
            if(nodes == null)
            {
                return false;
            }
            foreach (var item in nodes)
            {
                if(item != null && item.Data != null)
                {
                    item.IsSelected = true;
                    DoSelecedAppChanged(item);
                    return true;
                }
                if(item.TreeNodes != null)
                {
                    if(SelectDefaultNode(item.TreeNodes))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }

   
}
