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
                transform.SetParent(null);
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

        [Header("UI References - Runtime Only since no real UI lib")]
        public string HUD_Text;
        public string Inventory_Text;
        public string Status_Text;

        public Farming.BioCropData SelectedSeed { get; set; }

        private void Update()
        {
            if (ResourceManager.Instance != null)
            {
                HUD_Text = $"Blood: {ResourceManager.Instance.GetFluidAmount(Farming.FluidType.Blood):F0} | Sweat: {ResourceManager.Instance.GetFluidAmount(Farming.FluidType.Sweat):F0} | Urine: {ResourceManager.Instance.GetFluidAmount(Farming.FluidType.Urine):F0}";

                string inv = "Inventory: ";
                foreach (Farming.BioCropType type in Enum.GetValues(typeof(Farming.BioCropType)))
                {
                    int amt = ResourceManager.Instance.GetBodyPartAmount(type);
                    if (amt > 0) inv += $"{type}:{amt} ";
                }
                Inventory_Text = inv;
            }

            var lifecycle = FindObjectOfType<MyGardenFriend.Assembly.FriendLifeCycle>();
            if (lifecycle != null)
            {
                if (lifecycle.IsAlive)
                {
                    Status_Text = $"State: {lifecycle.CurrentState} | Time: {(lifecycle.RemainingLifeTime < 0 ? "Infinite" : lifecycle.RemainingLifeTime.ToString("F1"))}";
                }
                else
                {
                    Status_Text = "Dead / Not Born";
                }
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

    }
}
