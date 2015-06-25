using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LittleFlame.States.GreenHills;

namespace LittleFlame.States
{
    class MainMenu : Menu
    {
        string lastsavename = "";

        public MainMenu(Game1 _game, KeyboardState _oldKeyboardState, GamePadState _oldGamePadState)
            : base(_game, _oldKeyboardState, _oldGamePadState)
        {

        }

        public override void LoadContent()
        {
            font = Game.Assets.Load<SpriteFont>("font");
            background = Game.Assets.Load<Texture2D>("greenhills");
            backgroundPosition = new Vector2(0, -304);
            backgroundEffect = SpriteEffects.FlipHorizontally;

#if WINDOWS
            if (File.Exists("lastsave.txt"))
            {
                FileStream fs = new FileStream("lastsave.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                lastsavename = sr.ReadLine();
                Console.WriteLine("Last save name is called: " + lastsavename);

                sr.Close();
                fs.Close();
            }
            else
            {
                Console.WriteLine("File not found");
            }
#endif

#if XBOX
            Save.GlobalData gd = new Save.GlobalData();
            gd.InitiateLoad(Game, "global");
            if (gd.Lastsave != null) lastsavename = gd.Lastsave;
#endif

            int xPos = Game.GraphicsDevice.Viewport.Width - 300;
            int yPos = Game.GraphicsDevice.Viewport.Height - 220;

            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "New Game", new Vector2(xPos, yPos), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Load Last Save", new Vector2(xPos, yPos + 30), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Level Select", new Vector2(xPos, yPos + 60), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Credits", new Vector2(xPos, yPos + 90), font));
            entries.Add(new MenuObjects.MenuEntry(Game, SpriteBatch, "Quit game", new Vector2(xPos, yPos + 120), font));

            base.LoadContent();
        }

        protected override void selectCurrentItem()
        {
            switch (selectedEntry.Index)
            {
                case 0: Game.goToNextState(new LoadingScreen(Game, 0)); break;
                case 1:
                    {
                        if (lastsavename != "") Game.goToNextState(new LoadingScreen(Game, lastsavename));
                        else Game.goToNextState(new LoadingScreen(Game, 0));
                    }
                    break;
                case 2: Game.goToNextState(new GreenHillsSelection(Game, keyboardState, gamePadState)); break;
                case 3: Game.goToNextState(new Credits(Game, keyboardState, gamePadState)); break;
                case 4: Game.Exit(); break;
            }
        }
    }
}
