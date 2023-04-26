namespace Enemies
{
    public class Spitter : Enemy
    {
        protected override void Awake()
        {
            base.Awake();
        }
        
        protected override bool TakeDamage(int damage)
        {
            return base.TakeDamage(3);
        }
    }
}