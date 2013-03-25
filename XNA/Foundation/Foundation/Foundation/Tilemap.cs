using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Foundation
{
    public static class Tilemap
    {
        // t - terrain
        // s - structures

        #region Declarations
        public const int tileSize = 32;
        public const int tileWidth = 32;
        public const int tileHeight = 32;
        public const int mapWidth = 50;
        public const int mapHeight = 50;
        public const int tGrass = 0;
        public const int sWall = 99;
        public const int wallPercent = 10;

        static private Texture2D texture;

        static private List<Rectangle> tiles = new List<Rectangle>();

        static public List<Rectangle> collisionRectangles = new List<Rectangle>();

        static private int[,] mapSquares = new int[mapWidth, mapHeight];

        static private Random rand = new Random();
        #endregion

        #region Public Methods
        static public int toTileLoc(int tileNum)
        {
            //translates a tile number into the correct pixel location
            return (tileNum * tileSize);
        }
        #endregion

        #region Initilization
        static public void Initialize(Texture2D tileTexture)
        {
            texture = tileTexture;
            tiles.Clear();
            LoadTileSet(texture);

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (x == 0 || y == 0 ||
                        y == mapHeight - 1 || x == mapWidth - 1)
                    {
                        mapSquares[x, y] = sWall;
                        collisionRectangles.Add(new Rectangle(toTileLoc(x), toTileLoc(y), 32, 32));
                    }
                    else
                    {
                        mapSquares[x, y] = tGrass;
                    }
                }
            }

            for (int x = 3; x < mapWidth; x++)
            {
                for (int y = 3; y < mapHeight; y++)
                {
                    if (rand.Next(0, 100) <= wallPercent)
                    {
                        mapSquares[x, y] = sWall;
                        collisionRectangles.Add(new Rectangle(toTileLoc(x), toTileLoc(y), 32, 32));
                    }
                }
            }


        }
        #endregion

        #region TileSheet
        static public void LoadTileSet(Texture2D tileSheet)
        {
            Rectangle bounds;

            int noOfTilesX = (int)tileSheet.Width / tileWidth;
            int noOfTilesY = (int)tileSheet.Height / tileHeight;

            for (int j = 0; j < noOfTilesY; ++j)
            {
                for (int i = 0; i < noOfTilesX; ++i)
                {
                    bounds = new Rectangle(i * tileWidth, j * tileHeight, tileWidth, tileHeight);
                    tiles.Add(bounds);
                }
            }
        }
        #endregion

        #region Map Squares
        static public Rectangle SquareWorldRectangle(int x, int y)
        {
            return new Rectangle(
                toTileLoc(x),
                toTileLoc(y),
                tileWidth,
                tileHeight);
        }

        static public Rectangle SquareScreenRectangle(int x, int y)
        {
            return Camera.Transform(SquareWorldRectangle(x, y));
        }
        #endregion

        #region Draw
        static public void Draw(SpriteBatch spriteBatch)
        {
            int startX = (int)Camera.Position.X / tileWidth;
            int endX = ((int)Camera.Position.X +
                Camera.ViewPortWidth / tileWidth);

            int startY = (int)Camera.Position.Y / tileHeight;
            int endY = ((int)Camera.Position.Y +
                Camera.ViewPortHeight) / tileHeight;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (y >= 0) &&
                        (x < mapWidth) && (y < mapHeight))
                    {
                        spriteBatch.Draw(
                            texture,
                            SquareScreenRectangle(x, y),
                            tiles[mapSquares[x, y]],
                            Color.White);
                    }
                }
            }
        }
        #endregion


    }
}
