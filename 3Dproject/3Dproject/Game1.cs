using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace _3Dproject
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public static Camera MainCamera;
        MouseState LastMouseState = new MouseState();

        Vector2 half;       

        public static Terrain t;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            MainCamera = new Camera(GraphicsDevice);            
            half = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);            

            t = new Terrain(GraphicsDevice, Content, 10f);

            this.IsMouseVisible = true;
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

            MouseState mouseState = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

            MainCamera.Update(mousePos, half, mouseState.ScrollWheelValue);
            t.Update();

            //Debug.WriteLine("" + mousePos.X + "," + mousePos.Y);
            //LastMouseState = mouseState;
            Mouse.SetPosition((int)half.X, (int)half.Y);
            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            t.Draw(GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
