﻿using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using PulsarModLoader.Content.Components.CPU;
using static Exotic_Components.Reactors;
namespace Exotic_Components
{
    [HarmonyLib.HarmonyPatch(typeof(PLShipInfo),"Update")]
    internal class Update
    {
        static float LastDamage = Time.time;
        static void Postfix(PLShipInfo __instance)
        {
            try
            {
                if (__instance.MyWarpDrive != null && (__instance.MyWarpDrive.Name == "Ultimate Explorer" || __instance.MyWarpDrive.Name == "Ultimate Explorer MK2" || __instance.MyWarpDrive.Name == "\'The traveller\'") && PLServer.Instance.m_ShipCourseGoals.Count > 0)
                {
                    PLSectorInfo plsectorInfo3 = PLGlobal.Instance.Galaxy.AllSectorInfos[PLServer.Instance.GetCurrentHubID()];
                    PLSectorInfo plsectorInfo4 = PLGlobal.Instance.Galaxy.AllSectorInfos[PLServer.Instance.m_ShipCourseGoals[0]];
                    if (PLEncounterManager.Instance.PlayerShip.WarpChargeStage != EWarpChargeStage.E_WCS_ACTIVE && plsectorInfo4 != null && plsectorInfo4.IsThisSectorWithinPlayerWarpRange() && plsectorInfo4.VisualIndication != ESectorVisualIndication.TOPSEC && plsectorInfo4.VisualIndication != ESectorVisualIndication.LCWBATTLE && (plsectorInfo4.VisualIndication == ESectorVisualIndication.COMET || PLStarmap.ShouldShowSectorBG(plsectorInfo4)))
                    {
                        float closestWarpTargetDot = 0f;
                        Vector3 relPos_PlayerToSector = PLGlobal.GetRelPos_PlayerToSector(plsectorInfo4, plsectorInfo3);
                        float num14 = Vector3.Dot(__instance.ExteriorTransformCached.forward, relPos_PlayerToSector.normalized);
                        if (num14 > closestWarpTargetDot)
                        {
                            closestWarpTargetDot = num14;
                            if (closestWarpTargetDot > 0.996f)
                            {
                                __instance.WarpTargetID = plsectorInfo4.ID;
                            }
                        }
                    }
                    else if (PLEncounterManager.Instance.PlayerShip.WarpChargeStage == EWarpChargeStage.E_WCS_ACTIVE && __instance.MyWarpDrive.Name == "Ultimate Explorer MK2" && Heart.failing)
                    {
                        __instance.WarpTargetID = Heart.destinyID;
                    }
                }
                bool doorshouldstuck = false;
                if (__instance.MyStats != null)
                {
                    foreach (PLShipComponent component in __instance.MyStats.GetSlot(ESlotType.E_COMP_VIRUS))
                    {
                        if (component.SubType == PulsarModLoader.Content.Components.Virus.VirusModManager.Instance.GetVirusIDFromName("Door Stuck"))
                        {
                            doorshouldstuck = true;
                            break;
                        }
                    }
                }
                if (__instance.InteriorDynamic != null)
                {
                    foreach (PLDoor door in __instance.InteriorDynamic.GetComponentsInChildren<PLDoor>())
                    {
                        door.Automatic = !doorshouldstuck;
                    }
                }

                if (PhotonNetwork.isMasterClient && Time.time - LastDamage > 1 && Reactors.BiscuitReactor.effects.ContainsKey((int)EPawnStatusEffectType.MOLTEN) && __instance.MyReactor != null && __instance.MyReactor.Name == "Ultimate Fluffy Biscuit Reactor") 
                {
                    LastDamage = Time.time;
                    foreach(PLShipInfoBase ship in PLEncounterManager.Instance.AllShips.Values) 
                    {
                        if(ship != null && ship != __instance && Vector3.Distance(ship.Exterior.transform.position,__instance.Exterior.transform.position) < 500) 
                        {
                            PLServer.Instance.photonView.RPC("ClientShipTakeDamage", PhotonTargets.All, new object[]
                            {
                                ship.ShipID,
                                50f,
                                false,
                                (int)EDamageType.E_FIRE,
                                UnityEngine.Random.Range(0f, 1f),
                                -1,
                                __instance.ShipID,
                                -1
                            });
                        }
                    }
                }

                if (!__instance.GetIsPlayerShip()) return;
                if (PLServer.GetCurrentSector().Name == "The Core(MOD)" && !PLEncounterManager.Instance.PlayerShip.Get_IsInWarpMode())
                {
                    InitialStore.UpdateCore();
                }
                bool found = false;
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
                if(PLInput.Instance.GetButtonUp(PLInputBase.EInputActionName.pilot_ability) && (!(__instance is PLOldWarsShip_Sylvassi) || (__instance is PLOldWarsShip_Sylvassi && (__instance as PLOldWarsShip_Sylvassi).SlicerFiredInThisSector)) && __instance.GetCurrentShipControllerPlayerID() == PLNetworkManager.Instance.LocalPlayerID) 
                {
                    if (__instance.MyWarpDrive != null && __instance.MyWarpDrive.Name == "The Phase Drive" && Time.time - Warp_Drive.PhaseDrive.LastPhase > 10f) 
                    {
                        PulsarModLoader.ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.RecivePhase", PhotonTargets.All, new object[0]);
                    }
                    else if (__instance.MyShieldGenerator != null && __instance.MyShieldGenerator.Name == "Electric Wall" && __instance.MyStats.ShieldsCurrent / __instance.MyStats.ShieldsMax >= 0.9f)
                    {
                        PulsarModLoader.ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.EMPPulse", PhotonTargets.All, new object[] 
                        {
                            __instance.ShipID
                        });
                    }
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
                if (__instance.MyReactor != null && __instance.MyReactor.GetItemName() != "ThermoPoint Reactor" && __instance.Exterior.GetComponent<PLSpaceHeatVolume>() != null) 
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
                
            }
            catch { }
        }
    }
    [HarmonyPatch(typeof(PLShipStats), "ClearStats")]
    class ResetStatus
    {
        static void Postfix(PLShipStats __instance)
        {
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

    [HarmonyPatch(typeof(PLShipInfoBase), "HasPilotAbilityToDisplay")]
    internal class ShouldShowAbility
    {
        internal static void Postfix(PLShipInfoBase __instance, ref bool __result)
        {
            if (__instance.MyWarpDrive != null && __instance.MyWarpDrive.Name == "The Phase Drive" && Time.time - Warp_Drive.PhaseDrive.LastPhase > 10f)
            {
                __result = true;
            }
            else if (__instance.MyShieldGenerator != null && __instance.MyShieldGenerator.Name == "Electric Wall" && __instance.MyStats.ShieldsCurrent / __instance.MyStats.ShieldsMax >= 0.9f)
            {
                __result = true;
            }
        }
    }
    [HarmonyPatch(typeof(PLOldWarsShip_Sylvassi), "HasPilotAbilityToDisplay")]
    class ShouldShowAbilitySwordShip
    {
        static void Postfix(PLOldWarsShip_Sylvassi __instance, ref bool __result)
        {
            ShouldShowAbility.Postfix(__instance, ref __result);
        }
    }
    [HarmonyPatch(typeof(PLShipInfoBase), "GetPilotAbilityText")]
    internal class AbilityName
    {
        internal static void Postfix(PLShipInfoBase __instance, ref string __result)
        {
            if (__instance.MyWarpDrive != null && __instance.MyWarpDrive.Name == "The Phase Drive" && Time.time - Warp_Drive.PhaseDrive.LastPhase > 10f)
            {
                __result = "Phase Drive";
            }
            else if (__instance.MyShieldGenerator != null && __instance.MyShieldGenerator.Name == "Electric Wall" && __instance.MyStats.ShieldsCurrent / __instance.MyStats.ShieldsMax >= 0.9f)
            {
                __result = "Electric Wall";
            }
        }
    }
    [HarmonyPatch(typeof(PLOldWarsShip_Sylvassi), "GetPilotAbilityText")]
    class AbilityNameSwordShip
    {
        static void Postfix(PLOldWarsShip_Sylvassi __instance, ref string __result)
        {
            if (__instance.SlicerFiredInThisSector)
            {
                AbilityName.Postfix(__instance, ref __result);
            }
        }
    }

    [HarmonyPatch(typeof(PLShipInfoBase), "TakeDamage")]
    class TakeAnyDamage 
    {
        static void Postfix(PLShipInfoBase attackingShip, float dmg) 
        {
            if(attackingShip != null && attackingShip.MyReactor != null && attackingShip.MyReactor.Name == "Ultimate Fluffy Biscuit Reactor" && attackingShip.MyShieldGenerator != null && Reactors.BiscuitReactor.effects.ContainsKey((int)EPawnStatusEffectType.LIFESTEAL)) 
            {
                attackingShip.MyShieldGenerator.Current += dmg * 0.2f;
            }
        }
    }
    [HarmonyPatch(typeof(PLPawn), "OnDeath")]
    class PreventDeath
    {
        static bool Prefix(PLPawn __instance)
        {
            if (!__instance.IsDead && BiscuitReactor.effects.ContainsKey((int)EPawnStatusEffectType.REVIVAL))
            {
                BiscuitReactor.effects.Remove((int)EPawnStatusEffectType.REVIVAL);
                __instance.Health = 50;
                return false;
            }
            if (__instance.MyPlayer != null && __instance.MyPlayer.StartingShip != null)
            {
                List<PLShipComponent> CPUs = __instance.MyPlayer.StartingShip.MyStats.GetComponentsOfType(ESlotType.E_COMP_CPU, false);
                foreach (PLShipComponent plshipComponent in CPUs)
                {
                    if (plshipComponent != null && plshipComponent.SubType == CPUModManager.Instance.GetCPUIDFromName("Imortality Processor") && plshipComponent.IsEquipped)
                    {
                        __instance.Health = __instance.MaxHealth;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
