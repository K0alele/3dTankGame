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
    class Bullet
    {
        BasicEffect effect;

        Model BulletModel;
        Matrix worldMatrix;

        Matrix[] boneTransforms;

        Vector3 position;
        Vector3 direction;
        Vector3 gravity;

        float yaw, pitch, speed, scale;

        public Bullet(BasicEffect _effect, Model _model, Vector3 _position, float _yaw, float _pitch, float _speed)
        {            
            effect = _effect;
            BulletModel = _model;
            
            boneTransforms = new Matrix[BulletModel.Bones.Count];
            worldMatrix = Matrix.Identity;            

            scale = 0.2f;
            position = _position;
            position.Y += 3.5f;
            yaw = _yaw;
            pitch = _pitch;
            speed = _speed;

            gravity = Vector3.Zero;

            direction.X = (float)Math.Cos(MathHelper.ToRadians(-yaw + 90)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * speed;
            direction.Y = (float)Math.Sin(MathHelper.ToRadians(pitch)) * speed;
            direction.Z = (float)Math.Sin(MathHelper.ToRadians(-yaw + 90)) * (float)Math.Cos(MathHelper.ToRadians(pitch)) * speed;
        }

        public Vector3 returnPosition()
        {
            return position;
        }

        public void Update()
        {
            position += direction - gravity;
            gravity.Y += 0.005f;
        }

        public void Draw()
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
                }                
                mesh.Draw();
            }                        
        }
    }
}
