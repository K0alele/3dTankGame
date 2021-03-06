﻿using Microsoft.Xna.Framework;
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
        private const float delay = 1.5f;
        private float remainingDelay = delay;
        private bool canFire = true;

        public PlayerTank(GraphicsDevice device, ContentManager content, Vector3 _position, int _id,Keys[] _movementKeys) : base(device, content, _position,_id ,_movementKeys)
        {

        }

        public override void Update(GameTime gameTime)
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
                if ((keyboardState.IsKeyUp(movementKeys[2])) && (keyboardState.IsKeyUp(movementKeys[3])))
                {
                    wheelsRotation = addArrays(wheelsRotation, new float[] { -5f, -5f, -5f, 5f });
                    steerMult = -1f;
                    steerYaw = MathHelper.Clamp(steerYaw, -90, 90);
                }

                wheelYaw = new float[] { steerYaw + TankYaw, steerYaw + TankYaw, TankYaw, TankYaw };
                particleSystem.CreateParticles(wheelsPos, RightY, wheelYaw, steerMult, .2f, new Color(128, 57, 9));
            }

            else if (keyboardState.IsKeyDown(movementKeys[1]))
            {
                TankYaw -= 2f;
                steerYaw -= 5f * steerMult;
                if ((keyboardState.IsKeyUp(movementKeys[2])) && (keyboardState.IsKeyUp(movementKeys[3])))
                {
                    wheelsRotation = addArrays(wheelsRotation, new float[] { -5f, -5f, 5f, -5f });
                    steerMult = -1f;
                    steerYaw = MathHelper.Clamp(steerYaw, -90, 90);
                }

                wheelYaw = new float[] { steerYaw + TankYaw, steerYaw + TankYaw, TankYaw, TankYaw };
                particleSystem.CreateParticles(wheelsPos, RightY, wheelYaw, steerMult, .2f, new Color(128, 57, 9));
            }

            else if (!keyboardState.IsKeyDown(movementKeys[0]) && !keyboardState.IsKeyDown(movementKeys[1]))
            {
                if (steerYaw > 0)
                    steerYaw -= 5f;
                else if (steerYaw < 0)
                    steerYaw += 5f;

                wheelYaw = new float[] { steerYaw + TankYaw, steerYaw + TankYaw, TankYaw, TankYaw };
            }

            if (keyboardState.IsKeyDown(movementKeys[2]))
            {
                direction.X += (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.5f;
                direction.Z += (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.5f;
                wheelsRotation = addArrays(wheelsRotation, new float[] { 10f, 10f, 10f, 10f });
                steerMult = 1f;
                directionClamp();
                particleSystem.CreateParticles(wheelsPos, RightY, wheelYaw, steerMult, 1, new Color(128, 57, 9));
            }

            else if (keyboardState.IsKeyDown(movementKeys[3]))
            {
                direction.X -= (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.2f;
                direction.Z -= (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.2f;
                wheelsRotation = addArrays(wheelsRotation, new float[] { -5f, -5f, -5f, -5f });
                steerMult = -1f;
                directionClamp();
                particleSystem.CreateParticles(wheelsPos, RightY, wheelYaw, steerMult, .75f, new Color(128, 57, 9));
            }

            if (keyboardState.IsKeyDown(Keys.NumPad2))
                hatchRotation += 2f;
            else if (keyboardState.IsKeyDown(Keys.NumPad3))
                hatchRotation -= 2f;

            float timer = (float)gameTime.ElapsedGameTime.TotalSeconds;

            remainingDelay -= timer;

            if (remainingDelay <= 0)            
                canFire = true;                            

            if (keyboardState.IsKeyDown(movementKeys[4]) && canFire)
            {
                bulletList.Add(new Bullet(effect, Bullet, position + cannonPos + BulletTrajectory, BulletTrajectory, ID, 1f));
                particleSystem.FireParticles(position + cannonPos + BulletTrajectory*2, BulletTrajectory, tankNormal, tankRight, 500, Color.Yellow);
                canFire = false;
                remainingDelay = delay;
            }            

            bool col = collides(position + direction);

            if (!col)
                position += direction;

            hatchRotation = MathHelper.Clamp(hatchRotation, 0, 90);

            canonPitch = MathHelper.Clamp(canonPitch, -40, 90);

            position.X = MathHelper.Clamp(position.X, Sphere.Radius, limitX - Sphere.Radius);
            position.Z = MathHelper.Clamp(position.Z, Sphere.Radius, limitZ - Sphere.Radius);

            float minHeight = Game1.terrain.retCameraHeight(position);

            position.Y = minHeight;
        }        

        private void directionClamp()
        {
            if (steerYaw > 30)
                steerYaw -= 10f;
            else if (steerYaw < -30)
                steerYaw += 10f;
            else MathHelper.Clamp(steerYaw, -29, 29);            
        }
    }
}

