using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame.States.MenuObjects
{
    class MenuEntry : MenuObject
    {
        public MenuEntry(Game1 _game, SpriteBatch _spriteBatch, String _text, Vector2 _position, SpriteFont _font)
            : base(_game, _spriteBatch, _position, Color.OrangeRed)
        {
            this.text = _text;
            this.font = _font;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
