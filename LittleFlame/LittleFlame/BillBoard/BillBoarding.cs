using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame.BillBoard
{
    public class BillboardSystem
    {
        // Vertex buffer and index buffer, particle
        // and index arrays
        VertexBuffer verts;
        IndexBuffer ints;
        VertexPositionTexture[] vertices;
        int[] indices;
        // Billboard settings
        int nBillboards;
        Vector2 billboardSize;
        private Texture2D   texture,
                            driedTexture,
                            greenCircle;
                           
        // GraphicsDevice and Effect
        GraphicsDevice graphicsDevice;
        Effect effect;


        private Vector3[] positions;

        public BillboardSystem(GraphicsDevice graphicsDevice, Functions.AssetHolder assets, Texture2D texture, Vector2 billboardSize, Vector3[] positions)
        {
            this.positions = positions;
            this.nBillboards = positions.Length;
            this.billboardSize = billboardSize;
            this.graphicsDevice = graphicsDevice;
            this.texture = texture;
            greenCircle = assets.Load<Texture2D>("circle");
            effect = assets.Load<Effect>("billboardingeffect");
            

            Random r = new Random();


            generateBillBoard(positions);

        }

        private void generateBillBoard(Vector3[] positions)
        {
            // Create vertex and index arrays
            vertices = new VertexPositionTexture[nBillboards * 8];
            indices = new int[nBillboards * 12];
            int x = 0;

            // For each billboard...
            for (int i = 0; i < nBillboards * 8; i += 8)
            {
                Vector3 pos = positions[i / 8];
                Vector3 offsetX = new Vector3(billboardSize.X / 2.0f, billboardSize.Y / 2.0f, 0);
                Vector3 offsetZ = new Vector3(0, offsetX.Y, offsetX.X);

                // Add 4 vertices per rectangle
                vertices[i + 0] = new VertexPositionTexture(pos + new Vector3(-1, 1, 0) * offsetX, new Vector2(0, 0));
                vertices[i + 1] = new VertexPositionTexture(pos + new Vector3(-1, -1, 0) * offsetX, new Vector2(0, 1));
                vertices[i + 2] = new VertexPositionTexture(pos + new Vector3(1, -1, 0) * offsetX, new Vector2(1, 1));
                vertices[i + 3] = new VertexPositionTexture(pos + new Vector3(1, 1, 0) * offsetX, new Vector2(1, 0));
                vertices[i + 4] = new VertexPositionTexture(pos + new Vector3(0, 1, -1) * offsetZ, new Vector2(0, 0));
                vertices[i + 5] = new VertexPositionTexture(pos + new Vector3(0, -1, -1) * offsetZ, new Vector2(0, 1));
                vertices[i + 6] = new VertexPositionTexture(pos + new Vector3(0, -1, 1) * offsetZ, new Vector2(1, 1));
                vertices[i + 7] = new VertexPositionTexture(pos + new Vector3(0, 1, 1) * offsetZ, new Vector2(1, 0));

                // Add 6 indices per rectangle to form four triangles
                indices[x++] = i + 0;
                indices[x++] = i + 3;
                indices[x++] = i + 2;
                indices[x++] = i + 2;
                indices[x++] = i + 1;
                indices[x++] = i + 0;
                indices[x++] = i + 0 + 4;
                indices[x++] = i + 3 + 4;
                indices[x++] = i + 2 + 4;
                indices[x++] = i + 2 + 4;
                indices[x++] = i + 1 + 4;
                indices[x++] = i + 0 + 4;
            }

            // Create and set the vertex buffer
            verts = new VertexBuffer(graphicsDevice,
            typeof(VertexPositionTexture),
            nBillboards * 8, BufferUsage.WriteOnly);
            verts.SetData<VertexPositionTexture>(vertices);
            
            // Create and set the index buffer
            ints = new IndexBuffer(graphicsDevice,
            IndexElementSize.ThirtyTwoBits,
            nBillboards * 12, BufferUsage.WriteOnly);
            ints.SetData<int>(indices);


        }
 
        private void setEffectParameters(Matrix View, Matrix Projection, Texture2D text2D)
        {
            effect.Parameters["ParticleTexture"].SetValue(text2D);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
        }

        public void Draw(Matrix View, Matrix Projection)
        {
            // Set the vertex and index buffer to the graphics card
            graphicsDevice.SetVertexBuffer(verts);
            graphicsDevice.Indices = ints;
            setEffectParameters(View, Projection, texture);

            //NO ALPHA IN COLORS
            graphicsDevice.BlendState = BlendState.AlphaBlend;
  
            drawOpaquePixels();
            drawTransparentPixels();

            // Reset render states
            graphicsDevice.BlendState = BlendState.Opaque;

            // Un-set the vertex and index buffer
            graphicsDevice.SetVertexBuffer(null);
            graphicsDevice.Indices = null;
        }


        private void drawOpaquePixels()
        {
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.Parameters["AlphaTest"].SetValue(true);
            effect.Parameters["AlphaTestGreater"].SetValue(true);
            drawBillboards();
        }

        private void drawTransparentPixels()
        {
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            effect.Parameters["AlphaTest"].SetValue(true);
            effect.Parameters["AlphaTestGreater"].SetValue(false);
            drawBillboards();
        }


        private void drawBillboards()
        {
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nBillboards * 8, 0, nBillboards * 4);
        }
    }
}
