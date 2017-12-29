
using ProjectExtend.Context;
using System.Linq;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ProxyService;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewModels.Main.CaseManagement;

namespace XLY.SF.Project.MirrorView
{
    class MirrorFileParser
    {
        readonly MsgAggregation _messageAggregation;

        public MirrorFileParser(MsgAggregation MessageAggregation)
        {
            _messageAggregation = MessageAggregation;
        }

        /// <summary>
        /// 解析镜像
        /// </summary>
        /// <param name="_mirrorFile"></param>
        public void ParseMirror(MirrorFile _mirrorFile)
        {
            if (_mirrorFile == null
               || !_mirrorFile.IsIntegrated)
            {
                return;
            }
            string selectedFileName = _mirrorFile.FilePath;
            string selectedPlatform = ProxyFactory.LocalFile.GetOSType(selectedFileName).GetDescriptionX();

            LocalFileDeviceTypeData localFileDeviceTypeData = new LocalFileDeviceTypeData();
            var dicOSType = localFileDeviceTypeData.DicOSType;
            var dicFlshType = localFileDeviceTypeData.DicFlshType;
            EnumOSType platformType = dicOSType[selectedPlatform];
            if (platformType == EnumOSType.None)
            {
                return;
            }

            var localFileDevice = new LocalFileDevice(selectedFileName, false);
            if (platformType == EnumOSType.YunOS)
            {
                platformType = EnumOSType.Android;
            }
            localFileDevice.OSType = platformType;
            if (dicFlshType.ContainsKey(platformType))       //如果是山寨机，则设置芯片类型和设备类型
            {
                localFileDevice.CottageFlshType = dicFlshType[platformType].Item1;
                localFileDevice.CottageDevType = dicFlshType[platformType].Item2;
            }
            CreateDevice(localFileDevice);
        }

        /// <summary>
        /// 创建一个设备，如果设备已经存在，则跳转到该设备
        /// </summary>
        /// <param name="device"></param>
        private void CreateDevice(IDevice device)
        {
            if (device.DeviceType == EnumDeviceType.None)
            {
                return;
            }
            DeviceExtractionAdorner dea = new DeviceExtractionAdorner();
            var ca = SystemContext.Instance.CurrentCase;
            var dev = ca.DeviceExtractions.FirstOrDefault(d => d[DeviceExternsion.XLY_IdKey] == device.ID);
            if (dev == null)     //不存在则创建新设备
            {
                dev = SystemContext.Instance.CurrentCase.CreateDeviceExtraction(device.Name, device.DeviceType.ToString());
                dea.Target = dev;
                dea.Device = device;
                dea.Save();
            }
            else
            {
                dea.Target = dev;
            }
            //跳转到设备
            _messageAggregation.SendGeneralMsg(new GeneralArgs<DeviceExtractionAdorner>(ExportKeys.DeviceAddedMsg) { Parameters = dea });
        }
    }
}
