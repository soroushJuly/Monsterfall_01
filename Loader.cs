using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Monsterfall_01
{
    internal class Loader
    {
        private Stream mFileStream;
        public Loader() { }
        public Loader(Stream stream)
        {
            mFileStream = stream;
        }
        public List<string> ReadLinesFromTextFile()
        {
            // Don't have to worry about string building here, as we are only 
            // reading a line at a time 
            string line = "";

            // Initialise a list to contain the results 
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(mFileStream))
                {
                    // Now we'll keep reading until the end of the file and 
                    // store each line in a collection 
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Add the line to the collection 
                        lines.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: File could not be read!");
                Console.WriteLine("Exception Message: " + e.Message);
            }

            return lines;
        }
        public void ReadXML(string filename)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    GameInfo.Instance = (GameInfo)new XmlSerializer(typeof(GameInfo)).Deserialize(reader.BaseStream);
                }
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message 
                // describing the error 
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
        }
        //public void Save(string filename, object dataObj)
        //{

        //        System.IO.MemoryStream tempMemStream = new System.IO.MemoryStream();
        //        System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
        //        string tempString;

        //        settings.Indent = true;
        //        settings.Encoding = System.Text.Encoding.UTF8;
        //        settings.NewLineChars = System.Environment.NewLine;
        //        settings.NewLineHandling = System.Xml.NewLineHandling.Replace;

        //        using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
        //        {
        //            using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(stringWriter, settings))
        //            {
        //                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(p_object.GetType());
        //                serializer.Serialize(xmlWriter, p_object);

        //                tempString = stringWriter.ToString();
        //                xmlWriter.Close();
        //            }

        //            stringWriter.Close();
        //        }

        //    using (Stream stream = storageContainer.CreateFile(filename))
        //    {
        //        XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
        //        serializer.Serialize(stream, saveGameData);
        //    }

        //    return tempString;

        //}
    }
}
