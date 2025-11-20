using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Levels
{
    public class TileMap
    {
        public Texture2D TextureAtlas;
        private List<Dictionary<Point, int>> _tileLayers;
        private Dictionary<Point, int> _collisions;
        private int tileSize = 50;
        private int num_tiles_per_row = 16;
        private int pixel_tileAtlas = 16;
        public int TILESIZE = 50;

        public Dictionary<Point, int> Collisions { get { return _collisions; } }
        public Dictionary<Point, int> SpaunLayer { get { return _tileLayers[1]; } }
        public TileMap()
        {
            _tileLayers = new List<Dictionary<Point, int>>();
            _collisions = new Dictionary<Point, int>();
        }

        
        public float ReturnSizeMapX() { return (_tileLayers[0].Keys.Max(v => v.X) + 1) * TILESIZE; }
        public float ReturnSizeMapY() { return (_tileLayers[0].Keys.Max(v => v.Y) + 1) * TILESIZE; }
        public void AddTileLayer(string FileName)
        {
            _tileLayers.Add(LoadMap(FileName));
        }
        public void AddCollisionLayer(string FileName)
        {
            _collisions = LoadMap(FileName);
        }
        private Dictionary<Point, int> LoadMap(string filename)
        {
            Dictionary<Point, int> result = new();

            StreamReader reader = new(filename);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1)
                        {
                            result[new Point(x, y)] = value;
                        }
                    }
                }
                y++;
            }
            return result;
        }
        public void Draw(SpriteBatch spriteBatch,int numLayer = 0)
        {
            if (_tileLayers.Count == 0 || numLayer >= _tileLayers.Count)
                return; // ������ ��������
            Rectangle dest, src;
            foreach (var tile in _tileLayers[numLayer])
            {
                dest = new(
                    tile.Key.X * tileSize,
                    tile.Key.Y * tileSize,
                    tileSize, tileSize
                    );

                int x = tile.Value % num_tiles_per_row;
                int y = tile.Value / num_tiles_per_row;

                src = new(
                   x * pixel_tileAtlas,
                   y * pixel_tileAtlas,
                   pixel_tileAtlas,
                   pixel_tileAtlas);

                spriteBatch.Draw(TextureAtlas, dest, src, Color.White);
            }
        }
        public Rectangle UpdateCollision(Rectangle objRect, ref Vector2 velocity, ref bool OnGround)
        {
            // Двигаем по Х
            objRect.X += (int)velocity.X;

            foreach (var tile in GetIntersectingTiles(objRect))
            {
                if (_collisions.ContainsKey(tile))
                {
                    Rectangle tileRect = new(tile.X * TILESIZE, tile.Y * TILESIZE, TILESIZE, TILESIZE);

                    if (velocity.X > 0)
                        objRect.X = tileRect.Left - objRect.Width;
                    else if (velocity.X < 0)
                        objRect.X = tileRect.Right;
                }
            }

            // Коллизия по У
            objRect.Y += (int)velocity.Y;

            OnGround = false;// обнуляем касание земли
            foreach (var tile in GetIntersectingTiles(objRect))
            {
                if (_collisions.ContainsKey(tile))
                {
                    Rectangle tileRect = new(tile.X * TILESIZE, tile.Y * TILESIZE, TILESIZE, TILESIZE);

                    if (velocity.Y > 0)
                    {
                        objRect.Y = tileRect.Top - objRect.Height;
                        velocity.Y = 0;
                        OnGround = true;
                    }
                    else if (velocity.Y < 0)
                    {
                        objRect.Y = tileRect.Bottom;
                        velocity.Y = 0;
                    }
                }
            }

            return objRect;
        }

        private List<Point> GetIntersectingTiles(Rectangle rect)
        {
            List<Point> tiles = new();

            int leftTile = (rect.Left) / TILESIZE;
            int rightTile = (rect.Right-1) / TILESIZE;
            int topTile = (rect.Top) / TILESIZE;
            int bottomTile = (rect.Bottom -1)/ TILESIZE;

            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    tiles.Add(new Point(x, y));
                }
            }

            return tiles;
        }

        // ���������, ���� �� ������ ���� ��� ��������� ������
        public bool HasGroundBelow(Rectangle objRect, Vector2 direction)
        {
            // ����� ��� "������" �����
            int checkX = direction.X > 0 ? objRect.Right + 1 : direction.X < 0 ? objRect.Left - 1 : objRect.Center.X;
            int checkY = objRect.Bottom + 1; // ���� ���� ���

            int tileX = checkX / TILESIZE;
            int tileY = checkY / TILESIZE;

            return _collisions.ContainsKey(new Point(tileX, tileY));
        }

        // ���������, ���� �� ����� ����� rect
        public bool HasWallAhead(Rectangle objRect, Vector2 direction)
        {
            int checkX = direction.X > 0 ? objRect.Right + 1 : objRect.Left - 1;
            int checkY1 = objRect.Top + 10;         // ������� �����
            int checkY2 = objRect.Bottom - 10;      // ������ �����

            int tileX = checkX / TILESIZE;
            int tileY1 = checkY1 / TILESIZE;
            int tileY2 = checkY2 / TILESIZE;

            return _collisions.ContainsKey(new Point(tileX, tileY1)) ||
                   _collisions.ContainsKey(new Point(tileX, tileY2));
        }

    }
}




