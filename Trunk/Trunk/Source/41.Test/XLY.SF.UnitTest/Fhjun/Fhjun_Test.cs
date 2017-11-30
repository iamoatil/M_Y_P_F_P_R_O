using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.Plugin.DataReport;

/* ==============================================================================
* Assembly   ：	XLY.SF.UnitTest.Fhjun_Test
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/29 13:51:56
* ==============================================================================*/

namespace XLY.SF.UnitTest
{
    /// <summary>
    /// Fhjun_Test
    /// </summary>
    [TestClass]
    public class Fhjun_Test
    {
        private void Load()
        {
            Console.WriteLine("开始启动服务");
            IocManagerSingle.Instance.LoadParts(this.GetType().Assembly);
            PluginAdapter.Instance.Initialization(null);
        }

        private void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]{message}");
        }

        private string DeskPath(string relativePath)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), relativePath);
        }

        #region 测试Html报表导出
        private List<IDataSource> CreateDataSource(string path)
        {
            string DB_PATH = path + @"\test1.db";
            List<IDataSource> dataPool = new List<IDataSource>();
            TreeDataSource treeDataSource = new TreeDataSource();
            SimpleDataSource callDataSource = new SimpleDataSource();

            treeDataSource.PluginInfo = new DataParsePluginInfo() { Name = "微信", Guid = "11FC356E-3EA6-481F-ACF6-D96925F80A4C", DeviceOSType = EnumOSType.Android };
            treeDataSource.TreeNodes = new List<TreeNode>();
            var rootNode = new TreeNode() { Text = "微信账户", TreeNodes = new List<TreeNode>(), Type = typeof(WeChatFriendShow), Items = new DataItems<WeChatFriendShow>(DB_PATH) };
            treeDataSource.TreeNodes.Add(rootNode);
            for (int i = 0; i < 2; i++)
            {
                TreeNode t = new TreeNode();
                t.Text = "账号" + i;
                rootNode.TreeNodes.Add(t);
                rootNode.Items.Add(new WeChatFriendShow() { Nick = "账号" + i, WeChatId = "账号" + i, Gender = EnumSex.Female });

                TreeNode accouts = new TreeNode();
                accouts.Text = "通讯录";
                accouts.IsHideChildren = true;
                t.TreeNodes.Add(accouts);
                accouts.Type = typeof(WeChatFriendShow);
                accouts.Items = new DataItems<WeChatFriendShow>(DB_PATH);
                for (int j = 0; j < 10; j++)
                {
                    accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j, Gender = EnumSex.Female });
                }

                TreeNode accouts2 = new TreeNode();
                accouts2.Text = "消息列表";
                accouts2.IsHideChildren = i % 2 == 0;
                accouts2.Type = typeof(WeChatFriendShow);
                accouts2.Items = new DataItems<WeChatFriendShow>(DB_PATH);
                t.TreeNodes.Add(accouts2);
                for (int j = 0; j < 5; j += 2)
                {
                    accouts2.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j });
                    TreeNode friend = new TreeNode();
                    friend.Text = "昵称" + j;
                    friend.Type = typeof(MessageCore);
                    friend.Items = new DataItems<MessageCore>(DB_PATH);
                    accouts2.TreeNodes.Add(friend);

                    for (int k = 0; k < 100; k++)
                    {
                        MessageCore msg = new MessageCore() { SenderName = friend.Text, SenderImage = "images/zds.png", Receiver = t.Text, Content = "发送信息内容" + k, MessageType = k % 4 == 0 ? "图片" : "文本", SendState = EnumSendState.Send };
                        friend.Items.Add(msg);
                        MessageCore msg2 = new MessageCore() { Receiver = friend.Text, SenderImage = "images/zjq.png", SenderName = t.Text, Content = "返回消息内容" + k, MessageType = k % 5 == 0 ? "图片" : "文本", SendState = EnumSendState.Receive };
                        friend.Items.Add(msg2);
                    }
                }

                TreeNode accouts3 = new TreeNode();
                accouts3.Text = "群";
                accouts3.IsHideChildren = true;
                t.TreeNodes.Add(accouts3);

                TreeNode accouts4 = new TreeNode();
                accouts4.Text = "群消息";
                accouts4.IsHideChildren = true;
                t.TreeNodes.Add(accouts4);
            }
            treeDataSource.BuildParent();
            dataPool.Add(treeDataSource);

            callDataSource.PluginInfo = new DataParsePluginInfo() { Name = "短信", Guid = "DDDC356E-3EA6-481F-ACF6-D96925F80EEE", DeviceOSType = EnumOSType.Android };
            callDataSource.Items = new DataItems<Call>(DB_PATH);
            for (int i = 0; i < 10; i++)
            {
                callDataSource.Items.Add(new Call() { DurationSecond = 10000, Name = "张三_" + i, Number = "10086" });
            }

            callDataSource.BuildParent();
            dataPool.Add(callDataSource);

            return dataPool;
        }
        /// <summary>
        /// 测试Html报表导出
        /// </summary>
        [TestMethod]
        public void TestHtmlReport()
        {
            Load();
            Log("开始测试");
            var pluginModules = PluginAdapter.Instance.GetPluginsByType<DataReportModulePluginInfo>(PluginType.SpfReportModule).ToList().ConvertAll(p => (AbstractDataReportModulePlugin)p.Value)
                .ConvertAll(m => m.PluginInfo as DataReportModulePluginInfo).OrderBy(m => m.OrderIndex);
            var  reportPlugins = PluginAdapter.Instance.GetPluginsByType<DataReportPluginInfo>(PluginType.SpfReport).ToList().ConvertAll(p => (AbstractDataReportPlugin)p.Value);
            foreach (var p in reportPlugins)   //添加报表模板信息
            {
                if (p.PluginInfo is DataReportPluginInfo rp)
                {
                    rp.Modules = pluginModules.Where(m => m != null && m.ReportId == rp.Guid).ToList();
                }
            }
            Assert.IsTrue(reportPlugins.Any());
            var destPath = reportPlugins.FirstOrDefault(pl => pl.PluginInfo.Name.Contains("Html报表")).Execute(new DataReportPluginArgument()
            {
                DataPool = CreateDataSource(DeskPath(@"")),
                ReportModuleName = "Html模板2(Bootstrap)",
                ReportPath = DeskPath(@"TestReport\"),
                CollectionInfo = new ExportCollectionInfo()
                {
                    CaseCode = "1244",
                    CaseName = "杀入按",
                    CaseType = "抢劫",
                    CollectLocation = "环球中心",
                    CollectLocationCode = "610000",
                    CollectorCertificateCode = "CollectorCertificateCode",
                    CollectorName = "by",
                    CollectTime = "2012-3-2"
                },
                DeviceInfo = new ExportDeviceInfo()
                {
                    BloothMac = "29:21:23:42:13:d9",
                    IMEI = "2343353453454",
                    Name = "fsdfi"
                }
            }, null);
            Log("Save OK!" + destPath);
            System.Diagnostics.Process.Start(destPath.ToSafeString());
            Log("测试结束");
        }

        [TestMethod]
        public void TestBcpReport()
        {
            Load();
            Log("开始测试");
            var pluginModules = PluginAdapter.Instance.GetPluginsByType<DataReportModulePluginInfo>(PluginType.SpfReportModule).ToList().ConvertAll(p => (AbstractDataReportModulePlugin)p.Value)
                .ConvertAll(m => m.PluginInfo as DataReportModulePluginInfo).OrderBy(m => m.OrderIndex);
            var reportPlugins = PluginAdapter.Instance.GetPluginsByType<DataReportPluginInfo>(PluginType.SpfReport).ToList().ConvertAll(p => (AbstractDataReportPlugin)p.Value);
            foreach (var p in reportPlugins)   //添加报表模板信息
            {
                if (p.PluginInfo is DataReportPluginInfo rp)
                {
                    rp.Modules = pluginModules.Where(m => m != null && m.ReportId == rp.Guid).ToList();
                }
            }
            Assert.IsTrue(reportPlugins.Any());
            var destPath = reportPlugins.FirstOrDefault(pl => pl.PluginInfo.Name.Contains("BCP")).Execute(new DataReportPluginArgument()
            {
                DataPool = CreateDataSource(DeskPath(@"")),
                ReportModuleName = null,
                ReportPath = DeskPath(@"TestReport\"),
                CollectionInfo = new ExportCollectionInfo()
                {
                    CaseCode = "1244",
                    CaseName = "杀入按",
                    CaseType = "抢劫",
                    CollectLocation = "环球中心",
                    CollectLocationCode = "610000",
                    CollectorCertificateCode = "CollectorCertificateCode",
                    CollectorName = "by",
                    CollectTime = "2012-3-2"
                },
                DeviceInfo = new ExportDeviceInfo()
                {
                    BloothMac = "29:21:23:42:13:d9",
                    IMEI = "2343353453454",
                    Name = "fsdfi"
                }
            }, null);
            Log("Save OK!" + destPath);
            System.Diagnostics.Process.Start(destPath.ToSafeString());
            Log("测试结束");
        }
        #endregion

        #region 测试反射的速度
        [TestMethod]
        public void TestRefectionSpeed()
        {
            int total = 100000;
            Console.WriteLine("测试反射速度：" + total);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            TestRefectionSpeed_Refection(total);
            Console.WriteLine("反射：" + watch.ElapsedMilliseconds);
            GC.Collect();

            watch.Reset();
            watch.Start();
            TestRefectionSpeed_Expression(total);
            Console.WriteLine("Expression：" + watch.ElapsedMilliseconds);
            GC.Collect();

            watch.Reset();
            watch.Start();
            TestRefectionSpeed_Emit(total);
            Console.WriteLine("Emit：" + watch.ElapsedMilliseconds);
            GC.Collect();

            watch.Reset();
            watch.Start();
            TestRefectionSpeed_Emit2(total);
            Console.WriteLine("Emit2：" + watch.ElapsedMilliseconds);
            GC.Collect();

            watch.Reset();
            watch.Start();
            TestRefectionSpeed_CS(total);
            Console.WriteLine("CS：" + watch.ElapsedMilliseconds);
            GC.Collect();
        }

        private void TestRefectionSpeed_Refection(int total)
        {
            Call call = new Call() { StartDate = new DateTime(2013, 12, 3), Name = "张三" };
            for (int i = 0; i < total; i++)
            {
                var type = call.GetType();
                var propertyInfo = type.GetProperty("StartDate");
                var propertyValue = propertyInfo.GetValue(call);
            }

            for (int i = 0; i < total; i++)
            {
                var type = call.GetType();
                var propertyInfo = type.GetProperty("StartDate");
                propertyInfo.SetValue(call, DateTime.Now.AddDays(i));
            }
        }

        private void TestRefectionSpeed_Expression(int total)
        {
            Call call = new Call() { StartDate = new DateTime(2013, 12, 3), Name = "张三" };
            var getter = ExpresionGetter<Call>("Name");
            var setter = ExpresionSetter<Call>("Name");
            for (int i = 0; i < total; i++)
            {
                var propertyValue = getter(call);
            }

            for (int i = 0; i < total; i++)
            {
                setter(call, "在胜多负少的");
            }
        }

        private void TestRefectionSpeed_Emit(int total)
        {
            Call call = new Call() { StartDate = new DateTime(2013, 12, 3), Name = "张三" };
            var getter = EmitGetter<Call>("StartDate");
            var setter = EmitSetter<Call>("StartDate");
            for (int i = 0; i < total; i++)
            {
                var propertyValue = getter(call);
            }

            for (int i = 0; i < total; i++)
            {
                setter(call, DateTime.Now.AddDays(i));
            }
        }

        private void TestRefectionSpeed_Emit2(int total)
        {
            Call call = new Call() { StartDate = new DateTime(2013, 12, 3), Name = "张三" };
            for (int i = 0; i < total; i++)
            {
                var propertyValue = call.Getter("StartDate");
            }

            for (int i = 0; i < total; i++)
            {
                call.Setter("StartDate", DateTime.Now.AddDays(i));
            }
        }

        private void TestRefectionSpeed_CS(int total)
        {
            Call call = new Call() { StartDate = new DateTime(2013, 12, 3), Name = "张三" };
            for (int i = 0; i < total; i++)
            {
                var propertyValue = call.StartDate;
            }

            for (int i = 0; i < total; i++)
            {
                call.StartDate = DateTime.Now.AddDays(i);
            }
        }

        private static Func<T, object> ExpresionGetter<T>(string propertyName)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);


            //// 对象实例
            var parameterExpression = Expression.Parameter(typeof(object), "obj");


            //// 转换参数为真实类型
            var unaryExpression = Expression.Convert(parameterExpression, type);


            //// 调用获取属性的方法
            var callMethod = Expression.Call(unaryExpression, property.GetGetMethod());
            var expression = Expression.Lambda<Func<T, object>>(callMethod, parameterExpression);


            return expression.Compile();
        }

        public static Action<T, object> ExpresionSetter<T>(string propertyName)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);


            var objectParameterExpression = Expression.Parameter(typeof(object), "obj");
            var objectUnaryExpression = Expression.Convert(objectParameterExpression, type);


            var valueParameterExpression = Expression.Parameter(typeof(object), "val");
            var valueUnaryExpression = Expression.Convert(valueParameterExpression, property.PropertyType);


            //// 调用给属性赋值的方法
            var body = Expression.Call(objectUnaryExpression, property.GetSetMethod(), valueUnaryExpression);
            var expression = Expression.Lambda<Action<T, object>>(body, objectParameterExpression, valueParameterExpression);


            return expression.Compile();
        }

        public static Func<T, object> EmitGetter<T>(string propertyName)
        {
            var type = typeof(T);


            var dynamicMethod = new DynamicMethod("get_" + propertyName, typeof(object), new[] { type }, type);
            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);


            var property = type.GetProperty(propertyName);
            iLGenerator.Emit(OpCodes.Callvirt, property.GetMethod);


            if (property.PropertyType.IsValueType)
            {
                // 如果是值类型，装箱
                iLGenerator.Emit(OpCodes.Box, property.PropertyType);
            }
            else
            {
                // 如果是引用类型，转换
                iLGenerator.Emit(OpCodes.Castclass, property.PropertyType);
            }


            iLGenerator.Emit(OpCodes.Ret);


            return dynamicMethod.CreateDelegate(typeof(Func<T, object>)) as Func<T, object>;
        }
        public static Action<T, object> EmitSetter<T>(string propertyName)
        {
            var type = typeof(T);


            var dynamicMethod = new DynamicMethod("EmitCallable", null, new[] { type, typeof(object) }, type.Module);
            var iLGenerator = dynamicMethod.GetILGenerator();


            var callMethod = type.GetMethod("set_" + propertyName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            var parameterInfo = callMethod.GetParameters()[0];
            var local = iLGenerator.DeclareLocal(parameterInfo.ParameterType, true);


            iLGenerator.Emit(OpCodes.Ldarg_1);
            if (parameterInfo.ParameterType.IsValueType)
            {
                // 如果是值类型，拆箱
                iLGenerator.Emit(OpCodes.Unbox_Any, parameterInfo.ParameterType);
            }
            else
            {
                // 如果是引用类型，转换
                iLGenerator.Emit(OpCodes.Castclass, parameterInfo.ParameterType);
            }


            iLGenerator.Emit(OpCodes.Stloc, local);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldloc, local);


            iLGenerator.EmitCall(OpCodes.Callvirt, callMethod, null);
            iLGenerator.Emit(OpCodes.Ret);


            return dynamicMethod.CreateDelegate(typeof(Action<T, object>)) as Action<T, object>;
        }

        [TestMethod]
        public void Write()
        {
            using (StreamWriter sw = new StreamWriter(@"C:\Users\fhjun\Desktop\TestReport2\data\6e5dfe84-a361-4f28-a985-88784e4fbc55.js"))
            {
                sw.WriteLine("var __data = [");
                for (int i = 0; i < 100000; i++)
                {
                    if (i != 0)
                    {
                        sw.Write(",");
                    }
                    sw.Write(@"
{
  '$type': 'XLY.SF.Project.Domains.Call, XLY.SF.Project.Domains',
  'ContactName': '张三_1(10086)',
  'Number': '第" + i + @"行',
  'Name': '张三_1',
  'StartDate': null,
  '_StartDate': '',
  'DurationSecond': 10000,
  'DurationSecondDesc': '24640',
  'LocationInfo': '- ',
  'Type': 0,
  'TypeDesc': 'None',
  'City': null,
  'Province': '四川省',
  'Country': null,
  'Operator': '哈哈都是风景是的',
  'Dynamic': null,
  'Longitude': 0.0,
  'Latitude': 0.0,
  'EndDate': null,
  'LastContactDate': null,
  'DataState': 2,
  'DataStateDesc': '',
  'MD5': '6825dd7f5b8b95dd0a67155b05b9f16d',
  'BookMarkId': -1,
  'IsVisible': true,
  'IsSensitive': false
}
");

                }
                sw.WriteLine("];var __columns = [{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'ContactName','title':'ContactName','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'Number','title':'Number','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'Name','title':'Name','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'StartDate','title':'StartDate','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'_StartDate','title':'_StartDate','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'DurationSecond','title':'DurationSecond','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'DurationSecondDesc','title':'DurationSecondDesc','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'LocationInfo','title':'LocationInfo','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'Type','title':'Type','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'TypeDesc','title':'TypeDesc','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'DataStateDesc','title':'DataStateDesc','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'MD5','title':'MD5','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'BookMarkId','title':'BookMarkId','align':null,'valign':null,'width':null,'sortable':true},{'$type':'XLY.SF.Project.DataReport.JsonExportColumn, XLY.SF.Project.DataReport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','field':'IsVisible','title':'IsVisible','align':null,'valign':null,'width':null,'sortable':true}];");
            }
        }
        #endregion

        #region 测试ChangeType
        [TestMethod]
        public void TestChangeType()
        {

            object r1 = Convert.ChangeType("234", typeof(string));
            object r2 = Convert.ChangeType("234", typeof(int));
            object r3 = Convert.ChangeType("234", typeof(uint));
            object r4 = Convert.ChangeType("234", typeof(double));
            object r5 = "Android".ChangeType(typeof(EnumOSType));// Convert.ChangeType("Android", typeof(EnumOSType));
            object r6 = "2".ChangeType(typeof(EnumOSType)); //Convert.ChangeType("2", typeof(EnumOSType));
            object r7 = Convert.ChangeType("true", typeof(bool));
            object r8 = Convert.ChangeType("True", typeof(bool));

            //var dic = AbstractDevice.Save(new Device() { Name = "张三", SerialNumber = "8438583454", ID = "333333333" });
            //IDevice d = AbstractDevice.Load(dic);
        }
        #endregion

        #region 测试USB连接
        [TestMethod]
        public void TestUSB()
        {
            //string UsbExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\USBMonitorService.exe");
            //int rt = USBMonitorCoreDll.Initialize(Encoding.Unicode.GetBytes(UsbExePath));
            //Assert.AreEqual(rt, 0);


        }
        #endregion

        #region 插件序列化和反序列化

        /// <summary>
        /// 插件序列化和反序列化
        /// </summary>	
        [TestMethod]
        public void TestPluginSerizal()
        {
            Log("-----------开始测试插件序列化和反序列化----------------");
            DataViewPluginInfo pi = new DataViewPluginInfo()
            {
                Guid = "{8B8D2903-AAE7-449C-B422-1B6FE625ABA9}",
                Name = "插件1",
                PluginType = PluginType.SpfDataView,
                VersionStr = "1.0.0.1",
                ViewType = new List<DataViewSupportItem>()
                  {
                       new DataViewSupportItem(){ PluginName = "微信", PluginId = "微信ID", TypeName = "MessageCore"},
                       new DataViewSupportItem(){ PluginName = "短信", PluginId = "短信ID", TypeName = "Message2"}
                  }
            };

            Serializer.SerializeToXML(pi, @"C:\Users\fhjun\Desktop\123.xml");
        }
        #endregion

        #region 数据Json格式化速度

        /// <summary>
        /// 数据Json格式化速度
        /// </summary>	
        [TestMethod]
        public void TestItemsToJsonSpeed()
        {
            Log("-----------开始测试数据Json格式化速度----------------");
            int count = 100000;
            IDataItems items = new DataItems<MessageCore>(@"C:\Users\fhjun\Desktop\123.db");
            for (int i = 0; i < count; i++)
            {
                items.Add(new MessageCore() { Content = "发送的消息的内容是的俄日文2342！32<>^*", Date = DateTime.Now.AddDays(i), SenderName = "张三", MessageType = "文本" });
            }
            items.Commit();
            items.Filter();
            Stopwatch t = new Stopwatch();
            t.Start();
            using (StreamWriter sw = new StreamWriter(@"C:\Users\fhjun\Desktop\123.js", false, Encoding.UTF8))
            {
                sw.Write("var __data = [");
                int r = 0;
                foreach (var c in items.View)
                {
                    if (r != 0)
                        sw.Write(",");
                    sw.Write(Serializer.JsonSerilize(c));
                    r++;
                }
                sw.Write("];");
            }
            Log($"执行时间：{t.ElapsedMilliseconds}ms");//6.5s--100000数据
        }
        #endregion

        #region 数据修改后更新数据库

        /// <summary>
        /// 数据修改后更新数据库
        /// </summary>	
        [TestMethod]
        public void TestDataItemPropertyModify()
        {
            Log("-----------开始测试数据修改后更新数据库----------------");

            string db = @"C:\Users\fhjun\Desktop\TestDataItemPropertyModify.db";
            string db2 = db.Insert(db.LastIndexOf('.'), "_bmk");
            if (File.Exists(db))
            {
                File.Delete(db);
            }
            if (File.Exists(db2))
            {
                File.Delete(db2);
            }
            IDataItems items = new DataItems<MessageCore>(db);
            //MessageCore mm3 = null;
            //for (int i = 0; i < 1; i++)
            //{
            //    MessageCore mm = new MessageCore() { Content = "正常消息", Date = DateTime.Now.AddDays(i), SenderName = "张三", MessageType = "文本" };
            //    mm.Content = "正常消息1";
            //    items.Add(mm);
            //    if (i == 3)
            //        mm.Content = "这是修改Content后的数据";
            //    if (i % 3 == 0)
            //        mm.BookMarkId = 2;
            //    if (i == 0)
            //        mm3 = mm;
            //}
            MessageCore mm = new MessageCore() { Content = "正常消息", Date = DateTime.Now.AddDays(0), SenderName = "张三", MessageType = "文本" };
            mm.Content = "正常消息1";
            items.Add(mm);
            mm.Content = "这是修改Content后的数据";
            mm.Content = "这是修改Content后的数据2222";

            items.Commit();
            items.Filter();

            mm.BookMarkId = 3;
            mm.BookMarkId = -1;
        }
        #endregion

        #region 数据的序列化和反序列化

        /// <summary>
        /// 数据的序列化和反序列化
        /// </summary>	
        [TestMethod]
        public void TestDataSeriazal()
        {
            Log("-----------开始测试数据的序列化和反序列化----------------");

            string DB_PATH = DeskPath("TestDataSeriazal.db");

            //var DataSource = new TreeDataSource();
            //DataSource.TreeNodes = new List<TreeNode>();
            //for (int i = 0; i < 2; i++)
            //{
            //    TreeNode t = new TreeNode();
            //    t.Text = "账号" + i;
            //    DataSource.TreeNodes.Add(t);

            //    TreeNode accouts = new TreeNode();
            //    accouts.Text = "好友列表";
            //    accouts.IsHideChildren = true;
            //    t.TreeNodes.Add(accouts);
            //    accouts.Type = typeof(WeChatFriendShow);
            //    accouts.Items = new DataItems<WeChatFriendShow>(DB_PATH);
            //    for (int j = 0; j < 10; j++)
            //    {
            //        accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j });
            //    }

            //    TreeNode accouts2 = new TreeNode();
            //    accouts2.Text = "聊天记录";
            //    accouts2.IsHideChildren = i % 2 == 0;
            //    accouts2.Type = typeof(WeChatFriendShow);
            //    accouts2.Items = new DataItems<WeChatFriendShow>(DB_PATH);
            //    t.TreeNodes.Add(accouts2);
            //    for (int j = 0; j < 5; j += 2)
            //    {
            //        accouts2.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j });
            //        TreeNode friend = new TreeNode();
            //        friend.Text = "昵称" + j;
            //        friend.Type = typeof(MessageCore);
            //        friend.Items = new DataItems<MessageCore>(DB_PATH);
            //        accouts2.TreeNodes.Add(friend);

            //        for (int k = 0; k < 100; k++)
            //        {
            //            MessageCore msg = new MessageCore() { SenderName = friend.Text, SenderImage = "images/zds.png", Receiver = t.Text, Content = "消息内容" + k, MessageType = k % 4 == 0 ? "图片" : "文本", SendState = EnumSendState.Send };
            //            friend.Items.Add(msg);
            //            MessageCore msg2 = new MessageCore() { Receiver = friend.Text, SenderImage = "images/zjq.png", SenderName = t.Text, Content = "返回消息内容" + k, MessageType = k % 5 == 0 ? "图片" : "文本", SendState = EnumSendState.Receive };
            //            friend.Items.Add(msg2);
            //        }
            //    }

            //    TreeNode accouts3 = new TreeNode();
            //    accouts3.Text = "群消息";
            //    accouts3.IsHideChildren = true;
            //    t.TreeNodes.Add(accouts3);

            //    TreeNode accouts4 = new TreeNode();
            //    accouts4.Text = "发现";
            //    accouts4.IsHideChildren = true;
            //    t.TreeNodes.Add(accouts4);
            //}
            //DataSource.BuildParent();

            //System.IO.File.WriteAllText(DB_PATH + ".js", Serializer.JsonSerilize(DataSource));

            //var source2 = Serializer.JsonDeserilize<TreeDataSource>(System.IO.File.ReadAllText(DB_PATH + ".js"));
            //source2.BuildParent();
            //var dsss = source2.TreeNodes[0].TreeNodes[0].Items;
            //var t = source2.TreeNodes[0].TreeNodes[0].Total;
            ////dsss.Filter();
            //foreach (AbstractDataItem i in dsss.View)
            //{
            //    i.BookMarkId = 5;
            //    Console.WriteLine($"1111111111111"); 
            //}

            SimpleDataSource ds = new SimpleDataSource();
            ds.Type = typeof(SMS);
            ds.Items = new DataItems<SMS>(DB_PATH);
            for (int i = 0; i < 200; i++)
            {
                ds.Items.Add(new SMS() { Content = "内容" + i });
            }
            ds.BuildParent();

            ds.Filter<dynamic>();
            List<string> ls = new List<string>();
            foreach (SMS item in ds.Items.View)
            {
                ls.Add(item.Content);
            }
            List<string> ls2 = new List<string>();
            foreach (SMS item in (ds.Items as DataItems<SMS>).ViewAll)
            {
                ls2.Add(item.Content);
            }
            Assert.AreEqual(ls2.Count, 200);
            Assert.AreEqual(ls.Count, 200);
        }
        #endregion

        #region DataEntity定义的数据转换为多语言xml

        /// <summary>
        /// DataEntity定义的数据转换为多语言xml
        /// </summary>	
        [TestMethod]
        public void TestDataEntity2LanguageXml()
        {
            //var a = DeviceExternsion.LoadDeviceData(@"C:\Users\fhjun\Desktop\默认案例_20171115[081055]\默认案例_20171115[081055]\R7007_20171115[081055]");

            Log("-----------开始测试DataEntity定义的数据转换为多语言xml----------------");
            Stream sm = typeof(IDataSource).Assembly.GetManifestResourceStream("XLY.SF.Project.Domains.Language.Language_Cn.xml");
            XDocument doc = XDocument.Load(sm);
            XElement DataEntityLanguage = doc.Element("LanguageResource").Element("DataEntityLanguage");
            if (DataEntityLanguage == null)
            {
                DataEntityLanguage = new XElement("DataEntityLanguage", new XAttribute("Prompt", "数据定义语言资源"));
                doc.Element("LanguageResource").Add(DataEntityLanguage);
            }
            foreach (var type in typeof(IDataSource).Assembly.GetTypes())
            {
                foreach (var property in type.GetProperties())
                {
                    var attr = property.GetCustomAttribute<DisplayAttribute>();
                    if (attr != null)
                    {
                        string langkey = !string.IsNullOrWhiteSpace(attr.Key) ? attr.Key : $"{type.Name}_{property.Name}";  //没有设置语言Key，则默认为“类名_属性名”

                        if (DataEntityLanguage.Element(langkey) == null)
                                DataEntityLanguage.Add(new XElement(langkey, $"{attr.Key ?? property.Name}"));
                    }
                }
            }
            doc.Save(DeskPath(@"Language_Cn.xml"));
        }
        #endregion

    }
}
