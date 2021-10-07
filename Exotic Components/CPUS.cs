using HarmonyLib;
using PulsarModLoader.Content.Components.CPU;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace Exotic_Components
{
    public class CPUS
    {
        public class The_Premonition : CPUMod
        {
            public static int lastLive = 20;

            public static float lastHull = 5000;

            public static List<PLPersistantShipInfo> others = new List<PLPersistantShipInfo>();

            public static Vector3[] crewPosition = new Vector3[5] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

            public override string Name => "The Premonition";

            public override string Description => "A special CPU that calculates millions of timelines and will bring you back when you die in a timeline. It was marked as contraband because it is \"illegal to mess with the nature of time\".";

            public override int MarketPrice => 50000;

            public override bool CanBeDroppedOnShipDeath => false;

            public override bool Contraband => true;

            public override Texture2D IconTexture => base.IconTexture;

            public override float MaxPowerUsage_Watts => 7500f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Charge:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                if (lastLive > 20) return "charged";
                return ((20-lastLive)/2) + " jumps remains";
            }
        }
    }

    [HarmonyPatch(typeof(PLShipStats), "TakeHullDamage")]
    class Inevitable 
    {
        static void Postfix(PLShipStats __instance, float __result) 
        {
            List<PLShipComponent> listofhulls = __instance.GetComponentsOfType(ESlotType.E_COMP_HULL, false);
            if (listofhulls.Count <= 0) return;
            PLHull hull = listofhulls[0] as PLHull;
            if (hull.Current > __result) return;
            List<PLShipComponent> componentsOfType2 = __instance.GetComponentsOfType(ESlotType.E_COMP_CPU, false);
            if (componentsOfType2 != null)
            {
                bool found = false;
                foreach (PLShipComponent plshipComponent in componentsOfType2)
                {
                    if (plshipComponent != null && plshipComponent.SubType == CPUModManager.Instance.GetCPUIDFromName("The Premonition") && !plshipComponent.IsFlaggedForSelfDestruction() && CPUS.The_Premonition.lastLive > 20)
                    {
                        found = true;
                        if (PhotonNetwork.isMasterClient)
                        {
                            PLServer.Instance.photonView.RPC("AddNotification_OneString_LocalizedString", PhotonTargets.All, new object[]
                            {
                                        "[STR0] Death detected! Ending timeline visualisation!",
                                        -1,
                                        PLServer.Instance.GetEstimatedServerMs() + 8000,
                                        true,
                                        plshipComponent.Name_NoTranslation
                            });
                        }
                        CPUS.The_Premonition.lastLive = -2;
                        hull.Current = CPUS.The_Premonition.lastHull;
                        __instance.HullCurrent = CPUS.The_Premonition.lastHull;
                        List<PLShipComponent> listofshields = __instance.GetComponentsOfType(ESlotType.E_COMP_SHLD, false);
                        if (listofshields.Count <= 0) break;
                        (listofshields[0] as PLShieldGenerator).Current = __instance.ShieldsMax;
                        break;
                    }
                }
                if (found) 
                {
                    PLShipInfo ship = PLEncounterManager.Instance.PlayerShip;
                    PLMusic.PostEvent("play_sx_env_ship_powerdownslow", ship.gameObject);
                    ship.WarpTargetID = PLServer.GetCurrentSector().ID;
                    ship.SetInWarp(true);
                    int classID = 0;
                    foreach (Vector3 position in CPUS.The_Premonition.crewPosition)
                    {
                        PLPlayer player = PLEncounterManager.Instance.PlayerShip.GetRelevantCrewMember(classID, false);
                        if (position.y != 0 && position.x != 0 && position.z != 0 && player != null && player.GetPawn() != null && !player.GetPawn().IsDead)
                        {
                            player.GetPawn().Heal(5000);
                            player.photonView.RPC("RecallPawnToPos", PhotonTargets.All, new object[]
                            {
                        position
                            });
                        }
                        classID++;
                    }
                    List<PLFire> firelist = ship.AllFires.Values.ToList();
                    for (int i = firelist.Count - 1; i > -1; i--)
                    {
                        PLServer.Instance.photonView.RPC("RemoveFireWithID", PhotonTargets.All, new object[]
                        {
                            ship.ShipID,
                            firelist[i].FireID
                        });
                    }
                    ship.EngineeringSystem.Health = ship.EngineeringSystem.MaxHealth;
                    ship.WeaponsSystem.Health = ship.WeaponsSystem.MaxHealth;
                    ship.ComputerSystem.Health = ship.ComputerSystem.MaxHealth;
                    ship.LifeSupportSystem.Health = ship.LifeSupportSystem.MaxHealth;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLPersistantEncounterInstance), "SpawnEnemyShip")]
    class StoreEnemies 
    {
        static void Postfix(PLShipInfoBase __result) 
        {
            for(int i = 0; i < 5; i++) 
            {
                PLPlayer player = PLEncounterManager.Instance.PlayerShip.GetRelevantCrewMember(i, false);
                if (player == null)
                {
                    CPUS.The_Premonition.crewPosition[i] = new Vector3(0, 0, 0);
                }
                else if(player.GetPawn() != null && !player.GetPawn().IsDead)
                {
                    CPUS.The_Premonition.crewPosition[i] = player.GetPawn().gameObject.transform.position;
                }
            }
        }
    }
}
