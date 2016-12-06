using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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

        public void Update(GameTime gameTime)
        {
            //Update das Particulas
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
                if (!particles[i].IsAlive())
                    particles.RemoveAt(i);
            }
        }

        public void CreateParticles(Vector3[] pos, float RightY, float[] yaw, float mult, float ratio)
        {
            //Adicionar particulas se a quantidade das mesmas for menor que a quantidade maxima
            for (int i = 0; i < (int)(amount * ratio); i++)
            {
                if (particles.Count < maxAmount)
                    particles.Add(new Particle(i%pos.Length, pos[i % pos.Length], RightY, yaw[i % yaw.Length], mult, random));
                else break;
            }
        }

        public void FireParticles(Vector3 pos, Vector3 outroPos, int quant)
        {
            for (int i = 0; i < quant; i++)
            {
                if (particles.Count < maxAmount)
                    particles.Add(new Particle(pos,outroPos, random));
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
                    vertices[i * 2 + 1] = new VertexPositionColor(particles[i].Lenght, particleColor);
                }

                device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, particles.Count);
            }
        }
    }
}
