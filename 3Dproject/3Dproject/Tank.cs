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

        protected BasicEffect effect;

        Model tankModel;
        protected Model Bullet;
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

        protected float scale, steerMult = 1, HP = 100;
        protected float turretYaw = 0f;
        protected float canonPitch = 0f;

        string[] wheelNames = { "l_front_wheel_geo", "r_front_wheel_geo", "l_back_wheel_geo", "r_back_wheel_geo" };
        string[] steerNames = { "r_steer_geo", "l_steer_geo" };

        protected float TankYaw = 0f, steerYaw = 0, hatchRotation = 0;
        protected float[] wheelsRotation;
        protected float[] wheelYaw;
        protected float limitZ, limitX;

        protected Vector3 position;
        protected Vector3 BulletTrajectory = Vector3.Zero;            

        protected static List<Bullet> bulletList;
        protected Keys[] movementKeys;   

        public BoundingSphere Sphere;

        protected int ID;
        protected Vector3 cannonPos = Vector3.Zero;
        protected PSystem particleSystem;
        protected Vector3[] wheelsPos;
        protected float RightY;
        protected Vector3 direction = Vector3.Zero;
        protected Vector3 tankNormal = Vector3.Zero;
        protected Vector3 tankRight = Vector3.Zero;
        protected Vector3 tankFront = Vector3.Zero;
        protected Matrix inclinationMatrix;

        Random rand;
        Vector3[] WheelsDir;
        Vector3 TanKMid;
        private bool exploded = false;
        private bool respawn = false;
        private bool isBot;

        private const float delay = 3f;
        private float remainingDelay = delay;

        public Tank(GraphicsDevice device, ContentManager content, Vector3 _position,int _id ,Keys[] _movementKeys, bool _isBot)
        {
            rand = new Random();
            isBot = _isBot;
            scale = 0.01f;
            position = _position;
            wheelsPos = new Vector3[wheelNames.Length];
            WheelsDir = new Vector3[wheelNames.Length];
            //TEST
            basicEffect = new BasicEffect(device);
            //TEST

            ID = _id;
            movementKeys = _movementKeys;

            effect = new BasicEffect(device);

            tankModel = content.Load<Model>("Tank/tank");
            Bullet = content.Load<Model>("Bullet/Sphere");

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

            limitX = (Game1.terrain.Width - 2);
            limitZ = (Game1.terrain.Height - 2);

            boneTransforms = new Matrix[tankModel.Bones.Count];
            wheelsRotation = new float[4];
            worldMatrix = Matrix.Identity;
            bulletList = new List<Bullet>();

            foreach (var item in tankModel.Meshes)
            {
                Sphere = BoundingSphere.CreateMerged(Sphere, new BoundingSphere(new Vector3(item.BoundingSphere.Center.X * scale, item.BoundingSphere.Center.Y * scale, item.BoundingSphere.Center.Z * scale), item.BoundingSphere.Radius * scale));
            }

            wheelYaw = new float[] { steerYaw + TankYaw, steerYaw + TankYaw, TankYaw, TankYaw };
            RightY = 0;
            particleSystem = new PSystem(device, 2f, 200, 10000, Color.Yellow/* new Color(130,75,0)*/);
        }

        public bool collides(Vector3 _center)
        {
            Sphere.Center = _center;
            foreach (var other in Game1.TankList)
            {
                if (other != this)
                {
                    Vector3 distance = Sphere.Center - other.Sphere.Center;
                    if (distance.Length() <= Sphere.Radius + other.Sphere.Radius)                    
                        return true;                    
                }
            }
            return false;
        }        

        public void GotHit()
        {
            HP -= 40;

            if (HP <= 0)
            {
                exploded = true;
                TanKMid = Vector3.Up / 2;
                for (int i = 0; i < wheelNames.Length; i++)
                {
                    WheelsDir[i] = wheelsPos[i] - position;
                    WheelsDir[i] = Vector3.Transform(WheelsDir[i], Matrix.CreateFromAxisAngle(tankNormal, (float)-rand.NextDouble() * 20 + (float)rand.NextDouble() * 20));
                }
                particleSystem.FireParticles(position, tankNormal, tankNormal * 2, tankRight, 1000, Color.Yellow);
            }
        }

        public Vector3 returnPosition()
        {
            return position;
        }
        public float returnYaw()
        {
            return TankYaw;
        }
        public bool isAlive()
        {
            return !exploded;
        }
        public bool Respawn()
        {
            return respawn;
        }
        public bool IsBot()
        {
            return isBot;
        }

        public float[] addArrays(float[] a1, float[] a2)
        {
            float[] result = new float[a1.Length];

            for (int i = 0; i < a1.Length; i++)
                result[i] = a1[i] + a2[i];
            return result;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public void UpdateBullets()
        {            
            for (int i = 0; i < bulletList.Count; i++)
            {
                Vector3 pos = bulletList[i].returnPosition();
                float minHeight = Game1.terrain.retCameraHeight(pos);
                if (pos.Y <= minHeight || pos.X <= 0 || pos.X >= limitX || pos.Z <= 0 || pos.Z >= limitZ || bulletList[i].hit)
                {
                    Vector3 bullPos = bulletList[i].returnPosition();
                    Vector3 terrNorm = Game1.terrain.retTerrainNormal(bullPos);
                    particleSystem.FireParticles(bullPos, terrNorm , terrNorm ,Vector3.Right, 500, Color.Yellow);
                    bulletList.Remove(bulletList[i]);
                }
                else bulletList[i].Update();
            }
        }

        public void UpdateParticles(GameTime gameTime)
        {
            particleSystem.Update(gameTime);
        }

        public void DrawBullets(GraphicsDevice device, BoundingFrustum frustum)
        {
            List<Bullet> visibleBullets = bulletList.Where(m => frustum.Contains(m.Sphere) != ContainmentType.Disjoint).ToList();
            foreach (var item in visibleBullets)
                item.Draw(device);
        }

        public void Draw(GraphicsDevice device, GameTime gameTime)
        {
            direction = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(MathHelper.ToRadians(270 + TankYaw)));

            tankNormal = Game1.terrain.retTerrainNormal(position);
            tankRight = Vector3.Cross(direction, tankNormal);
            tankFront = Vector3.Cross(tankNormal, tankRight);
            inclinationMatrix = Matrix.CreateWorld(position, tankFront, tankNormal);

            tankModel.Root.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * inclinationMatrix;

            turretBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(turretYaw)) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-canonPitch)) * cannonTransform;
            hatchBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(hatchRotation)) * hatchtransform;

            if (!exploded)
            {
                for (int i = 0; i < wheelsBones.Length; i++)
                    wheelsBones[i].Transform = Matrix.CreateRotationX(MathHelper.ToRadians(wheelsRotation[i])) * wheelsTransform[i];

                for (int i = 0; i < steerBones.Length; i++)
                    steerBones[i].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(steerYaw)) * steerTransform[i];
            }
            else
            {
                for (int i = 0; i < wheelsBones.Length; i++)
                {
                    wheelsBones[i].Transform = Matrix.CreateTranslation(WheelsDir[i]) * wheelsTransform[i];
                    WheelsDir[i] += WheelsDir[i];
                }
                for (int i = 0; i < steerBones.Length; i++)
                    steerBones[i].Transform = Matrix.CreateTranslation(WheelsDir[i]) * steerTransform[i];

                if (position.Y >= Game1.terrain.retCameraHeight(position) - 1)
                {
                    position += TanKMid;
                    TanKMid.Y -= 0.01f;
                }
                float timer = (float)gameTime.ElapsedGameTime.TotalSeconds;

                remainingDelay -= timer;

                if (remainingDelay <= 0)
                    respawn = true;                  
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
            }

            BulletTrajectory = Vector3.Transform(tankFront, Matrix.CreateFromAxisAngle(tankRight,
                                                MathHelper.ToRadians(canonPitch)) * Matrix.CreateFromAxisAngle(tankNormal,
                                                MathHelper.ToRadians(turretYaw)));

            cannonPos = boneTransforms[tankModel.Meshes["canon_geo"].ParentBone.Index].Translation * scale;

            //Posição das rodas
            for (int i = 0; i < wheelNames.Length; i++)
            {
                wheelsPos[i] = position + boneTransforms[tankModel.Meshes[wheelNames[i]].ParentBone.Index].Translation * scale;
            }

            //Desenhar Particulas
            RightY = tankRight.Y;
            particleSystem.Draw(device, Game1.MainCamera.viewMatrix, Game1.MainCamera.projectionMatrix);

            //for (int i=0;i<4;i++)
            //    DrawVectors(device, position, position + scale * boneTransforms[tankModel.Meshes[wheelNames[i]].ParentBone.Index].Translation, Color.White);


            //TEST

            //DrawVectors(device, position, position + new Vector3(scale * boneTransforms[tankModel.Meshes[wheelNames[0]].ParentBone.Index].Translation.X + 1.5f * (float)Math.Cos(MathHelper.ToRadians(steerYaw + TankYaw)),
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[0]].ParentBone.Index].Translation.Y - 1.8f * tankRight.Y,
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[0]].ParentBone.Index].Translation.Z - 1.5f * (float)Math.Sin(MathHelper.ToRadians(steerYaw + TankYaw))),
            //                                                     Color.Red);
            //DrawVectors(device, position, position + tankRight, Color.Red);

            //DrawVectors(device, position, position + new Vector3(scale * boneTransforms[tankModel.Meshes[wheelNames[1]].ParentBone.Index].Translation.X - 1.5f * (float)Math.Cos(MathHelper.ToRadians(steerYaw + TankYaw)),
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[1]].ParentBone.Index].Translation.Y + 1.8f * tankRight.Y,
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[1]].ParentBone.Index].Translation.Z + 1.5f * (float)Math.Sin(MathHelper.ToRadians(steerYaw + TankYaw))),
            //                                                     Color.Yellow);
            //DrawVectors(device, position, position + new Vector3(scale * boneTransforms[tankModel.Meshes[wheelNames[2]].ParentBone.Index].Translation.X - 1.5f * (float)Math.Cos(MathHelper.ToRadians(TankYaw)),
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[2]].ParentBone.Index].Translation.Y + 1.8f * tankRight.Y,
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[2]].ParentBone.Index].Translation.Z + 1.5f * (float)Math.Sin(MathHelper.ToRadians(TankYaw))),
            //                                                     Color.Blue);
            //DrawVectors(device, position, position + new Vector3(scale * boneTransforms[tankModel.Meshes[wheelNames[3]].ParentBone.Index].Translation.X + 1.5f * (float)Math.Cos(MathHelper.ToRadians(TankYaw)),
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[3]].ParentBone.Index].Translation.Y - 1.8f * tankRight.Y,
            //                                                     scale * boneTransforms[tankModel.Meshes[wheelNames[3]].ParentBone.Index].Translation.Z - 1.5f * (float)Math.Sin(MathHelper.ToRadians(TankYaw))),
            //                                                     Color.Green);

            //DrawVectors(device, position + scale * boneTransforms[tankModel.Meshes[wheelNames[0]].ParentBone.Index].Translation,
            //                    position + scale * boneTransforms[tankModel.Meshes[wheelNames[0]].ParentBone.Index].Translation - 1.8f * tankRight, Color.Violet); 


            //DrawVectors(device, position + cannonPos, position + cannonPos + BulletTrajectory, Color.Green);
            //DrawVectors(device, position, position + tankFront, Color.White);            
            //DrawVectors(device, position, position + direction, Color.HotPink);
            //DrawVectors(device, position, position + BulletTrajectory, Color.LightBlue);
            //DrawVectors(device, position, position + new Vector3(raio, 0, 0), Color.HotPink);
            //DrawVectors(device, position, position + new Vector3(-raio, 0, 0), Color.HotPink);
            //DrawVectors(device, position, position + new Vector3(0, raio, 0), Color.HotPink);
            //DrawVectors(device, position, position + new Vector3(0, -raio, 0), Color.HotPink);
            //DrawVectors(device, position, position + new Vector3(0, 0, raio), Color.HotPink);
            //DrawVectors(device, position, position + new Vector3(0, 0, -raio), Color.HotPink);
            //TEST
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