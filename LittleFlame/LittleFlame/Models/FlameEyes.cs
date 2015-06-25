using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace LittleFlame.Models
{
    class FlameEyes : LFModel
    {
        public const float RADIUS = 40f;

        private LittleFlame player;
        private Vector3 offset;

        /// <summary>
        /// Add the eyes for the player.
        /// </summary>
        /// <param name="game">The game in which you want to add the eyes.</param>
        /// <param name="_level">The level in which you want to add the eyes.</param>
        /// <param name="model">A Model object with a path to the model.</param>
        /// <param name="rotation">The starting rotation of the eyes.</param>
        /// <param name="scale">The eye's scale.</param>
        /// <param name="player">The player object which will be used to update this object's position.</param>
        public FlameEyes(Game game, Model model, Vector3 rotation, Vector3 scale, LittleFlame player)
            : base(game, model, Vector3.Zero, rotation, scale)
        {
            this.player = player;
            this.offset = new Vector3(0, 0.25f, RADIUS);
            this.position = player.Position + offset;
        }

        public override void Update(GameTime gameTime)
        {
            //Refresh the position in case the player is climbing a tree or flying or at least not moving by the wind.
            this.position = new Vector3(player.Position.X, player.Position.Y + this.offset.Y, player.Position.Z);
            base.Update(gameTime);
        }

        public void Rotate(Vector3 direction)
        {
            //Calulate the angle of the offset vector.
            this.rotation.Y = -(float)Math.Atan2(direction.Z, direction.X) - (float)Math.PI / 2;
            //Put the new value's in the current position and rotation.
            this.position = new Vector3(player.Position.X, player.Position.Y + this.offset.Y, player.Position.Z);
        }

        protected override Matrix GetTransformation()
        {
            /*
             * First translate the eyes to the offset so that when you rotate, the eyes rotate around the Y-axis instead of on the Y-axis.
             * Then rotate and translate so the eyes are positioned correctly.
             */
            Matrix transform = Matrix.CreateTranslation(-this.offset) *
                Matrix.CreateRotationY(this.rotation.Y) *
                Matrix.CreateScale(this.scale) *
                Matrix.CreateTranslation(this.position);
            return transform;
        }
    }
}
