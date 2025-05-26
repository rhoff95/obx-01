using UnityEngine;

namespace Weapons
{
    public class Projectile : MonoBehaviour
    {
        public Weapon sourceWeapon;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            Destroy(gameObject, 10f);
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        public void Init(Weapon source, Quaternion rotation, float speed)
        {
            sourceWeapon = source;
            transform.rotation = rotation; //Quaternion.AngleAxis(rotation, Vector3.forward);

            var right = new Vector2(transform.right.x, transform.right.y);
            _rb.linearVelocity = speed * right;
        }

        public float GetDamage()
        {
            return sourceWeapon.baseDamage;
        }

        public void Impact()
        {
            Destroy(gameObject);
        }
    }
}