using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using XLY.SF.Project.ViewDomain.VModel.DevHomePage;

namespace XLY.SF.Project.Views.Converters
{
    /// <summary>
    /// 设备首页编辑项转换器
    /// </summary>
    public class DevHomePageEditItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DevHomePageEditItemModel editResult = null;
            if (values != null && values.Length == 11)
            {
                editResult = new DevHomePageEditItemModel();
                bool isSave;
                if (!bool.TryParse(values[10].ToString(), out isSave) || !isSave)
                {
                    editResult = null;
                }
                else
                {
                    editResult.No = values[0].ToString();
                    editResult.Holder = values[1].ToString();
                    editResult.CredentialsType = values[2]?.ToString();
                    editResult.HolderCredentialsNo = values[3].ToString();
                    editResult.CensorshipPerson = values[4].ToString();
                    editResult.UnitName = values[5].ToString();
                    editResult.CensorshipPersonCredentialsNo = values[6].ToString();
                    editResult.Operator = values[7].ToString();
                    editResult.CredentialsNo = values[8].ToString();
                    editResult.Desciption = values[9].ToString();
                }
            }
            return editResult;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
