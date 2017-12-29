using System;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 无法修改的案例信息。
    /// </summary>
    public sealed class RestrictedCaseInfo: CaseInfo
    {
        #region Fields

        internal readonly CaseInfo _caseInfo;

        #endregion

        #region Constructors

        internal RestrictedCaseInfo(CaseInfo caseInfo)
        {
            _caseInfo = caseInfo ?? throw new ArgumentNullException("caseInfo");
            Init(caseInfo);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 编号。
        /// </summary>
        public override String Number
        {
            get => base.Number;
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 路径。
        /// </summary>
        public override String Path
        {
            get => base.Path;
        }

        /// <summary>
        /// 案例所在目录。
        /// </summary>
        public String Directory { get; internal set; }

        /// <summary>
        /// 与之关联的可修改的案例信息实例。
        /// </summary>
        public CaseInfo Target => _caseInfo;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 恢复到修改之前的值。
        /// </summary>
        public void Reset()
        {
            Name = _caseInfo.Name;
            Type = _caseInfo.Type;
            Author = _caseInfo.Author;
        }

        #endregion

        #region Internal

        internal void Commit()
        {
            _caseInfo.Name = Name;
            _caseInfo.Type = Type;
            _caseInfo.Author = Author;
        }

        #endregion

        #region Private

        private void Init(CaseInfo caseInfo)
        {
            Id = caseInfo.Id;
            Timestamp = caseInfo.Timestamp;
            Name = caseInfo.Name;
            Type = caseInfo.Type;
            Author = caseInfo.Author;

            base.Number = caseInfo.Number;
            base.Path = caseInfo.Path;
        }

        #endregion

        #endregion
    }
}
