using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LittleFlame.Functions
{
    public class AnimatedTexture
    {
        private int framecount, Frame;
        private Texture2D myTexture;
        private float timePerFrame, totalElapsed;
        private bool paused;
        private float rotation, scale, depth;

        public Vector2 Origin { get; private set; }

        /// <summary>
        /// Create a new texture using a spritesheet.
        /// </summary>
        /// <param name="rotation">The rotation of the texture.</param>
        /// <param name="scale">The scale of the texture.</param>
        /// <param name="depth">The depth of the texture.</param>
        public AnimatedTexture(float rotation, float scale, float depth)
        {
            this.rotation = rotation;
            this.scale = scale;
            this.depth = depth;
        }

        /// <summary>
        /// Use this method to load a new texture. This can be used for a first time but also to load a new texture later on.
        /// </summary>
        /// <param name="assets">The contentmanager.</param>
        /// <param name="key">The path to the key in your content porject.</param>
        /// <param name="frameCount">The amount of frames the spritesheet consists of.</param>
        /// <param name="framesPerSec">How many frames you want to show per second.</param>
        public void Load(Functions.AssetHolder assets, string key, int frameCount, int framesPerSec)
        {
            framecount = frameCount;
            myTexture = assets.Load<Texture2D>(key);
            this.Origin = new Vector2(this.myTexture.Width / (2 * framecount), this.myTexture.Height / 2);
            timePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            totalElapsed = 0;
            paused = false;
        }

        /// <summary>
        /// Update the animated texture.
        /// </summary>
        /// <param name="elapsed">The amount of seconds past in the last frame.</param>
        public void UpdateFrame(float elapsed)
        {
            //Stop if the animation is paused.
            if (paused) {
                return;
            }

            totalElapsed += elapsed;
            if (totalElapsed > timePerFrame) {
                Frame++;
                // Keep the Frame between 0 and the total frames, minus one. 
                Frame = Frame % framecount;
                totalElapsed -= timePerFrame;
            }
        }

        /// <summary>
        /// Draws the sprite at a certain frame. The frame changes in the UpdateFrame method.
        /// </summary>
        /// <param name="batch">The batch you use for drawing.</param>
        /// <param name="screenPos">The position you want to draw it compared to the Origin.</param>
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame(batch, Frame, screenPos);
        }

        /// <summary>
        /// Draws the sprite at a custom frame.
        /// </summary>
        /// <param name="batch">The batch you use for drawing.</param>
        /// <param name="frame">The frame you want to show.</param>
        /// <param name="screenPos">The position you want to draw it compared to the Origin.</param>
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
        {
            int FrameWidth = myTexture.Width / framecount;
            //Select the texture to draw by creating a retangle which will 'hover' over the selected texture part.
            Rectangle sourcerect = new Rectangle(FrameWidth * frame, 0,
                FrameWidth, myTexture.Height);
            batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                rotation, Origin, scale, SpriteEffects.None, depth);
        }

        public bool IsPaused
        {
            get { return paused; }
        }

        public void Reset()
        {
            Frame = 0;
            totalElapsed = 0f;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Play()
        {
            paused = false;
        }

        public void Pause()
        {
            paused = true;
        }

    } 
}
