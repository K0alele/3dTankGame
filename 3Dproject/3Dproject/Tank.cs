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
    public class Tank
    {
        //TESTE
        BasicEffect basicEffect;
        //TESTE

        BasicEffect effect;

        Model tankModel;
        Matrix worldMatrix;

        ModelBone turretBone;
        ModelBone cannonBone;

        Matrix cannonTransform;
        Matrix turretTransform;

        Matrix[] boneTransforms;

        float scale;
        float turretYaw = 0f;
        float canonPitch = 0f;
        public float TankYaw = 0f, TankPitch = 0f;

        public Vector3 position = new Vector3(10, 0, 10);

        public Tank(GraphicsDevice device,ContentManager content)
        {
            scale = 0.01f;

            basicEffect = new BasicEffect(device);
            effect = new BasicEffect(device);

            tankModel = content.Load<Model>("Tank/tank");

            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
            effect.DirectionalLight0.Direction = new Vector3(.5f, -1f, 0);

            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;

            boneTransforms = new Matrix[tankModel.Bones.Count];
            worldMatrix = Matrix.Identity;
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Up))            
                canonPitch += 2f;            
            if (keyboardState.IsKeyDown(Keys.Down))            
                canonPitch -= 2f;            
            if (keyboardState.IsKeyDown(Keys.Right))           
                turretYaw -= 2f;            
            if (keyboardState.IsKeyDown(Keys.Left))            
                turretYaw += 2f;

            if (keyboardState.IsKeyDown(Keys.A))
            {
                TankYaw += 2f;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                TankYaw -= 2f;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                position.X += (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.5f;
                position.Z += (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.5f;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                position.X -= (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.2f;
                position.Z -= (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.2f;
            }

            canonPitch = MathHelper.Clamp(canonPitch, -20, 90);

            position.X = MathHelper.Clamp(position.X, 0, (Game1.terrain.Width - 2));
            position.Z = MathHelper.Clamp(position.Z, 0, (Game1.terrain.Height - 2));

            float minHeight = Game1.terrain.retCameraHeight(position);
            Vector3 tankNormal = Game1.terrain.retTerrainNormal(position);                    
           
            position.Y = minHeight;       
        }

        public void Draw(GraphicsDevice device)
        {
            Vector3 direction = Vector3.Transform(position, Matrix.CreateRotationY(MathHelper.ToRadians(TankYaw)));
            //direction.Normalize();

            Vector3 tankNormal = Game1.terrain.retTerrainNormal(position);
            Vector3 tankRight = Vector3.Cross(direction, tankNormal);
            Vector3 tankFront = Vector3.Cross(tankNormal, tankRight);

            Matrix inclinationMatrix = Matrix.CreateWorld(position, tankFront, tankNormal);

            tankModel.Root.Transform =  inclinationMatrix * Matrix.CreateScale(scale);

            turretBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(turretYaw - TankYaw)) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-canonPitch)) * cannonTransform;

            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Debug.WriteLine("YAW : "+ TankYaw);

            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = Game1.MainCamera.projectionMatrix;
                    effect.View = Game1.MainCamera.viewMatrix;
                    effect.World = boneTransforms[mesh.ParentBone.Index] /** Matrix.CreateScale(scale) */* Matrix.CreateTranslation(position);

                    effect.LightingEnabled = true;

                    effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
                    effect.DirectionalLight0.Direction = new Vector3(.5f, -1f, 0);
                }
                mesh.Draw();
                //TEST  
                DrawVectors(device, position, position + tankNormal, Color.Red);
                DrawVectors(device, position, position + tankRight, Color.Green);
                DrawVectors(device, position, position + tankFront, Color.Black);
                DrawVectors(device, position, position + direction, Color.HotPink);
                //TEST
            }
        }

        public void DrawVectors(GraphicsDevice device, Vector3 startPoint, Vector3 endPoint, Color color)
        {
            basicEffect.Projection = Game1.MainCamera.projectionMatrix;
            basicEffect.View = Game1.MainCamera.viewMatrix;
            basicEffect.World = worldMatrix;
            basicEffect.VertexColorEnabled = true;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            startPoint.Y += 4;
            endPoint.Y += 4;
            var vertices = new[] { new VertexPositionColor(startPoint, color), new VertexPositionColor(endPoint, color) };
            device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
