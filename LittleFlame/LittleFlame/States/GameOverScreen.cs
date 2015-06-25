using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LittleFlame.States.GreenHills;
using System.Threading;

namespace LittleFlame.States
{
    class GameOverScreen : Menu
    {
        string lastsavename;
        int levelnumber;

        public GameOverScreen(Game1 _game, KeyboardState _oldKeyboardState, GamePadState _oldGamePadState, string lastsavename, int levelnumber)
            : base(_game, _oldKeyboardState, _oldGamePadState)
        {
            this.lastsavename = lastsavename;
            this.levelnumber = levelnumber;
        }

        public override void LoadContent()
        {
            font = Game.Assets.Load<SpriteFont>("font");
            background = Game.Assets.Load<Texture2D>("gameover");

            int xPos = 50;
            int yPos = Game.GraphicsDevice.Viewport.Height - 160;

            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Restart Level", new Vector2(xPos, yPos + 30), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Return to main menu", new Vector2(xPos, yPos + 60), font));

            base.LoadContent();
        }

        protected override void selectCurrentItem()
        {
            switch (selectedEntry.Index)
            {
                case 0: Game.goToNextState(new LoadingScreen(Game, levelnumber)); break;
                case 1: Game.goToNextState(new States.MainMenu(Game, keyboardState, gamePadState)); break;
            }
        }
    }
}
