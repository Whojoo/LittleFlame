using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LittleFlame.Camera
{
    public class DevelopCamera : Camera
    {
        public DevelopCamera(Vector3 _camPos, float _camSpeed, GraphicsDevice _graphicsDevice)
            : base(_camPos, _camSpeed, _graphicsDevice)
        {
        }

        public override void Update(GameTime gameTime, Vector3 flamePos)
        {
            //set keyboard and Mouse states
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Reset the vector to zero
            Vector3 moveVector = Vector3.Zero;
            float moveFactor = camSpeed * timeDelta;
            
            //Process keyboard input
            if (keyboardState.IsKeyDown(Keys.W)) { moveVector.Z -= moveFactor; }
            if (keyboardState.IsKeyDown(Keys.S)) { moveVector.Z += moveFactor; }
            if (keyboardState.IsKeyDown(Keys.A)) { moveVector.X -= moveFactor; }
            if (keyboardState.IsKeyDown(Keys.D)) { moveVector.X += moveFactor; }
            if (keyboardState.IsKeyDown(Keys.Q)) { moveVector.Y -= moveFactor; }
            if (keyboardState.IsKeyDown(Keys.X)) { moveVector.Y += moveFactor; }

            //get the mouse position
            float mouseX = currentMouseState.X - originalMouseState.X;
            float mouseY = currentMouseState.Y - originalMouseState.Y;

            //Calculate Yaw and Pitch rotations
            cameraYaw -= (mouseX * 0.05f) * timeDelta;
            cameraPitch -= (mouseY * 0.05f) * timeDelta;

            //Clamp the pitch to a range of -90 to 90 degrees
            cameraPitch = MathHelper.Clamp(cameraPitch, MathHelper.ToRadians(-90), MathHelper.ToRadians(90));

            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);

            Matrix cameraRotationMatrix = Matrix.CreateRotationX(cameraPitch) * Matrix.CreateRotationY(cameraYaw);

            Vector3 transformedCameraReference = Vector3.Transform(camForward, cameraRotationMatrix);

            camPos += Vector3.Transform(moveVector, cameraRotationMatrix);

            camTarget = transformedCameraReference + camPos;

            Vector3 cameraRotatedUpVector = Vector3.Transform(camUp, cameraRotationMatrix);

            //Set the campos to the moveVector and the Rotation Vector
            //camPos += moveVector;

            viewMatrix = Matrix.CreateLookAt(camPos, camTarget, cameraRotatedUpVector);
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio, 1.0f, 300.0f);
        }
    }
}
