#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
#endregion

namespace LittleFlame.Particles
{
    public class SandParticleSystem : ParticleSystem
    {
        public SandParticleSystem(Game1 game, ContentManager content)
            : base(game, content)
        {
            
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Textures\\Particles\\sand";

            settings.MaxParticles = 300;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = -1;
            settings.MaxHorizontalVelocity = 1f;

            settings.MinRotateSpeed = -5f;
            settings.MaxRotateSpeed = 5f;

            settings.Gravity = new Vector3(0, 5f, 0);

            settings.MinColor = new Color(255, 255, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;

            settings.MinEndSize = 1;
            settings.MaxEndSize = 1;

            settings.BlendState = BlendState.AlphaBlend;
        }
    }
}
