using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model.MessageElement;
using XLY.SF.Project.ViewModels.Tools;

namespace XLY.SF.Project.ViewModels.Main
{
    [Export(ExportKeys.HomePageViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomePageViewModel : ViewModelBase
    {
        private IPopupWindowService _winService;

        #region Constructors

        [ImportingConstructor]
        public HomePageViewModel(IDatabaseContext dbService, IPopupWindowService service)
        {
            _dbService = dbService;
            _winService = service;

            CreateCaseCommand = new ProxyRelayCommand(ExecuteCreateCaseCommand);
            OpenAllCaseCommand = new ProxyRelayCommand(ExecuteOpenAllCaseCommand);
            OpenLocalCaseCommand = new ProxyRelayCommand(ExecuteOpenLocalCaseCommand);
            OpenCaseCommand = new ProxyRelayCommand<RecentCaseEntityModel>(ExecuteOpenCaseCommand);

            RecentCaseItems = new ObservableCollection<RecentCaseEntityModel>();

            LoadRecentCreationCaseItems();
        }

        #endregion

        #region Commands

        /// <summary>
        /// 新建案例
        /// </summary>
        public ProxyRelayCommand CreateCaseCommand { get; set; }
        /// <summary>
        /// 打开所有案例
        /// </summary>
        public ProxyRelayCommand OpenAllCaseCommand { get; set; }
        /// <summary>
        /// 打开本地案例
        /// </summary>
        public ProxyRelayCommand OpenLocalCaseCommand { get; set; }
        /// <summary>
        /// 打开案例
        /// </summary>
        public ProxyRelayCommand<RecentCaseEntityModel> OpenCaseCommand { get; set; }

        #endregion

        #region Propertes

        #region Private

        /// <summary>
        /// 数据库服务
        /// </summary>
        private IDatabaseContext _dbService;

        #endregion

        /// <summary>
        /// 最近案例
        /// </summary>
        public ObservableCollection<RecentCaseEntityModel> RecentCaseItems { get; set; }

        #endregion

        #region Tools

        //加载最新创建案例
        private void LoadRecentCreationCaseItems()
        {
            var @case = _dbService.RecentCases.OrderByDescending((t) => t.Timestamp).Take(10).ToModels<RecentCase, RecentCaseEntityModel>();
            foreach (var item in @case)
            {
                RecentCaseItems.Add(item);
            }
        }

        #endregion

        protected override void Closed()
        {

        }

        #region ExcuteCommands

        private string ExecuteOpenCaseCommand(RecentCaseEntityModel arg)
        {
            var @case = Case.Open(arg.CaseProjectFile);
            SystemContext.Instance.CurrentCase = @case;
            base.NavigationForMainWindow(ExportKeys.DeviceSelectView);
            return string.Format("打开案例{0}成功", @case.Name);
        }

        private string ExecuteCreateCaseCommand()
        {
            //展开创建案例界面
            SubViewMsgModel subViewStatus = new SubViewMsgModel(false, true);
            SubViewNavigationHelper.SetSubViewStatus(subViewStatus);

            return "打开新建案例";
        }

        private string ExecuteOpenAllCaseCommand()
        {
            base.NavigationForNewDislogWindow(ExportKeys.CaseListView);
            return "打开所有案例";
        }

        private string ExecuteOpenLocalCaseCommand()
        {
            string fileFullPath = _winService.OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(fileFullPath))
            {
                var @case = Case.Open(fileFullPath);
                SystemContext.Instance.CurrentCase = @case;
                base.NavigationForMainWindow(ExportKeys.DeviceSelectView);
                return string.Format("打开本地案例{0}成功", @case.Name);
            }
            return "";
        }

        #endregion
    }
}
