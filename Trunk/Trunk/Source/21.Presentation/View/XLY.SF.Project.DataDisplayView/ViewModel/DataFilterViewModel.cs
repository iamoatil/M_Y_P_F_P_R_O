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

            MessageAggregation.RegisterGeneralMsg<ObservableCollection<DataExtactionItem>>(this, MessageKeys.SetDataListKey, SetDataListKey);
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
        private Dictionary<int, string>  _KeywordTypeSource = null;

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
        }
        #endregion

        #region 开始筛选命令

        public RelayCommand StartFilterCommond { get; set; }

        /// <summary>
        /// 开始筛选命令
        /// </summary>
        private void DoStartFilterCommond()
        {
            List<FilterArgs> list = new List<FilterArgs>();
            if(StartTime != null || EndTime != null)
            {
                list.Add(new FilterByDateRangeArgs() { StartTime = StartTime, EndTime = EndTime });
            }
            if(!string.IsNullOrWhiteSpace(Keyword))
            {
                if(KeywordType == 0)
                {
                    list.Add(new FilterByStringContainsArgs() { PatternText = Keyword });
                }
                else
                {
                    list.Add(new FilterByRegexArgs() { Regex = new System.Text.RegularExpressions.Regex(Keyword) });
                }
            }
            if(BookmarkId != -2)
            {
                list.Add(new FilterByBookmarkArgs() { BookmarkId = BookmarkId });
            }
            if(DataState != EnumDataState.None)
            {
                list.Add(new FilterByEnumStateArgs() { State = DataState });
            }

            Task.Factory.StartNew(() => 
            {
                MessageAggregation.SendGeneralMsg(new GeneralArgs<bool>(MessageKeys.StartFilterKey) { Parameters = true });
                Filter(DataListSource, list.ToArray());
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
                if(item.IsChecked != false)
                {
                    if(item.Data != null && item.Data is IDataSource ds)
                    {
                        ds.Filter<dynamic>(args);
                    }
                    Filter(item.TreeNodes, args);
                }
            }
        }
        #endregion


        #endregion

        /// <summary>
        /// 重新设置数据
        /// </summary>
        /// <param name="obj"></param>
        private void SetDataListKey(GeneralArgs<ObservableCollection<DataExtactionItem>> obj)
        {
            DataListSource = obj.Parameters;
            DoClearCommond();
        }
     
    }
}
