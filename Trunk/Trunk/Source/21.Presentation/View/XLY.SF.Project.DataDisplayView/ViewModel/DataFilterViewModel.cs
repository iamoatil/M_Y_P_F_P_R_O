using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract;
using XLY.SF.Framework.Core.Base.MessageBase;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataDisplayView.ViewModel.DataFilterViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/17 15:44:14
* ==============================================================================*/

namespace XLY.SF.Project.DataDisplayView.ViewModel
{
    /// <summary>
    /// DataFilterViewModel
    /// </summary>
    public class DataFilterViewModel : ViewModelBase
    {
        public DataFilterViewModel()
        {
            ClearCommond = new RelayCommand(DoClearCommond);
            StartFilterCommond = new RelayCommand(DoStartFilterCommond);

            BookmarkSource = new Dictionary<int, string>()
            {
                {-2, Languagekeys.BookmarkAll },
                {0, Languagekeys.BookmarkYes },
                {-1, Languagekeys.BookmarkNone },
            };
            DataStateSource = new Dictionary<EnumDataState, string>()
            {
                { EnumDataState.None, Languagekeys.DataStateAll },
                { EnumDataState.Normal,Languagekeys.DataStateNormal },
                { EnumDataState.Deleted ,Languagekeys.DataStateDelete }
            };
            KeywordTypeSource = new Dictionary<int, string>()
            {
                {0, Languagekeys.Keyword },
                {1, Languagekeys.ZhengZe},
            };
        }

        #region 绑定数据源

        #region 起始时间
        private DateTime? _StartTime = null;

        /// <summary>
        /// 起始时间
        /// </summary>	
        public DateTime? StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 结束时间
        private DateTime? _EndTime = null;

        /// <summary>
        /// 结束时间
        /// </summary>	
        public DateTime? EndTime
        {
            get { return _EndTime; }
            set
            {
                _EndTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 数据状态
        private EnumDataState _DataState = EnumDataState.None;

        /// <summary>
        /// 数据状态
        /// </summary>	
        public EnumDataState DataState
        {
            get { return _DataState; }
            set
            {
                _DataState = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 数据列表
        private ObservableCollection<DataExtactionItem> _DataListSource = null;

        /// <summary>
        /// 数据列表
        /// </summary>	
        public ObservableCollection<DataExtactionItem> DataListSource
        {
            get { return _DataListSource; }
            set
            {
                _DataListSource = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 绑定的数据列表，用于设置
        private ObservableCollection<DataExtactionItem> _BindDataListSource = null;

        /// <summary>
        /// 绑定的数据列表，用于设置
        /// </summary>	
        public ObservableCollection<DataExtactionItem> BindDataListSource
        {
            get { return _BindDataListSource; }
            set
            {
                _BindDataListSource = value;
                OnPropertyChanged();
            }
        }
        #endregion


        #region 应用筛选显示文本
        private string _DataListText = "";

        /// <summary>
        /// 应用筛选显示文本
        /// </summary>	
        public string DataListText
        {
            get { return _DataListText; }
            set
            {
                _DataListText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 标记
        private int _BookmarkId = -2;

        /// <summary>
        /// 标记
        /// </summary>	
        public int BookmarkId
        {
            get { return _BookmarkId; }
            set
            {
                _BookmarkId = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 关键词还是正则表达式
        private int _KeywordType = 0;

        /// <summary>
        /// 关键词还是正则表达式
        /// </summary>	
        public int KeywordType
        {
            get { return _KeywordType; }
            set
            {
                _KeywordType = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 关键词内容
        private string _Keyword = "";

        /// <summary>
        /// 关键词内容
        /// </summary>	
        public string Keyword
        {
            get { return _Keyword; }
            set
            {
                _Keyword = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 数据状态列表
        private Dictionary<EnumDataState, string> _DataStateSource = null;

        /// <summary>
        /// 数据状态列表
        /// </summary>	
        public Dictionary<EnumDataState, string> DataStateSource
        {
            get { return _DataStateSource; }
            set
            {
                _DataStateSource = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 标记列表
        private Dictionary<int, string> _BookmarkSource = null;

        /// <summary>
        /// 标记列表
        /// </summary>	
        public Dictionary<int, string> BookmarkSource
        {
            get { return _BookmarkSource; }
            set
            {
                _BookmarkSource = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 关键词还是正则列表
        private Dictionary<int, string> _KeywordTypeSource = null;

        /// <summary>
        /// 关键词还是正则列表
        /// </summary>	
        public Dictionary<int, string> KeywordTypeSource
        {
            get { return _KeywordTypeSource; }
            set
            {
                _KeywordTypeSource = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 是否是智能预警
        private bool _IsInspection = false;

        /// <summary>
        /// 是否是智能预警
        /// </summary>	
        public bool IsInspection
        {
            get { return _IsInspection; }
            set
            {
                _IsInspection = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 智能预警分类列表
        private ObservableCollection<InspectionItem> _InspectionList = new ObservableCollection<InspectionItem>();

        /// <summary>
        /// 智能预警分类列表
        /// </summary>	
        public ObservableCollection<InspectionItem> InspectionList
        {
            get { return _InspectionList; }
            set
            {
                _InspectionList = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前选择的智能预警项
        private InspectionItem _SelectedInspectionItem = null;

        /// <summary>
        /// 当前选择的智能预警项
        /// </summary>	
        public InspectionItem SelectedInspectionItem
        {
            get { return _SelectedInspectionItem; }
            set
            {
                _SelectedInspectionItem = value;
                OnPropertyChanged();

                DoStartFilterCommond();
            }
        }
        #endregion

        #endregion

        #region 命令
        #region 清空所有参数

        public RelayCommand ClearCommond { get; set; }

        /// <summary>
        /// 清空所有参数
        /// </summary>
        private void DoClearCommond()
        {
            Keyword = "";
            KeywordType = 0;
            BookmarkId = -2;
            DataState = EnumDataState.None;
            StartTime = null;
            EndTime = null;

            if (BindDataListSource != null)
            {
                foreach (var item in BindDataListSource)
                {
                    item.IsChecked = true;
                }
            }
            DataListText = Languagekeys.AllApps;
        }
        #endregion

        #region 开始筛选命令

        public RelayCommand StartFilterCommond { get; set; }

        /// <summary>
        /// 开始筛选命令
        /// </summary>
        private void DoStartFilterCommond()
        {
            var args = GetFilterArgs();
            Task.Factory.StartNew(() =>
            {
                MessageAggregation.SendGeneralMsg(new GeneralArgs<bool>(MessageKeys.StartFilterKey) { Parameters = true });
                Filter(DataListSource, args);
                ResetTotal(DataListSource);
                ResetTreeNodeVisible(DataListSource, args.Length > 0);
                MessageAggregation.SendGeneralMsg(new GeneralArgs<bool>(MessageKeys.StartFilterKey) { Parameters = false });
            });
        }

        /// <summary>
        /// 遍历每个数据开始过滤
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <param name="args"></param>
        private void Filter(ObservableCollection<DataExtactionItem> treeNodes, params FilterArgs[] args)
        {
            if (treeNodes == null)
                return;
            foreach (var item in treeNodes)
            {
                //if (item.IsVisible == false)
                //    continue;
                var setitem = BindDataListSource.FindExtactionItemFromTree(item);
                if (setitem != null && setitem.IsChecked == false)      //设置为隐藏，则不需要过滤
                    continue;
                if (item.Data != null && item.Data is IDataSource ds)
                {
                    ds.Filter<dynamic>(args);
                }
                Filter(item.TreeNodes, args);
            }
        }

        /// <summary>
        /// 查询后重新设置数据总数统计
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <returns></returns>
        private int[] ResetTotal(ObservableCollection<DataExtactionItem> treeNodes)
        {
            int[] arr = new int[2] { 0, 0 };

            if (treeNodes == null)
                return arr;
            int total = 0;
            int deltotal = 0;
            foreach (var item in treeNodes)
            {
                //if (item.IsVisible == false)
                //{
                //    continue;
                //}
                var setitem = BindDataListSource.FindExtactionItemFromTree(item);
                if (setitem != null && setitem.IsChecked == false)      //设置为隐藏，则不需要过滤
                    continue;
                if (item.Data != null && item.Data is IDataSource ds)
                {
                    item.Total = ds.Total;
                    item.DeleteTotal = ds.DeleteTotal;
                }
                else
                {
                    var aa = ResetTotal(item.TreeNodes);
                    item.Total = aa[0];
                    item.DeleteTotal = aa[1];
                }
                total += item.Total;
                deltotal += item.DeleteTotal;
            }
            arr[0] = total;
            arr[1] = deltotal;
            return arr;
        }

        /// <summary>
        /// 重置树节点的显示状态，如果在过滤并且数据量为0，则隐藏节点
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <param name="isHideEmptyNode"></param>
        private void ResetTreeNodeVisible(ObservableCollection<DataExtactionItem> treeNodes, bool isHideEmptyNode)
        {
            if (treeNodes == null)
                return;
            foreach (var item in treeNodes)
            {
                //if (item.IsVisible == false)
                //{
                //    continue;
                //}
                var setitem = BindDataListSource.FindExtactionItemFromTree(item);
                if (setitem != null && setitem.IsChecked == false)      //设置为隐藏，则不需要过滤
                    continue;
                if (isHideEmptyNode && item.Total == 0)
                {
                    item.IsVisible = false;
                    continue;
                }
                
                if (item.Data != null && item.Data is TreeDataSource ds)
                {
                    item.IsVisible = null;
                    //ds.TreeNodes?.ForEach(tn => tn.TranverseTree(c=>((TreeNode)c).IsVisible = !isHideEmptyNode ||((TreeNode)c).Total > 0));
                    ds.TranverseTree(c=>((TreeNode)c).IsVisible = !isHideEmptyNode ||((TreeNode)c).Total > 0);
                }
                else if (item.Data != null && item.Data is IDataSource)
                {
                    item.IsVisible = true;
                }
                ResetTreeNodeVisible(item.TreeNodes, isHideEmptyNode);
            }
        }
        #endregion

        #endregion

        #region 方法
        /// <summary>
        /// 重新设置数据
        /// </summary>
        /// <param name="obj"></param>
        public void SetDataListKey(ObservableCollection<DataExtactionItem> obj)
        {
            DataListSource = obj;
            BindDataListSource = new ObservableCollection<DataExtactionItem>(DataListSource.CopyDataExtactionItem());
            foreach (var item in BindDataListSource)
            {
                item.TranverseTree(node =>
                {
                    (node as DataExtactionItem).CheckedChanged -= DataExtactionItem_CheckedChanged;
                    (node as DataExtactionItem).CheckedChanged += DataExtactionItem_CheckedChanged;
                });
            }
            DoClearCommond();
        }

        private void DataExtactionItem_CheckedChanged(object sender, EventArgs e)
        {
            List<DataExtactionItem> ls = new List<DataExtactionItem>();
            bool isAllSelected = true;
            foreach (var item in BindDataListSource)
            {
                item.TranverseTree(node =>
                {
                    if (node is DataExtactionItem n)
                    {
                        if (n.IsChecked != false)
                        {
                            //if (n.Data != null)
                            //    ls.Add(n);
                            var a = DataListSource.FindExtactionItemFromTree(n as DataExtactionItem);
                            if (a != null && a.Data != null)  
                            {
                                ls.Add(n);
                            }
                        }
                        else
                        {
                            isAllSelected = false;
                        }
                    }
                });
            }
            var tn = DataListSource.FindExtactionItemFromTree(sender as DataExtactionItem);
            if(tn != null)   //设置目标节点的可见性
            {
                tn.IsVisible = (sender as DataExtactionItem)?.IsChecked;
            }
            DataListText = !isAllSelected ? string.Join(",", ls.Select(l => l.Text)) : (string)Languagekeys.AllApps;
        }

        /// <summary>
        /// 数据加载完成后更新
        /// </summary>
        public void OnDataLoadedCompleted()
        {
            if (IsInspection)
            {
                SelectedInspectionItem = InspectionList.FirstOrDefault();
            }
        }

        public void SetInspectionConfig(List<Inspection> config)
        {
            IsInspection = config != null;
            InspectionList.Clear();
            if (IsInspection)
            {
                InspectionList.AddRange(config.Select(i => new InspectionItem() { Id = i.ID, Name = LanguageHelper.LanguageManager.Type == Framework.Language.LanguageType.En ? i.CategoryEn : i.CategoryCn, Icon = null, Total = 0 }));
                //SelectedInspectionItem = InspectionList.FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取当前设置的过滤参数
        /// </summary>
        /// <returns></returns>
        private FilterArgs[] GetFilterArgs()
        {
            List<FilterArgs> list = new List<FilterArgs>();
            if (StartTime != null || EndTime != null)
            {
                list.Add(new FilterByDateRangeArgs() { StartTime = StartTime, EndTime = EndTime });
            }
            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                if (KeywordType == 0)
                {
                    list.Add(new FilterByStringContainsArgs() { PatternText = Keyword });
                }
                else
                {
                    list.Add(new FilterByRegexArgs() { Regex = new System.Text.RegularExpressions.Regex(Keyword) });
                }
            }
            if (BookmarkId != -2)
            {
                list.Add(new FilterByBookmarkArgs() { BookmarkId = BookmarkId });
            }
            if (DataState != EnumDataState.None)
            {
                list.Add(new FilterByEnumStateArgs() { State = DataState });
            }

            if (IsInspection && SelectedInspectionItem != null)
            {
                list.Add(new FilterBySensitiveArgs() { SensitiveId = SelectedInspectionItem.Id });
            }
            return list.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// 智能预警项
    /// </summary>
    public class InspectionItem:NotifyPropertyBase
    {
        #region ID
        private int _Id = -1;

        /// <summary>
        /// ID
        /// </summary>	
        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Name
        private string _Name = null;

        /// <summary>
        /// Name
        /// </summary>	
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Icon
        private string _Icon = null;

        /// <summary>
        /// Icon
        /// </summary>	
        public string Icon
        {
            get { return _Icon; }
            set
            {
                _Icon = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Total
        private int _Total = 0;

        /// <summary>
        /// Total
        /// </summary>	
        public int Total
        {
            get { return _Total; }
            set
            {
                _Total = value;
                OnPropertyChanged();
            }
        }
        #endregion

    }

    public class Inspection
    {
        public Int32 ID { get; set; }

        public String NameCn { get; set; }

        public String CategoryCn { get; set; }

        public String NameEn { get; set; }

        public String CategoryEn { get; set; }

        public String ConfigFile { get; set; }

        public Int32 SelectedToken { get; set; }

        public Boolean IsSelected
        {
            get => SelectedToken > 0;
            set => SelectedToken = value ? 1 : 0;
        }
    }

    public class InspectionConfig
    {
        public List<Inspection> Config { get; set; }
        public string DevicePath { get; set; }
    }

}
