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
        int PrevScrollWeelValue = 0;
        float yaw = 0f, pitch = 0f,aspectRatio, scale = 1f;
        float HeightOffset = 2f, aux = 0;
        Vector3 position, cameraTarguet,add = new Vector3(10,0,10);

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

        public void Update(Vector2 currPos, Vector2 HalfHalf, int scrollValue)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
                add.Z += 0.2f;
            if (keyboardState.IsKeyDown(Keys.S))
                add.Z -= 0.2f;
            if (keyboardState.IsKeyDown(Keys.A))
                add.X += 0.2f;
            if (keyboardState.IsKeyDown(Keys.D))
                add.X -= 0.2f;

            if (scrollValue < PrevScrollWeelValue)
                add.Y -= 0.8f;
            if (scrollValue > PrevScrollWeelValue)
                add.Y += 0.8f;

            yaw += (currPos.X - HalfHalf.X) * scale / 20;
            pitch += (currPos.Y - HalfHalf.Y) * scale / 20;            

            pitch = MathHelper.Clamp(pitch, -90, 90);
            add.X = MathHelper.Clamp(add.X,0,(Game1.t.Width -2));
            add.Z = MathHelper.Clamp(add.Z,0,(Game1.t.Height -2));

            aux = Game1.t.retCameraHight(add);
            add.Y = MathHelper.Clamp(add.Y, aux + HeightOffset, 60);            

            viewMatrix = Matrix.CreateLookAt(add,add + cameraTarguet,Vector3.Up)
                * Matrix.CreateRotationY(MathHelper.ToRadians(yaw))
                * Matrix.CreateRotationX(MathHelper.ToRadians(pitch));

            PrevScrollWeelValue = scrollValue;
            Debug.WriteLine("Position : ("+ (int)add.X+ ","+ (int)add.Y+ ","+(int)add.Z + ")-" + aux + "-" + add.Y + "\n TARGUET ("+ cameraTarguet.X+"|"+ cameraTarguet.Y+"|"+cameraTarguet.Z +")");
        }

    }
}