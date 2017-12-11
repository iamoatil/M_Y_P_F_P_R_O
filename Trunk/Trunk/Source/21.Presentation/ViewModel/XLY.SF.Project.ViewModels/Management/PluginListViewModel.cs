using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Management
{
    [Export(ExportKeys.SettingsPluginListViewViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PluginListViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _searchProxyCommand;

        private IEnumerable<AbstractPluginInfo> _caches;

        #endregion

        #region Constructors

        public PluginListViewModel()
        {
            _searchProxyCommand = new ProxyRelayCommand(Search, base.ModelName, () => Plugins != null);
        }

        #endregion

        #region Properties

        public ICommand SearchCommand => _searchProxyCommand.ViewExecuteCmd;

        #region Keyword

        private String _keyword = String.Empty;
        public String Keyword
        {
            get => _keyword;
            set
            {
                if (value == null)
                {
                    _keyword = String.Empty;
                }
                else
                {
                    _keyword = value.Trim();
                }
                OnPropertyChanged();
            }
        }

        #endregion

        #region Keyword

        private IEnumerable<AbstractPluginInfo> _plugins;
        public IEnumerable<AbstractPluginInfo> Plugins
        {
            get => _plugins;
            set
            {
                _plugins = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            _caches = PluginAdapter.Instance.Plugins.Keys.Where(x=>x.PluginType == PluginType.SpfDataParse);
            Plugins = _caches.ToArray();
        }

        #endregion

        #region Private

        private String Search()
        {
            String keyword = Keyword;
            if (String.IsNullOrWhiteSpace(keyword))
            {
                Plugins = _caches.ToArray();
            }
            else
            {
                Plugins = _caches.Cast<DataParsePluginInfo>().Where(x => x.Name.Contains(keyword,StringComparison.OrdinalIgnoreCase)
                || x.Group.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || keyword.IsSet(x.DeviceOSType)
                || keyword.IsSet(x.Pump)
                || x.AppName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || x.Version.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || x.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }
            return $"搜索关键字：{Keyword}";
        }

        #endregion

        #endregion
    }
}
