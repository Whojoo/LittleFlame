using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace LittleFlame.Player
{
    
    public class Healthbar : DrawableGameComponent
    {
        private Texture2D barText;
        private Vector2 position;
        private LittleFlame player;
        private int height;
        private Rectangle healthRect;
        private SpriteBatch spriteBatch;
        private int boostMin;
        private Game game;

        public Healthbar(Game game, Texture2D barText, Vector2 position, int height, int boostMin, LittleFlame player)
            : base(game)
        {
            this.barText = barText;
            this.position = position;
            this.height = height;
            this.player = player;
            this.boostMin = boostMin;
            this.game = game;
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            healthRect = new Rectangle((int)position.X, (int)position.Y, (int)player.FlameSize, height);
            Console.WriteLine((int)player.FlameSize);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //You can boost now
            if ((int)player.FlameSize >= boostMin)
                spriteBatch.Draw(barText, healthRect, Color.White);
            else
                spriteBatch.Draw(barText, healthRect, Color.White);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
