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
    public class Terrain
    {
        BasicEffect effect;
        Matrix worldMatrix;
               
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;                
        
        public int Width = 0;
        public int Height = 0;
        private float YSCALE = 0f;
        float[,] HeightData;
        
        Texture2D texture;
        Vector3[,] NormalData;

        public Terrain(GraphicsDevice device, ContentManager content , float _yScale)
        {            
            effect = new BasicEffect(device);

            texture = content.Load<Texture2D>("TerrainTexture1");

            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
            effect.DirectionalLight0.Direction = new Vector3(.5f, -1f, 0);
            worldMatrix = Matrix.Identity;

            //FOG YAY
            //effect.FogEnabled = true;
            //effect.FogColor = Color.LightGray.ToVector3();            
            //effect.FogStart = 5f;
            //effect.FogEnd = 100f;

            YSCALE = _yScale;

            CreateHeightMap(content);
            CreateGeometry(device);      
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
       
        public Vector3 retTerrainNormal(Vector3 P)
        {
            Vector3 UpLeft = NormalData[(int)P.X, (int)P.Z];
            Vector3 UpRight = NormalData[(int)P.X + 1, (int)P.Z];
            Vector3 BotLeft = NormalData[(int)P.X, (int)P.Z + 1];
            Vector3 BotRight = NormalData[(int)P.X + 1, (int)P.Z + 1];

            float dX = 1 - (P.X - (int)P.X);
            float dY = 1 - (P.Z - (int)P.Z);

            return UpLeft * dX * dY + UpRight * dY * (1 - dX) + BotLeft * dX * (1 - dY) + BotRight * (1 - dX) * (1 - dY);
        }

        private void CreateGeometry(GraphicsDevice device)
        {      
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[Width * Height];
            NormalData = new Vector3[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Height; z++)
                {
                    vertices[x + z * Width] = new VertexPositionNormalTexture(new Vector3(x, (float)HeightData[x, z], z)
                        , Vector3.Zero,
                        new Vector2((float)x /30, (float)z / 30));
                }                
            }

            short[] index = new short[Width * 2 * (Height - 1)];
            int count = 0;
            for (int y = 0; y < Height - 1; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //TriangleStrip
                    int top = x + y * Width;
                    int bottom = x + (y + 1) * Width;

                    index[count] = (short)bottom;
                    count++;
                    index[count] = (short)top;
                    count++;
                }
            }
            
            createNormals(vertices);

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
            indexBuffer = new IndexBuffer(device, typeof(short), index.Length, BufferUsage.None);
            indexBuffer.SetData<short>(index);            
        }

        public void createNormals(VertexPositionNormalTexture[] vertices)
        {
            // Primeira Linha
            for (int x = 1; x < Width - 1; x++)
            {
                int y = 0;
                int id5 = x + 0 + (y + 0) * Width;

                int id4 = x - 1 + (y + 0) * Width;
                int id6 = x + 1 + (y + 0) * Width;
                int id7 = x - 1 + (y + 1) * Width;
                int id8 = x + 0 + (y + 1) * Width;
                int id9 = x + 1 + (y + 1) * Width;

                Vector3 Length1 = vertices[id5].Position - vertices[id6].Position;
                Vector3 Length2 = vertices[id5].Position - vertices[id9].Position;
                Vector3 Length3 = vertices[id5].Position - vertices[id8].Position;
                Vector3 Length4 = vertices[id5].Position - vertices[id7].Position;
                Vector3 Length5 = vertices[id5].Position - vertices[id4].Position;

                Vector3 normal3 = Vector3.Cross(Length2, Length1);
                Vector3 normal4 = Vector3.Cross(Length3, Length2);
                Vector3 normal5 = Vector3.Cross(Length4, Length3);
                Vector3 normal6 = Vector3.Cross(Length5, Length4);


                Vector3 media = (normal3 + normal4 + normal5 + normal6) / 4;
                media.Normalize();
                vertices[id5].Normal = media;
                NormalData[x, y] = media;

            }
            // Ultima Linha
            for (int x = 1; x < Width - 1; x++)
            {
                int y = Height - 1;

                int id5 = x + 0 + (y + 0) * Width;

                int id1 = x - 1 + (y - 1) * Width;
                int id2 = x + 0 + (y - 1) * Width;
                int id3 = x + 1 + (y - 1) * Width;
                int id4 = x - 1 + (y + 0) * Width;
                int id6 = x + 1 + (y + 0) * Width;

                Vector3 Length1 = vertices[id5].Position - vertices[id1].Position;
                Vector3 Length2 = vertices[id5].Position - vertices[id2].Position;
                Vector3 Length3 = vertices[id5].Position - vertices[id3].Position;
                Vector3 Length4 = vertices[id5].Position - vertices[id4].Position;
                Vector3 Length5 = vertices[id5].Position - vertices[id6].Position;

                Vector3 normal3 = Vector3.Cross(Length2, Length1);
                Vector3 normal4 = Vector3.Cross(Length3, Length2);
                Vector3 normal5 = Vector3.Cross(Length4, Length3);
                Vector3 normal6 = Vector3.Cross(Length5, Length4);

                Vector3 media = (normal3 + normal4 + normal5 + normal6) / 4;
                media.Normalize();
                vertices[id5].Normal = media;
                NormalData[x, y] = media;

            }

            //Priemira Coluna
            for (int y = 1; y < Height - 1; y++)
            {
                int x = 0;

                int id5 = x + 0 + (y + 0) * Width;

                int id2 = x + 0 + (y - 1) * Width;
                int id3 = x + 1 + (y - 1) * Width;
                int id6 = x + 1 + (y + 0) * Width;
                int id8 = x + 0 + (y + 1) * Width;
                int id9 = x + 1 + (y + 1) * Width;

                Vector3 Length1 = vertices[id5].Position - vertices[id2].Position;
                Vector3 Length2 = vertices[id5].Position - vertices[id3].Position;
                Vector3 Length3 = vertices[id5].Position - vertices[id6].Position;
                Vector3 Length4 = vertices[id5].Position - vertices[id9].Position;
                Vector3 Length5 = vertices[id5].Position - vertices[id8].Position;

                Vector3 normal3 = Vector3.Cross(Length2, Length1);
                Vector3 normal4 = Vector3.Cross(Length3, Length2);
                Vector3 normal5 = Vector3.Cross(Length4, Length3);
                Vector3 normal6 = Vector3.Cross(Length5, Length4);


                Vector3 media = (normal3 + normal4 + normal5 + normal6) / 4;
                media.Normalize();
                vertices[id5].Normal = media;
                NormalData[x, y] = media;

            }

            //Ultima coluna
            for (int y = 1; y < Height - 1; y++)
            {
                int x = Width - 1;

                int id5 = x + 0 + (y + 0) * Width;

                int id1 = x - 1 + (y - 1) * Width;
                int id2 = x + 0 + (y - 1) * Width;
                int id4 = x - 1 + (y + 0) * Width;
                int id7 = x - 1 + (y + 1) * Width;
                int id8 = x + 0 + (y + 1) * Width;

                Vector3 Length1 = vertices[id5].Position - vertices[id8].Position;
                Vector3 Length2 = vertices[id5].Position - vertices[id7].Position;
                Vector3 Length3 = vertices[id5].Position - vertices[id4].Position;
                Vector3 Length4 = vertices[id5].Position - vertices[id1].Position;
                Vector3 Length5 = vertices[id5].Position - vertices[id2].Position;

                Vector3 normal3 = Vector3.Cross(Length2, Length1);
                Vector3 normal4 = Vector3.Cross(Length3, Length2);
                Vector3 normal5 = Vector3.Cross(Length4, Length3);
                Vector3 normal6 = Vector3.Cross(Length5, Length4);


                Vector3 media = (normal3 + normal4 + normal5 + normal6) / 4;
                media.Normalize();
                vertices[id5].Normal = media;
                NormalData[x, y] = media;

            }

            //Meio
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    int id5 = x + 0 + (y + 0) * Width;

                    int id1 = x - 1 + (y - 1) * Width;
                    int id2 = x + 0 + (y - 1) * Width;
                    int id3 = x + 1 + (y - 1) * Width;
                    int id4 = x - 1 + (y + 0) * Width;
                    int id6 = x + 1 + (y + 0) * Width;
                    int id7 = x - 1 + (y + 1) * Width;
                    int id8 = x + 0 + (y + 1) * Width;
                    int id9 = x + 1 + (y + 1) * Width;

                    Vector3 Length1 = vertices[id5].Position - vertices[id1].Position;
                    Vector3 Length2 = vertices[id5].Position - vertices[id2].Position;
                    Vector3 Length3 = vertices[id5].Position - vertices[id3].Position;
                    Vector3 Length4 = vertices[id5].Position - vertices[id4].Position;
                    Vector3 Length5 = vertices[id5].Position - vertices[id6].Position;
                    Vector3 Length6 = vertices[id5].Position - vertices[id7].Position;
                    Vector3 Length7 = vertices[id5].Position - vertices[id8].Position;
                    Vector3 Length8 = vertices[id5].Position - vertices[id9].Position;

                    Vector3 normal1 = Vector3.Cross(Length2, Length1);
                    Vector3 normal2 = Vector3.Cross(Length3, Length2);
                    Vector3 normal3 = Vector3.Cross(Length5, Length4);
                    Vector3 normal4 = Vector3.Cross(Length8, Length5);
                    Vector3 normal5 = Vector3.Cross(Length7, Length8);
                    Vector3 normal6 = Vector3.Cross(Length6, Length7);
                    Vector3 normal7 = Vector3.Cross(Length4, Length6);
                    Vector3 normal8 = Vector3.Cross(Length1, Length4);

                    Vector3 media = (normal1 + normal2 + normal3 + normal4 + normal5 + normal6 + normal7 + normal8) / 8;
                    media.Normalize();
                    vertices[id5].Normal = media;
                    NormalData[x, y] = media;
                }
            }

            int Nid1 = 0;
            int Nid2 = 0;
            int Nid3 = 0;
            int Nid4 = 0;
            int Nid6 = 0;
            int Nid7 = 0;
            int Nid8 = 0;
            int Nid9 = 0;

            //Canto 0,0
            int x1 = 0, y1 = 0;
            int Nid5 = x1 + 0 + (y1 + 0) * Width;

            Nid6 = x1 + 1 + (y1 + 0) * Width;
            Nid8 = x1 + 0 + (y1 + 1) * Width;
            Nid9 = x1 + 1 + (y1 + 1) * Width;

            Vector3 NLength1 = vertices[Nid5].Position - vertices[Nid6].Position;
            Vector3 NLength2 = vertices[Nid5].Position - vertices[Nid8].Position;
            Vector3 NLength3 = vertices[Nid5].Position - vertices[Nid9].Position;

            Vector3 Nnormal1 = Vector3.Cross(NLength3, NLength1);
            Vector3 Nnormal2 = Vector3.Cross(NLength2, NLength3);

            Vector3 Nmedia = (Nnormal1 + Nnormal2) / 2;
            Nmedia.Normalize();
            vertices[Nid5].Normal = Nmedia;
            NormalData[x1, y1] = Nmedia;

            //Canto Width,0;
            x1 = Width - 1;
            y1 = 0;
            Nid5 = x1 + 0 + (y1 + 0) * Width;

            Nid4 = x1 - 1 + (y1 + 0) * Width;
            Nid7 = x1 + 0 + (y1 + 1) * Width;
            Nid8 = x1 + 1 + (y1 + 1) * Width;

            NLength1 = vertices[Nid5].Position - vertices[Nid4].Position;
            NLength2 = vertices[Nid5].Position - vertices[Nid7].Position;
            NLength3 = vertices[Nid5].Position - vertices[Nid8].Position;

            Nnormal1 = Vector3.Cross(NLength2, NLength1);
            Nnormal2 = Vector3.Cross(NLength3, NLength2);

            Nmedia = (Nnormal1 + Nnormal2) / 2;
            Nmedia.Normalize();
            vertices[Nid5].Normal = Nmedia;
            NormalData[x1, y1] = Nmedia;

            //Canto 0, Height
            x1 = 0;
            y1 = Height - 1;
            Nid5 = x1 + 0 + (y1 + 0) * Width;

            Nid2 = x1 + 0 + (y1 - 1) * Width;
            Nid3 = x1 + 1 + (y1 - 1) * Width;
            Nid6 = x1 + 1 + (y1 + 0) * Width;

            NLength1 = vertices[Nid5].Position - vertices[Nid2].Position;
            NLength2 = vertices[Nid5].Position - vertices[Nid3].Position;
            NLength3 = vertices[Nid5].Position - vertices[Nid6].Position;

            Nnormal1 = Vector3.Cross(NLength2, NLength1);
            Nnormal2 = Vector3.Cross(NLength3, NLength2);

            Nmedia = (Nnormal1 + Nnormal2) / 2;
            Nmedia.Normalize();
            vertices[Nid5].Normal = Nmedia;
            NormalData[x1, y1] = Nmedia;

            //Canto width,Height
            x1 = Width - 1;
            y1 = Height - 1;
            Nid5 = x1 + 0 + (y1 + 0) * Width;

            Nid2 = x1 + 0 + (y1 - 1) * Width;
            Nid1 = x1 - 1 + (y1 - 1) * Width;
            Nid4 = x1 - 1 + (y1 + 0) * Width;

            NLength1 = vertices[Nid5].Position - vertices[Nid2].Position;
            NLength2 = vertices[Nid5].Position - vertices[Nid1].Position;
            NLength3 = vertices[Nid5].Position - vertices[Nid4].Position;

            Nnormal1 = Vector3.Cross(NLength2, NLength1);
            Nnormal2 = Vector3.Cross(NLength3, NLength2);

            Nmedia = (Nnormal1 + Nnormal2) / 2;
            Nmedia.Normalize();
            vertices[Nid5].Normal = Nmedia;
            NormalData[x1, y1] = Nmedia;
        }

        public void Draw(GraphicsDevice device)
        { 
            effect.World = worldMatrix;
            effect.View = Game1.MainCamera.viewMatrix;
            effect.Projection = Game1.MainCamera.projectionMatrix;
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            for (int y = 0; y < Height - 1; y++)
            {
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, y * 2 * Width, (Width - 1) * 2);
            }           
        }
    }
}