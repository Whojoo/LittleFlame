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
using LittleFlame.Particles;
using System.Diagnostics;
using LittleFlame.Models;

#endregion

namespace LittleFlame.States
{
    public class Level : State
    {

        #region Fields
        //Song
        public const int SMALLMEDIUMBORDER = 200;
        public const int MEDIUMBIGBORDER = 350;
        protected Song smallFireSong;
        protected Song mediumFireSong;
        protected Song bigFireSong;
        protected Song currentlyPlaying;
        protected float songPlayTimeCounter;

        // Terrain
        protected Terrain terrain;
        protected Vector3 midTerrain;

        //Water
        private WaterBase water;
        protected float waterLevel;
        protected String waveKey;
        protected String baseWaterKey;
        protected int baseWaterRowX;
        protected int baseWaterRowY;
        protected int amountWaves;

        //Sky
        private Sky sky;
        protected String skyTextureKey;

        // Camera
        public static Camera.Camera cam;
        private float camSpeed;
        private bool devCamOn;
        private Vector3 camStartPos;
        private float camSwitchDelay;

        // Switching
        private float lfStateSwitchDelay;

        // Flame  
        public static LittleFlame player;
        private float treeClimb;

        // Flame Size
        private float waterSizeMod;

        KeyboardState oldState;
        GamePadState oldGamepadState;

        private Matrix worldMatrix;

        // Sound
        private SoundEffectInstance treeFallInstance;
        private SoundEffectInstance collectMeteorInstance;
        private SoundEffectInstance waterSplash;
        private SoundEffectInstance burnSomething;
        private SoundEffectInstance collectStarInstance;

        // Models
        protected List<Models.LFModel> models;

        // Random
        Random random = new Random();

        // Saving
        protected string loadName;
        protected string saveName;

        // Pause state saveholders
        private LittleFlame playerHolder;

        // TEMP
        RasterizerState rs;

        //Score.
        protected float burnScore;
        protected float timeScore;
        protected int collectablesFound; 
        protected Score.Score score;
        protected String scoremapKey;

        //Level strings
        protected String levelAsset;
        private String heightmapKey;
        private String texturemapKey;
        private String modelmapKey;

        //Position of the player at start
        protected Vector3 startPosition = Vector3.Zero;

        private bool GameSaveRequested = false;

        #endregion

        #region Constructor

        public Level(Game1 _game, string loadName)
            : base(_game)
        {
            this.loadName = loadName;
            this.timeScore = 0;
        }

        #endregion

        #region Initialize

        public override void Initialize()
        {
            heightmapKey = "HM" + levelAsset;
            texturemapKey = "TM" + levelAsset;
            modelmapKey = "MM" + levelAsset;
            scoremapKey = "SM" + levelAsset;

            // Camera
            camStartPos = new Vector3(0.0f, 40.0f, 0.0f);
            camSpeed = 50.0f;
            devCamOn = true;
            cam = new Camera.DevelopCamera(camStartPos, camSpeed, Game.GraphicsDevice);
            if (Game.Services.GetService(typeof(Camera.Camera)) == null)
                Game.Services.AddService(typeof(Camera.Camera), cam);

            //Add the level service
            if (Game.Services.GetService(typeof(States.Level)) == null) {
                Game.Services.AddService(typeof(States.Level), this);
            } else {
                Game.Services.RemoveService(typeof(States.Level));
                Game.Services.AddService(typeof(States.Level), this);
            }

            // Models
            models = new List<Models.LFModel>();

            //Set water
            waveKey = "wave";
            baseWaterKey = "water";
            baseWaterRowX = 10;
            baseWaterRowY = 10;
            amountWaves = 400;

            skyTextureKey = "sky";

            if (this.timeScore == 0) {
                throw new NotImplementedException("Initialise the timeScore variable in Level's subclass.");
            }
            this.collectablesFound = 0;

            // Water 
            waterLevel = 4.5f;
            waterSizeMod = -3;

            worldMatrix = Matrix.CreateTranslation(0, 0, 0);
            //Create the terrain
            terrain = new Terrain(Game, heightmapKey, texturemapKey, this);
            Game.Components.Add(terrain);

            //Make a terrain service
            if (Game.Services.GetService(typeof(Terrain)) != null)
                Game.Services.RemoveService(typeof(Terrain));
            Game.Services.AddService(typeof(Terrain), terrain);

            score = new Score.Score(Game.Assets.Load<Texture2D>(scoremapKey), terrain);

            base.Initialize();
        }

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            switchCamera();

            midTerrain = new Vector3(terrain.terrainWidth / 2, waterLevel, terrain.terrainLength / 2);

              //Create water
            water = new WaterBase(midTerrain, waterLevel, new Vector2(terrain.TerrainWidth, terrain.TerrainLength), baseWaterKey, waveKey, baseWaterRowX, baseWaterRowY, "water", amountWaves, Game);
            Game.Components.Add(water);

            //Adding the models
            Models.AddModels addModels = new Models.AddModels(Game, models, terrain, modelmapKey, Game.Content);

            foreach (Models.LFModel model in models)
                if (!Game.Components.Contains(model))
                    Game.Components.Add(model);

            //Load songs.
            this.smallFireSong = Game.Assets.Load<Song>("flamesmall");
            this.mediumFireSong = Game.Assets.Load<Song>("flamemedium");
            this.bigFireSong = Game.Assets.Load<Song>("flamebig");
            this.songPlayTimeCounter = 0;
            this.currentlyPlaying = this.smallFireSong;
            MediaPlayer.Play(this.currentlyPlaying);

            //Load Soundeffects.
            SoundEffect s = Game.Assets.Load<SoundEffect>("collectmeteor");
            this.collectMeteorInstance = s.CreateInstance();
            s = Game.Assets.Load<SoundEffect>("treefall");
            this.treeFallInstance = s.CreateInstance();
            s = Game.Assets.Load<SoundEffect>("watersplash");
            this.waterSplash = s.CreateInstance();
            s = Game.Assets.Load<SoundEffect>("burnsomething");
            this.burnSomething = s.CreateInstance();
            s = Game.Assets.Load<SoundEffect>("collectstar");
            this.collectStarInstance = s.CreateInstance();

            foreach (Models.LFModel model in models) {
                if (model is Coal) {
                    Vector3 pos = model.Position + new Vector3(0, 0.5f, -1.5f);
                    BillBoard.HorizontalBillboarding circle = new BillBoard.HorizontalBillboarding(pos, pos.Y, new Vector2(6), Game.Assets.Load<Texture2D>("circle"), 1, 1, Game.GraphicsDevice, Game.Content, Game);
                    Game.Components.Add(circle);
                } else if (model is Meteoriet) {
                    Vector3 pos = model.Position + new Vector3(0, 32, 0);
                    BillBoard.VerticalBillBoard beam = new BillBoard.VerticalBillBoard(pos, pos.Y, new Vector2(6, 50), Game.Assets.Load<Texture2D>("lightbeam"), 1, 1, Game.GraphicsDevice, Game.Content, Game);
                    Game.Components.Add(beam);
                } else if (model is Tree && this is States.GreenHills.Tutorial) {
                    Vector3 pos = model.Position + new Vector3(0, 0.5f, 0f);
                    BillBoard.HorizontalBillboarding circle = new BillBoard.HorizontalBillboarding(pos, pos.Y, new Vector2(7), Game.Assets.Load<Texture2D>("circletree"), 1, 1, Game.GraphicsDevice, Game.Content, Game);
                    Game.Components.Add(circle);
                }
            }

            foreach (Models.LFModel model in models) {
                if (model is Meteoriet) {
                    Vector3 pos = model.Position + new Vector3(0.5f, 0.5f, 0f);
                    BillBoard.HorizontalBillboarding circle = new BillBoard.HorizontalBillboarding(pos, pos.Y, new Vector2(8), Game.Assets.Load<Texture2D>("circle"), 1, 1, Game.GraphicsDevice, Game.Content, Game);
                    Game.Components.Add(circle);
                }
            }

            // If there is a load file, load the game from that point.
            if (loadName != "") {
                loadProgress(loadName);
            }

            //Set the player's position
            player.Position = this.terrain.GetHeightAtPosition(startPosition.X, startPosition.Z, 0);

            //Create the sky
            sky = new Sky(Game, Game.Assets.Load<TextureCube>("sky"));
            Game.Components.Add(sky);
        }

        public override void UnloadContent()
        {
            playerHolder = player;

            base.UnloadContent();
        }

        public override void ReloadContent()
        {
            player = playerHolder;

            base.ReloadContent();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            updateControls();
            cam.Update(gameTime, player.Position);
            updateDelayTimers(gameTime);
            if (!devCamOn) modelInteraction(gameTime);
            if (player.WindBlowing && !player.IsInTree && player.CurrentState == LittleFlame.Flames.NORMAL)
                underWater(player.Position.Y);
            levelBoundaries(player.Position.X, player.Position.Z);
            if (player.WindBlowing && !player.IsInTree && player.CurrentState == LittleFlame.Flames.NORMAL) {
                terrain.BurningField(player.Position, gameTime, Game, this);
            }
            this.PlaySong(gameTime);
            startPosition = player.Position;

            if (this.timeScore < 0) {
                this.timeScore = 0;
            } else if (this.timeScore > 0) {
                this.timeScore -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
#if WINDOWS
            if (GameSaveRequested == true) {
                GameSaveRequested = false;
                //saveProgress(saveName);
            }
#endif

#if XBOX
            if ((!Guide.IsVisible) && (GameSaveRequested == true))
            {
                GameSaveRequested = false;
                //saveProgress(saveName);
            }
#endif

            base.Update(gameTime);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            Matrix worldMatrix = Matrix.CreateTranslation(0, 0, 0);
            resetGraphics();
            DrawSprites(gameTime);
            resetGraphics();
            endGame();
            if (player != null && player.CurrentState == LittleFlame.Flames.SPARK) {
                terrain.OverSand(Game, waterLevel);
            }
            base.Draw(gameTime);
        }

        protected virtual void DrawSprites(GameTime gameTime)
        {

        }

        #endregion

        #region Camera Control

        private void switchCamera()
        {
            Vector3 _position = cam.CamPos;
            if (devCamOn && camSwitchDelay == 0) {
                devCamOn = false;
                Game.Services.RemoveService(typeof(Camera.Camera));
                cam = new Camera.LFCamera(_position, camSpeed, Game.GraphicsDevice);
                Game.Services.AddService(typeof(Camera.Camera), cam);
                if (!Game.Components.Contains(player)) {
                    player = new LittleFlame(Game, startPosition, devCamOn, this);
                    player.DevCamOn = false;
                    if (Game.Services.GetService(typeof(LittleFlame)) == null) {
                        Game.Services.AddService(typeof(LittleFlame), player);
                    } else {
                        Game.Services.RemoveService(typeof(LittleFlame));
                        Game.Services.AddService(typeof(LittleFlame), player);
                    }
                    Game.Components.Add(player);
                }
                camSwitchDelay = 1;
            } else if (camSwitchDelay == 0) {
                devCamOn = player.DevCamOn = true;
                Game.Services.RemoveService(typeof(Camera.Camera));
                cam = new Camera.DevelopCamera(_position, camSpeed, Game.GraphicsDevice);
                Game.Services.AddService(typeof(Camera.Camera), cam);
                if (Game.Components.Contains(player)) {
                    Game.Components.Remove(player);
                    Game.Services.RemoveService(typeof(LittleFlame));
                }
                camSwitchDelay = 1;
            }
        }

        #endregion

        #region Mechanics

        private void updateControls()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                Game.Exit();

            if ((keyboardState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
                || (gamePadState.IsButtonDown(Buttons.Start) && !oldGamepadState.IsButtonDown(Buttons.Start))) {
                    Game.goToPauseState();
                MediaPlayer.Pause();
            }

            if (keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.A))
                switchCamera();

# if WINDOWS
            if (keyboardState.IsKeyDown(Keys.W)) {
                player.WindBlowing = true;
            } else {
                player.WindBlowing = false;
            }
#endif

            oldState = keyboardState;
            oldGamepadState = gamePadState;
        }

        private void updateDelayTimers(GameTime gameTime)
        {
            if (camSwitchDelay > 0)
                camSwitchDelay -= (float)(gameTime.ElapsedGameTime.TotalSeconds);
            else camSwitchDelay = 0;

            if (lfStateSwitchDelay > 0)
                lfStateSwitchDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else lfStateSwitchDelay = 0;
        }

        private void modelInteraction(GameTime gameTime)
        {
            for (int i = models.Count - 1; i >= 0; i--) {
                if (models[i] is Models.Tree) {
                    Models.Tree tree = (Models.Tree)models[i];
                    float maxTreeClimb = tree.TreeHeight;
                    if (!tree.IsBurned && tree.inRangeDistance(player.Position, player.FlameSize)) {
                        if (!player.IsInTree) {
                            player.ChangeBackFromSpark();
                            player.IsInTree = true;
                            PlaySoundEffect(burnSomething, false);
                        }

                        if (player.TreeClimb >= maxTreeClimb) {
                            player.TreeClimb = maxTreeClimb;
                            if (!tree.IsFalling && player.IsInTree && player.WindBlowing) {
                                tree.StartFall(player.Acceleration);
                                this.PlaySoundEffect(this.treeFallInstance, false);
                            }
                        } else {
                            player.TreeClimb += 0.1f;
                            player.Position = terrain.GetHeightAtPosition(tree.Position.X, tree.Position.Z, player.TreeClimb);
                        }
                    }
                } else if (models[i] is Models.Coal) {
                    Models.Coal coal = (Models.Coal)models[i];
                    if (coal.inRangeDistance(player.Position, player.FlameSize)) {
                        player.ChangeToSpark();
                        PlaySoundEffect(burnSomething, false);
                    }
                } else if (models[i] is Models.Meteoriet) {
                    Models.Meteoriet meteor = (Models.Meteoriet)models[i];
                    if (meteor.inRangeDistance(player.Position, player.FlameSize)) {
                        if (meteor.IsTouched == false) {
                            player.meteorHud.UnlockedMeteors += 1;
                            meteor.IsTouched = true;
                            if (!MeteorsTouched()) {
                                GameSaveRequested = true;
                                this.PlaySoundEffect(this.collectMeteorInstance, false);
                            }
                        }
                    }
                
                } else if (models[i] is Models.Star && models[i].inRangeDistance(player.Position, player.FlameSize)) {
                    this.Game.Components.Remove(models[i]);
                    this.models.Remove(models[i]);
                    this.collectablesFound++;
                    this.PlaySoundEffect(this.collectStarInstance, false);
                }
            }
        }

        private void underWater(float posY)
        {
            if (posY <= waterLevel) {
                player.Position = player.LastPosition;
                player.ChangeSize(waterSizeMod);
                PlaySoundEffect(waterSplash, false);
            }
        }

        private void levelBoundaries(float posX, float posZ)
        {
            int offsetBoundaries = 3;
            if (posX >= terrain.terrainWidth - offsetBoundaries || posX <= offsetBoundaries || posZ >= terrain.terrainLength - offsetBoundaries || posZ <= offsetBoundaries)
                player.Position = player.LastPosition;
        }

        private void endGame()
        {
            if (MeteorsTouched()) {
                goToNextLevel();
            }

            if (player != null && player.FlameSize < 50) {
                int levelnumber = 0;
                if (this is States.GreenHills.Tutorial) levelnumber = 0;
                if (this is States.GreenHills.LevelZero) levelnumber = 1;
                if (this is States.GreenHills.LevelOne) levelnumber = 2;
                if (this is States.GreenHills.LevelTwo) levelnumber = 3;
                if (this is States.GreenHills.LevelThree) levelnumber = 4;
                Game.goToNextState(new GameOverScreen(Game, Keyboard.GetState(), GamePad.GetState(0), saveName, levelnumber));
                player.IsKilled = true;
                MediaPlayer.Stop();
            }
        }

        private bool MeteorsTouched()
        {
            bool meteorsTouched = true;
            for (int i = 0; i < models.Count; i++) {
                if (models.ElementAt(i) is Models.Meteoriet) {
                    Models.Meteoriet m = (Meteoriet)models.ElementAt(i);
                    if (!m.IsTouched)
                        meteorsTouched = false;
                }
            }
            return meteorsTouched;
        }

        protected virtual void goToNextLevel()
        {
            player.Unload();
            models.RemoveRange(0, models.Count - 1);
            
        }

        #endregion

        #region Load and Save

        private void loadProgress(string loadName)
        {
            SaveData sd = new SaveData();
            sd.InitiateLoad(Game, loadName);

            Console.WriteLine("Set player position");
            if (sd.PlayerPosition != null) player.Position = sd.PlayerPosition;
            if (sd.PlayerSize != null) player.FlameSize = sd.PlayerSize;
            if (sd.Terrain != null) terrain.TerrainVertices = sd.Terrain;
            if (sd.ModelRotation != null && sd.ModelActivated != null) {
                for (int i = 0; i < models.Count; i++) {
                    models[i].Rotation = sd.ModelRotation[i];
                    if (models[i] is Tree) {
                        Tree m = (Tree)models[i];
                        m.IsBurned = sd.ModelActivated[i];
                    }

                    if (models[i] is Meteoriet) {
                        Meteoriet m = (Meteoriet)models[i];
                        m.IsTouched = sd.ModelActivated[i];
                    }
                }
            }
        }

        private void saveProgress(string saveName)
        {
            SaveData sd = new SaveData();
            Vector3[] modelRotation = new Vector3[models.Count];
            bool[] modelActivated = new bool[models.Count];
            for (int i = 0; i < models.Count; i++) {
                modelRotation[i] = models[i].Rotation;
                if (models[i] is Tree) {
                    Tree m = (Tree)models[i];
                    modelActivated[i] = m.IsBurned;
                }

                if (models[i] is Meteoriet) {
                    Meteoriet m = (Meteoriet)models[i];
                    modelActivated[i] = m.IsTouched;
                }
            }
            sd.InitiateSave(Game, player.Position, player.FlameSize, terrain.TerrainVertices, modelRotation, modelActivated, saveName);
        }

        #endregion

        #region Graphics and Sound

        /// <summary>
        /// Use this if you want a soundeffect to play looped. Use StopSoundEffect to stop it.
        /// </summary>
        /// <param name="instance">The sound you want to play.</param>
        private void PlaySoundEffect(SoundEffectInstance instance, bool looped)
        {
            if (instance.State != SoundState.Playing && !instance.IsLooped) {
                instance.Play();
                if (instance.IsLooped != looped) {
                    instance.IsLooped = looped;
                }
            }
        }

        /// <summary>
        /// Stop a looped soundeffect.
        /// </summary>
        /// <param name="instance">The sound you want to stop.</param>
        private void StopSoundEffect(SoundEffectInstance instance)
        {
            if (instance.IsLooped) {
                instance.Stop();
                instance.IsLooped = false;
            }
        }

        private void PlaySong(GameTime gameTime)
        {
            this.songPlayTimeCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (player.FlameSize <= SMALLMEDIUMBORDER && this.songPlayTimeCounter >= this.currentlyPlaying.Duration.TotalSeconds) {
                Debug.WriteLine("Switch to small");
                Debug.WriteLine("FlameSize: {0}", player.FlameSize);
                MediaPlayer.Stop();
                this.currentlyPlaying = this.smallFireSong;
                MediaPlayer.Play(this.currentlyPlaying);
                this.songPlayTimeCounter = 0;
            } else if (player.FlameSize >= MEDIUMBIGBORDER && this.songPlayTimeCounter >= this.currentlyPlaying.Duration.TotalSeconds) {
                Debug.WriteLine("Switch to big");
                Debug.WriteLine("FlameSize: {0}", player.FlameSize);
                MediaPlayer.Stop();
                this.currentlyPlaying = this.bigFireSong;
                MediaPlayer.Play(this.currentlyPlaying);
                this.songPlayTimeCounter = 0;
            } else if (this.songPlayTimeCounter >= this.currentlyPlaying.Duration.TotalSeconds) {
                Debug.WriteLine("Switch to medium");
                Debug.WriteLine("FlameSize: {0}", player.FlameSize);
                MediaPlayer.Stop();
                this.currentlyPlaying = this.mediumFireSong;
                MediaPlayer.Play(this.currentlyPlaying);
                this.songPlayTimeCounter = 0;
            }
        }

        private void resetGraphics()
        {
            rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            Game.GraphicsDevice.RasterizerState = rs;
            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        #endregion

        #region Getters and Setters

        public LittleFlame Player
        {
            get { return player; }
            set { player = value; }
        }

        public Camera.Camera Cam
        {
            get { return cam; }
            set { cam = value; }
        }

        public float TreeClimb
        {
            get { return treeClimb; }
            set { treeClimb = value; }
        }

        public Matrix World
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        #endregion

        public Score.Score getScore()
        {
            return score;
        }
    }
}

