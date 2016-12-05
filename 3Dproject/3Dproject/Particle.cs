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
    class Particle
    {
        private Vector3 Position;
        private Vector3 direction;

        bool state = true;//nao utilizado
        float DownSpeed = 10f, accel = 0.01f;
        float particleLength;

        //Caso necessitasse de um timer para cada particula
        //private const float delay = 5;
        //private float remainingDelay = delay;

        public Particle(float raio, Vector3 Center, Vector3 _direction, Random _random, float _particleLength)
        {
            particleLength = _particleLength;

            //criar angulo random de 0 a 360
            float angulo = (float)_random.NextDouble() * 360;
            raio *= (float)_random.NextDouble();
            float x = raio * (float)Math.Cos(MathHelper.ToRadians(angulo));
            float z = raio * -(float)Math.Sin(MathHelper.ToRadians(angulo));

            //Para cada particula nao caia á mesma velocidade e pareca mais real
            DownSpeed = DownSpeed / _random.Next(6, 14);

            Position = Center + new Vector3(x, Center.Y + 2f, z);

            //direçao random transformando o vetor 3 (1,0,0)
            //direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(360 * (float)_random.NextDouble()));
            //direction = new Vector3(direction.X / (25), DownSpeed * accel, direction.Z / (25));
            direction = new Vector3(_direction.X/100f , 0.02f, _direction.Z/100f);
        }

        public void Update(GameTime gameTime)
        {
            //var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //remainingDelay -= timer;

            //if (remainingDelay <= 0)
            //    state = false;

            Position += direction;
            if (accel < 1.3)
                accel *= 1.035f;

            direction.Y = DownSpeed * accel;

            Position.X = MathHelper.Clamp(Position.X, 0, Game1.terrain.Width - 2);
            Position.Z = MathHelper.Clamp(Position.Z, 0, Game1.terrain.Height - 2);
        }

        public Vector3 retPosition()
        {
            return Position;
        }

        public Vector3 retLastPosition()
        {
            return new Vector3(Position.X, Position.Y + particleLength, Position.Z);
        }

        public float retHeight()
        {
            return Position.Y;
        }

        public bool IsAlive()
        {
            return state;
        }
    }
}
