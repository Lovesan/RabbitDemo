using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace RabbitDemo.Common
{
    /// <summary>
    /// Helper class for serializing objects into bytes
    /// </summary>
    internal static class SerializerHelper
    {
        private const int BufferSize = 4096;

        private static readonly JsonSerializerSettings Settings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.None
            };

        private static readonly Encoding Encoding = new UTF8Encoding(false, false);

        /// <summary>
        /// Serializes an object to a byte array
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>Serialized object</returns>
        public static byte[] Write<T>(T obj)
        {
            var serializer = JsonSerializer.Create(Settings);
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms, Encoding, BufferSize, true))
                {
                    serializer.Serialize(writer, obj);
                }
                var buffer = new byte[ms.Length];
                Array.Copy(ms.GetBuffer(), 0, buffer, 0, ms.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Deserializes an object from a byte array
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="data">Serialized object</param>
        /// <param name="offset">Byte array offset</param>
        /// <param name="count">Byte count</param>
        /// <returns>Deserialized object</returns>
        public static T Read<T>(byte[] data, int offset, int count)
        {
            var serializer = JsonSerializer.Create(Settings);
            using (var ms = new MemoryStream(data, offset, count, false))
            using (var reader = new StreamReader(ms, Encoding, true))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
