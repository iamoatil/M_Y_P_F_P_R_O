using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XLY.SF.Project.CameraView
{
    /// <summary>
    /// 提供各种序列化方法
    /// </summary>
    public static class Serializer
    {

        #region 将对象序列化到二进制文件中
        /// <summary>
        /// 将对象序列化到二进制文件中
        /// </summary>
        /// <param name="instance">要序列化的对象</param>
        /// <param name="fileName">文件名，保存二进制序列化数据的位置,后缀一般为.bin</param>
        public static void SerializeToBinary(object instance, string fileName)
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            String directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            //创建文件流
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                //开始序列化对象
                serializer.Serialize(fs, instance);
            }
        }

        /// <summary>
        /// 将对象序列化为二进制数据
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(object instance)
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            //创建文件流
            using (MemoryStream _memory = new MemoryStream())
            {
                //开始序列化对象
                serializer.Serialize(_memory, instance);

                _memory.Position = 0;
                byte[] read = new byte[_memory.Length];
                _memory.Read(read, 0, read.Length);
                _memory.Close();
                return read;
            }
        }

        #endregion

        #region 将二进制文件反序列化为对象
        /// <summary>
        /// 将二进制文件反序列化为对象
        /// </summary> <typeparam name="T">要获取的类</typeparam>
        /// <param name="fileName">文件名，保存二进制序列化数据的位置</param>        
        public static T DeSerializeFromBinary<T>(string fileName) where T : class
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            //创建文件流
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,FileShare.Read))
            {
                //开始反序列化对象-
                return serializer.Deserialize(fs) as T;
            }
        }

        /// <summary>
        /// 将二进制数据反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T DeSerializeFromBinary<T>(byte[] buffer) where T : class
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            //创建文件流
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.Position = 0;

                //开始反序列化对象-
                return serializer.Deserialize(ms) as T;
            }
        }

        #endregion
        

    }
}
