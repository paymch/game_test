using System;
using UnityEngine;

namespace MyGardenFriend.Core
{
    public enum GamePhase
    {
        Farming,
        Assembly
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GamePhase CurrentPhase { get; private set; }

        public event Action<GamePhase> OnPhaseChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetPhase(GamePhase.Farming);
        }

        public void SetPhase(GamePhase newPhase)
        {
            if (CurrentPhase != newPhase)
            {
                CurrentPhase = newPhase;
                OnPhaseChanged?.Invoke(CurrentPhase);
            }
        }

        public void TogglePhase()
        {
            if (CurrentPhase == GamePhase.Farming)
            {
                SetPhase(GamePhase.Assembly);
            }
            else
            {
                SetPhase(GamePhase.Farming);
            }
        }

        // Simple UI Hooks
        public void AttemptRevive()
        {
            var lifecycle = FindObjectOfType<MyGardenFriend.Assembly.FriendLifeCycle>();
            if (lifecycle != null)
            {
                lifecycle.AttemptBirth();
            }
        }

        public void PlantSelectedSeed(MyGardenFriend.Farming.BioCropData cropData)
        {
            var controller = FindObjectOfType<MyGardenFriend.Farming.GardenPlotController>();
            if (controller != null && controller.Grid != null)
            {
                foreach(var tile in controller.Grid)
                {
                    if (tile.PlantSeed(cropData))
                    {
                        break;
                    }
                }
            }
        }
    }
}
