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
using System.Diagnostics;
using LittleFlame.Particles;


namespace LittleFlame.Functions
{
    public class FireSpread : DrawableGameComponent
    {
        public const byte FLAME_PARTICLES_PER_FRAME = 6;
        private const byte EXTINGUISH_MULTIPLIER = 15;

        private Game1 game;
        private float spreadTimer;
        private bool isExtinguished;

        public FireSpeadParticleSystem Fire { get; private set; }
        public float FlameSize { get; private set; }


        public FireSpread(Game1 game, bool isPlayer)
            : base(game)
        {
            this.Fire = new FireSpeadParticleSystem(game);
            this.game = game;
        }

        public override void Initialize()
        {
            this.isExtinguished = false;
            this.Fire.DrawOrder = 100;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.FlameSize -= (float) gameTime.ElapsedGameTime.TotalSeconds * EXTINGUISH_MULTIPLIER;
            this.Fire.FlameSize = this.FlameSize;

            //If the fire is to small, remove it from the components so the fire won't be drawn anymore.
            if (this.FlameSize < 10) {
                this.isExtinguished = true;
                this.Game.Components.Remove(Fire);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Change the states back for 3d models.
            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }

        internal void Reset(Vector3 position, float size)
        {
            this.Position = position;
            this.Fire.Position = new Vector3(position.X, position.Y, position.Z);
            this.FlameSize = size;
            this.SpreadTimer = 0;
            this.Game.Components.Add(Fire);
        }

        public Vector3 Position
        {
            get { return this.Fire.Position; }
            set { this.Fire.Position = value; }
        }

        public float SpreadTimer
        {
            get { return spreadTimer; }
            set { spreadTimer = value; }
        }

        public bool IsExtinguished
        {
            get { return isExtinguished; }
            set { isExtinguished = value; }
        }
    }
}
