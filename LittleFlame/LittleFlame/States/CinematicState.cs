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
using LittleFlame.States.GreenHills;


namespace LittleFlame.States
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CinematicState : State
    {
        private const string PicturesLocation = "Textures/Cutscene/picture";
        private const string Key = "cinematicpicture";
        private const float ToSecondPicture = 6f;
        private const float ToThirdPicture = 12f;
        private const float ToFourthPicture = 15f;
        private const float ToFifthPicture = 20f;
        private const float ToSixthPicture = 28f;
        private const float ToTutorial = 41f;
        private const float Easing = 0.01f;

        private float duration;
        private SpriteBatch batch;
        private Camera.Camera2D camera;
        private Texture2D[] pictures;
        private Vector2[] picturePositions;
        private Vector2 pictureHalfSize;
        private string loadName;

        public CinematicState(Game1 game, string loadName)
            : base(game)
        {
            this.loadName = loadName;
        }

        public override void Initialize()
        {
            //Initialise the SpriteBatch and load the Textures and calculate the center of the screen.
            this.batch = new SpriteBatch(this.Game.GraphicsDevice);
            Vector2 center = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 2);
            this.pictures = new Texture2D[6];
            for (int i = 1; i <= this.pictures.Length; i++) {
                //Check if the picture is loaded already.
                if (!this.Game.Assets.ContainsKey<Texture2D>(Key + i.ToString())) {
                    this.Game.Assets.SaveAsset<Texture2D>(Key + i.ToString(), PicturesLocation + i.ToString());
                }
                this.pictures[i - 1] = this.Game.Assets.Load<Texture2D>(Key + i.ToString());
            }
            //Initialise the sum of the pictures width and height for a worldwidth worldheight and use those to initialise the camera.
            int worldWidth = 0;
            int worldHeight = 0;
            for (int i = 0; i < this.pictures.Length; i++) {
                worldHeight += this.pictures[i].Height;
                worldWidth += this.pictures[i].Width;
            }
            this.camera = new Camera.Camera2D(this.Game.GraphicsDevice, worldWidth, worldHeight, 1);

            //Calculate the positions of the pictures. All pictures are 1080 × 720.
            this.picturePositions = new Vector2[this.pictures.Length];
            for (int i = 0; i < this.pictures.Length; i++) {
                this.picturePositions[i] = new Vector2(center.X + this.pictures[i].Width * i, center.Y);
            }
            this.pictureHalfSize = new Vector2(this.pictures[0].Width / 2, this.pictures[0].Height / 2);

            //Initialise and play the narrative.
            Song narrative = this.Game.Assets.Load<Song>("story");
            MediaPlayer.Play(narrative);
            this.duration = 0f;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.duration += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.duration >= ToSecondPicture && this.duration <= ToThirdPicture) {
                this.camera.EaseTo(this.picturePositions[1], Easing);
            } else if (this.duration >= ToThirdPicture && this.duration <= ToFourthPicture) {
                this.camera.EaseTo(this.picturePositions[2], Easing);
            } else if (this.duration >= ToFourthPicture && this.duration <= ToFifthPicture) {
                this.camera.EaseTo(this.picturePositions[3], Easing);
            } else if (this.duration >= ToFifthPicture && this.duration <= ToSixthPicture) {
                this.camera.EaseTo(this.picturePositions[4], Easing);
            } else if (this.duration >= ToSixthPicture && this.duration <= ToTutorial) {
                this.camera.EaseTo(this.picturePositions[5], Easing);
            } else if (this.duration >= ToTutorial) {
                this.Game.goToNextState(new Tutorial(this.Game, this.loadName));
            }

            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsButtonDown(Buttons.A) || pad.IsButtonDown(Buttons.B) || pad.IsButtonDown(Buttons.Back) || pad.IsButtonDown(Buttons.X) ||
                pad.IsButtonDown(Buttons.Y) || pad.IsButtonDown(Buttons.Start) || Keyboard.GetState().IsKeyDown(Keys.Space)) {
                    this.Game.goToNextState(new Tutorial(this.Game, this.loadName));
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            this.batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, this.camera.GetTransformation());
            for (int i = 0; i < this.pictures.Length; i++) {
                this.batch.Draw(this.pictures[i], this.picturePositions[i], null, Color.White, 0, this.pictureHalfSize, 1, SpriteEffects.None, 0);
            }
            this.batch.End();

            base.Draw(gameTime);
        }
    }
}
