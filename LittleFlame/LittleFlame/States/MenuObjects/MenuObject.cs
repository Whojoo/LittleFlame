using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame.States.MenuObjects
{
    class MenuObject
    {
        protected String text;
        protected SpriteFont font;

        protected Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        protected Game1 game;
        protected SpriteBatch spriteBatch;
        protected Vector2 position;

        protected float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        private MenuObject next;
        public MenuObject Next
        {
            get { return next; }
            set { next = value; }
        }

        private MenuObject previous;
        public MenuObject Previous
        {
            get { return previous; }
            set { previous = value; }
        }

        public MenuObject(Game1 _game, SpriteBatch _spriteBatch, Vector2 _position, Color _color)
        {
            this.game = _game;
            this.spriteBatch = _spriteBatch;
            this.position = _position;
            this.color = _color;
            scale = 1;
        }
        
        public virtual void Draw(GameTime gameTime) { }
        public virtual void SelectThis() { }
        public virtual void DeselectThis() { }
    }
}
