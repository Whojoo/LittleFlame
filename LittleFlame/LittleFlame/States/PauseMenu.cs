using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LittleFlame.States.GreenHills;
using System.Threading;
using Microsoft.Xna.Framework.Media;

namespace LittleFlame.States
{
    class PauseMenu : Menu
    {
        string lastsavename;
        State lastState;
        IGameComponent[] oldComponents;

        public PauseMenu(Game1 _game, KeyboardState _oldKeyboardState, GamePadState _oldGamePadState, string lastsavename, State lastState)
            : base(_game, _oldKeyboardState, _oldGamePadState)
        {
            this.lastState = lastState;
            this.oldComponents = new IGameComponent[_game.Components.Count];
            for (int i = _game.Components.Count - 1; i >= 0; i--) {
                this.oldComponents[i] = _game.Components[i];
                _game.Components.RemoveAt(i);
            }
            this.lastsavename = lastsavename;
        }

        public override void LoadContent()
        {
            font = Game.Assets.Load<SpriteFont>("font");
            background = Game.Assets.Load<Texture2D>("spacebackground");

            int xPos = 50;
            int yPos = Game.GraphicsDevice.Viewport.Height - 190;

            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Return to Game", new Vector2(xPos, yPos), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Load Last Checkpoint", new Vector2(xPos, yPos + 30), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Quit to Main Menu", new Vector2(xPos, yPos + 60), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Quit game", new Vector2(xPos, yPos + 90), font));

            base.LoadContent();
        }

        protected override void selectCurrentItem()
        {
            switch (selectedEntry.Index)
            {
                case 0: 
                    Game.goToBackFromPauseState(this.lastState); 
                    int i = 0;
                    for (i = Game.Components.Count - 1; i >= 0; i--) {
                        Game.Components.RemoveAt(i);
                    }
                    for (i = this.oldComponents.Length - 1; i >= 0; i--) {
                        Game.Components.Add(this.oldComponents[i]);
                        this.oldComponents[i] = null;
                    }
                    MediaPlayer.Resume(); break;
                case 1: Game.goToNextState(new LoadingScreen(Game, lastsavename)); break;
                case 2: Game.goToNextState(new MainMenu(Game, keyboardState, gamePadState)); break;
                case 3: Game.Exit(); break;
            }
        }
    }
}
