using PulsarModLoader.Content.Components.Shield;
using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
using PulsarModLoader;
using static UnityEngine.TouchScreenKeyboard;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
                if (me != null && me.ShipStats != null && me.ShipStats.Ship.MyShieldGenerator != null && me.ShipStats.Ship.MyShieldGenerator.Name == "Layered Shield")
                {
                    float multiplier = (1 / Mathf.Clamp01(me.ShipStats.ShieldsCurrent / me.ShipStats.ShieldsMax));
                    if (multiplier > 5f) multiplier = 5f;
                    me.CalculatedMaxPowerUsage_Watts = Mathf.RoundToInt(MaxPowerUsage_Watts * multiplier);
                }
            }
        }
        class EletricWall : ShieldMod
        {
            public override string Name => "Electric Wall";

            public override string Description => "A modified version of the Second Hull that not only has 10% resistance against energy attacks, it also allows your pilot to emit an EMP pulse if the integrity is above 90%, just be careful with the recoil. It was marked as contraband because of this.";

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
            public override string Name => "Flagship Shield";

            public override string Description => "This is the best shield ever made, planned for the third flagship, it will hold its own for a long time, while also keeping intruders away because of the size flagships have. Hope you have the money and the power for this.";

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

            public override float ChargeRateMax => 300f;

            public override float RecoveryRate => 13f;

            public override float MinIntegrityPercentForQuantumShield => 0.7f;

            public override float MaxPowerUsage_Watts => 8000f;

            public override int MinIntegrityAfterDamage => 100;

            public override void Tick(PLShipComponent InComp)
            {
                PLShieldGenerator me = InComp as PLShieldGenerator;
                me.IsPowerActive = true;
                me.RequestPowerUsage_Percent = 1f;
                me.CalculatedMaxPowerUsage_Watts = 8000f * me.LevelMultiplier(0.2f, 1f);
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
                if (PLEncounterManager.Instance.GetCPEI() != null && PLEncounterManager.Instance.GetCPEI().DisableShieldsInSector)
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
        class RegenerationShield : ShieldMod
        {
            public override string Name => "Extreme Particle Shield";

            public override string Description => "This shield has a very special design that allows for extremely quick reboot rate and low energy usage, at the cost of no recharge rate and low durability";

            public override int MarketPrice => 12000;

            public override bool Experimental => true;

            public override float ShieldMax => 320f;

            public override float ChargeRateMax => 0;

            public override float RecoveryRate => 100;

            public override float MinIntegrityPercentForQuantumShield => 0.15f;

            public override float MaxPowerUsage_Watts => 3200f;

            public override int MinIntegrityAfterDamage => 320;

            public override void Tick(PLShipComponent InComp)
            {
                base.Tick(InComp);
                PLShieldGenerator shield = InComp as PLShieldGenerator;
                shield.MinIntegrityAfterDamage = Mathf.RoundToInt(shield.CurrentMax) - 5;
                shield.MinIntegrityAfterDamageScaled = shield.MinIntegrityAfterDamage;
                shield.RecoveryRate = shield.CurrentMax / 3;
            }

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                base.FinalLateAddStats(InComp);
                PLShipInfoBase ship = InComp.ShipStats.Ship;
                if (ship != null)
                {
                    if (ship.ShieldIsActive)
                    {
                        InComp.ShipStats.ShieldsChargeRateMax = 0;
                        InComp.ShipStats.ShieldsChargeRate = 0;
                    }
                    else
                    {
                        InComp.ShipStats.ShieldsChargeRateMax += (InComp as PLShieldGenerator).RecoveryRate;
                        InComp.ShipStats.ShieldsChargeRate += (InComp as PLShieldGenerator).RecoveryRate * (InComp as PLShieldGenerator).GetPowerPercentInput();
                    }
                }
            }
        }
        class ImunityShield : ShieldMod
        {
            public override string Name => "Quantum Shield";

            public override string Description => "A shield made with top of the line technology, this shield will allow complete immunity to damage based on the shield frequency, however the counterpart damage will be multiplied in strengh. Your scientist will surely enjoy some extra work";

            public override int MarketPrice => 53000;

            public override bool Experimental => true;

            public override float ShieldMax => 1600;

            public override float ChargeRateMax => 7;

            public override float MinIntegrityPercentForQuantumShield => 0.70f;

            public override float MaxPowerUsage_Watts => 18000;

            public override int MinIntegrityAfterDamage => 1500;
        }
        class AbsortionShield : ShieldMod
        {
            public override string Name => "The Absortion Field";

            public override string Description => "This \"Shield Generator\" doesn't actually block incoming damage, instead using a special system connected with the ship reactor, this field will absorve up to 75% of the incoming damage, but for that a percentage of your reactor power is needed, also it doesn't have quantum shields!";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override Texture2D IconTexture => (Texture2D)Resources.Load("Icons/41_Sheilds");

            public override float ShieldMax => 1;

            public override float ChargeRateMax => 1;

            public override float MinIntegrityPercentForQuantumShield => 2f;

            public override float MaxPowerUsage_Watts => 0;

            public override int MinIntegrityAfterDamage => 0;

            public override float Price_LevelMultiplierExponent => 1.6f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                    PLLocalize.Localize("Damage Reduction", false),
                    "\n",
                    PLLocalize.Localize("Reactor Power Percent: ", false)
                });
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                    (InComp as PLShieldGenerator).IsEquipped ? ((InComp as PLShieldGenerator).GetPowerPercentInput() * 75).ToString("N1") + "%" : "?%" ,
                    "\n",
                    $"{50-InComp.Level}%"
                });
            }

            public override void Tick(PLShipComponent InComp)
            {
                base.Tick(InComp);
                PLShieldGenerator shieldGenerator = InComp as PLShieldGenerator;
                if (shieldGenerator.IsEquipped)
                {
                    shieldGenerator.IsPowerActive = true;
                    shieldGenerator.m_MaxPowerUsage_Watts = Mathf.Max(Mathf.Round(shieldGenerator.ShipStats.ReactorBoostedOutputMax * (0.5f - 0.01f * shieldGenerator.Level)), Mathf.Round(shieldGenerator.ShipStats.ReactorBoostedOutputMax * 0.25f));
                    shieldGenerator.RequestPowerUsage_Percent = 1f;
                    shieldGenerator.MinIntegrityForBubble = 0f;
                    shieldGenerator.MinIntegrityToCreateBubble = 0;
                    float Power = shieldGenerator.GetPowerPercentInput();
                    shieldGenerator.ShipStats.Ship.IsQuantumShieldActive = false;
                }
            }

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                base.FinalLateAddStats(InComp);
                InComp.ShipStats.ShieldsMax = 1;
                InComp.ShipStats.ShieldsCurrent = 1;
                InComp.ShipStats.ShieldsChargeRate = InComp.ShipStats.ShieldsChargeRateMax;
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
        static bool Prefix(ref float inDmg, EDamageType dmgType, float DT_ShieldBoost, float shieldDamageMod, PLTurret turret, ref float __result, PLShipStats __instance)
        {
            if (turret != null && turret is PhaseShieldTurret)
            {
                __result = inDmg;
                return false;
            }
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
            if (turret != null && (turret is HullSmasher || turret is AntiShield || turret is TweakedAntiShield))
            {
                inDmg *= 0.7f;
            }
            if (!PLShipInfoBase.IsDamageTypePhysical(dmgType) && shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("Electric Wall"))
            {
                inDmg *= 0.9f;
            }
            else if (shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("Layered Shield"))
            {
                inDmg *= Mathf.Max(0.2f, shipComponent.Current / shipComponent.CurrentMax);
            }
            else if (dmgType == EDamageType.E_PHYSICAL && shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("Anti-Infected Shield") && turret != null && turret is PLSporeTurret)
            {
                inDmg *= 0.2f;
            }
            else if (shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("Quantum Shield"))
            {
                bool isPhysical = PLShipInfoBase.IsDamageTypePhysical(dmgType);
                if ((__instance.Ship.ShieldFreqMode == 0 && !isPhysical) || (__instance.Ship.ShieldFreqMode == 1 && isPhysical))
                {
                    __result = 0;
                    return false;
                }
                else
                {
                    inDmg *= 3;
                    return true;
                }
            }
            else if (shipComponent.SubType == ShieldModManager.Instance.GetShieldIDFromName("The Absortion Field"))
            {
                __result = inDmg - inDmg * (shipComponent.GetPowerPercentInput() * 0.75f);
                return false;
            }
            return true;
        }
    }

    class EMPPulse : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            int shipID = (int)arguments[0];
            PLShipStats mystats = PLEncounterManager.Instance.GetShipFromID(shipID).MyStats;
            PLShipInfo Targetship = PLEncounterManager.Instance.GetShipFromID(shipID) as PLShipInfo;
            Object.Instantiate(PLGlobal.Instance.EMPExplosionPrefab, Targetship.Exterior.transform.position, Quaternion.identity);
            if (PhotonNetwork.isMasterClient)
            {
                foreach (PLShipInfoBase ship in Object.FindObjectsOfType(typeof(PLShipInfoBase)))
                {
                    if (ship != Targetship) ship.photonView.RPC("Overcharged", PhotonTargets.All, new object[0]);
                }
            }
            Targetship.EngineeringSystem.TakeDamage(Random.Range(0, 20));
            Targetship.ComputerSystem.TakeDamage(Random.Range(0, 20));
            Targetship.WeaponsSystem.TakeDamage(Random.Range(0, 20));
            Targetship.LifeSupportSystem.TakeDamage(Random.Range(0, 20));
            Targetship.MyShieldGenerator.Current = 0f;
            if (Random.Range(0, 20) == 4 && PhotonNetwork.isMasterClient) Targetship.photonView.RPC("Overcharged", PhotonTargets.All, new object[0]);
        }
    }
}

