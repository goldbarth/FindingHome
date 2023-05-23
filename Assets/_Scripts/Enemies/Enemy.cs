using UnityEngine;
using AddIns;

namespace Enemies
{
    public abstract class Enemy : GenerateGuid
    {
        [SerializeField] protected int _health = 30;

        /// <summary>
        /// Default health is 30.<br/>
        /// Adjust the damage to get the desired result.
        /// </summary>
        /// <param name="damage">int</param>
        /// <returns>Returns true if enemy is dead.</returns>
        protected internal virtual bool TakeDamage(int damage)
        {
            _health -= damage;
            var isDead = _health <= 0;
            if (isDead) Die();
            return isDead;
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}