using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Common.Net
{
    public static class BinarySerializer
    {
        public static byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return (T)new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}
