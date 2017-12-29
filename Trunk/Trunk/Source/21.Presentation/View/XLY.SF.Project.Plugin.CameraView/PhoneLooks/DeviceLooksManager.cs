using System.Collections.Generic;
using System.IO;

namespace XLY.SF.Project.CameraView
{
    public class DeviceLooksManager
    {
        public List<DeviceLooks> DeviceLooksList
        {
            get
            {
                return _deviceLooks;
            }
        }

        private List<DeviceLooks> _deviceLooks = new List<DeviceLooks>();

        public DeviceLooks First { get { return _deviceLooks[0]; } }
        

        /// <summary>
        /// 当前选中的条目
        /// </summary>
        public DeviceLooks SelectedItem
        {
            get
            {
                foreach (DeviceLooks item in DeviceLooksList)
                {
                    if(item.IsSelected == true)
                    {
                        return item;
                    }
                }
                DeviceLooks curItem=DeviceLooksList[0];
                curItem.IsSelected = true;
                return curItem;
            }
        }

        public bool IsInitialized { get; private set; }

        private string _dir;

        public void Initialize(string dir)
        {
            //修整目录
            if (!dir.EndsWith("\\"))
            {
                dir += "\\";
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _dir = dir;

            IsInitialized = InnerInitialize();
        }

        private bool InnerInitialize()
        {
            _deviceLooks.Add(new DeviceLooks("正面", _dir + "front.jpg") { IsSelected = true });
            _deviceLooks.Add(new DeviceLooks("背面", _dir + "back.jpg"));
            _deviceLooks.Add(new DeviceLooks("侧面", _dir + "side.jpg"));
            return true;
        }

    }
}
