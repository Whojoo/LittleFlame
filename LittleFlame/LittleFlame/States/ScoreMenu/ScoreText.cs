using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LittleFlame.States.ScoreMenu
{
    class ScoreText
    {
        private SpriteFont font;
        private Vector2 position;
        private string text;
        private Game1 game;
        private SpriteBatch batch;

        public ScoreText(Game1 game, Vector2 position, string text)
        {
            this.position = position;
            this.text = text;
            this.game = game;
        }

        public void LoadContent()
        {
            this.font = game.Assets.Load<SpriteFont>("font");
            this.batch = new SpriteBatch(game.GraphicsDevice);
        }

        public void Draw()
        {
            this.batch.Begin();
            this.batch.DrawString(this.font, this.text, this.position, Color.Wheat);
            this.batch.End();
        }
    }
}
