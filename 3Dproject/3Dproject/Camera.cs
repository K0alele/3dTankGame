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
        bool canPress = true;
        Vector3 position, cameraTarguet,add = new Vector3(10,0,10);

        Keys[] cameraKeys = { Keys.F1, Keys.F2 };        

        public Matrix viewMatrix, projectionMatrix;        

        public Camera(GraphicsDevice device)
        {
            position = new Vector3(0, 60, -10);
            cameraTarguet = new Vector3(10, 0, 0);
            viewMatrix = Matrix.CreateLookAt(position,cameraTarguet, Vector3.Up);

            aspectRatio = (float)(device.Viewport.Width /
            device.Viewport.Height);                     
            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.3f, 1000.0f);
        }

        public void Update(Vector2 currPos, Vector2 HalfHalf)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            yaw += (currPos.X - HalfHalf.X) * scale / 20;
            pitch -= (currPos.Y - HalfHalf.Y) * scale / 20;

            if (keyboardState.IsKeyDown(Keys.NumPad8))
            {
                add.X += (float)Math.Cos(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
                add.Y += (float)Math.Sin(MathHelper.ToRadians(-pitch)) * CameraSpeed[0];
                add.Z += (float)Math.Sin(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
            }
            if (keyboardState.IsKeyDown(Keys.NumPad5))
            {
                add.X -= (float)Math.Cos(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
                add.Y -= (float)Math.Sin(MathHelper.ToRadians(-pitch)) * CameraSpeed[0];
                add.Z -= (float)Math.Sin(MathHelper.ToRadians(yaw)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * CameraSpeed[0];
            }
            if (keyboardState.IsKeyDown(Keys.NumPad4))
            {
                add.X += (float)Math.Cos(MathHelper.ToRadians(yaw - 90)) * CameraSpeed[1] * 0.8f;
                add.Z += (float)Math.Sin(MathHelper.ToRadians(yaw - 90)) * CameraSpeed[1] * 0.8f;
            }
            if (keyboardState.IsKeyDown(Keys.NumPad6))
            {
                add.X += (float)Math.Cos(MathHelper.ToRadians(yaw + 90)) * CameraSpeed[1] * 0.8f;
                add.Z += (float)Math.Sin(MathHelper.ToRadians(yaw + 90)) * CameraSpeed[1] * 0.8f;
            }

            UpdateCameraHeight(keyboardState);

            pitch = MathHelper.Clamp(pitch, -90, 90);
            add.X = MathHelper.Clamp(add.X, 0, (Game1.terrain.Width - 2));
            add.Z = MathHelper.Clamp(add.Z, 0, (Game1.terrain.Height - 2));                      
            
            viewMatrix = Matrix.CreateLookAt(add,add + cameraTarguet,Vector3.Up)
                * Matrix.CreateRotationY(MathHelper.ToRadians(yaw))
                * Matrix.CreateRotationX(MathHelper.ToRadians(pitch));

            
            //Debug.WriteLine("Position : ("+ (int)add.X+ ","+ (int)add.Y+ ","+(int)add.Z + ")-" + minHeight + "-" + add.Y + "\n TARGUET ("+ yaw+"|"+ pitch+"|"+cameraTarguet.Z +")");
        }
        private void UpdateCameraHeight(KeyboardState keyboardState)
        {
            for (int i = 0; i < cameraKeys.Length; i++)
            {
                if (keyboardState.IsKeyDown(cameraKeys[i]))
                {
                    CameraId = i;
                    break;
                }
            }            

            float minHeight = Game1.terrain.retCameraHeight(add);

            switch (CameraId)
            {
                case 0:
                    add.Y = minHeight + HeightOffset;
                    break;
                case 1:
                    if (keyboardState.IsKeyDown(Keys.NumPad1))
                        add.Y -= 1f;
                    if (keyboardState.IsKeyDown(Keys.NumPad7))
                        add.Y += 1f;
                    add.Y = MathHelper.Clamp(add.Y, minHeight + HeightOffset, 100);
                    break;                
                default:                    
                    break;
            }                   
        }

        private int NextCamera( int _id)
        {
            if (_id == 2) return 1;
            return _id + 1;
        }
    }
}