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
        private const float delay = 1.5f;
        private float remainingDelay = delay;
        private bool canFire = true;
        private float speed , prevYaw = 0f;        

        public BotTank(GraphicsDevice device, ContentManager content, Vector3 _position, int _id, Keys[] _movementKeys) : base(device, content, _position, _id, _movementKeys)
        {
            speed = 0.2f;
        }

        public override void Update(GameTime gameTime)
        {                    
            Vector3 otherTank = Game1.TankList[0].returnPosition();

            Vector3 distance = (otherTank - position);
            Vector3 direction = distance;
            direction.Normalize();
            direction *= speed;

            float tankYaw = Game1.TankList[0].returnYaw();

            tankF = Vector3.Backward;

            float angle = MathHelper.ToDegrees((float)Math.Acos((tankF.X * distance.X + tankF.Y * distance.Y + tankF.Z * distance.Z) / (tankF.Length() * distance.Length())));

            if (otherTank.X <= position.X)            
                angle = 360f - angle;

            prevYaw = TankYaw;
            TankYaw = angle;

            bool col = collides(position + direction);

            float mult = 1f;
            if (Math.Abs(prevYaw - TankYaw) <= 1 && Math.Abs(prevYaw - TankYaw) >= 0)            
                mult = Math.Abs(prevYaw - TankYaw);                

            if (prevYaw < TankYaw)
            {
                steerYaw += 1f * mult;
            }
            else if (prevYaw > TankYaw)
            {
                steerYaw -= 1f * mult;
            }

            if (distance.Length() >= 20 && !col)
            {
                position += direction;
                wheelsRotation = addArrays(wheelsRotation, new float[] { 10f, 10f, 10f, 10f });
                steerMult = 1f;               
                particleSystem.CreateParticles(wheelsPos, RightY, wheelYaw, steerMult, 1, new Color(128, 57, 9));
            }

            wheelYaw = new float[] { steerYaw + TankYaw, steerYaw + TankYaw, TankYaw, TankYaw };

            hatchRotation = MathHelper.Clamp(hatchRotation, 0, 90);

            canonPitch = MathHelper.Clamp(canonPitch, -20, 90);

            position.X = MathHelper.Clamp(position.X, Sphere.Radius, limitX - Sphere.Radius);
            position.Z = MathHelper.Clamp(position.Z, Sphere.Radius, limitZ - Sphere.Radius);

            float minHeight = Game1.terrain.retCameraHeight(position);

            position.Y = minHeight;

            //Update Particulas
            particleSystem.Update(gameTime);
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
