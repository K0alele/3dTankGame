using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _3Dproject
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public static Camera MainCamera;
        public static Terrain terrain;
        public static List<Tank> TankList;
        public static List<Tank> visible;

        BoundingFrustum frustum;
        
        Vector2 half;        

        private bool canPress = true;
        public bool Paused = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";            
        }

        protected override void Initialize()
        {
            MainCamera = new Camera(GraphicsDevice);    
                    
            half = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);            

            terrain = new Terrain(GraphicsDevice, Content, 16f);
            TankList = new List<Tank>();
            TankList.Add(CreateTanks(false));
            TankList.Add(CreateTanks(true));             

            visible = new List<Tank>();

            this.IsMouseVisible = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        private Tank CreateTanks(bool bot)
        {
            if (bot)
                return new BotTank(GraphicsDevice, Content, new Vector3(400, 0, 400), 1, new[] { Keys.J, Keys.L, Keys.I, Keys.K, Keys.Enter });
            else
                return new PlayerTank(GraphicsDevice, Content, new Vector3(10, 0, 10), 0, new[] { Keys.A, Keys.D, Keys.W, Keys.S, Keys.Space });                                    
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();            

            if (Keyboard.GetState().IsKeyDown(Keys.P) && canPress)
            {
                Paused = !Paused;
                canPress = false;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.P))            
                canPress = true;
            if (!Paused)
            {
                MouseState mouseState = Mouse.GetState();
                Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);                               

                MainCamera.Update(mousePos, half, mouseState.ScrollWheelValue);

                if (TankList.ElementAtOrDefault(0) != null)
                    TankList[0].UpdateBullets();
                else if (TankList.ElementAtOrDefault(1) != null)                
                    TankList[0].UpdateBullets();

                for (int i = 0; i < TankList.Count; i++)
                {
                    TankList[i].Update(gameTime);
                    TankList[i].UpdateParticles(gameTime);
                }

                frustum = new BoundingFrustum(MainCamera.viewMatrix * MainCamera.projectionMatrix);
                visible = TankList.Where(m => frustum.Contains(m.Sphere) != ContainmentType.Disjoint).ToList();

                Mouse.SetPosition((int)half.X, (int)half.Y);
                base.Update(gameTime);
            }                                         
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);          

            terrain.Draw(GraphicsDevice);

            if (TankList.ElementAtOrDefault(0) != null)            
                TankList[0].DrawBullets(GraphicsDevice, frustum);
            else if (TankList.ElementAtOrDefault(1) != null)            
                TankList[1].DrawBullets(GraphicsDevice, frustum);            
                              
            foreach (var item in visible)
                item.Draw(GraphicsDevice);

            base.Draw(gameTime);              
        }
    }
}
