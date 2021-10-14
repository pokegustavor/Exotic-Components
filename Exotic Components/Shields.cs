using PulsarModLoader.Content.Components.Shield;
using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
namespace Exotic_Components
{
    internal class Shields
    {
        class LayeredShieldMod : ShieldMod
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

            public override int MinIntegrityAfterDamage => Mathf.RoundToInt(ShieldMax * 0.5f);

            public override void Tick(PLShipComponent InComp)
            {
                PLShieldGenerator me = InComp as PLShieldGenerator;
                if (me != null && me.ShipStats != null && me.ShipStats.Ship.MyShieldGenerator.Name == "Layered Shield")
                {
                    me.Deflection = (3f - Mathf.Clamp01(me.ShipStats.ShieldsCurrent / me.ShipStats.ShieldsMax)) + (1.1f + me.Level*0.7f);
                    float multiplier = (1 / Mathf.Clamp01(me.ShipStats.ShieldsCurrent / me.ShipStats.ShieldsMax));
                    if (multiplier > 3.5f) multiplier = 3.5f;
                    me.CalculatedMaxPowerUsage_Watts = MaxPowerUsage_Watts * multiplier;
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
            if (!PhotonNetwork.isMasterClient) return;
            PLShipStats mystats = PLEncounterManager.Instance.GetShipFromID(shipID).MyStats;
            if (PLEncounterManager.Instance.GetShipFromID(shipID).MyShieldGenerator.Name == "Eletric Wall" && !status &&  mystats.ShieldsCurrent/mystats.ShieldsMax >= 0.9f)
            {
                PhotonNetwork.Instantiate("Assets/PrefabInstance/EMPExplosion", PLEncounterManager.Instance.PlayerShip.GetCurrentSensorPosition(), new Quaternion(), 0);
                Object.Instantiate(PLGlobal.Instance.EMPExplosionPrefab, PLEncounterManager.Instance.PlayerShip.Exterior.transform.position, Quaternion.identity);
                foreach (PLShipInfoBase ship in Object.FindObjectsOfType(typeof(PLShipInfoBase)))
                {
                    if (ship as PLShipInfo != null && !ship.GetIsPlayerShip())
                    {
                        (ship as PLShipInfo).EndGameSequenceActive = true;
                    }
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

