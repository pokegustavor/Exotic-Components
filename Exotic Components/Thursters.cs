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

    
}
