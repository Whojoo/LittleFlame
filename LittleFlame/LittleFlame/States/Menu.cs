using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LittleFlame.States.MenuObjects;

namespace LittleFlame.States
{
    class Menu : State
    {
        protected SpriteFont font;
        protected List<MenuObject> entries;
        protected MenuObject selectedEntry;
        protected KeyboardState keyboardState;
        protected GamePadState gamePadState;
        protected KeyboardState oldKeyboardState;
        protected GamePadState oldGamePadState;
        protected Texture2D background;
        protected Vector2 backgroundPosition;
        protected SpriteEffects backgroundEffect;

        public Menu(Game1 _game, KeyboardState _oldKeyboardState, GamePadState _oldGamePadState)
            : base(_game)
        {
            this.oldKeyboardState = _oldKeyboardState;
            this.oldGamePadState = _oldGamePadState;
        }

        public override void Initialize()
        {
            entries = new List<MenuObject>();
        }

        public override void LoadContent()
        {
            for (int i = 0; i < entries.Count; i++)
            {
                entries.ElementAt(i).Index = i;
                if (i == entries.Count - 1)
                    entries.ElementAt(i).Next = entries.ElementAt(0);
                else
                    entries.ElementAt(i).Next = entries.ElementAt(i + 1);

                if (i == 0)
                    entries.ElementAt(i).Previous = entries.ElementAt(entries.Count - 1);
                else
                    entries.ElementAt(i).Previous = entries.ElementAt(i - 1);
            }
            selectedEntry = entries.ElementAt(0);
            selectedEntry.SelectThis();
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
                || (gamePadState.IsButtonDown(Buttons.LeftThumbstickDown) && !oldGamePadState.IsButtonDown(Buttons.LeftThumbstickDown)))
            {
                selectedEntry.Scale = 1;
                if (selectedEntry is MenuEntry) selectedEntry.Color = Color.OrangeRed;
                else selectedEntry.Color = Color.DimGray;
                selectedEntry.DeselectThis();
                selectedEntry = selectedEntry.Next;
                selectedEntry.SelectThis();
            }
            if ((keyboardState.IsKeyDown(Keys.Up) && !oldKeyboardState.IsKeyDown(Keys.Up))
                || (gamePadState.IsButtonDown(Buttons.LeftThumbstickUp) && !oldGamePadState.IsButtonDown(Buttons.LeftThumbstickUp)))
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
            if (background != null)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(background, backgroundPosition, null, Color.White, 0, Vector2.Zero, 1, backgroundEffect, 0);
                SpriteBatch.End();
            }

            for (int i = 0; i < entries.Count; i++)
               entries[i].Draw(gameTime);
        }

        protected virtual void selectCurrentItem()
        {

        }
    }
}
