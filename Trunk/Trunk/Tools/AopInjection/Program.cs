using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AopInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllPath = args != null && args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory; //当前扫描的目录
            bool isComfirmWhenExit = args != null && args.Length > 1 ? args[1].Equals("true", StringComparison.OrdinalIgnoreCase) : true;       //结束时是否等待确认后再退出

            Injection(dllPath);

            if (isComfirmWhenExit)
            {
                Console.WriteLine("执行完成，按任意键退出...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 向dll或exe文件中注入方法
        /// </summary>
        /// <param name="dllPath"></param>
        private static void Injection(string dllPath)
        {
            Console.WriteLine($"扫描文件夹{dllPath}, 开始植入IL代码...");

            var filelist = new DirectoryInfo(dllPath).GetFiles("*.*", SearchOption.AllDirectories).
                Where(f => f.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) || f.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase));
            foreach (var file in filelist)
            {
                try
                {
                    Console.WriteLine($"开始植入IL代码：{file.FullName}");
                    bool isChanged = false;
                    AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(file.FullName);
                    foreach (ModuleDefinition module in asm.Modules)
                    {
                        foreach (TypeDefinition type in module.Types)
                        {
                            if (!IsAssignFromInterface(type, "IAOPPropertyChangedMonitor"))     //该类必须继承IAOPPropertyChangedMonitor接口
                            {
                                continue;
                            }
                            var propertyChanged = GetMethod(type, "OnPropertyValueChanged");         //获取属性改变事件
                            foreach (var p in type.Methods.Where(m => m.Name.StartsWith("set_")))       //获取所有属性的set方法
                            {
                                ReWriteProperties(p, propertyChanged, p.Parameters[0].ParameterType);
                                isChanged = true;
                            }
                        }
                    }
                    if(isChanged)
                    {
                        asm.Write(file.FullName, new WriterParameters() { WriteSymbols = false });
                    }
                    Console.WriteLine($"成功植入IL代码：{file.FullName}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"植入IL代码错误：{e.Message}");
                }
            }
        }

        /// <summary>
        /// 判断该类型是否继承自某个接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        private static bool IsAssignFromInterface(TypeDefinition type, string interfaceName)
        {
            if (type == null)
            {
                return false;
            }
            if (type.Interfaces.FirstOrDefault(i => i.Name == interfaceName) != null)
            {
                return true;
            }
            return IsAssignFromInterface(type.BaseType as TypeDefinition, interfaceName);
        }

        /// <summary>
        /// 获取某个方法，如果没有则在其父类中查询
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private static MethodDefinition GetMethod(TypeDefinition type, string methodName)
        {
            if (type == null)
            {
                return null;
            }
            var mtd = type.Methods.FirstOrDefault(m => m.Name.Equals(methodName));
            if (mtd == null)
            {
                return GetMethod(type.BaseType as TypeDefinition, methodName);
            }
            return mtd;
        }

        /// <summary>
        /// 重写属性的set方法，在最后加入propertyChanged执行方法
        /// </summary>
        /// <param name="setMethod"></param>
        /// <param name="propertyChanged"></param>
        /// <param name="propertyType"></param>
        private static void ReWriteProperties(MethodDefinition setMethod, MethodReference propertyChanged, TypeReference propertyType)
        {
            ILProcessor iL = setMethod.Body.GetILProcessor();
            var ins = setMethod.Body.Instructions[setMethod.Body.Instructions.Count - 1];
            iL.InsertBefore(ins, iL.Create(OpCodes.Ldarg_0));
            iL.InsertBefore(ins, iL.Create(OpCodes.Ldarg_1));
            if (propertyType.IsValueType)       //如果是值类型，则先装箱
            {
                iL.InsertBefore(ins, iL.Create(OpCodes.Box, propertyType));
            }
            iL.InsertBefore(ins, iL.Create(OpCodes.Ldstr, setMethod.Name.Remove(0, 4)));        //移除方法名称前面的set_
            iL.InsertBefore(ins, iL.Create(OpCodes.Call, propertyChanged));     //执行propertyChanged方法
            iL.InsertBefore(ins, iL.Create(OpCodes.Nop));

            Console.WriteLine($"成功修改属性：{setMethod.Name.Remove(0, 4)}");
        }
    }
}
