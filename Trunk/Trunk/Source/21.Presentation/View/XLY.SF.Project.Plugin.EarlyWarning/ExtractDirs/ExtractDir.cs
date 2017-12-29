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
    class ExtractDir
    {
        /// <summary>
        /// DataSources
        /// </summary>
        public List<DeviceDataSource> DataSources { get { return _dataSources; } }
        List<DeviceDataSource> _dataSources = new List<DeviceDataSource>();

        /// <summary>
        /// Result目录
        /// </summary>
        public string ResultDir { get; private set; }

        /// <summary>
        /// 目录
        /// </summary>
        public string Dir { get; private set; }

        /// <summary>
        /// Db文件路径
        /// </summary>
        public string DbFile { get; private set; }

        /// <summary>
        /// 是否初始化成功
        /// </summary>
        public bool Initialized { get; private set; }    

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(string dir)
        {
            Initialized = false;
            //修整目录
            if (!dir.EndsWith("\\"))
            {
                dir += "\\";
            }
            string resultDir = dir  + "Result";

            if (!Directory.Exists(resultDir))
            {
                return ;
            }
            ResultDir = resultDir;

            string dbFile = dir + "data.db";
            if (!File.Exists(dbFile))
            {
                return ;
            }
            DbFile = dbFile;
            Dir = dir;

            Initialized = true;
        }
    
        /// <summary>
        /// 装载
        /// </summary>
        /// <param name="dir"></param>
        public void LoadDataSource()
        {
            if(!Initialized)
            {
                return;
            }

            DataSources.Clear();
            //获取此目录下所有的ds文件
            foreach (var dsFile in Directory.GetFiles(ResultDir, "*.ds"))        //ds为IDataSource二进制序列化包
            {
                IDataSource ds = Serializer.DeSerializeFromBinary<IDataSource>(dsFile);                
                ds.SetCurrentPath(Dir);
                if(string.IsNullOrWhiteSpace(DbFile))
                {
                    DbFile=ds.Items.DbFilePath;
                }
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
