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
    public class BotTank : Tank
    {
        private const float delay = 2.5f;
        private float remainingDelay = delay;
        private bool canFire = false;
        private float speed , prevYaw = 0f;
        Vector3 aux = Vector3.Backward;

        public BotTank(GraphicsDevice device, ContentManager content, Vector3 _position, int _id, Keys[] _movementKeys, bool _isBot) : base(device, content, _position, _id, _movementKeys, _isBot)
        {
            speed = 0.2f;
        }

        public override void Update(GameTime gameTime)
        {
            float timer = (float)gameTime.ElapsedGameTime.TotalSeconds;

            remainingDelay -= timer;

            if (remainingDelay <= 0)
                canFire = true;

            Vector3 otherTank = Game1.TankList[0].returnPosition();

            Vector3 distance = (otherTank - position);
            Vector3 direction = distance;
            direction.Normalize();

            //Calcular o pitch do canhão dependendo da direçao e do pitch do tank
            float directionAngle = Math.Abs(direction.Y) / direction.Y * MathHelper.ToDegrees((float)Math.Acos((double)(new Vector3(direction.X,0,direction.Z).Length()/direction.Length())));
            float frontAngle = Math.Abs(tankFront.Y)/tankFront.Y * MathHelper.ToDegrees((float)Math.Acos((double)(new Vector3(tankFront.X, 0, tankFront.Z).Length() / tankFront.Length())));

            canonPitch = directionAngle - frontAngle;

            direction *= speed;

            float angle = MathHelper.ToDegrees((float)Math.Acos((aux.X * distance.X + aux.Y * distance.Y + aux.Z * distance.Z) / (aux.Length() * distance.Length())));

            if (otherTank.X < position.X)            
                angle = 360f - angle;

            prevYaw = TankYaw;
            TankYaw = angle;

            bool col = collides(position + direction);

            float mult = 1f;
            if (Math.Abs(prevYaw - TankYaw) <= 1 && Math.Abs(prevYaw - TankYaw) >= 0)            
                mult = Math.Abs(prevYaw - TankYaw);                

            if (prevYaw < TankYaw)           
                steerYaw += mult;          
            else if (prevYaw > TankYaw)           
                steerYaw -=  mult;

            if (distance.Length() >= 20 && !col)
            {
                position += direction;
                wheelsRotation = addArrays(wheelsRotation, new float[] { 10f, 10f, 10f, 10f });
                steerMult = 1f;               
                particleSystem.CreateParticles(wheelsPos, RightY, wheelYaw, steerMult, 1, new Color(128, 57, 9));
            }
            else if (canFire && distance.Length() < 20)
            {                
                bulletList.Add(new Bullet(effect, Bullet, position + cannonPos + BulletTrajectory, BulletTrajectory, ID, 1f));
                canFire = false;
                remainingDelay = delay;
            }            

            wheelYaw = new float[] { steerYaw + TankYaw, steerYaw + TankYaw, TankYaw, TankYaw };

            hatchRotation = MathHelper.Clamp(hatchRotation, 0, 90);

            canonPitch = MathHelper.Clamp(canonPitch, -40, 90);

            position.X = MathHelper.Clamp(position.X, Sphere.Radius, limitX - Sphere.Radius);
            position.Z = MathHelper.Clamp(position.Z, Sphere.Radius, limitZ - Sphere.Radius);

            float minHeight = Game1.terrain.retCameraHeight(position);

            position.Y = minHeight;

            directionClamp();
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
