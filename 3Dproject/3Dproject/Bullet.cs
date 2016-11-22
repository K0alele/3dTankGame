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
    public class Bullet
    {
        BasicEffect basicEffect;

        Model BulletModel;
        Matrix worldMatrix;

        Matrix[] boneTransforms;

        Vector3 position;
        Vector3 direction;
        Vector3 gravity;

        float yaw, pitch, speed, scale, raio = 0.21f;
        public bool hit = false;
        private bool saiu = false;
        private int TankId;

        List<Vector3> prevPos = new List<Vector3>();

        BoundingSphere c;
        
        public Bullet(BasicEffect _effect, Model _model, Vector3 _position , Vector3 _direction,int _TankId, float _speed)
        {                        
            BulletModel = _model;
            
            boneTransforms = new Matrix[BulletModel.Bones.Count];
            worldMatrix = Matrix.Identity;            

            scale = 0.2f;
            position = _position;
            direction = _direction;          
            speed = _speed;
            TankId = _TankId;

            gravity = Vector3.Zero;

            c = new BoundingSphere(_position, raio);       
        }

        public Vector3 returnPosition()
        {
            return position;
        }

        public void Update()
        {
            c.Center = position;
            position += (direction - gravity) * speed;
            gravity.Y += 0.005f;
            collides();
            prevPos.Add(position);
        }

        public void collides()
        {
            c.Center = position;
            foreach (var item in Game1.TankList)
            {
                Vector3 distance = c.Center - item.Sphere.Center;
                if (distance.Length() <= raio + item.Sphere.Radius)
                {
                    if (saiu)
                    {
                        item.GotHit();
                        hit = true;
                    }                                         
                }
                if (TankId == Game1.TankList.IndexOf(item) && distance.Length() >= raio + item.Sphere.Radius)
                {
                    saiu = true;
                }
            }
        }

        public void Draw(GraphicsDevice device)
        {         
            BulletModel.Root.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(180));           

            BulletModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in BulletModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = Game1.MainCamera.projectionMatrix;
                    effect.View = Game1.MainCamera.viewMatrix;
                    effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

                    effect.LightingEnabled = false;                    

                    effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.DirectionalLight0.Direction = new Vector3(.5f, -1f, 0);
                    effect.DiffuseColor = new Vector3(0,0,0);
                }                
                mesh.Draw();
            }

            basicEffect = new BasicEffect(device);
        }

        public void DrawVectors(GraphicsDevice device, Vector3 startPoint, Vector3 endPoint, Color color)
        {
            basicEffect.Projection = Game1.MainCamera.projectionMatrix;
            basicEffect.View = Game1.MainCamera.viewMatrix;
            basicEffect.World = worldMatrix;
            basicEffect.VertexColorEnabled = true;
            basicEffect.CurrentTechnique.Passes[0].Apply();

            VertexPositionColor[] vertices = new[] { new VertexPositionColor(startPoint, color), new VertexPositionColor(endPoint, color) };
            device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
