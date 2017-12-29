using Newtonsoft.Json;
using System;
using System.IO;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据泵/提取对象
    /// </summary>
    public class Pump
    {
        #region Fields

        /// <summary>
        /// 源文件目录名称。
        /// </summary>
        public const String SourceDirectory = "Source";

        /// <summary>
        /// 提取结果目录名称。
        /// </summary>
        public const String ResultDirectory = "Result";

        #endregion

        #region Constructors

        public Pump(String savePath, String dbFileName)
            :this(savePath,dbFileName,Guid.NewGuid().ToString())
        {
        }

        [JsonConstructor]
        public Pump(String savePath, String dbFileName, String id)
        {
            SavePath = savePath ?? throw new ArgumentNullException("savePath");
            DbFileName = dbFileName ?? throw new ArgumentNullException("dbFileName");
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 唯一标识。
        /// </summary>
        public String Id { get; }

        /// <summary>
        /// 操作系统类型。
        /// </summary>
        public EnumOSType OSType { get; set; }

        /// <summary>
        /// 提取数据的源设备,根据提取方式的不同而不同
        /// 可能是手机(USB提取)、镜像文件（镜像提取）、SD卡（SD卡提取）、本地文件夹路径（文件夹提取）等等
        /// </summary>
        public Object Source { get; set; }

        /// <summary>
        /// 保存路径。
        /// </summary>
        public String SavePath { get;}
        
        /// <summary>
        /// 源文件存储路径。
        /// </summary>
        public String SourceStorePath => Path.Combine(SavePath, SourceDirectory);

        /// <summary>
        /// 数据库文件名。
        /// </summary>
        public String DbFileName { get; }

        /// <summary>
        /// 数据库文件路径。
        /// </summary>
        public String DbFilePath => Path.Combine(SavePath, DbFileName);

        /// <summary>
        /// 提取结果路径。
        /// </summary>
        public String ResultPath=> Path.Combine(SavePath, ResultDirectory);

        /// <summary>
        /// 提取方式
        /// </summary>
        public EnumPump Type { get; set; }

        /// <summary>
        /// 扫描模式(镜像文件、SDCard)
        /// </summary>
        public ScanFileModel ScanModel { get; set; } = ScanFileModel.Quick;

        /// <summary>
        /// 提取方案。
        /// </summary>
        public PumpSolution Solution { get; set; } = PumpSolution.TempRoot | PumpSolution.AppInjection;

        #endregion
    }
}