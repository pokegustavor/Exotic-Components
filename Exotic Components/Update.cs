﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exotic_Components
{
    [HarmonyLib.HarmonyPatch(typeof(PLShipInfo),"Update")]
    internal class Update
    {
        static void Postfix()
        {
            try
            {
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
            }
            catch { }
        }
}
}
