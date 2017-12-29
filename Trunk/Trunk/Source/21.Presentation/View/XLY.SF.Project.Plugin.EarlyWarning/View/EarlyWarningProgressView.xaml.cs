using System;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewModels.Main.DeviceMain;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// EarlyWarningProgressView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.AutoWarningProgressView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class EarlyWarningProgressView : UcViewBase
    {
        public EarlyWarningProgressView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.AutoWarningProgressViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;

                _earlyWarningProgressVm = DataSource as EarlyWarningProgressViewModel;
                _earlyWarningProgressVm.ReplaceViewContentAction += OnShowContent;

                _originalContent = this.Content;
            }
        }

        EarlyWarningProgressViewModel _earlyWarningProgressVm;

        object _originalContent;

        /// <summary>
        /// 使用结果视图替换现有进度视图
        /// </summary>
        private void OnShowContent(bool isOriginal)
        {
            this.Dispatcher.BeginInvoke(new Action(()=> { OnInnerShowContent(isOriginal); }),null);
        }

        private void OnInnerShowContent(bool isOriginal)
        {
            if (!isOriginal)
            {
                var inspection = "Inspection;" + _earlyWarningProgressVm.Path;

                UcViewBase autoWarningView = IocManagerSingle.Instance.GetViewPart(ExportKeys.AutoWarningView);
                autoWarningView.DataSource.LoadViewModel(inspection);

                this.Content = autoWarningView;
            }
            else
            {
                this.Content = _originalContent;
            }
        }
    }
}
