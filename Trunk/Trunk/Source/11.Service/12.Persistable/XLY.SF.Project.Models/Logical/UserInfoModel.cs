using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;

namespace XLY.SF.Project.Models.Logical
{
    public class UserInfoModel :  LogicalModelBase<UserInfo>
    {
        #region Constructors

        public UserInfoModel(UserInfo entity)
            : base(entity)
        {
        }

        public UserInfoModel()
        {
        }

        #endregion

        #region Properties

        public Int64 UserID => Entity.UserID;

        [Required]
        [StringLength(10)]
        public virtual String UserName
        {
            get => Entity.UserName;
            set
            {
                Entity.UserName = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        public virtual String WorkUnit
        {
            get => Entity.WorkUnit;
            set
            {
                Entity.WorkUnit = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        public virtual String IdNumber
        {
            get => Entity.IdNumber;
            set
            {
                Entity.IdNumber = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        public virtual String PhoneNumber
        {
            get => Entity.PhoneNumber;
            set
            {
                Entity.PhoneNumber = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        [Required]
        [StringLength(10)]
        public virtual String LoginUserName
        {
            get => Entity.LoginUserName;
            set
            {
                Entity.LoginUserName = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        #region LoginPassword

        private String _loginPassword=string.Empty;
        [Required]
        public virtual String LoginPassword
        {
            get => _loginPassword;
            set
            {
                _loginPassword = value;
                MD5CryptoServiceProvider md5Psd = new MD5CryptoServiceProvider();
                String newPsd = BitConverter.ToString(md5Psd.ComputeHash(Encoding.ASCII.GetBytes(this.LoginPassword)));
                Entity.LoginPassword = newPsd;
                OnPropertyChanged();
            }
        }

        #endregion

        public virtual DateTime LoginTime
        {
            get => Entity.LoginTime;
            set
            {
                Entity.LoginTime = value;
                OnPropertyChanged();
            }
        }

        #region IsChecked

        private Boolean _isChecked;
        public Boolean IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Password

        private String _password = String.Empty;
        public String Password
        {
            get => _password;
            set
            {
                _password = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        #endregion

        #region ConfirmPassword

        private String _confirmPassword = String.Empty;
        public String ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取用户信息只读实例。
        /// </summary>
        /// <returns>只读实例。</returns>
        public UserInfoReadOnlyModel ToReadOnly()
        {
            return new UserInfoReadOnlyModel(Entity);
        }

        #endregion

        #endregion
    }
}
