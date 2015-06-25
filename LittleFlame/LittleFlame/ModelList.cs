using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LittleFlame
{
    class ModelList
    {
        public Vector3 Rotation{
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 Position{
            get { return position; }
            set { position = value; }
        }

        public Vector3 TopTree{
            get { return topTree; }
            set { topTree = value; }
        }
        private Vector3 topTree;
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale;

        private Model model;
        private Matrix[] modelTransforms;
        private float rangeDistance;
        GraphicsDevice graphicsDevice;

        public ModelList(Model model, Vector3 position, Vector3 rotation, Vector3 scale, float rangeDistance, float maxTreeClimb,GraphicsDevice graphicsDevice)
        {
            this.model = model;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.rangeDistance = rangeDistance;
            this.graphicsDevice = graphicsDevice;
            
            topTree = new Vector3(position.X, position.Y += maxTreeClimb, position.Z);


            //Number of array is number of bones in the model
            modelTransforms = new Matrix[model.Bones.Count];
            //In an absolute transform, each bone is transformed according to the position of all parent bones.
            //The resulting array contains the transforms that describe how each ModelMesh is located relative to one another in the Model. 
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

        }

        public void Draw(Matrix _view, Matrix _projection)
        {
            //Calculate the base transformation by combining translation, rotation and scaling
            Matrix baseWorld = Matrix.CreateScale(scale) *
                                Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) *
                                Matrix.CreateTranslation(position);
            
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                
                Matrix localWorld = modelTransforms[mesh.ParentBone.Index] * baseWorld;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    BasicEffect effect = (BasicEffect)meshPart.Effect;
                    effect.World = localWorld;
                    effect.View = _view;
                    effect.Projection = _projection;

                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

        public bool inRangeDistance(Vector3  _flamePos)
        {
            //Pythagoras to check te range between model and flame
            double range = Math.Pow((Math.Pow(_flamePos.X - position.X, 2.0) + Math.Pow(_flamePos.Z - position.Z, 2.0)), 0.5);
            
            //If the range is smaller than the range distance return true
            if (range <= rangeDistance)
                return true;
            else
                return false;

        }

        public double TreeFall(Vector3 camPos)
        {
            float camPosX = camPos.X - position.X;
            float camPosZ = camPos.Z - position.Z;
            
            double degrees = MathHelper.ToDegrees((float)Math.Atan2(camPosZ, -camPosX));
            //Console.WriteLine(camPosX + "  " + camPosZ + "  " + degrees);
            return degrees;
        }
    }
}
