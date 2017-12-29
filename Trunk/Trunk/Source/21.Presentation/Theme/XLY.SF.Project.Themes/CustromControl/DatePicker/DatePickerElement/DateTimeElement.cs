using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Themes.CustromControl
{
    internal class DateTimeElement
    {
        public DateTimeElement()
        {
            var dateTmp = DateTime.Now;

            Year = dateTmp.Year;
            Month = dateTmp.Month;
            Day = dateTmp.Day;
            Hour = dateTmp.Hour;
            Minute = dateTmp.Minute;
            Second = dateTmp.Second;
        }

        public int Year { get; private set; }

        public int Month { get; private set; }

        public int Day { get; private set; }

        public int Hour { get; private set; }

        public int Minute { get; private set; }

        public int Second { get; private set; }

        /// <summary>
        /// 获取对应的时间
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Second);
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public void SetTime(int? hour, int? minute, int? second)
        {
            Hour = hour.HasValue ? hour.Value : Hour;
            Minute = minute.HasValue ? minute.Value : Minute;
            Second = second.HasValue ? second.Value : Second;
        }

        /// <summary>
        /// 设置日期
        /// </summary>
        /// <param name="datetime"></param>
        public void SetDateTime(DateTime datetime)
        {
            Year = datetime.Year;
            Month = datetime.Month;
            Day = datetime.Day;
            Hour = datetime.Hour;
            Minute = datetime.Minute;
            Second = datetime.Second;
        }

        /// <summary>
        /// 设置日期
        /// </summary>
        /// <param name="day"></param>
        public void SetDay(int day)
        {
            Day = day;
        }

        /// <summary>
        /// 设置年
        /// </summary>
        public void SetYear(int year)
        {
            Year = year;
        }

        /// <summary>
        /// 添加年
        /// </summary>
        /// <param name="year"></param>
        public void AddYears(int year)
        {
            Year += year;
        }

        /// <summary>
        /// 添加月
        /// </summary>
        /// <param name="month"></param>
        public void AddMonths(int month)
        {
            var tmpValue = Month + month;
            if (month >= 0)
            {
                var yearTmp = tmpValue / 13;
                var monthTmp = tmpValue % 13 == 0 ? 1 : tmpValue % 13;
                Year += yearTmp;
                Month = monthTmp;
            }
            else
            {
                var yearTmp = tmpValue <= 0 ? 1 : 0;
                Year -= yearTmp;
                Month = tmpValue <= 0 ? 12 : tmpValue;
            }
        }

        /// <summary>
        /// 设置月份
        /// </summary>
        /// <param name="month"></param>
        public void SetMonth(int month)
        {
            Month = month;
        }
    }
}
