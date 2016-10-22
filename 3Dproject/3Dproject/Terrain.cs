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
        private float YSCALE = 0f;
        float[,] HeightData;

        VertexPositionNormalTexture[] vertices;

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
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.LightingEnabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
            effect.DirectionalLight0.Direction = new Vector3(.5f, -0.8f, 0);
            worldMatrix = Matrix.Identity;          

            YSCALE = _yScale;

            CreateHeightMap(content);
            CreateGeometry(device);      
        }
           
        public int[] RetWidthAndHeight()
        {
            int[] aux = {Width , Height};
            return aux;
        } 
                
        private void CreateHeightMap(ContentManager content)
        {
            Texture2D YTexture = content.Load<Texture2D>("Hmap1");

            Color[] colorArray = new Color[YTexture.Width * YTexture.Height];
            YTexture.GetData(colorArray);

            HeightData = new float[YTexture.Width, YTexture.Height];

            Width = YTexture.Width;
            Height = YTexture.Height;

            for (int x = 0; x < YTexture.Width; x++)
            {
                for (int y = 0; y < YTexture.Height; y++)
                {
                    HeightData[x, y] = colorArray[x + y * YTexture.Width].R / (YSCALE);
                }
            }            
        }

        public float retCameraHeight(Vector3 P)
        {
            float UpLeft = HeightData[(int)P.X, (int)P.Z];
            float UpRight = HeightData[(int)P.X + 1, (int)P.Z];
            float BotLeft = HeightData[(int)P.X, (int)P.Z + 1];
            float BotRight = HeightData[(int)P.X + 1, (int)P.Z + 1];

            float dX = 1 - (P.X - (int)P.X);
            float dY = 1 - (P.Z - (int)P.Z);

            return UpLeft * dX * dY + UpRight * dY * (1 - dX) + BotLeft * dX * (1 - dY) + BotRight * (1 - dX) * (1 - dY);
        }
       
        private void CreateGeometry(GraphicsDevice device)
        {

            vertices = new VertexPositionNormalTexture[Width * Height];

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Height; z++)
                {

                    vertices[x + z * Width] = new VertexPositionNormalTexture(new Vector3(x, (float)HeightData[x, z], z)
                        , Vector3.Zero,
                        new Vector2((float)x / 30f, (float)z / 30f));
                }                
            }

            short[] index = new short[Width * 2*(Height - 1)];

            int count = 0;
            for (int y = 0; y < Height - 1; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //TriangleStrip
                    int bottom = x + y * Width;
                    int top = x + (y + 1) * Width;

                    index[count] = (short)bottom;
                    count++;
                    index[count] = (short)top;
                    count++;
                    /*
                    //TriangleList
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
                    */
                }
            }

            for (int i = 0; i < index.Length / 3; i++)
            {
                int id1 = index[3 * i];
                int id2 = index[3 * i + 1];
                int id3 = index[3 * i + 2];

                Vector3 upLenght = vertices[id1].Position - vertices[id3].Position;
                Vector3 downLength = vertices[id1].Position - vertices[id2].Position;
                Vector3 normal = Vector3.Cross(upLenght, downLength);
                normal.Normalize();

                vertices[id1].Normal = normal;
                vertices[id2].Normal = normal;
                vertices[id3].Normal = normal;
            }

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
            indexBuffer = new IndexBuffer(device, typeof(short), index.Length, BufferUsage.None);
            indexBuffer.SetData<short>(index);            
       }

        public void Draw(GraphicsDevice device)
        { 
            effect.World =  worldMatrix;
            effect.View = Game1.MainCamera.viewMatrix;
            effect.Projection = Game1.MainCamera.projectionMatrix;
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            for (int y = 0; y < Height - 1; y++)
            {
                device.DrawIndexedPrimitives(PrimitiveType.LineStrip, 0, y * 2 * Width, (Width - 1) * 2);
            }
        }
    }
}