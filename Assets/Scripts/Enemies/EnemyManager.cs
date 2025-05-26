using System.Collections;
using System.Collections.Generic;
using Stages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        public Stage stage;
        public float boundsMargin = 1f;

        private Coroutine _wave;
        private readonly HashSet<Enemy> _enemies = new();
        private Camera _camera;

        private float _screenAspect;
        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        private void Start()
        {
            _camera = Camera.main;

            if (_camera is null)
            {
                return;
            }

            _screenAspect = Screen.width / (float)Screen.height;

            var vertExtent = _camera.orthographicSize;
            var horzExtent = vertExtent * _screenAspect;

            _minX = -horzExtent - boundsMargin;
            _maxX = horzExtent + boundsMargin;
            _minY = -vertExtent - boundsMargin;
            _maxY = vertExtent + boundsMargin;

            var waves = stage.waves;

            if (waves is null)
            {
                return;
            }

            StartCoroutine(StartStage(waves));
        }

        private IEnumerator StartStage(Wave[] waves)
        {
            foreach (var wave in waves)
            {
                var timeUntilWave = Mathf.Max(0f, wave.timeElapsedSeconds - Time.time);

                yield return new WaitForSeconds(timeUntilWave);

                if (_wave != null)
                {
                    StopCoroutine(_wave);
                    _wave = null;
                }

                _wave = StartCoroutine(StartWave(wave));
            }
        }

        private IEnumerator StartWave(Wave wave)
        {
            while (true)
            {
                UpdateEnemySpawns(wave);

                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }

        private void UpdateEnemySpawns(Wave wave)
        {
            var current = _enemies.Count;
            var target = wave.enemyMinimum;

            var numberToSpawn = Mathf.Min(Mathf.Max(0, target - current), target / 5);

            for (var i = 0; i < numberToSpawn; i++)
            {
                SpawnEnemy(wave.enemyPrefab);
            }
        }

        private void SpawnEnemy(GameObject enemyPrefab)
        {
            var cameraPosition = _camera.transform.position;
            var position = new Vector3(cameraPosition.x, cameraPosition.y, 0f) + GetRandomBoundsPosition();

            var go = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
            var enemy = go.GetComponent<Enemy>();

            _enemies.Add(enemy);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        private Vector3 GetRandomBoundsPosition()
        {
            var value = Random.value * (_screenAspect + 1f);

            // On horizontal side
            if (value > 1f)
            {
                var x = Random.Range(_minX, _maxX);
                var y = Random.value > 0.5f ? _minY : _maxY;

                return new Vector3(x, y, 0f);
            }
            else
            {
                var y = Random.Range(_minY, _maxY);
                var x = Random.value > 0.5f ? _minX : _maxX;

                return new Vector3(x, y, 0f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                new Vector3(_minX, _minY, 0f),
                new Vector3(_minX, _maxY, 0f)
            );
            Gizmos.DrawLine(
                new Vector3(_maxX, _minY, 0f),
                new Vector3(_maxX, _maxY, 0f)
            );
            Gizmos.DrawLine(
                new Vector3(_minX, _minY, 0f),
                new Vector3(_maxX, _minY, 0f)
            );
            Gizmos.DrawLine(
                new Vector3(_minX, _maxY, 0f),
                new Vector3(_maxX, _maxY, 0f)
            );
        }
    }
}