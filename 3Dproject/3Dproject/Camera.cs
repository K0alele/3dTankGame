using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3Dproject
{
    public class Camera
    {
        float yaw = 0f, pitch = 0f,aspectRatio, scale = 1f;
        Vector3 position, direction,add = Vector3.Zero;

        public Matrix viewMatrix, projectionMatrix;        

        public Camera(GraphicsDevice device)
        {
            position = new Vector3(0, 60, -10);
            direction = new Vector3(0, 0, 1);
            viewMatrix = Matrix.CreateLookAt(position, position + direction, Vector3.Up);

            aspectRatio = (float)(device.Viewport.Width /
            device.Viewport.Height);                     
            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.3f, 1000.0f);
        }

        public void Update(Vector2 currPos, Vector2 HalfHalf)
        {

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
                add.Z += 0.2f;
            if (keyboardState.IsKeyDown(Keys.S))
                add.Z -= 0.2f;
            if (keyboardState.IsKeyDown(Keys.A))
                add.X -= 0.2f;
            if (keyboardState.IsKeyDown(Keys.D))
                add.X += 0.2f;

            if (keyboardState.IsKeyDown(Keys.Z))
                add.Y -= 0.2f;
            if (keyboardState.IsKeyDown(Keys.X))
                add.Y += 0.2f;

            yaw += (currPos.X - HalfHalf.X) * scale / 20;
            pitch += (currPos.Y - HalfHalf.Y) * scale / 20;

            pitch = MathHelper.Clamp(pitch, -90, 90);

            viewMatrix = Matrix.CreateScale(0.5f)
                * Matrix.CreateTranslation(add)
                * Matrix.CreateRotationY(MathHelper.ToRadians(yaw))
                * Matrix.CreateRotationX(MathHelper.ToRadians(pitch));
        }

    }
}