#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#endregion

namespace LittleFlame.States.GreenHills
{
    class LevelTwo : Level
    {
        public LevelTwo(Game1 game, string loadName)
            : base(game, loadName)
        {

        }

        #region LoadContent

        public override void Initialize()
        {
            saveName = "3";

            this.timeScore = 300;
            levelAsset = "two";

            base.Initialize();
        }

        public override void LoadContent()
        {
            startPosition = new Vector3(188, 0, 274);

            base.LoadContent();
        }

        protected override void goToNextLevel()
        {
            Game.goToNextState(new ScoreMenu.ScoreMenu(Game, 10.0f, (int)score.CalculateEndPerc(), score.CalculateScore(), this.collectablesFound, ScoreMenu.ScoreMenu.Levels.LevelTwo,
                ScoreMenu.ScoreMenu.Levels.LevelOne));
            base.goToNextLevel();
        }

        #endregion

    }
}
