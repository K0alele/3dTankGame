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
        
        private int amount, maxAmount;

        private Random random = new Random();
        private List<Particle> particles;
        private Matrix worldMatrix;

        private VertexPositionColor[] vertices;
        private Color particleColor;

        public PSystem(GraphicsDevice device, int _quant, int _quantMax, Color _color)
        {
            amount = _quant; //quantidade de particulas criadas por update
            particleColor = _color;
            maxAmount = _quantMax; //maxima quantidade de particulas

            vertices = new VertexPositionColor[maxAmount * 2];

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

        public void CreateParticles(Vector3[] pos, float RightY, float[] yaw, float mult, float ratio, Color color)
        {
            //Adicionar particulas se a quantidade das mesmas for menor que a quantidade maxima
            for (int i = 0; i < (int)(amount * ratio); i++)
            {
                if (particles.Count < maxAmount)
                    particles.Add(new Particle(i%pos.Length, pos[i % pos.Length], RightY, yaw[i % yaw.Length], mult, random, color));
                else break;
            }
        }

        public void FireParticles(Vector3 pos, Vector3 dir, Vector3 Up , Vector3 Right, int quant, Color color)
        {
            for (int i = 0; i < quant; i++)
            {
                if (particles.Count < maxAmount)
                    particles.Add(new Particle(pos, dir, Up, Right, random, color));
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

                for (int i = 0; i < particles.Count; i++)
                {
                    Color partColor = particles[i].Color;
                    vertices[i * 2] = new VertexPositionColor(particles[i].Position, partColor);
                    vertices[i * 2 + 1] = new VertexPositionColor(particles[i].Lenght, partColor);
                }

                device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, particles.Count);
            }
        }
    }
}
