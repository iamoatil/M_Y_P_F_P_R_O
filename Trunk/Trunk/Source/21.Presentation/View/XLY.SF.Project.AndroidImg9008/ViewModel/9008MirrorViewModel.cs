using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.AndroidImg9008
{
    [Export(ExportKeys.Mirror9008ViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class _9008MirrorViewModel : ViewModelBase
    {
        public _9008MirrorViewModel()
        {
            string pmbnfileDir = AppDomain.CurrentDomain.BaseDirectory + "res9008";
            MsgDatas = new ObservableCollection<string>();
            Ob_ComDev = new ObservableCollection<string>();
            AsyncOperation = AsyncOperationManager.CreateOperation(this);

            DeviceDetectionCommand = new RelayCommand(ExecuteDeviceDetectionCommand);
            InstallCommand = new RelayCommand(ExecuteInstallCommand);
            AdbIntoCommand = new RelayCommand(ExecuteAdbIntoCommand);
            GeneralCommand = new RelayCommand(ExecuteGeneralCommand);
            LineCommand = new RelayCommand(ExecuteLineCommand);
            SelectedPathCommand = new RelayCommand(ExecuteSelectedPathCommand);
            BeginCommand = new RelayCommand(ExecuteBeginCommand);

            // 镜像回掉
            this._ImageDataCallBack = ImageDataAction;

            _timer = new System.Timers.Timer(Interval.TotalMilliseconds);
            _timer.Elapsed += _timer_Elapsed;
        }
        /// <summary>
        /// 底层数据回调
        /// </summary>
        /// <param name="data">每次返回数据</param>
        /// <param name="datasize">每次返回字节数</param>
        /// <param name="stop">是否停止，0-继续，1-停止</param>
        /// <returns></returns>
        private int ImageDataAction(IntPtr data, int datasize, ref int stop)
        {
            try
            {
                var buff = new byte[datasize];
                Marshal.Copy(data, buff, 0, datasize);
                // 数据写入
                this.MirrorStream.Write(buff, 0, datasize);

                //界面显示完成大小
                var comsize = FileHelper.GetFileSize((long)this.ProgressValue);
                //界面显示所需时间
                //var totalSeconds = Timer9008.GetElapsed().TotalSeconds;
                //// 执行时间
                //var time = new DateTime(Timer9008.GetElapsed().Ticks).ToString("HH:mm:ss");
                //界面显示镜像速度
                //var speed = FileHelper.GetFileSize((long)((this.ProgressValue + datasize) / totalSeconds));
                var percent = (this.ProgressValue + datasize) * 100 / this.ProgressMaxValue;
                isImgSuccess = percent >= 100;
                //this.ProgressMessage = LanguageHelper.GetFormat("LANGKEY_9008_ImgJinDuMiaoShu_05178", ImgSizeDesc, comsize, percent, time, speed);
                if (IsStop)
                {
                    stop = 1;
                    if (this.MirrorStream != null)
                    {
                        this.MirrorStream.Flush();
                        this.MirrorStream.Close();
                        this.MirrorStream = null;
                    }
                  //  this.ProgressMessage = LanguageHelper.Get("LANGKEY_9008_YiZanTingShuJuJingXiang_05178");
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                this.ProgressValue += datasize;//进度条完成量
            }

            return 0;
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TotalElapsed += Interval;
        }
        private void ExecuteBeginCommand()
        {
            //if (null == SelectedBrand || string.IsNullOrEmpty(SelectedBrand.PhoneNo))
            //{
            //    ProgressMessage = LanguageHelper.Get("LANGKEY_9008_SelectPhoneNo_05178");
            //    ImgIsEnabled = true;
            //    return;
            //}
            //if (!SavePath.IsValid())
            //{
            //    ProgressMessage = LanguageHelper.Get("LANGKEY_9008_SavePath_05178");
            //    ImgIsEnabled = true;
            //    return;
            //}
            //if (string.IsNullOrEmpty(SelectedCom))
            //{
            //    ProgressMessage = LanguageHelper.Get("LANGKEY_9008_QingXuanZeCOMKou_05178");
            //    ImgIsEnabled = true;
            //    return;
            //}

            //StringBuilder sbMsg = new StringBuilder();
            bool result = false;
            Task<bool>.Factory.StartNew((t) =>
            {
                try
                {
                    // 1，初始化
                    result = ImgMount(this.SelectedBrand.PhoneNo, this.SelectedCom, this.SavePath);
                    if (!result)
                    {
                        return false;
                    }

                    // 2，数据镜像、停止镜像
                    result = ImgDataZone();
                    if (!result)
                    {
                        return false;
                    }

                    // 3，生成校验码
                    if (this.IsStop == false)
                    {
                        result = Createmd5File(this.MirrorLocal);
                        if (!result)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex, Languagekeys.ANGKEY_9008_ShuJuJingXiangYiChang_05178);
                    return false;
                }
            }, null).ContinueWith((t) =>
            {
                AsyncOperation.Post((d) =>
                {
                    bool returnValue = false;
                    bool.TryParse(t.Result.ToString(), out returnValue);
                    if (returnValue)
                    {
                        if (IsStop == false)
                        {
                            //string verifyCode = Md5Helper.GetFileMd5Code(this.MirrorName);
                            Init(this.MirrorName, this.VerifyCode, true);
                            this.CloseHandle();
                            this.ProgressValue += 1;
                        }
                        else
                        {
                            this.ImgIsEnabled = true;
                        }
                    }
                    else
                    {
                        Init();
                        this.CloseHandle();
                        this.ProgressValue = this.ProgressMaxValue;
                    }
                    //打开文件位置
                    if (t.Result && isImgSuccess)
                    {
                        try
                        {
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                            psi.Arguments = "/e,/select," + this.MirrorLocal;
                            System.Diagnostics.Process.Start(psi);
                        }
                        catch (Exception ex)
                        {
                            LoggerManagerSingle.Instance.Error(ex, Languagekeys.ANGKEY_ChaKanJingXiangMuBiaoShiChuCuo_05202);
                        }
                    }

                }, null);
            });
        }
        ///<summary>
        /// 数据镜像
        /// </summary>
        private bool ImgDataZone()
        {
            if (!this.MirrorLocal.IsValid())
            {
                return false;
            }

            try
            {
                if (!System.IO.File.Exists(this.MirrorLocal))
                {
                    FileHelper.CreateFile(this.MirrorLocal);
                }

                // 开始扇区数（默认0）：镜像文件字节总数/512
                this.MirrorStream = new FileStream(this.MirrorLocal, FileMode.Append, FileAccess.Write);
                Int64 start = this.MirrorStream.Length / 512;
                // 镜像总数（默认-1）：扇区长度-开始扇区数
                Int64 count = this.Sectorlen - start;
                var result = FileServiceCoreDll.Android_9008_Img_ImageDataZone(this.Handle9008, start, count, _ImageDataCallBack);
                if (result != 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.MirrorStream != null)
                {
                    this.MirrorStream.Flush();
                    this.MirrorStream.Close();
                }
            }
            return true;
        }
        /// <summary>
        /// 生成校验码
        /// </summary>
        public bool Createmd5File(string local)
        {
            try
            {
                if (!System.IO.File.Exists(local))
                {
                    return false;
                }

                this.VerifyCode = FileHelper.MD5FromFile(local);

                //设备镜像，写入MD5值
                string md5File = local.Replace("bin", "md5");
                FileHelper.DeleteFile(md5File);
                FileHelper.CreateFile(md5File, this.VerifyCode);
                return true;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex,Languagekeys.ANGKEY_ShengChengXiaoYanMaYiChang_05201);
                return false;
            }
        }
        /// <summary>
        /// 卸载
        /// </summary>
        public void CloseHandle()
        {
            try
            {
                if (this.Handle9008 != IntPtr.Zero)
                {
                    int result = FileServiceCoreDll.Android_9008_Img_UNMount(ref this.Handle9008);
                    this.Handle9008 = IntPtr.Zero;
                }
                if (!string.IsNullOrWhiteSpace(this.Temp_PmbnfileDir))
                {
                    FileHelper.DeleteDirectory(this.Temp_PmbnfileDir);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="phoneNo">手机型号</param>
        /// <param name="comName">COM口</param>
        /// <returns>初始化是否成功，true-成功，反之亦然</returns>
        private bool ImgMount(string phoneNo, string comName, string imgPath)
        {
            // 判断是否已初始化
            if (this.Handle9008 != IntPtr.Zero)
            {
                return true;
            }
            try
            {
                // 服务启动失败：是否进行过重试
                int doretry = 0;
                DoRetry:

                string pmbnfileDir = AppDomain.CurrentDomain.BaseDirectory + @"Lib\vcdllX64\Android_Img9008\res9008";
                string pQSaharaServerfilepath = AppDomain.CurrentDomain.BaseDirectory + @"Lib\vcdllX64\Android_Img9008\QCUSBXLY.exe";
                this.Temp_PmbnfileDir = string.Empty;
                if (doretry == 0)
                {
                    pmbnfileDir = AppDomain.CurrentDomain.BaseDirectory + @"Lib\vcdllX64\Android_Img9008\res9008";
                    pQSaharaServerfilepath = AppDomain.CurrentDomain.BaseDirectory + @"Lib\vcdllX64\Android_Img9008\QCUSBXLY.exe";
                }
                else
                {
                    // 镜像目录
                    FileHelper.CreateDirectory(this.SavePath);
                    // 资源目录
                    string temp_pmbnfileDir = System.IO.Path.Combine(this.SavePath, "res9008");
                    // 资源exe
                    string temp_pQSaharaServerfilepath = System.IO.Path.Combine(temp_pmbnfileDir, "QCUSBXLY.exe");

                    // 拷贝资源目录及exe到镜像目录
                    FileHelper.CopyDirectory(pmbnfileDir, temp_pmbnfileDir);

                    //【Bob】
                    System.IO.File.Copy(pQSaharaServerfilepath, temp_pQSaharaServerfilepath, true);

                    this.Temp_PmbnfileDir = pmbnfileDir = temp_pmbnfileDir;
                    pQSaharaServerfilepath = temp_pQSaharaServerfilepath;
                }


                Handle9008 = IntPtr.Zero;
                var res = FileServiceCoreDll.Android_9008_Img_Mount(comName, phoneNo, pmbnfileDir, pQSaharaServerfilepath, ref Handle9008);
                if (Handle9008 == IntPtr.Zero)
                {

                    if (doretry == 0)
                    {
                        doretry = 1;
                        goto DoRetry;
                    }
                    return false;
                }

                // 扇区数获取
                //20170525 songbing 修改
                //部分手机第一次获取长度会失败，第二次就成功
                //修改成最多连续获取3次，直到成功
                int result = -1;
                int retry = 3;
                while (0 != result)
                {
                    if (retry <= 0)
                    {
                        break;
                    }
                    retry--;

                    Thread.Sleep(100);

                    result = FileServiceCoreDll.Android_9008_Img_TetdiskSectors(this.Handle9008, ref Sectorlen);
                }

                if (result != 0)
                {
                    return false;
                }

                var len_kb = Sectorlen / 2 * 1024L;
                this.ImgSizeDesc = FileHelper.GetFileSize(len_kb, "F2");

                //进度条进度
                this.ProgressMaxValue = len_kb + 1;
                this.ProgressValue = 0;

                // 镜像路径
                FileHelper.CreateDirectory(this.SavePath);
                this.MirrorName = string.Format("{0}_{1}.bin", phoneNo, DateTime.Now.ToString("yyyyMMddHHmmss"));
                this.MirrorLocal = System.IO.Path.Combine(imgPath, MirrorName);
                this.VerifyCode = string.Empty;

                // 开始计时
                _timer.Start();
                TotalElapsed = TimeSpan.Zero;
                return true;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex,Languagekeys.ANGKEY_9008_InitException_05178);
                return false;
            }
        }
        [Import(typeof(IPopupWindowService))]
        private IPopupWindowService PopupService { get; set; }
        private void ExecuteSelectedPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            SavePath = directory;
        }
        //工程线
        private void ExecuteLineCommand()
        {
            MsgDatas.Clear();
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message21);
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message22);
        }
        //普通方案
        private void ExecuteGeneralCommand()
        {
            MsgDatas.Clear();
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message23);
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message24);
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message25);
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message26);
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message27);
            MsgDatas.Add(Languagekeys.ANGKEY_9008_Message28);
        }
        //ADB
        private void ExecuteAdbIntoCommand()
        {
            MsgDatas.Clear();
            if (this.Handle9008 != IntPtr.Zero)
            {
                MsgDatas.Add(Languagekeys.ANGKEY_9008_ZhengZaiShuJuJingXiangQingNaiX_05178);
                return;
            }

            //if (null == SelectedBrand || string.IsNullOrEmpty(SelectedBrand.PhoneNo))
            //{
            //    MsgDatas.Add(LanguageHelper.Get("LANGKEY_9008_SelectPhoneNo_05178"));
            //    return;
            //}

            //命令1：切换目录
            string cmd_1 = "cd " + AppDomain.CurrentDomain.BaseDirectory + "adb";
            // 命令2：提取文件命令
            string cmd_2 = this.Reboot_l;
            // 命令拼接：用这种方法可以同时执行多条命令，而不管命令是否执行成功
            string cmd = string.Format("{0}&{1}", cmd_1, cmd_2);
            
            string[] strTemps =ExeCommand(cmd, 2000);
            if (!strTemps.IsValid() || strTemps[0] != "0")
            {
                MsgDatas.Add(Languagekeys.ANGKEY_9008_QingQueRenShouJiKaiJi_05178);
               // LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_9008_ZhiLingCuoWuCuoWuXinXi_05178"), strTemps[1]));
                return;
            }
            else
            {
                LoadComs();
               // Init();

                MsgDatas.Add(Ob_ComDev.Count > 0 ?
                   Languagekeys.ANGKEY_9008_ChengGongJinRu9008Mode_05178 :
                    Languagekeys.ANGKEY_9008_Message20);
            }
        }
        //安装驱动
        private void ExecuteInstallCommand()
        {
            string pathTmp = Path.Combine(Environment.CurrentDirectory, @"高通HS-USB驱动\Setup.exe");
            if (System.IO.File.Exists(pathTmp))
            {
                //安装驱动
                Process.Start(pathTmp);
            }
        }
        //设备检测
        private void ExecuteDeviceDetectionCommand()
        {
            LoadComs();
            MsgDatas.Add(Ob_ComDev.Count > 0 ? Languagekeys.ANGKEY_9008_Message19 : Languagekeys.ANGKEY_9008_Message20);
        }
        /// <summary>
        /// 执行DOS命令，返回DOS命令的输出
        /// dos命令
        /// 等待命令执行的时间（单位：毫秒），如果设定为0，则无限等待
        /// 返回输出，如果发生异常，返回空字符串
        /// </summary>
        /// <param name="dosCommand">命令文本</param>
        /// <param name="milliseconds">等待关联进程退出的时间（以毫秒为单位）</param>
        /// <returns></returns>
        public static string[] ExeCommand(string dosCommand, int milliseconds = 0)
        {
            string[] returnValues = new string[2] { "0", "OK" };
            if (!dosCommand.IsValid())
            {
                returnValues[0] = "-1";
                returnValues[1] = "命令文本为空";
                return returnValues;
            }

            using (Process process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";                     //设定需要执行的命令
                startInfo.Arguments = "/C " + dosCommand;           //设定参数，其中的“/C”表示执行完命令后马上退出
                startInfo.UseShellExecute = false;                  //不使用系统外壳程序启动
                startInfo.RedirectStandardInput = false;            //不重定向输入
                startInfo.RedirectStandardOutput = true;            //重定向输出
                startInfo.CreateNoWindow = true;                    //不创建窗口
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())                            //开始进程
                    {
                        if (milliseconds == 0)
                        {
                            process.WaitForExit();                  //这里无限等待进程结束
                        }
                        else
                        {
                            process.WaitForExit(milliseconds);      //这里等待进程结束，等待时间为指定的毫秒
                        }
                        returnValues[1] = process.StandardOutput.ReadToEnd();//读取进程的输出
                    }
                }
                catch (Exception e)
                {
                    returnValues[0] = "-1";
                    returnValues[1] = "执行DOS命令，返回DOS命令的输出异常：" + e.Message;

                    LoggerManagerSingle.Instance.Error(e, "执行DOS命令，返回DOS命令的输出异常");
                }
            }
            return returnValues;
        }
        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="isSuccess">true-成功初始化，false-失败或初始化(默认)</param>
        private void Init(string mirrorName = null, string mirrorMD5 = null, bool isSuccess = false)
        {

            if (!isSuccess)
            {
                this.ProgressMaxValue = 4;
                this.ProgressValue = 0;

            }
            this.IsStop = true;
            this._timer.Stop();
        }
        private bool isImgSuccess = false;
        /// <summary>
        /// 正则匹配形如"(COM6)"
        /// </summary>
        private Regex _rgCName = new Regex(@"\(COM\d+\)");

        /// <summary>
        /// 资源临时目录
        /// </summary>
        public string Temp_PmbnfileDir { get; set; }
        /// <summary>
        /// 获取COM口
        /// </summary>
        private void LoadComs()
        {
            Ob_ComDev.Clear();
            string[] mulGetHardwareInfos = MulHardware.GetMulHardwareInfo(HardwareEnum.Win32_PnPEntity, "Name");
            foreach (var com in mulGetHardwareInfos)
            {
                if (!com.Contains("9008"))
                {
                    continue;
                }
                string temp = _rgCName.Match(com).Value.Trim('(', ')');
                Ob_ComDev.Add(temp);
            }
        }

        private bool _isStop;
        /// <summary>
        /// 镜像是否停止：true-停止，反之亦然
        /// </summary>
        public bool IsStop
        {
            get { return _isStop; }

            set
            {
                _isStop = value;
                OnPropertyChanged("IsStop");
            }
        }
        /// <summary>
        /// 镜像数据回调
        /// </summary>
        private FileServiceCoreDll.ImageDataCallBack _ImageDataCallBack;

        private long _ProgressMaxValue = 1;
        /// <summary>
        /// 进度条最大值
        /// </summary>
        public long ProgressMaxValue
        {
            get { return _ProgressMaxValue; }
            set
            {
                _ProgressMaxValue = value;
                OnPropertyChanged("ProgressMaxValue");
            }
        }

        private long _ProgressValue;
        /// <summary>
        /// 当前进度值
        /// </summary>
        public long ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                _ProgressValue = value;
                ProgressPercentage = (int)(this.ProgressMaxValue > 0 ? (this.ProgressValue * 100 / this.ProgressMaxValue) : 0);
                OnPropertyChanged("ProgressValue");
            }
        }

        private int _ProgressPercentage;
        /// <summary>
        /// 进度百分比信息
        /// </summary>
        public int ProgressPercentage
        {
            get { return _ProgressPercentage; }
            set
            {
                _ProgressPercentage = value;
                OnPropertyChanged("ProgressPercentage");
            }
        }

        #region COM口

        /// <summary>
        /// COM口列表
        /// </summary>
        public ObservableCollection<string> Ob_ComDev { get; set; }
        /// <summary>
        /// 提示文字
        /// </summary>
        public ObservableCollection<string> MsgDatas { get; set; }

        private string _selectedCom;
        /// <summary>
        /// 当前选择的COM
        /// </summary>
        public string SelectedCom
        {
            get { return _selectedCom; }
            set
            {
                _selectedCom = value;
                OnPropertyChanged("SelectedCom");
            }
        }
        private bool _ImgIsEnabled = true;

        /// <summary>
        /// 数据镜像按钮是否可用
        /// </summary>
        public bool ImgIsEnabled
        {
            get { return _ImgIsEnabled; }
            set
            {
                _ImgIsEnabled = value;
                OnPropertyChanged("ImgIsEnabled");
            }
        }

        private string _savePath = "C:\\temp9008";
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                OnPropertyChanged("SavePath");
            }
        }
        /// <summary>
        /// 镜像扇区长度：当前字节数 / 512，转换为扇区长度
        /// </summary>
        private Int64 Sectorlen = 0;
        /// <summary>
        /// 镜像大小描述：扇区长度/ 2 * 1024L，转换为字节
        /// </summary>
        public string ImgSizeDesc { get; set; }

        private readonly System.Timers.Timer _timer;

        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        private TimeSpan _totalElapsed = TimeSpan.Zero;
        public TimeSpan TotalElapsed
        {
            get => _totalElapsed;
            private set
            {
                _totalElapsed = value;
                OnPropertyChanged();
            }
        }

        private PhoneNoInfo _selectedBrand;

        public PhoneNoInfo SelectedBrand
        {
            get { return _selectedBrand; }
            set
            {
                _selectedBrand = value;
                OnPropertyChanged("SelectedBrand");
            }
        }
        /// <summary>
        /// 异步跟踪操作
        /// </summary>
        private AsyncOperation AsyncOperation { get; set; }
        /// <summary>
        /// 镜像文件名
        /// </summary>
        public string MirrorName { get; set; }
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string MirrorLocal { get; set; }
        /// <summary>
        /// MD5值
        /// </summary>
        public string VerifyCode { get; set; }
        /// <summary>
        /// 镜像文件流
        /// </summary>
        public FileStream MirrorStream { get; set; }
        /// <summary>
        /// 初始化句柄
        /// </summary>
        public IntPtr Handle9008 = IntPtr.Zero;
        /// <summary>
        /// 指令进入
        /// </summary>
        public string Reboot_l = @"adb reboot edl";

        #endregion
        public ICommand DeviceDetectionCommand { get; private set; }
        public ICommand InstallCommand { get; private set; }
        public ICommand AdbIntoCommand { get; private set; }
        public ICommand GeneralCommand { get; private set; }
        public ICommand LineCommand { get; private set; }
        public ICommand SelectedPathCommand { get; private set; }
        public ICommand BeginCommand { get; private set; }
    }
}
