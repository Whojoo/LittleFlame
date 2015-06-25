using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LittleFlame.Camera
{
    public class LFCamera : Camera
    {
        private const byte CameraRotateSpeedMultiplier = 100;

        private Vector2 rightThumbStickDeadzone;
        private float camPosY;
        private float camPosZ;
        private float camPosX;

        private int distanceFromFlame;
        private int maxCameraPitch;
        private int minCameraPitch;

        public LFCamera(Vector3 _camPos, float _camSpeed, GraphicsDevice _graphicsDevice)
            : base(_camPos, _camSpeed, _graphicsDevice)
        {
            distanceFromFlame = -30;
            maxCameraPitch = -10;
            minCameraPitch = -35;

            //Initialise the custom deadzones. 
            rightThumbStickDeadzone = new Vector2(0.1f, 0.1f);
        }

        public override void Update(GameTime gameTime, Vector3 flamePos)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            //Check if an xbox controler is connected. If it isn't, the mouse is activated.
            if (gamePadState.IsConnected) {
                
                //Reset the vector to zero
                Vector3 moveVector = Vector3.Zero;
                float moveFactor = camSpeed * timeDelta;

                float deltaX = 0;
                float deltaY = 0;

                //Check if the left thumbstick is used.
                if ((float)Math.Sqrt(Math.Pow(gamePadState.ThumbSticks.Right.Y, 2)) > this.rightThumbStickDeadzone.Y) {
                    deltaY -= (float)gamePadState.ThumbSticks.Right.Y * CameraRotateSpeedMultiplier;
                }
                if ((float)Math.Sqrt(Math.Pow(gamePadState.ThumbSticks.Right.X, 2)) > this.rightThumbStickDeadzone.X) {
                    deltaX += (float)gamePadState.ThumbSticks.Right.X * CameraRotateSpeedMultiplier;
                }

                //Calculate Yaw and Pitch rotations
                cameraYaw -= (deltaX * moveFactor) * timeDelta;
                cameraPitch -= (deltaY * moveFactor) * timeDelta;

                //Clamp the pitch to a range of -90 to 90 degrees
                cameraPitch = MathHelper.Clamp(cameraPitch, -35, -10);
            } else {
                //set keyboard and Mouse states
                MouseState currentMouseState = Mouse.GetState();

                //Reset the vector to zero
                Vector3 moveVector = Vector3.Zero;
                float moveFactor = camSpeed * timeDelta;

                //get the mouse position
                float mouseX = currentMouseState.X - originalMouseState.X;
                float mouseY = currentMouseState.Y - originalMouseState.Y;

                //Calculate Yaw and Pitch rotations 
                cameraYaw -= (mouseX * moveFactor) * timeDelta;
                if (cameraYaw > 360)
                    cameraYaw = 0;
                else if (cameraYaw < 0)
                    cameraYaw = 360;

                cameraPitch -= (mouseY * moveFactor) * timeDelta;

                //Clamp the pitch to a range of -90 to 90 degrees
                cameraPitch = MathHelper.Clamp(cameraPitch, minCameraPitch, maxCameraPitch);

                Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);

                
            }

            //Calculate position from the flame
            camPosX = distanceFromFlame * (float)Math.Sin(MathHelper.ToRadians(cameraYaw));
            camPosZ = distanceFromFlame * (float)Math.Cos(MathHelper.ToRadians(cameraYaw));
            camPosY = distanceFromFlame * (float)Math.Sin(MathHelper.ToRadians(cameraPitch));

            Vector3 cameraPositionVector = new Vector3(camPosX, camPosY, camPosZ);

            camPos = flamePos + cameraPositionVector;

            camTarget = flamePos;

            //Creating the look
            viewMatrix = Matrix.CreateLookAt(camPos, camTarget, Vector3.Up);
        }


    }
}