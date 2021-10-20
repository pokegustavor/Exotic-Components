using PulsarModLoader.Content.Components.PolytechModule;
using UnityEngine;
namespace Exotic_Components
{
    internal class Polytech
    {
        class ArmorBoost : PolytechModuleMod
        {
            public override string Name => "P.T. Module: Armor";

            public override string Description => "4x hull armor while equipped\n<color=red>Warning: This component causes the hull to have 50% less integrity. Not recommended for extended use</color>";

            public override int MarketPrice => 15000;

            public override bool Experimental => true;

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                PLShipStats stats = InComp.ShipStats;
                stats.HullArmor *= 4f;
                stats.HullCurrent *= 0.5f;
                stats.HullMax *= 0.5f;
            }
        }
        class ReactorBoost : PolytechModuleMod
        {
            public override string Name => "P.T. Module: Reactor Power";

            public override string Description => "2x reactor power while equipped\n<color=red>Warning: This component degrades the hull and decreases core stability. Not recommended for extended use</color>";

            public override int MarketPrice => 15000;

            public override bool Experimental => true;

            public override void Tick(PLShipComponent InComp)
            {
                if (PhotonNetwork.isMasterClient && InComp.IsEquipped)
                { 
                    InComp.ShipStats.Ship.CoreInstability += Time.deltaTime * 0.05f;
                    InComp.ShipStats.Ship.MyHull.Current -= Time.deltaTime * InComp.ShipStats.HullMax * 0.01f;
                    if (InComp.ShipStats.HullCurrent <= 0f) InComp.ShipStats.Ship.AboutToBeDestroyed();
                }
            }

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                PLShipStats stats = InComp.ShipStats;
                stats.ReactorBoostedOutputMax *= 2f;
            }
        }
    }
}
