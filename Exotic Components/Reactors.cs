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

            public override string Description => "A questionable reactor by Dragon's INC that somehow makes more power the colder it is, but why question the logic behind if it works";

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
        class PipeReactor : ReactorMod
        {
            public override string Name => "Pipe Reactor";

            public override string Description => "This reactor has extended internal cooling system that leaks into space when hull is damaged, causing it to cool faster at lower hull integrity.";

            public override int MarketPrice => 14300;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 24000f;

            public override float EnergySignatureAmount => 9f;

            public override float MaxTemp => 2100f;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                me.HeatOutput = Mathf.Clamp(me.ShipStats.HullCurrent / me.ShipStats.HullMax, 0.65f, 1f);
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
        class Thermopoint : ReactorMod
        {
            public override string Name => "ThermoPoint Reactor";

            public override string Description => "This special reactor by badteck™ may not make much energy, but it can handle massive amounts of heat, when too hot it releases some of the heat to the exterior and interior of the ship, also causing fires.";

            public override int MarketPrice => 15000;

            public override bool Contraband => true;

            public override float EnergyOutputMax => 12700f;

            public override float EnergySignatureAmount => 20f;

            public override float MaxTemp => 6800f;

            public override float EmergencyCooldownTime => 15f;

            public override void Tick(PLShipComponent InComp)
            {
                if (InComp.ShipStats.Ship.Exterior.GetComponent<PLSpaceHeatVolume>() == null)
                {
                    InComp.ShipStats.Ship.Exterior.AddComponent<PLSpaceHeatVolume>();
                }
                PLSpaceHeatVolume heatVolume = InComp.ShipStats.Ship.Exterior.GetComponent<PLSpaceHeatVolume>();
                if (heatVolume.MyPS == null)
                {
                    GameObject IceDust = Object.Instantiate(PLGlobal.Instance.IceDustPrefab);
                    IceDust.transform.SetParent(InComp.ShipStats.Ship.Exterior.transform, false);
                    IceDust.transform.localPosition = Vector3.zero;
                    IceDust.transform.localRotation = Quaternion.identity;
                    IceDust.transform.localScale = Vector3.one;
                    heatVolume.MyPS = IceDust.GetComponent<PLIceDust>().MyPS;
                    heatVolume.MyPS.startColor = new Color(1, 0.394f, 0.14f, 0.78f);
                }
                if (!InComp.IsEquipped || InComp.ShipStats.ReactorTempCurrent/InComp.ShipStats.ReactorTempMax < 0.7f)
                {
                    heatVolume.MyPS.enableEmission = false;
                    ParticleSystem.ShapeModule shape = heatVolume.MyPS.shape;
                    shape.radius = 0.0001f;
                    shape.scale = new Vector3(0,0,0);
                    heatVolume.MyPS.gameObject.transform.localPosition = InComp.ShipStats.Ship.Exterior.transform.localPosition - new Vector3(0, -50, 0);
                }
                else 
                {
                    heatVolume.MyPS.enableEmission = true;
                    ParticleSystem.ShapeModule shape = heatVolume.MyPS.shape;
                    shape.radius = 60f;
                    shape.scale = new Vector3(1, 1, 1);
                    heatVolume.MyPS.gameObject.transform.position = InComp.ShipStats.Ship.Exterior.transform.position;
                }
                InComp.ShipStats.Ship.MyTLI.AtmoSettings.Temperature = 1f + InComp.ShipStats.ReactorTempCurrent / InComp.ShipStats.ReactorTempMax * 1.5f;
                if (!InComp.IsEquipped) 
                {
                    InComp.ShipStats.Ship.MyTLI.AtmoSettings.Temperature = 1f;
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
        class StableReactor : ReactorMod
        {
            public override string Name => "ZeroPoint Reactor";

            public override string Description => "This state of the art reactor can work at any capacity without increasing heat, but it has come with the cost of extreme low energy production. At least you won't be using any coolant, and it emits no EM signature!";

            public override int MarketPrice => 9000;

            public override bool Experimental => true;

            public override Texture2D IconTexture => (Texture2D)Resources.Load("Icons/29_Reactor");

            public override float EnergyOutputMax => 8600f;

            public override float EnergySignatureAmount => 0f;

            public override float MaxTemp => 750f;

            public override float EmergencyCooldownTime => 0f;

            public override float HeatOutput => 0f;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                if (me.IsEquipped) 
                {
                    me.ShipStats.ReactorTempCurrent = 217f;
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
        class CombustionReactor : ReactorMod 
        {
            public override string Name => "Internal Fusion Reactor";

            public override string Description => "This strange reactor makes a decent amount of power, but it is able to make even more power the more unstable it is. Just be sure not to let stability reach 0%";

            public override int MarketPrice => 36000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 16000f;

            public override float MaxTemp => 3100f;

            public override float EmergencyCooldownTime => 10f;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                me.EnergyOutputMax = me.OriginalEnergyOutputMax * (1f + me.ShipStats.Ship.CoreInstability * 1.5f);
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

        [HarmonyPatch(typeof(PLSpaceHeatVolume),"Update")]
        class HeatOustide 
        {
            static void Postfix(PLSpaceHeatVolume __instance) 
            {
                if(__instance.GetComponentInParent<PLShipInfo>() != null && __instance.GetComponentInParent<PLShipInfo>().MyReactor != null && __instance.GetComponentInParent<PLShipInfo>().MyReactor.GetItemName() == "ThermoPoint Reactor" && __instance.NearbyShips.Contains(__instance.GetComponentInParent<PLShipInfo>()))
                {
                    __instance.NearbyShips.Remove(__instance.GetComponentInParent<PLShipInfo>());
                    if (__instance.GetComponentInParent<PLShipInfo>().Exterior.GetComponentInChildren<PLIceDust>() != null && __instance.GetComponentInParent<PLShipInfo>().Exterior.GetComponentInChildren<PLIceDust>().MyPS == __instance.MyPS)
                    {
                        __instance.GetComponentInParent<PLShipInfo>().Exterior.GetComponentInChildren<PLIceDust>().AttachedTo_Ship = __instance.GetComponentInParent<PLShipInfo>();
                    }
                }
            }
        }
        [HarmonyPatch(typeof(PLIceDust), "ShipWithinDust")]
        class NoCooling 
        {
            static void Postfix(PLIceDust __instance, ref bool __result) 
            {
                if(__instance.AttachedTo_Ship != null && __instance.AttachedTo_Ship.MyReactor != null && __instance.AttachedTo_Ship.MyReactor.GetItemName() == "ThermoPoint Reactor") 
                {
                    __result = false;
                }
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
