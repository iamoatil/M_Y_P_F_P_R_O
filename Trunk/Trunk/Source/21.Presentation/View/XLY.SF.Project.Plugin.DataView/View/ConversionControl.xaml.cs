﻿using System;
using System.Collections.Generic;
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
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// ConversionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ConversionControl : UserControl
    {
        public ConversionControl()
        {
            InitializeComponent();

            this.Loaded += ConversionControl_Loaded;
        }

        private void ConversionControl_Loaded(object sender, RoutedEventArgs e)
        {
            //设置数据源，将Items设置到CollectionViewSource中，便于按时间分组展示消息
            CollectionViewSource converisonList = (CollectionViewSource)this.FindResource("conversionCollectionViewSource");
            converisonList.Source = (this.DataContext as DataViewPluginArgument)?.Items?.View;
        }

        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;

        DataViewPluginArgument _arg => this.DataContext as DataViewPluginArgument;
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedDataChanged?.Invoke(new DataPreviewPluginArgument() { CurrentData = lsb1.SelectedValue, PluginId = _arg.DataSource?.PluginInfo?.Guid, Type = (_arg.CurrentData as dynamic)?.Type });
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = true;
        }
    }

    //public class ConversionTemplateSelector : DataTemplateSelector
    //{
    //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    //    {
    //        var fe = container as FrameworkElement;
    //        var message = item as MessageCore;
    //        DataTemplate dt = null;
    //        if (message != null && fe != null)
    //        {
    //            if (message.SendState == EnumSendState.Send)
    //            {
    //                if (message.Type == EnumColumnType.String)
    //                {
    //                    dt = fe.FindResource("left_txt") as DataTemplate;
    //                }
    //                else if (message.Type == EnumColumnType.Image)
    //                {
    //                    dt = fe.FindResource("left_txt") as DataTemplate;
    //                }
    //            }
    //            else if (message.SendState == EnumSendState.Receive)
    //            {
    //                if (message.Type == EnumColumnType.String)
    //                {
    //                    dt = fe.FindResource("right_txt") as DataTemplate;
    //                }
    //                else if (message.Type == EnumColumnType.Image)
    //                {
    //                    dt = fe.FindResource("right_txt") as DataTemplate;
    //                }
    //            }
    //        }
    //        return dt;
    //    }
    //}

    //public class ConversionStyleSelector: StyleSelector
    //{
    //    public Style StyleLeft { get; set; }
    //    public Style StyleRight { get; set; }
    //    public override Style SelectStyle(object item, DependencyObject container)
    //    {
    //        Style re = null;
    //        var fe = container as FrameworkElement;
    //        var message = item as MessageCore;
    //        if (message != null && fe != null)
    //        {
    //            if (message.SendState == EnumSendState.Send)
    //            {
    //                return StyleLeft;
    //            }
    //            else if (message.SendState == EnumSendState.Receive)
    //            {
    //                return StyleRight;
    //            }
    //        }
    //        return re;
    //    }
    //}
}
