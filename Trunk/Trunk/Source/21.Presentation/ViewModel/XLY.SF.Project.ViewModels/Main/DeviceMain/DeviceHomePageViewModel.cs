using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.DataExtract;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model;
using XLY.SF.Project.ViewDomain.VModel.DevHomePage;
using XLY.SF.Project.ViewModels.Main.CaseManagement;
using XLY.SF.Project.ViewModels.Main.DeviceMain.Navigation;

namespace XLY.SF.Project.ViewModels.Main.DeviceMain
{
    [Export(ExportKeys.DeviceHomePageViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceHomePageViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// 推荐方案
        /// </summary>
        public ObservableCollection<StrategyElement> StrategyRecommendItems { get; private set; }

        /// <summary>
        /// 小工具
        /// </summary>
        public ObservableCollection<string> ToolkitItems { get; private set; }

        #region 设备信息【和设备主页是同一实例】

        private DeviceModel _curDevModel;
        /// <summary>
        /// 当前设备信息
        /// </summary>
        public DeviceModel CurDevModel
        {
            get
            {
                return _curDevModel;
            }
            set
            {
                _curDevModel = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 设备首页导航

        private object _subView;
        /// <summary>
        /// 子界面内容
        /// </summary>
        public object SubView
        {
            get
            {
                return this._subView;
            }

            set
            {
                this._subView = value;
                base.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 设备首页界面
        /// </summary>
        public SubViewCacheManager DevHomePageSubViewHelper { get; private set; }

        #endregion

        #region 界面控制

        #region 当前编辑状态【信息录入】

        private bool _curEditStatus;
        /// <summary>
        /// 当前编辑状态
        /// </summary>
        public bool CurEditStatus
        {
            get
            {
                return this._curEditStatus;
            }

            set
            {
                this._curEditStatus = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 采集人【当前登录人】

        /// <summary>
        /// 采集人姓名
        /// </summary>
        public string CurLoginUserName
        {
            get;
        }

        /// <summary>
        /// 采集人证件号
        /// </summary>
        public string CurLoginUserID
        {
            get;
        }

        #endregion

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// 取消编辑
        /// </summary>
        public ICommand CancelEditCommand { get; private set; }

        /// <summary>
        /// 保存编辑信息
        /// </summary>
        public ProxyRelayCommand<DevHomePageEditItemModel> SaveEditCommand { get; private set; }

        /// <summary>
        /// 自动提取
        /// </summary>
        public ICommand AutoExtractCommand { get; private set; }

        /// <summary>
        /// 手机拍照
        /// </summary>
        public ProxyRelayCommand PhoneTakePhotoCommand { get; private set; }

        /// <summary>
        /// 推荐方案
        /// </summary>
        public ProxyRelayCommand<StrategyElement> StrategyRecommendCommand { get; private set; }

        #endregion

        public DeviceHomePageViewModel()
        {
            StrategyRecommendItems = new ObservableCollection<StrategyElement>();
            ToolkitItems = new ObservableCollection<string>();

            PhoneTakePhotoCommand = new ProxyRelayCommand(ExecutePhoneTakePhotoCommand);
            StrategyRecommendCommand = new ProxyRelayCommand<StrategyElement>(ExecuteStrategyRecommendCommand);
            SaveEditCommand = new ProxyRelayCommand<DevHomePageEditItemModel>(ExecuteSaveEditCommand);
            CancelEditCommand = new RelayCommand(ExecuteCancelEditCommand);
            AutoExtractCommand = new RelayCommand(ExecuteAutoExtractCommand);

            CurLoginUserName = SystemContext.Instance.CurUserInfo.UserName;
            CurLoginUserID = SystemContext.Instance.CurUserInfo.IdNumber;






            StrategyRecommendItems.Add(new StrategyElement() { SolutionStrategyName = "物理镜像" });
            StrategyRecommendItems.Add(new StrategyElement() { SolutionStrategyName = "备份提取" });
            StrategyRecommendItems.Add(new StrategyElement() { SolutionStrategyName = "APP植入" });
            StrategyRecommendItems.Add(new StrategyElement() { SolutionStrategyName = "降级提取" });
            StrategyRecommendItems.Add(new StrategyElement() { SolutionStrategyName = "截屏取证" });

            ToolkitItems.Add("地理轨迹分析");
            ToolkitItems.Add("图片轨迹分析");
            ToolkitItems.Add("Android九宫格破解");
            ToolkitItems.Add("黑莓大容量模式");
            ToolkitItems.Add("测试立刻集散地法");
            ToolkitItems.Add("佛挡杀佛");
            ToolkitItems.Add("阿萨德放到");
        }

        #region 重载

        protected override void InitLoad(object parameters)
        {
            CurDevModel = parameters as DeviceModel;
            if (CurDevModel == null)
                throw new NullReferenceException($"CurDevModel为null");

            //根据设备ID创建对应设备的子界面导航器
            DevHomePageSubViewHelper = new SubViewCacheManager(CurDevModel.IDevSource.ID);
        }

        #endregion

        #region ExecuteCommands

        //保存编辑信息
        private string ExecuteSaveEditCommand(DevHomePageEditItemModel arg)
        {
            if (arg != null)
            {
                //保存信息
                CurDevModel.IDevSource.CollectionInfo = CurDevModel.IDevSource.CollectionInfo ?? new ExportCollectionInfo();
                CurDevModel.IDevSource.CollectionInfo.DataNo = arg.No;
                CurDevModel.IDevSource.CollectionInfo.HolderName = arg.Holder;
                CurDevModel.IDevSource.CollectionInfo.HolderCertificateType = arg.CredentialsType;
                CurDevModel.IDevSource.CollectionInfo.HolderCertificateCode = arg.HolderCredentialsNo;
                CurDevModel.IDevSource.CollectionInfo.SenderName = arg.CensorshipPerson;
                CurDevModel.IDevSource.CollectionInfo.SenderCompany = arg.UnitName;
                CurDevModel.IDevSource.CollectionInfo.SenderCertificateCode = arg.CensorshipPersonCredentialsNo;
                CurDevModel.IDevSource.CollectionInfo.CollectorName = arg.Operator;
                CurDevModel.IDevSource.CollectionInfo.CollectorCertificateCode = arg.CredentialsNo;
                CurDevModel.IDevSource.CollectionInfo.Description = arg.Desciption;

                return "保存提取信息成功";
            }

            CurEditStatus = !CurEditStatus;
            return string.Empty;
        }

        //执行推荐方案
        private string ExecuteStrategyRecommendCommand(StrategyElement arg)
        {
            //物理镜像【测试代码】
            SubView = DevHomePageSubViewHelper.GetOrCreateView(ExportKeys.MirrorView, CurDevModel.IDevSource);

            return $"执行推荐方案{arg.SolutionStrategyName}";
        }

        private void ExecuteAutoExtractCommand()
        {
            var adorner = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
            XLY.SF.Project.CaseManagement.ExtractItem ei =
                adorner.Target.CreateExtract(SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_StrategyRecommend_AutoExtraction],
                                            SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_StrategyRecommend_AutoExtraction]);

            Pump @params = new Pump(ei.Path, "data.db");
            @params.Type = EnumPump.USB;
            @params.OSType = (CurDevModel.IDevSource as Domains.Device).OSType;
            @params.Source = CurDevModel.IDevSource;
            SubView = DevHomePageSubViewHelper.GetOrCreateView(ExportKeys.ExtractionView, @params);
        }

        private string ExecutePhoneTakePhotoCommand()
        {

            return string.Empty;
        }

        //取消编辑状态
        private void ExecuteCancelEditCommand()
        {
            CurEditStatus = false;
            //恢复之前的值






        }

        #endregion
    }
}
