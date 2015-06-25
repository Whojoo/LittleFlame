using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace LittleFlame
{
    public class Sky : DrawableGameComponent
    {
        private Models.LFModel sphereModel;
        private Effect skyEffect;

        private Game1 game;
        private Camera.Camera camera;

        public Sky(Game1 game, TextureCube skyTexture)
            : base(game)
        {
            this.game = game;

            //Get the camera (only littleflame camera)
            camera = (Camera.Camera)game.Services.GetService(typeof(Camera.Camera));

            //Create model
            sphereModel = new Models.LFModel(game, game.Assets.Load<Model>("skysphere"), camera.CamPos, Vector3.Zero, new Vector3(12f));
            game.Components.Add(sphereModel);

            //Load the effect
            skyEffect = game.Assets.Load<Effect>("sky");

            //Set the cubemap
            skyEffect.Parameters["CubeMap"].SetValue(skyTexture);
            sphereModel.SetModelEffect(skyEffect, false);
        }

        public override void Draw(GameTime gameTime){         
            //skyEffect.Parameters["redColor"].SetValue(red++);
            //Disable the depth buffer
            //game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            //Move the model with the camera, so it looks like the world never ends
            sphereModel.Position = camera.CamPos;
            //Reset the depth buffer
            //game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }
        
    }

}
