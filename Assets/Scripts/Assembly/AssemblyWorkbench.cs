using UnityEngine;
using System.Collections.Generic;
using MyGardenFriend.Core;
using MyGardenFriend.Farming;

namespace MyGardenFriend.Assembly
{
    public class AssemblyWorkbench : MonoBehaviour
    {
        public List<AssemblySlot> slots = new List<AssemblySlot>();

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
    }
}
