using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame.BillBoard
{
    class Horizontal
    {
        private VertexPositionNormalTexture[] vertices;
        private float height;
        private Vector2 size;
        private Vector3 origin, upperLeft, upperRight, lowerLeft, lowerRight;
        private Texture2D waterTexture;
        private AlphaTestEffect effect;
        private GraphicsDevice graphicsDevice;
        private int xRowTextures, yRowTextures;
        private Vector3 midTerrain;
        private float waterLevel;
        private int p;
        private int p_2;
        private Texture2D texture2D;
        private ContentManager contentManager;

        public Horizontal(Vector3 origin, Vector2 size, float height, Texture2D texture, int xRowTextures, int yRowTextures, GraphicsDevice graphicsDevice, ContentManager content)
        {
            vertices = new VertexPositionNormalTexture[4];

            this.height = height;
            this.origin = origin;
            this.size = size;
            //this.waterTexture = texture;
            this.xRowTextures = xRowTextures;
            this.yRowTextures = yRowTextures;
            this.graphicsDevice = graphicsDevice;

            effect = new AlphaTestEffect(graphicsDevice);

            //Calculates were the corner vectors of the quad are qoing to be
            CalcVertices();

            FillVertices();
        }
        private void CalcVertices()
        {
            upperLeft = new Vector3(origin.X - (size.X/ 2), height, origin.Z + (size.Y / 2));
            upperRight = new Vector3(origin.X + (size.X / 2), height, origin.Z + (size.Y / 2));
            lowerLeft = new Vector3(origin.X - (size.X / 2), height, origin.Z - (size.Y / 2));
            lowerRight = new Vector3(origin.X + (size.X / 2), height, origin.Z - (size.Y / 2));
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
            vertices[0].TextureCoordinate= textureLowerLeft;
            vertices[1].Position = lowerRight;
            vertices[1].TextureCoordinate = textureLowerRight;
            vertices[2].Position = upperLeft;
            vertices[2].TextureCoordinate = textureUpperLeft;
            vertices[3].Position = upperRight;
            vertices[3].TextureCoordinate = textureUpperRight;

        }

        public void drawWater(Matrix _world, Matrix _view, Matrix _projection)
        {
            // Set our effect to use the specified texture and camera matrices.
            effect.Texture = waterTexture;

            effect.World = _world;
            effect.View = _view;
            effect.Projection = _projection;

            // Draw the quad.
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
    }
}
