using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3Dproject
{
    public class Camera
    {
        private int CameraId = 0;
        private float[] CameraSpeed = { 0.5f, 0.4f};
        float yaw = 0f, pitch = 0f,aspectRatio, scale = 1f;
        float HeightOffset = 8f;
        Vector3 cameraTarguet, pos;

        Keys[] cameraKeys = { Keys.F1, Keys.F2 };        

        public Matrix viewMatrix, projectionMatrix;        

        public Camera(GraphicsDevice device)
        {
            pos = new Vector3(10, 0, 10);
            cameraTarguet = new Vector3(10, 0, 0);
            viewMatrix = Matrix.CreateLookAt(pos,pos+cameraTarguet, Vector3.Up);

            aspectRatio = (float)(device.Viewport.Width /
            device.Viewport.Height);                     
            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.3f, 1000.0f);
        }

        public void Update(Vector2 currPos, Vector2 HalfHalf)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            //Update da câmara a ser usada
            for (int i = 0; i < cameraKeys.Length; i++)
            {
                if (keyboardState.IsKeyDown(cameraKeys[i]))
                {
                    CameraId = i;
                    break;
                }
            }

            
            yaw += (currPos.X - HalfHalf.X) * scale / 20;
            pitch -= (currPos.Y - HalfHalf.Y) * scale / 20;
            pitch = MathHelper.Clamp(pitch, -90, 90);

            //Cálculo da altura mínima
            float minHeight = Game1.terrain.retCameraHeight(pos);

            switch (CameraId)
            {
                //SurfaceFollow
                case 0:
                    if (keyboardState.IsKeyDown(Keys.NumPad8))
                    {
                        pos.X += (float)Math.Cos(MathHelper.ToRadians(yaw)) * CameraSpeed[1];
                        pos.Z += (float)Math.Sin(MathHelper.ToRadians(yaw)) * CameraSpeed[1];
                    }

                    if (keyboardState.IsKeyDown(Keys.NumPad5))
                    {
                        pos.X -= (float)Math.Cos(MathHelper.ToRadians(yaw)) * CameraSpeed[1];
                        pos.Z -= (float)Math.Sin(MathHelper.ToRadians(yaw)) * CameraSpeed[1];
                    }

                    if (keyboardState.IsKeyDown(Keys.NumPad4))
                    {
                        pos.X += (float)Math.Cos(MathHelper.ToRadians(yaw - 90)) * CameraSpeed[1] * 0.5f;
                        pos.Z += (float)Math.Sin(MathHelper.ToRadians(yaw - 90)) * CameraSpeed[1] * 0.5f;
                    }

                    if (keyboardState.IsKeyDown(Keys.NumPad6))
                    {
                        pos.X += (float)Math.Cos(MathHelper.ToRadians(yaw + 90)) * CameraSpeed[1] * 0.75f;
                        pos.Z += (float)Math.Sin(MathHelper.ToRadians(yaw + 90)) * CameraSpeed[1] * 0.75f;
                    }

                    pos.Y = minHeight + HeightOffset;
                    break;

                //FreeRoam
                case 1:
                    if (keyboardState.IsKeyDown(Keys.NumPad8))
                    {
                        pos.X += (float)Math.Cos(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
                        pos.Y += (float)Math.Sin(MathHelper.ToRadians(-pitch)) * CameraSpeed[0];
                        pos.Z += (float)Math.Sin(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
                    }

                    if (keyboardState.IsKeyDown(Keys.NumPad5))
                    {
                        pos.X -= (float)Math.Cos(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
                        pos.Y -= (float)Math.Sin(MathHelper.ToRadians(-pitch)) * CameraSpeed[0];
                        pos.Z -= (float)Math.Sin(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
                    }

                    if (keyboardState.IsKeyDown(Keys.NumPad4))
                    {
                        pos.X += (float)Math.Cos(MathHelper.ToRadians(yaw - 90)) * CameraSpeed[1] * 0.8f;
                        pos.Z += (float)Math.Sin(MathHelper.ToRadians(yaw - 90)) * CameraSpeed[1] * 0.8f;
                    }

                    if (keyboardState.IsKeyDown(Keys.NumPad6))
                    {
                        pos.X += (float)Math.Cos(MathHelper.ToRadians(yaw + 90)) * CameraSpeed[1] * 0.8f;
                        pos.Z += (float)Math.Sin(MathHelper.ToRadians(yaw + 90)) * CameraSpeed[1] * 0.8f;
                    }

                    if (keyboardState.IsKeyDown(Keys.NumPad1))
                        pos.Y -= 1f;

                    if (keyboardState.IsKeyDown(Keys.NumPad7))
                        pos.Y += 1f;

                    pos.Y = MathHelper.Clamp(pos.Y, minHeight + HeightOffset, 100);
                    break;
                default:
                    break;
            }

            pos.X = MathHelper.Clamp(pos.X, 0, (Game1.terrain.Width - 2));
            pos.Z = MathHelper.Clamp(pos.Z, 0, (Game1.terrain.Height - 2));

            viewMatrix = Matrix.CreateLookAt(pos,pos + cameraTarguet,Vector3.Up)
                * Matrix.CreateRotationY(MathHelper.ToRadians(yaw))
                * Matrix.CreateRotationX(MathHelper.ToRadians(pitch));
        }
    }
}