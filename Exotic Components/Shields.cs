using PulsarModLoader.Content.Components.Shield;
using UnityEngine;
using System.Collections.Generic;
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

            public override int MinIntegrityAfterDamage => Mathf.RoundToInt(ShieldMax*0.5f);

            public override void Tick(PLShipComponent InComp)
            {
				PLShieldGenerator me = InComp as PLShieldGenerator;
				if (me != null && me.ShipStats != null && me.ShipStats.Ship.MyShieldGenerator.Name == "Layered Shield")
				{
					me.Deflection = (3f - Mathf.Clamp01(me.ShipStats.ShieldsCurrent / me.ShipStats.ShieldsMax));
					float multiplier = (1 / Mathf.Clamp01(me.ShipStats.ShieldsCurrent / me.ShipStats.ShieldsMax));
					if (multiplier > 3.5f) multiplier = 3.5f;
					me.CalculatedMaxPowerUsage_Watts = MaxPowerUsage_Watts * multiplier;
				}
			}
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PLShipStats), "TakeShieldDamage")]
    class ShieldDamage 
    {
        static bool Prefix(float inDmg, EDamageType dmgType, float DT_ShieldBoost, float shieldDamageMod, PLTurret turret, ref float __result, PLShipStats __instance) 
        {
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
				num2 *= 1f / DT_ShieldBoost * (1f / shieldDamageMod);
				num2 += (__instance.ShieldsDeflection - (0.6f * __instance.Ship.MyShieldGenerator.LevelMultiplier(0.1f, 1f)))*1.5f;
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
    }

}
