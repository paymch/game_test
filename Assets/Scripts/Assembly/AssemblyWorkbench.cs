using UnityEngine;
using System.Collections.Generic;
using MyGardenFriend.Core;
using MyGardenFriend.Farming;

namespace MyGardenFriend.Assembly
{
    public class AssemblyWorkbench : MonoBehaviour
    {
        public List<AssemblySlot> slots = new List<AssemblySlot>();

        private void Start()
        {
            if (slots == null || slots.Count == 0)
            {
                slots = new List<AssemblySlot>(GetComponentsInChildren<AssemblySlot>());
            }
        }

        public void AttemptToSlotPart(BioCropType partType)
        {
            if (ResourceManager.Instance.GetBodyPartAmount(partType) > 0)
            {
                foreach (AssemblySlot slot in slots)
                {
                    if (slot.TrySlotPart(partType))
                    {
                        ResourceManager.Instance.ConsumeBodyPart(partType, 1);
                        return;
                    }
                }
            }
        }

        private void OnMouseDown()
        {
            // Simple click-to-slot fallback interaction on the workbench
            // In a real scenario you would drag an item, but clicking the bench tries to slot an available part
            foreach (BioCropType type in System.Enum.GetValues(typeof(BioCropType)))
            {
                if (ResourceManager.Instance.GetBodyPartAmount(type) > 0)
                {
                    AttemptToSlotPart(type);
                    break;
                }
            }
        }
    }
}
