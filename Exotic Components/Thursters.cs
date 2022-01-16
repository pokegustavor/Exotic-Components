using PulsarModLoader.Content.Components.Thruster;
using PulsarModLoader.Content.Components.ManeuverThruster;
using PulsarModLoader.Content.Components.InertiaThruster;
namespace Exotic_Components
{
    internal class Thursters
    {
        class FlagshipThruster : ThrusterMod
        {
            public override string Name => "Flagship Thruster";

            public override string Description => "Designed to allow the flagship to move at a decent speed, it uses a lot of power, but should move your small ship really fast, and be careful with the close turns.";

            public override int MarketPrice => 600000;

            public override float MaxOutput => 17f;

            public override float MaxPowerUsage_Watts => 80000f;
        }
    }
    class InertiaThrusters 
    {
        class FlagshipInertiathruster : InertiaThrusterMod 
        {
            public override string Name => "Flagship Inertia Thruster";

            public override string Description => "Designed to allow the flagship to actually turn, it may be a little overkill for your light ship, just don't become a Beyblade, and be careful with power.";

            public override int MarketPrice => 800000;

            public override float MaxOutput => 30f;

            public override float MaxPowerUsage_Watts => 60000f;
        }
    
    }
    class ManeuverThrusters 
    {
        class FlagshipManeuverthruster : ManeuverThrusterMod
        {
            public override string Name => "Flagship Maneuvering Thruster";

            public override string Description => "Designed to allow the flagship to move sideways a little bit, just be careful with crashing in to walls, and keep an eye on the reactor, this (and probably the shield recharge) will take quite some power.";

            public override int MarketPrice => 700000;

            public override float MaxOutput => 7f;

            public override float MaxPowerUsage_Watts => 100000f;

        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PLShipStats), "ClearStats")]
    class ResetMass 
    {
        static void Postfix(PLShipStats __instance) 
        {
            switch (__instance.Ship.ShipTypeID) 
            {
                default:
                    __instance.Mass = 380f;
                    break;
                case EShipType.E_BOUNTY_HUNTER_01:
                case EShipType.E_BEACON:
                case EShipType.E_WDDRONE1:
                case EShipType.E_INFECTED_FIGHTER:
                case EShipType.E_INFECTED_FIGHTER_HEAVY:
                case EShipType.E_MATRIX_DRONE:
                case EShipType.E_REPAIR_DRONE:
                case EShipType.E_SELF_DESTRUCT_DRONE:
                    __instance.Mass = 480f;
                    break;
                case EShipType.E_CIVILIAN_STARTING_SHIP:
                case EShipType.E_ACADEMY:
                case EShipType.E_INFECTED_CARRIER:
                case EShipType.E_INTREPID:
                    __instance.Mass = 500f;
                    break;
                case EShipType.E_WDCRUISER:
                    __instance.Mass = 520f;
                    break;
                case EShipType.E_ALIEN_TENTACLE_EYE:
                    __instance.Mass = 12931f;
                    break;
                case EShipType.E_DEATHSEEKER_DRONE_SC:
                    __instance.Mass = 300f;
                    break;
                case EShipType.E_DEATHSEEKER_DRONE:
                    __instance.Mass = 1200f;
                    break;
                case EShipType.E_FLUFFY_DELIVERY:
                case EShipType.E_OUTRIDER:
                case EShipType.E_POLYTECH_SHIP:
                case EShipType.E_PTDRONE:
                    __instance.Mass = 600f;
                    break;
                case EShipType.OLDWARS_HUMAN:
                    __instance.Mass = 680f;
                    break;
                case EShipType.E_ROLAND:
                    __instance.Mass = 1020f;
                    break;
                case EShipType.E_STARGAZER:
                    __instance.Mass = 540f;
                    break;
                case EShipType.E_SWARM_KEEPER:
                    __instance.Mass = 2000f;
                    break;
                case EShipType.E_SHOCK_DRONE:
                    __instance.Mass = 1800f;
                    break;
                case EShipType.E_UNSEEN_FIGHTER:
                    __instance.Mass = 700f;
                    break;
                case EShipType.E_GUARDIAN:
                    __instance.Mass = 3600f;
                    break;
                case EShipType.E_ANNIHILATOR:
                    __instance.Mass = 655f;
                    break;
                case EShipType.E_DESTROYER:
                    __instance.Mass = 750f;
                    break;
            }
        }
    }
}
