using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/Enemy Stats")]
    public class EnemyStats : ScriptableObject
    {
        public float health;
        public float power;
        public float mSpeed;
        public float knockback;
        public float knockbackMax;
        public int xp;
    }
}