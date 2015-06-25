using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame.States.MenuObjects
{
    class MenuButton : MenuObject
    {
        Texture2D texture;
        Texture2D idleTexture;
        Texture2D selectedTexture;

        public MenuButton(Game1 _game, SpriteBatch _spriteBatch, Vector2 _position, string _idleTextureKey, string _selectedTextureKey)
            : base(_game, _spriteBatch, _position, Color.DimGray)
        {
            this.idleTexture = _game.Assets.Load<Texture2D>(_idleTextureKey);
            this.selectedTexture = _game.Assets.Load<Texture2D>(_selectedTextureKey);
            texture = idleTexture;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, position, null, color, 0, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public override void SelectThis()
        {
            texture = selectedTexture;
        }

        public override void DeselectThis()
        {
            texture = idleTexture;
        }
    }
}
