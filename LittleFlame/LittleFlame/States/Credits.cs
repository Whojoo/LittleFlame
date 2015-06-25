using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using LittleFlame.States;
using LittleFlame.States.MenuObjects;

namespace LittleFlame.States
{
    class Credits: Menu
    {
        private Song backgroundMusic;
        private float songTimeCounter = 0;

        public Credits(Game1 _game, KeyboardState _oldKeyboardState, GamePadState _oldGamePadState)
            : base(_game, _oldKeyboardState, _oldGamePadState)
        {
            
        }

        public override void LoadContent()
        {
            font = Game.Assets.Load<SpriteFont>("font");
            background = Game.Assets.Load<Texture2D>("credits");

            entries.Add(new MenuEntry(Game, SpriteBatch, "Back", new Vector2(Game.GraphicsDevice.Viewport.Width - 100, Game.GraphicsDevice.Viewport.Height - 100), font));

            backgroundMusic = Game.Assets.Load<Song>("credits");
            MediaPlayer.Play(backgroundMusic);

            base.LoadContent();
        }

        protected override void selectCurrentItem()
        {
            switch (selectedEntry.Index)
            {
                case 0:
                {
                    MediaPlayer.Stop();
                    Game.goToNextState(new MainMenu(Game, oldKeyboardState, oldGamePadState));
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.songTimeCounter >= this.backgroundMusic.Duration.TotalSeconds)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(backgroundMusic);
                this.songTimeCounter = 0;
            }
            
            base.Update(gameTime);
        }
    }
}