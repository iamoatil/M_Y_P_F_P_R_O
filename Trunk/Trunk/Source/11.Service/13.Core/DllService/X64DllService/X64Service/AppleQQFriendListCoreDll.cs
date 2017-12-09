/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/12/5 13:37:10 
 * explain :  
 *
*****************************************************************************/

using System.Runtime.InteropServices;

namespace X64Service
{
    public static class AppleQQFriendListCoreDll
    {
        private const string _Dll = @"Lib\vcdllX64\AppleQQFriendList\QQFriendList_App.dll";

        /// <summary>
        /// 解析QQ好友列表plist文件
        /// </summary>
        /// <param name="QQFriendListFile">QQFriendList_v3.plist文件路径</param>
        /// <param name="SaveToXmlFile">文件保存路径</param>
        /// <returns></returns>
        [DllImport(_Dll)]
        public static extern int GetAppleQQFriendList(string QQFriendListFile, string SaveToXmlFile);
    }
}
