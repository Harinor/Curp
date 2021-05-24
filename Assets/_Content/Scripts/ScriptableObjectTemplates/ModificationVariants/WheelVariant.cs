using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelVariant", menuName = "Variants/WheelVariant", order = 1)]
public class WheelVariant : ModificationVariant
{
    [SerializeField] GameObject wheelFront;
    [SerializeField] GameObject wheelRear;
    [SerializeField] WheelSlots wheelSlots;

    public override void Apply()
    {
        base.Apply();

        InstallNewWheels(wheelSlots.GetFrontWheelSlots(), wheelFront, wheelRear);
        InstallNewWheels(wheelSlots.GetRearWheelSlots(), wheelRear, wheelFront);
        /*
        foreach (var slotName in wheelSlots.GetFrontWheelSlots())
        {
            Transform slot = SharedData.ActiveVehicle.transform.Find(slotName);

            //--- Destroy old mods ---
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }

            //--- Install new mod ---
            if (wheelFront != null)
            {
                Instantiate(wheelFront, slot);                
            }
            else if (wheelRear != null)
            {
                Instantiate(wheelRear, slot); 
            }
        }

        foreach (var slotName in wheelSlots.GetRearWheelSlots())
        {
            Transform slot = SharedData.ActiveVehicle.transform.Find(slotName);

            //--- Destroy old mods ---
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }

            //--- Install new mod ---
            if (wheelRear != null)
            {
                Instantiate(wheelRear, slot);                
            }
            else if (wheelFront != null)
            {
                Instantiate(wheelFront, slot); 
            }
        }
        */
    }

    private void InstallNewWheels(IEnumerable<string> modSlots, GameObject mod, GameObject backup)
    {
        foreach (var slotName in modSlots)
        {
            Transform slot = SharedData.ActiveVehicle.transform.Find(slotName);

            //--- Destroy old mods ---
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }

            //--- Install new mod ---
            if (mod != null)
            {
                Instantiate(mod, slot);
            }
            else if (backup != null)
            {
                Instantiate(backup, slot);
            }
        }
    }

    [System.Serializable]
    public class WheelSlots
    {
        [SerializeField] string[] frontLeft;
        [SerializeField] string[] frontRight;
        [SerializeField] string[] rearRight;
        [SerializeField] string[] rearLeft;

        public IEnumerable<string> GetFrontWheelSlots()
        {
            foreach (string slot in frontLeft)
            {
                yield return slot;
            }
            foreach (string slot in frontRight)
            {
                yield return slot;
            }
        }

        public IEnumerable<string> GetRearWheelSlots()
        {
            foreach (string slot in rearLeft)
            {
                yield return slot;
            }
            foreach (string slot in rearRight)
            {
                yield return slot;
            }
        }

    }
}


