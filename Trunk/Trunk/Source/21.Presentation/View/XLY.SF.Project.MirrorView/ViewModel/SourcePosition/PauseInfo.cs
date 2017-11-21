/* ==============================================================================
* Description：PauseInfo  
* Author     ：litao
* Create Date：2017/11/20 16:32:43
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.MirrorView
{
    /// <summary>
    /// 暂停的信息
    /// </summary>
    internal class PauseInfo 
    {
        /// <summary>
        /// 暂停的位置
        /// </summary>
        public long PausePos { get; private set; }

        /// <summary>
        /// 暂停的文件在列表中的索引
        /// </summary>
        public int  PauseFileIndex { get; private set; }

        /// <summary>
        /// 设置暂停的文件列表
        /// </summary>
        public List<MirrorBlockInfo> MirrorBlockInfos { get; private set; }

        /// <summary>
        /// 暂停时的位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetPausePos(long pos)
        {
            PausePos = pos;
        }

        /// <summary>
        /// 暂停时的文件信息
        /// </summary>
        /// <param name="path"></param>
        public void SetPauseFileInfo(int index,List<MirrorBlockInfo> list)
        {
            MirrorBlockInfos = list;
            PauseFileIndex = index;
        }    
    }
}
