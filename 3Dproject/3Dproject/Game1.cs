using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace _3Dproject
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public static Camera MainCamera;
        public static Terrain terrain;
        public static List<Tank> TankList;

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
            TankList.Add(new PlayerTank(GraphicsDevice, Content, new Vector3(10, 0, 10), 0,new[] { Keys.A, Keys.D, Keys.W, Keys.S, Keys.Space }));
            TankList.Add(new PlayerTank(GraphicsDevice, Content, new Vector3(20, 0, 10), 1,new[] { Keys.J, Keys.L, Keys.I, Keys.K, Keys.Enter }));            

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

                MainCamera.Update(mousePos, half, mouseState.ScrollWheelValue, new[] {TankList[0].returnPosition(), TankList[1].returnPosition() });

                TankList[0].UpdateBullets();
                foreach (PlayerTank item in TankList)                
                    item.Update(gameTime);

                Mouse.SetPosition((int)half.X, (int)half.Y);
                base.Update(gameTime);
            }                             
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            terrain.Draw(GraphicsDevice);
            foreach (PlayerTank item in TankList)
                item.Draw(GraphicsDevice, gameTime);

            base.Draw(gameTime);
        }
    }
}
