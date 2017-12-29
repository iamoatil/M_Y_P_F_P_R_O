namespace XLY.SF.Project.DataMirrorApp
{
    interface IMirror
    {
        bool IsInitialized { get; }        

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
    }
}
