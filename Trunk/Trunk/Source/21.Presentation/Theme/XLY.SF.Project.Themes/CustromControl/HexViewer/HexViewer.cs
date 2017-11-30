using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.SF.Project.Themes.CustromControl
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl.HexViewer"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl.HexViewer;assembly=XLY.SF.Project.Themes.CustromControl.HexViewer"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:HexViewer/>
    ///
    /// </summary>
    public class HexViewer : Control
    {
        static HexViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer), new FrameworkPropertyMetadata(typeof(HexViewer)));
        }

        Grid _gridLayout;
        TextBox _tbLine;
        TextBox _tbByte;
        TextBox _tbText;
        ScrollBar _scroll;
        ComboBox _cbEncoding;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _gridLayout = GetTemplateChild("PART_Container") as Grid;
            _tbLine = GetTemplateChild("PART_Line") as TextBox;
            _tbByte = GetTemplateChild("PART_Byte") as TextBox;
            _tbText = GetTemplateChild("PART_Text") as TextBox;
            _scroll = GetTemplateChild("PART_Scroll") as ScrollBar;
            _cbEncoding = GetTemplateChild("PART_Encoding") as ComboBox;
        }
    }
}
