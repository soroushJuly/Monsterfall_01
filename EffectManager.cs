using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine;

namespace Monsterfall_01
{
    // A class to manage effects in game
    internal class EffectManager
    {
        // List of all effects currently in the game
        private List<Animation> effects;
        // Map of each effect with its animation
        private Dictionary<string, Texture2D> animationTextures;
        public EffectManager() 
        {
            animationTextures = new Dictionary<string, Texture2D>();
            effects = new List<Animation>();
        }
        // Add an animation texture to animation map
        public void AddAnimation(string name, Texture2D texture)
        {
            animationTextures[name] = texture;
        }
        public void Update(GameTime gameTime)
        {
            foreach (var animation in effects)
            {
                animation.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var animation in effects)
            {
                animation.Draw(spriteBatch);
            }
        }

        internal void AddBloodEffect(object sender, Vector2 e)
        {
            effects.Add(new Animation(animationTextures["Blood"], e, 512, 512,
                14, 40, Color.White, 0.3f, false, 4));
        }
        internal void AddPowerUpEffect(object sender, Vector2 e)
        {
            effects.Add(new Animation(animationTextures["PowerUp"], e, 192, 192,
                20, 45, Color.White, 1.0f, false, 5));
        }
    }
}
