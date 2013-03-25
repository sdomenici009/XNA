using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Foundation
{
    static class Player
    {
        #region Declarations
        public static Sprite playerSprite;
        private static Vector2 playerAngle = Vector2.Zero;
        private static float playerSpeed = 250f;
        private static Rectangle playerHitBoxRec;
        #endregion

        #region Initialization
        public static void Initialize(
            Texture2D texture,
            Rectangle playerInitialFrame,
            Rectangle playerHitBox,
            int playerFrameCount,
            Vector2 worldLocation,
            string status,
            string modifier,
            string faction,
            string karma,
            string personality)
        {
            int frameWidth  = playerInitialFrame.Width;
            int frameHeight = playerInitialFrame.Height;
            playerHitBoxRec = playerHitBox;

            playerSprite = new Sprite(
                worldLocation,
                texture,
                playerInitialFrame,
                Vector2.Zero,
                Vector2.Zero,
                1);

            playerSprite.boundingXPadding = 0;
            playerSprite.boundingYPadding = 0;
            playerSprite.animateWhenStopped = false;
            playerSprite.animate = false;

            for (int x = 1; x < playerFrameCount; x++)
            {
                playerSprite.AddFrame(
                    new Rectangle(
                    playerInitialFrame.X + (frameWidth * x),
                    playerInitialFrame.Y,
                    frameWidth,
                    frameHeight));
            }
        }
        #endregion


        #region Update and Draw
        public static void Update(GameTime gameTime)
        {
            handleInput(gameTime);
            playerSprite.Update(gameTime);
            clampToWorld();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
        }
        #endregion

        #region Input Handling
        private static Vector2 handleKeyboardmovement(KeyboardState keystate)
        {
            Vector2 keyMovement = Vector2.Zero;

            if (keystate.IsKeyDown(Keys.W))
            {
                playerSprite.Frame = 1;
                keyMovement.Y--;
            }

            if (keystate.IsKeyDown(Keys.D))
            {
                playerSprite.Frame = 3;
                keyMovement.X++;
            }

            if (keystate.IsKeyDown(Keys.S))
            {
                playerSprite.Frame = 2;
                keyMovement.Y++;
            }

            if (keystate.IsKeyDown(Keys.A))
            {
                playerSprite.Frame = 0;
                keyMovement.X--;
            }

            return keyMovement;
        }

        private static void handleInput(GameTime gametime)
        {
            float elapsed = (float)gametime.ElapsedGameTime.TotalSeconds;

            Vector2 moveAngle = Vector2.Zero;
            Vector2 weaponAngle = Vector2.Zero;

            moveAngle += handleKeyboardmovement(Keyboard.GetState());
            //weaponAngle += handleKeyboardShots(Mouse.GetState());

            if (moveAngle != Vector2.Zero)
            {
                moveAngle.Normalize();
                playerAngle = moveAngle;
                moveAngle = checkTileObstacles(elapsed, moveAngle);
            }

            /*
            if (weaponAngle != Vector2.Zero)
            {
                weaponAngle.Normalize();

                if (Weapon.CanSlashWeapon)
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {

                        Weapon.SwordBool = true;
                        checkWeaponObstacles(Weapon.WeaponSprite.WorldRectangle);
                        Weapon.slashTimer = 0.0f;
                    }
                }
            }
             

            Weapon.WeaponSprite.WorldLocation = calculateLocation(weaponAngle);

            Weapon.WeaponSprite.RotateTo(weaponAngle);
            */

            playerSprite.Velocity = moveAngle * playerSpeed;
            //Weapon.WeaponSprite.Velocity = playerSprite.Velocity;

            repositionCamera(gametime, moveAngle);
        }
        #endregion


        #region Movement Limitations
        private static void clampToWorld()
        {
            float currentX = playerSprite.WorldLocation.X;
            float currentY = playerSprite.WorldLocation.Y;

            currentX = MathHelper.Clamp(
                currentX,
                0,
                Camera.WorldRectangle.Right - playerSprite.FrameWidth);

            currentY = MathHelper.Clamp(
                currentY,
                0,
                Camera.WorldRectangle.Bottom - playerSprite.FrameHeight);

            playerSprite.WorldLocation = new Vector2(currentX, currentY);
        }

        private static Rectangle scrollArea =
            new Rectangle(Camera.ViewPortWidth / 2, Camera.ViewPortHeight / 2, 1, 1);

        private static void repositionCamera(
            GameTime gameTime,
            Vector2 moveAngle)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float moveScale = playerSpeed * elapsed;

            if ((playerSprite.ScreenRectangle.X < scrollArea.X) &&
                (moveAngle.X < 0))
            {
                Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
            }

            if ((playerSprite.ScreenRectangle.Right > scrollArea.Right) &&
                (moveAngle.X > 0))
            {
                Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
            }

            if ((playerSprite.ScreenRectangle.Y < scrollArea.Y) &&
                (moveAngle.Y < 0))
            {
                Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
            }

            if ((playerSprite.ScreenRectangle.Bottom > scrollArea.Bottom) &&
                (moveAngle.Y > 0))
            {
                Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
            }
        }

        private static Vector2 checkTileObstacles(
            float elapsedTime,
            Vector2 moveAngle)
        {
            Vector2 newHorizontalLocation = playerSprite.WorldLocation +
                (new Vector2(moveAngle.X, 0) * (playerSpeed *
                elapsedTime));

            Vector2 newVerticalLocation = playerSprite.WorldLocation +
                (new Vector2(0, moveAngle.Y) * (playerSpeed *
                elapsedTime));

            Rectangle newHorizontalRect = new Rectangle(
                (int)newHorizontalLocation.X,
                (int)playerSprite.WorldLocation.Y + 16,
                playerHitBoxRec.Width - 4,
                playerHitBoxRec.Height - 4);

            Rectangle newVerticalRect = new Rectangle(
                (int)playerSprite.WorldLocation.X,
                (int)newVerticalLocation.Y + 16,
                playerHitBoxRec.Width - 4,
                playerHitBoxRec.Height - 4);

            if (moveAngle.X != 0)
            {
                for (int i = 0; i < Tilemap.collisionRectangles.Count; i++)
                {
                    if (newHorizontalRect.Intersects(Tilemap.collisionRectangles[i]))
                    {
                        moveAngle.X = 0;
                        break;
                    }
                }
            }

            if (moveAngle.Y != 0)
            {
                for (int i = 0; i < Tilemap.collisionRectangles.Count; i++)
                {
                    if (newVerticalRect.Intersects(Tilemap.collisionRectangles[i]))
                    {
                        moveAngle.Y = 0;
                        break;
                    }
                }
            }
            return moveAngle;
        }
        #endregion
    }
}
