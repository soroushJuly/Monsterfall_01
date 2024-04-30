using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine;

namespace Monsterfall_01
{
    public class WaveArgs : EventArgs
    {
        public WaveArgs(List<Enemy> enemies)
        {
            this.enemies = enemies;
        }
        private readonly List<Enemy> enemies;
    }
    internal class EnemyManager
    {
        List<Enemy> enemyList;
        List<Wave> waves;
        int currentWave;
        float waveTimeLeft;
        private Dictionary<string, List<Animation>> enemyAnimations;

        int mapLimitX;
        int mapLimitY;
        // A random number generator  
        Random random;
        
        public ref List<Enemy> GetEnemies() { return ref enemyList; }
        public int GetWaveCount() { return waves.Count; }
        public int GetCurrentWave() { return currentWave; }
        public float GetTimeToNextWave() { return waveTimeLeft; }
        public bool IsLastWave() { return waves.Count == (currentWave + 1); }

        public event EventHandler<WaveArgs> OnLoadWave;
        public event EventHandler<int> OnEnemyDied;
        public event EventHandler<Vector2> OnEnemyHit;
        public EnemyManager() 
        {
            enemyAnimations = new Dictionary<string, List<Animation>>();
            enemyList = new List<Enemy>();
            waves = new List<Wave>();
            currentWave = -1;
            waveTimeLeft = 5;

            random = new Random();
        }

        public void Initialize(List<Wave> waves, Vector2 mapSize)
        {
            this.waves = waves;
            mapLimitX = (int)(mapSize.X - 1);
            mapLimitY = (int)(mapSize.Y - 1);
        }
        public void AddAnimations(string name, List<Animation> animations)
        {
            enemyAnimations[name] = animations;
        }
        public void Remove(Enemy enemy)
        {
            enemyList.Remove(enemy);
        }
        private void LoadWave(int waveIndex)
        {
            waveTimeLeft = waves[waveIndex].timeToNextWave;
            for (int i = 0; i < waves[waveIndex].enemyCount; i++)
            {
                Enemy enemy = new Enemy();
                // TODO: MonsterIce will come from data in future
                // When there are more types of enemies
                // This will make the chance of enemies generate at the corners of the map higher
                int enemyX = Math.Min(random.Next(1, 2 * mapLimitX), mapLimitX);
                int enemyY = Math.Min(random.Next(1, 2 * mapLimitY), mapLimitY);
                // the add new vector(i,i) is just to make the position different in case they had the same tile
                enemy.Initialize(enemyAnimations["MonsterIce"], MapToScreen(enemyX, enemyY) + new Vector2(i,i));
                enemy.EnemyDied += (object sender, int e) => { OnEnemyDied(this, e); };
                enemy.EnemyHit += (object sender, Vector2 e) => { OnEnemyHit(this, e); };
                enemyList.Add(enemy);
            }
            OnLoadWave(this, new WaveArgs(enemyList));
        }
        public void Update(GameTime gameTime)
        {
            waveTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (waveTimeLeft < 0 && currentWave < (waves.Count - 1))
            {
                currentWave++;
                LoadWave(currentWave);
            }
            foreach(Enemy enemy in enemyList)
            {
                enemy.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch _spriteBatch,GraphicsDevice graphicsDevice)
        {
            foreach (Enemy enemy in enemyList)
            {
                enemy.Draw(_spriteBatch, graphicsDevice);
            }
        }
        // TODO: this better be a helper class
        private Vector2 MapToScreen(int x, int y)
        {
            // To map the tile toghether in ISOMETRIC way
            var screenX = x * 256 / 2 - y * (256 / 2) + 0;
            var screenY = y * (148 / 2 - 10) + x * (148 / 2 - 10) + 0;

            return new Vector2(screenX, screenY);
        }
    }
}
