using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：DirectoryPair  
* Author     ：litao
* Create Date：2017/11/2 13:41:49
* ==============================================================================*/

namespace CopyDll
{
    public class DirectoryPair
    {
        public DirectoryPair(string sourceDir,string targetDir)
        {
            SourceDir = new FileDirectory(sourceDir,"*.*", SearchOption.AllDirectories);
            TargetDir = new FileDirectory(targetDir, "*.*", SearchOption.AllDirectories);
        }

        public FileDirectory SourceDir { get; private set; }
        public FileDirectory TargetDir { get; private set; }     
    }
}
