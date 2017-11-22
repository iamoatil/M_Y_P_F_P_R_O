/* ==============================================================================
* Description：Md5ConfigFile  
* Author     ：litao
* Create Date：2017/11/22 16:56:01
* ==============================================================================*/


using System.IO;

namespace XLY.SF.Project.EarlyWarningView
{
    class Md5ConfigFileDir: AbstractConfigFileDir
    {
        public override bool Initialize(string dir)
        {
            IsInitialize = false;

            if (!Directory.Exists(dir))
            {
                return IsInitialize;
            }
            _dir = dir;

            IsInitialize = true;
            return IsInitialize;
        }

        protected override string RootName { get { return "FileMd5Collection"; } }

        protected override string Dir { get { return _dir; } }
        private string _dir;
    }
}
