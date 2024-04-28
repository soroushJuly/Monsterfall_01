using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System.IO;
using Monsterfall_01.Engine;


namespace Monsterfall_01
{
    internal class MapData
    {
        // List of decorations extracted from text file
        List<Decoration> decorations;
        // Map size extracted from txt file
        Vector2 mapSize;
        // Shop (upgrades) size extracted from txt file
        Point shopLocation;

        public List<Decoration> GetDecorations() { return decorations; }
        public Vector2 GetMapSize() { return mapSize; }
        public Point GetShopLocation() { return shopLocation; }
        public MapData()
        {
            decorations = new List<Decoration>();
            mapSize = Vector2.Zero;
            shopLocation = Point.Zero;
        }
        public void ReadMapData(ContentManager Content, int levelIndex)
        {
            // List of lines read from text file
            List<string> lines = new List<string>();
            string levelPath = string.Format("Content\\Maps\\{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
            {
                bool isDecoraitons = false;
                // loader helper class to read files
                Loader loader = new Loader(fileStream);
                lines = loader.ReadLinesFromTextFile();
                foreach (string line in lines)
                {
                    if (isDecoraitons)
                    {
                        // break after seeing the second "Decorations"
                        if (line == "Decorations")
                        {
                            break;
                        }
                        string[] widthLine = line.Split(":");
                        // name of the decoration tile in the image file
                        String name = widthLine[0];
                        String point = widthLine[1];
                        string[] coords = point.Split(",");
                        Vector2 location = new Vector2(int.Parse(coords[0]), int.Parse(coords[1]));
                        string path = string.Format("Graphics\\Env\\Dungeon\\{0}", name);
                        Texture2D decorTexture = Content.Load<Texture2D>(path);
                        decorations.Add(new Decoration(location, decorTexture));
                        continue;
                    }
                    if (line.Contains("Shop:"))
                    {
                        // Seperate the name and coordinates
                        string[] widthLine = line.Split(":");
                        string point = widthLine[1];
                        string[] coords = point.Split(",");
                        shopLocation = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                    }
                    if (line.Contains("Width:"))
                    {
                        // Seperate the name and value
                        string[] widthLine = line.Split(":");
                        mapSize.X = int.Parse(widthLine[1]);
                    }
                    if (line.Contains("Height:"))
                    {
                        // Seperate the name and value
                        string[] heightLine = line.Split(":");
                        mapSize.Y = int.Parse(heightLine[1]);
                    }
                    if (line.Contains("Decorations"))
                    {
                        // if decoration read the next line
                        isDecoraitons = true;
                    }
                }
            }
        }
    }
}
