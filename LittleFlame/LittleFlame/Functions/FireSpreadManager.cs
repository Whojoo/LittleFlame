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


namespace LittleFlame.Functions
{
    public class FireSpreadManager : DrawableGameComponent
    {
        private Game game;
        private List<FireSpread> graveyard;
        private Random randy = new Random();
        private List<FireSpread> active;

        public FireSpreadManager(Game game)
            : base(game)
        {
            this.game = game;
        }

        public override void Initialize()
        {
            this.graveyard = new List<FireSpread>();
            this.active = new List<FireSpread>();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateFire(gameTime);
            this.Spread();
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public void Unload()
        {
            foreach (FireSpread spread in active) {
                spread.Dispose();
            }
            foreach (FireSpread spread in graveyard) {
                spread.Dispose();
            }
        }

        /// <summary>
        /// Checks all active fires. If a fire becomes to small, the fire will be extinguished and the variable will be placed in the graveyard for
        /// later use.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateFire(GameTime gameTime)
        {
            for (int i = this.active.Count - 1; i >= 0; i--) {
                if (this.active[i].IsExtinguished) {
                    this.graveyard.Add(this.active[i]);
                    this.Game.Components.Remove(this.active[i]);
                    this.active.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// This method manages the spread of fire. 
        /// </summary>
        /// <param name="gameTime">Used to add time between spreading fire.</param>
        private void Spread()
        {
            for (int i = 0; i < this.active.Count; i++) {
                FireSpread fire = (FireSpread) this.active[i];
                for (int j = 0; j < 2; j++) {
                    Vector3 temp = new Vector3(randy.Next(100) - 50, 0, randy.Next(100) - 50);
                    temp.Normalize();
                    temp = Vector3.Add(fire.Position, temp * 2);
                    fire.Fire.AddParticle(temp, Vector3.Zero);
                }
            }
        }

        public void addSpread(Vector3 position, float flameSize)
        {
            FireSpread fire;
            //First check if there's an inactive object in the graveyard so it can be reused.
            if (this.graveyard.Count > 0) {
                fire = this.graveyard[0];
                this.graveyard.RemoveAt(0);
            } else {
                fire = new FireSpread((Game1)game, false);
            }

            fire.Reset(new Vector3(position.X, position.Y, position.Z), flameSize * 0.6f);
            this.active.Add(fire);
            this.Game.Components.Add(fire);
        }
    }
}
