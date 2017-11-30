using System.Collections.Generic;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Export
{
    /// <summary>
    /// ExportData.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.ExportDataView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ExportData : UcViewBase
    {
        public ExportData()
        {
            InitializeComponent();
            PropertyNodeItem node1tag3 = new PropertyNodeItem()
            {
                Name = "3333333333",
            };

            PropertyNodeItem node1tag33 = new PropertyNodeItem()
            {
                Name = "3333333333",

            };


            List<PropertyNodeItem> itemList = new List<PropertyNodeItem>();
            PropertyNodeItem node1 = new PropertyNodeItem()
            {
                IsItemStyle = true,
                Name = "11111111111111111111111111",
            };

            PropertyNodeItem node1tag1 = new PropertyNodeItem()
            {
                Name = "22222222222",
            };
            node1tag1.Children.Add(node1tag3);
            node1tag1.Children.Add(node1tag33);
            node1.Children.Add(node1tag1);


            PropertyNodeItem node1tag2 = new PropertyNodeItem()
            {
                Name = "22222222222",
            };
            node1tag2.Children.Add(node1tag3);
            node1tag2.Children.Add(node1tag33);
            node1.Children.Add(node1tag2);


            itemList.Add(node1);

            PropertyNodeItem node2 = new PropertyNodeItem()
            {
                IsItemStyle = true,
                Name = "11111111111111111111111",

            };

            PropertyNodeItem node2tag3 = new PropertyNodeItem()
            {

                Name = "22222222222222",

            };
            node2.Children.Add(node2tag3);

            PropertyNodeItem node2tag4 = new PropertyNodeItem()
            {

                Name = "22222222222222",

            };
            node2.Children.Add(node2tag4);
            itemList.Add(node2);


            this.tvProperties.ItemsSource = itemList;
        }

        [Import(ExportKeys.ExportDataViewViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }
    }
    public class PropertyNodeItem
    {
        public bool IsItemStyle { get; set; }
        public string Name { get; set; }

        public List<PropertyNodeItem> Children { get; set; }
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();
        }
    }
}
