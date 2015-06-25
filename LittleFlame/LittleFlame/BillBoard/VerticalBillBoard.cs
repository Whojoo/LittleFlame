using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace LittleFlame.BillBoard
{
    public class VerticalBillBoard : DrawableGameComponent
    {
        VertexBuffer treeVertexBuffer;
        VertexDeclaration treeVertexDeclaration;

        private Vector2 size;
        private Vector3 origin, upperLeft, upperRight, lowerLeft, lowerRight, upperLeft2, upperRight2, lowerLeft2, lowerRight2;
        private Texture2D texture;
        private AlphaTestEffect effect;
        private GraphicsDevice graphicsDevice;
        private int xRowTextures, yRowTextures;
        private ContentManager content;
        private Camera.Camera camera;
        private States.Level level;
        private Effect bbEffect;
        private Game1 game;

        public VerticalBillBoard(Vector3 origin, float height, Vector2 size, Texture2D texture, int xRowTextures, int yRowTextures, GraphicsDevice graphicsDevice, ContentManager content, Game1 game)
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

            camera = (Camera.Camera)game.Services.GetService(typeof(Camera.Camera));
            level = (States.Level)game.Services.GetService(typeof(States.Level));

            bbEffect = game.Assets.Load<Effect>("bbeffect");

            //Calculates were the corner vectors of the quad are qoing to be
            CalcVertices();

            FillVertices();
        }

        private void CalcVertices()
        {
            upperLeft = new Vector3(origin.X - (size.X / 2), origin.Y + (size.Y / 2), origin.Z);
            upperRight = new Vector3(origin.X + (size.X / 2), origin.Y + (size.Y / 2), origin.Z);
            lowerLeft = new Vector3(origin.X - (size.X / 2), origin.Y - (size.Y / 2), origin.Z);
            lowerRight = new Vector3(origin.X + (size.X / 2), origin.Y - (size.Y / 2), origin.Z);

            upperLeft2 = new Vector3(origin.X, origin.Y + (size.Y / 2), origin.Z - (size.X / 2));
            upperRight2 = new Vector3(origin.X, origin.Y + (size.Y / 2), origin.Z + (size.X / 2));
            lowerLeft2 = new Vector3(origin.X, origin.Y - (size.Y / 2), origin.Z - (size.X / 2));
            lowerRight2 = new Vector3(origin.X, origin.Y - (size.Y / 2), origin.Z +(size.X / 2));
        }

        private void FillVertices()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[12];

            int i = 0;

            vertices[i++] = new VertexPositionTexture(lowerLeft, new Vector2(0, 0));
            vertices[i++] = new VertexPositionTexture(upperLeft, new Vector2(1, 0));
            vertices[i++] = new VertexPositionTexture(upperRight, new Vector2(1, 1));

            vertices[i++] = new VertexPositionTexture(lowerLeft, new Vector2(0, 0));
            vertices[i++] = new VertexPositionTexture(upperRight, new Vector2(1, 1));
            vertices[i++] = new VertexPositionTexture(lowerRight, new Vector2(0, 1));

            vertices[i++] = new VertexPositionTexture(lowerLeft2, new Vector2(0, 0));
            vertices[i++] = new VertexPositionTexture(upperLeft2, new Vector2(1, 0));
            vertices[i++] = new VertexPositionTexture(upperRight2, new Vector2(1, 1));

            vertices[i++] = new VertexPositionTexture(lowerLeft2, new Vector2(0, 0));
            vertices[i++] = new VertexPositionTexture(upperRight2, new Vector2(1, 1));
            vertices[i++] = new VertexPositionTexture(lowerRight2, new Vector2(0, 1));

            VertexDeclaration vertexDeclaration = VertexPositionTexture.VertexDeclaration;
            treeVertexBuffer = new VertexBuffer(graphicsDevice, vertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            treeVertexBuffer.SetData(vertices);
            treeVertexDeclaration = vertexDeclaration;
        }


        public override void Draw(GameTime gameTime)
        {
            
            bbEffect.CurrentTechnique = bbEffect.Techniques["CylBillboard"];
            bbEffect.Parameters["xWorld"].SetValue(level.World);
            bbEffect.Parameters["xView"].SetValue(level.Cam.viewMatrix);
            bbEffect.Parameters["xProjection"].SetValue(level.Cam.projectionMatrix);
            bbEffect.Parameters["xCamPos"].SetValue(camera.CamPos);
            bbEffect.Parameters["xAllowedRotDir"].SetValue(new Vector3(1, 0, 1));
            bbEffect.Parameters["xBillboardTexture"].SetValue(texture);
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            
            foreach (EffectPass pass in bbEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.SetVertexBuffer(treeVertexBuffer);
                int noVertices = treeVertexBuffer.VertexCount;
                int noTriangles = noVertices / 3;
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
            }
            graphicsDevice.BlendState = BlendState.Opaque;
            
            base.Draw(gameTime);
        }

    }
}
