namespace LanguageToXls
{
    class LanguageWord
    {
        /// <summary>
        /// 是否已经初始化。只有初始化成功之后才执行其他逻辑
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 词语的虚拟路径
        /// </summary>
        public string VirtualPath { get; private set; }

        /// <summary>
        /// Word的内容
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize(string virtualPath,string content)
        {
            IsInitialized = false;

            VirtualPath = virtualPath;
            Content = content;
            return IsInitialized;
        }
    }
}
