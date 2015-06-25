using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;


namespace LittleFlame.Models
{
    public class AddModels
    {
        private Game1 game;
        
        private Texture2D modelMap;

        private Terrain terrain;

        private List<Models.LFModel> models;

        private int terrainWidth,
                    terrainHeight,
                    red;
        private float green,
                        size;

        private Color[] modelMapColors;

        private Model treeModel,
                        coalModel,
                        meteorModel,
                        stoneModel,
                        starmodel;

        //Fields for size control for each model.
        private float   treeStartSize = 50,
                        treeSizeModifier = 0.0005f,
                        coalStartSize = 50,
                        coalSizeModifier = 0.0005f;



        public AddModels(Game1 game, List<Models.LFModel> models, Terrain terrain, string modelMapKey, ContentManager content)
        {
            this.game = game;
            this.models = models;
            this.terrain = terrain;

            modelMap = game.Assets.Load<Texture2D>(modelMapKey);
            
            terrainWidth = modelMap.Width;
            terrainHeight = modelMap.Height;

            treeModel = game.Assets.Load<Model>("tree");
            coalModel = game.Assets.Load<Model>("coal");
            meteorModel = game.Assets.Load<Model>("meteor");
            starmodel = game.Assets.Load<Model>("star");
            stoneModel = game.Assets.Load<Model>("stone");
            

            modelMapColors = new Color[terrainWidth * terrainHeight];
            modelMap.GetData(modelMapColors);


            LoopThroughMap();

            }        
        
        private void LoopThroughMap()
        {
            //loop through model map
            for (int y = 0; y < terrainHeight; y++){
                for (int x = 0; x < terrainWidth; x++){
                    red = modelMapColors[x + y * terrainHeight].R;             
                    switch(red){
                        case 250:
                            green = modelMapColors[x + y * terrainHeight].G;
                            size = (treeStartSize + green) * treeSizeModifier;
                            models.Add(new Models.Tree(game, treeModel, terrain.GetHeightAtPosition((float)x, (float)y, -0.07f), Vector3.Zero, new Vector3(size)));
                            break;
                        case 200:
                            green = modelMapColors[x + y * terrainHeight].G;
                            size = green/1500;
                            models.Add(new Models.Meteoriet(game, meteorModel, terrain.GetHeightAtPosition((float)x, (float)y, 0.0f), Vector3.Zero, new Vector3(size)));
                            break;
                        case 150:
                            green = modelMapColors[x + y * terrainHeight].G;
                            size = (coalStartSize + green) * coalSizeModifier;
                            models.Add(new Models.Coal(game, coalModel, terrain.GetHeightAtPosition((float)x, (float)y, -0.07f), Vector3.Zero, new Vector3(size)));
                            break;
                        case 100:
                            green = modelMapColors[x + y * terrainHeight].G;
                            size = green / 1000;
                            models.Add(new Models.Stone(game, stoneModel, terrain.GetHeightAtPosition((float)x, (float)y, -0.07f), Vector3.Zero, new Vector3(size)));
                            break;
                        case 50:
                            green = modelMapColors[x + y * terrainHeight].G;
                            size = green / 30000;
                            models.Add(new Models.Star(game, starmodel, terrain.GetHeightAtPosition((float)x, (float)y, 3f), Vector3.Zero, new Vector3(size)));
                            break;
                        default:
                        break;
                    }
                }   
            }   
        }
    }
}
