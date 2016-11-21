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

        protected float TankYaw = 0f, steerYaw = 0, hatchRotation = 0, TankPitch = 0;
        protected float[] wheelsRotation;
        protected float limitZ, limitX;

        protected Vector3 position;
        protected Vector3 BulletTrajectory = Vector3.Zero;
        List<Vector3> BulletPath = new List<Vector3>();        

        protected static List<Bullet> bulletList;
        protected Keys[] movementKeys;

        protected KeyboardState prevKeyboard;        

        public BoundingSphere Sphere;

        protected int ID;
        private float raio = MathHelper.Pi + MathHelper.E / 3;
        protected Vector3 cannonPos = Vector3.Zero;

        public Tank(GraphicsDevice device, ContentManager content, Vector3 _position,int _id ,Keys[] _movementKeys)
        {
            scale = 0.01f;
            position = _position;
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
            prevKeyboard = Keyboard.GetState();
            bulletList = new List<Bullet>();

            foreach (var item in tankModel.Meshes)
            {
                Sphere = BoundingSphere.CreateMerged(Sphere, new BoundingSphere(new Vector3(item.BoundingSphere.Center.X * scale, item.BoundingSphere.Center.Y * scale, item.BoundingSphere.Center.Z * scale), item.BoundingSphere.Radius * scale));
            }

            //Sphere = new BoundingSphere(position, raio);
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
            HP -= 10;
        }

        public Vector3 returnPosition()
        {
            return position;
        }        

        public float[] addArrays(float[] a1, float[] a2)
        {
            float[] result = new float[a1.Length];

            for (int i = 0; i < a1.Length; i++)
                result[i] = a1[i] + a2[i];
            return result;
        }

        public void Draw(GraphicsDevice device)
        {
            Vector3 direction = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(MathHelper.ToRadians(270 + TankYaw)));

            Vector3 tankNormal = Game1.terrain.retTerrainNormal(position);
            Vector3 tankRight = Vector3.Cross(direction, tankNormal);
            Vector3 tankFront = Vector3.Cross(tankNormal, tankRight);

            Matrix inclinationMatrix = Matrix.CreateWorld(position, tankFront, tankNormal);

            tankModel.Root.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * inclinationMatrix;

            turretBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(turretYaw)) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-canonPitch)) * cannonTransform;
            hatchBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(hatchRotation)) * hatchtransform;

            for (int i = 0; i < wheelsBones.Length; i++)
                wheelsBones[i].Transform = Matrix.CreateRotationX(MathHelper.ToRadians(wheelsRotation[i])) * wheelsTransform[i];
            for (int i = 0; i < steerBones.Length; i++)
                steerBones[i].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(steerYaw)) * steerTransform[i];

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

            foreach (var item in bulletList)
                item.Draw(device);

            BulletTrajectory =  Vector3.Transform(tankFront, Matrix.CreateFromAxisAngle(tankRight,
                                                MathHelper.ToRadians(canonPitch)) * Matrix.CreateFromAxisAngle(tankNormal,
                                                MathHelper.ToRadians(turretYaw)));

            cannonPos = boneTransforms[tankModel.Meshes["canon_geo"].ParentBone.Index].Translation * scale;

            Debug.WriteLine("HP : " + HP);

            //TEST
            //DrawVectors(device, position, position + cannonPos, Color.Red);         
            //DrawVectors(device, position, position + tankNormal, Color.Red);
            //DrawVectors(device, position, position + tankRight, Color.Green);
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
            //startPoint.Y += 4;
            //endPoint.Y += 4;
            VertexPositionColor[] vertices = new[] { new VertexPositionColor(startPoint, color), new VertexPositionColor(endPoint, color) };
            device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}