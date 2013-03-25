using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Foundation
{
    public class Foundation : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState keyState;

        #region Textures
        Texture2D tileSheet;
        Texture2D playerSprite;
        #endregion

        public Vector2 clientBounds;


        public Foundation()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1080;
            //this.graphics.IsFullScreen = true;
            this.graphics.ApplyChanges();

            clientBounds = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Camera.WorldRectangle = new Rectangle(0, 0,
                Tilemap.mapWidth * Tilemap.tileSize, Tilemap.mapHeight * Tilemap.tileSize);
            Camera.ViewPortWidth = 1920;
            Camera.ViewPortHeight = 1080;

            tileSheet = Content.Load<Texture2D>(@"Artwork\Sprites\tileSheet");
            playerSprite = Content.Load<Texture2D>(@"Artwork\Sprites\playerSprite");

            Player.Initialize(
                playerSprite,
                new Rectangle(0, 0, 32, 48),
                new Rectangle(0, 0, 32, 32),
                4,
                new Vector2(100, 100),
                "n/a",
                "n/a",
                "n/a",
                "n/a",
                "n/a");

            Tilemap.Initialize(tileSheet);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {

            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.M))
            {
                this.Exit();
            }

            Vector2 cameraMove = Vector2.Zero;
            Camera.Move(cameraMove);
            Player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            Tilemap.Draw(spriteBatch);
            Player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
