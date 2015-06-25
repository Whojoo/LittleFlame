using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame.States
{
    public class Test : State
    {
        public Test(Game1 _game)
            : base(_game)
        {

        }

        public override void Initialize()
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            Game.Graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);
        }
    }
}
