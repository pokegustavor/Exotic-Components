using HarmonyLib;
using PulsarModLoader.Content.Components.MissionShipComponent;
using UnityEngine;
using System.Collections.Generic;
using System;
namespace Exotic_Components
{
    public class Missions
    {
        public class RecoverCPU
        {
            public static ComponentOverrideData data = new ComponentOverrideData() { CompType = 23, CompSubType = MissionShipComponentModManager.Instance.GetMissionShipComponentIDFromName("Intergalatic Jump Processor Core"), IsCargo = true };
            public static PickupMissionData Missiondata
            {
                get
                {
                    return CreateData();
                }
            }

            private static PickupMissionData CreateData()
            {
                PickupMissionData missionData = new PickupMissionData
                {
                    Desc = "I need you to find this ship who was supposed to deliver me a special jump processor core for a... project. The ship is really resistant and in a dangerous sector, so get the component and leave ASAP. When you get the thing just sell it to me, also bring it fast!",
                    Name = "Recover Jump Processor Core",
                    CanBeAbandonedByPlayers = true,
                    MissionID = 700,
                };
                List<RewardData> rewards = new List<RewardData>
            {
                new RewardData()
                {
                    RewardAmount = 0,
                    RewardDataA = 7,
                    RewardDataB = PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName("Research Processor"),
                    RwdType = 3
                }
            };
                List<ObjectiveData> objectivesData = new List<ObjectiveData>
            {
                new ObjectiveData
                {
                    ObjType = 2,
                    Data = new Dictionary<string, string>
                    {
                        {"PC_ComponentType","23"},
                        {"PC_SubType",MissionShipComponentModManager.Instance.GetMissionShipComponentIDFromName("Intergalatic Jump Processor Core").ToString()},
                        {"PC_CompName","Intergalatic Jump Processor Core"},
                        {"PC_AmountNeeded","1"},
                        {"PC_RemoveComponents","false"},
                    },
                },
                new ObjectiveData
                {
                    ObjType = 10,
                    Data = new Dictionary<string, string>
                    {
                        {"CMIJC_Value","14"},
                    },
                },
                new ObjectiveData
                {
                    ObjType = 0,
                    Data = new Dictionary<string, string>
                    {
                        {"CustomText","Sell processor to The Core"},
                    },
                }
            };
                missionData.SuccessRewards.AddRange(rewards);
                missionData.Objectives.AddRange(objectivesData);
                return missionData;
            }
            //public static PLMissionObjective_PickupComponent objective = new PLMissionObjective_PickupComponent(ESlotType.E_COMP_MISSION_COMPONENT, MissionShipComponentModManager.Instance.GetMissionShipComponentIDFromName("Intergalatic Jump Processor Core"), "Intergalatic Jump Processor Core") { RawCustomText = ""};
            //public static PLMissionObjective_CompleteWithinJumpCount objective2 = new PLMissionObjective_CompleteWithinJumpCount(14) { RawCustomText = "" };
            //public static PLMissionObjective_Custom objective3 = new PLMissionObjective_Custom() { CustomTextOriginal = "Sell processor to The Core", RawCustomText = "Sell processor to The Core" };
            public static void StartMission(bool loading = false)
            {
                if (!loading)
                {
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                        {
                        "NEW MISSION ACCEPTED",
                        Color.yellow,
                        0,
                        "MSN"
                        });
                }
                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/Missions/PickupMission", Vector3.zero, Quaternion.identity, 0, null);
                PLPickupMissionBase mission = gameObject.GetComponent<PLPickupMissionBase>();
                MissionDataBlock missionDataBlock = new MissionDataBlock
                {
                    MissionID = 700,
                    IsPickupMission = false
                };
                PickupMissionData missionData = Missiondata;
                List<PLMissionObjective> objectives = new List<PLMissionObjective>
                {
                new PLMissionObjective_PickupComponent(ESlotType.E_COMP_MISSION_COMPONENT, MissionShipComponentModManager.Instance.GetMissionShipComponentIDFromName("Intergalatic Jump Processor Core"), "Intergalatic Jump Processor Core") { RawCustomText = ""},
                new PLMissionObjective_CompleteWithinJumpCount(14) { RawCustomText = "" },
                new PLMissionObjective_Custom() { CustomTextOriginal = "Sell processor to The Core", RawCustomText = "Sell processor to The Core" }
                };
                foreach (PLMissionObjective objective in objectives)
                {
                    objective.Init();
                }
                mission.MyData = missionDataBlock;
                mission.MyMissionData = missionData;
                mission.MissionTypeID = 700;
                //mission.Objectives.Add(objective);
                //mission.Objectives.Add(objective2);
                //mission.Objectives.Add(objective3);
                mission.Objectives.AddRange(objectives);
                if (!loading)
                {
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
                }
                PLServer.Instance.AllMissions.Add(mission);
                PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(missionData);
            }
        }

        public class KillTaitor
        {
            public static PickupMissionData Missiondata
            {
                get
                {
                    return CreateData();
                }
            }
            private static PickupMissionData CreateData()
            {
                PickupMissionData missionData = new PickupMissionData
                {
                    Desc = "This guys stole some of my extra components, and I won't let that go so easy! Kill them, and you can keep whatever survives the explosion.",
                    Name = "Kill the Thieves",
                    CanBeAbandonedByPlayers = true,
                    Reward_Credits = 25000,
                    MissionID = 701
                };
                List<ObjectiveData> objectivesData = new List<ObjectiveData>
            {
                new ObjectiveData
                {
                    ObjType = 8,
                    Data = new Dictionary<string, string>
                    {
                        {"KSN_Name","Intergalatic Overloards"},
                        {"KSN_AmountNeeded","1"}
                    },

                }
            };
                List<RewardData> rewards = new List<RewardData>
            {
                new RewardData()
                {
                    RewardAmount = 25000,
                    RwdType = 1,
                }
            };
                missionData.SuccessRewards.AddRange(rewards);
                missionData.Objectives.AddRange(objectivesData);
                return missionData;
            }
            public static void StartMission(bool loading = false)
            {
                if (!loading)
                {
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                            {
                        "NEW MISSION ACCEPTED",
                        Color.yellow,
                        0,
                        "MSN"
                            });
                }
                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/Missions/PickupMission", Vector3.zero, Quaternion.identity, 0, null);
                PLPickupMissionBase mission = gameObject.GetComponent<PLPickupMissionBase>();
                MissionDataBlock missionDataBlock = new MissionDataBlock
                {
                    MissionID = 701,
                    IsPickupMission = false,
                    ObjectivesAmountNeeded = new CodeStage.AntiCheat.ObscuredTypes.ObscuredInt[1] { 1 }
                };
                PickupMissionData missionData = Missiondata;
                List<PLMissionObjective> objectives = new List<PLMissionObjective> { new PLMissionObjective_KillShipOfName("Intergalatic Overloards") { RawCustomText = "" } };
                foreach (PLMissionObjective objective in objectives)
                {
                    objective.Init();
                }
                mission.MyData = missionDataBlock;
                mission.MyMissionData = missionData;
                mission.IsPickupMission = true;
                mission.MissionTypeID = 701;
                mission.Objectives.AddRange(objectives);
                if (!loading)
                {
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
                        new ComponentOverrideData() { CompType = 17, CompSubType = PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramModManager.Instance.GetWarpDriveProgramIDFromName("DoorStuck [VIRUS]")},
                        new ComponentOverrideData() { CompType = 17, CompSubType = PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramModManager.Instance.GetWarpDriveProgramIDFromName("Anti-Life Pulse")},
                        new ComponentOverrideData() { CompType = 17, CompSubType = PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramModManager.Instance.GetWarpDriveProgramIDFromName("Self Destruction [VIRUS]")},
                    };
                        ship.CompOverrides.AddRange(overrides);
                        PLServer.Instance.AllPSIs.Add(ship);
                    }
                }
                PLServer.Instance.AllMissions.Add(mission);
                PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(missionData);
            }
        }

        public class ProtectJudge
        {
            public static PickupMissionData Missiondata
            {
                get
                {
                    return CreateData();
                }
            }
            private static PickupMissionData CreateData()
            {
                PickupMissionData missionData = new PickupMissionData
                {
                    Desc = "I need this jude alive to try freeing my \"Complicated situation\", if you kill those assassins so the judge lives, I will give you a exclusive pair of turrets and some money.",
                    Name = "Protect the Judge",
                    CanBeAbandonedByPlayers = true,
                    MissionID = 702,
                    Reward_Credits = 10000,
                    Reward_XP = 3
                };
                List<RewardData> rewards = new List<RewardData>
            {
                new RewardData()
                {
                    RewardAmount = 10000,
                    RwdType = 1,
                },
                new RewardData()
                {
                    RewardAmount = 3,
                    RwdType = 4,
                },
                new RewardData()
                {
                    RewardAmount = 0,
                    RewardDataA = 10,
                    RewardDataB = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Respected Nullifier Gun"),
                    RwdType = 3
                },
                new RewardData()
                {
                    RewardAmount = 0,
                    RewardDataA = 10,
                    RewardDataB = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Respected Nullifier Gun"),
                    RwdType = 3
                }
            };
                List<ObjectiveData> objectivesData = new List<ObjectiveData>
            {
                new ObjectiveData
                {
                    ObjType = 0,
                    Data = new Dictionary<string, string>
                    {
                        {"CustomText","Keep the Judge Alive"},
                    },
                },
                new ObjectiveData
                {
                    ObjType = 5,
                    Data = new Dictionary<string, string>
                    {
                        {"KST_AmountNeeded","3"},
                        {"KST_ShipType","52"},
                        {"CustomText","Eliminate Assassins"},
                    },
                },
                new ObjectiveData
                {
                    ObjType = 10,
                    Data = new Dictionary<string, string>
                    {
                        {"CMIJC_Value","6"},
                    },
                },
            };
                missionData.SuccessRewards.AddRange(rewards);
                missionData.Objectives.AddRange(objectivesData);
                return missionData;
            }
            public static void StartMission(bool loading = false)
            {
                if (!loading)
                {
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                            {
                        "NEW MISSION ACCEPTED",
                        Color.yellow,
                        0,
                        "MSN"
                            });
                }
                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/Missions/PickupMission", Vector3.zero, Quaternion.identity, 0, null);
                PLPickupMissionBase mission = gameObject.GetComponent<PLPickupMissionBase>();
                MissionDataBlock missionDataBlock = new MissionDataBlock
                {
                    MissionID = 702,
                    IsPickupMission = false,
                    ObjectivesAmountNeeded = new CodeStage.AntiCheat.ObscuredTypes.ObscuredInt[1] { 1 }
                };
                PickupMissionData missionData = Missiondata;
                List<PLMissionObjective> objectives = new List<PLMissionObjective>
                {
                new PLMissionObjective_Custom(){ CustomTextOriginal = "Keep the Judge Alive", RawCustomText = "Keep the Judge Alive" },
                new PLMissionObjective_KillShipOfType(EShipType.OLDWARS_SYLVASSI,3){CustomTextOriginal = "Eliminate Assassins", RawCustomText = "Eliminate Assassins"},
                new PLMissionObjective_CompleteWithinJumpCount(6) { RawCustomText = "" },

                };
                foreach (PLMissionObjective objective in objectives)
                {
                    objective.Init();
                }
                mission.MyData = missionDataBlock;
                mission.MyMissionData = missionData;
                mission.IsPickupMission = true;
                mission.MissionTypeID = 702;
                mission.Objectives.AddRange(objectives);
                if (!loading)
                {
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
                        plsectorInfo.Position = PLServer.GetSectorPositionAtDistance(6);
                        plsectorInfo.MissionSpecificID = 702;
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
                        PLPersistantShipInfo ship = new PLPersistantShipInfo(EShipType.OLDWARS_HUMAN, 0, plsectorInfo, 0, false, false, false);
                        ship.ShipName = "Run's Dead";
                        ship.ForcedHostileName = "[REDACTED]";
                        ship.ForcedHostileAll = false;
                        ship.ForcedHostile = false;
                        PLServer.Instance.AllPSIs.Add(ship);
                        PLPersistantShipInfo hunter1 = new PLPersistantShipInfo(EShipType.OLDWARS_SYLVASSI, 1, plsectorInfo, 0, false, false, false);
                        hunter1.ShipName = "[REDACTED]";
                        hunter1.ForcedHostileName = "Run's Dead";
                        hunter1.ForcedHostile = true;
                        PLServer.Instance.AllPSIs.Add(hunter1);
                        PLPersistantShipInfo hunter2 = new PLPersistantShipInfo(EShipType.OLDWARS_SYLVASSI, 1, plsectorInfo, 0, false, false, false);
                        hunter2.ShipName = "[REDACTED]";
                        hunter2.ForcedHostileName = "Run's Dead";
                        hunter2.ForcedHostile = true;
                        PLServer.Instance.AllPSIs.Add(hunter2);
                        PLPersistantShipInfo hunter3 = new PLPersistantShipInfo(EShipType.OLDWARS_SYLVASSI, 1, plsectorInfo, 0, false, false, false);
                        hunter3.ShipName = "[REDACTED]";
                        hunter3.ForcedHostileName = "Run's Dead";
                        hunter3.ForcedHostile = true;
                        PLServer.Instance.AllPSIs.Add(hunter3);
                    }
                }
                PLServer.Instance.AllMissions.Add(mission);
                PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(missionData);
            }
        }

        public class DeliverBiscuit
        {
            public static PLPersistantShipInfo BiscuitShip = null;
            public static PickupMissionData Missiondata
            {
                get
                {
                    return CreateData();
                }
            }

            private static PickupMissionData CreateData()
            {
                PickupMissionData missionData = new PickupMissionData
                {
                    Desc = "I heared that since the intergalatic warpnetwork was deactivated, my precious funky biscuits are not as avaliable as I wish they were, but I heared this ship is delivering more recipe modules to the factory, keep them alive!",
                    Name = "Protect Delivery Ship",
                    CanBeAbandonedByPlayers = true,
                    MissionID = 703,
                };
                List<RewardData> rewards = new List<RewardData>
            {
                new RewardData()
                {
                    RewardAmount = 150000,
                    RwdType = 1,
                },
                new RewardData()
                {
                    RewardAmount = 0,
                    RewardDataA = 7,
                    RewardDataB = PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName("Credits Processor"),
                    RwdType = 3
                }
            };
                List<ObjectiveData> objectivesData = new List<ObjectiveData>
            {
                new ObjectiveData
                {
                    ObjType = 0,
                    Data = new Dictionary<string, string>
                    {
                        {"CustomText","Keep Fluffy Ship Alive"},
                    },
                },
                new ObjectiveData
                {
                    ObjType = 0,
                    Data = new Dictionary<string, string>
                    {
                        {"CustomText","Guide the Fluffy ship to the Fluffy Factory 1 (appears as bounty hunter)"},
                    },
                }
            };
                missionData.SuccessRewards.AddRange(rewards);
                missionData.Objectives.AddRange(objectivesData);
                return missionData;
            }
            public static void StartMission(bool loading = false)
            {
                if (!loading)
                {
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                        {
                        "NEW MISSION ACCEPTED",
                        Color.yellow,
                        0,
                        "MSN"
                        });
                }
                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/Missions/PickupMission", Vector3.zero, Quaternion.identity, 0, null);
                PLPickupMissionBase mission = gameObject.GetComponent<PLPickupMissionBase>();
                MissionDataBlock missionDataBlock = new MissionDataBlock
                {
                    MissionID = 703,
                    IsPickupMission = false
                };
                PickupMissionData missionData = Missiondata;
                List<PLMissionObjective> objectives = new List<PLMissionObjective>
                {
                new PLMissionObjective_Custom() { CustomTextOriginal = "Keep Fluffy Ship Alive", RawCustomText = "Keep Fluffy Ship Alive" },
                new PLMissionObjective_Custom() { CustomTextOriginal = "Guide the Fluffy ship to the Fluffy Factory 1 (appears as bounty hunter)", RawCustomText = "Guide the Fluffy ship to the Fluffy Factory 1 (appears as bounty hunter)" }
                };
                foreach (PLMissionObjective objective in objectives)
                {
                    objective.Init();
                }
                mission.MyData = missionDataBlock;
                mission.MyMissionData = missionData;
                mission.MissionTypeID = 703;
                mission.Objectives.AddRange(objectives);
                PLSectorInfo plsectorInfo = PLGlobal.Instance.Galaxy.GetSectorOfVisualIndication(ESectorVisualIndication.GWG);
                PLPersistantShipInfo ship = new PLPersistantShipInfo(EShipType.E_FLUFFY_TWO, 1, plsectorInfo, 0, false, false, false);
                ship.ShipName = "Delivery #324";
                List<ComponentOverrideData> overrides = new List<ComponentOverrideData>
                    {
                        new ComponentOverrideData() { CompType = 1, CompSubType = 6, CompLevel = 3},
                        new ComponentOverrideData() { CompType = 2, CompSubType = 1, CompLevel = 4},
                        new ComponentOverrideData() { CompType = 3, CompSubType = 4, CompLevel = 1},
                        new ComponentOverrideData() { CompType = 5, CompSubType = 0, CompLevel = 3},
                        new ComponentOverrideData() { CompType = 6, CompSubType = 0, CompLevel = 1},
                        new ComponentOverrideData() { CompType = 7, CompSubType = 0},
                        new ComponentOverrideData() { CompType = 7, CompSubType = 2, CompLevel = 2},
                        new ComponentOverrideData() { CompType = 7, CompSubType = 1, CompLevel = 1},
                        new ComponentOverrideData() { CompType = 8, CompSubType = 0, CompLevel = 0},
                        new ComponentOverrideData() { CompType = 9, CompSubType = 0, CompLevel = 2},
                        new ComponentOverrideData() { CompType = 9, CompSubType = 0, CompLevel = 2},
                        new ComponentOverrideData() { CompType = 10, CompSubType = 0, CompLevel = 3},
                        new ComponentOverrideData() { CompType = 10, CompSubType = 1, CompLevel = 1},
                        new ComponentOverrideData() { CompType = 11, CompSubType = 0, CompLevel = 1},
                        new ComponentOverrideData() { CompType = 16, CompSubType = 0, CompLevel = 0},
                        new ComponentOverrideData() { CompType = 17, CompSubType = 33},
                        new ComponentOverrideData() { CompType = 17, CompSubType = 35},
                        new ComponentOverrideData() { CompType = 17, CompSubType = 22},
                        new ComponentOverrideData() { CompType = 17, CompSubType = 24},
                        new ComponentOverrideData() { CompType = 17, CompSubType = 3},
                        new ComponentOverrideData() { CompType = 20, CompSubType = 10},
                        new ComponentOverrideData() { CompType = 22, CompSubType = 0},
                        new ComponentOverrideData() { CompType = 25, CompSubType = 4, CompLevel = 4},
                        new ComponentOverrideData() { CompType = 26, CompSubType = 0, CompLevel = 0},
                        new ComponentOverrideData() { CompType = 28, CompSubType = 0},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                        new ComponentOverrideData() { CompType = 30, CompSubType = 16, IsCargo = true},
                    };
                ship.CompOverrides.AddRange(overrides);
                BiscuitShip = ship;
                PLServer.Instance.AllPSIs.Add(ship);
                PLServer.Instance.AllMissions.Add(mission);
                PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(missionData);
            }

            public static void SpawnEnemy(PLSectorInfo sector, int maxEnemy = 2) 
            {
                List<EShipType> possibleHunters = new List<EShipType>()
                {
                    EShipType.E_INTREPID,
                    EShipType.E_OUTRIDER,
                    EShipType.E_ROLAND,
                    EShipType.E_WDCRUISER,
                    EShipType.E_DESTROYER,
                    EShipType.E_ANNIHILATOR,
                    EShipType.E_STARGAZER,
                    EShipType.E_CARRIER,
                    EShipType.OLDWARS_SYLVASSI,
                    EShipType.OLDWARS_HUMAN,
                    EShipType.E_FLUFFY_DELIVERY,
                };
                for (int i = 0; i < UnityEngine.Random.Range(1, maxEnemy); i++)
                {
                    PLPersistantShipInfo hunter = new PLPersistantShipInfo(possibleHunters[UnityEngine.Random.Range(0, possibleHunters.Count - 1)], 1, sector, 0, false, false, false);
                    hunter.ForcedHostileToFactionID = 3;
                    hunter.ShipName = "Bounty Hunter";
                    hunter.IsFlagged = true;
                    PLServer.Instance.AllPSIs.Add(hunter);
                }
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
    [HarmonyPatch(typeof(PLLocalize), "Localize", new Type[] { typeof(string), typeof(string), typeof(bool) })]
    class LocalizationNegation
    {
        static void Postfix(ref string __result, string value)
        {
            if (__result == string.Empty)
            {
                __result = value;
            }
        }
    }

    [HarmonyPatch(typeof(PLShipInfoBase), "AboutToBeDestroyed")]
    class OnShipDeath
    {
        static void Prefix(PLShipInfoBase __instance)
        {
            if (!__instance.GetIsPlayerShip() && __instance.MyStats.HullCurrent <= 0f)
            {
                if (__instance.ShipNameValue == "Run's Dead" && PLServer.GetCurrentSector().MissionSpecificID == 702)
                {
                    foreach (PLMissionBase mission in PLServer.Instance.AllMissions)
                    {
                        if (mission.MissionTypeID == 702 && !mission.Abandoned)
                        {
                            mission.FailMission();
                            break;
                        }
                    }
                }
                if (Missions.DeliverBiscuit.BiscuitShip != null && Missions.DeliverBiscuit.BiscuitShip.ShipInstance == __instance)
                {
                    foreach (PLMissionBase mission in PLServer.Instance.AllMissions)
                    {
                        if (mission.MissionTypeID == 703 && !mission.Abandoned)
                        {
                            mission.FailMission();
                            Missions.DeliverBiscuit.BiscuitShip = null;
                            PLServer.Instance.ActiveBountyHunter_SectorID = -1;
                            PLServer.Instance.ActiveBountyHunter_TypeID = -1;
                            break;
                        }
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "ClaimShip")]
    class ClaimRemove
    {
        static void Prefix(int inShipID)
        {
            PLShipInfo ship = PLEncounterManager.Instance.GetShipFromID(inShipID) as PLShipInfo;
            if (ship != null)
            {
                if (ship.ShipNameValue == "Run's Dead" && PLServer.GetCurrentSector().MissionSpecificID == 702)
                {
                    foreach (PLMissionBase mission in PLServer.Instance.AllMissions)
                    {
                        if (mission.MissionTypeID == 702 && !mission.Abandoned)
                        {
                            mission.FailMission();
                            break;
                        }
                    }
                }
                if (Missions.DeliverBiscuit.BiscuitShip != null && Missions.DeliverBiscuit.BiscuitShip.ShipInstance == ship)
                {
                    foreach (PLMissionBase mission in PLServer.Instance.AllMissions)
                    {
                        if (mission.MissionTypeID == 703 && !mission.Abandoned)
                        {
                            mission.FailMission();
                            Missions.DeliverBiscuit.BiscuitShip.m_IsShipDestroyed = true;
                            Missions.DeliverBiscuit.BiscuitShip = null;
                            PLServer.Instance.ActiveBountyHunter_SectorID = -1;
                            PLServer.Instance.ActiveBountyHunter_TypeID = -1;
                            break;
                        }
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLShipInfoBase), "Ship_WarpOutNow")]
    class AvoidWarping 
    {
        static bool Prefix(PLShipInfoBase __instance) 
        {
            if(Missions.DeliverBiscuit.BiscuitShip != null && Missions.DeliverBiscuit.BiscuitShip.ShipInstance == __instance) 
            {
                return false;
            }
            return true;
        }
    }
}
