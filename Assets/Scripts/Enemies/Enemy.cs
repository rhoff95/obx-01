using UnityEngine;
using Weapons;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        public EnemyStats stats;
        
        public float moveThreshold;
        public GameObject xpPrefab;

        private float _speed;
        private Transform _target;

        private float _health;

        private const float BaseSpeed = 0.35f;

        private void Awake()
        {
            _health = stats.health;
            _target = GameObject.FindWithTag("Player").transform;

            _speed = stats.mSpeed;
        }

        private void Update()
        {
            var direction = _target.position - transform.position;

            if (direction.magnitude <= moveThreshold)
            {
                return;
            }

            transform.position += (_speed * BaseSpeed* Time.deltaTime) * direction.normalized;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Projectile"))
            {
                return;
            }

            var projectile = other.GetComponent<Projectile>();
            Hit(projectile);

            projectile.Impact();
        }

        private void Hit(Projectile projectile)
        {
            _health -= projectile.GetDamage();

            if (_health <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            for (var i = 0; i < stats.xp; i++)
            {
                Instantiate(xpPrefab, transform.position, Quaternion.identity);
            }
            
            var enemyManager = FindFirstObjectByType<EnemyManager>();
            enemyManager.RemoveEnemy(this);
            
            Destroy(gameObject);
        }
    }
}