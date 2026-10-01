using UnityEngine;
using System;

namespace MyGardenFriend.Assembly
{
    public class FriendLifeCycle : MonoBehaviour
    {
        public CreatureViabilityEvaluator evaluator;

        public ViabilityState CurrentState { get; private set; } = ViabilityState.Incomplete;
        public float RemainingLifeTime { get; private set; } = -1f;
        public bool IsAlive { get; private set; } = false;

        public event Action OnFriendDied;
        public event Action<ViabilityState> OnFriendBorn;

        public void AttemptBirth()
        {
            if (evaluator == null) return;

            CurrentState = evaluator.EvaluateViability(out float initialSurvivalTimer);

            if (CurrentState != ViabilityState.Incomplete)
            {
                IsAlive = true;
                RemainingLifeTime = initialSurvivalTimer;
                OnFriendBorn?.Invoke(CurrentState);
                Debug.Log($"A friend is born! State: {CurrentState}, Lifetime: {RemainingLifeTime}");
            }
            else
            {
                Debug.Log("Cannot birth friend, parts are incomplete.");
            }
        }

        private void Update()
        {
            if (IsAlive && CurrentState == ViabilityState.Terminal)
            {
                if (RemainingLifeTime > 0)
                {
                    RemainingLifeTime -= Time.deltaTime;
                    if (RemainingLifeTime <= 0)
                    {
                        Die();
                    }
                }
            }
        }

        private void Die()
        {
            IsAlive = false;
            RemainingLifeTime = 0;
            OnFriendDied?.Invoke();
            Debug.Log("The friend has expired.");
        }
    }
}
