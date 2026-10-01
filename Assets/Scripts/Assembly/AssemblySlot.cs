using UnityEngine;
using MyGardenFriend.Farming;

namespace MyGardenFriend.Assembly
{
    public class AssemblySlot : MonoBehaviour
    {
        public BioCropType acceptedPartType;
        public bool isEssential;

        public bool IsFilled { get; private set; }

        // This could hold data about the specific part's quality/health later
        public bool IsDefective { get; private set; }

        public bool TrySlotPart(BioCropType partType, bool defective = false)
        {
            if (!IsFilled && partType == acceptedPartType)
            {
                IsFilled = true;
                IsDefective = defective;
                UpdateVisuals();
                return true;
            }
            return false;
        }

        public void RemovePart()
        {
            IsFilled = false;
            IsDefective = false;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            // Placeholder: Show/hide child mesh or change color
        }
    }
}
