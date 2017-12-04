using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Management.Settings
{
    /// <summary>
    /// SettingsView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.SettingsView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SettingsView : UcViewBase
    {
        private double[] ItemHeights;

        public SettingsView()
        {
            InitializeComponent();
            String[] items = new String[]
            {
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Settings_Basic],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Settings_CaseType],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Settings_Unit],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Settings_Inspection],
            };
            Blocks.ItemsSource = items;
            Blocks.SelectedIndex = 0;

            Blocks.SelectionChanged += Blocks_SelectionChanged;
            sv.ScrollChanged += Sv_ScrollChanged;
        }

        private void SettingsView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemHeights = new double[sp.Children.Count];
            for (int i = 0; i < ItemHeights.Length; i++)
            {
                var uiTmp = sp.Children[i] as FrameworkElement;
                ItemHeights[i] = uiTmp.ActualHeight;
            }
        }

        bool isMouseScroll;
        private void Blocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isMouseScroll)
            {
                var a = GetOffsetHeight(Blocks.SelectedIndex);
                sv.ScrollToVerticalOffset(a);
            }
        }

        private void Sv_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            isMouseScroll = true;
            //向下
            int index = 0;
            double tmpOffset = ItemHeights[index];
            while (tmpOffset <= e.VerticalOffset)
            {
                index++;
                tmpOffset += ItemHeights[index];
            }
            Blocks.SelectedIndex = index;

            //if (e.VerticalChange > 0)
            //{
            //}
            //else if (e.VerticalChange < 0)
            //{
            //    //向上

            //}
        }

        private double GetOffsetHeight(int selectedIndex)
        {
            double result = 0;
            selectedIndex--;
            for (int i = selectedIndex; i >= 0; i--)
            {
                result += ItemHeights[i];
            }
            return result;
        }

        private void Blocks_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseScroll = false;
        }
    }
}
