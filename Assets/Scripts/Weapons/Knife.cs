using UnityEngine;

namespace Weapons
{
    public class Knife : Weapon
    {
        public GameObject knifePrefab;

        public override void Attack(
            Vector2 position,
            Quaternion rotation
        )
        {
            var go = Instantiate(knifePrefab, position, Quaternion.identity);
            var projectile = go.GetComponent<Projectile>();

            projectile.Init(this, rotation, speed);
        }
    }
}