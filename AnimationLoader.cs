using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine;

namespace Monsterfall_01
{
    internal class AnimationLoader
    {
        // Load and add all the animations related to one state
        public void LoadAnimations(ContentManager Content, String BasePath, float Scale, List<Animation> Animations, int size, int framecount, int frametime, 
            int directioncount, int cols, bool isLooping = true)
        {
            for (int i = 0; i < directioncount; i++)
            {
                // Create texture with base path and the degree calculated by the function
                Texture2D playerTexture = Content.Load<Texture2D>(createTexturePath(BasePath, i, directioncount));
                Animation animation = new Animation(playerTexture, Vector2.Zero, size, size, framecount, frametime, Color.White, Scale, isLooping, cols);
                Animations.Add(animation);
            }
        }
        // Creating texture path string based on number of directions
        private String createTexturePath(String basePath, int i, int directionCount)
        {
            float degree = i * (360 / (float)directionCount);
            String degreePath;
            if (degree / 10 < 1) { degreePath = "000"; }
            else if (degree / 100 < 1) { degreePath = "0" + ((int)degree).ToString(); }
            else { degreePath = ((int)degree).ToString(); }
            // Full path = basePath + DegreePath
            return basePath + degreePath;

        }
    }
}
