using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
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

        #region 采集信息

        private DevHomePageEditItemModel _editInfo;
        /// <summary>
        /// 采集信息
        /// </summary>
        public DevHomePageEditItemModel EditInfo
        {
            get
            {
                return this._editInfo;
            }

            set
            {
                this._editInfo = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 返回按钮状态

        private bool _backToDevHomePageStatus;
        /// <summary>
        /// 返回按钮显示状态
        /// </summary>
        public bool BackToDevHomePageStatus
        {
            get
            {
                return this._backToDevHomePageStatus;
            }

            set
            {
                this._backToDevHomePageStatus = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 设备首页导航

        #region 子界面

        /// <summary>
        /// 推荐方案的Key
        /// </summary>
        private string _functionKey;

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

        #endregion

        /// <summary>
        /// 设备首页界面
        /// </summary>
        public SubViewCacheManager DevHomePageSubViewHelper { get; private set; }


        private IPopupWindowService PopupWindowService;

        #endregion

        #region 界面控制

        #region 子界面的操作

        /// <summary>
        /// 设置要显示的子界面
        /// </summary>
        /// <param name="subViewKey">子界面Key</param>
        /// <param name="params">参数</param>
        private void SetSubView(string subViewKey, object @params)
        {
            _functionKey = subViewKey;
            SubView = DevHomePageSubViewHelper.GetOrCreateView(_functionKey, @params);
        }

        /// <summary>
        /// 释放之前打开的子界面
        /// </summary>
        private void ReleaseSubView()
        {
            DevHomePageSubViewHelper.ReleaseView(_functionKey);
            SubView = null;
        }

        #endregion

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
        /// 返回首页
        /// </summary>
        public ICommand BackToDevHomePageCommand { get; private set; }

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
        /// 修改设备型号
        /// </summary>
        public ICommand EditPhoneCommand { get; private set; }

        /// <summary>
        /// 手机拍照
        /// </summary>
        public ProxyRelayCommand PhoneTakePhotoCommand { get; private set; }

        /// <summary>
        /// 推荐方案
        /// </summary>
        public ProxyRelayCommand<StrategyElement> StrategyRecommendCommand { get; private set; }

        #endregion

        [ImportingConstructor]
        public DeviceHomePageViewModel(IPopupWindowService popupService)
        {
            BackToDevHomePageStatus = true;

            base.MessageAggregation.RegisterGeneralMsg<KeyValuePair<String, Boolean>>(this, GeneralKeys.ExtractDeviceStateMsg, ExtractDeviceStateCallback);

            StrategyRecommendItems = new ObservableCollection<StrategyElement>();
            ToolkitItems = new ObservableCollection<string>();
            EditInfo = new DevHomePageEditItemModel();

            PhoneTakePhotoCommand = new ProxyRelayCommand(ExecutePhoneTakePhotoCommand, base.ModelName);
            StrategyRecommendCommand = new ProxyRelayCommand<StrategyElement>(ExecuteStrategyRecommendCommand, base.ModelName);
            SaveEditCommand = new ProxyRelayCommand<DevHomePageEditItemModel>(ExecuteSaveEditCommand, base.ModelName);
            CancelEditCommand = new RelayCommand(ExecuteCancelEditCommand);
            AutoExtractCommand = new RelayCommand(ExecuteAutoExtractCommand);
            EditPhoneCommand = new RelayCommand(ExecuteEditPhoneCommand);
            BackToDevHomePageCommand = new RelayCommand(ExeucteBackToDevHomePageCommand);

            CurLoginUserName = SystemContext.Instance.CurUserInfo.UserName;
            CurLoginUserID = SystemContext.Instance.CurUserInfo.IdNumber;

            PopupWindowService = popupService;




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

            //获取当前的采集信息
            if (CurDevModel.IDevSource.CollectionInfo != null)
            {
                EditInfo.CensorshipPerson = CurDevModel.IDevSource.CollectionInfo.SenderName;
                EditInfo.CensorshipPersonCredentialsNo = CurDevModel.IDevSource.CollectionInfo.SenderCertificateCode;
                EditInfo.CredentialsNo = CurDevModel.IDevSource.CollectionInfo.CollectorCertificateCode;
                EditInfo.CredentialsType = CurDevModel.IDevSource.CollectionInfo.HolderCertificateType;
                EditInfo.Desciption = CurDevModel.IDevSource.CollectionInfo.Description;
                EditInfo.Holder = CurDevModel.IDevSource.CollectionInfo.HolderName;
                EditInfo.HolderCredentialsNo = CurDevModel.IDevSource.CollectionInfo.HolderCertificateCode;
                EditInfo.No = CurDevModel.IDevSource.CollectionInfo.DataNo;
                EditInfo.Operator = CurDevModel.IDevSource.CollectionInfo.CollectorName;
                EditInfo.UnitName = CurDevModel.IDevSource.CollectionInfo.SenderCompany;
            }
            else
            {
                EditInfo.Operator = SystemContext.Instance.CurUserInfo.UserName;
                EditInfo.CredentialsNo = SystemContext.Instance.CurUserInfo.IdNumber;
            }
            SetEditInfo(EditInfo);

            //根据设备ID创建对应设备的子界面导航器\
            var adorner = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
            DevHomePageSubViewHelper = new SubViewCacheManager(adorner.Id);
        }

        #endregion

        #region ExecuteCommands

        /// <summary>
        /// 返回设备首页
        /// </summary>
        private void ExeucteBackToDevHomePageCommand()
        {
            ReleaseSubView();
        }

        //保存编辑信息
        private string ExecuteSaveEditCommand(DevHomePageEditItemModel editInfo)
        {
            string opResult = string.Empty;

            if (CurEditStatus && editInfo != null)
            {
                //保存信息
                CurDevModel.IDevSource.CollectionInfo = CurDevModel.IDevSource.CollectionInfo ?? new ExportCollectionInfo();
                CurDevModel.IDevSource.CollectionInfo.DataNo = editInfo.No;
                CurDevModel.IDevSource.CollectionInfo.HolderName = editInfo.Holder;
                CurDevModel.IDevSource.CollectionInfo.HolderCertificateType = editInfo.CredentialsType;
                CurDevModel.IDevSource.CollectionInfo.HolderCertificateCode = editInfo.HolderCredentialsNo;
                CurDevModel.IDevSource.CollectionInfo.SenderName = editInfo.CensorshipPerson;
                CurDevModel.IDevSource.CollectionInfo.SenderCompany = editInfo.UnitName;
                CurDevModel.IDevSource.CollectionInfo.SenderCertificateCode = editInfo.CensorshipPersonCredentialsNo;
                CurDevModel.IDevSource.CollectionInfo.CollectorName = editInfo.Operator;
                CurDevModel.IDevSource.CollectionInfo.CollectorCertificateCode = editInfo.CredentialsNo;
                CurDevModel.IDevSource.CollectionInfo.Description = editInfo.Desciption;
                var tmp = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
                tmp.Save();

                SetEditInfo(editInfo);
                opResult = "保存提取信息成功";
            }

            CurEditStatus = !CurEditStatus;
            return opResult;
        }

        //执行推荐方案
        private string ExecuteStrategyRecommendCommand(StrategyElement arg)
        {
            if (arg.SolutionStrategyName == "物理镜像")
            {
                //物理镜像【测试代码】
                SetSubView(ExportKeys.MirrorView, CurDevModel.IDevSource);
                //SetSubView(ExportKeys.Mirror9008View, CurDevModel.IDevSource);
            }
            else
            {
                var adorner = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
                XLY.SF.Project.CaseManagement.ExtractItem ei =
               adorner.Target.CreateExtract(SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_StrategyRecommend_AutoExtraction],
                                           SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_StrategyRecommend_AutoExtraction]);
                Domains.Device phone = (Domains.Device)CurDevModel.IDevSource;
                Pump @params = new Pump(ei.Path, "data.db", adorner.Id) { Solution = PumpSolution.Downgrading };
                @params.Type = EnumPump.USB;
                @params.OSType = phone.OSType;
                @params.Source = phone;

                SetSubView(ExportKeys.ExtractionView, @params);
            }

            return $"执行推荐方案{arg.SolutionStrategyName}";
        }

        public void ExecuteEditPhoneCommand()
        {
            DeviceInfoModel devideInfo = (DeviceInfoModel)PopupWindowService.ShowDialogWindow(ExportKeys.DeviceEditView);
        }

        private void ExecuteAutoExtractCommand()
        {
            var adorner = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
            XLY.SF.Project.CaseManagement.ExtractItem ei =
                adorner.Target.CreateExtract(SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_StrategyRecommend_AutoExtraction],
                                            SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_StrategyRecommend_AutoExtraction]);

            Pump @params = new Pump(ei.Path, "data.db", adorner.Id);

            switch (CurDevModel.IDevSource)
            {
                case Domains.Device phone://手机
                    @params.Type = EnumPump.USB;
                    @params.OSType = phone.OSType;
                    @params.Source = phone;
                    break;
                case LocalFileDevice lfDeivce://本地文件/文件夹
                    if (lfDeivce.IsDirectory)
                    {//文件夹
                        @params.Type = EnumPump.LocalData;
                        @params.OSType = lfDeivce.OSType;
                        @params.Source = lfDeivce.PathName;
                    }
                    else
                    {//镜像文件
                        @params.Type = EnumPump.Mirror;
                        @params.OSType = lfDeivce.OSType;
                        @params.Source = lfDeivce.PathName;
                    }
                    break;
            }

            SetSubView(ExportKeys.ExtractionView, @params);
        }

        private string ExecutePhoneTakePhotoCommand()
        {
            var adorner = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
            string path=adorner.Target.Path;
            PopupWindowService.ShowDialogWindow(ExportKeys.TakePhotoView, path);
            return string.Empty;
        }

        //取消编辑状态
        private void ExecuteCancelEditCommand()
        {
            CurEditStatus = false;
            //恢复之前的值
            SetEditInfo(EditInfo);
        }

        #endregion

        #region Tools

        private void ExtractDeviceStateCallback(GeneralArgs<KeyValuePair<String, Boolean>> args)
        {
            var tmp = (DeviceExtractionAdorner)CurDevModel.DeviceExtractionAdorner;
            if (args.Parameters.Key != tmp.Id) return;
            tmp.IsIdle = args.Parameters.Value;
            BackToDevHomePageStatus = args.Parameters.Value;
        }

        /// <summary>
        /// 设置编辑的显示值
        /// </summary>
        /// <param name="info"></param>
        private void SetEditInfo(DevHomePageEditItemModel info)
        {
            EditInfo.CensorshipPerson = info.CensorshipPerson;
            EditInfo.CensorshipPersonCredentialsNo = info.CensorshipPersonCredentialsNo;
            EditInfo.CredentialsNo = info.CredentialsNo;
            EditInfo.CredentialsType = info.CredentialsType;
            EditInfo.Desciption = info.Desciption;
            EditInfo.Holder = info.Holder;
            EditInfo.HolderCredentialsNo = info.HolderCredentialsNo;
            EditInfo.No = info.No;
            EditInfo.Operator = info.Operator;
            EditInfo.UnitName = info.UnitName;
        }

        #endregion
    }
}
