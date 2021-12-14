using PulsarModLoader.Content.Components.Shield;
using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
namespace Exotic_Components
{
    public class Shields
    {
        public class LayeredShieldMod : ShieldMod
        {
            public override string Name => "Layered Shield";

            public override string Description => "A special shield that overclocks whenever at low integrity, just be carefull with the higher power usage while at low integrity";

            public override int MarketPrice => 14000;

            public override bool Experimental => true;

            public override Texture2D IconTexture => (Texture2D)Resources.Load("Icons/43_Sheilds");

            public override float ShieldMax => 650f;

            public override float ChargeRateMax => 7f;

            public override float RecoveryRate => 3f;

            public override float Deflection => 2f;

            public override float MinIntegrityPercentForQuantumShield => 0.25f;

            public override float MaxPowerUsage_Watts => 8500f;

            public override int MinIntegrityAfterDamage => 380;

            public override void Tick(PLShipComponent InComp)
            {
                PLShieldGenerator me = InComp as PLShieldGenerator;
                if (me != null && me.ShipStats != null && me.ShipStats.Ship.MyShieldGenerator.Name == "Layered Shield")
                {
                    me.Deflection = (2.8f - Mathf.Clamp01(me.ShipStats.ShieldsCurrent / me.ShipStats.ShieldsMax)*4.2f);
                    if (me.Deflection < 0.6f) me.Deflection = 0.6f;
                    float multiplier = (1 / Mathf.Clamp01(me.ShipStats.ShieldsCurrent / me.ShipStats.ShieldsMax));
                    if (multiplier > 5f) multiplier = 5f;
                    me.CalculatedMaxPowerUsage_Watts = Mathf.RoundToInt(MaxPowerUsage_Watts * multiplier);
                }
            }
        }
        class EletricWall : ShieldMod
        {
            public override string Name => "Electric Wall";

            public override string Description => "A modified version of the Second Hull that not only has 10% resistance against energy attacks, it also can emit an EMP pulse if turned off while above 90% integrity, just be careful with the recoil. It was marked as contraband because of this.";

            public override int MarketPrice => 18000;

            public override bool Contraband => true;

            public override float ShieldMax => 1500f;

            public override float ChargeRateMax => 3f;

            public override float RecoveryRate => 1.2f;

            public override float MinIntegrityPercentForQuantumShield => 0.65f;

            public override float MaxPowerUsage_Watts => 10200f;

            public override int MinIntegrityAfterDamage => 550;
        }
        class FlagShipShield : ShieldMod
        {
            public override string Name => "Flagship shield";

            public override string Description => "This is the best shield ever made, planed for the third flagship, it will hold its own for a long time, while also keeping intruders away because of the size flagships have. Hope you have the money and the power for this.";

            public override int MarketPrice => 1400000;

            public override float ShieldMax => 40000f;

            public override float ChargeRateMax => 50f;

            public override float RecoveryRate => 20f;

            public override float MinIntegrityPercentForQuantumShield => 0.15f;

            public override float MaxPowerUsage_Watts => 180000f;

            public override int MinIntegrityAfterDamage => 20000;

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                InComp.ShipStats.Mass += 2500f;
            }
        }
        class GustavFrortress : ShieldMod 
        {
            public override string Name => "Gustav's Fortress";

            public override string Description => "This special shield was actually of my own making, it has a extreme charge rate and not too bad of power usage, but it is a little unstable, and requires constant power to stay online, but hey, the price is not the worst.";

            public override int MarketPrice => 18000;

            public override bool Experimental => true;

            public override float ShieldMax => 340f;

            public override float ChargeRateMax => 340f;

            public override float RecoveryRate => 13f;

            public override float MinIntegrityPercentForQuantumShield => 0.7f;

            public override float MaxPowerUsage_Watts => 8000f;

            public override int MinIntegrityAfterDamage => 100;

            public override void Tick(PLShipComponent InComp)
            {
                PLShieldGenerator me = InComp as PLShieldGenerator;
                me.IsPowerActive = true;
                me.RequestPowerUsage_Percent = 1f;
                me.ChargeRateCurrent = me.ChargeRateMax * me.LevelMultiplier(0.5f, 1f) * (me.GetPowerPercentInput() - 0.5f) * 2;
                if ((me.Current - me.ShipStats.ShieldsChargeRate * Time.deltaTime <= 0 && me.ChargeRateCurrent < 0) || (me.Current >= me.CurrentMax && me.ChargeRateCurrent > 0)) me.ShipStats.ShieldsChargeRate = 0f;
                if (me.Current < 0) me.Current = 0f;
            }
        }
        class AntiInfected : ShieldMod 
        {
            public override string Name => "Anti-Infected Shield";

            public override string Description => "This shield may not be one of the best, but it is able to resist the infected acid, allowing it to charge in infected space and resist their attack";

            public override int MarketPrice => 15000;

            public override bool Experimental => true;

            public override float ShieldMax => 290f;

            public override float ChargeRateMax => 12f;

            public override float RecoveryRate => 13f;

            public override float MinIntegrityPercentForQuantumShield => 0.6f;

            public override float MaxPowerUsage_Watts => 14000f;

            public override int MinIntegrityAfterDamage => 15;

            public override void AddStats(PLShipComponent InComp)
            {
                PLShieldGenerator me = InComp as PLShieldGenerator;
                PLShipStats inStats = me.ShipStats;
                if(PLEncounterManager.Instance.GetCPEI() != null && PLEncounterManager.Instance.GetCPEI().DisableShieldsInSector) 
                {
                    inStats.ShieldsChargeRate += me.ChargeRateCurrent;
                    inStats.ShieldsChargeRateMax += me.ChargeRateMax * me.LevelMultiplier(0.5f, 1f);
                }
            }

            public override void Tick(PLShipComponent InComp)
            {
                PLShieldGenerator me = InComp as PLShieldGenerator;
                if (PLEncounterManager.Instance.GetCPEI() != null && PLEncounterManager.Instance.GetCPEI().DisableShieldsInSector)
                {
                    me.Current += me.ShipStats.ShieldsChargeRate * Time.deltaTime;
                }
                me.ShipStats.ShieldsCurrent = me.Current;
            }
        }
        [HarmonyPatch(typeof(PLShieldGenerator), "Tick")]
        class ManualTick
        {
            static void Postfix(PLShieldGenerator __instance)
            {
                int subtypeformodded = __instance.SubType - ShieldModManager.Instance.VanillaShieldMaxType;
                if (subtypeformodded > -1 && subtypeformodded < ShieldModManager.Instance.ShieldTypes.Count && __instance.ShipStats != null)
                {
                    ShieldModManager.Instance.ShieldTypes[subtypeformodded].Tick(__instance);
                }
            }
        }
        [HarmonyPatch(typeof(PLShieldGenerator), "AddStats")]
        class ManualAddStats
        {
            static void Postfix(PLShieldGenerator __instance)
            {
                int subtypeformodded = __instance.SubType - ShieldModManager.Instance.VanillaShieldMaxType;
                if (subtypeformodded > -1 && subtypeformodded < ShieldModManager.Instance.ShieldTypes.Count && __instance.ShipStats != null)
                {
                    ShieldModManager.Instance.ShieldTypes[subtypeformodded].AddStats(__instance);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLShipStats), "TakeShieldDamage")]
    class ShieldDamage
    {
        static bool Prefix(float inDmg, EDamageType dmgType, float DT_ShieldBoost, float shieldDamageMod, PLTurret turret, ref float __result, PLShipStats __instance)
        {
            try
            {
                if (turret != null && (turret is AntiShield || turret is TweakedAntiShield) && (__instance.Ship.ShieldFreqMode != 0 || __instance.Ship.IsSensorWeaknessActive(ESensorWeakness.SHLD_WEAKPOINT)))
                {
                    __result = inDmg;
                    return false;
                }
                if (turret != null && turret is HullSmasher && (__instance.Ship.ShieldFreqMode != 1 || __instance.Ship.IsSensorWeaknessActive(ESensorWeakness.SHLD_WEAKPOINT)))
                {
                    __result = inDmg;
                    return false;
                }
                __instance.GetComponentsOfType(ESlotType.E_COMP_SHLD, false);
                PLShieldGenerator shipComponent = __instance.GetShipComponent<PLShieldGenerator>(ESlotType.E_COMP_SHLD, false);
                if (shipComponent == null || inDmg <= 0f)
                {
                    __result = 0f;
                    return false;
                }
                if (dmgType == EDamageType.E_INFECTED && inDmg == 10 && shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("Anti-Infected Shield")) 
                {
                    __result = 0f;
                    return false;
                }
                if ((float)shipComponent.MinIntegrityToCreateBubble <= shipComponent.Current)
                {
                    shipComponent.MinIntegrityToCreateBubble = 0;
                    float num = shipComponent.Current;
                    float num2 = 1f;
                    if (dmgType == EDamageType.E_BEAM)
                    {
                        if (shipComponent.SubType == 9)
                        {
                            num2 = 1.1764705f;
                        }
                        if (shipComponent.SubType == 12)
                        {
                            num2 = 1.4285715f;
                        }
                        if (shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("Eletric Wall"))
                        {
                            num2 = 1.1145238f;
                        }
                    }
                    else if (PLShipInfoBase.IsDamageTypePhysical(dmgType))
                    {
                        if (shipComponent.SubType == 7)
                        {
                            num2 = 1.0526316f;
                        }
                        if (shipComponent.SubType == 8)
                        {
                            num2 = 1.1111112f;
                        }
                        if (shipComponent.SubType == 15)
                        {
                            num2 = 1.25f;
                        }
                    }
                    else if (shipComponent.SubType == 16)
                    {
                        num2 = 1.1764705f;
                    }
                    if (turret != null && (turret is HullSmasher || turret is AntiShield || turret is TweakedAntiShield))
                    {
                        num2 += 0.3f;
                    }
                    if(dmgType == EDamageType.E_PHYSICAL && shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("Anti-Infected Shield") && turret != null && turret is PLSporeTurret) 
                    {
                        num2 = 1.5f;
                        inDmg *= 0.1f;
                        DT_ShieldBoost = 1f;
                    }
                    num2 *= 1f / DT_ShieldBoost * (1f / shieldDamageMod);
                    num2 += (__instance.ShieldsDeflection - (0.6f * __instance.Ship.MyShieldGenerator.LevelMultiplier(0.1f, 1f))) * 1.5f;
                    float num3 = Mathf.Min(inDmg, shipComponent.Current * num2);
                    float num4 = num3 / num2;
                    shipComponent.Current -= num4;
                    __instance.ShieldsCurrent -= num4;
                    __instance.Ship.VisibleDamage += num4;
                    __instance.Ship.LerpedShieldBaseIllum += num4 * 0.02f;
                    if (__instance.Ship.IsTestShip)
                    {
                        string text = "None";
                        if (turret != null)
                        {
                            text = turret.GetItemName(false);
                        }
                        if (!__instance.Ship.TestDPSDamageByTurretName.ContainsKey(text))
                        {
                            __instance.Ship.TestDPSDamageByTurretName[text] = 0f;
                        }
                        Dictionary<string, float> testDPSDamageByTurretName = __instance.Ship.TestDPSDamageByTurretName;
                        string key = text;
                        testDPSDamageByTurretName[key] += num4;
                    }
                    float value = inDmg - num3;
                    if (shipComponent.Current <= 0f)
                    {
                        shipComponent.MinIntegrityToCreateBubble = shipComponent.MinIntegrityAfterDamage;
                    }
                    __result = Mathf.Clamp(value, 0f, float.MaxValue);
                    return false;
                }
                __result = inDmg;
                return false;
            }
            catch
            {
                return true;
            }
        }
    }



    [HarmonyPatch(typeof(PLServer), "SetStartupSwitchStatus")]
    class EMPPulse 
    {
        static void Postfix(int shipID, bool status) 
        {
            PLShipStats mystats = PLEncounterManager.Instance.GetShipFromID(shipID).MyStats;
            if (PLEncounterManager.Instance.GetShipFromID(shipID).MyShieldGenerator.Name == "Eletric Wall" && !status &&  mystats.ShieldsCurrent/mystats.ShieldsMax >= 0.9f)
            {
                Object.Instantiate(PLGlobal.Instance.EMPExplosionPrefab, PLEncounterManager.Instance.PlayerShip.Exterior.transform.position, Quaternion.identity);
                if (!PhotonNetwork.isMasterClient) return;
                foreach (PLShipInfoBase ship in Object.FindObjectsOfType(typeof(PLShipInfoBase)))
                {
                    if (!ship.GetIsPlayerShip()) ship.photonView.RPC("Overcharged", PhotonTargets.All, new object[0]);
                }
                PLEncounterManager.Instance.PlayerShip.EngineeringSystem.TakeDamage(Random.Range(0, 20));
                PLEncounterManager.Instance.PlayerShip.ComputerSystem.TakeDamage(Random.Range(0, 20));
                PLEncounterManager.Instance.PlayerShip.WeaponsSystem.TakeDamage(Random.Range(0, 20));
                PLEncounterManager.Instance.PlayerShip.LifeSupportSystem.TakeDamage(Random.Range(0, 20));
                PLEncounterManager.Instance.GetShipFromID(shipID).MyShieldGenerator.Current = 0f;
                if (Random.Range(0,20) == 4) PLEncounterManager.Instance.PlayerShip.photonView.RPC("Overcharged", PhotonTargets.All, new object[0]);
            }
        }
    }
}

