using IdentityModel;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Common.Infrastructure.Extensions
{
    public static class StringHelper
    {
        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
        public static string Sha256(this string objectData)
        {
            return objectData.ToSha256();
        }
        public static string Base64(this string objectData)
        {
            return Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(objectData));
        }
        public static string DecodeBase64(this string objectData)
        {
            return UTF8Encoding.UTF8.GetString(Convert.FromBase64String(objectData));
        }
    }
}