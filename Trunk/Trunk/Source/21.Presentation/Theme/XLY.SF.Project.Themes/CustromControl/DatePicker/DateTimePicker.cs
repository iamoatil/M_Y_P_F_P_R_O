using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl;assembly=XLY.SF.Project.Themes.CustromControl"
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
    ///     <MyNamespace:DateTimePicker/>
    ///
    /// </summary>
    [TemplatePart(Name = "tgb_PopupStatus", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "dateSelector", Type = typeof(Popup))]
    [TemplatePart(Name = "DayContainer", Type = typeof(StackPanel))]
    [TemplatePart(Name = "gd_DayContainer", Type = typeof(Grid))]
    [TemplatePart(Name = "btn_DateTitle", Type = typeof(Button))]
    [TemplatePart(Name = "btn_Before", Type = typeof(Button))]
    [TemplatePart(Name = "btn_After", Type = typeof(Button))]
    public class DateTimePicker : Control
    {
        #region Properties

        private ToggleButton _dateSelectorStatus;
        private Popup _dateSelector;
        private StackPanel _dayContainer;
        private WrapPanel _yearContainer;
        private WrapPanel _monthContainer;
        private Button _btnDateTitle;
        private WrapPanel _wpTime;

        private List<RadioButton> _dayItems;

        /// <summary>
        /// 当前输入的时间
        /// </summary>
        private TextBox _curInputTime;

        #region 动画

        private Storyboard _onDayToMonth;
        private Storyboard _onMonthToYear;
        private Storyboard _onYearToMonth;
        private Storyboard _onMonthToDay;
        private Storyboard _onBefore;
        private Storyboard _onAfter;
        private Storyboard _onReset;

        #endregion

        #region 界面显示值【年月日时分秒】
        /// <summary>
        /// 当前界面显示值
        /// </summary>
        private DateTimeElement _dtEmt;

        #endregion

        #region 当前状态

        private DateTimePickerStatus _curStatus;

        /// <summary>
        /// 当前状态显示
        /// </summary>
        private DateTimePickerStatus CurStatus
        {
            get
            {
                return _curStatus;
            }
            set
            {
                _curStatus = value;
                RefreshDateTime(value, _dtEmt);
                RefreshTime(_dtEmt.Hour, _dtEmt.Minute, _dtEmt.Second);
            }
        }

        #endregion

        #endregion

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        public override void OnApplyTemplate()
        {
            _dtEmt = new DateTimeElement();

            _dateSelectorStatus = this.Template.FindName("tgb_PopupStatus", this) as ToggleButton;
            _dateSelector = this.Template.FindName("dateSelector", this) as Popup;
            _dayContainer = this.Template.FindName("DayContainer", this) as StackPanel;
            _yearContainer = this.Template.FindName("YearContainer", this) as WrapPanel;
            _monthContainer = this.Template.FindName("MonthContainer", this) as WrapPanel;
            _wpTime = this.Template.FindName("wp_Time", this) as WrapPanel;
            _btnDateTitle = this.Template.FindName("btn_DateTitle", this) as Button;
            var _popupMainContainer = this.Template.FindName("gd_PopupMain", this) as Grid;
            var hour = this.Template.FindName("Hour", this) as TextBox;
            var minute = this.Template.FindName("Minute", this) as TextBox;
            var second = this.Template.FindName("Second", this) as TextBox;

            _dayItems = new List<RadioButton>();

            if (_dateSelector != null && _dateSelectorStatus != null &&
                _dayContainer != null && _yearContainer != null &&
                _btnDateTitle != null && _monthContainer != null &&
                _wpTime != null && hour != null && minute != null && second != null && _popupMainContainer != null)
            {
                _onDayToMonth = _dateSelector.Resources["OnDayToMonth"] as Storyboard;
                _onMonthToYear = _dateSelector.Resources["OnMonthToYear"] as Storyboard;
                _onYearToMonth = _dateSelector.Resources["OnYearToMonth"] as Storyboard;
                _onMonthToDay = _dateSelector.Resources["OnMonthToDay"] as Storyboard;
                _onBefore = _dateSelector.Resources["OnBeforePage"] as Storyboard;
                _onAfter = _dateSelector.Resources["OnAfterPage"] as Storyboard;
                _onReset = _dateSelector.Resources["OnReset"] as Storyboard;

                _dateSelectorStatus.Checked += _dateSelectorStatus_Checked;
                _btnDateTitle.Click += _btnDateTitle_Click;
                _dateSelector.Closed += _dateSelector_Closed;
                hour.GotFocus += Hour_GotFocus;
                minute.GotFocus += Minute_GotFocus;
                second.GotFocus += Second_GotFocus;
                hour.TextChanged += Hour_TextChanged;
                minute.TextChanged += Hour_TextChanged;
                second.TextChanged += Hour_TextChanged;
                _monthContainer.AddHandler(RadioButton.ClickEvent, new RoutedEventHandler(InMonthCallback));
                _popupMainContainer.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClickCallback));



                InitTimeContainer();
                var days = LoadDateItems(_dtEmt.Year, _dtEmt.Month);
                InitDayContainer(days);
                var years = LoadYearItems(_dtEmt.Year);
                InitYearContainer(years);
            }
        }

        //按钮点击回调
        private void ButtonClickCallback(object sender, RoutedEventArgs e)
        {
            var clickBtn = e.OriginalSource as ButtonBase;
            if (clickBtn != null)
            {
                int selectValue;
                int.TryParse(clickBtn.Content?.ToString(), out selectValue);

                switch (clickBtn.Name)
                {
                    case "tb_Ok":
                        base.SetValue(DateTimePicker.SelectDateTimeProperty, _dtEmt.ToDateTime());
                        _dateSelector.IsOpen = false;
                        ExecuteCommand();
                        break;
                    case "DayItem":
                        _dtEmt.SetDay(selectValue);
                        break;
                    case "YearItem":
                        InYearCallback(selectValue);
                        break;
                    case "btn_Before":
                        BeforePage();
                        break;
                    case "btn_After":
                        AfterPage();
                        break;
                    case "TimeItem":
                        _curInputTime.Text = clickBtn.Content.ToString();
                        if (_curInputTime.Tag.ToString() == "H")
                            _dtEmt.SetTime(selectValue, null, null);
                        else if (_curInputTime.Tag.ToString() == "M")
                            _dtEmt.SetTime(null, selectValue, null);
                        else
                            _dtEmt.SetTime(null, null, selectValue);
                        break;
                    case "tb_Today":
                        Reset(true);
                        break;
                }
            }
        }

        //进入当前的月份或年
        private void _btnDateTitle_Click(object sender, RoutedEventArgs e)
        {
            if (Enum.IsDefined(typeof(DateTimePickerStatus), CurStatus + 1))
            {
                CurStatus = CurStatus + 1;
                if (CurStatus == DateTimePickerStatus.Month)
                {
                    RefreshDateTime(DateTimePickerStatus.Month, _dtEmt);
                    _onDayToMonth.Begin();
                }
                else if (CurStatus == DateTimePickerStatus.Year)
                {
                    RefreshDateTime(DateTimePickerStatus.Year, _dtEmt);
                    _onMonthToYear.Begin();
                }
            }
        }

        #region Popup控制

        private void _dateSelectorStatus_Checked(object sender, RoutedEventArgs e)
        {
            _dateSelector.IsOpen = true;
            _dateSelectorStatus.IsEnabled = false;
        }

        private void _dateSelector_Closed(object sender, EventArgs e)
        {
            _dateSelectorStatus.IsChecked = false;
            _dateSelectorStatus.IsEnabled = true;
            Reset();
        }

        #endregion

        #region 年月进入

        //进入月份
        private void InMonthCallback(object sender, RoutedEventArgs e)
        {
            CurStatus = CurStatus - 1;
            var selectMonth = e.OriginalSource as RadioButton;
            _dtEmt.SetMonth(int.Parse(selectMonth.Tag.ToString()));
            RefreshDateTime(DateTimePickerStatus.Day, _dtEmt);
            _onMonthToDay.Begin();
        }

        //进入年
        private void InYearCallback(int year)
        {
            CurStatus = CurStatus - 1;
            _dtEmt.SetYear(year);
            RefreshDateTime(DateTimePickerStatus.Month, _dtEmt);
            _onYearToMonth.Begin();
        }

        #endregion

        #region 翻页

        /// <summary>
        /// 后一页
        /// </summary>
        private void AfterPage()
        {
            _onAfter.Begin();
            PageTurning(true);
        }

        /// <summary>
        /// 前一页
        /// </summary>
        private void BeforePage()
        {
            _onBefore.Begin();
            PageTurning(false);
        }

        #endregion

        #region 时间输入效果展示【加载】

        private void Second_GotFocus(object sender, RoutedEventArgs e)
        {
            //加载秒
            for (int i = 0; i < 24; i++)
            {
                if (i > 11)
                    _wpTime.Children[i].SetValue(Button.ContentProperty, null);
                else
                    _wpTime.Children[i].SetValue(Button.ContentProperty, i * 5); ;
            }
            _curInputTime = sender as TextBox;
        }

        private void Minute_GotFocus(object sender, RoutedEventArgs e)
        {
            //加载分钟
            for (int i = 0; i < 24; i++)
            {
                if (i > 11)
                    _wpTime.Children[i].SetValue(Button.ContentProperty, null);
                else
                {
                    _wpTime.Children[i].SetValue(Button.ContentProperty, i * 5);
                }
            }
            _curInputTime = sender as TextBox;
        }

        private void Hour_GotFocus(object sender, RoutedEventArgs e)
        {
            //加载小时
            for (int i = 0; i < 24; i++)
            {
                _wpTime.Children[i].SetValue(Button.ContentProperty, i);
            }
            _curInputTime = sender as TextBox;
        }

        #endregion

        #region Tools

        /// <summary>
        /// 重置当前控件
        /// </summary>
        /// <param name="resetToToday">是否重置为当前时间</param>
        private void Reset(bool resetToToday = false)
        {
            //还原状态
            _onReset.Begin();
            if (resetToToday)
                _dtEmt.SetDateTime(DateTime.Now);
            CurStatus = DateTimePickerStatus.Day;
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        private void ExecuteCommand()
        {
            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }
        }

        #region 刷新界面

        /// <summary>
        /// 刷新时间
        /// </summary>
        /// <param name="status">当前显示状态</param>
        /// <param name="datatime">当前时间</param>
        /// <param name="isInYear">是否刷新为范围年【只针对年】</param>
        private void RefreshDateTime(DateTimePickerStatus status, DateTimeElement datatime)
        {
            //更新日期显示【_btnDateTitle】
            if (status == DateTimePickerStatus.Day)
            {
                RefreshDayItems(datatime.Year, datatime.Month, datatime.Day);
            }
            else if (status == DateTimePickerStatus.Month)
            {
                _btnDateTitle.Content = datatime.Year;
                RefreshMonthItems(datatime.Month);
            }
            else
            {
                RefreshYearItems(datatime.Year);
            }
        }

        #endregion

        #region 屏蔽用户输入非数字

        private void Hour_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbTmp = sender as TextBox;
            var change = e.Changes.Last();
            int inputTime;
            if (tbTmp.Text.Length > 2)
                tbTmp.Text = tbTmp.Text.Substring(0, 2);
            else if (!int.TryParse(tbTmp.Text, out inputTime))
            {
                tbTmp.Text = tbTmp.Text.Substring(0, change.Offset);
            }
            else
            {
                //过去时间，小时不能大于23，分钟、秒不能大于60
                var maxTime = tbTmp.Tag.ToString() == "H" ? 23 : 59;
                tbTmp.Text = inputTime > maxTime ? maxTime.ToString() : inputTime.ToString();
            }
            tbTmp.CaretIndex = change.Offset + 1;
        }

        #endregion

        #region 翻页

        /// <summary>
        /// 翻页
        /// </summary>
        /// <param name="after">是否为下一页</param>
        private void PageTurning(bool after)
        {
            int tmp = after ? 1 : -1;
            if (CurStatus == DateTimePickerStatus.Day)
            {
                _dtEmt.AddMonths(tmp);
                RefreshDateTime(DateTimePickerStatus.Day, _dtEmt);
            }
            else if (CurStatus == DateTimePickerStatus.Month)
            {
                _dtEmt.AddYears(tmp);
                RefreshDateTime(DateTimePickerStatus.Month, _dtEmt);
            }
            else
            {
                //由于一页年数据会显示12个，所以翻页采用收尾取
                var yearTmp = after ? _yearContainer.Children[_yearContainer.Children.Count - 1].GetValue(RadioButton.ContentProperty) :
                    _yearContainer.Children[0].GetValue(RadioButton.ContentProperty);
                //更新年【范围】
                _dtEmt.SetYear(int.Parse(yearTmp.ToString()));
                RefreshDateTime(DateTimePickerStatus.Year, _dtEmt);
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化时间容器
        /// </summary>
        private void InitTimeContainer()
        {
            for (int i = 0; i < 24; i++)
            {
                Button rbTime = new Button();
                rbTime.Name = "TimeItem";
                _wpTime.Children.Add(rbTime);
            }
        }

        /// <summary>
        /// 初始化年容器【12个】
        /// </summary>
        /// <param name="years"></param>
        private void InitYearContainer(List<int> years)
        {
            foreach (var item in years)
            {
                RadioButton bTmp = new RadioButton();
                bTmp.Name = "YearItem";
                bTmp.Content = item;
                _yearContainer.Children.Add(bTmp);
            }
        }

        /// <summary>
        /// 初始化天数容器
        /// </summary>
        /// <param name="days"></param>
        private void InitDayContainer(List<int?> days)
        {
            int dayIndex = 0;
            for (int i = 0; i < 6; i++)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                for (int j = 0; j < 7; j++)
                {
                    RadioButton rb = new RadioButton();
                    rb.Name = "DayItem";
                    rb.Content = days[dayIndex];
                    sp.Children.Add(rb);
                    dayIndex++;
                    //方便更新
                    _dayItems.Add(rb);
                }
                _dayContainer.Children.Add(sp);
            }
        }

        #endregion

        #region 日期操作

        /// <summary>
        /// 刷新月份
        /// </summary>
        /// <param name="selectedMonth"></param>
        private void RefreshMonthItems(int selectedMonth)
        {
            for (int i = 1; i <= 12; i++)
            {
                _monthContainer.Children[i - 1].SetValue(RadioButton.IsCheckedProperty, i == selectedMonth);
            }
        }

        /// <summary>
        /// 刷新日期项
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        private void RefreshDayItems(int year, int month, int selectedDay)
        {
            var a = LoadDateItems(year, month);
            for (int i = 0; i < a.Count; i++)
            {
                _dayItems[i].IsChecked = a[i].HasValue && a[i].Value == selectedDay;
                _dayItems[i].Content = a[i];
            }
            _btnDateTitle.Content = string.Format("{0}-{1}", _dtEmt.Year, _dtEmt.Month);
        }

        /// <summary>
        /// 刷新时间
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        private void RefreshTime(int hour, int minute, int second)
        {
            var tbHour = this.Template.FindName("Hour", this) as TextBox;
            var tbMinute = this.Template.FindName("Minute", this) as TextBox;
            var tbSecond = this.Template.FindName("Second", this) as TextBox;
            if (tbHour != null && tbMinute != null && tbSecond != null)
            {
                tbHour.Text = hour.ToString();
                tbMinute.Text = minute.ToString();
                tbSecond.Text = second.ToString();
            }
        }

        /// <summary>
        /// 刷新年【范围】
        /// </summary>
        /// <param name="year"></param>
        private void RefreshYearItems(int year)
        {
            var a = LoadYearItems(year);
            for (int i = 0; i < a.Count; i++)
            {
                _yearContainer.Children[i].SetValue(RadioButton.IsCheckedProperty, a[i] == year);
                _yearContainer.Children[i].SetValue(RadioButton.ContentProperty, a[i]);
            }
            _btnDateTitle.Content = string.Format("{0}-{1}", a[0], a[a.Count - 1]);
        }

        #region 数据加载操作

        /// <summary>
        /// 根据年月加载日期项
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        private List<int?> LoadDateItems(int year, int month)
        {
            List<int?> dayResult = new List<int?>();
            DateTime curDate = new DateTime(year, month, 1);
            //跳过日期
            for (int i = 0; i < (int)curDate.DayOfWeek; i++)
            {
                dayResult.Add(null);
            }
            var days = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= days; i++)
            {
                dayResult.Add(i);
            }
            //凑够42
            int tmp = 42 - dayResult.Count;
            for (int i = 0; i < tmp; i++)
            {
                dayResult.Add(null);
            }
            return dayResult;
        }

        /// <summary>
        /// 加载年数据
        /// </summary>
        /// <param name="curYear"></param>
        /// <returns></returns>
        private List<int> LoadYearItems(int curYear)
        {
            List<int> yearResult = new List<int>();
            if (curYear < 5)
            {
                int intdexTmp = 0;
                while (yearResult.Count < 12)
                {
                    yearResult.Add(++intdexTmp);
                }
            }
            else
            {
                yearResult.Add(curYear);
                yearResult.Add(curYear + 1);
                for (int i = 1; i <= 5; i++)
                {
                    yearResult.Insert(0, curYear - i);
                    yearResult.Add(curYear + i + 1);
                }
            }
            return yearResult;
        }

        #endregion

        #endregion

        #region 重置时间

        /// <summary>
        /// 重置时间
        /// </summary>
        /// <param name="datetime"></param>
        private void ResetDateTime(DateTime datetime)
        {
            _dtEmt.SetDateTime(datetime);
            RefreshDateTime(DateTimePickerStatus.Day, _dtEmt);
            RefreshTime(_dtEmt.Hour, _dtEmt.Minute, _dtEmt.Second);
        }

        #endregion

        #endregion

        #region 语言

        public bool IsChinese
        {
            get { return (bool)GetValue(IsChineseProperty); }
            set { SetValue(IsChineseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChinese.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsChineseProperty =
            DependencyProperty.Register("IsChinese", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(true));

        #endregion

        #region 当前日期

        public DateTime SelectDateTime
        {
            get { return (DateTime)GetValue(SelectDateTimeProperty); }
            set { SetValue(SelectDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectDateTimeProperty =
            DependencyProperty.Register("SelectDateTime", typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SelectDateTimeCallback)));

        private static void SelectDateTimeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = d as DateTimePicker;
            var newDate = (DateTime)e.NewValue;
            //newDate = newDate != default(DateTime) ? newDate : DateTime.Now;
            timePicker.ResetDateTime(newDate);
        }

        #endregion

        #region 时间输入Command

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DateTimePicker), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(DateTimePicker), new PropertyMetadata(null));

        #endregion
    }
}
