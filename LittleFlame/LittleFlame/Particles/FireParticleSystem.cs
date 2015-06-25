#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
#endregion

namespace LittleFlame.Particles
{
    /// <summary>
    /// Custom particle system for creating a flame effect.
    /// </summary>
    public class FireParticleSystem : ParticleSystem
    {
        private float sizeDiff = 0.1f;
        private float sizeMod = 0.0133f;

        float flameSize;
        public float FlameSize
        {
            get { return flameSize; }
            set { flameSize = value; }
        }

        public FireParticleSystem(Game game, ContentManager content)
            : base(game, content)
        {
        }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            particleKey = "fireparticle";

            settings.MaxParticles = 800;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = -1;
            settings.MaxHorizontalVelocity = 1f;

            settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = 2;

            settings.MinRotateSpeed = 2f;
            settings.MaxRotateSpeed = -1f;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 1.5f, 0);

            settings.MinColor = new Color(255, 255, 255, 255);
            settings.MaxColor = new Color(255, 0, 0, 255);

            settings.MinStartSize = 1;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 1;
            settings.MaxEndSize = 4;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }

        public override void Draw(GameTime gameTime)
        {
            LittleFlame player = (LittleFlame)game.Services.GetService(typeof(LittleFlame));
            if (player != null)
                this.flameSize = player.FlameSize;

            EffectParameterCollection parameters = particleEffect.Parameters;

            /*parameters["Duration"].SetValue((float)settings.Duration.TotalSeconds);
            parameters["DurationRandomness"].SetValue(settings.DurationRandomness);
            parameters["Gravity"].SetValue(settings.Gravity);
            parameters["EndVelocity"].SetValue(settings.EndVelocity);
            parameters["MinColor"].SetValue(settings.MinColor.ToVector4());
            parameters["MaxColor"].SetValue(settings.MaxColor.ToVector4());

            parameters["RotateSpeed"].SetValue(new Vector2(settings.MinRotateSpeed, settings.MaxRotateSpeed));*/
            parameters["StartSize"].SetValue(new Vector2((flameSize - (flameSize * sizeDiff)) * sizeMod, (flameSize + (flameSize * sizeDiff)) * sizeMod));
            parameters["EndSize"].SetValue(new Vector2((flameSize - (flameSize * sizeDiff)) * sizeMod, (flameSize + (flameSize * sizeDiff)) * sizeMod));


            base.Draw(gameTime);
        }

    }
}
