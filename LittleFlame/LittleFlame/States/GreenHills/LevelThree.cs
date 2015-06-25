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
    class LevelThree : Level
    {
        public LevelThree(Game1 game, string loadName)
            : base(game, loadName)
        {
            
        }

        #region LoadContent

        public override void Initialize()
        {
            saveName = "4";

            this.timeScore = 300;

            levelAsset = "three";

            base.Initialize();
        }

        public override void LoadContent()
        {
            startPosition = new Vector3(181, 0, 304);

            base.LoadContent();
        }

        protected override void goToNextLevel()
        {
            Game.goToNextState(new ScoreMenu.ScoreMenu(Game, 10.0f, (int)score.CalculateEndPerc(), score.CalculateScore(), this.collectablesFound, ScoreMenu.ScoreMenu.Levels.Tutorial,
                 ScoreMenu.ScoreMenu.Levels.LevelThree));
            base.goToNextLevel();
        }

        #endregion

    }
}
