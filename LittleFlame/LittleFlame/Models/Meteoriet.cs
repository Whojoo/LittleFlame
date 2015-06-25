using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LittleFlame.Particles;

namespace LittleFlame.Models
{

    class Meteoriet : LFModel
    {
        public Vector3 TopTree
        {
            get { return topTree; }
            set { topTree = value; }
        }
        private Vector3 topTree;

        private bool notTouched;

        public bool IsTouched
        {
            get { return notTouched; }
            set { notTouched = value; }
        }

        private MeteorParticleSystem particles;
        private Game1 game;

        Random rnd;

        public Meteoriet(Game1 game, Model model, Vector3 rotation, Vector3 position, Vector3 scale)
            : base(game, model, rotation, position, scale)
        {
            this.game = game;
            rangeDistance = 2;
            notTouched = false;
        }

        protected override void LoadContent()
        {
            particles = new MeteorParticleSystem(game, Game.Content);
            Game.Components.Add(particles);

            rnd = new Random();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsTouched == true)
            {
                const int fireParticlesPerFrame = 3;
                for (int i = 0; i < fireParticlesPerFrame; i++)
                {
                    // Calculate the position for the particles. The particles will spawn in a radius of 4 around the meteor.
                    float randomNr = (float) rnd.NextDouble() * 4 * (float) Math.PI;
                    Vector3 pos = this.position + new Vector3((float) Math.Cos(randomNr) * 2f, 0, (float) Math.Sin(randomNr) * 2f);
                    particles.AddParticle(pos, Vector3.Zero);
                }
            }

            this.GraphicsDevice.BlendState = BlendState.Opaque;

            Camera.Camera cam = (Camera.Camera)this.Game.Services.GetService(typeof(Camera.Camera));
            particles.SetCamera(cam.viewMatrix, cam.projectionMatrix);

            base.Draw(gameTime);
        }
    }
}