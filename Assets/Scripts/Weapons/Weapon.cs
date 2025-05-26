using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        public float baseDamage = 10f;
        public float area = 1f;
        public float speed = 1f;
        public int amount = 1;
        public float cooldown = 1f;
        public float interval = 1f;
        public float knockback = 1f;
        public float chance = 0.2f;
        public float criticalMultiplier = 2;

        public abstract void Attack(
            Vector2 position,
            Quaternion rotation
        );
    }
}