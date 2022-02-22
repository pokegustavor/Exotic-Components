using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Exotic_Components
{
    [HarmonyLib.HarmonyPatch(typeof(PLShipInfo),"Update")]
    internal class Update
    {
        
        static void Postfix(PLShipInfo __instance)
        {
            try
            {
                if (!__instance.GetIsPlayerShip()) return;
                if (PLServer.GetCurrentSector().Name == "The Core(MOD)" && !PLEncounterManager.Instance.PlayerShip.Get_IsInWarpMode())
                {
                    InitialStore.UpdateCore();
                }
                bool found = false;
                foreach(PLCPU cpu in PLEncounterManager.Instance.PlayerShip.MyStats.GetComponentsOfType(ESlotType.E_COMP_CPU,false)) 
                {
                    if (cpu.Name == "Turret Thermo Boost")
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) CPUS.ThermoBoost.MaxHeat = 1.1f;
                found = false;
                if(PLServer.GetCurrentSector().MissionSpecificID == 702) 
                {
                    PLShipInfo judge = null;
                    foreach(PLShipInfoBase ship in PLEncounterManager.Instance.AllShips.Values) 
                    {
                        if(ship.ShipNameValue == "[REDACTED]" && ship.ShipTypeID == EShipType.OLDWARS_SYLVASSI && !ship.GetIsPlayerShip() && !ship.HasBeenDestroyed) 
                        {
                            found = true;
                        }
                        if(ship.ShipNameValue == "Run's Dead" && ship.ShipTypeID == EShipType.OLDWARS_HUMAN && !ship.GetIsPlayerShip() && !ship.HasBeenDestroyed) 
                        {
                            judge = ship as PLShipInfo;
                            if (judge.FactionID == -1) judge.FactionID = PLEncounterManager.Instance.PlayerShip.FactionID;
                        }
                    }
                    if(!found && judge != null) 
                    {
                        judge.Ship_WarpOutNow();
                        foreach (PLMissionBase mission in PLServer.Instance.AllMissions)
                        {
                            if (mission.MissionTypeID == 702)
                            {
                                mission.Objectives[0].IsCompleted = true;
                                break;
                            }
                        }
                    }
                }
                if(__instance.MyWarpDrive != null && PLInput.Instance.GetButtonUp(PLInputBase.EInputActionName.pilot_ability) && __instance.MyWarpDrive.Name == "The Phase Drive" && Time.time - Warp_Drive.PhaseDrive.LastPhase > 10f && (!(__instance is PLOldWarsShip_Sylvassi) || (__instance is PLOldWarsShip_Sylvassi && (__instance as PLOldWarsShip_Sylvassi).SlicerFiredInThisSector)) && __instance.GetCurrentShipControllerPlayerID() == PLNetworkManager.Instance.LocalPlayerID) 
                {
                    PulsarModLoader.ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.RecivePhase", PhotonTargets.All, new object[0]);
                }
                if (Time.time - Warp_Drive.PhaseDrive.LastPhase < 1f && __instance.HullPlatingRenderers != null && Warp_Drive.PhaseDrive.Phasing) 
                {
                    foreach(Renderer rend in __instance.HullPlatingRenderers) 
                    {
                        if (rend != null)
                        {
                            rend.enabled = false;
                        }
                    }
                    __instance.MyStats.ThrustOutputMax = 0;
                    __instance.MyStats.InertiaThrustOutputMax = 0;
                    __instance.MyStats.ManeuverThrustOutputMax = 0;
                    if (__instance.ExteriorMeshRenderer != null)
                    {
                        __instance.ExteriorMeshRenderer.enabled = false;
                    }
                }
                if (__instance.MyReactor != null && __instance.MyReactor.GetItemName() != "" && __instance.Exterior.GetComponent<PLSpaceHeatVolume>() != null) 
                {
                    PLSpaceHeatVolume heatVolume = __instance.Exterior.GetComponent<PLSpaceHeatVolume>();
                    heatVolume.MyPS.enableEmission = false;
                    ParticleSystem.ShapeModule shape = heatVolume.MyPS.shape;
                    shape.radius = 0.0001f;
                    shape.scale = new Vector3(0, 0, 0);
                    heatVolume.MyPS.gameObject.transform.localPosition = __instance.Exterior.transform.localPosition - new Vector3(0, -50, 0);
                    __instance.MyTLI.AtmoSettings.Temperature = 1f;
                }
                if(PLServer.Instance.HasMissionWithID(703) && !PLServer.Instance.GetMissionWithID(703).Abandoned && !PLServer.Instance.GetMissionWithID(703).Ended) 
                {
                    
                    PLPersistantShipInfo objective = null;
                    if (Missions.DeliverBiscuit.BiscuitShip == null)
                    {
                        foreach (PLPersistantShipInfo ship in PLServer.Instance.AllPSIs)
                        {
                            if (ship.ShipName == "Delivery #324" && !ship.m_IsShipDestroyed)
                            {
                                objective = ship;
                                Missions.DeliverBiscuit.BiscuitShip = ship;
                                break;
                            }
                        }
                    }
                    else 
                    {
                        objective = Missions.DeliverBiscuit.BiscuitShip;
                    }
                    if(objective != null) 
                    {
                        PLServer.Instance.ActiveBountyHunter_TypeID = 5;
                        PLServer.Instance.ActiveBountyHunter_SectorID = objective.MyCurrentSector.ID;
                        PLServer.Instance.ActiveBountyHunter_SecondsSinceWarp = 0f;
                        PLServer.Instance.m_ActiveBountyHunter_LastUpdateTime = Time.time;
                        if (objective.ShipInstance != null)
                        {
                            foreach (int enemyID in PLEncounterManager.Instance.PlayerShip.HostileShips)
                            {
                                if(enemyID != objective.ShipInstance.ShipID && !objective.ShipInstance.HostileShips.Contains(enemyID))
                                {
                                    objective.ShipInstance.HostileShips.Add(enemyID);
                                }
                            }
                            foreach (int enemyID in objective.ShipInstance.HostileShips)
                            {
                                if (enemyID != objective.ShipInstance.ShipID && !PLEncounterManager.Instance.PlayerShip.HostileShips.Contains(enemyID))
                                {
                                    PLEncounterManager.Instance.PlayerShip.HostileShips.Add(enemyID);
                                }
                            }
                            objective.ShipInstance.HostileShips.RemoveAll((int ID) => PLEncounterManager.Instance.GetShipFromID(ID) == null || PLEncounterManager.Instance.PlayerShip.ShipID == ID);
                            PLEncounterManager.Instance.PlayerShip.HostileShips.RemoveAll((int ID) => objective.ShipInstance.ShipID == ID);
                        }
                        else if(PLServer.Instance.ActiveBountyHunter_SectorID == PLServer.GetCurrentSector().ID && !PLEncounterManager.Instance.PlayerShip.InWarp) 
                        {
                            objective.CreateShipInstance(PLEncounterManager.Instance.GetCPEI());
                        }
                    }
                }
                if (!PLCampaignIO.Instance.m_CampaignData.MissionTypes.Contains(Missions.RecoverCPU.Missiondata)) 
                {
                    PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(Missions.RecoverCPU.Missiondata);
                }
                if (!PLCampaignIO.Instance.m_CampaignData.MissionTypes.Contains(Missions.KillTaitor.Missiondata))
                {
                    PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(Missions.KillTaitor.Missiondata);
                }
                if (!PLCampaignIO.Instance.m_CampaignData.MissionTypes.Contains(Missions.ProtectJudge.Missiondata))
                {
                    PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(Missions.ProtectJudge.Missiondata);
                }
                if (!PLCampaignIO.Instance.m_CampaignData.MissionTypes.Contains(Missions.DeliverBiscuit.Missiondata))
                {
                    PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(Missions.DeliverBiscuit.Missiondata);
                }
            }
            catch { }
        }
    }
    [HarmonyLib.HarmonyPatch(typeof(PLShipStats), "ClearStats")]
    class ResetStatus
    {
        static void Postfix(PLShipStats __instance)
        {
            __instance.ReactorCoolantCapacity = 800f;
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
