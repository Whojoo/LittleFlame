using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace LittleFlame.Models
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Star : LFModel
    {
        public Star(Game game, Model model, Vector3 position, Vector3 rotation, Vector3 scale)
            : base(game, model, position, rotation, scale)
        {
            this.rangeDistance = 2;
        }

        public override void Update(GameTime gameTime)
        {
            this.Rotation += new Vector3(0, MathHelper.Pi / 100, 0);

            base.Update(gameTime);
        }
    }
}
