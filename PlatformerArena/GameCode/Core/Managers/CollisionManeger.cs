using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Levels;
using Entity;

namespace Core
{
    public class CollisionManeger
    {
        private static CollisionManeger _instance;
        public Texture2D ColliderTexture {  get; set; }

        public TileCollision TileCollision { get; private set; }
        private CollisionManeger() { }

        public void InitColision(Dictionary<Point, int> collisionsLayer)
        {
            TileCollision = new TileCollision(collisionsLayer);
        }
        public void UpdateCollision(Player player)
        {
            TileCollision.UpdateCollision(player);
        }
        public void UpdateCollision(Enemy Enemy)
        {
            TileCollision.UpdateCollision(Enemy);
        }
        public void UpdateCollision(Slime slime)
        {
            TileCollision.UpdateCollision(slime);
        }
        public void UpdateCollision(Knight knight)
        {
            TileCollision.UpdateCollision(knight);
        }
        public void UpdateCollision(Boss1 boss)
        {
            TileCollision.UpdateCollision(boss);
        }
        public void UpdateCollision(Rectangle player, Coin coin)
        {
            if (!coin.IsActive)
                return;
            Vector2 dept = player.GetIntersectionDepth(coin.Dest);
            if (dept == Vector2.Zero)
                return;
            else if (dept.X > player.Width / 3 || dept.Y > player.Height / 3)
            {
                coin.IsActive = false;
                EventManager.Instance.Trigger(new CoinColectEvent(10));
            }

        }
        public void UpdateCollision(Player player, Enemys enemy)
        {
            if(enemy is Enemy)
                UpdateCollision(player,(Enemy)enemy);
            else if(enemy is Slime)
                UpdateCollision(player,(Slime)enemy);
            else if(enemy is Knight)
                UpdateCollision(player,(Knight)enemy);
            else if(enemy is Boss1)
                UpdateCollision(player,(Boss1)enemy);
        }

        public void UpdateCollision(Player player, Boss1 enemy)
        {
            if (player.Collider.ColliderRectangle.Intersects(enemy.Rect))
            {
                bool hitFromAbove =
        player.Velocity.Y > 0 &&                          // Игрок падал
        player.Collider.ColliderRectangle.Bottom - player.Velocity.Y <= enemy.Rect.Top + 5;  // До столкновения был выше


                // Если игрок на земле — это заведомо удар сбоку
                if (player.IsOnGround)
                {
                    player.TakeDamage(enemy.Damage, new Vector2(enemy.Velocity.X * 50, enemy.Velocity.Y * 50));
                    return;
                }

                // Проверка сверху
                if (hitFromAbove)
                {
                    // Игрок упал на врага
                    enemy.TakeDamage(player.Damage, Vector2.Zero);
                    player.Velocity.Y = -30f;
                    if (player.Velocity.X < 0)
                        player.Velocity.X = -30;
                    else
                        player.Velocity.X = 30;
                    EventManager.Instance.Trigger(new ScoreColectEvent(20));//?
                }
                else
                {
                    // Удар сбоку или снизу
                    player.TakeDamage(enemy.Damage, enemy.Velocity);
                }
            }
        }
        public void UpdateCollision(Player player, Knight enemy)
        {
            if (player.Collider.ColliderRectangle.Intersects(enemy.Rect))
            {
                bool hitFromAbove =
        player.Velocity.Y > 0 &&                          // Игрок падал
        player.Collider.ColliderRectangle.Bottom - player.Velocity.Y <= enemy.Rect.Top + 5;  // До столкновения был выше


                // Если игрок на земле — это заведомо удар сбоку
                if (player.IsOnGround)
                {
                    player.TakeDamage(enemy.Damage, new Vector2(enemy.Velocity.X * 50, enemy.Velocity.Y * 50));
                    return;
                }

                // Проверка сверху
                if (hitFromAbove)
                {
                    // Игрок упал на врага
                    enemy.TakeDamage(player.Damage, Vector2.Zero);
                    player.Velocity.Y = -30f;
                    if (player.Velocity.X < 0)
                        player.Velocity.X = -30;
                    else
                        player.Velocity.X = 30;
                    EventManager.Instance.Trigger(new ScoreColectEvent(20));//?
                }
                else
                {
                    // Удар сбоку или снизу
                    player.TakeDamage(enemy.Damage, enemy.Velocity);
                }
            }
        }
        public void UpdateCollision(Player player, Slime enemy)
        {
            if (player.Collider.ColliderRectangle.Intersects(enemy.Rect))
            {
                bool hitFromAbove =
        player.Velocity.Y > 0 &&                          // Игрок падал
        player.Collider.ColliderRectangle.Bottom - player.Velocity.Y <= enemy.Rect.Top + 5;  // До столкновения был выше


                // Если игрок на земле — это заведомо удар сбоку
                if (player.IsOnGround)
                {
                    player.TakeDamage(enemy.Damage, new Vector2(enemy.Velocity.X * 50, enemy.Velocity.Y * 50));
                    return;
                }

                // Проверка сверху
                if (hitFromAbove)
                {
                    // Игрок упал на врага
                    enemy.TakeDamage(enemy.Health, Vector2.Zero);
                    player.Velocity.Y = -30f;
                    EventManager.Instance.Trigger(new ScoreColectEvent(20));//?
                }
                else
                {
                    // Удар сбоку или снизу
                    player.TakeDamage(enemy.Damage, enemy.Velocity);
                }
            }
        }
        public void UpdateCollision(Player player, Enemy enemy)
        {
            if (player.Collider.ColliderRectangle.Intersects(enemy.Rect))
            {
                bool hitFromAbove =
        player.Velocity.Y > 0 &&                          // Игрок падал
        player.Collider.ColliderRectangle.Bottom - player.Velocity.Y <= enemy.Rect.Top+5;  // До столкновения был выше

                
                // Если игрок на земле — это заведомо удар сбоку
                if (player.IsOnGround)
                {
                    player.TakeDamage(enemy.Damage,new Vector2(enemy.Velocity.X*50,enemy.Velocity.Y*50));
                    return;
                }

                // Проверка сверху
                if (hitFromAbove)
                {
                    // Игрок упал на врага
                    enemy.TakeDamage(enemy.Health, Vector2.Zero);
                    player.Velocity.Y = -30f;
                    EventManager.Instance.Trigger(new ScoreColectEvent(20));//?
                }
                else
                {
                    // Удар сбоку или снизу
                    player.TakeDamage(enemy.Damage, enemy.Velocity);
                }
            }
        }

        public static CollisionManeger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CollisionManeger();
                return _instance;
            }
        }

    }
    public class Collider
    {
        public Rectangle ColliderRectangle;
        private Vector2 _colliderOffset;
        private Vector2 _colliderSize;
        public Collider(Vector2 colliderOffset, Vector2 colliderSize,Rectangle rectangle)
        {
            _colliderOffset = colliderOffset;
            _colliderSize = colliderSize;
            ColliderRectangle = new Rectangle((int)(rectangle.X + colliderOffset.X),
                (int)(rectangle.Y + colliderOffset.Y), (int)colliderSize.X,(int)colliderSize.Y);
        }

        public Rectangle UpdateSpriteRect(Rectangle rectangle)
        {
            rectangle.X = ColliderRectangle.X - (int)_colliderOffset.X;
            rectangle.Y = ColliderRectangle.Y - (int)_colliderOffset.Y;
            return rectangle;
        }
    }
}

