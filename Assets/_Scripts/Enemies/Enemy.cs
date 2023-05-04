using AddIns;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : GenerateGuid
    {
        private int _health;

        protected virtual void Awake()
        {
            _health = 30;
        }
        
        /// <summary>
        /// Default health is 30.<br/>
        /// Adjust the damage to get the desired result.
        /// </summary>
        /// <param name="damage">int</param>
        /// <returns>Returns true if enemy is dead</returns>
        protected internal bool TakeDamage(int damage)
        {
            _health -= damage;
            Debug.Log("Enemy health: " + _health);
            var isDead = _health <= 0;
            if (isDead) Die();
            return isDead;
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}