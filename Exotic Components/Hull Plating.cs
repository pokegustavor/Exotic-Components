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

        class TurtleMod : HullPlatingMod
        {
            public override string Name => "TurtleP";

            public override PLShipComponent PLHullPlating => new TurtleMode(EHullPlatingType.E_HULLPLATING_CCGE, 0);
        }

        class GlassMod : HullPlatingMod
        {
            public override string Name => "GlassP";

            public override PLShipComponent PLHullPlating => new GlassMode(EHullPlatingType.E_HULLPLATING_CCGE, 0);
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

        internal class TurtleMode : PLHullPlating 
        {
            public TurtleMode(EHullPlatingType inType, int inLevel) : base(inType, inLevel)
            {
                base.SubType = HullPlatingModManager.Instance.GetHullPlatingIDFromName("TurtleP");
                base.Level = inLevel;
                this.Name = "\'Turtle Plating\'";
                this.Desc = "This hull plating uses extra reinforced plates that heavily improves hull armor, but the heavy plates will decrease the turret power!";
                Experimental = true;
                this.m_MarketPrice = 20000;
            }
            public override void FinalLateAddStats(PLShipStats inStats)
            {
                base.FinalLateAddStats(inStats);
                inStats.HullArmor *= 5;
                inStats.TurretDamageFactor *= 0.75f;
            }

            public override string GetStatLineLeft()
            {
                return string.Concat(new string[]
                {
                    "Ship hull armor 5x",
                    "\n",
                    "<color=red>Ship turret damage decreased by 25%</color>"
                });
            }
        }

        internal class GlassMode : PLHullPlating
        {
            public GlassMode(EHullPlatingType inType, int inLevel) : base(inType, inLevel)
            {
                base.SubType = HullPlatingModManager.Instance.GetHullPlatingIDFromName("GlassP");
                base.Level = inLevel;
                this.Name = "\'Glass Plating\'";
                this.Desc = "This hull plating will connect to all turrets to heavily improve their damage, however they are extremely fragile, this way completely negating armor strengh!";
                Experimental = true;
                this.m_MarketPrice = 25000;
            }
            public override void FinalLateAddStats(PLShipStats inStats)
            {
                base.FinalLateAddStats(inStats);
                inStats.HullArmor = 0;
                inStats.TurretDamageFactor *= 2.5f;
            }

            public override string GetStatLineLeft()
            {
                return string.Concat(new string[]
                {
                    "Ship turret damage 2.5x",
                    "\n",
                    "<color=red>Ship hull armor 0x</color>"
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
