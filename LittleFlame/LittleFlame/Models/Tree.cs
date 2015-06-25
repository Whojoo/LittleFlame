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
    class Tree : LFModel
    {
        //Make sure that 90 % XRotation == 0.
        private const float XRotation = 5;

        private Vector3 topTree;
        private float treeHeight;
        private float scaleY;
        private bool isFalling;
        private bool isBurned;
        private float fallspeed;
        private Vector2 fallDirection;
        LittleFlame player;
        Terrain terrain;
        
        public Tree(Game game, Model model, Vector3 position, Vector3 rotation, Vector3 scale)
            : base(game, model, position, rotation, scale)
        {
            this.scaleY = scale.Y;
            this.treeHeight = 360 * scale.Y;
        }

        public override void Initialize()
        {
            rangeDistance = 1;
            player = (LittleFlame)Game.Services.GetService(typeof(LittleFlame));
            terrain = (Terrain)Game.Services.GetService(typeof(Terrain));
            isBurned = false;

            //The fallspeed starts at a sixteenth of a circle each second (a half circle equals PI and divide that by eigth multiplied by 60 fps).
            this.fallspeed = MathHelper.Pi / 480;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (isFalling) {
                Fall((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            base.Update(gameTime);
        }

        protected override Matrix GetTransformation()
        {
            /*
             * This rotates the tree by the following:
             * In this case the tree rotates around the X axis. So the tree would fall in the Z axis' direction. To be able to let the tree fall in any
             * direction, we need to rotate around the Y axis.
             * Example: to be able to let the tree fall in the X direction you do the following.
             * Rotate the falling direction perpendicular to the X axis around the Y axis by subtracting from rotation to the starting rotation.
             * Rotate around the X axis.
             * Rotate back around the Y axis to the starting rotation.
             */
            Matrix transform = Matrix.CreateRotationY(this.rotation.Y) *
               Matrix.CreateRotationX(this.rotation.X) *
               Matrix.CreateRotationY(-this.rotation.Y) *
               Matrix.CreateScale(this.scale) *
               Matrix.CreateTranslation(this.position);
            return transform;
        }

        /// <summary>
        /// Initiates the falling of the tree.
        /// </summary>
        /// <param name="direction">The direction the tree is supposed to fall.</param>
        public void StartFall(Vector3 direction)
        {
            this.fallDirection = new Vector2(direction.X, direction.Z);
            //Check if the falldirection can be normalized.
            if (this.fallDirection != Vector2.Zero) {
                fallDirection.Normalize();
            } else {
                this.fallDirection = new Vector2(0, 1);
            }
            this.rotation.Y = (float)Math.Atan2(this.fallDirection.Y, this.fallDirection.X) + (float)Math.PI / 2;
            isFalling = true;
        }

        private void Fall(float _deltaSeconds)
        {
            //Take the total time which will be used to keep accelerating the falling.
            //this.totalFallTime += _deltaSeconds;
            this.rotation.X -= MathHelper.Pi / 8 * this.fallspeed;//MathHelper.ToRadians(XRotation * totalFallTime);
            this.fallspeed += .1f * _deltaSeconds;
            //this.rotation.X = this.rotation.X * this.rotation.X;
            //Calculate the distance the tree has fallen.
            float distFallen = (float)Math.Sin(rotation.X) * this.treeHeight;
            //Use the rotationAngle around the Y axis and the current distFromTree to calculate the X and Z values.
            float topTreeX = (float)Math.Sin(-this.rotation.Y) * distFallen + this.position.X;
            float topTreeZ = (float)Math.Cos(-this.rotation.Y) * distFallen + this.position.Z;
            //Calculate the top of the tree.
            Vector3 terrainPos = this.terrain.GetHeightAtPosition(topTreeX, topTreeZ, 0);
            //float topTreeY = (float)Math.Sqrt(Math.Pow(this.treeHeight, 2) - Math.Pow(distFallen, 2)) + terrainPos.Y;
            float topTreeY = (float)Math.Cos(this.rotation.X) * this.treeHeight;

            //Use the terrain to determine if the tree top has hit the ground. Stop the falling if it has.
            if (topTreeY <= terrainPos.Y) {
                topTree = terrainPos;
                StopFall(this.player);
            } else {
                topTree = new Vector3(topTreeX, topTreeY, topTreeZ);
                this.player.Position = this.topTree;
            }
        }

        private void StopFall(LittleFlame player)
        {
            isFalling = false;
            isBurned = true;

            player.Position = topTree;
            player.IsInTree = false;
            player.TreeClimb = 0;
        }

        public float TreeHeight
        {
            get { return treeHeight; }
            set { treeHeight = value; }
        }

        public bool IsFalling
        {
            get { return isFalling; }
            set { isFalling = value; }
        }

        public bool IsBurned
        {
            get { return isBurned; }
            set { isBurned = value; }
        }

        public float ScaleY
        {
            get { return scaleY; }
        }

        public Vector3 TopTree
        {
            get { return topTree; }
        }
    }
}
