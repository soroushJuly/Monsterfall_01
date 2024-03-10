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
        private List<Enemy> enemyList;
        private List<Wave> waves;
        private int currentWave;
        private float waveTimeLeft;
        private Dictionary<string, List<Animation>> enemyAnimations;
        public ref List<Enemy> GetEnemies() { return ref enemyList; }
        public int GetWaveCount() { return waves.Count; }
        public int GetCurrentWave() { return currentWave; }
        public float GetTimeToNextWave() { return waveTimeLeft; }
        public bool IsLastWave() { return waves.Count == (currentWave + 1); }

        public event EventHandler<WaveArgs> OnLoadWave;
        public event EventHandler<int> OnEnemyDied;
        public EnemyManager() 
        {
            enemyAnimations = new Dictionary<string, List<Animation>>();
            enemyList = new List<Enemy>();
            waves = new List<Wave>();
            currentWave = -1;
            waveTimeLeft = 5;
        }

        public void AddWaves(List<Wave> waves)
        {
            this.waves = waves;
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
                enemy.Initialize(enemyAnimations["MonsterIce"], new Vector2(i * 150 + 500, i));
                enemy.EnemyDied += (object sender, int e) => { OnEnemyDied(this, e); };
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
    }
}
