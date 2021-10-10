using HarmonyLib;
using PulsarModLoader.Content.Components.CPU;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
namespace Exotic_Components
{
    public class CPUS
    {
        public class The_Premonition : CPUMod
        {
            public static int lastLive = 0;

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
                if (PLServer.GetCurrentSector().VisualIndication == ESectorVisualIndication.LCWBATTLE || PLServer.GetCurrentSector().VisualIndication == ESectorVisualIndication.TOPSEC || PLServer.GetCurrentSector().VisualIndication == ESectorVisualIndication.UNSEEN_MS)
                {
                    lastLive = 1;
                    return "Unkown Error!";
                }
                if (lastLive >= 20) return "charged";
                return ((20-lastLive)/2) + " jumps remains";
            }
        }

        public class ThermoBoost : CPUMod 
        {
            public static float MaxHeat = 1.1f;

            public override string Name => "Turret Thermo Boost";

            public override string Description => "This processor adds extra heat limit for all turrets in the ship. That is what I call efficient cooling!";

            public override int MarketPrice => 20000;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 1f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Turret extra Heat:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                return (0.3f * InComp.LevelMultiplier(0.31f, 1) * 100) + " %";
            }

            public override void AddStats(PLShipComponent InComp)
            {
                MaxHeat = 1.1f + 0.3f * InComp.LevelMultiplier(0.31f, 1);
            }
        }
    }

    [HarmonyPatch(typeof(PLTurret),"Tick")]
    class HeatMax 
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions)
        {
            List<CodeInstruction> instructionsList = Instructions.ToList();
            instructionsList[105].opcode = OpCodes.Ldsfld;
            instructionsList[105].operand = AccessTools.Field(typeof(CPUS.ThermoBoost), "MaxHeat");
            instructionsList[166].opcode = OpCodes.Ldsfld;
            instructionsList[166].operand = AccessTools.Field(typeof(CPUS.ThermoBoost), "MaxHeat");
            return instructionsList.AsEnumerable();
        }
    }
    [HarmonyPatch(typeof(PLUITurretUI),"Update")]
    class HeatUIUpdate 
    {
        static void Postfix(PLUITurretUI __instance) 
        {
            PLTurret plturret = null;
            PLShipInfoBase plshipInfoBase = null;
            if (PLEncounterManager.Instance.PlayerShip == null) return;
            if (PLNetworkManager.Instance.MyLocalPawn != null)
            {
                if (PLNetworkManager.Instance.MyLocalPawn.IsDead) return;
                plshipInfoBase = PLNetworkManager.Instance.MyLocalPawn.CurrentShip;
            }
            if (plshipInfoBase == null && PLCameraSystem.Instance.GetModeString() == "Turret" && PLCameraSystem.Instance.CurrentCameraMode != null)
            {
                PLCameraMode_Turret plcameraMode_Turret = PLCameraSystem.Instance.CurrentCameraMode as PLCameraMode_Turret;
                if (plcameraMode_Turret != null)
                {
                    plshipInfoBase = plcameraMode_Turret.ShipInfo;
                }
            }
            if (plshipInfoBase != PLEncounterManager.Instance.PlayerShip) return;
            if (PLNetworkManager.Instance.MyLocalPawn != null && plshipInfoBase != null)
            {
                for (int i = 0; i < plshipInfoBase.GetCurrentTurretControllerMaxTurretIndex(); i++)
                {
                    if (plshipInfoBase.GetCurrentTurretControllerPlayerID(i) == PLNetworkManager.Instance.LocalPlayerID)
                    {
                        plturret = plshipInfoBase.GetTurretAtID(i);
                        break;
                    }
                }
            }
            if (plturret != null) 
            {
                float value = plturret.Heat / CPUS.ThermoBoost.MaxHeat;
                __instance.RightUI_Fill.fillAmount = value * 0.25f;
                __instance.RightUI_FillOut.fillAmount = value * 0.25f;
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
                    if (plshipComponent != null && plshipComponent.SubType == CPUModManager.Instance.GetCPUIDFromName("The Premonition") && !plshipComponent.IsFlaggedForSelfDestruction() && CPUS.The_Premonition.lastLive >= 20 &&
                        PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.LCWBATTLE && PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.TOPSEC && PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.UNSEEN_MS)
                    {
                        found = true;
                        if (PhotonNetwork.isMasterClient)
                        {
                            PLServer.Instance.photonView.RPC("AddNotification_OneString_LocalizedString", PhotonTargets.All, new object[]
                            {
                                        "[STR0] detected a death! Ending timeline visualisation!",
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
                    ship.NumberOfFuelCapsules++;
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
                    if(ship.LifeSupportSystem != null)ship.LifeSupportSystem.Health = ship.LifeSupportSystem.MaxHealth;
                    ship.NewShipController(-1);
                    ship.NewSensorDishController(-1);
                    ship.NewTurretController(0, -1);
                    ship.NewTurretController(1, -1);
                    ship.NewTurretController(2, -1);
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
                if (player != null && player.GetPawn() != null && !player.GetPawn().IsDead)
                {
                    CPUS.The_Premonition.crewPosition[i] = player.GetPawn().gameObject.transform.position;
                }
                else
                {
                    CPUS.The_Premonition.crewPosition[i] = new Vector3(0, 0, 0);
                }
            }
        }
    }
}
