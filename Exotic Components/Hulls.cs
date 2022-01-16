using PulsarModLoader.Content.Components.Hull;
using UnityEngine;
namespace Exotic_Components
{
    class Hulls
    {
        class NanoActiveMK2 : HullMod
        {
            public override string Name => "Nano Active MK2";

            public override string Description => "An upgraded version of the Nano Active Hull that allows healling even during combat. However the resistence was heavily affected due to the increassed amount of nano bots";

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
                PLHull me = InComp as PLHull;
                return string.Concat(new string[]
                {
                (me.Max * InComp.LevelMultiplier(0.2f, 1f)).ToString("0"),
                "\n",
                (me.Armor * 250f * InComp.LevelMultiplier(0.15f, 1f)).ToString("0"),
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
                        me.ShipStats.Ship.AcidicAtmoBoostAlpha += Mathf.Min(me.ShipStats.HullMax / (me.ShipStats.HullCurrent*100) * 0.05f,0.7f);
                    }
                }
            }
        }
        class FlagShipHull : HullMod 
        {
            public override string Name => "Flagship Hull";

            public override string Description => "Originally planned to be used in a third flagship, this hull is virtually indestructible, but not impenetrable. Once you pay me a fortune to install it you won't explode, but be carefull with the infected.";

            public override int MarketPrice => 1700000;

            public override bool CanBeDroppedOnShipDeath => false;

            public override float HullMax => 50000f;

            public override float Armor => 5f;

            public override float Defense => 2f;

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
                PLHull me = InComp as PLHull;
                return string.Concat(new string[]
                {
                (me.Max * InComp.LevelMultiplier(0.2f, 1f)).ToString("0"),
                "\n",
                (me.Armor * 250f * InComp.LevelMultiplier(0.15f, 1f)).ToString("0"),
                "\n",
                (200f * InComp.LevelMultiplier(0.15f, 1f)).ToString("0")
                });
            }

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                InComp.ShipStats.Mass += 4800f;
            }
        }
        class AntiInfectedHull : HullMod 
        {
            public override string Name => "Anti-Infected Hull";

            public override string Description => "This resistant hull was designed to be resistent to the infected acid, allowing it to handle any infected sector.";

            public override int MarketPrice => 19000;

            public override bool CanBeDroppedOnShipDeath => false;

            public override bool Experimental => true;

            public override float HullMax => 710f;

            public override float Armor => 0.4f;

            public override float Defense => 0.3f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                PLLocalize.Localize("Integrity", false),
                "\n",
                PLLocalize.Localize("Armor", false),
                });
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLHull me = InComp as PLHull;
                return string.Concat(new string[]
                {
                (me.Max * InComp.LevelMultiplier(0.2f, 1f)).ToString("0"),
                "\n",
                (me.Armor * 250f * InComp.LevelMultiplier(0.15f, 1f)).ToString("0"),
                });
            }

        }
        class PhaseDriverHull : HullMod 
        {
            public override string Name => "The Phase Driver Hull";

            public override string Description => "This special hull has quite a bit of resistence, designed to survive multiple short range warp with no damage. But it doesn't survive imploding by warping in a wall, so hope that safety system works in the warp drive.";

            public override int MarketPrice => 13000;

            public override bool CanBeDroppedOnShipDeath => false;

            public override bool Experimental => true;

            public override float HullMax => 950f;

            public override float Armor => 0.35f;

            public override float Defense => 0.2f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return string.Concat(new string[]
                {
                PLLocalize.Localize("Integrity", false),
                "\n",
                PLLocalize.Localize("Armor", false),
                });
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLHull me = InComp as PLHull;
                return string.Concat(new string[]
                {
                (me.Max * InComp.LevelMultiplier(0.2f, 1f)).ToString("0"),
                "\n",
                (me.Armor * 250f * InComp.LevelMultiplier(0.15f, 1f)).ToString("0"),
                });
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PLHull), "Tick")]
        class ManualTick
        {
            static void Postfix(PLHull __instance)
            {
                int subtypeformodded = __instance.SubType - HullModManager.Instance.VanillaHullMaxType;
                if (subtypeformodded > -1 && subtypeformodded < HullModManager.Instance.HullTypes.Count && __instance.ShipStats != null)
                {
                    HullModManager.Instance.HullTypes[subtypeformodded].Tick(__instance);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PLShipStats), "TakeHullDamage")]
        class HullDamage
        {
            static bool Prefix(ref float inDmg, EDamageType inDmgType, PLShipInfoBase attackingShip, PLTurret turret, PLShipStats __instance) 
            {
                PLHull shipComponent = __instance.GetShipComponent<PLHull>(ESlotType.E_COMP_HULL, false);
                if(inDmgType == EDamageType.E_INFECTED && inDmg == 10 && HullModManager.Instance.GetHullIDFromName("Anti-Infected Hull") == shipComponent.SubType) 
                {
                    return false;
                }
                else if(inDmgType == EDamageType.E_INFECTED && HullModManager.Instance.GetHullIDFromName("Anti-Infected Hull") == shipComponent.SubType) 
                {
                    inDmg *= 0.2f;
                }
                if(turret is AutoScrapper) 
                {
                    if (PhotonNetwork.isMasterClient && !__instance.Ship.GetIsPlayerShip() && Time.time - __instance.Ship.LastServerDroppedScrapperScrap > 20f && Random.Range(0, 40) == 3)
                    {
                        __instance.Ship.LastServerDroppedScrapperScrap = Time.time;
                        PLServer.Instance.photonView.RPC("CreateSpaceScrapAtLocation", PhotonTargets.All, new object[]
                        {
                        __instance.Ship.Exterior.transform.position + UnityEngine.Random.onUnitSphere,
                        __instance.Ship.ShipID,
                        true
                        });
                    }
                }
                return true;
            }
        }
    }
}
