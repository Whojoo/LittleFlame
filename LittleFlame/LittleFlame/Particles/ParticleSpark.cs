using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LittleFlame.Particles;

namespace LittleFlame
{
    public class ParticleSpark: ParticleSystem
    {
        private float sizeDiff = 0.1f;
        private float sizeMod = 0.01f;

        public ParticleSpark(Game game, ContentManager content)
            : base(game, content)
        {
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            particleKey = "sparkparticle";

            settings.MaxParticles = 200;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = -1;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = 1;
            settings.MaxVerticalVelocity = 3;

            settings.MinRotateSpeed = 1;
            settings.MaxRotateSpeed = 5;

            // Set gravity upside down, so the flames will 'fall' upward.
            //settings.Gravity = new Vector3(1f, 1f, 1f);

            settings.MinColor = new Color(100, 100, 100, 255);
            settings.MaxColor = new Color(150, 150, 150, 100);

            settings.MinStartSize = 1;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 3;
            settings.MaxEndSize = 4;

            // Use additive blending.
            settings.BlendState = BlendState.AlphaBlend;
        }

        public override void Draw(GameTime gameTime)
        {
            LittleFlame player = (LittleFlame)game.Services.GetService(typeof(LittleFlame));
            float flameSize;
            if (player != null) flameSize = player.FlameSize;
            else flameSize = 0;

            EffectParameterCollection parameters = particleEffect.Parameters;

            parameters["StartSize"].SetValue(new Vector2((flameSize - (flameSize * sizeDiff)) * sizeMod,(flameSize + (flameSize * sizeDiff)) * sizeMod));
            parameters["EndSize"].SetValue(new Vector2((flameSize - (flameSize * sizeDiff)) * sizeMod, (flameSize + (flameSize * sizeDiff)) * sizeMod));
            
            base.Draw(gameTime);
        }
    }
}
