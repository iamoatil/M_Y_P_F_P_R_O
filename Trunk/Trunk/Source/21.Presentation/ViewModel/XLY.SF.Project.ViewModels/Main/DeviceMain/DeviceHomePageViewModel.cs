using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model;
using XLY.SF.Project.ViewDomain.VModel.DevHomePage;

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

        #endregion

        public DeviceHomePageViewModel()
        {
            StrategyRecommendItems = new ObservableCollection<StrategyElement>();
            ToolkitItems = new ObservableCollection<string>();

            CancelEditCommand = new RelayCommand(ExecuteCancelEditCommand);










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

        protected override void LoadCore(object parameters)
        {
        }

        #endregion

        #region Commands

        /// <summary>
        /// 取消编辑
        /// </summary>
        public ICommand CancelEditCommand { get; set; }

        #endregion

        #region ExecuteCommands

        private void ExecuteCancelEditCommand()
        {

        }

        #endregion
    }
}
