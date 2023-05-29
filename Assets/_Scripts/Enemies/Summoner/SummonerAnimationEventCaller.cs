using UnityEngine;

namespace Enemies.Summoner
{
    public class SummonerAnimationEventCaller : MonoBehaviour
    {
        private Summoner _summoner;

        private void Awake()
        {
            _summoner = transform.parent.GetComponentInChildren<Summoner>();
        }

        public void CallSummonerEvent(SummonerAnimationEvents toCall)
        {
            _summoner.CallAnimationEvent(toCall);
        }
    }
}