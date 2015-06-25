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
    public class MeteorParticleSystem : ParticleSystem
    {
        public MeteorParticleSystem(Game1 game, ContentManager content)
            : base(game, content)
        {
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            particleKey = "meteorparticle";

            settings.MaxParticles = 300;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 1;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 5f, 0);

            settings.MinColor = new Color(255, 55, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;

            settings.MinEndSize = 1;
            settings.MaxEndSize = 1;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
