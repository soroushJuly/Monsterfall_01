using System;
using System.Collections.Generic;
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

        private List<ShopItem> shopItems;
        public List<ShopItem> ShopItems { get { return shopItems; } }

        public void Initialize(Vector2 mapSize, ContentManager content, List<Decoration> decorations,
            List<ShopItem> shopItems, Point shopLocation)
        {
            this.mapSize = mapSize;
            this.decorTiles = new List<Tile>();
            this.shopItems = shopItems;

            mapOffset = new Point(0, 0);
            tiles = new Tile[(int)mapSize.X, (int)mapSize.Y];

            Texture2D tileTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stone_E1");
            tileSize.X = tileTexture.Width;
            tileSize.Y = tileTexture.Height;

            AddMapWalls(content);
           
            for (int j = 1; j < mapSize.Y; j++)
            {
                for(int i = 1; i < mapSize.X; i++)
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
            for (int i = 0; i < shopItems.Count; i++)
            {
                shopItems[i].Initialize(MapToScreen((int)shopLocation.X, (int)shopLocation.Y + i));
            }
        }
        // Add the walls surrounding the map
        private void AddMapWalls(ContentManager content)
        {
            Texture2D wallEastTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stoneWall_E");
            Texture2D wallWestTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stoneWall_W");
            Texture2D wallNorthTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stoneWall_N");
            Texture2D wallSouthTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stoneWall_S");
            // Facing South, North
            Texture2D cornerSouthTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stoneWallCorner_S");
            Texture2D cornerNorthTexture = content.Load<Texture2D>("Graphics\\Env\\Dungeon\\stoneWallCorner_N");

            // First corners as other walls covering them
            decorTiles.Add(new Tile(cornerSouthTexture, MapToScreen(0, 0), true, .5f, 0.8f));

            // Top left walls
            for (int j = 1; j < mapSize.Y - 1; j++)
                decorTiles.Add(new Tile(wallEastTexture, MapToScreen(0, j), true, .6f, 0.8f));
            // Top right walls
            for (int i = 1; i < mapSize.X - 1; i++)
                decorTiles.Add(new Tile(wallSouthTexture, MapToScreen(i, 0), true, .6f, 0.6f, 50));

            // Bottom right walls
            for (int j = 1; j < mapSize.Y - 1; j++)
                decorTiles.Add(new Tile(wallWestTexture, MapToScreen((int)(mapSize.X - 1), j), true, .6f, 0.3f, 0 , 100));
            // Bottom Left walls
            for (int i = 1; i < mapSize.X - 1; i++)
                decorTiles.Add(new Tile(wallNorthTexture, MapToScreen(i, (int)(mapSize.Y - 1)), true, .7f, 0.25f, 0, 70));

            // South corner (Facing north)
            decorTiles.Add(new Tile(cornerNorthTexture, MapToScreen((int)(mapSize.X - 1), (int)(mapSize.Y - 1)), true, .5f, 0.7f));
        }

        private Vector2 MapToScreen(int x, int y)
        {
            // To map the tile toghether in ISOMETRIC way
            var screenX = x * tileSize.X / 2 - y * (tileSize.X / 2) + mapOffset.X;
            var screenY = y * (tileSize.Y / 2 - 10) + x * (tileSize.Y / 2 - 10) + mapOffset.Y;

            return new Vector2(screenX, screenY);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevices, SpriteFont font)
        {
            for (int j = 1; j < mapSize.Y; j++)
            {
                for (int i = 1; i < mapSize.X; i++)
                {
                    tiles[i, j].Draw(spriteBatch, GraphicsDevices);
                }
            }
            foreach(Tile tile in decorTiles)
            {
                tile.Draw(spriteBatch, GraphicsDevices);
            }
            foreach (ShopItem shopItem in shopItems)
            {
                shopItem.Draw(spriteBatch, font);
            }
        }

    }
}
