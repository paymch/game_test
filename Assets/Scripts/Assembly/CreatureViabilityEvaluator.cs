using UnityEngine;
using System.Collections.Generic;

namespace MyGardenFriend.Assembly
{
    public enum ViabilityState
    {
        Incomplete,
        Terminal,
        Crippled,
        FullLife
    }

    public class CreatureViabilityEvaluator : MonoBehaviour
    {
        public AssemblyWorkbench workbench;

        private void Start()
        {
            if (workbench == null)
            {
                workbench = FindObjectOfType<AssemblyWorkbench>();
            }
        }

        public ViabilityState EvaluateViability(out float survivalTimer)
        {
            survivalTimer = -1f; // -1 means infinite/stable
            if (workbench == null) return ViabilityState.Incomplete;

            bool allEssentialsPresent = true;
            bool anyEssentialDefective = false;
            int sensoryCount = 0;
            bool missingNonEssentials = false;

            foreach (var slot in workbench.slots)
            {
                if (slot.isEssential)
                {
                    if (!slot.IsFilled)
                    {
                        allEssentialsPresent = false;
                    }
                    else if (slot.IsDefective)
                    {
                        anyEssentialDefective = true;
                    }
                }
                else
                {
                    if (slot.IsFilled)
                    {
                        if (slot.acceptedPartType == Farming.BioCropType.Eyes || slot.acceptedPartType == Farming.BioCropType.Ears)
                        {
                            sensoryCount++;
                        }
                    }
                    else
                    {
                        missingNonEssentials = true;
                    }
                }
            }

            if (!allEssentialsPresent)
            {
                return ViabilityState.Incomplete;
            }

            if (anyEssentialDefective)
            {
                survivalTimer = Random.Range(60f, 180f);
                return ViabilityState.Terminal;
            }

            if (missingNonEssentials || sensoryCount < 1)
            {
                return ViabilityState.Crippled;
            }

            return ViabilityState.FullLife;
        }
    }
}
