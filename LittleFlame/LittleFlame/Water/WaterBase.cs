using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LittleFlame.Water;

namespace LittleFlame
{
    class WaterBase : DrawableGameComponent
    {
        //Water
        private VertexPositionNormalTexture[] vertices;
        private Vector2 size;
        private Vector3 origin, upperLeft, upperRight, lowerLeft, lowerRight;

        //Textures
        private Texture2D baseTexture;
        private String baseTextureKey;
        private Texture2D waveTexture;
        private String waveTextureKey;

        //Effect
        private AlphaTestEffect baseEffect;
        private Effect waveEffect;
        private String effectKey;

        private int xRowTextures, yRowTextures;

        //Random
        private Random randy;

        //Services
        private Camera.Camera camera;
        private States.Level level;

        //Waves class variable
        private Water.Wave[] waves;
        private int waveDensity;

        //Standard
        private GraphicsDevice graphicsDevice;
        private ContentManager content;
        private Game1 game;

        public WaterBase(Vector3 origin, float height, Vector2 size, String baseTextureKey, String waveTextureKey, int xRowTextures, int yRowTextures, string effectKey, int waveDensity, Game1 game)
            : base(game)
        {
            this.game = game;
            this.origin = origin;
            this.size = size;
            this.baseTextureKey = baseTextureKey;
            this.waveTextureKey = waveTextureKey;
            this.xRowTextures = xRowTextures;
            this.yRowTextures = yRowTextures;
            this.graphicsDevice = game.GraphicsDevice;
            this.content = game.Content;
            this.effectKey = effectKey;
            this.waveDensity = waveDensity;
            
            vertices = new VertexPositionNormalTexture[4];

            camera = (Camera.Camera)game.Services.GetService(typeof(Camera.Camera));
            level = (States.Level)game.Services.GetService(typeof(States.Level));

            baseEffect = new AlphaTestEffect(graphicsDevice);

            randy = new Random();

            LoadContent();

            //Calculates were the corner vectors of the quad are qoing to be
            CalcVertices();
            FillVertices();
            LoadWaves();
        }

        protected override void LoadContent()
        {
            waveEffect = game.Assets.Load<Effect>(effectKey);
            baseTexture = game.Assets.Load<Texture2D>(baseTextureKey);
            waveTexture = game.Assets.Load<Texture2D>(waveTextureKey);

            base.LoadContent();
        }

        public void Unload()
        {
            foreach (Wave wave in waves) {
                wave.Unload();
            }
        }

        private void CalcVertices()
        {
            upperLeft = new Vector3(origin.X - (size.X / 2), origin.Y, origin.Z + (size.Y / 2));
            upperRight = new Vector3(origin.X + (size.X / 2), origin.Y, origin.Z + (size.Y / 2));
            lowerLeft = new Vector3(origin.X - (size.X / 2), origin.Y, origin.Z - (size.Y / 2));
            lowerRight = new Vector3(origin.X + (size.X / 2), origin.Y, origin.Z - (size.Y / 2));
        }

        private void FillVertices()
        {
            //Used for where the textures are going to be.
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(xRowTextures, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, yRowTextures);
            Vector2 textureLowerRight = new Vector2(xRowTextures, yRowTextures);

            //Set the positions of the vertices to the calculated position
            vertices[0].Position = lowerLeft;
            vertices[0].TextureCoordinate = textureLowerLeft;
            vertices[1].Position = lowerRight;
            vertices[1].TextureCoordinate = textureLowerRight;
            vertices[2].Position = upperLeft;
            vertices[2].TextureCoordinate = textureUpperLeft;
            vertices[3].Position = upperRight;
            vertices[3].TextureCoordinate = textureUpperRight;
        }

        private void LoadWaves()
        {
            Vector3 posWave = Vector3.Zero;
            waves = new Water.Wave[waveDensity];
            Effect watereffect = game.Assets.Load<Effect>("water");
            int waveSize = 5;
            for (int i = 0; i < waves.Length ; i++){
                posWave = new Vector3((float)randy.Next(0, 384), origin.Y + 0.01f, (float)randy.Next(0, 384));
                waves[i] = new Water.Wave(game, 2, new Vector2(waveSize), posWave,  waveTexture, (float)randy.NextDouble(), watereffect);
           
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Draw the waves
            for (int i = 0; i < waves.Length; i++){
                waves[i].Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            camera = (Camera.Camera)game.Services.GetService(typeof(Camera.Camera));

            graphicsDevice.BlendState = BlendState.Opaque;

            // Set our effect to use the specified texture and camera matrices.
            level = (States.Level)game.Services.GetService(typeof(States.Level));

            //Set the parameters of the effects
            baseEffect.Texture = baseTexture;
            baseEffect.World = level.World;
            baseEffect.View = camera.viewMatrix;
            baseEffect.Projection = camera.projectionMatrix;

            baseEffect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
            graphicsDevice.BlendState = BlendState.AlphaBlend;

            //Draw the waves
            for (int i = 0; i < waves.Length; i++)
            {
                waves[i].Draw(gameTime, camera, level);
            }
            
            base.Draw(gameTime);
        }

        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
    }
}
