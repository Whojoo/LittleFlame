using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace LittleFlame.Models
{
    public class LFModel : DrawableGameComponent
    {
        protected Vector3 position;
        protected Vector3 rotation;
        protected Vector3 scale;

        private Camera.Camera cam;
        private SpriteBatch spriteBatch;

        protected Model model;
        private Matrix[] modelTransforms;
        protected float rangeDistance;
        private GraphicsDevice graphicsDevice;

        public LFModel(Game game, Model model, Vector3 position, Vector3 rotation, Vector3 scale)
            : base(game)
        {
            this.model = model;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            generateTags();
            //Number of array is number of bones in the model
            modelTransforms = new Matrix[model.Bones.Count];
            //In an absolute transform, each bone is transformed according to the position of all parent bones.
            //The resulting array contains the transforms that describe how each ModelMesh is located relative to one another in the Model. 
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);


        }

        public override void Initialize()
        {
            graphicsDevice = Game.GraphicsDevice;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            cam = Game.Services.GetService(typeof(Camera.Camera)) as Camera.Camera;

            base.Update(gameTime);
        }

        override public void Draw(GameTime gameTime)
        {
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix localWorld = modelTransforms[mesh.ParentBone.Index] * this.GetTransformation();
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    Effect effect = meshPart.Effect;
                    if (effect is BasicEffect)
                    {
                        ((BasicEffect)effect).World = localWorld;
                        ((BasicEffect)effect).View = cam.viewMatrix;
                        ((BasicEffect)effect).Projection = cam.projectionMatrix;
                        ((BasicEffect)effect).EnableDefaultLighting();
                    }
                    else
                    {
                        setEffectParameter(effect, "World", localWorld);
                        setEffectParameter(effect, "View", cam.viewMatrix);
                        setEffectParameter(effect, "Projection", cam.projectionMatrix);
                        setEffectParameter(effect, "CameraPosition", cam.CamPos);
                    }


                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public bool inRangeDistance(Vector3 _flamePos, float _flameSize)
        {
            //Pythagoras to check te range between model and flame
            double range = Math.Pow((Math.Pow(_flamePos.X - position.X, 2.0) + Math.Pow(_flamePos.Z - position.Z, 2.0)), 0.5);
            float currentRangeDistance = rangeDistance + _flameSize / 90;

            //If the range is smaller than the range distance return true
            if (range <= currentRangeDistance)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Gets the transformation used for drawing the model.
        /// Subclasses can override this if neccesary.
        /// </summary>
        /// <returns>A transform matrix.</returns>
        protected virtual Matrix GetTransformation()
        {
            Matrix transform = Matrix.CreateRotationX(this.rotation.X) *
                Matrix.CreateRotationY(this.rotation.Y) *
                Matrix.CreateRotationZ(this.rotation.Z) *
                Matrix.CreateScale(this.scale) *
                Matrix.CreateTranslation(this.position);
            return transform;
        }

        private void generateTags()
        {
            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    if (part.Effect is BasicEffect)
                    {
                        BasicEffect effect = (BasicEffect)part.Effect;
                        MeshTag tag = new MeshTag(effect.DiffuseColor, effect.Texture,
                        effect.SpecularPower);
                        part.Tag = tag;
                    }
        }


        public void SetModelEffect(Effect effect, bool CopyEffect)
        {
            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Effect toSet = effect;
                    // Copy the effect if necessary
                    if (CopyEffect)
                        toSet = effect.Clone();

                    MeshTag tag = ((MeshTag)part.Tag);

                    // If this ModelMeshPart has a texture, set it to the effect
                    if (tag.Texture != null)
                    {
                        setEffectParameter(toSet, "BasicTexture", tag.Texture);
                        setEffectParameter(toSet, "TextureEnabled", true);
                    }
                    else
                        setEffectParameter(toSet, "TextureEnabled", false);

                    // Set our remaining parameters to the effect
                    setEffectParameter(toSet, "DiffuseColor", tag.Color);
                    setEffectParameter(toSet, "SpecularPower", tag.SpecularPower);

                    part.Effect = toSet;

                }
        }

        private void setEffectParameter(Effect effect, string paramName, object val)
        {
            //If the paramname of the effect null is, skip the function
            if (effect.Parameters[paramName] == null)
                return;

            if (val is Vector3)
                effect.Parameters[paramName].SetValue((Vector3)val);
            else if (val is bool)
                effect.Parameters[paramName].SetValue((bool)val);
            else if (val is Matrix)
                effect.Parameters[paramName].SetValue((Matrix)val);
            //else if (val is Texture2D)
            //    effect.Parameters[paramName].SetValue((Texture2D)val);
        }


        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
