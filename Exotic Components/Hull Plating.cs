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

            public override PLShipComponent PLHullPlating => new AntiBreach(EHullPlatingType.E_HULLPLATING_CCGE, 0);
        }

        class MegaHullPMod : HullPlatingMod
        {
            public override string Name => "MegaHullP";

            public override PLShipComponent PLHullPlating => new MegaHullP(EHullPlatingType.E_HULLPLATING_CCGE, 0);
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

            public override string GetStatLineLeft()
            {
                return string.Concat(new string[]
                {
                    "Ship systems immune to damage",
                    "\n",
                    "<color=red>Ship hull damage 2x</color>"
                });
            }
        }

        internal class MegaHullP : PLHullPlating
        {
            public MegaHullP(EHullPlatingType inType, int inLevel) : base(inType, inLevel)
            {
                base.SubType = HullPlatingModManager.Instance.GetHullPlatingIDFromName("MegaHullP");
                base.Level = inLevel;
                this.Name = "Reinforced Hull Plating";
                this.Desc = "This hull plating is designed to reduce the damage taken by the hull in half, however the ship systems have become more even more exposed, making them take quadruple damage!";
                Experimental = true;
                this.m_MarketPrice = 25000;
            }

            public override string GetStatLineLeft()
            {
                return string.Concat(new string[]
                {
                    "Ship hull damage 50% reduction",
                    "\n",
                    "<color=red>Ship system damage 4x</color>"
                });
            }
        }

        [HarmonyPatch(typeof(PLMainSystem), "TakeDamage")]
        class SystemDamage
        {
            internal static bool Prefix(ref float inDmg, PLMainSystem __instance)
            {
                if (__instance.MyShipInfo != null && __instance.MyShipInfo.MyStats != null)
                {
                    PLHullPlating plating = __instance.MyShipInfo.MyStats.GetShipComponent<PLHullPlating>(ESlotType.E_COMP_HULLPLATING, false);
                    if (plating != null)
                    {
                        if (plating is AntiBreach)
                        {
                            return false;
                        }
                        if (plating is MegaHullP)
                        {
                            inDmg *= 4;
                        }
                    }
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(PLMainSystem), "TakeBoltDamage")]
        class SystemBulletDamage
        {
            static bool Prefix(ref float inDmg, PLMainSystem __instance)
            {
                return SystemDamage.Prefix(ref inDmg, __instance);
            }
        }
    }
}
