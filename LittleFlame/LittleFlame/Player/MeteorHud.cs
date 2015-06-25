using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace LittleFlame.Player
{
    public class MeteorHud : DrawableGameComponent
    {
        private Game game; 
        private Texture2D meteorTexture; 
        private Texture2D lockedMeteorTexture; 
        private Vector2 position; 
        private Vector2 size; 
        private int amountMeteors;
        private float offsetBetweenMeteors;
        private SpriteBatch spriteBatch;
        private Rectangle[] meteors;
        private int unlockedMeteors;

        public MeteorHud(Game game, Texture2D meteorTexture, Texture2D lockedMeteorTexture, Vector2 position, Vector2 size, int amountMeteors, float offsetBetweenMeteors)
            : base(game)
        {
            this.game = game;
            this.meteorTexture = meteorTexture;
            this.lockedMeteorTexture = lockedMeteorTexture;
            this.position = position;
            this.size = size;
            this.amountMeteors = amountMeteors;
            this.offsetBetweenMeteors = offsetBetweenMeteors;
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            meteors = new Rectangle[amountMeteors];

            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            float tempOffset = position.X;
            for (int i = 0; i < meteors.Length; i++){
                tempOffset += (size.X + offsetBetweenMeteors);
                meteors[i] = new Rectangle((int)tempOffset, (int)position.Y, (int)size.X, (int)size.Y);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();
            for (int i = 0; i < meteors.Length; i++){
                if (i < unlockedMeteors)
                    spriteBatch.Draw(lockedMeteorTexture, meteors[i], Color.White);
                else
                    spriteBatch.Draw(meteorTexture, meteors[i], Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public int UnlockedMeteors
        {
            get { return unlockedMeteors; }
            set { unlockedMeteors = value; }
        }

    }
}
