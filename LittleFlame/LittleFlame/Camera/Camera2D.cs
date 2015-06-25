using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace LittleFlame.Camera
{
    public class Camera2D
    {
        private const float ZOOMUPPERLIMIT = 1.5f;
        private const float ZOOMLOWERLIMIT = 0.5f;

        private float zoom;
        private Matrix transfrom;
        private Vector2 position;
        private float rotation;
        private int viewportWidth;
        private int viewportHeight;
        private int worldWidth;
        private int worldHeight;

        public Camera2D(GraphicsDevice graphics, int worldWidth, int worldHeight, float initialZoom)
        {
            this.zoom = initialZoom;
            this.rotation = 0.0f;
            this.viewportWidth = graphics.Viewport.Width;
            this.viewportHeight = graphics.Viewport.Height;
            this.worldHeight = worldHeight;
            this.worldWidth = worldWidth;
            this.position = new Vector2(this.viewportWidth / 2, this.viewportHeight / 2);
        }

        #region Properties

        public float Zoom
        {
            get { return this.zoom; }
            set
            {
                this.zoom = value;
                if (this.zoom < ZOOMLOWERLIMIT) {
                    this.zoom = ZOOMLOWERLIMIT;
                } else if (this.zoom > ZOOMUPPERLIMIT) {
                    this.zoom = ZOOMUPPERLIMIT;
                }
            }
        }

        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        public void Move(Vector2 amount)
        {
            this.Position += amount;
        }

        public Vector2 Position
        {
            get { return this.position; }
            set
            {
                float leftBarrier = (float)this.viewportWidth * 0.5f / this.zoom;
                float rightBarrier = (float)this.worldWidth - (float)this.viewportWidth * 0.5f / this.zoom;
                float topBarrier = (float)this.worldHeight - (float)this.viewportHeight * 0.5f / this.zoom;
                float bottomBarrier = (float)this.viewportHeight * 0.5f / this.zoom;
                this.position = value;

                if (this.position.X < leftBarrier) {
                    this.position.X = leftBarrier;
                } else if (this.position.X > rightBarrier) {
                    this.position.X = rightBarrier;
                } else if (this.position.Y > topBarrier) {
                    this.position.Y = topBarrier;
                } else if (this.position.Y < bottomBarrier) {
                    this.position.Y = bottomBarrier;
                }
            }
        }

        public float LeftBoundry()
        {
            return this.position.X - this.viewportWidth / 2;
        }

        public float RightBoundry()
        {
            return this.position.X + this.viewportWidth / 2;
        }

        public float UpperBoundry()
        {
            return this.position.Y - this.viewportHeight / 2;
        }

        public float LowerBoundry()
        {
            return this.position.Y + this.viewportHeight / 2 - 150;
        }

        #endregion

        public Matrix GetTransformation()
        {
            this.transfrom =
                Matrix.CreateTranslation(new Vector3(-this.position.X, -this.position.Y, 0)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateScale(new Vector3(this.zoom, this.zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(this.viewportWidth * 0.5f, this.viewportHeight * 0.5f, 0));
            return this.transfrom;
        }

        public void EaseTo(Vector2 target, float easing)
        {
            Vector2 easeVec = Vector2.Subtract(target, this.position);
            if (easeVec.LengthSquared() > 1) {
                this.position += easeVec * easing;
            }
        }
    }
}
