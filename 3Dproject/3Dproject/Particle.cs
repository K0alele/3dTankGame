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
        private Vector3 posicao, direc;
        private bool state = true;

        //Aceleração Static para que todas as particulas tenham o mesmo comportamento quanto à gravidade/vento
        private Vector3 accel = new Vector3(0, -.01f, 0);


        public Particle(Vector3 pos, float yaw, float mult, Random d)
        {
            //Criar particula
            //Posição Aleatória

            float angulo = (float)d.NextDouble() * 45;
            float dist = (float)d.NextDouble() * 1.5f;

            posicao = pos /*+ dist * new Vector3(Math.Cos(yaw), 0;*/;
            direc = new Vector3(0, (float)Math.Sin(MathHelper.ToRadians(angulo)), 0);

            //Direção aleatória
            angulo = (float)d.NextDouble() * 90;

            direc.X = - mult * (float)Math.Cos(MathHelper.ToRadians(-yaw + angulo+45));
            direc.Z = - mult * (float)Math.Sin(MathHelper.ToRadians(-yaw + angulo+45));
            direc.Normalize();

            dist = (float)d.NextDouble();
            direc *= dist*dist*dist;
        }

        //Update sem vento
        public void Update()
        {
            direc += accel;
            posicao += direc ;

            //UpdateState
            if (Position.X <= 2 || Position.X >= Game1.terrain.Width - 2 ||
                Position.Z <= 2 || Position.Z >= Game1.terrain.Height - 2 ||
                Position.Y <= Game1.terrain.retCameraHeight(Position))
                state = false;
        }

        //Variáveis que se queiram noutras classes
        public Vector3 Acceleration
        {
            set { accel = value; }
            get { return accel; }
        }

        public Vector3 Position
        {
            get { return posicao; }
        }

        public Vector3 OutroPos
        {
            get { return posicao - direc/4; }
        }

        public bool IsAlive()
        {
            return state;
        }
    }
}
//    }
//    private Vector3 Position;
//        private Vector3 direction;

//        bool state = true;
//        float DownSpeed = 10f, accel = 0.01f;
//        float particleLength;

//        //Caso necessitasse de um timer para cada particula
//        //private const float delay = 5;
//        //private float remainingDelay = delay;

//        public Particle(float raio, Vector3 Center, Vector3 _direction, Random _random, float _particleLength)
//        {
//            particleLength = _particleLength;

//            //criar angulo random de 0 a 360
//            float angulo = (float)_random.NextDouble() * 360;
//            //raio *= (float)_random.NextDouble();
//            //float x = raio * (float)Math.Cos(MathHelper.ToRadians(angulo));
//            //float z = raio * -(float)Math.Sin(MathHelper.ToRadians(angulo));

//            //Para cada particula nao caia á mesma velocidade e pareca mais real
//            DownSpeed = DownSpeed / _random.Next(6, 14);

//            Position = new Vector3(Game1.terrain.Width / 2, 10, Game1.terrain.Height);/*Center + new Vector3(0, Center.Y + 1f, 0);*/

//            //direçao random transformando o vetor 3 (1,0,0)
//            //direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(360 * (float)_random.NextDouble()));
//            //direction = new Vector3(direction.X / (25), DownSpeed * accel, direction.Z / (25));
//            direction = new Vector3(_direction.X/100f , _direction.Y+0.02f, _direction.Z/100f);
//        }

//        public void Update(GameTime gameTime)
//        {
//            //var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;

//            //remainingDelay -= timer;

//            //if (remainingDelay <= 0)
//            //    state = false;

//            Position += direction;
//            if (accel < 1.3)
//                accel *= 1.035f;

//            direction.Y -= DownSpeed * accel*.01f;

//            //UpdateState
//            if (Position.X <= 2 || Position.X >= Game1.terrain.Width - 2 ||
//                Position.Z <= 2 || Position.Z >= Game1.terrain.Height - 2||
//                Position.Y <= Game1.terrain.retCameraHeight(Position))
//                state = false;
//        }

//        public Vector3 retPosition()
//        {
//            return Position;
//        }

//        public Vector3 retLastPosition()
//        {
//            return Position-direction/3;
//        }

//        public float retHeight()
//        {
//            return Position.Y;
//        }

//        public bool IsAlive()
//        {
//            return state;
//        }
//    }
//}
