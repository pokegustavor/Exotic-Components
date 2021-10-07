using PulsarModLoader.Content.Components.Reactor;
using UnityEngine;

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
        }
        [HarmonyLib.HarmonyPatch(typeof(PLShipStats), "CalculateStats")]
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

        [HarmonyLib.HarmonyPatch(typeof(PLReactor), "Tick")]

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
