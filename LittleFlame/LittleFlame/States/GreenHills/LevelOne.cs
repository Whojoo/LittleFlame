#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace LittleFlame.States.GreenHills
{
    class LevelOne : Level
    {
        public LevelOne(Game1 game, string loadName)
            : base(game, loadName)
        {
            
        }
        
        #region LoadContent

        public override void Initialize()
        {
            saveName = "2";

            this.timeScore = 300;
            levelAsset = "one";

            base.Initialize();
        }

        public override void LoadContent()
        {
            startPosition = new Vector3(185, 0, 305);

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
