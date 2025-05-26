using UnityEngine;

namespace Stages
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Wave", order = 1)]
    public class Wave : ScriptableObject
    {
        public float timeElapsedSeconds = 0f;
        public GameObject enemyPrefab;
        public int enemyMinimum = 1;
        public float spawnInterval = 1f;
    }
}