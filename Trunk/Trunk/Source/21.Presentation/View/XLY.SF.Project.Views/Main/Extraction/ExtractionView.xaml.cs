using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Extraction
{
    /// <summary>
    /// ExtractionView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.ExtractionView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ExtractionView : UcViewBase
    {
        #region Fields

        private readonly Dictionary<Object, CheckBox> _headers = new Dictionary<Object, CheckBox>();

        #endregion

        #region Constructors

        public ExtractionView()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        [Import(ExportKeys.ExtractionViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource { get => base.DataSource; set => base.DataSource = value; }

        #endregion

        #region Methods

        private void SelectAll_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeSelectAll(true);
        }

        private void SelectAll_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeSelectAll(false);
        }

        private void ChangeSelectAll(Boolean isChecked)
        {
            foreach (CheckBox cb in _headers.Values)
            {
                cb.IsChecked = isChecked;
            }
        }

        private void UpdateSelectAll()
        {
            Boolean? isChecked = false;
            if (_headers.Count != 0)
            {
                if (_headers.Values.All(x => x.IsChecked.HasValue && x.IsChecked.Value))
                {
                    isChecked = true;
                }
                else if (_headers.Values.All(x => x.IsChecked.HasValue && !x.IsChecked.Value))
                {
                    isChecked = false;
                }
                else
                {
                    isChecked = null;
                }
            }
            SelectAll.IsChecked = isChecked;
        }

        private void ItemHeader_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)e.Source;
            CollectionViewGroup group = (CollectionViewGroup)cb.DataContext;
            if (!_headers.ContainsKey(group.Name))
            {
                _headers.Add(group.Name, cb);
            }
        }

        private void ItemHeader_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            dynamic internalCollection = ((CheckBox)sender).DataContext;
            SelectGroup((IEnumerable)internalCollection.Items, true);
        }

        private void ItemHeader_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            dynamic internalCollection = ((CheckBox)sender).DataContext;
            SelectGroup((IEnumerable)internalCollection.Items, false);
        }

        private void SelectGroup(IEnumerable items, Boolean isChecked)
        {
            Object groupName = null;
            foreach (dynamic item in items)
            {
                item.IsChecked = isChecked;
                if (groupName == null)
                {
                    groupName = item.Group;
                }
            }
            if (groupName != null)
            {
                _headers[groupName].IsChecked = isChecked;
            }
            UpdateSelectAll();
        }

        private void cb_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox source = (CheckBox)sender;
            dynamic o = source.DataContext;
            UpdateSelectionWhenItemSelectionChanged(o.Group);
        }

        private void UpdateSelectionWhenItemSelectionChanged(String group)
        {
            CheckBox cb = _headers[group];
            var temp = ExtractItems.ItemsSource.Cast<dynamic>().Where(x => x.Group == group);
            if (temp.All(x => x.IsChecked))
            {
                cb.IsChecked = true;
            }
            else if (temp.All(x => !x.IsChecked))
            {
                cb.IsChecked = false;
            }
            else
            {
                cb.IsChecked = null;
            }
            UpdateSelectAll();
        }

        private void Root_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dynamic obj = DataSource;
            obj.SelectedPlan = null;
        }

        #endregion
    }
}
