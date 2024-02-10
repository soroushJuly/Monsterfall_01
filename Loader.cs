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
    }
}
