using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace LittleFlame.Water
{
    public class Wave
    {
        private VertexPositionNormalTexture[] vertices;
        private Vector3 position;
        private Effect effect;
        private Texture2D texture;
        private Game game;
        private Vector3 upperLeft, upperRight, lowerLeft, lowerRight;

        //Time
        public float totalTime;
        private float endTime;
        private float time;

        //Alpha
        private double alpha;
        private float endAlpha;

        private float waveSpeed;
        private Vector2 sizeWave;

        public Wave(Game game, float waveSpeed, Vector2 sizeWave, Vector3 position, Texture2D texture, float startAlpha, Effect effect)
        {
            this.game = game;
            this.waveSpeed = waveSpeed;
            this.sizeWave = sizeWave;
            this.position = position;
            this.texture = texture;
            this.waveSpeed = waveSpeed; 
            this.effect = effect;
            this.alpha = startAlpha;
            
            endTime = 10;
            endAlpha = 5;           
            vertices = new VertexPositionNormalTexture[4];
            time = startAlpha * 10;
            CalcVertices();
            FillVertices();
        }

        private void CalcVertices()
        {
            upperLeft = new Vector3(position.X - (sizeWave.X), position.Y, position.Z + (sizeWave.Y));
            upperRight = new Vector3(position.X + (sizeWave.X), position.Y, position.Z + (sizeWave.Y));
            lowerLeft = new Vector3(position.X - (sizeWave.X), position.Y, position.Z - (sizeWave.Y));
            lowerRight = new Vector3(position.X + (sizeWave.X), position.Y, position.Z - (sizeWave.Y));
        }

        private void FillVertices()
        {
            //Used for where the textures are going to be.
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1);
            Vector2 textureLowerRight = new Vector2(1, 1);

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

        public void Update(GameTime gameTime)
        {

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (time >= 1 && time <= endAlpha){
                if (alpha < 1)
                    alpha += 0.005f;
                else
                    alpha = 1;
            }else if (time > endAlpha && time < endTime - 2){
                if (alpha > 0)
                    alpha -= 0.006f;
                else
                    alpha = 0;
            }else if (time >= endTime){
                time = 0;
            }


        }

        public void Unload()
        {
            
        }

        public void Draw(GameTime gameTime, Camera.Camera camera, States.Level level)
        {
            //Set the technique we will use
            effect.CurrentTechnique = effect.Techniques["Waves"];

            //Set the parameters of the effects
            effect.Parameters["waveTexture"].SetValue(texture);
            effect.Parameters["World"].SetValue(level.World);
            effect.Parameters["View"].SetValue(camera.viewMatrix);
            effect.Parameters["Projection"].SetValue(camera.projectionMatrix);
            effect.Parameters["time"].SetValue(time);
            effect.Parameters["alpha"].SetValue((float)alpha);

            //Apply the technique
            effect.CurrentTechnique.Passes[0].Apply();

            //Draw the vertices
            game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
