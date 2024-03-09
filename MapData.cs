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
        private List<Decoration> decorations;
        private Vector2 mapSize;
        private Point shopLocation;
        public MapData() 
        { 
            decorations = new List<Decoration>();
            mapSize = Vector2.Zero;
            shopLocation = Point.Zero;
        }
        // TODO: clean this method
        public void ReadMapData(List<string> lines, ContentManager Content, int levelIndex)
        {
            //List<Decoration> decorations = new List<Decoration>();
            string levelPath = string.Format("Content\\Maps\\{0}.txt", levelIndex);
            int width = 0;
            int height = 0;
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
            {
                bool isDecoraitons = false;
                Loader loader = new Loader(fileStream);
                lines = loader.ReadLinesFromTextFile();
                foreach (string line in lines)
                {
                    if (isDecoraitons)
                    {
                        if (line == "Decorations")
                        {
                            break;
                        }
                        string[] widthLine = line.Split(":");
                        String title = widthLine[0];
                        String point = widthLine[1];
                        string[] coords = point.Split(",");
                        Vector2 location = new Vector2(int.Parse(coords[0]), int.Parse(coords[1]));
                        string path = string.Format("Graphics\\Env\\Dungeon\\{0}", title);
                        Texture2D decorTexture = Content.Load<Texture2D>(path);
                        decorations.Add(new Decoration(location, decorTexture));
                        continue;
                    }
                    if (line.Contains("Shop:"))
                    {
                        string[] widthLine = line.Split(":");
                        string point = widthLine[1];
                        string[] coords = point.Split(",");
                        shopLocation = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                    }
                    if (line.Contains("Width:"))
                    {
                        string[] widthLine = line.Split(":");
                        width = int.Parse(widthLine[1]);
                    }
                    if (line.Contains("Height:"))
                    {
                        string[] heightLine = line.Split(":");
                        height = int.Parse(heightLine[1]);
                    }
                    if (line.Contains("Decorations"))
                    {
                        isDecoraitons = true;
                    }
                }
            }
            mapSize = new Vector2(width, height);

        }
        public List<Decoration> GetDecorations() { return decorations; }
        public Vector2 GetMapSize() { return mapSize; }
        public Point GetShopLocation() { return shopLocation; }
    }
}
