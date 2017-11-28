/* ==============================================================================
* Description：ExtactionItemParser  
* Author     ：litao
* Create Date：2017/11/24 13:40:28
* ==============================================================================*/


using System.Collections.Generic;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class DeviceDataParser
    {
        public List<DeviceDataSource> DataSources { get { return _dataSources; } }
        List<DeviceDataSource> _dataSources = new List<DeviceDataSource>();

        public void LoadDeviceData()
        {
            string dir = @"C:\Users\litao\Desktop\迭代66\123_20171123[021724]\R7_20171123[021726]";
            DataSources.Clear();
            if (!Directory.Exists(dir))
            {
                return;
            }
            foreach (var subDir in Directory.GetDirectories(dir))
            {
                LoadDsFile(subDir);
            }
        }

        /// <summary>
        /// 加载DsFile
        /// </summary>
        private void LoadDsFile(string dir)
        {
            string resultDir = dir + "\\" + "Result";
            if (!Directory.Exists(resultDir))
            {
                return;
            }

            foreach (var dsFile in Directory.GetFiles(resultDir, "*.ds"))        //ds为IDataSource二进制序列化包
            {
                IDataSource ds = Serializer.DeSerializeFromBinary<IDataSource>(dsFile);
                ds.SetCurrentPath(dir);
                DataSources.Add(new DeviceDataSource(dsFile, ds));
            }
        }
    }

    public class DeviceDataSource
    {
        public DeviceDataSource(string dsFile, IDataSource ds)
        {
            this.DsFilePath = dsFile;
            this.DataSource = ds;
        }

        public IDataSource DataSource { get; private set; }
        public string DsFilePath { get; private set; }
    }
}
