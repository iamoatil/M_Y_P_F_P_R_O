using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ProxyService;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.MessageBase;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.ViewModels.Device.LocalFileSelectControlViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/24 10:06:43
* ==============================================================================*/

namespace XLY.SF.Project.ViewModels.Device
{
    /// <summary>
    /// 本地文件选择时文件系统类型判断窗口ViewModel
    /// </summary>
    [Export(ExportKeys.DeviceSelectFileViewModel, typeof(ViewModelBase))]
    public class LocalFileSelectControlViewModel : ViewModelBase
    {
        public LocalFileSelectControlViewModel()
        {
            OKCommond = new RelayCommand(DoOKCommond);
            CancelCommond = new RelayCommand(DoCancelCommond);
            SelectFileDlgCommond = new RelayCommand(DoSelectFileDlgCommond);

            _dicOSType = Enum.GetValues(typeof(EnumOSType)).Cast<EnumOSType>().ToDictionary(o => o.GetDescriptionX(), o => o);
            PlatformCollection.AddRange(_dicOSType.Keys);
            SelectedPlatform = EnumOSType.Android.GetDescriptionX();

            _dicFlshType = new Dictionary<EnumOSType, Tuple<FlshType, DevType>>()
            {
                {EnumOSType.MTK, new Tuple<FlshType, DevType>(FlshType.FT_MTK, DevType.DT_11)},
                {EnumOSType.Spreadtrum, new Tuple<FlshType, DevType>(FlshType.FT_Spreadtrum, DevType.DT_11)},
                {EnumOSType.Symbian, new Tuple<FlshType, DevType>(FlshType.FT_Symbian, DevType.DT_11)},
                //{EnumOSType.WindowsPhone, new Tuple<FlshType, DevType>(FlshType.FT_WindowsPhone, DevType.DT_11)},
                {EnumOSType.MStar, new Tuple<FlshType, DevType>(FlshType.FT_MStar, DevType.DT_11)},
                {EnumOSType.WebOS, new Tuple<FlshType, DevType>(FlshType.FT_WebOS, DevType.DT_11)},
                {EnumOSType.WindowsMobile, new Tuple<FlshType, DevType>(FlshType.FT_WindowsMobile, DevType.DT_11)},
                {EnumOSType.Bada, new Tuple<FlshType, DevType>(FlshType.FT_Bada, DevType.DT_11)},
                {EnumOSType.ADI, new Tuple<FlshType, DevType>(FlshType.FT_ADI, DevType.DT_11)},
                {EnumOSType.Infineon, new Tuple<FlshType, DevType>(FlshType.FT_Infineon, DevType.DT_11)},
                {EnumOSType.CoolSand, new Tuple<FlshType, DevType>(FlshType.FT_CoolSand, DevType.DT_11)},
                {EnumOSType.Sky, new Tuple<FlshType, DevType>(FlshType.FT_Sky, DevType.DT_11)}
            };
        }

        #region 属性

        #region 支持的操作系统列表
        private ObservableCollection<string> _PlatformCollection = new ObservableCollection<string>();

        /// <summary>
        /// 支持的操作系统列表
        /// </summary>	
        public ObservableCollection<string> PlatformCollection
        {
            get { return _PlatformCollection; }
            set
            {
                _PlatformCollection = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前选择的操作系统
        private string _selectedPlatform;

        /// <summary>
        /// 当前选择的操作系统
        /// </summary>	
        public string SelectedPlatform
        {
            get { return _selectedPlatform; }
            set
            {
                _selectedPlatform = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前选择的文件路径
        private string _selectedFileName;

        /// <summary>
        /// 当前选择的文件路径
        /// </summary>	
        public string SelectedFileName
        {
            get { return _selectedFileName; }
            set
            {
                _selectedFileName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        [Import(typeof(IPopupWindowService))]
        private IPopupWindowService _fileDlg
        {
            get;
            set;
        }

        private LocalFileDevice _selectedFile = null;
        public override object GetResult()
        {
            return _selectedFile;
        }
        #endregion

        #region Commond

        #region 点击了确定按钮

        public RelayCommand OKCommond { get; set; }

        /// <summary>
        /// 点击了确定按钮
        /// </summary>
        private void DoOKCommond()
        {
            if (!File.Exists(SelectedFileName))
            {
                MessageBox.ShowDialogErrorMsg("请先选择文件!");
                return;
            }
            EnumOSType selectPlatform = _dicOSType[SelectedPlatform];
            if (selectPlatform == EnumOSType.None)
            {
                MessageBox.ShowDialogErrorMsg("请先选择系统类型!");
                return;
            }

            _selectedFile = new LocalFileDevice() { IsDirectory = false, PathName = SelectedFileName };
            if (selectPlatform == EnumOSType.YunOS)
            {
                selectPlatform = EnumOSType.Android;
            }
            _selectedFile.OSType = selectPlatform;
            if (_dicFlshType.ContainsKey(selectPlatform))       //如果是山寨机，则设置芯片类型和设备类型
            {
                _selectedFile.CottageFlshType = _dicFlshType[selectPlatform].Item1;
                _selectedFile.CottageDevType = _dicFlshType[selectPlatform].Item2;
            }
            base.DialogResult = true;
            base.CloseView();
        }

        #region 点击了取消按钮

        public RelayCommand CancelCommond { get; set; }

        /// <summary>
        /// 点击了取消按钮
        /// </summary>
        private void DoCancelCommond()
        {
            _selectedFile = null;
            base.CloseView();
        }
        #endregion

        #endregion

        #region 点击了选择文件对话框按钮

        public RelayCommand SelectFileDlgCommond { get; set; }

        /// <summary>
        /// 点击了选择文件对话框按钮
        /// </summary>
        private void DoSelectFileDlgCommond()
        {
            string path = _fileDlg.OpenFileDialog("All Files|*.*");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            SelectedFileName = path;
            SelectedPlatform = ProxyFactory.LocalFile.GetOSType(path).GetDescriptionX();       //自动判断文件类型
        }

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }
        #endregion


        #endregion

        #region 私有属性
        private Dictionary<string, EnumOSType> _dicOSType;
        private Dictionary<EnumOSType, Tuple<FlshType, DevType>> _dicFlshType;
        #endregion
    }
}
