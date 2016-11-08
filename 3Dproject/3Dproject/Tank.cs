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
        ModelBone[] wheelsBones = new ModelBone[4];
        ModelBone[] steerBones = new ModelBone[2];
        ModelBone hatchBone;

        Matrix cannonTransform;
        Matrix turretTransform;
        Matrix[] wheelsTransform = new Matrix[4];
        Matrix[] steerTransform = new Matrix[2];
        Matrix hatchtransform;

        Matrix[] boneTransforms;

        float scale, steerMult = 1;
        private float turretYaw = 0f;
        private float canonPitch = 0f;

        string[] wheelNames = { "l_front_wheel_geo", "r_front_wheel_geo", "l_back_wheel_geo", "r_back_wheel_geo" };
        string[] steerNames = { "r_steer_geo", "l_steer_geo" };

        private float TankYaw = 0f, TankPitch = 0f, wheelsRotation = 0f, steerYaw = 0, hatchRotation = 0;

        public Vector3 position = new Vector3(10, 0, 10);

        public Tank(GraphicsDevice device, ContentManager content)
        {
            scale = 0.01f;

            //TEST
            basicEffect = new BasicEffect(device);
            //TEST

            effect = new BasicEffect(device);

            tankModel = content.Load<Model>("Tank/tank");

            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
            effect.DirectionalLight0.Direction = new Vector3(.5f, -1f, 0);

            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            for (int i = 0; i < wheelsBones.Length; i++)
                wheelsBones[i] = tankModel.Bones[wheelNames[i]];
            for (int i = 0; i < steerBones.Length; i++)
                steerBones[i] = tankModel.Bones[steerNames[i]];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchtransform = hatchBone.Transform;

            for (int i = 0; i < wheelsBones.Length; i++)
                wheelsTransform[i] = wheelsBones[i].Transform;
            for (int i = 0; i < steerBones.Length; i++)
                steerTransform[i] = steerBones[i].Transform;

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
                steerYaw += 2f * steerMult;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                TankYaw -= 2f;
                steerYaw -= 2f * steerMult;
            }
            if (!keyboardState.IsKeyDown(Keys.D) && !keyboardState.IsKeyDown(Keys.A))
            {
                if (steerYaw > 0)
                    steerYaw -= 4f;
                else
                    steerYaw += 4f;
            }

            steerYaw = MathHelper.Clamp(steerYaw, -25, 25);

            if (keyboardState.IsKeyDown(Keys.W))
            {
                position.X += (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.5f;
                position.Z += (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.5f;
                wheelsRotation += 10f;
                steerMult = 1f;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                position.X -= (float)Math.Sin(MathHelper.ToRadians(TankYaw)) * 0.2f;
                position.Z -= (float)Math.Cos(MathHelper.ToRadians(TankYaw)) * 0.2f;
                wheelsRotation -= 5f;
                steerMult = -1f;
            }

            if (keyboardState.IsKeyDown(Keys.O))
                hatchRotation += 2f;
            else if (keyboardState.IsKeyDown(Keys.I))
                hatchRotation -= 2f;

            hatchRotation = MathHelper.Clamp(hatchRotation, 0, 90);

            canonPitch = MathHelper.Clamp(canonPitch, -20, 90);

            position.X = MathHelper.Clamp(position.X, 0, (Game1.terrain.Width - 2));
            position.Z = MathHelper.Clamp(position.Z, 0, (Game1.terrain.Height - 2));

            float minHeight = Game1.terrain.retCameraHeight(position);
            Vector3 tankNormal = Game1.terrain.retTerrainNormal(position);

            position.Y = minHeight;
        }

        public void Draw(GraphicsDevice device)
        {
            Vector3 direction = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(MathHelper.ToRadians(270 + TankYaw)));
            //direction.Normalize();

            Vector3 tankNormal = Game1.terrain.retTerrainNormal(position);
            Vector3 tankRight = Vector3.Cross(direction, tankNormal);
            Vector3 tankFront = Vector3.Cross(tankNormal, tankRight);

            Matrix inclinationMatrix = Matrix.CreateWorld(position, tankFront, tankNormal);

            tankModel.Root.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * inclinationMatrix;

            turretBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(turretYaw - TankYaw)) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-canonPitch)) * cannonTransform;
            hatchBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(hatchRotation)) * hatchtransform;

            for (int i = 0; i < wheelsBones.Length; i++)
            {
                wheelsBones[i].Transform = Matrix.CreateRotationX(MathHelper.ToRadians(wheelsRotation)) * wheelsTransform[i];
            }
            for (int i = 0; i < steerBones.Length; i++)
            {
                steerBones[i].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(steerYaw)) * steerTransform[i];
            }

            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = Game1.MainCamera.projectionMatrix;
                    effect.View = Game1.MainCamera.viewMatrix;
                    effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

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
            VertexPositionColor[] vertices = new[] { new VertexPositionColor(startPoint, color), new VertexPositionColor(endPoint, color) };
            device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
