using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using LittleFlame.Functions;
using System.Diagnostics;

namespace LittleFlame.States.GreenHills
{
    class Tutorial : Level
    {
        private enum Type
        {
            CAMERA,
            MOVEMENT
        }

        //Assets for the tutorial buttons.
        private const string LeftStickKey = "tutleftstick";
        private const string RightStickKey = "tutrightstick";

        private Game1 game;
        private Vector2 positionController;
        private bool controlsOff;
        private Type currentType;
        private float countSeconds, inputValue;
        private Color color;
        private AnimatedTexture animatedSprite;

        public Tutorial(Game1 game, string loadName)
            : base(game, loadName)
        {
            this.game = game;
            this.currentType = Type.CAMERA;
            this.controlsOff = true;
            this.countSeconds = 0;
            this.color = Color.Wheat;
        }

        public override void Initialize()
        {
            saveName = "0";

            this.positionController = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, 100);
            startPosition = new Vector3(200, 0, 270);
            this.timeScore = 300;

            levelAsset = "tutorial";


            base.Initialize();
        }

        public override void LoadContent()
        {
            this.animatedSprite = new AnimatedTexture(0, 1, 0);
            this.animatedSprite.Load(this.game.Assets, LeftStickKey, 4, 6);

            this.timeScore = 300;
            Debug.WriteLine("burnable: {0}", this.burnScore);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.animatedSprite.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

            KeyboardState temp = Keyboard.GetState();
            if (temp.IsKeyDown(Keys.D1)) {
                this.goToNextLevel();
            }

            base.Update(gameTime);
        }

        protected override void DrawSprites(GameTime gameTime)
        {
            float deltaSecond = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            if (this.controlsOff) {
                //Select which type of tutorial will be used.
                switch (this.currentType) {
                    case Type.CAMERA: DrawIntroText(deltaSecond);
                        this.animatedSprite.DrawFrame(this.SpriteBatch, this.positionController); 
                        break;
                    case Type.MOVEMENT: DrawBurningText(deltaSecond); 
                        this.animatedSprite.DrawFrame(this.SpriteBatch, this.positionController); 
                        break;
                }
            }
            this.SpriteBatch.End();

            base.DrawSprites(gameTime);
        }

        private void DrawBurningText(float deltaSecond)
        {
            this.countSeconds += deltaSecond;
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            //Set the rightTrigger value to positive before you add it to inputvalue.
            this.inputValue += (float)Math.Sqrt(Math.Pow(pad.ThumbSticks.Right.X, 2));

            if (this.inputValue > 30) {
                this.animatedSprite.Stop();
                this.controlsOff = false;
                this.countSeconds = 0;
            }
        }

        /// <summary>
        /// Draws the first sprite.
        /// </summary>
        private void DrawIntroText(float deltaSecond)
        {
            this.countSeconds += deltaSecond;
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            //Set the leftstick value to positive before you add it to inputvalue.
            this.inputValue += (float)Math.Sqrt(Math.Pow(pad.ThumbSticks.Left.X, 2));
            if (this.inputValue > 30) {
                this.countSeconds = 0;
                this.inputValue = 0;
                this.currentType = Type.MOVEMENT;
                this.animatedSprite.Load(this.game.Assets, RightStickKey, 4, 6);
            }
        }

        protected override void goToNextLevel()
        {
            Debug.WriteLine("burnable: {0}", this.burnScore);
            Game.goToNextState(new ScoreMenu.ScoreMenu(Game, 10.0f, (int)score.CalculateEndPerc(), score.CalculateScore(), this.collectablesFound, ScoreMenu.ScoreMenu.Levels.LevelZero,
                 ScoreMenu.ScoreMenu.Levels.Tutorial));
                
            base.goToNextLevel();
            //game.gotonextstate(new scoremenu.scoremenu(game, this.timescore, this.burnscore, this.collectablesfound,
            //    scoremenu.scoremenu.levels.levelzero, scoremenu.scoremenu.levels.tutorial));
        }
    }
}
