﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Monsterfall_01
{
    public class Arrow : Collidable
    {
        public Texture2D arrowTexture;
        // the speed that arrow travels  
        float arrowMoveSpeed;

        public Vector2 Position;
        // Arrow initial position (when created)
        public Vector2 firstPosition;
        // Origin of the rotation
        private Vector2 origin;
        // Scale of arrow texture
        const float SCALE = 0.25f;

        // Arrow Damage power
        public readonly int Damage = 100;
        // Angle between arrow and (0,1) vector
        private double angle;
        // Arrow range.  
        int range = 550;

        private int GetWidth() { return (int)(SCALE * Game1.arrowTexture.Width); }
        private int GetHeight() { return (int)(SCALE * Game1.arrowTexture.Height); }

        public Arrow(Vector2 position, int directionIndex)
        {
            this.Position = position;
            this.firstPosition = position;

            this.angle = (directionIndex * (Math.PI / 4));
            this.arrowMoveSpeed = 8.5f;

            origin = Vector2.Zero;
        }
        public void Update(GameTime gameTime)
        {
            // Bounding box is a small box on tip of the arrow
            this.box = new Rectangle((int)(Position.X), 
                (int)(Position.Y ), 10, 10);
            Position.X += (float)(arrowMoveSpeed * Math.Sin(angle));
            Position.Y -= (float)(arrowMoveSpeed * Math.Cos(angle));

            // Delete if it goes out of range
            if (Vector2.Distance(firstPosition, Position) > range)
                flagForRemoval = true;
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, GetWidth(), GetHeight());
            Rectangle src = new Rectangle(0,0, Game1.arrowTexture.Width, Game1.arrowTexture.Height);

            DrawBoundingBox(spriteBatch, graphicsDevice);
            spriteBatch.Draw(Game1.arrowTexture, dest, src, Color.White, (float)angle, origin, SpriteEffects.None, 0f);
        }
        public override bool CollisionTest(Collidable obj)
        {
            if (this.Intersects(obj))
                return true;
            return false;
        }

        public override void OnCollision(Collidable obj)
        {
            Enemy enemy = obj as Enemy;
            if (enemy != null)
            {
                flagForRemoval = true;
            }
        }
    }
}
