using System;
using System.Globalization;
using System.Windows.Data;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.DataExtraction
{
    public class TaskStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskState state = (TaskState)value;
            switch (state)
            {
                case TaskState.Running:
                    return "正在提取...";
                case TaskState.Starting:
                    return "开始提取...";
                case TaskState.Stopping:
                    return "正在停止...";
                case TaskState.Stopped:
                    return "停止解析";
                case TaskState.Completed:
                    return "提取完成";
                case TaskState.Failed:
                    return "提取出错";
                case TaskState.Idle:
                default:
                    return "未提取";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
