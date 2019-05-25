using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Company_Review.core.Converter
{
    class XMLSerializerWrapper
    {
        public static Object deserialize(Type t, String filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            XmlSerializer serializer = new XmlSerializer(t);
            Object returnObject = serializer.Deserialize(reader);
            reader.Close();
            return returnObject;
        }

        public static void serialize(Type t, String filePath, Object pDataObject)
        {
            StreamWriter writer = new StreamWriter(filePath);
            XmlSerializer serializer = new XmlSerializer(t);
            serializer.Serialize(writer, pDataObject);
            writer.Flush();
            writer.Close();
        }
    }
}
