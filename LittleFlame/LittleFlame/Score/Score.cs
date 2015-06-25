using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame.Score
{
    public class Score
    {
        private Terrain terrain;
        private Texture2D scoreTexture;
        private int startPecentage, width, height, 
                    pixelsBlack, pixelsGrass, pixelsStartBurnable, pixelsBurned;
        private float endPercentage;

        public Score(Texture2D scoreTexture, Terrain terrain)
        {
            this.scoreTexture = scoreTexture;
            this.terrain = terrain;
            startPecentage = CalculateStartPerc();
        }

        private int CalculateStartPerc(){
            int percentage = 0;

            //Set the terrain width and height to the size of the texture
            width = scoreTexture.Width;
            height = scoreTexture.Height;

            //Color array
            Color[] textureMapColors = new Color[width * height];
            scoreTexture.GetData(textureMapColors);

            for (int y = 0; y < height - 1; y++){
                for (int x = 0; x < width - 1; x++){
                    int red = textureMapColors[x + y * width].R;
                    //If grass
                    if (red == 74 || red == 100 || red == 80 || red == 164){
                        pixelsGrass++;
                    }else{
                        pixelsBlack++;
                    }
                }
            }

            pixelsStartBurnable = pixelsGrass;

            return percentage;
        }

        public float CalculateEndPerc()
        {
            pixelsBurned = terrain.EndBurnedVertices();
            endPercentage = (float)pixelsBurned / (float)pixelsStartBurnable;
            endPercentage *= 100;
            return endPercentage;
        }

        public int CalculateScore()
        {
            return (100 - (int)endPercentage) * 10;
        }
    }
}
