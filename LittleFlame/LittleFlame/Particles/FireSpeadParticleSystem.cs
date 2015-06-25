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
using LittleFlame.Functions;
using LittleFlame.Camera;


namespace LittleFlame.Particles
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FireSpeadParticleSystem : ParticleSystem
    {
        private float sizeDiff = 0.33f;
        private float sizeMod = 0.02f;

        public Vector3 Position { get; set; }
        public float FlameSize { get; set; }

        public FireSpeadParticleSystem(Game1 game)
            : base(game, game.Content)
        {
            this.Position = new Vector3();
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            particleKey = "spreadparticle";
            
            settings.MaxParticles = 300;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1.5f;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 2f;

            settings.MinRotateSpeed = 2f;
            settings.MaxRotateSpeed = -1f;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 1f, 0);

            settings.MinColor = new Color(255, 255, 255, 155);
            settings.MaxColor = new Color(255, 0, 0, 155);

            settings.MinStartSize = 1;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 1;
            settings.MaxEndSize = 4;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }

        public override void Draw(GameTime gameTime)
        {
            EffectParameterCollection parameters = particleEffect.Parameters;
            parameters["StartSize"].SetValue(new Vector2((FlameSize - (FlameSize * sizeDiff)) * sizeMod, (FlameSize + (FlameSize * sizeDiff)) * sizeMod));
            parameters["EndSize"].SetValue(new Vector2((FlameSize - (FlameSize * sizeDiff)) * sizeMod, (FlameSize + (FlameSize * sizeDiff)) * sizeMod));

            for (int u = 0; u < FireSpread.FLAME_PARTICLES_PER_FRAME; u++) {
                this.AddParticle(this.Position, Vector3.Zero);
            }

            Camera.Camera cam = (Camera.Camera)this.Game.Services.GetService(typeof(Camera.Camera));
            this.SetCamera(cam.viewMatrix, cam.projectionMatrix);

            base.Draw(gameTime);
        }
    }
}
