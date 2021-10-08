using PulsarModLoader.Content.Components.Reactor;
using UnityEngine;
using HarmonyLib;
namespace Exotic_Components
{
    class Reactors
    {
        class ColdReactor : ReactorMod
        {
            public override string Name => "CryoCore MK2";

            public override string Description => "A questinable reactor by Dragon's INC that somehow makes more power the colder it is, but why question the logic behind if it works";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 68000f;

            public override float MaxTemp => 3000f;

            public override float EmergencyCooldownTime => 30f;

            public override float HeatOutput => 2f;

            public override void Tick(PLShipComponent InComp)
            {
                base.Tick(InComp);
                (InComp as PLReactor).EnergyOutputMax = 68000f * (1f - Mathf.Clamp((InComp as PLReactor).ShipStats.ReactorTempCurrent / (InComp as PLReactor).ShipStats.ReactorTempMax, 0f, 0.95f));
            }
            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                return string.Concat(new string[]
                {
                    (me.TempMax * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " kP\n",
                    me.EmergencyCooldownTime.ToString("0.0"),
                    " sec\n",
                    (me.OriginalEnergyOutputMax * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " MW\n",
                });
            }
            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                   PLLocalize.Localize("Max Temp", false),
                    "\n",
                    PLLocalize.Localize("Emer. Cooldown", false),
                    "\n",
                    PLLocalize.Localize("Output", false),
                });
            }
        }
        class SteamReactor : ReactorMod
        {
            public override string Name => "Steam Core";

            public override string Description => "This steampunk looking reactor makes more energy the hotter it is, while also adding to the thrust power by releasing the core steam. Where does all that water come from? Don't ask too many questions";

            public override int MarketPrice => 48000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 34000f;

            public override float EnergySignatureAmount => 7f;

            public override float MaxTemp => 4300f;

            public override float EmergencyCooldownTime => 20f;

            public override float HeatOutput => 1.3f;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                me.EnergyOutputMax = me.OriginalEnergyOutputMax * Mathf.Clamp(me.ShipStats.ReactorTempCurrent / me.ShipStats.ReactorTempMax, 0.02f, 1f);
            }
            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                return string.Concat(new string[]
                {
                    (me.TempMax * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " kP\n",
                    me.EmergencyCooldownTime.ToString("0.0"),
                    " sec\n",
                    (me.OriginalEnergyOutputMax * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " MW\n",
                });
            }
            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                   PLLocalize.Localize("Max Temp", false),
                    "\n",
                    PLLocalize.Localize("Emer. Cooldown", false),
                    "\n",
                    PLLocalize.Localize("Output", false),
                });
            }
        }
        class DoomReactor : ReactorMod
        {
            public override string Name => "Doom Reactor";

            public override string Description => "This reactor is a modified version of something know as the Ancient Reactor. This version can be used on any ship, but without proper maintenance it will lose levels, and may explode!";

            public override int MarketPrice => 56000;

            public override bool Unstable => true;

            public override bool Contraband => true;

            public override Texture2D IconTexture => (Texture2D)Resources.Load("Icons/27_Reactor");

            public override float EnergyOutputMax => 50000f;

            public override float EnergySignatureAmount => 10f;

            public override float MaxTemp => 1500f;

            public override float EmergencyCooldownTime => 7f;

            public override void OnWarp(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                if (!me.IsEquipped) return;
                if (me.Unstable)
                {
                    int num = me.Unstable_JumpCounter;
                    me.Unstable_JumpCounter = num + 1;
                }
                if (me.Level == 0 && me.Unstable_JumpCounter <= 6)
                {
                    if (me.Unstable_JumpCounter < 6) PulsarModLoader.Utilities.Messaging.Notification("Warning! Reactor unstable! " + (7 - me.Unstable_JumpCounter) + " Jumps until explosion! Upgrade it as soon as possible!", PhotonTargets.All, default, 10000, true);
                    else PulsarModLoader.Utilities.Messaging.Notification("Warning! Reactor criticaly unstable! It will explode next jump! Upgrade or remove it IMMEDIATELY", PhotonTargets.All, default, 10000, true);
                }
                if (me.Unstable_JumpCounter > 6)
                {
                    me.Unstable_JumpCounter = 0;
                    if (me.Level > 0)
                    {
                        int num = me.Level;
                        me.Level = num - 1;
                        PulsarModLoader.Utilities.Messaging.Notification("Warning! Doom Reactor has lost a level! Current level: " + (me.Level + 1), PhotonTargets.All, default, 5000, true);
                    }
                    else 
                    {
                        PLServer.Instance.ServerEjectReactorCore(PLEncounterManager.Instance.PlayerShip.ShipID, 300000);
                    }
                }
            }
            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                return string.Concat(new string[]
                {
                    (me.TempMax * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " kP\n",
                    me.EmergencyCooldownTime.ToString("0.0"),
                    " sec\n",
                    (me.GetDisplayedEnergyOutputMax() * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " MW\n",
                });
            }
            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                   PLLocalize.Localize("Max Temp", false),
                    "\n",
                    PLLocalize.Localize("Emer. Cooldown", false),
                    "\n",
                    PLLocalize.Localize("Output", false),
                });
            }
        }
        class BiscuitReactor : ReactorMod
        {
            public static float BiscuitBoost = 0;
            public override string Name => "Ultimate Fluffy Biscuit Reactor";

            public override string Description => "An upgraded Fluffy Biscuit Jumbo Reactor that can gain more power if you atomize some biscuits. Does this make you question what they put in those biscuits?";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 13600f;

            public override float EnergySignatureAmount => 4f;

            public override float MaxTemp => 2100f;

            public override float EmergencyCooldownTime => 3f;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                me.EnergyOutputMax = me.OriginalEnergyOutputMax * Mathf.Clamp(BiscuitBoost/5f + 1f, 1f, 4f);
            }

            public override void OnWarp(PLShipComponent InComp)
            {
                BiscuitBoost = Mathf.Clamp(BiscuitBoost - 5f, 0f, 20f);
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                return string.Concat(new string[]
                {
                    (me.TempMax * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " kP\n",
                    me.EmergencyCooldownTime.ToString("0.0"),
                    " sec\n",
                    (me.GetDisplayedEnergyOutputMax() * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " MW\n",
                    (Mathf.Clamp(BiscuitBoost/5f, 0f, 3f) * 100).ToString("0"),
                    " %"
                });
            }

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                   PLLocalize.Localize("Max Temp", false),
                    "\n",
                    PLLocalize.Localize("Emer. Cooldown", false),
                    "\n",
                    PLLocalize.Localize("Output", false),
                    "\n",
                    "Biscuit Boost"
                });
            }
        }
        [HarmonyPatch(typeof(PLReactor), "GetStatLineLeft")]
        class LeftDescFix
        {
            static void Postfix(PLReactor __instance, ref string __result)
            {
                int subtypeformodded = __instance.SubType - ReactorModManager.Instance.VanillaReactorMaxType;
                if (subtypeformodded > -1 && subtypeformodded < ReactorModManager.Instance.ReactorTypes.Count && __instance.ShipStats != null)
                {
                    __result = ReactorModManager.Instance.ReactorTypes[subtypeformodded].GetStatLineLeft(__instance);
                }
            }
        }
        [HarmonyPatch(typeof(PLReactor), "GetStatLineRight")]
        class RightDescFix
        {
            static void Postfix(PLReactor __instance, ref string __result)
            {
                int subtypeformodded = __instance.SubType - ReactorModManager.Instance.VanillaReactorMaxType;
                if (subtypeformodded > -1 && subtypeformodded < ReactorModManager.Instance.ReactorTypes.Count && __instance.ShipStats != null)
                {
                    __result = ReactorModManager.Instance.ReactorTypes[subtypeformodded].GetStatLineRight(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(PLShipInfo), "ServerClickAtomize")]
        class BiscuitCharge 
        {
            static void Prefix() 
            {
                foreach (PLPawnItem plpawnItem in PLServer.Instance.ResearchLockerInventory.AllItems)
                {
                    if (plpawnItem.GetItemName(true).Contains("Biscuit")) 
                    {
                        BiscuitReactor.BiscuitBoost = Mathf.Clamp(BiscuitReactor.BiscuitBoost + 1f,0f,20f);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PLShipComponent), "OnWarp")]

        class ManualOnWarp
        {
            static bool Prefix(PLShipComponent __instance)
            {
                PLReactor pLReactor = __instance as PLReactor;
                if (pLReactor == null) return true;
                int subtypeformodded = pLReactor.SubType - ReactorModManager.Instance.VanillaReactorMaxType;
                if (subtypeformodded > -1 && subtypeformodded < ReactorModManager.Instance.ReactorTypes.Count && pLReactor.ShipStats != null)
                {
                    ReactorModManager.Instance.ReactorTypes[subtypeformodded].OnWarp(pLReactor);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PLShipStats), "CalculateStats")]
        class UpdateStats 
        {
            static void Postfix(PLShipStats __instance) 
            {
                if(__instance.Ship.MyReactor!= null && __instance.Ship.MyReactor.Name == "Steam Core") 
                {

                    float multiplier = Mathf.Clamp(__instance.ReactorTempCurrent * 2f / __instance.ReactorTempMax, 1f, 1.5f);
                    __instance.m_ThrustOutputCurrent *= multiplier;
                    __instance.m_ThrustOutputMax *= multiplier;
                }
            }
        }

        [HarmonyPatch(typeof(PLReactor), "Tick")]

        class ManualTick
        {
            static void Postfix(PLReactor __instance)
            {
                int subtypeformodded = __instance.SubType - ReactorModManager.Instance.VanillaReactorMaxType;
                if (subtypeformodded > -1 && subtypeformodded < ReactorModManager.Instance.ReactorTypes.Count && __instance.ShipStats != null)
                {
                    ReactorModManager.Instance.ReactorTypes[subtypeformodded].Tick(__instance);
                }
            }
        }
    }
}
