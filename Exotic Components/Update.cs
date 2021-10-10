using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exotic_Components
{
    [HarmonyLib.HarmonyPatch(typeof(PLShipInfo),"Update")]
    internal class Update
    {
        static void Postfix()
        {
            try
            {
                if (PLServer.GetCurrentSector().Name == "The Core(MOD)" && !PLEncounterManager.Instance.PlayerShip.Get_IsInWarpMode())
                {
                    InitialStore.UpdateCore();
                }
                bool found = false;
                foreach(PLCPU cpu in PLEncounterManager.Instance.PlayerShip.MyStats.GetComponentsOfType(ESlotType.E_COMP_CPU,false)) 
                {
                    if (cpu.Name == "Turret Thermo Boost")
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) CPUS.ThermoBoost.MaxHeat = 1.1f;
            }
            catch { }
        }
}
}
