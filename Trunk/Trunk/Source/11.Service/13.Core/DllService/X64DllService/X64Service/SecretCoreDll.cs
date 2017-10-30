using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace X64Service
{
    /// <summary>
    /// 授权服务类
    /// </summary>
  public static class SecretCoreDll
    {

        private const string _XlyHasp = "bin\\xlyhasp.dll";

        private const string _Secret = "bin\\Secret.dll";

        #region 检测老加密狗

        /// <summary>
        /// 装载加密狗dll
        /// </summary>
        [DllImport(_XlyHasp, EntryPoint = "#10")]
        public static extern IntPtr hasp_api_A(UInt32 VendorCode = 0, Int32 len = 0);

        /// <summary>
        /// 卸载加密狗dll
        /// </summary>
        [DllImport(_XlyHasp, EntryPoint = "#11")]
        public static extern void hasp_api_B(IntPtr mountDev);

        /// <summary>
        /// 检测是否插上加密狗
        /// </summary>
        [DllImport(_XlyHasp, EntryPoint = "#13")]
        public static extern UInt32 hasp_api_D(IntPtr mountDev, Int32 hasp_feature_id);

        #endregion
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
            return SecretImpl.CheckModule(moduleName);
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
}
