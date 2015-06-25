using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using LittleFlame.States;
using System.Diagnostics;


namespace LittleFlame
{
    /*
     * This is a custom vertex type. We use this because we want to use more then one
     * texture. It look a lot like VertexPositionNormalTexture but it adds the textureWeights.
     */
    public struct VertexMultitextured : IVertexType
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector4 textureCoordinate;
        public Vector4 textures1;
        public Vector4 textures2;

        /*
         * VertexDeclaration defines data per vertex. So you can save the data you want for each vertex
         * Every element in vertexDeclaration should be added by vertexElement
         * 
         * sizeof(float) = 4
         */
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * (3 + 3), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * (3 + 3 + 4), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * (3 + 3 + 4 + 4), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2)
        );

        public VertexMultitextured(Vector3 position, Vector3 normal, Vector4 textureCoordinate, Vector4 textures1, Vector4 textures2)
        {
            this.position = position;
            this.normal = normal;
            this.textureCoordinate = textureCoordinate;
            this.textures1 = textures1;
            this.textures2 = textures2;
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public Vector4 TextureCoordinate
        {
            get { return textureCoordinate; }
            set { textureCoordinate = value; }
        }

        public Vector4 Textures1
        {
            get { return textures1; }
            set { textures1 = value; }
        }

        public Vector4 Textures2
        {
            get { return textures2; }
            set { textures2 = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }


    }

    public class Terrain : DrawableGameComponent
    {
        private GraphicsDevice graphicsDevice;
        private ContentManager content;

        private float timeTillUpdate;
        private Game1 game;

        private Level level;
        private LittleFlame player;

        //Terrain settings
        public int terrainWidth;
        public int terrainLength;
        private float[,] heightData;
        //The higher the smoother the terrain will be
        private float smooth;


        private Effect effect;

        private VertexBuffer terrainVertexBuffer;
        private IndexBuffer terrainIndexBuffer;

        private VertexMultitextured[] terrainVertices;
        private int[] terrainIndices;

        private Ground[,] ground;
        private string heightMapKey;
        private string textureMapKey;

        //Textures
        private Texture2D heightMap;
        private Texture2D textureMap;
        private Texture2D[] textures = new Texture2D[8];

        private Vector3 lightDirection;
        private Matrix worldMatrix;

        // Flame Size
        private float normalGrassSizeMod;
        private float dryGrassSizeMod;
        private float sandSizeMod;
        private float ashSizeMod;
        private float rockSizeMod;

        /*
         * GETTERS AND SETTERS
         */
        public VertexMultitextured[] TerrainVertices
        {
            get { return terrainVertices; }
            set { terrainVertices = value; }
        }

        public int TerrainWidth
        {
            get { return this.terrainWidth; }
        }

        public int TerrainLength
        {
            get { return this.terrainLength; }
        }

        /// <summary>
        /// Terrain Constructor
        /// </summary>
        /// <param name="game">This game</param>
        /// <param name="heightmapKey">The key of the heightmapKey</param>
        /// <param name="graphicsdevice"></param>
        /// <param name="content"></param>
        public Terrain(Game1 game, string heightmapKey, string texturemapKey, Level level)
            : base(game)
        {
            this.game = game;
            this.heightMapKey = heightmapKey;
            this.textureMapKey = texturemapKey;
            this.graphicsDevice = game.GraphicsDevice;;
            this.content = game.Content;
            this.level = level;
        }

        public override void Initialize()
        {
            player = (LittleFlame)Game.Services.GetService(typeof(LittleFlame));

            normalGrassSizeMod = -0.7f;
            dryGrassSizeMod = 0.4f;
            sandSizeMod = -1;
            ashSizeMod = -1;
            rockSizeMod = -1;

            smooth = 10f;

            worldMatrix = Matrix.Identity;
            lightDirection = new Vector3(1, 0.5f, 1);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Loading of the heightmapKeuy and texturemapKey
            this.heightMap = game.Assets.Load<Texture2D>(heightMapKey);
            this.textureMap = game.Assets.Load<Texture2D>(textureMapKey);
            //Loading of the terrain effect
            effect = game.Assets.Load<Effect>("terrain");

            //Loading textures
            LoadTextures();
            //Loading the texturemapKey and heightmapKeuy
            LoadMap();

            //Loading the vertices
            terrainIndices = new int[(terrainWidth - 1) * (terrainLength - 1) * 6];
            SetUpTerrainVertices();
            SetUpTerrainIndices();

            terrainVertices = CalculateNormals(terrainVertices, terrainIndices);
            CopyToTerrainBuffers(terrainVertices, terrainIndices);

            base.LoadContent();
        }

        private void CopyToTerrainBuffers(VertexMultitextured[] vertices, int[] indices)
        {
            terrainVertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexMultitextured), vertices.Length, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(vertices);

            terrainIndexBuffer = new IndexBuffer(graphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(indices);
        }

        private void LoadMap()
        {
            //Set the terrain width and height to the size of the texture
            terrainWidth = textureMap.Width;
            terrainLength = textureMap.Height;
            
            ground = new Ground[terrainWidth, terrainLength];
            
            //Texturemap initialize
            Color[] textureMapColors = new Color[terrainWidth * terrainLength];
            textureMap.GetData(textureMapColors);
            
            //Heightmap initialize
            Color[] heightMapColors = new Color[terrainWidth * terrainLength];
            heightMap.GetData(heightMapColors);
            heightData = new float[terrainWidth, terrainLength];

            //Loop through all the pixels
            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainLength; y++) {
                    //Get the red value of this pixel from texturemapKey
                    int red = textureMapColors[x + y * terrainWidth].R;
                    ground[x, y] = new Ground(red);
                    //Get the red value of this pixel from heighmap, smooth it by divide and set is to the heightdata
                    heightData[x, y] = (heightMapColors[x + y * terrainWidth].R / smooth);

                }
         
        }

        /*
         * LoadTextures
         * load every ground texture
         */
        private void LoadTextures()
        {
            //Textures1
            textures[0] = this.game.Assets.Load<Texture2D>("ash");             //W     Ash
            textures[1] = this.game.Assets.Load<Texture2D>("sand");            //X     Sand
            textures[2] = this.game.Assets.Load<Texture2D>("grass");           //Y     Grass
            textures[3] = this.game.Assets.Load<Texture2D>("drygrass");        //Z     DryGrass

            //Textures2
            textures[4] = this.game.Assets.Load<Texture2D>("rock");            //W     Rock
            textures[5] = this.game.Assets.Load<Texture2D>("mud");             //X     Mud
            textures[6] = this.game.Assets.Load<Texture2D>("flowers");         //Y     flowers
            textures[7] = this.game.Assets.Load<Texture2D>("drygrassflowers"); //Z     DryGrassFlowers
        }

        /*
         * SetUpTerrainVertices()
         * uses the structuur class VertexMultitextured that we make at the top of this file 
         */
        private void SetUpTerrainVertices()
        {
            terrainVertices = new VertexMultitextured[terrainWidth * terrainLength];
            int thisPos = 0;
            for (int x = 0; x < terrainWidth; x++) {
                for (int y = 0; y < terrainLength; y++) {
                    thisPos = x + y * terrainWidth;

                    terrainVertices[thisPos].position = new Vector3(x, heightData[x, y], y);
                    terrainVertices[thisPos].textureCoordinate.X = (float)x / 10f;
                    terrainVertices[thisPos].textureCoordinate.Y = (float)y / 10f;

                    //Texture the terrain

                    /*
                     * Summary
                     *  GRASS           =   textures1.Y
                     *  GRASSFLOWERS    =   textures2.Y
                     *  DRYGRASS        =   textures1.Z
                     *  DRYGRASSFLOWERS =   textures2.Z
                     *  
                     *  ASH             =   textures1.W
                     *  SAND            =   textures1.X
                     *  ROCK            =   textures2.W
                     *  MUD             =   textures2.X
                     */

                    //GRASS
                    if (ground[x, y].Grounds == (int)Ground.GroundTypes.GRASS){
                        terrainVertices[thisPos].textures1.Y = 1;
                    }else if (ground[x, y].Grounds == (int)Ground.GroundTypes.GRASSFLOWERS){
                        terrainVertices[thisPos].textures2.Y = 1;
                    //DRYGRASS   
                    }else if (ground[x, y].Grounds == (int)Ground.GroundTypes.DRYGRASS){
                        terrainVertices[thisPos].textures1.Z = 1;
                    }else if (ground[x, y].Grounds == (int)Ground.GroundTypes.DRYGRASSFLOWERS){
                        terrainVertices[thisPos].textures2.Z = 1;
                    //ASH
                    }else if (ground[x, y].Grounds == (int)Ground.GroundTypes.ASH)
                        terrainVertices[thisPos].textures1.W = 1;
                    //SAND
                    else if (ground[x, y].Grounds == (int)Ground.GroundTypes.SAND)
                        terrainVertices[thisPos].textures1.X = 1;
                    //Mud
                    else if (ground[x, y].Grounds == (int)Ground.GroundTypes.MUD)
                        terrainVertices[thisPos].textures2.X = 1;
                    //Rock
                    else if (ground[x, y].Grounds == (int)Ground.GroundTypes.ROCK)
                        terrainVertices[thisPos].textures2.W = 1;
                    
                }
            }
        }

        private VertexMultitextured[] CalculateNormals(VertexMultitextured[] vertices, int[] indices)
        {
            //Set al the vertices on Normal 0 just to clear
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            //calculate the normal of each triangle
            for (int i = 0; i < indices.Length / 3; i++) {
                //The indices for the vertex on this i
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];
                
                //Calculates the sides of this triangle
                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            //Normalize the normals, so they don't have a huge vector
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }

        /*
         * SetUpIndices
         */
        private void SetUpTerrainIndices()
        {
            int counter = 0;
            //Loop through world
            for (int y = 0; y < terrainLength - 1; y++) {
                for (int x = 0; x < terrainWidth - 1; x++) {
                    //Positions for 2 triangles
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    //Set the indices to the positions
                    terrainIndices[counter++] = topLeft;
                    terrainIndices[counter++] = lowerRight;
                    terrainIndices[counter++] = lowerLeft;

                    terrainIndices[counter++] = topLeft;
                    terrainIndices[counter++] = topRight;
                    terrainIndices[counter++] = lowerRight;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {

            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            //Set the current technique for the graphicscard
            effect.CurrentTechnique = effect.Techniques["Multitextured"];
            //Draw every texture to the effect
            for (int i = 0; i < textures.Length; i++){
                effect.Parameters["xTexture" + i].SetValue(textures[i]);
            }

            effect.Parameters["xView"].SetValue(level.Cam.viewMatrix);
            effect.Parameters["xProjection"].SetValue(level.Cam.projectionMatrix);
            effect.Parameters["xWorld"].SetValue(worldMatrix);

            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(0.4f);
            effect.Parameters["xEnableLighting"].SetValue(true);

            //Just one pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                //Begin of this pass
                pass.Apply();

                graphicsDevice.SetVertexBuffer(terrainVertexBuffer);
                graphicsDevice.Indices = terrainIndexBuffer;

                //Number of vertices and triangles
                int vertices = terrainVertexBuffer.VertexCount;
                int triangles = terrainIndexBuffer.IndexCount / 3;

                //Draw all the vertices en triangles
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices, 0, triangles);
            }

            base.Draw(gameTime);
        }


        /*
         * GetHeightAtPosition
         * Gets the height of a specific X and Z position. if player is of the map it will return back
         * @param xPos 
         * @param zPos
         * @param yOffset
         */
        public Vector3 GetHeightAtPosition(float xPos, float zPos, float yOffset)
        {
            try {
                Vector3 pos = new Vector3(xPos, heightData[(int)xPos, (int)zPos] + yOffset, zPos);
                return pos;
            } catch (IndexOutOfRangeException) {
                Console.WriteLine("Go back on the map");
                Vector3 alternativeVect = (GetHeightAtPosition(60, 60, 0));
                return alternativeVect;
            }
        }

        public void BurningField(Vector3 flamePos, GameTime gametime, Game game, Level level)
        {
            ground[(int)flamePos.X, (int)flamePos.Z].BurnValue -= 2;
            player = (LittleFlame)Game.Services.GetService(typeof(LittleFlame));

            int burnSize = (int)player.FlameSize / 75;
            switch (ground[(int)flamePos.X, (int)flamePos.Z].Grounds) {
                case (int)Ground.GroundTypes.GRASS:
                    UpdateGrass(flamePos, burnSize, false, false, normalGrassSizeMod, 1, 0.5);
                    break;
                case (int)Ground.GroundTypes.GRASSFLOWERS:
                    UpdateGrass(flamePos, burnSize, false, true, normalGrassSizeMod, 1, 0.5);
                    break;
                case (int)Ground.GroundTypes.DRYGRASS:
                    UpdateGrass(flamePos, burnSize, true, false, dryGrassSizeMod, 0.5, 0);
                    break;
                case (int)Ground.GroundTypes.DRYGRASSFLOWERS:
                    UpdateGrass(flamePos, burnSize, true, true, dryGrassSizeMod, 0.5, 0);
                    break;
                case (int)Ground.GroundTypes.SAND:
                    NonWalkableTerrain(sandSizeMod);
                    break;
                case (int)Ground.GroundTypes.MUD:
                    NonWalkableTerrain(sandSizeMod);
                    break;
                case (int)Ground.GroundTypes.ROCK:
                    NonWalkableTerrain(rockSizeMod);
                    break;
                case (int)Ground.GroundTypes.ASH:
                    NonWalkableTerrain(ashSizeMod);
                    break;
                    
                default: break;
            }

        }

        private void NonWalkableTerrain(float sizemod)
        {
            player.Velocity = Vector3.Zero;
            player.Position = player.LastPosition;
            player.ChangeSize(sizemod);
        }

        private void UpdateGrass(Vector3 flamePos, int burnSize, bool dryGrass, bool flowers, float sizeMod, double startUpdatingAt, double startPlayerSize)
        {
            if (ground[(int)flamePos.X, (int)flamePos.Z].Burned <= startUpdatingAt){
                UpdateGrassVertices(flamePos, burnSize, flowers, dryGrass);
                player.ChangeSize(sizeMod);
            }else if (ground[(int)flamePos.X, (int)flamePos.Z].Burned <= startPlayerSize){
                updateStates(flamePos, 3);
                player.ChangeSize(sizeMod);
            }
        }
        

        // This method is used when the player's state is a spark. Sand interferes with the spark and shrinks it.
        public void OverSand(Game game, float waterLevel)
        {
            LittleFlame player = (LittleFlame)game.Services.GetService(typeof(LittleFlame));
            // This should not happen when the player is over water
            if (player != null && ground[(int)player.Position.X, (int)player.Position.Z].Grounds == (int)Ground.GroundTypes.SAND
                && GetHeightAtPosition(player.Position.X, player.Position.Z, 0).Y > waterLevel) {
                // Shrink the flame
                player.overSand();
            }
        }

        private void UpdateGrassVertices(Vector3 pos, int size, bool flowers, bool dryGrass)
        {
            //for the size
            for (int i = -size + 1; i < size; i++) {
                for (int j = -size + 1; j < size; j++) {
                    if(dryGrass){
                        if (!flowers){
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures1.Z = 0;
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures1.W = 1;
                        }else{
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures2.Z = 0;
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures2.W = 1;
                        }
                    }else if(!dryGrass){
                        if (!flowers){
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures1.Y = 0;
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures1.W = 1;
                        }else{
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures2.Y = 0;
                            terrainVertices[((int)pos.X + i) + ((int)pos.Z + j) * terrainWidth].textures2.W = 1;
                        }
                    }
                }
            }
        }

        private void updateStates(Vector3 pos, int toGroundState)
        {
            ground[(int)pos.X + 1, (int)pos.Z].Grounds = toGroundState;
            ground[(int)pos.X - 1, (int)pos.Z].Grounds = toGroundState;
            ground[(int)pos.X, (int)pos.Z - 1].Grounds = toGroundState;
            ground[(int)pos.X, (int)pos.Z + 1].Grounds = toGroundState;
            ground[(int)pos.X + 1, (int)pos.Z + 1].Grounds = toGroundState;
            ground[(int)pos.X - 1, (int)pos.Z - 1].Grounds = toGroundState;
            ground[(int)pos.X - 1, (int)pos.Z + 1].Grounds = toGroundState;
            ground[(int)pos.X + 1, (int)pos.Z - 1].Grounds = toGroundState;
            ground[(int)pos.X, (int)pos.Z].Grounds = toGroundState;
        }

        public override void Update(GameTime gameTime)
        {
            timeTillUpdate -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeTillUpdate <= 0) {
                this.terrainVertexBuffer.SetData(terrainVertices);
                timeTillUpdate = 1f;
            }

            base.Update(gameTime);
        }

        public int EndBurnedVertices()
        {
            int amount = 0;

            for (int z = 0; z < terrainLength - 1; z++){
                for (int x = 0; x < terrainWidth - 1; x++){
                    if(terrainVertices[((int)x) + ((int)z) * terrainWidth].textures1.W == 1){
                        amount++;
                    }
                }
            }

            return amount;
        }
    }
}
