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
    public class Terrain
    {
        BasicEffect effect;
        Matrix worldMatrix;
               
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        Vector4 add = Vector4.Zero;

        Vector3 position = Vector3.Zero;                        
        
        public int Width = 0;
        public int Height = 0;
        private float YSCALE = 0f, yaw = 0;

        Vector3 viewPos = new Vector3(4f, 10f, -5.0f);

        Texture2D texture;

        public Terrain(GraphicsDevice device, ContentManager content , float _yScale)
        {            
            effect = new BasicEffect(device);

            texture = content.Load<Texture2D>("TerrainTexture1");

            float aspectRatio = (float)(device.Viewport.Width /
            device.Viewport.Height);
            effect.View = Matrix.CreateLookAt(
            viewPos,
            new Vector3(0, 5, 0), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45.0f),
            device.Viewport.AspectRatio, 1.0f, 500f);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            worldMatrix = Matrix.Identity;
            // Cria os eixos 3D          

            YSCALE = _yScale;
            
            CreateGeometry(device, content);      
        }
           
        public int[] RetWidthAndHeight()
        {
            int[] aux = {Width , Height};
            return aux;
        }
           
        public void Update()
        {
            //KeyboardState keyboardState = Keyboard.GetState();

            //if (keyboardState.IsKeyDown(Keys.Up))
            //    add.Z += 0.2f;
            //if (keyboardState.IsKeyDown(Keys.Down))
            //    add.Z -= 0.2f;
            //if (keyboardState.IsKeyDown(Keys.Left))
            //    add.X -= 2f;
            //if (keyboardState.IsKeyDown(Keys.Right))
            //    add.X += 2f;
            //if (keyboardState.IsKeyDown(Keys.W))
            //    add.Y -= 0.2f;
            //if (keyboardState.IsKeyDown(Keys.S))
            //    add.Y += 0.2f;
            //if (keyboardState.IsKeyDown(Keys.Z))
            //    add.W += 2f;
            //if (keyboardState.IsKeyDown(Keys.X))
            //    add.W -= 2f;

            //worldMatrix = Matrix.CreateScale(0.5f)
            //    * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(add.X), MathHelper.ToRadians(add.W), 0)
            //    * Matrix.CreateTranslation(new Vector3(0, add.Y, add.Z));

            ////KeyboardState keyboardState = Keyboard.GetState();

            ////float speed = 0.5f;
            ////if (keyboardState.IsKeyDown(Keys.W))
            ////{
            ////    position.X += (float)Math.Cos(yaw) * speed;
            ////    position.Z += -(float)Math.Sin(yaw) * speed;
            ////}
            ////if (keyboardState.IsKeyDown(Keys.S))
            ////{
            ////    position.X -= (float)Math.Cos(yaw) * speed;
            ////    position.Z -= -(float)Math.Sin(yaw) * speed;
            ////}
            ////if (keyboardState.IsKeyDown(Keys.Z))            
            ////    position.Y += 2f;            
            ////if (keyboardState.IsKeyDown(Keys.X))            
            ////    position.Y -= 2f;            
            ////if (keyboardState.IsKeyDown(Keys.A))
            ////    yaw -= 0.1f;
            ////if (keyboardState.IsKeyDown(Keys.D))
            ////    yaw += 0.1f;

            ////worldMatrix = Matrix.CreateScale(0.5f)
            ////    * Matrix.CreateRotationY(yaw)
            ////    * Matrix.CreateTranslation(position);
        }
        
        private int[,] CreateHightMap(ContentManager content)
        {
            Texture2D YTexture = content.Load<Texture2D>("Hmap");

            Color[] colorArray = new Color[YTexture.Width * YTexture.Height];
            YTexture.GetData(colorArray);

            int[,] hData = new int[YTexture.Width, YTexture.Height];

            Width = YTexture.Width;
            Height = YTexture.Height;

            for (int x = 0; x < YTexture.Width; x++)
            {
                for (int y = 0; y < YTexture.Height; y++)
                {
                    hData[x, y] = colorArray[x + y * YTexture.Width].R;
                }
            }
            return hData;
        }

        private void CreateGeometry(GraphicsDevice device, ContentManager content)
        {
            int[,] HeightData = CreateHightMap(content);

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[Width * Height];

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Height; z++)
                {                                                                    
                    vertices[x + z * Width] = new VertexPositionColorTexture(new Vector3(-x, (float)HeightData[x, z]/ YSCALE, -z)
                        , new Color(HeightData[x,z], HeightData[x, z]
                        , HeightData[x,z]),new Vector2((float)x / 30f, (float)z / 30f));
                }                
            }

            short[] index = new short[((Width - 1) * (Height - 1)) * 6];

            int count = 0;
            for (int y = 0; y < Height - 1; y++)
            {
                for (int x = 0; x < Width - 1; x++)
                {
                    int botomL = x + y * Width;
                    int topL = (x + 1) + y * Width;
                    int botomR = x + (y + 1) * Width;
                    int topR = (x + 1) + (y + 1) * Width;

                    index[count] = (short)botomL;
                    count++;
                    index[count] = (short)topL;
                    count++;
                    index[count] = (short)botomR;
                    count++;
                    index[count] = (short)botomR;
                    count++;
                    index[count] = (short)topL;
                    count++;
                    index[count] = (short)topR;
                    count++;
                }
            }            
            //for (int x = 0; x < Width - 1; x++)
            //{
            //    for (int y = 0; y < Height - 1; y++)
            //    {
            //        int botom = y % Width + Height * (x + 1);
            //        int top = botom - Height;

            //        index[count] = (short)botom;
            //        count++;
            //        index[count] = (short)top;
            //        count++;
            //    }
            //}
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColorTexture>(vertices);
            indexBuffer = new IndexBuffer(device, typeof(short), index.Length, BufferUsage.None);
            indexBuffer.SetData<short>(index);
        }

        public void Draw(GraphicsDevice device)
        {
            // World Matrix
            effect.World = worldMatrix;
            effect.View = Game1.MainCamera.viewMatrix;
            effect.Projection = Game1.MainCamera.projectionMatrix;
            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;                        
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, indexBuffer.IndexCount/3);         
        }
    }
}