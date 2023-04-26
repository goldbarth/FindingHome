namespace Enemies
{
    public class Summoner : Enemy
    {
        protected override void Awake()
        {
            base.Awake();
        }
        
        public bool TakeHit()
        {
            return base.TakeDamage(10);
        }
    }
}