using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// JSON辅助类。
    /// </summary>
    public static class JsonHelper
    {
        #region Fields

        /// <summary>
        /// 消息JSON序列化配置。
        /// </summary>
        public static readonly JsonSerializerSettings JsonSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 JsonHelper。
        /// </summary>
        static JsonHelper()
        {
            JsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
                TypeNameHandling = TypeNameHandling.Auto
            };
            DefaultContractResolver resolver = new DefaultContractResolver();
            resolver.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            JsonSettings.ContractResolver = resolver;
        }

        #endregion

        #region Methods

        #region Public

        #region DeserializeObject

        /// <summary>
        /// 反序列化JSON字符串。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="json">JSON字符串。</param>
        /// <returns>类型 T 的实例。</returns>
        public static T DeserializeObject<T>(this String json)
        {
            return json.DeserializeObject<T>(default(T));
        }

        /// <summary>
        /// 反序列化JSON字符串。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="json">JSON字符串。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>类型 T 的实例。</returns>
        public static T DeserializeObject<T>(this String json, T defaultValue)
        {
            try
            {
                if (json == null) return defaultValue;
                return JsonConvert.DeserializeObject<T>(json, JsonSettings);
            }
            catch (JsonException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 反序列化JSON字符串。
        /// </summary>
        /// <param name="json">JSON字符串。</param>
        /// <param name="type">类型。</param>
        /// <returns>实例。</returns>
        public static Object DeserializeObject(this String json, Type type)
        {
            try
            {
                if (json == null) return null;
                return JsonConvert.DeserializeObject(json, type, JsonSettings);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// 反序列化JSON字符串。
        /// </summary>
        /// <param name="json">JSON字符串。</param>
        /// <returns>实例。</returns>
        public static Object DeserializeObject(this String json)
        {
            try
            {
                if (json == null) return null;
                return JsonConvert.DeserializeObject(json, JsonSettings);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        #endregion

        #region SerializeObject

        /// <summary>
        /// 序列化对象。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>JSON字符串</returns>
        public static String SerializeObject(this Object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj, JsonSettings);
        }

        #endregion

        #endregion

        #endregion
    }
}
