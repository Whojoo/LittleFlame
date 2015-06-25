using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using LittleFlame.Functions;
using LittleFlame.Models;
using LittleFlame.States;
using Microsoft.Xna.Framework.Audio;
using LittleFlame.Particles;

namespace LittleFlame
{
    public class LittleFlame : DrawableGameComponent
    {
        private const float SPREADTIME = 0.3f;
        private const int ParticlesPerFrame = 1;


        private bool isKilled;
        private float timeTillSpread;
        private Vector3 position;
        private Vector3 lastposition;
        private Flames currentState;
        private Vector3 velocity;
        private Vector3 gravity;
        private Vector3 launchVelocity;
        private Vector3 acceleration;
        private float drag;
        private Vector3 maxVelocity;
        private float velocityMultiplication;
        private Terrain terrain;
        private bool devCamOn;
        private Camera.Camera cam;
        private Vector2 leftThumbstickDeadzone;
        private bool isInTree;
        private bool windBlowing;
        private float treeClimb;
        private FireSpreadManager spreadManager;
        private FlameEyes eyes;
        private FireParticleSystem fireParticles;
        private ParticleSpark sparkParticles;
        private ParticleSystem particles;

        public static bool check = false;
        // Variables for the flame size.
        private float flameSize;
        private int minSize;
        private int maxSize;
        private Level level;
        private SpriteBatch batch;
        private Game1 game;

        // Sand
        Random rnd;
        float sandSparkSizeMod;
        float sandHeight;

        //Hud
        public Player.MeteorHud meteorHud;

        // Particles
        //private Particles.SandParticleSystem sandParticles;
        
        //Flame states
        public enum Flames
        {
            NORMAL = 0,
            SPARK = 1,
            BOOST = 2,
        }

        public LittleFlame(Game1 game, Vector3 pos, bool devCamOn, Level level)
            : base(game)
        {
            this.game = game;
            this.level = level;
            this.Position = pos;
            this.devCamOn = devCamOn;
        }

        

        public override void Initialize()
        {
            this.currentState = Flames.NORMAL;
            this.velocity = new Vector3(0, 0, 0);
            this.gravity = new Vector3(0, 0.00125f, 0);
            this.launchVelocity = new Vector3(0, 0.25f, 0);
            this.drag = 0.6f;
            this.maxVelocity = new Vector3(0.02f, -0.01f, 0.02f);
            this.velocityMultiplication = 1;
            this.leftThumbstickDeadzone = new Vector2(0.1f, 0.1f);
            this.spreadManager = new FireSpreadManager(this.game);
            this.Game.Components.Add(this.spreadManager);
            this.timeTillSpread = SPREADTIME;
            this.isKilled = false;

            //Particles.
            this.fireParticles = new FireParticleSystem(game, Game.Content);
            this.sparkParticles = new ParticleSpark(game, Game.Content);
            this.particles = this.fireParticles;
            this.Game.Components.Add(this.fireParticles);
            this.Game.Components.Add(this.sparkParticles);

            this.fireParticles.DrawOrder = 100;
            this.sparkParticles.DrawOrder = 100;

            //Sets the dimensions of the flame.
            flameSize = 200;
            minSize = 0;
            maxSize = 400;

            rnd = new Random();
            sandSparkSizeMod = -0.7f;
            sandHeight = 7.8f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Model eyeModel = game.Assets.Load<Model>("eyes");
            eyes = new FlameEyes(Game, eyeModel, Vector3.Zero, new Vector3(0.04f,0.04f,0.04f), this);
            this.Game.Components.Add(eyes);

            batch = new SpriteBatch(GraphicsDevice);
            //burningSound = this.Game.Content.Load<SoundEffect>(@"Music\SoundEffects\wind");
            //burningSoundInstance = burningSound.CreateInstance();
            //burningSoundInstance.IsLooped = true;
            //burningSoundInstance.Play();

            this.cam = (Camera.Camera)Game.Services.GetService(typeof(Camera.Camera));
            this.terrain = (Terrain)Game.Services.GetService(typeof(Terrain));

            // Particles
            //sandParticles = new Particles.SandParticleSystem(Game, Game.Content);
            //Game.Components.Add(sandParticles);

            //Add healthbar
            Player.Healthbar healthBar = new Player.Healthbar(Game, Game.Content.Load<Texture2D>("Textures\\healthbar"), new Vector2(10, 10), 20, 300, this);
            //Player.Healthbar healthBar = new Player.Healthbar(game, game.Assets.Load<Texture2D>("healthbar"), new Vector2(10, 10), 20, this);
            Game.Components.Add(healthBar);

            meteorHud = new Player.MeteorHud(game, game.Assets.Load<Texture2D>("lockedmeteorhud"), game.Assets.Load<Texture2D>("meteorhud"), new Vector2(10, 40), new Vector2(20), 3, 5);
            Game.Components.Add(meteorHud);

            base.LoadContent();
        }

        public void Unload()
        {
            this.particles.Dispose();
            this.spreadManager.Unload();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < ParticlesPerFrame; i++) {
                particles.AddParticle(this.position, Vector3.Zero);
            }

            lastposition = position;

            this.Gravity(deltaSeconds);
            this.Move(deltaSeconds);
            this.windBlowing = ThumbstickOutOfDeadzone();

            this.position += this.velocity;
            this.timeTillSpread -= deltaSeconds;
            if (this.currentState == Flames.NORMAL && this.windBlowing && timeTillSpread <= 0) {
                this.spreadManager.addSpread(this.position, this.flameSize);
                this.timeTillSpread = SPREADTIME;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This object's gravity. It uses the terrain class' GetHeightAtPosition() method.
        /// </summary>
        /// <param name="deltaSeconds">The elapsed seconds of the last frame.</param>
        private void Gravity(float deltaSeconds)
        {
            //Check if the flame is hitting the ground or is affected by gravity.
            if (this.position.Y < terrain.GetHeightAtPosition(this.position.X, this.position.Z, 0).Y) {
                this.Position = terrain.GetHeightAtPosition(this.Position.X, this.Position.Z, 0);
                this.velocity.Y *= 0;
                this.ChangeBackFromSpark();
            } else {
                if (!isInTree) {
                    this.velocity -= this.gravity;
                }
            }
        }

        /// <summary>
        /// This Method contains this object's moving abilities. 
        /// The player can use the W button for moving instead of the xbox triggers if there is no xbox controller plugged in.
        /// </summary>
        /// <param name="deltaSeconds">The ellapsed seconds of the last frame.</param>
        private void Move(float deltaSeconds)
        {
            //get the gamePadState.
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState kbState = Keyboard.GetState();

            if (!devCamOn) {
                //Slow flame down when wind isn't blowing.
                if (this.velocity != Vector3.Zero) {
                    this.velocity.X *= this.drag;
                    this.velocity.Z *= this.drag;
                } else
                    this.velocity = Vector3.Zero;

                //First check if the player isnt going to fast and if either trigger is pressed.
                if (this.VelocityLowerThanMaxVelocity() && this.ThumbstickOutOfDeadzone()) {
                    //If there isn't a xbox controller connected, the program will automaticly allow the W key to be used instead.
                    //Acceleration only affects the X and Z values since the flame can't fly by itself.
                    if (gamePadState.IsConnected) {
                        //Matrix used to rotate the acceleration.
                        Matrix mat = Matrix.CreateRotationY(this.cam.CameraYaw / 180.0f * (float)Math.PI);
                        if (this.OutOfUpperOrLowerDeadzone(gamePadState) && this.OutOfLeftOrRichtDeadzone(gamePadState)) {
                            acceleration = new Vector3(-gamePadState.ThumbSticks.Left.X, 0, gamePadState.ThumbSticks.Left.Y) * deltaSeconds;
                        } else {
                            if (this.OutOfLeftOrRichtDeadzone(gamePadState)) { //Controler controls.
                                acceleration = new Vector3(-gamePadState.ThumbSticks.Left.X, 0, 0) * deltaSeconds;
                            }
                            if (this.OutOfUpperOrLowerDeadzone(gamePadState)) {
                                acceleration = new Vector3(0, 0, gamePadState.ThumbSticks.Left.Y) * deltaSeconds;
                            }
                        }
                        if (this.acceleration != Vector3.Zero) {
                            //Rotate the acceleration compared with the current camera position.
                            this.acceleration = Vector3.Transform(this.acceleration, mat);
                            this.acceleration = Vector3.Normalize(this.acceleration) * deltaSeconds * 3.5f;
                        }
                        this.velocity += this.acceleration;
                    } else { //Keyboard controls.
                        //Gives acceleration the same direction as the vector from the camera to the player.
                        acceleration = Vector3.Normalize(this.position - cam.CamPos);
                        this.velocity += this.acceleration;
                    }
                    //Rotate the flame's eyes.
                    this.eyes.Rotate(Vector3.Normalize(this.acceleration));
                    //this.position += this.velocity;
                }
            }
        }

        /// <summary>
        /// Checks if a thumbstick of the controler is pushed further than the deadzone.
        /// </summary>
        /// <returns>True if a thumstick is pushed past the deadzone value, false if neither trigger is pushed past the deadzone value.</returns>
        private bool ThumbstickOutOfDeadzone()
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            KeyboardState kbState = Keyboard.GetState();
            return (this.OutOfLeftOrRichtDeadzone(pad) || this.OutOfUpperOrLowerDeadzone(pad) || kbState.IsKeyDown(Keys.W));
        }

        private bool OutOfLeftOrRichtDeadzone(GamePadState pad)
        {
            return pad.ThumbSticks.Left.X < this.leftThumbstickDeadzone.X * -1 || pad.ThumbSticks.Left.X > this.leftThumbstickDeadzone.X;
        }

        private bool OutOfUpperOrLowerDeadzone(GamePadState pad)
        {
            return pad.ThumbSticks.Left.Y < this.leftThumbstickDeadzone.Y * -1 || pad.ThumbSticks.Left.Y > this.leftThumbstickDeadzone.Y;
        }

        /// <summary>
        /// Checks if the flame is not going to fast on the X and X values.
        /// This methode uses the current Velocity's X and Z length (X * X + Z * Z) and compares it
        /// with the MaxVelocity's X and Z length (X * X + Z * Z) and the negative MaxVelocity's X and Z length.
        /// </summary>
        /// <returns>True if this object hasn't reached max velocity yet, false if it has.</returns>
        private bool VelocityLowerThanMaxVelocity()
        {
            float XZLengthVelocity = (float)Math.Sqrt(this.velocity.X * this.velocity.X * this.velocity.Z * this.velocity.Z);
            float XZLengthMaxVelociy = (float)Math.Sqrt(this.maxVelocity.X * this.maxVelocity.X + this.maxVelocity.Z * this.maxVelocity.Z);
            return XZLengthVelocity < XZLengthMaxVelociy;
        }

        public override void Draw(GameTime gameTime)
        {
            particles.SetCamera(cam.viewMatrix, cam.projectionMatrix);

            //Change the states back for 3d models.
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }

        //Change the size of the flame.
        public void ChangeSize(float _size)
        {
            flameSize += _size;
            if (flameSize < minSize) flameSize = minSize;
            if (flameSize > maxSize) flameSize = maxSize;
        }

        //Changes the flame into a spark.
        public void ChangeToSpark()
        {
            if (currentState != Flames.SPARK) {
                currentState = Flames.SPARK;
                //Lauches this object into the sky.
                this.Velocity = Vector3.Add(this.Velocity, launchVelocity);
                this.particles = this.sparkParticles;
            }

        }

        //Changes the flame from a spark back to a flame.
        public void ChangeBackFromSpark()
        {
            if (currentState == Flames.SPARK) {
                currentState = (int)Flames.NORMAL;
                this.particles = this.fireParticles;
            }
        }

        // This method is called when the player is floating over sand.
        public void overSand()
        {
            const int fireParticlesPerFrame = 3;
            for (int i = 0; i < fireParticlesPerFrame; i++)
            {
                float randomNr = (float)rnd.NextDouble() * 4 * (float)Math.PI;
                float xpos = Position.X + (float)Math.Cos(randomNr) * 2f;
                float zpos = Position.Z + (float)Math.Sin(randomNr) * 2f;
                Vector3 pos = terrain.GetHeightAtPosition(xpos,zpos,0);
                //sandParticles.AddParticle(pos, Vector3.Zero);
            }
            Camera.Camera cam = (Camera.Camera)this.Game.Services.GetService(typeof(Camera.Camera));
            //sandParticles.SetCamera(cam.viewMatrix, cam.projectionMatrix);

            if (Position.Y - terrain.GetHeightAtPosition(Position.X, Position.Z, 0).Y < sandHeight)
            {
                ChangeSize(sandSparkSizeMod);
            }
        }

        #region Properties
        public bool IsKilled
        {
            get { return this.isKilled; }
            set { this.isKilled = value; }
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 LastPosition
        {
            get { return lastposition; }
            set { lastposition = value; }
        }
        public Flames CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public bool DevCamOn
        {
            get { return devCamOn; }
            set { devCamOn = value; }
        }

        public bool IsInTree
        {
            get { return isInTree; }
            set { isInTree = value; }
        }

        public float FlameSize
        {
            get { return flameSize; }
            set { flameSize = value; }
        }
        public bool WindBlowing
        {
            get { return windBlowing; }
            set { windBlowing = value; }
        }
        public float TreeClimb
        {
            get { return treeClimb; }
            set { treeClimb = value; }
        }
        public Vector3 Acceleration
        {
            get { return this.acceleration; }
        }

        #endregion
    }
}
