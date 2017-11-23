using System;
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
using XLY.SF.Project.Themes;

namespace XLY.SF.Project.Plugin.DataView.View.Controls
{
    /// <summary>
    /// DefaultGridViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class DefaultGridViewControl : UserControl
    {
        public DefaultGridViewControl()
        {
            InitializeComponent();
            this.Loaded += DefaultGridViewControl_Loaded;
        }

        private void DefaultGridViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindGrid();
        }

        #region 公共属性和方法

        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;
        #endregion

        #region praviate
        DataViewPluginArgument _arg => this.DataContext as DataViewPluginArgument;
        #endregion

        #region 事件
        /// <summary>
        /// 表格行选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedDataChanged?.Invoke((sender as DataGrid).SelectedItem);
        }

        /// <summary>
        /// 点击了全部标记按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int newBmk = (bool)cb.IsChecked ? 0 : -1;
            foreach (var item in _arg.Items.View)
            {
                (item as AbstractDataItem).BookMarkId = newBmk;
            }
        }

        /// <summary>
        /// 列模板定义中的属性名称，在读取列模板时替换为实际的属性
        /// </summary>
        private const string PROPERTY_NAME = "$PropertyName$";

        /// <summary>
        /// 动态生成表格列
        /// </summary>
        private void BindGrid()
        {
            dg.Columns.Clear();
            object type = _arg.CurrentData is TreeNode node ? node.Type : _arg.CurrentData is AbstractDataSource sp ? sp.Type : null;
            if(type == null)
            {
                return;
            }
            //添加书签列
            DataGridTemplateColumn bmkCol = this.FindResource("bookmarkColumnTemplate") as DataGridTemplateColumn;
            dg.Columns.Add(bmkCol);

            if (type is Type t)
            {
                foreach(var attr in DisplayAttributeHelper.FindDisplayAttributes(t).OrderBy(d=>d.ColumnIndex))
                {
                    if (attr.Visibility == EnumDisplayVisibility.ShowInDatabase)        //该属性不需要显示在界面上
                        continue;

                    if(attr.Owner.Name == "DataState")      //如果是状态列，则单独处理
                    {
                        DataGridTemplateColumn stateCol = new DataGridTemplateColumn() { Header = attr.Text };
                        stateCol.CellTemplate = XamlResouceReader.ToDataTemplate<DataTemplate>("ThemesStyle.DataGridStyle.DataGridDataStateColumnTemplate.xaml", c => c.Replace(PROPERTY_NAME, attr.Owner.Name));
                        dg.Columns.Add(stateCol);
                    }
                    else if (attr.ColumnType == EnumColumnType.URL) //超链接列
                    {
                        DataGridTemplateColumn col = new DataGridTemplateColumn() { Header = attr.Text };
                        col.CellTemplate = XamlResouceReader.ToDataTemplate<DataTemplate>("ThemesStyle.DataGridStyle.DataGridUrlColumnTemplate.xaml", c => c.Replace(PROPERTY_NAME, attr.Owner.Name));
                        dg.Columns.Add(col);
                    }
                    else if (attr.ColumnType == EnumColumnType.Image)  //图片列
                    {
                        DataGridTemplateColumn col = new DataGridTemplateColumn() { Header = attr.Text };
                        col.CellTemplate = XamlResouceReader.ToDataTemplate<DataTemplate>("ThemesStyle.DataGridStyle.DataGridImageColumnTemplate.xaml", c => c.Replace(PROPERTY_NAME, attr.Owner.Name));
                        dg.Columns.Add(col);
                    }
                    else
                    {
                        DataGridBoundColumn col = new DataGridTextColumn() { Header = attr.Text, Binding = new Binding(attr.Owner.Name), Width = attr.Width, MinWidth = 50 };
                        dg.Columns.Add(col);
                    }
                }
            }
            else if(type is string)
            {

            }
        }
        #endregion

    }
}
