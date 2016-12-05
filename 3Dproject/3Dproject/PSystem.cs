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

        public void Update(GameTime gameTime, Vector3[] direction, Vector3[] positions)
        {
            //Update das Particulas
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
                Vector3 pos = particles[i].retPosition();
                if (pos.Y <= Game1.terrain.retCameraHeight(pos) || !particles[i].IsAlive())
                    particles.RemoveAt(i);
            }
            //Adicionar particulas se a quantidade das mesmas for menor que a quantidade maxima
            for (int i = 0; i < amount; i++)
            {
                if (particles.Count < maxAmount)
                    particles.Add(new Particle(raio, positions[i % positions.Count() ], direction[i % positions.Count()], random, 0.4f));
                else break;
            }
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            effect.Projection = projection;
            effect.View = view;
            effect.World = worldMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            VertexPositionColor[] vertices = new VertexPositionColor[particles.Count * 2];

            for (int i = 0; i < particles.Count; i++)
            {
                vertices[i * 2] = new VertexPositionColor(particles[i].retPosition(), particleColor);
                vertices[i * 2 + 1] = new VertexPositionColor(particles[i].retLastPosition(), particleColor);
            }
            device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, particles.Count / 2);
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
