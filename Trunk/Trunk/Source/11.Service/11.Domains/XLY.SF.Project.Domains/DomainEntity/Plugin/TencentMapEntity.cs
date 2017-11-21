using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    [Serializable]
    public class TencentMapAddress
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon
        {
            get;
            set;
        }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Address.IsValid() ? string.Format("{0}\r\n经度:{1} 纬度:{2}", Address, Lon, Lat) : string.Empty;
        }
    }

    /// <summary>
    /// 腾讯地图相关抽象类 
    /// </summary>
    [Serializable]
    public abstract class AbstractTencentMapEntity : AbstractDataItem
    {
    }

    /// <summary>
    /// 腾讯地图帐号信息
    /// </summary>
    [Serializable]
    public class TencentMapAccount : AbstractTencentMapEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Display]
        public string NickName
        {
            get;
            set;
        }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [Display]
        public DateTime? LastLoginTime
        {
            get;
            set;
        }

        /// <summary>
        /// 最后登录城市
        /// </summary>
        [Display]
        public string LastLoginCity
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 腾讯地图搜索地点
    /// </summary>
    [Serializable]
    public class TencentMapSearchAddress : AbstractTencentMapEntity
    {
        [Display]
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 搜索结果
        /// </summary>
        [Display]
        public string ResultAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 最后搜索时间
        /// </summary>
        [Display]
        public DateTime? LastTime
        {
            get;
            set;
        }

        [Display]
        public int SearchCount
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 腾讯地图搜索路线
    /// </summary>
    [Serializable]
    public class TencentMapSearchRoute : AbstractTencentMapEntity
    {
        [Display]
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 搜索起点
        /// </summary>
        [Display]
        public string RouteFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 搜索终点
        /// </summary>
        [Display]
        public string RouteTo
        {
            get;
            set;
        }

        /// <summary>
        /// 最后搜索时间
        /// </summary>
        [Display]
        public DateTime? LastTime
        {
            get;
            set;
        }

        [Display]
        public int SearchCount
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 腾讯地图收藏地点
    /// </summary>
    [Serializable]
    public class TencentMapFavoriteAddr : AbstractTencentMapEntity
    {
        [Display]
        public string Name
        {
            get;
            set;
        }

        [Display]
        public DateTime? Time
        {
            get;
            set;
        }

        /// <summary>
        /// 经度
        /// </summary>
        [Display]
        public double Lon
        {
            get;
            set;
        }

        /// <summary>
        /// 纬度
        /// </summary>
        [Display]
        public double Lat
        {
            get;
            set;
        }

        [Display]
        public string Tag
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 腾讯地图收藏路线
    /// </summary>
    [Serializable]
    public class TencentMapFavoriteRoute : AbstractTencentMapEntity
    {
        [Display]
        public string Name
        {
            get;
            set;
        }

        [Display]
        public string From
        {
            get;
            set;
        }

        [Display]
        public string To
        {
            get;
            set;
        }

        [Display]
        public string RouteType
        {
            get;
            set;
        }

        [Display]
        public string TakeTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 腾讯地图收藏街景
    /// </summary>
    [Serializable]
    public class TencentMapFavoriteStreet : AbstractTencentMapEntity
    {
        [Display]
        public string Name
        {
            get;
            set;
        }

        [Display]
        public DateTime? Time
        {
            get;
            set;
        }

        [Display]
        public string FileInfo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 腾讯地图同行聊天消息
    /// </summary>
    [Serializable]
    public class TencentMapTongxingMsg : AbstractTencentMapEntity
    {
        [Display]
        public string GroupId
        {
            get;
            set;
        }

        [Display]
        public string Sender
        {
            get;
            set;
        }

        [Display]
        public DateTime? Time
        {
            get;
            set;
        }

        [Display]
        public string Msg
        {
            get;
            set;
        }

    }
}
