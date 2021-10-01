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
    }
}
