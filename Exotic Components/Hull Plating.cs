using UnityEngine;
using PulsarModLoader.Content.Components.HullPlating;
using HarmonyLib;
namespace Exotic_Components
{
    internal class Hull_Plating
    {
        class AntiBreachMod : HullPlatingMod
        {
            public override string Name => "AntiBreach";

            public override PLShipComponent PLHullPlating => new AntiBreach(EHullPlatingType.E_HULLPLATING_CCGE,0);
        }

        internal class AntiBreach : PLHullPlating
        {
            public AntiBreach(EHullPlatingType inType, int inLevel) : base(inType, inLevel)
            {
                base.SubType = HullPlatingModManager.Instance.GetHullPlatingIDFromName("AntiBreach");
                base.Level = inLevel;
                this.Name = "Anti-Breach Plating";
                this.Desc = "This hull plating covers your entire ship and reinforces your ship systems ensuring that they will NEVER be damaged, however the material used causes you to take double the hull damage!";
                Experimental = true;
                this.m_MarketPrice = 20000;
            }
        }

        [HarmonyPatch(typeof(PLMainSystem), "TakeDamage")]
        class SystemDamage 
        {
            static bool Prefix(PLMainSystem __instance) 
            {
                if(__instance.MyShipInfo != null && __instance.MyShipInfo.MyStats != null) 
                {
                    PLHullPlating plating = __instance.MyShipInfo.MyStats.GetShipComponent<PLHullPlating>(ESlotType.E_COMP_HULLPLATING, false);
                    if(plating != null) 
                    {
                        if(plating is AntiBreach) 
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
