using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using LittleFlame.States.MenuObjects;
using Microsoft.Xna.Framework.Graphics;
using LittleFlame.States.GreenHills;

namespace LittleFlame.States.ScoreMenu
{
    class ScoreMenu : Menu
    {
        public enum Levels
        {
            Tutorial,
            LevelZero,
            LevelOne,
            LevelTwo,
            LevelThree
        }

        private const float BurnScoreMod = 2.0f, TimeScoreMod = 2.0f, CollectableScoreMod = 50.0f, HeightDifference = 30f;

        private float totalTimeScore, totalBurnScore, totalCollectableScore, totalScore, percentageBurned;
        private ScoreText showTimeScore, showBurnScore, showCollectablesScore, showTotalScore, showPercentageBurned;
        private List<ScoreText> texts;
        private Levels currentLevel, nextLevel;

        public ScoreMenu(Game1 game, float timeleft, int percentageBurned, int score, int collectables,
            Levels nextLevel, Levels currentLevel)
            : base(game, Keyboard.GetState(), GamePad.GetState(PlayerIndex.One))
        {
            this.totalTimeScore = timeleft * TimeScoreMod;
            this.totalBurnScore = score;
            this.percentageBurned = percentageBurned;
            this.totalCollectableScore = collectables * CollectableScoreMod;
            this.totalScore = this.totalTimeScore + this.totalBurnScore + this.totalCollectableScore;
            this.currentLevel = currentLevel;
            this.nextLevel = nextLevel;
        }

        public override void LoadContent()
        {
            background = Game.Assets.Load<Texture2D>("spacebackground");

            int xPos = (int)Game.GraphicsDevice.Viewport.Width / 5 * 3;
            int yPos = 70;

            //initialise the text's.
            this.showTimeScore = new ScoreText(this.Game, new Vector2(xPos, yPos), "TimeScore: " + this.totalTimeScore.ToString());
            this.showBurnScore = new ScoreText(this.Game, new Vector2(xPos, yPos) + new Vector2(0, HeightDifference),
                "BurnScore: " + this.totalBurnScore.ToString());
            this.showCollectablesScore = new ScoreText(this.Game, new Vector2(xPos, yPos) + new Vector2(0, 2 * HeightDifference),
                "CollectablesScore: " + this.totalCollectableScore.ToString());
            this.showPercentageBurned = new  ScoreText(this.Game, new Vector2(xPos, yPos) + new Vector2(0, 4 * HeightDifference),
                "Percentage burned: " + this.percentageBurned.ToString());
            this.showTotalScore = new ScoreText(this.Game, new Vector2(xPos, yPos) + new Vector2(0, 6 * HeightDifference),
                "TotalScore: " + this.totalScore.ToString());

            //Add the scoreTexts to a list.
            this.texts = new List<ScoreText>();
            this.texts.Add(this.showTimeScore);
            this.texts.Add(this.showBurnScore);
            this.texts.Add(this.showCollectablesScore);
            this.texts.Add(this.showPercentageBurned);
            this.texts.Add(this.showTotalScore);

            foreach (ScoreText text in this.texts) {
                text.LoadContent();
            }

            xPos = 50;
            yPos = Game.GraphicsDevice.Viewport.Height - 190;

            this.font = this.Game.Assets.Load<SpriteFont>("font");
            this.entries.Add(new MenuEntry(this.Game, this.SpriteBatch, "Next Level", new Vector2(xPos, yPos), this.font));
            this.entries.Add(new MenuEntry(this.Game, this.SpriteBatch, "Replay Previous Level", new Vector2(xPos, yPos + 30), this.font));
            this.entries.Add(new MenuEntry(this.Game, this.SpriteBatch, "Quit to Main Menu", new Vector2(xPos, yPos + 60), this.font));
            this.entries.Add(new MenuEntry(this.Game, this.SpriteBatch, "Quit Game", new Vector2(xPos, yPos + 90), this.font));

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (ScoreText text in this.texts) {
                text.Draw();
            }
        }

        protected override void selectCurrentItem()
        {
            switch (this.selectedEntry.Index) {
                case 0: this.LoadLevel(this.nextLevel); break;
                case 1: this.LoadLevel(this.currentLevel); break;
                case 2: this.Game.goToNextState(new MainMenu(this.Game, Keyboard.GetState(), GamePad.GetState(PlayerIndex.One))); break;
                case 3: this.Game.Exit(); break;
                default: break;
            }
        }

        private void LoadLevel(Levels levels)
        {
            Console.WriteLine("Loading level: " + levels);
            switch (levels) {
                case Levels.Tutorial: this.Game.goToNextState(new States.LoadingScreen(this.Game, 0)); break;
                case Levels.LevelZero: this.Game.goToNextState(new States.LoadingScreen(this.Game, 1)); break;
                case Levels.LevelOne: this.Game.goToNextState(new States.LoadingScreen(this.Game, 2)); break;
                case Levels.LevelTwo: this.Game.goToNextState(new States.LoadingScreen(this.Game, 3)); break;
                case Levels.LevelThree: this.Game.goToNextState(new States.LoadingScreen(this.Game, 4)); break;
                default: break;
            }
        }
    }
}
