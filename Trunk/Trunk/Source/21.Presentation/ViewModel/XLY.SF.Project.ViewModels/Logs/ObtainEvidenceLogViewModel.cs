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
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model;

namespace XLY.SF.Project.ViewModels.Logs
{

    [Export(ExportKeys.ObtainEvidenceLogViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ObtainEvidenceLogViewModel : ViewModelBase
    {
        #region Properties

        #region 取证日志数据

        /// <summary>
        /// 取证日志数据
        /// </summary>
        public ObservableCollection<ObtainEvidenceLogModel> LogItems { get; private set; }

        #endregion

        #region 全选标识

        private bool? _checkedAllLogs;

        public bool? CheckedAllLogs
        {
            get
            {
                return this._checkedAllLogs;
            }

            set
            {
                this._checkedAllLogs = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region ICommands

        /// <summary>
        /// 选择单项日志
        /// </summary>
        public ICommand CheckedLogCommand { get; private set; }
        /// <summary>
        /// 全选状态
        /// </summary>
        public ICommand SelectedAllStatusCommand { get; private set; }
        /// <summary>
        /// 查看图片
        /// </summary>
        public ICommand ShowOpImageCommand { get; private set; }

        #endregion

        #endregion

        public ObtainEvidenceLogViewModel()
        {
            LogItems = new ObservableCollection<ObtainEvidenceLogModel>();

            CheckedLogCommand = new RelayCommand(ExecuteCheckedLogCommand);
            SelectedAllStatusCommand = new RelayCommand<bool>(ExecuteSelectedAllStatusCommand);
            ShowOpImageCommand = new RelayCommand<ObtainEvidenceLogModel>(ExecuteShowOpImageCommand);

            for (int i = 0; i < 10; i++)
            {
                LogItems.Add(new ObtainEvidenceLogModel()
                {
                    Index = i,
                    ImageNameForScreenShot = "",
                    OpContent = $"测试内容{i}",
                    OperationModel = $"测试模块{i}",
                    OpTime = DateTime.Now,
                    DisplayOperator = SystemContext.Instance.CurUserInfo.UserName,
                });
            }

            CheckedAllLogs = false;
        }

        #region ExecuteCommands

        private void ExecuteShowOpImageCommand(ObtainEvidenceLogModel obj)
        {

        }

        private void ExecuteSelectedAllStatusCommand(bool obj)
        {
            foreach (var item in LogItems)
            {
                item.IsChecked = obj;
            }
        }

        private void ExecuteCheckedLogCommand()
        {
            var checkedCount = LogItems.LongCount(t => t.IsChecked);
            if (checkedCount == 0)
                CheckedAllLogs = false;
            else if (checkedCount == LogItems.Count)
                CheckedAllLogs = true;
            else
                CheckedAllLogs = null;
        }

        #endregion
    }
}
