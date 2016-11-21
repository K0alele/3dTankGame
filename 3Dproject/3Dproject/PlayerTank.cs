using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public class PlayerTank : Tank
    {
        public PlayerTank(GraphicsDevice device, ContentManager content, Vector3 _position, int _id,Keys[] _movementKeys) : base(device, content, _position,_id ,_movementKeys)
        {

        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            Vector3 direction = Vector3.Zero;

            if (keyboardState.IsKeyDown(Keys.Up))
                canonPitch += 2f;
            if (keyboardState.IsKeyDown(Keys.Down))
                canonPitch -= 2f;
            if (keyboardState.IsKeyDown(Keys.Right))
                turretYaw -= 2.5f;
            if (keyboardState.IsKeyDown(Keys.Left))
                turretYaw += 2.5f;

            if (keyboardState.IsKeyDown(movementKeys[0]))
            {
                TankYaw += 2f;
                steerYaw += 5f * steerMult;
                if (!keyboardState.IsKeyDown(Keys.W) && !keyboardState.IsKeyDown(Keys.S))
                {
                    wheelsRotation = addArrays(wheelsRotation, new float[] { -5f, -5f, -5f, 5f });
                    steerMult = -1f;
                    steerYaw = MathHelper.Clamp(steerYaw, -90, 90);
                }
                else
                {
                    if (steerYaw > 30)
                        steerYaw -= 5f;
                    if (steerYaw < -30)
                        steerYaw += 5f;
                }
            }
            if (keyboardState.IsKeyDown(movementKeys[1]))
            {
                TankYaw -= 2f;
                steerYaw -= 5f * steerMult;
                if (!keyboardState.IsKeyDown(Keys.W) && !keyboardState.IsKeyDown(Keys.S))
                {
                    wheelsRotation = addArrays(wheelsRotation, new float[] { -5f, -5f, 5f, -5f });
                    steerMult = -1f;
                    steerYaw = MathHelper.Clamp(steerYaw, -90, 90);
                }
                else
                {
                    if (steerYaw > 30)
                        steerYaw -= 5f;
                    if (steerYaw < -30)
                        steerYaw += 5f;
                }
            }

            if (!keyboardState.IsKeyDown(movementKeys[0]) && !keyboardState.IsKeyDown(movementKeys[1]))
            {
                if (steerYaw > 0)
                    steerYaw -= 5f;
                else if (steerYaw < 0)
                    steerYaw += 5f;
            }

            if (keyboardState.IsKeyDown(movementKeys[2]))
            {
                direction.X += (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.5f;
                direction.Z += (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.5f;
                wheelsRotation = addArrays(wheelsRotation, new float[] { 10f, 10f, 10f, 10f });
                steerMult = 1f;

            }
            if (keyboardState.IsKeyDown(movementKeys[3]))
            {
                direction.X -= (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.2f;
                direction.Z -= (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.2f;
                wheelsRotation = addArrays(wheelsRotation, new float[] { -5f, -5f, -5f, -5f });
                steerMult = -1f;
            }

            if (keyboardState.IsKeyDown(Keys.NumPad2))
                hatchRotation += 2f;
            else if (keyboardState.IsKeyDown(Keys.NumPad3))
                hatchRotation -= 2f;

            if (keyboardState.IsKeyDown(movementKeys[4]) && !prevKeyboard.IsKeyDown(movementKeys[4]))
            {
                bulletList.Add(new Bullet(effect, Bullet, position + cannonPos, BulletTrajectory, ID,0.1f));
            }

            float minHeight = Game1.terrain.retCameraHeight(position);

            for (int i = 0; i < bulletList.Count; i++)
            {
                Vector3 pos = bulletList[i].returnPosition();
                if (pos.Y <= minHeight || pos.X <= 0 || pos.X >= limitX || pos.Z <= 0 || pos.Z >= limitZ || bulletList[i].hit)
                    bulletList.Remove(bulletList[i]);
                else bulletList[i].Update();
            }

            bool col = collides(position + direction);

            if (!col)
                position += direction;

            hatchRotation = MathHelper.Clamp(hatchRotation, 0, 90);

            canonPitch = MathHelper.Clamp(canonPitch, -20, 90);

            position.X = MathHelper.Clamp(position.X, Sphere.Radius, limitX - Sphere.Radius);
            position.Z = MathHelper.Clamp(position.Z, Sphere.Radius, limitZ - Sphere.Radius);

            prevKeyboard = keyboardState;

            position.Y = minHeight;
        }
    }
}

