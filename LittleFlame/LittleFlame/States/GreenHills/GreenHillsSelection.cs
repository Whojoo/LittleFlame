using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using LittleFlame.States.MenuObjects;

namespace LittleFlame.States.GreenHills
{
    class GreenHillsSelection : Menu
    {
        public GreenHillsSelection(Game1 _game, KeyboardState _oldKeyboardState, GamePadState _oldGamePadState)
            : base(_game, _oldKeyboardState, _oldGamePadState)
        {

        }

        public override void LoadContent()
        {
            font = Game.Assets.Load<SpriteFont>("font");
            background = Game.Assets.Load<Texture2D>("spacebackground");

            int xPos = 150;
            int yPos = Game.GraphicsDevice.Viewport.Height / 2;

            entries.Add(new MenuButton(Game, SpriteBatch, new Vector2(xPos, yPos), "one", "onesel"));
            entries.Add(new MenuButton(Game, SpriteBatch, new Vector2(xPos + 200, yPos), "two", "twosel"));
            entries.Add(new MenuButton(Game, SpriteBatch, new Vector2(xPos + 2 * 200, yPos), "three", "threesel"));
            entries.Add(new MenuButton(Game, SpriteBatch, new Vector2(xPos + 3 * (200), yPos), "four", "foursel"));
            entries.Add(new MenuButton(Game, SpriteBatch, new Vector2(xPos + 4 * (200), yPos), "five", "fivesel"));
            entries.Add(new MenuEntry(Game, SpriteBatch, "Back", new Vector2(Game.GraphicsDevice.Viewport.Width - 100, Game.GraphicsDevice.Viewport.Height - 100), font));

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if ((keyboardState.IsKeyDown(Keys.Enter) && !oldKeyboardState.IsKeyDown(Keys.Enter))
                || (gamePadState.IsButtonDown(Buttons.A) && !oldGamePadState.IsButtonDown(Buttons.A)))
            {
                selectCurrentItem();
            }


            if ((keyboardState.IsKeyDown(Keys.Down) && !oldKeyboardState.IsKeyDown(Keys.Down))
                || (gamePadState.IsButtonDown(Buttons.LeftThumbstickRight) && !oldGamePadState.IsButtonDown(Buttons.LeftThumbstickRight)))
            {
                selectedEntry.Scale = 1;
                if (selectedEntry is MenuEntry) selectedEntry.Color = Color.OrangeRed;
                else selectedEntry.Color = Color.DimGray;
                selectedEntry.DeselectThis();
                selectedEntry = selectedEntry.Next;
                selectedEntry.SelectThis();
            }
            if ((keyboardState.IsKeyDown(Keys.Up) && !oldKeyboardState.IsKeyDown(Keys.Up))
                || (gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldGamePadState.IsButtonDown(Buttons.LeftThumbstickLeft)))
            {
                selectedEntry.Scale = 1;
                if (selectedEntry is MenuEntry) selectedEntry.Color = Color.OrangeRed;
                else selectedEntry.Color = Color.DimGray;
                selectedEntry.DeselectThis();
                selectedEntry = selectedEntry.Previous;
                selectedEntry.SelectThis();
            }
            selectedEntry.Scale = 1.2f;
            selectedEntry.Color = Color.White;

            oldKeyboardState = keyboardState;
            oldGamePadState = gamePadState;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            string levelName = "";

            switch (selectedEntry.Index)
            {
                case 0: levelName = "Level: Tutorial"; break;
                case 1: levelName = "Level 1: First Steps"; break;
                case 2: levelName = "Level 2: Forest Fire"; break;
                case 3: levelName = "Level 3: Ready for Takeoff!"; break;
                case 4: levelName = "Level 4: Final Countdown"; break;
            }

            SpriteBatch.Begin();
            SpriteBatch.DrawString(font, levelName, new Vector2(50, 20), Color.OrangeRed, 0, Vector2.Zero, new Vector2(1.5f), SpriteEffects.None, 0);
            SpriteBatch.End();
        }

        protected override void selectCurrentItem()
        {
            switch (selectedEntry.Index)
            {
                case 0: Game.goToNextState(new LoadingScreen(Game, 0)); break;
                case 1: Game.goToNextState(new LoadingScreen(Game, 1)); break;
                case 2: Game.goToNextState(new LoadingScreen(Game, 2)); break;
                case 3: Game.goToNextState(new LoadingScreen(Game, 3)); break;
                case 4: Game.goToNextState(new LoadingScreen(Game, 4)); break;
                case 5: Game.goToNextState(new MainMenu(Game, oldKeyboardState, oldGamePadState)); break;
            }
        }
    }
}