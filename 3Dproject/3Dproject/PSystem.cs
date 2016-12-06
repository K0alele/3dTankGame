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
    public class PSystem
    {
        private BasicEffect effect;

        private float raio;
        private int amount, maxAmount;
        private float StartAmount = 2;
        private bool isPaused = false;

        private Random random = new Random();
        private List<Particle> particles;
        private Matrix worldMatrix;

        private Color particleColor;

        public PSystem(GraphicsDevice device ,float _raio, int _quant, int _quantMax, Color _color)
        {

            raio = _raio;
            amount = _quant; //quantidade de particulas criadas por update
            particleColor = _color;
            maxAmount = _quantMax; //maxima quantidade de particulas

            effect = new BasicEffect(device);

            effect.VertexColorEnabled = true;

            particles = new List<Particle>();

            worldMatrix = Matrix.Identity;
        }

        public void Update()
        {
            //Update das Particulas
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update();
                if (!particles[i].IsAlive())
                    particles.RemoveAt(i);
            }
            Debug.WriteLine(particles.Count.ToString());

        }

        public void CreateParticles(Vector3[] pos, float[] yaw, float mult)
        {
            //Adicionar particulas se a quantidade das mesmas for menor que a quantidade maxima
            for (int i = 0; i < amount; i++)
            {
                if (particles.Count < maxAmount)
                    particles.Add(new Particle(pos[i % pos.Length], yaw[i % yaw.Length], mult, random));
                else break;
            }
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            if (particles.Count != 0)
            {
                effect.Projection = projection;
                effect.View = view;
                effect.World = worldMatrix;

                effect.CurrentTechnique.Passes[0].Apply();

                VertexPositionColor[] vertices = new VertexPositionColor[particles.Count * 2];

                for (int i = 0; i < particles.Count; i++)
                {
                    vertices[i * 2] = new VertexPositionColor(particles[i].Position, particleColor);
                    vertices[i * 2 + 1] = new VertexPositionColor(particles[i].OutroPos, particleColor);
                }
                //Debug.WriteLine(particles.Count.ToString() + "\npos1: " + particles[0].Position.ToString() + "\npos2: " + particles[0].OutroPos.ToString());

                device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, particles.Count);
            }
        }
        //Recomeçar a chuva
        public void ClearRain()
        {
            particles.Clear();
            StartAmount = 2;
        }
        //Pausar ou tirar a pausa da chuva
        public void ChangeState()
        {
            isPaused = !isPaused;
        }
    }
}
