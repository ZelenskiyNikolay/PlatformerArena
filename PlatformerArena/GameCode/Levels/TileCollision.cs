using Core;
using Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Levels
{
    public class TileCollision
    {
        private Dictionary<Point, int> _collisions;

        private const int num_tiles_per_row = 16;
        private const int pixel_tileAtlas = 16;
        private const int TILESIZE = 50;

        public TileCollision(Dictionary<Point, int> collisionsLayer)
        {
            _collisions = new Dictionary<Point, int>();
            _collisions = collisionsLayer;
           
        }
        
        public void UpdateCollision(Player player)
        {
            player.Collider.ColliderRectangle = 
                UpdateCollision(player.Collider.ColliderRectangle, ref player.Velocity, ref player.IsOnGround);
        }
        public void UpdateCollision(Enemy enemy)
        {
            enemy.Rect = UpdateCollision(enemy.Rect, ref enemy.Velocity, ref enemy.OnGround);
        }

        public void UpdateCollision(Slime slime)
        {
            slime.Rect = UpdateCollision(slime.Rect, ref slime.Velocity, ref slime.OnGround);
        }
        public void UpdateCollision(Knight knight)
        {
            knight.Rect = UpdateCollision(knight.Rect, ref knight.Velocity, ref knight.OnGround);
        }
        public void UpdateCollision(Boss1 boss)
        {
            boss.Rect = UpdateCollision(boss.Rect, ref boss.Velocity, ref boss.OnGround);
        }

        /// <summary>
        /// Метод обработки коллиизиий
        /// </summary>
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
        /// <summary>
        /// Метод выдает список окружающих прямоугольник тайлов
        /// </summary>
        /// <param name="rect">Прямоугольник объекта</param>
        /// <returns></returns>
        private List<Point> GetIntersectingTiles(Rectangle rect)
        {
            List<Point> tiles = new();

            int leftTile = (rect.Left) / TILESIZE;
            int rightTile = (rect.Right - 1) / TILESIZE;
            int topTile = (rect.Top) / TILESIZE;
            int bottomTile = (rect.Bottom - 1) / TILESIZE;

            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    tiles.Add(new Point(x, y));
                }
            }

            return tiles;
        }

        /// <summary>
        /// Проверка есть ли пол впереди
        /// </summary>
        /// <param name="objRect">Прямоугольник объекта</param>
        /// <param name="direction">Вектор движения</param>
        /// <returns></returns>
        public bool HasGroundBelow(Rectangle objRect, Vector2 direction)
        {
            int checkX = direction.X > 0 ? objRect.Right + 1 : direction.X < 0 ? objRect.Left - 1 : objRect.Center.X;
            int checkY = objRect.Bottom + 1;

            int tileX = checkX / TILESIZE;
            int tileY = checkY / TILESIZE;

            return _collisions.ContainsKey(new Point(tileX, tileY));
        }

        /// <summary>
        /// Проверяет есть ли впереди стена
        /// </summary>
        /// <param name="objRect">Прямоугольник объекта</param>
        /// <param name="direction">Вектор движения</param>
        /// <returns></returns>
        public bool HasWallAhead(Rectangle objRect, Vector2 direction)
        {
            int checkX = direction.X > 0 ? objRect.Right + 1 : objRect.Left - 1;
            int checkY1 = objRect.Top + 10;
            int checkY2 = objRect.Bottom - 10;

            int tileX = checkX / TILESIZE;
            int tileY1 = checkY1 / TILESIZE;
            int tileY2 = checkY2 / TILESIZE;

            return _collisions.ContainsKey(new Point(tileX, tileY1)) ||
                   _collisions.ContainsKey(new Point(tileX, tileY2));
        }
    }
}

