using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace LittleFlame.Camera
{
    public abstract class Camera
    {
        protected Vector3 camPos;
        public Vector3 CamPos {
            get { return camPos; }
        }

        protected Vector3 camUp;
        public Vector3 CamUp {
            get { return camUp; }
        }

        protected Vector3 camTarget;
        public Vector3 CamTarget {
            get { return camTarget; }
        }

        protected Vector3 camRight;
        public Vector3 CamRight
        {
            get { return camRight; }
        }

        protected Vector3 camForward;

        protected float camSpeed;
        protected GraphicsDevice graphicsDevice;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        //Mouse
        protected float cameraYaw;    //rotation about the Y axis
        public float CameraYaw
        {
            get { return cameraYaw; }
        }

        protected float cameraPitch;  //rotation about the X axis
        protected MouseState originalMouseState;

        public Camera() { }

        public Camera(Vector3 camPos, float camSpeed, GraphicsDevice graphicsDevice)
        {
            //Camera view
            this.camPos = camPos;
            this.camUp = Vector3.Up;
            this.camForward = Vector3.Forward;
            this.camTarget = this.camForward + this.camPos;
            this.camSpeed = camSpeed;
            this.graphicsDevice = graphicsDevice;
            this.camRight = Vector3.Cross(camForward, camUp);
            //Mouse
            this.cameraYaw = 180.0f;
            this.cameraPitch = 0.0f;

            //Set mouse to the middle
            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

            viewMatrix = Matrix.CreateLookAt(this.camPos, new Vector3(0, 0, 0), this.camUp);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.graphicsDevice.Viewport.AspectRatio, 1.0f, 400.0f);
        }

        public abstract void Update(GameTime gameTime, Vector3 flamePos);


    }
}
