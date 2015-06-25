using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace LittleFlame.BillBoard
{
    
    public class HorizontalBillboarding : DrawableGameComponent
    {
        private VertexPositionNormalTexture[] vertices;
        private Vector2 size;
        private Vector3 origin, upperLeft, upperRight, lowerLeft, lowerRight;
        private Texture2D texture;
        private AlphaTestEffect effect;
        private GraphicsDevice graphicsDevice;
        private int xRowTextures, yRowTextures;
        private ContentManager content;
        private Camera.Camera camera;
        private States.Level level;
        private Game game;

        public HorizontalBillboarding(Vector3 origin, float height, Vector2 size, Texture2D texture, int xRowTextures, int yRowTextures, GraphicsDevice graphicsDevice, ContentManager content, Game game)
            : base(game)
        {
            this.origin = origin;
            this.size = size;
            this.texture = texture;
            this.xRowTextures = xRowTextures;
            this.yRowTextures = yRowTextures;
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.game = game;

            vertices = new VertexPositionNormalTexture[4];

            camera = (Camera.Camera)game.Services.GetService(typeof(Camera.Camera));
            level = (States.Level)game.Services.GetService(typeof(States.Level));

            effect = new AlphaTestEffect(graphicsDevice);

            //Calculates were the corner vectors of the quad are qoing to be
            CalcVertices();

            FillVertices();
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

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.BlendState = BlendState.AlphaBlend;

            // Set our effect to use the specified texture and camera matrices.
            level = (States.Level)game.Services.GetService(typeof(States.Level));

            effect.Texture = texture;
            effect.World = level.World;
            effect.View = level.Cam.viewMatrix;
            effect.Projection = level.Cam.projectionMatrix;

            // Draw the quad.
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);

            graphicsDevice.BlendState = BlendState.Opaque;

            base.Draw(gameTime);
        }

        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
    }
}
