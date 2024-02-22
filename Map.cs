using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Monsterfall_01
{
    internal class Map
    {
        private Vector2 mapSize;
        private Point mapOffset;
        private Point tileSize;
        private Tile[,] tiles;
        private List<Tile> decorTiles;
        public List<Tile> DecorTiles { get { return decorTiles; } }
        private List<Decoration> decorations;

        public void Initialize(Vector2 mapSize, ContentManager content, List<Decoration> decorations)
        {
            this.decorations = decorations;
            this.mapSize = mapSize;
            decorTiles = new List<Tile>();
            //mapOffset = new Point(mapSize.X * 100, 0);
            mapOffset = new Point(0, 0);
            tiles = new Tile[(int)mapSize.X, (int)mapSize.Y];

            Texture2D tileTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stone_E1");
            tileSize.X = tileTexture.Width;
            tileSize.Y = tileTexture.Height;

            for(int j = 0; j < mapSize.Y; j++)
            {
                for(int i = 0; i < mapSize.X; i++)
                {
                    Tile tile = new Tile();
                    tiles[i, j] = tile.Initialize(tileTexture, MapToScreen(i, j));
                }
            }
            foreach (Decoration decoration in decorations)
            {
                Tile tile = new Tile(decoration.texture, MapToScreen((int)decoration.location.X, (int)decoration.location.Y), true);
                decorTiles.Add(tile);
            }
        }

        private Vector2 MapToScreen(int x, int y)
        {
            // This code works only for ground tiles right now
            var screenX = x * tileSize.X / 2 - y * (tileSize.X / 2) + mapOffset.X;
            var screenY = y * (tileSize.Y / 2 - 10) + x * (tileSize.Y / 2 - 10) + mapOffset.Y;

            return new Vector2(screenX, screenY);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices)
        {
            for (int j = 0; j < mapSize.Y; j++)
            {
                for (int i = 0; i < mapSize.X; i++)
                {
                    tiles[i, j].Draw(spriteBatch, GraphicsDevices);
                }
            }
            foreach(Tile tile in decorTiles)
            {
                tile.Draw(spriteBatch, GraphicsDevices);
            }
        }

    }
}
