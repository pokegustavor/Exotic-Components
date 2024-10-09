using PulsarModLoader.Content.Components.Reactor;
using PulsarModLoader.Content.Components.Shield;
using PulsarModLoader.Content.Components.Hull;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using static UILabel;
using PulsarModLoader.Content.Components.CPU;
namespace Exotic_Components
{
    public class Reactors
    {
        static public bool CollectedColor = false;
        static public Color lightColor = Color.black;
        static public Color particleColor = Color.black;
        static public float BaseHeat = 5f;
        static public float MaxCool = 11f;
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

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                PLReactor reactor = InComp as PLReactor;
                float multiplier = Mathf.Clamp(reactor.ShipStats.ReactorTempCurrent * 2f / reactor.ShipStats.ReactorTempMax, 1f, 1.5f);
                reactor.ShipStats.ThrustOutputCurrent *= multiplier;
                reactor.ShipStats.ThrustOutputMax *= multiplier;
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
        }
        internal class BiscuitReactor : ReactorMod
        {
            public static float BiscuitBoost = 0;

            public static Dictionary<int, float> effects = new Dictionary<int, float>();

            public static float LastHeal = Time.time;
            public override string Name => "Ultimate Fluffy Biscuit Reactor";

            public override string Description => "An upgraded Fluffy Biscuit Jumbo Reactor that can gain more power if you atomize some biscuits, and the biscuit effects will apply for 30 seconds to your ship. Does this make you question what they put in those biscuits?";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 13600f;

            public override float EnergySignatureAmount => 4f;

            public override float MaxTemp => 2100f;

            public override float EmergencyCooldownTime => 3f;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                me.EnergyOutputMax = me.OriginalEnergyOutputMax * ((BiscuitBoost / 5f) + 1);
            }

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                List<int> deleteList = new List<int>();
                foreach (int key in effects.Keys)
                {
                    if (Time.time - effects[key] > 30)
                    {
                        deleteList.Add(key);
                    }
                }
                foreach (int key in deleteList)
                {
                    effects.Remove(key);
                }

                if (effects.ContainsKey((int)EPawnStatusEffectType.SPICY) || effects.ContainsKey((int)EPawnStatusEffectType.SPICY_TEAM))
                {
                    InComp.ShipStats.TurretDamageFactor *= 1.4f;
                }

                if (effects.ContainsKey((int)EPawnStatusEffectType.GUN_COOLING))
                {
                    InComp.ShipStats.TurretCoolingSpeedFactor *= 1.4f;
                }

                if (effects.ContainsKey((int)EPawnStatusEffectType.EMERGENCY))
                {
                    InComp.ShipStats.HullArmor *= 1.4f;
                }

                if (effects.ContainsKey((int)EPawnStatusEffectType.ACCURACY))
                {
                    InComp.ShipStats.ThrustOutputCurrent *= 1.4f;
                    InComp.ShipStats.ThrustOutputMax *= 1.4f;
                    InComp.ShipStats.InertiaThrustOutputCurrent *= 1.4f;
                    InComp.ShipStats.InertiaThrustOutputMax *= 1.4f;
                    InComp.ShipStats.ManeuverThrustOutputCurrent *= 1.4f;
                    InComp.ShipStats.ManeuverThrustOutputMax *= 1.4f;
                }

                if ((Time.time - LastHeal > 1) && (effects.ContainsKey((int)EPawnStatusEffectType.HEARTY) || effects.ContainsKey((int)EPawnStatusEffectType.HEALTH_REGEN) || effects.ContainsKey((int)EPawnStatusEffectType.HEAL_TEAM)))
                {
                    LastHeal = Time.time;
                    foreach (PLPlayer player in PLServer.Instance.AllPlayers)
                    {
                        if (player.TeamID == 0 && player.GetPawn() != null && !player.GetPawn().IsDead && player.GetPawn().MyCurrentTLI == InComp.ShipStats.Ship.MyTLI)
                        {
                            player.GetPawn().Health += 5;
                            player.GetPawn().Health = Mathf.Min(player.GetPawn().Health, player.GetPawn().MaxHealth);
                        }
                    }
                }
            }

            public override void OnWarp(PLShipComponent InComp)
            {
                if (BiscuitBoost <= 20)
                {
                    BiscuitBoost -= 5;
                }
                else
                {
                    BiscuitBoost *= 0.8f;
                }
                if (BiscuitBoost < 0) { BiscuitBoost = 0; }
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
                me.HeatOutput = Mathf.Clamp(me.ShipStats.HullCurrent / me.ShipStats.HullMax * 1.5f, 0.65f, 1f);
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
                if (!InComp.IsEquipped || InComp.ShipStats.ReactorTempCurrent / InComp.ShipStats.ReactorTempMax < 0.7f)
                {
                    heatVolume.MyPS.enableEmission = false;
                    ParticleSystem.ShapeModule shape = heatVolume.MyPS.shape;
                    shape.radius = 0.0001f;
                    shape.scale = new Vector3(0, 0, 0);
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
        }
        class UnstableReactor : ReactorMod
        {
            public override string Name => "Overclocked ZeroPoint Reactor";

            public override string Description => "Unlike the previous version, this reactor makes way more power, but not using the power causes heat to increase, SO ONLY BUY IF YOU CAN KEEP IT BUSY.";

            public override int MarketPrice => 31000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 44000f;

            public override float EnergySignatureAmount => 7f;

            public override float MaxTemp => 4100f;

            public override float EmergencyCooldownTime => 15f;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                if (me.IsEquipped)
                {
                    me.HeatOutput = Reactors.BaseHeat - me.ShipStats.ReactorTotalUsagePercent * Reactors.MaxCool;
                    if (me.ShipStats.ReactorTotalUsagePercent < 10 && !me.ShipStats.Ship.IsReactorOverheated() && !me.ShipStats.Ship.IsAbandoned()) me.ShipStats.ReactorTempCurrent += 200 * Time.deltaTime;
                }
            }
        }
        class CheapReactor : ReactorMod
        {
            public override string Name => "Mini Fusion Reactor";

            public override string Description => "I will be honest with you, I got scammed with this reactor, but if you are some kind of cheap bastard, go ahead and buy it, hope you like suffering";

            public override int MarketPrice => 5000;

            public override float EnergyOutputMax => 7500f;

            public override float MaxTemp => 1200f;

            public override float EmergencyCooldownTime => 3f;

            public override float EnergySignatureAmount => 3f;
        }
        class FlagShipReactor : ReactorMod
        {
            public override string Name => "Flagship Reactor";

            public override string Description => "This reactor was supposed to be used in a third Flagship, but with some contacts and favors, I got it from the W.D. Factory that created it. I will say, won't be cheap to install it in your ship";

            public override int MarketPrice => 1500000;

            public override float EnergyOutputMax => 517691f;

            public override float MaxTemp => 12007f;

            public override float EmergencyCooldownTime => 3600f;

            public override float EnergySignatureAmount => 500f;

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                InComp.ShipStats.Mass += 2760f;
            }
        }
        class InfectedReactor : ReactorMod
        {
            public override string Name => "Infected Reactor";

            public override string Description => "This reactor uses infected parts to generate power, but it does damage your ship hull a little every jump so be carefull. I don't know why it works, but I found out while using an overcharge processor in an infected sector.";

            public override int MarketPrice => 25000;

            public override float EnergyOutputMax => 21000f;

            public override bool Contraband => true;

            public override float MaxTemp => 3800f;

            public override float EmergencyCooldownTime => 10f;

            public override float EnergySignatureAmount => 13f;

            public override void OnWarp(PLShipComponent InComp)
            {
                if (InComp.IsEquipped && InComp.ShipStats != null)
                {
                    InComp.ShipStats.TakeHullDamage(InComp.ShipStats.HullMax * 0.05f, EDamageType.E_INFECTED, null, null);
                }
            }
            public override void Tick(PLShipComponent InComp)
            {
                if (InComp.IsEquipped && InComp.ShipStats != null && (InComp.ShipStats.Ship is PLShipInfo) && (InComp.ShipStats.Ship as PLShipInfo).ReactorInstance != null)
                {
                    foreach (Light light in (InComp.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentsInChildren<Light>())
                    {
                        light.color = Color.red;
                    }
                    foreach (ParticleSystem particle in (InComp.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        particle.startColor = Color.red;
                    }
                }
            }
        }
        class StarReactor : ReactorMod
        {
            public override string Name => "Micro Star Reactor";

            public override string Description => "This small star will produce you more power the hoter it is (DON'T YOU SAY THIS IS JUST A THERMOCORE!)";

            public override int MarketPrice => 39000;

            public override float EnergyOutputMax => 14000f;

            public override bool Experimental => true;

            public override float MaxTemp => 6800f;

            public override float EmergencyCooldownTime => 10f;

            public override float EnergySignatureAmount => 13f;

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLReactor me = InComp as PLReactor;
                return string.Concat(new string[]
                {
                    (me.TempMax * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
                    " kP\n",
                    me.EmergencyCooldownTime.ToString("0.0"),
                    " sec\n",
                    (22000f * me.LevelMultiplier(0.1f, 1f)).ToString("0"),
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
                    PLLocalize.Localize("Max Output", false),
                });
            }
            public override void Tick(PLShipComponent InComp)
            {
                if (InComp.IsEquipped && InComp.ShipStats != null && (InComp.ShipStats.Ship is PLShipInfo) && (InComp.ShipStats.Ship as PLShipInfo).ReactorInstance != null)
                {
                    PLReactor reactor = InComp as PLReactor;
                    float Percentage = InComp.ShipStats.ReactorTempCurrent / InComp.ShipStats.ReactorTempMax;
                    Color target = Color.white;
                    if (Percentage < 0.2f)
                    {
                        target = Color.red;
                        reactor.OriginalEnergyOutputMax = 14000f;
                    }
                    else if (Percentage < 0.4f)
                    {
                        target = new Color(0.9686f, 0.5843f, 0f, 0.91f);
                        reactor.OriginalEnergyOutputMax = 16000f;
                    }
                    else if (Percentage < 0.6)
                    {
                        target = Color.yellow;
                        reactor.OriginalEnergyOutputMax = 18000f;
                    }
                    else if (Percentage < 0.8)
                    {
                        target = Color.white;
                        reactor.OriginalEnergyOutputMax = 20000f;
                    }
                    else
                    {
                        target = Color.blue;
                        reactor.OriginalEnergyOutputMax = 22000f;
                    }
                    reactor.EnergyOutputMax = reactor.OriginalEnergyOutputMax * reactor.LevelMultiplier(0.1f, 1f);
                    foreach (Light light in (InComp.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentsInChildren<Light>())
                    {
                        light.color = target;
                    }
                    foreach (ParticleSystem particle in (InComp.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        particle.startColor = target;
                    }
                }
            }
        }
        class OverStrongReactor : ReactorMod
        {
            public override string Name => "Modified Strongpoint Reactor";

            public override string Description => "This Strongpoint reactor has been modified to not lose power when systems are damaged and has even more power, however it now may... explode if you don't take care of your systems";

            public override int MarketPrice => 27000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 36000f;

            public override float EnergySignatureAmount => 12f;

            public override float MaxTemp => 4000f;

            public override float EmergencyCooldownTime => 10f;

            public override void Tick(PLShipComponent InComp)
            {
                if (InComp.IsEquipped)
                {
                    float num = 0f;
                    int num2 = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        PLMainSystem systemFromID = InComp.ShipStats.Ship.GetSystemFromID(i);
                        if (systemFromID != null)
                        {
                            num += systemFromID.GetHealthRatio();
                            num2++;
                        }
                    }
                    if (num2 != 0)
                    {
                        num /= (float)num2;
                    }
                    else
                    {
                        num = 1f;
                    }

                    InComp.ShipStats.Ship.CoreInstability += Time.deltaTime * 0.2f * (1 - num);
                }
            }
        }
        class OverchargeReactor : ReactorMod
        {
            public override string Name => "The Overcharge";

            public override string Description => "This reactor was designed to not only get improved stats based on your ship overcharge, but it will also NEVER suffer an overchage and shutdown!";

            public override int MarketPrice => 32000;

            public override bool Experimental => true;

            public override float EnergyOutputMax => 22000;

            public override float EnergySignatureAmount => 9f;

            public override float MaxTemp => 3000;

            public override void Tick(PLShipComponent InComp)
            {
                PLReactor me = (PLReactor)InComp;
                if (me.IsEquipped)
                {
                    me.EnergyOutputMax = 22000 * (1 + me.ShipStats.Ship.DischargeAmount);
                }
            }
        }
        [HarmonyPatch(typeof(PLSpaceHeatVolume), "Update")]
        class HeatOustide
        {
            static void Postfix(PLSpaceHeatVolume __instance)
            {
                if (__instance.GetComponentInParent<PLShipInfo>() != null && __instance.GetComponentInParent<PLShipInfo>().MyReactor != null && __instance.GetComponentInParent<PLShipInfo>().MyReactor.GetItemName() == "ThermoPoint Reactor" && __instance.NearbyShips.Contains(__instance.GetComponentInParent<PLShipInfo>()))
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
                if (__instance.AttachedTo_Ship != null && __instance.AttachedTo_Ship.MyReactor != null && __instance.AttachedTo_Ship.MyReactor.GetItemName() == "ThermoPoint Reactor")
                {
                    __result = false;
                }
            }
        }
        [HarmonyPatch(typeof(PLShipComponent), "FinalLateAddStats")]
        class AddStatsFix
        {
            static void Postfix(PLShipComponent __instance)
            {

                switch (__instance.ActualSlotType)
                {
                    case ESlotType.E_COMP_REACTOR:
                        int subtypeformodded = __instance.SubType - ReactorModManager.Instance.VanillaReactorMaxType;
                        if (subtypeformodded > -1 && subtypeformodded < ReactorModManager.Instance.ReactorTypes.Count && __instance.ShipStats != null)
                        {
                            ReactorModManager.Instance.ReactorTypes[subtypeformodded].FinalLateAddStats(__instance);
                        }
                        break;
                    case ESlotType.E_COMP_SHLD:
                        int subtypeformoddeds = __instance.SubType - ShieldModManager.Instance.VanillaShieldMaxType;
                        if (subtypeformoddeds > -1 && subtypeformoddeds < ShieldModManager.Instance.ShieldTypes.Count && __instance.ShipStats != null)
                        {
                            ShieldModManager.Instance.ShieldTypes[subtypeformoddeds].FinalLateAddStats(__instance);
                        }
                        break;
                    case ESlotType.E_COMP_HULL:
                        int subtypeformoddedh = __instance.SubType - HullModManager.Instance.VanillaHullMaxType;
                        if (subtypeformoddedh > -1 && subtypeformoddedh < HullModManager.Instance.HullTypes.Count && __instance.ShipStats != null)
                        {
                            HullModManager.Instance.HullTypes[subtypeformoddedh].FinalLateAddStats(__instance);
                        }
                        break;
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
                    if (plpawnItem.GetItemName(true).Contains("Biscuit") && PLEncounterManager.Instance.PlayerShip != null && PLEncounterManager.Instance.PlayerShip.MyReactor != null && PLEncounterManager.Instance.PlayerShip.MyReactor.Name == "Ultimate Fluffy Biscuit Reactor")
                    {
                        BiscuitReactor.BiscuitBoost += 1;
                        EFoodType efoodType = (EFoodType)plpawnItem.SubType;
                        if (efoodType == EFoodType.FUNKY_BISCUIT)
                        {
                            switch ((plpawnItem.NetID + PLServer.Instance.GalaxySeed) % 10)
                            {
                                default:
                                    efoodType = EFoodType.ACCURACY_BISCUIT;
                                    break;
                                case 1:
                                    efoodType = EFoodType.EMERGENCY_BISCUIT;
                                    break;
                                case 2:
                                    efoodType = EFoodType.HEALTH_REGEN_BISCUIT;
                                    break;
                                case 3:
                                    efoodType = EFoodType.HEARTY_BISCUIT;
                                    break;
                                case 4:
                                    efoodType = EFoodType.OXYGEN_BISCUIT;
                                    break;
                                case 5:
                                    efoodType = EFoodType.REVIVAL_BISCUIT;
                                    break;
                                case 6:
                                    efoodType = EFoodType.SPICY_BISCUIT;
                                    break;
                                case 7:
                                    efoodType = EFoodType.MOLTEN_BISCUIT;
                                    break;
                                case 8:
                                    efoodType = EFoodType.HEALBERRY_CRUMBLE_BISCUIT;
                                    break;
                                case 9:
                                    efoodType = EFoodType.SPICY_CRUMBLE_BISCUIT;
                                    break;
                            }
                        }
                        EPawnStatusEffectType biscuitType;
                        switch (efoodType)
                        {
                            default:
                            case EFoodType.REVIVAL_BISCUIT:
                                biscuitType = EPawnStatusEffectType.REVIVAL;
                                break;
                            case EFoodType.EMERGENCY_BISCUIT:
                                biscuitType = EPawnStatusEffectType.EMERGENCY;
                                break;
                            case EFoodType.HEARTY_BISCUIT:
                                biscuitType = EPawnStatusEffectType.HEARTY;
                                break;
                            case EFoodType.HEALTH_REGEN_BISCUIT:
                                biscuitType = EPawnStatusEffectType.HEALTH_REGEN;
                                break;
                            case EFoodType.ACCURACY_BISCUIT:
                                biscuitType = EPawnStatusEffectType.ACCURACY;
                                break;
                            case EFoodType.HEALBERRY_CRUMBLE_BISCUIT:
                                biscuitType = EPawnStatusEffectType.HEAL_TEAM;
                                break;
                            case EFoodType.GARLIC_BISCUIT:
                                biscuitType = EPawnStatusEffectType.LIFESTEAL;
                                break;
                            case EFoodType.FROZEN_BISCUIT:
                                biscuitType = EPawnStatusEffectType.GUN_COOLING;
                                break;
                            case EFoodType.LUCKY_BISCUIT:
                                biscuitType = EPawnStatusEffectType.LUCKY;
                                break;
                            case EFoodType.SPICY_BISCUIT:
                                biscuitType = EPawnStatusEffectType.SPICY;
                                break;
                            case EFoodType.SPICY_CRUMBLE_BISCUIT:
                                biscuitType = EPawnStatusEffectType.SPICY_TEAM;
                                break;
                            case EFoodType.MOLTEN_BISCUIT:
                                biscuitType = EPawnStatusEffectType.MOLTEN;
                                break;
                        }
                        int TypeID = (int)biscuitType;
                        if (BiscuitReactor.effects.ContainsKey(TypeID))
                        {
                            BiscuitReactor.effects[TypeID] = Time.time;
                        }
                        else
                        {
                            BiscuitReactor.effects.Add(TypeID, Time.time);
                        }
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
        [HarmonyPatch(typeof(PLReactor), "Tick")]
        class ManualTick
        {
            static void Postfix(PLReactor __instance)
            {
                if (!CollectedColor && __instance.ShipStats != null && (__instance.ShipStats.Ship is PLShipInfo) && (__instance.ShipStats.Ship as PLShipInfo).ReactorInstance != null)
                {
                    lightColor = (__instance.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentInChildren<Light>().color;
                    particleColor = (__instance.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentInChildren<ParticleSystem>().startColor;
                    CollectedColor = true;
                }
                else if (__instance.ShipStats != null && (__instance.ShipStats.Ship is PLShipInfo) && (__instance.ShipStats.Ship as PLShipInfo).ReactorInstance != null)
                {
                    foreach (Light light in (__instance.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentsInChildren<Light>())
                    {
                        light.color = lightColor;
                    }
                    foreach (ParticleSystem particle in (__instance.ShipStats.Ship as PLShipInfo).ReactorInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        particle.startColor = particleColor;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(PLShipInfoBase), "Update")]
        class PreventOvercharge
        {
            static void Prefix(PLShipInfoBase __instance)
            {
                if (__instance.MyReactor != null && __instance.MyReactor.Name == "The Overcharge" && __instance.DischargeAmount >= 0.999f)
                {
                    __instance.DischargeAmount = 0.98f;
                }
            }
        }
        [HarmonyPatch(typeof(PLSpaceScrap), "OnCollect")]
        class IncreaseOdds 
        {
            static bool Prefix(PLSpaceScrap __instance) 
            {
                if (BiscuitReactor.effects.ContainsKey((int)EPawnStatusEffectType.LUCKY)) 
                {
                    if (!__instance.Collected)
                    {
                        PLShipInfo plshipInfo = PLEncounterManager.Instance.PlayerShip;
                        if (PLAcademyShipInfo.Instance != null)
                        {
                            plshipInfo = PLAcademyShipInfo.Instance;
                        }
                        if (PhotonNetwork.isMasterClient && plshipInfo != null && PLServer.Instance != null)
                        {
                            PLSlot slot = plshipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO);
                            if (slot != null && (slot.Count < slot.MaxItems || plshipInfo.ShipTypeID == EShipType.E_ACADEMY))
                            {
                                __instance.Collected = true;
                                PLServer.Instance.photonView.RPC("ScrapCollectedEffect", PhotonTargets.All, new object[]
                                {
                                    __instance.transform.position
                                });
                                if (plshipInfo.ShipTypeID == EShipType.E_ACADEMY)
                                {
                                    return false;
                                }
                                PLServer.Instance.photonView.RPC("ScrapLateCollected", PhotonTargets.All, new object[]
                                {
                                    __instance.EncounterNetID
                                });
                                if (__instance.IsSpecificComponentScrap)
                                {
                                    plshipInfo.MyStats.AddShipComponent(PLWare.CreateFromHash(1, __instance.SpecificComponent_CompHash) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
                                    return false;
                                }
                                PLRand plrand = new PLRand(PLServer.Instance.GalaxySeed + PLServer.Instance.GetCurrentHubID() + __instance.m_EncounterNetID);
                                int inLevel = 0;
                                if (__instance.CanGiveComponent)
                                {
                                    int num = plrand.Next(0, 100);
                                    if (PLEncounterManager.Instance.PlayerShip.ShipTypeID == EShipType.E_CARRIER)
                                    {
                                        num = plrand.Next(0, 50);
                                    }
                                    Mathf.RoundToInt(Mathf.Pow(plrand.Next(0f, 1f), 4f) * PLServer.Instance.ChaosLevel);
                                    PLShipComponent plshipComponent = null;
                                    if (num < 50 && __instance.SpecificComponent_CompHash != -1)
                                    {
                                        plshipComponent = PLShipComponent.CreateShipComponentFromHash(__instance.SpecificComponent_CompHash, null);
                                    }
                                    if (plshipComponent == null)
                                    {
                                        plshipComponent = new PLScrapCargo(inLevel);
                                    }
                                    plshipInfo.MyStats.AddShipComponent(plshipComponent, -1, ESlotType.E_COMP_CARGO);
                                    return false;
                                }
                                plshipInfo.MyStats.AddShipComponent(new PLScrapCargo(inLevel), -1, ESlotType.E_COMP_CARGO);
                            }
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
