using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
//using static System.Net.Mime.MediaTypeNames;

namespace Monsterfall_01
{
    public class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;
        float scale;
        // The time since we last updated the frame  
        int elapsedTime;
        // The time we display a frame until the next one  
        int frameTime;
        // The number of frames that the animation contains  
        int frameCount;
        // The index of the current frame we are displaying  
        int currentFrame;
        // Number of cols of images in the sprite
        int cols;
        // The color of the frame we will be displaying  
        Color color;
        // The area of the image strip we want to display  
        Rectangle sourceRect = new Rectangle();
        // The area where we want to display the image strip in the game  
        Rectangle destinationRect = new Rectangle();

        public int frameWidth;
        public int frameHeight;
        // The state of the Animation  
        public bool Active;
        // Determines if the animation will keep playing or deactivate after one run  
        public bool Looping;
        // Width of a given frame???? (same as FrameWidth??)  
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount,
            int frametime, Color color, float scale, bool looping, int cols = 1)
        {
            // Keep a local copy of the values passed in  
            this.color = color;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;
            this.cols = cols;
            Looping = looping;
            Position = position;
            spriteStrip = texture;
            // Set the time to zero elapsedTime = 0; 
            currentFrame = 0;
            // Set the Animation to active by default   
            Active = true;
        }
        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active  
            if (Active == false) return;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            // If the elapsed time is larger than the frame time   
            // we need to switch frames  
            if (elapsedTime > frameTime)
            {
                // Move to the next frame  
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero  
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation                       
                    if (Looping == false)
                        Active = false;
                }
                elapsedTime = 0;
            }
            // SourceRect and DestinationRect are about pixels
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
            if (cols == 1)
            {
                sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            }
            else
            {
                sourceRect = new Rectangle((currentFrame % cols) * frameWidth, (currentFrame / cols) * frameHeight, frameWidth, frameHeight);
            }
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width   
            destinationRect = new Rectangle((int)Position.X - (int)(frameWidth * scale) / 2,
                (int)Position.Y - (int)(frameHeight * scale) / 2, (int)(frameWidth * scale), (int)(frameHeight * scale));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active        
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}
