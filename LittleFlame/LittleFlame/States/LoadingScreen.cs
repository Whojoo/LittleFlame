using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LittleFlame.States.GreenHills;
using Microsoft.Xna.Framework;

namespace LittleFlame.States
{
    class LoadingScreen : State
    {
        private State state;
        private int level;
        private string loadname;

        public LoadingScreen(Game1 game, int level)
            : base(game)
        {
            this.loadname = "";
            this.level = level;
        }

        public LoadingScreen(Game1 game, string loadname)
            : base(game)
        {
            this.loadname = loadname;
            this.level = loadname[0] - 48;
        }

        public override void Initialize()
        {
            switch (level)
            {
                case 0: state = new CinematicState(Game, loadname); break;
                case 1: state = new LevelZero(Game, loadname); break;
                case 2: state = new LevelOne(Game, loadname); break;
                case 3: state = new LevelTwo(Game, loadname); break;
                case 4: state = new LevelThree(Game, loadname); break;
                default: break;
            }
        }

        public override void LoadContent()
        {
            Game.Graphics.GraphicsDevice.Clear(Color.Black);
            Game.goToNextState(state);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        }
    }
}
