using HarmonyLib;
using PulsarModLoader.Content.Components.MissionShipComponent;
using UnityEngine;
using System.Collections.Generic;
using System;
namespace Exotic_Components
{
    class Missions
    {
        public class RecoverCPU 
        {
            public static ComponentOverrideData data = new ComponentOverrideData() { CompType = 23, CompSubType = MissionShipComponentModManager.Instance.GetMissionShipComponentIDFromName("Intergalatic Jump Processor Core"),IsCargo = true };
            public static PLMissionObjective_PickupComponent objective = new PLMissionObjective_PickupComponent(ESlotType.E_COMP_MISSION_COMPONENT, MissionShipComponentModManager.Instance.GetMissionShipComponentIDFromName("Intergalatic Jump Processor Core"), "Intergalatic Jump Processor Core") { RawCustomText = ""};
            public static PLMissionObjective_CompleteWithinJumpCount objective2 = new PLMissionObjective_CompleteWithinJumpCount(14) { RawCustomText = "" };
            public static PLMissionObjective_Custom objective3 = new PLMissionObjective_Custom() { CustomTextOriginal = "Sell processor to The Core", RawCustomText = "Sell processor to The Core" };
            public static void StartMission() 
            {
                PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                    {
                        "NEW MISSION ACCEPTED",
                        Color.yellow,
                        0,
                        "MSN"
                    });
                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/Missions/PickupMission", Vector3.zero, Quaternion.identity, 0, null);
                PLPickupMissionBase mission = gameObject.GetComponent<PLPickupMissionBase>();
                MissionDataBlock missionDataBlock = new MissionDataBlock
                {
                    MissionID = 700,
                    IsPickupMission = false
                };
                PickupMissionData missionData = new PickupMissionData
                {
                    Desc = "I need you to find this ship who was supposed to deliver me a special jump processor core for a... project. The ship is really resistant and in a dangerous sector, so get the component and leave ASAP. When you get the thing just sell it to me, also bring it fast!",
                    Name = "Recover Jump Processor Core",
                    CanBeAbandonedByPlayers = true,
                    MissionID = 700,
                };
                objective2.Init();
                RewardData reward = new RewardData()
                {
                    RewardAmount = 0,
                    RewardDataA = 7,
                    RewardDataB = PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName("Research Processor")
                };
                reward.RwdType = 3;
                missionData.SuccessRewards.Add(reward);
                mission.MyData = missionDataBlock;
                mission.MyMissionData = missionData;
                mission.MissionTypeID = 700;
                mission.Objectives.Add(objective);
                mission.Objectives.Add(objective2);
                mission.Objectives.Add(objective3);
                int minimumFreeSectorNumber = PLGlobal.Instance.Galaxy.GetMinimumFreeSectorNumber();
                if (minimumFreeSectorNumber != -1)
                {
                    PLSectorInfo plsectorInfo = new PLSectorInfo();
                    plsectorInfo.Discovered = false;
                    plsectorInfo.Visited = false;
                    PLGlobal.Instance.Galaxy.AllSectorInfos.Add(minimumFreeSectorNumber, plsectorInfo);
                    plsectorInfo.ID = minimumFreeSectorNumber;
                    plsectorInfo.MySPI = SectorProceduralInfo.Create(PLGlobal.Instance.Galaxy, ref plsectorInfo, plsectorInfo.ID);
                    plsectorInfo.FactionStrength = 0.5f;
                    plsectorInfo.VisualIndication = ESectorVisualIndication.BLACKHOLE;
                    plsectorInfo.MySPI.Faction = 5;
                    plsectorInfo.Position = PLServer.GetSectorPositionAtDistance(10);
                    plsectorInfo.MissionSpecificID = 700;
                    PLServer.Instance.photonView.RPC("ClientInitialGetSectorData", PhotonTargets.Others, new object[]
                            {
                                PLServer.StarmapDataFromSector(plsectorInfo),
                                plsectorInfo.Position
                            });
                    PLServer.Instance.photonView.RPC("ClientUpdateSectorName", PhotonTargets.Others, new object[]
                    {
                                plsectorInfo.ID,
                                plsectorInfo.Name
                    });
                    PLServer.Instance.photonView.RPC("ClientUpdateMissionID", PhotonTargets.Others, new object[]
                    {
                                plsectorInfo.ID,
                                plsectorInfo.MissionSpecificID
                    });
                    PLPersistantShipInfo ship = new PLPersistantShipInfo(EShipType.E_OUTRIDER, 1, plsectorInfo, 0, false, false, true);
                    ship.ShipName = "The Retriver";

                    ship.CompOverrides.Add(data);
                    PLServer.Instance.AllPSIs.Add(ship);
                }
                PLServer.Instance.AllMissions.Add(mission);
                PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(missionData);
            }
        }

        public class KillTaitor 
        {
            public static PLMissionObjective_KillShipOfName objective = new PLMissionObjective_KillShipOfName("Intergalatic Overloards") { RawCustomText = "" };

            public static void StartMission() 
            {
                PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                        {
                        "NEW MISSION ACCEPTED",
                        Color.yellow,
                        0,
                        "MSN"
                        });
                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/Missions/PickupMission", Vector3.zero, Quaternion.identity, 0, null);
                PLPickupMissionBase mission = gameObject.GetComponent<PLPickupMissionBase>();
                MissionDataBlock missionDataBlock = new MissionDataBlock
                {
                    MissionID = 701,
                    IsPickupMission = false,
                    ObjectivesAmountNeeded = new CodeStage.AntiCheat.ObscuredTypes.ObscuredInt[1] { 1 }
                };
                PickupMissionData missionData = new PickupMissionData
                {
                    Desc = "This guys stole some of my extra components, and I won't let that go so easy! Kill them, and you can keep whatever survives the explosion.",
                    Name = "Kill the Thiefs",
                    CanBeAbandonedByPlayers = true,
                    Reward_Credits = 25000,
                    MissionID = 701
                };
                objective.Init();
                RewardData reward = new RewardData()
                {
                    RewardAmount = 25000,
                };
                reward.RwdType = 1;
                missionData.SuccessRewards.Add(reward);
                mission.MyData = missionDataBlock;
                mission.MyMissionData = missionData;
                mission.IsPickupMission = true;
                mission.MissionTypeID = 701;
                mission.Objectives.Add(objective);
                int minimumFreeSectorNumber = PLGlobal.Instance.Galaxy.GetMinimumFreeSectorNumber();
                if (minimumFreeSectorNumber != -1)
                {
                    PLSectorInfo plsectorInfo = new PLSectorInfo();
                    plsectorInfo.Discovered = false;
                    plsectorInfo.Visited = false;
                    PLGlobal.Instance.Galaxy.AllSectorInfos.Add(minimumFreeSectorNumber, plsectorInfo);
                    plsectorInfo.ID = minimumFreeSectorNumber;
                    plsectorInfo.MySPI = SectorProceduralInfo.Create(PLGlobal.Instance.Galaxy, ref plsectorInfo, plsectorInfo.ID);
                    plsectorInfo.FactionStrength = 0.5f;
                    plsectorInfo.VisualIndication = ESectorVisualIndication.NONE;
                    plsectorInfo.MySPI.Faction = 5;
                    plsectorInfo.Position = PLServer.GetSectorPositionAtDistance(UnityEngine.Random.Range(7, 15));
                    plsectorInfo.MissionSpecificID = 701;
                    PLServer.Instance.photonView.RPC("ClientInitialGetSectorData", PhotonTargets.Others, new object[]
                            {
                                PLServer.StarmapDataFromSector(plsectorInfo),
                                plsectorInfo.Position
                            });
                    PLServer.Instance.photonView.RPC("ClientUpdateSectorName", PhotonTargets.Others, new object[]
                    {
                                plsectorInfo.ID,
                                plsectorInfo.Name
                    });
                    PLServer.Instance.photonView.RPC("ClientUpdateMissionID", PhotonTargets.Others, new object[]
                    {
                                plsectorInfo.ID,
                                plsectorInfo.MissionSpecificID
                    });
                    PLPersistantShipInfo ship = new PLPersistantShipInfo(EShipType.E_CIVILIAN_STARTING_SHIP, 1, plsectorInfo, 0, false, false, false);
                    ship.ShipName = "Intergalatic Overloards";
                    PLRand shipDeterministicRand = PLShipInfoBase.GetShipDeterministicRand(ship, 0);
                    List<ComponentOverrideData> overrides = new List<ComponentOverrideData>
                    {
                        new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Layered Shield"), ReplaceExistingComp = true, CompLevel = 2 + PLShipInfoBase.GetChaosBoost(ship,shipDeterministicRand.Next() % 50), IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                        new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("Ultimate Explorer MK2"), ReplaceExistingComp = true, CompLevel = PLShipInfoBase.GetChaosBoost(ship,shipDeterministicRand.Next() % 50), IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                        new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("Steam Core"), ReplaceExistingComp = true, CompLevel =  2 + PLShipInfoBase.GetChaosBoost(ship,shipDeterministicRand.Next() % 50), IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                        new ComponentOverrideData() { CompType = 5, CompSubType = 0, ReplaceExistingComp = true, CompLevel = 4 + PLShipInfoBase.GetChaosBoost(ship,shipDeterministicRand.Next() % 50), IsCargo = false, CompTypeToReplace = 5, SlotNumberToReplace = 0},
                        new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Nano Active MK2"), ReplaceExistingComp = true, CompLevel = 3 + PLShipInfoBase.GetChaosBoost(ship,shipDeterministicRand.Next() % 50), IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                        new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Tweaked Anti-Shield"), ReplaceExistingComp = true, CompLevel = 2 + PLShipInfoBase.GetChaosBoost(ship,shipDeterministicRand.Next() % 50), IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                        new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("TweakedMachineGunMainTurret"), ReplaceExistingComp = true, CompLevel = 2 + PLShipInfoBase.GetChaosBoost(ship,shipDeterministicRand.Next() % 50), IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                        new ComponentOverrideData() { CompType = 17, CompSubType = PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramModManager.Instance.GetWarpDriveProgramIDFromName("BlindFold [VIRUS]")},
                        new ComponentOverrideData() { CompType = 17, CompSubType = PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramModManager.Instance.GetWarpDriveProgramIDFromName("Anti-Life Pulse")},
                    };  
                    ship.CompOverrides.AddRange(overrides);
                    PLServer.Instance.AllPSIs.Add(ship);
                }
                PLServer.Instance.AllMissions.Add(mission);
                PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(missionData);
            }
        }
    }

    class MissionItems 
    {
        class IntergalaticJumpProcessorCore : MissionShipComponentMod
        {
            public override string Name => "Intergalatic Jump Processor Core";

            public override string Description => "This is a very special Jump Processor Core that can be used to create a Intergalatic Jump Processor. Still needs a very special blueprint to not damage on assembly!";

            public override int MarketPrice => 1;
        }
    }

    [HarmonyPatch(typeof(PLBlackHole), "FixedUpdate")]
    class BlackHoleDamage
    {
        static void Postfix(PLBlackHole __instance)
        {
            bool damage = false;
            foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
            {
                if (plshipInfoBase != null && !plshipInfoBase.InWarp && plshipInfoBase.ExteriorRigidbody != null)
                {
                    Vector3 normalized2 = (__instance.transform.position - plshipInfoBase.Exterior.transform.position).normalized;
                    plshipInfoBase.ExteriorRigidbody.AddForce(normalized2 * Mathf.Clamp(300f * plshipInfoBase.MyShipControl.TimeSinceWarp, 1000f, 30000f));
                    float magnitude = (__instance.transform.position - plshipInfoBase.Exterior.transform.position).magnitude;
                    float num = 9000f;
                    if (magnitude < num && Time.time - __instance.LastDamageTime > 0.8f)
                    {

                        float num2 = Mathf.Pow((num - magnitude) / num, 1.65f) * 260f;
                        if (num2 > 20f)
                        {
                            plshipInfoBase.TakeDamage(num2, true, EDamageType.E_BEAM, 0f, -1, null, -1);
                        }
                        if (magnitude < 350f)
                        {
                            plshipInfoBase.TakeDamage(1600f, true, EDamageType.E_BEAM, 0f, -1, null, -1);
                        }
                        damage = true;
                    }
                }
            }
            if (damage) __instance.LastDamageTime = Time.time;
        }
    }
    [HarmonyPatch(typeof(PLLocalize), "Localize", new Type[] {typeof(string),typeof(string),typeof(bool)})]
    class LocalizationNegation 
    {
        static void Postfix(ref string __result, string value) 
        {
            if(__result == string.Empty) 
            {
                __result = value;
            }
        }
    }
}
