using Aladdin.HASP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using XLY.SF.Framework.BaseUtility;

namespace X64Service
{
    /// <summary>
    /// 授权服务类
    /// </summary>
    public static class SecretCoreDll
    {
        private static string VendorCode = "7hbRO/VFNaV1zBsQcJ3/E7PqmcK6rLMsdouGkaifIlnvWO5JDlT06+BkKuNlXNq9dWBvsqME2oNKvXv4wgrY3pp6dfTuPoa/Pu0BRrFBRep9czqd6nylQL0aNVzl5I4tToqyIRfja9HzmS4Uf/E5bpSxXaIIfN4TpN1B1NB28gyBlSEbNafBqiRNZvewizrjzPiPwkY0xYVDLhB0czhT3Yv6fb6KNG+X5uIcPqEZg5pWFuF047kzsZuCZHyUKxK/cSwkFbLdbksTorj1/UYLm7zwhAKVctVuksTa+1Mleg+/o9gepuLSdYuT3AiZaSSfo0CnnlKOwPNuRt9A56+xAbN//Bwbgc3rWHlZpJWK8cl2LQ9tzZ7XsmrjfSgHB2BUjN3KNlsIwtKeLEC1KXK3twBKxH461trBOx6qo5YckwIpkY8vMXcjjORkBEC5xtNPERHjI8VCuow7E2dkVzgD+e2GnA24W8DGAmDPGEXq0ExnkbJwWVM8N/ZknO9nJCq2BUG7JUIlEG9EcSeD98TT6W13MAhPir7ti4OLHU9Z1EzG9V8ycJhkXfzoYVzzB5myQ7E9n16NEhZQPVOQaFvFq55Y+rKsG97RGT5Ba/131iUkPQGHAjWfx5WezU5R4p4BpEh/A+bTZ2dKD9KIParIVxux8oYetYguIrqpvWT0nfaQh6dRKokzHL+FZO1ZR4VbJucyHbjeAnv2th2TvLjRGOdqzAvXDCIf1X/2PJOUct8+AooHQwIFN1Iv8JNcgc0bF+xDf3qDpMLn1YPDJX4BEyKshBEPQxLPw2wE+iYsGb/b6dntwgyBfpCzBbe8/ANJJFRV3pebyw6TFCrLgyfImiVQQH8HZKpjkMsWvTdC3fu2umzb4+QmG5pSAVpAzp56Y6bgfsNJnttL66x3zDvSRBVQWwIM3wJPot2Tu0o+gm8Jwej/j6QoByDrT3/JaxYZhSzf3GDsE5VWrBn0+WvK3w==";

        private const string _Secret = @"Lib\vcdllX64\Secret\Secret.dll";

        #region 新授权

        public static IXSLCSecret SecretImpl = null;

        /// <summary>
        /// 加载授权机密库
        /// </summary>
        /// <param name="productId">产品ID，表xemm_product_license[productkeyid]</param>
        /// <param name="instance">授权库接口对象</param>
        [DllImport(_Secret, EntryPoint = "GetSecretImpl", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern void GetSecretImpl([MarshalAs(UnmanagedType.AnsiBStr)] string productId, [MarshalAs(UnmanagedType.Interface)] out IXSLCSecret instance);

        /// <summary>
        /// 加载授权机密库
        /// </summary>
        /// <param name="productId">产品ID，表xemm_product_license[productkeyid]</param>
        public static void LoadSecret(string productId = "c48977aa4d9a4ccda14f3eb02e1aff9f")
        {
            GetSecretImpl(productId, out SecretImpl);
        }

        /// <summary>
        /// 检查模块授权数
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <returns></returns>
        public static int CheckModule(string moduleName)
        {
            var ss = SecretImpl.CheckModule(moduleName);
            return ss;
        }

        /// <summary>
        /// 授权注销
        /// </summary>
        public static void Cancellation()
        {
            SecretImpl.Cancellation();
        }

        /// <summary>
        /// 获取关键库数据
        /// </summary>
        /// <returns>关键库命名空间</returns>
        private static byte[] GetKeyData()
        {
            IntPtr data;
            int size = SecretImpl.GetKeyDataBegin(out data);
            byte[] ret = new byte[size];
            Marshal.Copy(data, ret, 0, size);
            SecretImpl.GetKeyDataEnd();
            return ret;
        }

        //使用授权库中的的加密数据反射生成关键库，并调用其中的方法（关键库为程序中必须的方法，目标是防止授权库的调用被注释）
        public static int TestStrToInt(string str)
        {
            byte[] datas = GetKeyData();
            var assembly = Assembly.Load(datas);
            var type = assembly.GetType("XlySoftSecret.KeyLib");
            var method = type.GetMethod("ToInt32");
            var instance = Activator.CreateInstance(type);
            object[] args = { str };
            var ret = method.Invoke(instance, args);

            return (int)ret;
        }

        /// <summary>
        /// 释放授权库
        /// </summary>
        public static void Free()
        {
            if (null != SecretImpl)
            {
                Marshal.ReleaseComObject(SecretImpl);
                SecretImpl = null;
                GC.Collect();
            }
        }
        public static void CheckDogIfNullThenExitApplication(string featureId)
        {
            var infos = GetSentinelInfos();
            bool flag = infos.Any(s => s.FeatureIdList.Contains(featureId));
            if (!flag)
            {
                LoadSecret();
            }
        }
        public static IEnumerable<SentinelModel> GetSentinelInfos()
        {
            var sentinelList = new List<SentinelModel>();
            string info = "";
            // 请求内容
            string scope = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><haspscope/>";
            // 返回内容项
            string queryFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                                 "<haspformat root=\"hasp_info\"> <hasp> <attribute name=\"id\" /> <attribute name=\"type\" />  <feature>" +
                                 " <attribute name=\"id\" />  </feature> </hasp></haspformat>";

            // 返回内容项
            string format = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><admin><hasp><element name=\"*\" /></hasp></admin>";

            HaspFeature feature = HaspFeature.Default;

            // this will perform a logout and object disposal
            // when the using scope is left.
            using (Hasp hasp = new Hasp(feature))
            {
                hasp.Login(VendorCode, scope);
            }
            Hasp.GetInfo(scope, queryFormat, VendorCode, ref info);
            if (info.IsInvalid())
            {
                return sentinelList;
            }

            // 解析xml字符串，获取加密狗ID
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(info);
            XmlElement root = doc.DocumentElement;
            if (root == null) return sentinelList;
            XmlNodeList nodeList = root.GetElementsByTagName("hasp");
            foreach (XmlNode node in nodeList)
            {
                SentinelModel sentinelModel = new SentinelModel();
                sentinelModel.HaspId = node.GetSafeAttributeValue("id");
                if (sentinelModel.HaspId.IsInvalid()) continue;
                XmlNodeList features = node.ChildNodes;
                sentinelModel.FeatureIdList = new List<string>();
                foreach (XmlNode featureNode in features)
                {
                    sentinelModel.FeatureIdList.Add(featureNode.GetSafeAttributeValue("id"));
                }

                sentinelList.Add(sentinelModel);
            }
            return sentinelList;
        }
    }
}
/// <summary>
/// 加密狗信息
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct SentinelModel
{
    /// <summary>
    /// 加密狗Id
    /// </summary>
    public string HaspId { get; set; }

    /// <summary>
    /// 加密狗Feature列表
    /// </summary>
    public List<string> FeatureIdList { get; set; }
}
#endregion

[ComVisible(true)]
[ComImport, Guid("1BA695E8-45D8-47CC-B2B5-9028FED150FB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IXSLCSecret
{
    /// <summary>
    /// 检查授权情况
    /// </summary>
    /// <returns>大于0表示授权可用，等于0表示授权内容无效，小于0表示无授权数据</returns>
    [MethodImplAttribute(MethodImplOptions.PreserveSig)]
    int CheckLicense();

    /// <summary>
    /// 检查模块授权数(检查升级期限时用“UPDATE_EXPIRE”做moduleName)
    /// </summary>
    /// <param name="moduleName">模块名</param>
    /// <returns>授权数量</returns>
    [MethodImplAttribute(MethodImplOptions.PreserveSig)]
    int CheckModule([MarshalAs(UnmanagedType.AnsiBStr)] string moduleName);

    /// <summary>
    /// 授权注销
    /// </summary>
    [MethodImplAttribute(MethodImplOptions.PreserveSig)]
    void Cancellation();

    /// <summary>
    /// 获取关键库数据开始
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImplAttribute(MethodImplOptions.PreserveSig)]
    int GetKeyDataBegin([Out]out IntPtr data);

    /// <summary>
    /// 获取关键库数据结束
    /// </summary>
    [MethodImplAttribute(MethodImplOptions.PreserveSig)]
    void GetKeyDataEnd();

}
