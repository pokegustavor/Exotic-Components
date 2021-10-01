using PulsarModLoader.Content.Components.Hull;
using UnityEngine;
namespace Exotic_Components
{
    class Hulls
    {
        class NanoActiveMK2 : HullMod
        {
            public override string Name => "Nano Active MK2";

            public override string Description => "An upgrade version of the Nano Active Hull that allows healling even during combat. However the resistence was heavily affected due to the increassed amount of nano bots";

            public override int MarketPrice => 12000;

            public override bool CanBeDroppedOnShipDeath => false;

            public override float HullMax => 535f;

            public override float Armor => 0.17f;

            public override void Tick(PLShipComponent InComp)
            {
                base.Tick(InComp);
                PLHull me = InComp as PLHull;
                PLShipStats myStats = me.ShipStats;
                if (myStats != null && me != null && myStats.Ship.MyHull.Name == "Nano Active MK2" && me.Current < myStats.HullMax) 
                {
                    me.Current += myStats.HullMax / 5000;
                    if (me.Current > myStats.HullMax) me.Current = myStats.HullMax;
                }
            }
        }
        class ToxicWall : HullMod
        {
            public override string Name => "Toxic Wall";

            public override string Description => "This impressive hull is said to have being made by the caustic corsair, extremely resistent, but special materials used may cause it to leak toxins when damaged, also losing armor at lower integrity";

            public override int MarketPrice => 14000;

            public override bool CanBeDroppedOnShipDeath => false;

            public override bool Contraband => true;

            public override float HullMax => 820f;

            public override float Armor => 0.8f;

            public override float Defense => 0.6f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                PLLocalize.Localize("Integrity", false),
                "\n",
                PLLocalize.Localize("Armor", false),
                "\n",
                PLLocalize.Localize("Armor (Max)", false)
                });
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                (this.HullMax * InComp.LevelMultiplier(0.2f, 1f)).ToString("0"),
                "\n",
                (this.Armor * 250f * InComp.LevelMultiplier(0.15f, 1f)).ToString("0"),
                "\n",
                (200f * InComp.LevelMultiplier(0.15f, 1f)).ToString("0")
                });
            }

            public override void Tick(PLShipComponent InComp)
            {
                base.Tick(InComp);
                PLHull me = InComp as PLHull;
                if (me != null && me.ShipStats != null && me.ShipStats.HullCurrent != 0 && me.ShipStats.Ship.MyHull.Name == "Toxic Wall")
                {
                    me.ShipStats.Ship.MyHull.Armor = (me.ShipStats.HullCurrent*0.8f) / me.ShipStats.HullMax ;
                    if (me.ShipStats.HullCurrent < me.ShipStats.HullMax * 0.75f)
                    {
                        me.ShipStats.Ship.AcidicAtmoBoostAlpha += me.ShipStats.HullCurrent / me.ShipStats.HullMax * 0.05f;
                    }
                }
            }
        }
    }
}
