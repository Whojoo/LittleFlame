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

namespace LittleFlame
{
    public class Game1 : Game
    {
        States.State currentState;
        States.State previousState;
        States.State previousPreviousState;

        // Graphics
        GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        GamerServicesComponent gamerService;
        public GamerServicesComponent GamerService
        {
            get { return gamerService; }
            set { gamerService = value; }
        }

        public Functions.AssetHolder Assets { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Set the game to a 1080 × 720 window
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            this.Assets = new Functions.AssetHolder(this.Content);
            //Load models.
            this.Assets.SaveAsset<Model>("tree", "Models/Model/three 3");
            this.Assets.SaveAsset<Model>("coal", "Models/Coal v4");
            this.Assets.SaveAsset<Model>("meteor", "Models/Model/meteor");
            this.Assets.SaveAsset<Model>("star", "Models/Model/star");
            this.Assets.SaveAsset<Model>("stone", "Models/Model/stoneObstacle");
            this.Assets.SaveAsset<Model>("eyes", "Models/Model/eyes");
            this.Assets.SaveAsset<Model>("skysphere", "Models/meteoriet");

            //Load textures.
            #region Backgrounds
            //Backgrounds.
            this.Assets.SaveAsset<Texture2D>("credits", "Textures/Menu/credits");
            this.Assets.SaveAsset<Texture2D>("gameover", "Textures/Menu/gameover");
            this.Assets.SaveAsset<Texture2D>("spacebackground", "Textures/Menu/spacebackground");
            this.Assets.SaveAsset<Texture2D>("greenhills", "Textures/Menu/greenhills");
            #endregion
            #region Menu level buttons
            //Menu level buttons.
            this.Assets.SaveAsset<Texture2D>("one", "Textures/Menu/greenhillsbutton");
            this.Assets.SaveAsset<Texture2D>("onesel", "Textures/Menu/greenhillsbuttonselected");
            this.Assets.SaveAsset<Texture2D>("two", "Textures/Menu/firststeps");
            this.Assets.SaveAsset<Texture2D>("twosel", "Textures/Menu/firststepsselected");
            this.Assets.SaveAsset<Texture2D>("three", "Textures/Menu/forestfire");
            this.Assets.SaveAsset<Texture2D>("threesel", "Textures/Menu/forestfireselected");
            this.Assets.SaveAsset<Texture2D>("four", "Textures/Menu/readyfortakeoff");
            this.Assets.SaveAsset<Texture2D>("foursel", "Textures/Menu/readyfortakeoffselected");
            this.Assets.SaveAsset<Texture2D>("five", "Textures/Menu/finale");
            this.Assets.SaveAsset<Texture2D>("fivesel", "Textures/Menu/finaleselected");
            #endregion
            #region GUI
            //GUI.
            this.Assets.SaveAsset<Texture2D>("lightbeam", "GUI/LightBeam");
            this.Assets.SaveAsset<Texture2D>("circletree", "GUI/CircleTree");
            this.Assets.SaveAsset<Texture2D>("circle", "GUI/Circle");
            #endregion
            #region Water
            //Water.
            this.Assets.SaveAsset<Texture2D>("wave", "Textures/Water/waves");
            this.Assets.SaveAsset<Texture2D>("water", "Textures/Water/baseWater");
            #endregion
            #region Terrain textures
            //Terrain textures.
            this.Assets.SaveAsset<Texture2D>("ash", "Textures/Ground/rock"); //Terrain ash.
            this.Assets.SaveAsset<Texture2D>("sand", "Textures/Ground/detailsand"); //Terrain sand.
            this.Assets.SaveAsset<Texture2D>("grass", "Textures/Ground/simplewetgrass"); //Terrain grass.
            this.Assets.SaveAsset<Texture2D>("drygrass", "Textures/Ground/simplegrass"); //Terrain drygrass.
            this.Assets.SaveAsset<Texture2D>("rock", "Textures/Ground/rock"); //Terrain rock.
            this.Assets.SaveAsset<Texture2D>("mud", "Textures/Ground/mud"); //Terrain mud.
            this.Assets.SaveAsset<Texture2D>("flowers", "Textures/Ground/wetflowers"); //Terrain flowers.
            this.Assets.SaveAsset<Texture2D>("drygrassflowers", "Textures/Ground/flowers"); //Terrain drygrassflowers.
            #endregion
            #region Height-, Texture and Modelmaps
            //Height-, Texture- and Modelmaps.
            //Tutorial.
            this.Assets.SaveAsset<Texture2D>("MMtutorial", "Textures/Modelmaps/MMTutorial");
            this.Assets.SaveAsset<Texture2D>("HMtutorial", "Textures/Heightmaps/HMTutorial");
            this.Assets.SaveAsset<Texture2D>("TMtutorial", "Textures/Texturemaps/TMTutorial");
            this.Assets.SaveAsset<Texture2D>("SMtutorial", "Textures/ScoreMaps/SMTutorial");
            //LevelZero.
            this.Assets.SaveAsset<Texture2D>("MMzero", "Textures/Modelmaps/MMLevelZero");
            this.Assets.SaveAsset<Texture2D>("HMzero", "Textures/Heightmaps/HMLevelZero");
            this.Assets.SaveAsset<Texture2D>("TMzero", "Textures/Texturemaps/TMLevelZero");
            this.Assets.SaveAsset<Texture2D>("SMzero", "Textures/ScoreMaps/SMLevelZero");
            //LevelOne.
            this.Assets.SaveAsset<Texture2D>("MMone", "Textures/Modelmaps/MMLevelOne");
            this.Assets.SaveAsset<Texture2D>("HMone", "Textures/Heightmaps/HMLevelOne");
            this.Assets.SaveAsset<Texture2D>("TMone", "Textures/Texturemaps/TMLevelOne");
            this.Assets.SaveAsset<Texture2D>("SMone", "Textures/ScoreMaps/SMLevelOne");
            //LevelTwo.
            this.Assets.SaveAsset<Texture2D>("MMtwo", "Textures/Modelmaps/MMLevelTwo");
            this.Assets.SaveAsset<Texture2D>("HMtwo", "Textures/Heightmaps/HMLevelTwo");
            this.Assets.SaveAsset<Texture2D>("TMtwo", "Textures/Texturemaps/TMLevelTwo");
            this.Assets.SaveAsset<Texture2D>("SMtwo", "Textures/ScoreMaps/SMLevelTwo");
            //LevelThree.
            this.Assets.SaveAsset<Texture2D>("MMthree", "Textures/Modelmaps/MMLevelThree");
            this.Assets.SaveAsset<Texture2D>("HMthree", "Textures/Heightmaps/HMLevelThree");
            this.Assets.SaveAsset<Texture2D>("TMthree", "Textures/Texturemaps/TMLevelThree");
            this.Assets.SaveAsset<Texture2D>("SMthree", "Textures/ScoreMaps/SMLevelThree");
            #endregion
            #region Controller spritesheets
            //Controller spritesheets.
            this.Assets.SaveAsset<Texture2D>("tutleftstick", "GUI/ControllerLeftStick");
            this.Assets.SaveAsset<Texture2D>("tutrightstick", "GUI/ControllerRightStick");
            #endregion
            #region Particles
            //Particles.
            this.Assets.SaveAsset<Texture2D>("fireparticle", "Textures/Particles/fire");
            this.Assets.SaveAsset<Texture2D>("meteorparticle", "Textures/Particles/meteor");
            this.Assets.SaveAsset<Texture2D>("sparkparticle", "Textures/Particles/spark");
            this.Assets.SaveAsset<Texture2D>("spreadparticle", "Textures/Particles/flamespread");
            #endregion
            #region Player hud
            //Player hud.
            this.Assets.SaveAsset<Texture2D>("healthbar", "Textures/healthbar");
            this.Assets.SaveAsset<Texture2D>("lockedmeteorhud", "Textures/Hud/lockedMeteorHud");
            this.Assets.SaveAsset<Texture2D>("meteorhud", "Textures/Hud/meteorHud");
            #endregion

            //Load songs.
            this.Assets.SaveAsset<Song>("story", "Music/story");
            this.Assets.SaveAsset<Song>("credits", "Music/credits");
            this.Assets.SaveAsset<Song>("flamesmall", "Music/flame2v1");
            this.Assets.SaveAsset<Song>("flamemedium", "Music/flame2v2");
            this.Assets.SaveAsset<Song>("flamebig", "Music/flame on! grass");

            //Load soundeffects.
            this.Assets.SaveAsset<SoundEffect>("collectmeteor", "Music/Soundeffects/collect meteor");
            this.Assets.SaveAsset<SoundEffect>("treefall", "Music/Soundeffects/tree fall");
            this.Assets.SaveAsset<SoundEffect>("watersplash", "Music/Soundeffects/water-splash-3");
            this.Assets.SaveAsset<SoundEffect>("burnsomething", "Music/Soundeffects/matches-1");
            this.Assets.SaveAsset<SoundEffect>("collectstar", "Music/Soundeffects/collect star");

            //Load effects.
            this.Assets.SaveAsset<Effect>("bbeffect", "Effects/bbEffect");
            this.Assets.SaveAsset<Effect>("sky", "Effects/SkyEffect");
            this.Assets.SaveAsset<Effect>("particle", "Effects/ParticleEffect");
            this.Assets.SaveAsset<Effect>("effect", "Effects/Effect");
            this.Assets.SaveAsset<Effect>("terrain", "Effects/TerrainEffect");
            this.Assets.SaveAsset<Effect>("water", "Effects/waterEffect");
            this.Assets.SaveAsset<Effect>("wave", "Effects/waterEffect");

            //Load texturecubes.
            this.Assets.SaveAsset<TextureCube>("sky", "Textures/skybox/greenhillscubemap");

            //Load spritefonts.
            this.Assets.SaveAsset<SpriteFont>("font", "SpriteFont1");

            Window.Title = "Little Flame";

#if XBOX
            gamerService = new GamerServicesComponent(this);
            Components.Add(gamerService);
#endif

            base.Initialize();
        }

        protected override void LoadContent()
        {
            goToNextState(new States.MainMenu(this, new KeyboardState(), new GamePadState()));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            currentState.Draw(gameTime);
        }

        public void goToNextState(States.State state)
        {
            for (int i = this.Components.Count - 1; i >= 0; i--) {
                if (this.Components[i] is GameComponent) {
                    GameComponent temp = (GameComponent)this.Components[i];
                    temp.Dispose();
                }
            }
            currentState = state;
            currentState.Initialize();
            currentState.LoadContent();
        }

        public void goToPauseState()
        {
            States.State temp = currentState;
            currentState = new States.PauseMenu(this, Keyboard.GetState(), GamePad.GetState(PlayerIndex.One), "", temp);
            currentState.Initialize();
            currentState.LoadContent();
        }

        public void goToBackFromPauseState(States.State previousState)
        {
            States.State tempState = previousState;
            currentState = tempState;
        }
    }
}