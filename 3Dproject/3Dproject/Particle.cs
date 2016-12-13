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
        private bool state = true, checkHeight;
        private double timer;
        private Color color;

        private Vector3 accel = new Vector3(0, -.01f, 0);


        //Criar particula
        public Particle(int id, Vector3 pos, float RightY, float yaw, float mult, Random d, Color c)
        {
            checkHeight = true;
            timer= d.NextDouble() * 1.5;

            //Posição de saída aleatória (dentro dos limites)
            posicao = pos + GetDist(id, RightY, yaw, d);

            //Pitch de saída aleatória
            float angulo = (float)d.NextDouble() * 45;
            direc = new Vector3(0, (float)Math.Sin(MathHelper.ToRadians(angulo)), 0);

            //Yaw de saída aleatório
            angulo = (float)d.NextDouble() * 90;
            direc.X = -mult * (float)Math.Cos(MathHelper.ToRadians(-yaw + angulo + 45));
            direc.Z = -mult * (float)Math.Sin(MathHelper.ToRadians(-yaw + angulo + 45));
            direc.Normalize();

            //Tornar a velocidade mais aleatoria
            float vel = (float)d.NextDouble() * 1.5f;
            direc *= vel;
            int cOff = d.Next(-100, 50);
            color = new Color(c.R + cOff, c.G + cOff , c.B + cOff);
        }

        public Particle(Vector3 pos, Vector3 direction, Vector3 Up, Vector3 Right, Random d, Color c)
        {
            checkHeight = false;
            timer = d.NextDouble() * 1.5;

            posicao = pos;

            direc = 2 * Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Right,MathHelper.ToRadians((float)d.NextDouble()*-90f + (float)d.NextDouble() * 90f))
                                                 *Matrix.CreateFromAxisAngle(Up,MathHelper.ToRadians((float)d.NextDouble() * -90f + (float)d.NextDouble() * 90f)));
                                                                                
            direc *= 0.2f;
            ////Pitch de Saída aleatóro
            //float angulo = (float)d.NextDouble() * 140;
            //direc = new Vector3(0, (float)Math.Sin(MathHelper.ToRadians(pitch - 70 + angulo)), 0);

            ////Yaw de saída aleatório
            //angulo = (float)d.NextDouble() * 140;
            //direc.X = (float)Math.Cos(MathHelper.ToRadians(-yaw - 70 + angulo + 90));
            //direc.Z = (float)Math.Sin(MathHelper.ToRadians(-yaw - 70 + angulo + 90));
            //direc.Normalize();

            //Tornar a velocidade mais aleatoria
            float vel = (float)d.NextDouble() * 1.5f;
            direc *= vel;
            color = c;
        }

        public void Update(GameTime gametime)
        {
            direc += accel;
            posicao += direc * .25f;
            timer -= gametime.ElapsedGameTime.TotalSeconds;

            //UpdateState
            if (Position.X <= 2 || Position.X >= Game1.terrain.Width - 2 ||
                Position.Z <= 2 || Position.Z >= Game1.terrain.Height - 2 ||
                (checkHeight && Position.Y <= Game1.terrain.retCameraHeight(Position)) ||       
                timer <= 0)
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

        public Vector3 Lenght
        {
            get { return posicao - direc / 10; }
        }

        public Color Color
        {
            get { return color; }
        }

        public bool IsAlive()
        {
            return state;
        }

        //Indica uma distância aleatória ao longo do comprimento da roda toda
        private Vector3 GetDist(int id, float RightY, float yaw, Random random)
        {
            Vector3 dist=Vector3.Zero;
            switch(id)
            {
                case 0:
                case 3:
                    {
                        dist = new Vector3(1.5f * (float)Math.Cos(MathHelper.ToRadians(yaw)),
                                           - 1.8f * RightY,
                                           - 1.5f * (float)Math.Sin(MathHelper.ToRadians(yaw)));
                        break;
                    }
                case 1:
                case 2:
                    {
                        dist = new Vector3(- 1.5f * (float)Math.Cos(MathHelper.ToRadians(yaw)),
                                            1.8f * RightY,
                                            1.5f * (float)Math.Sin(MathHelper.ToRadians(yaw)));
                        break;
                    }

                default: break;
            }

            dist *= (float)random.NextDouble();

            return dist;
        }
    }
}