using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Levels
{
    internal class TileCollisionNew
    {
        private Dictionary<Point, int> _collisions;
        private CollisionValue[,] _collisionValues;

        private const int num_tiles_per_row = 16;
        private const int pixel_tileAtlas = 16;
        private const int TILESIZE = 50;

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * TILESIZE, y * TILESIZE, TILESIZE, TILESIZE);
        }
        public TileCollisionNew(Dictionary<Point, int> collisionsLayer)
        {
            _collisions = new Dictionary<Point, int>();
            _collisions = collisionsLayer;
            SetValuesCollisino(collisionsLayer);

            PrintCollision();
        }
        public float ReturnSizeMapX() { return (_collisions.Keys.Max(v => v.X) + 1) * TILESIZE; }
        public float ReturnSizeMapY() { return (_collisions.Keys.Max(v => v.Y) + 1) * TILESIZE; }

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

        
        
        /// <summary>
        /// Метод обработки коллиизиий
        /// </summary>
        private void HandleCollisions(Rectangle BoundingRectangle, float previousBottom, Vector2 Position, bool isOnGround)
        {
            // Выбор тайлов вокруг объекта
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / TILESIZE);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / TILESIZE)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / TILESIZE);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / TILESIZE)) - 1;

            // Сброс флага косания земли
            isOnGround = false;

            // перебор по тайлам, с которыми пересечение возможно
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // Пропускаем тайлы, если онии явно без коллизии
                    CollisionValue collision = GetCollision(x, y);
                    if (collision != CollisionValue.Passable)
                    {
                        // Получение границ тайла и глубины пересеченя
                         Rectangle tileBounds = GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            //Сравнение глубины пересечениия по осям
                            if (absDepthY < absDepthX || collision == CollisionValue.Platform)
                            {
                                // устраняем пересечение по оси У 
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Для Impassable — твёрдого блока — всегда разрешаем Y-коллизию (сдвигаем по Y).
                                if (collision == CollisionValue.Impassable || isOnGround)
                                {
                                    // мы корректируем вертикально позицию игрока так, чтобы он больше не пересекал тайл.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // пересчитываем bounds, т.к. позиция изменилась;
                                    // это позволяет корректно продолжить проверки с обновлённой позицией
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == CollisionValue.Impassable) // Ignore platforms.
                            {
                                //Платформы не блокируют по X — только Impassable блоки.
                                //Корректируем по X и обновляем bounds.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // пересчитываем bounds, т.к. позиция изменилась;
                                // это позволяет корректно продолжить проверки с обновлённой позицией
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Сохраняем нижнюю границу текущего bounds (после всех разрешений),
            // чтобы в следующем кадре иметь корректное значение
            previousBottom = bounds.Bottom;
        }

        public CollisionValue GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= _collisionValues.GetLength(1))
                return CollisionValue.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= _collisionValues.GetLength(0))
                return CollisionValue.Passable;

            return _collisionValues[y, x];
        }

        public void SetValuesCollisino(Dictionary<Point, int> collisionsLayer)
        {
            // если словарь пустой — выходим
            if (_collisions == null || _collisions.Count == 0)
                return;

            // вычисляем размеры по максимальным координатам
            int width = _collisions.Keys.Max(p => p.X) + 1;
            int height = _collisions.Keys.Max(p => p.Y) + 1;

            _collisionValues = new CollisionValue[height,width];

            foreach (var colosion in _collisions)
            {
                Point p = colosion.Key;
                int value = colosion.Value;

                if (value < 0)
                {
                    _collisionValues[p.Y, p.X] = CollisionValue.Passable;
                }
                else if (value > 150)
                {
                    _collisionValues[p.Y,p.X] = CollisionValue.Impassable;
                }
                else if(value == 209)
                {
                    _collisionValues[p.Y, p.X] = CollisionValue.Platform;
                }
            }
        }
        private void PrintCollision()
        {
            System.Diagnostics.Debug.WriteLine("Вывод значений колизии : ");

            int i = 0;
            int width = _collisionValues.GetLength(1);
            System.Diagnostics.Debug.WriteLine("Высотта: " + width);
            foreach (var col in _collisionValues)
            {
                if (i == width)
                {
                    System.Diagnostics.Debug.WriteLine("");
                    i = 0;
                }
                if (col == CollisionValue.Passable)
                    System.Diagnostics.Debug.Write("- ");
                else if (col == CollisionValue.Impassable)
                    System.Diagnostics.Debug.Write("8 ");
                i++;
            }
            System.Diagnostics.Debug.WriteLine("");
        }
    }
    public enum CollisionValue
    {
        Passable = 0,

        Impassable = 1, 

        Platform = 2,
    }
}

