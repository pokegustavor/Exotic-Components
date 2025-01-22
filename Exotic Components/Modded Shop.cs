﻿using HarmonyLib;
using PulsarModLoader;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
using PulsarModLoader.Utilities;

namespace Exotic_Components
{
    [HarmonyPatch(typeof(PLShipInfo), "UpdateAllHailTargetsList")]
    public class EnsureCoreComms
    {
        static void Postfix()
        {
            TheCoreComms comms = UnityEngine.Object.FindObjectOfType<TheCoreComms>();
            if (comms != null)
            {
                ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.RecieveCore", PhotonTargets.Others, new object[]
                {
                    comms.GetHailTargetID()
                });
            }
            else if (PhotonNetwork.isMasterClient && PLServer.GetCurrentSector().Name == "The Core(MOD)") 
            {
                InitialStore.UpdateCore();
            }
        }

        public static void CreateCore(int ID)
        {
            bool shouldCreate = true;
            foreach (PLHailTarget target in PLHailTarget.AllHailTargets)
            {
                if (target is TheCoreComms) 
                {
                    shouldCreate = false;
                    break;
                }
            }
            if (shouldCreate) 
            {
                InitialStore.UpdateCore();
            }
            TheCoreComms comms = UnityEngine.Object.FindObjectOfType<TheCoreComms>();
            comms.HailTargetID = ID;
        }
    }
    public class RecieveCore : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            EnsureCoreComms.CreateCore((int)arguments[0]);
        }
    }

    public class DeliverProcessor : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            CompleteMission();
        }

        void CompleteMission()
        {
            PLMissionBase mission = PLServer.Instance.GetActiveMissionWithID(700);
            if (mission != null && PhotonNetwork.isMasterClient)
            {
                mission.Objectives[2].IsCompleted = true;
            }
        }
    }

    public class StartExoticMission : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            int missionID = (int)arguments[0];
            switch (missionID)
            {
                case 700:
                    if (!PLServer.Instance.HasMissionWithID(700))
                    {
                        Missions.RecoverCPU.StartMission(false);
                    }
                    break;
                case 701:
                    if (!PLServer.Instance.HasMissionWithID(701))
                    {
                        Missions.KillTaitor.StartMission(false);
                    }
                    break;
                case 702:
                    if (!PLServer.Instance.HasMissionWithID(702))
                    {
                        Missions.ProtectJudge.StartMission(false);
                    }
                    break;
                case 703:
                    if (!PLServer.Instance.HasMissionWithID(703))
                    {
                        Missions.DeliverBiscuit.StartMission(false);
                    }
                    break;
            }
        }

    }


    [HarmonyPatch(typeof(PLTraderInfo), "SellPawnItem")]
    class SellIntergalatic
    {
        static void Prefix(int inNetID, int inPlayer)
        {
            if (!PhotonNetwork.isMasterClient || TheCoreComms.soldIntergalatic) return;
            if (PLServer.Instance.GetPlayerFromPlayerID(inPlayer).MyInventory.GetItemAtNetID(inNetID).GetItemName(true) == "Flagship Intergalactic Warp Schematics" && PLServer.GetCurrentSector().Name == "The Core(MOD)")
            {
                TheCoreComms.soldIntergalatic = true;
                PLServer.Instance.CurrentCrewCredits += 500000;
            }
        }
    }

    [HarmonyPatch(typeof(PLTraderInfo), "ServerAddWare")]
    class SellItem
    {
        static void Postfix(PLTraderInfo __instance, int wareType, int wareHash)
        {
            if (PLServer.GetCurrentSector().Name != "The Core(MOD)") return;
            PLWare maybeFlag = PLWare.CreateFromHash(wareType, wareHash);
            if (maybeFlag.GetItemName(true) == "Flagship Intergalactic Warp Schematics")
            {
                foreach (KeyValuePair<int, PLWare> items in __instance.MyPDE.Wares)
                {
                    if (items.Value.GetItemName(true) == "Flagship Intergalactic Warp Schematics")
                    {
                        __instance.photonView.RPC("RemoveWare", PhotonTargets.All, new object[]
                        {
                        items.Key
                        });
                        return;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLServer), "StartPlayer")]
    class CreateStore
    {
        static void Postfix(PLServer __instance)
        {
            foreach (PLSectorInfo sectors in PLGlobal.Instance.Galaxy.AllSectorInfos.Values)
            {
                if (sectors.Name == "The Core(MOD)") return;
            }
            int ID = PLGlobal.Instance.Galaxy.GetMinimumFreeSectorNumber();
            PLSectorInfo plsectorInfo3 = new PLSectorInfo
            {
                Discovered = false,
                Visited = false
            };
            PLGlobal.Instance.Galaxy.AllSectorInfos.Add(ID, plsectorInfo3);
            plsectorInfo3.ID = ID;
            plsectorInfo3.MySPI = SectorProceduralInfo.Create(PLGlobal.Instance.Galaxy, ref plsectorInfo3, plsectorInfo3.ID);
            plsectorInfo3.MySPI.Faction = 5;
            plsectorInfo3.VisualIndication = ESectorVisualIndication.EXOTIC4;
            plsectorInfo3.Position = new Vector3(0, 0, 0.0001f);
            plsectorInfo3.FactionStrength = 0.5f;
            plsectorInfo3.Name = "The Core(MOD)";
            Gauntlet.CreateGauntlet();
        }
    }

    [HarmonyPatch(typeof(PLCampaignIO), "GetMissionOfTypeID")]
    class CreateMissions
    {
        static void Postfix(PLCampaignIO __instance, int inID, ref MissionData __result)
        {
            if (__result == null)
            {
                PickupMissionData data = null;
                switch (inID)
                {
                    case 700:
                        data = Missions.RecoverCPU.Missiondata;
                        PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(data);
                        PLCampaignIO.Instance.m_CampaignData.PickupMissionTypes.Add(data);
                        __result = data;
                        break;
                    case 701:
                        data = Missions.KillTaitor.Missiondata;
                        PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(data);
                        PLCampaignIO.Instance.m_CampaignData.PickupMissionTypes.Add(data);
                        __result = data;
                        break;
                    case 702:
                        data = Missions.ProtectJudge.Missiondata;
                        PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(data);
                        PLCampaignIO.Instance.m_CampaignData.PickupMissionTypes.Add(data);
                        __result = data;
                        break;
                    case 703:
                        data = Missions.DeliverBiscuit.Missiondata;
                        PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(data);
                        PLCampaignIO.Instance.m_CampaignData.PickupMissionTypes.Add(data);
                        __result = data;
                        break;
                    case 704:
                        data = Missions.Testa.Missiondata;
                        PLCampaignIO.Instance.m_CampaignData.MissionTypes.Add(data);
                        PLCampaignIO.Instance.m_CampaignData.PickupMissionTypes.Add(data);
                        __result = data;
                        break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLTraderInfo), "Update")]
    class KeepStock
    {
        static bool reseted = false;
        static void Postfix(PLTraderInfo __instance)
        {
            if (PLEncounterManager.Instance.PlayerShip != null && PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().Name == "The Core(MOD)" && __instance is PLShop_Sylvassi)
            {
                if (PLEncounterManager.Instance.PlayerShip.InWarp)
                {
                    reseted = false;
                }
                else if (!reseted)
                {
                    InitialStore.UpdateCore();
                    __instance.MyPDE = new TraderPersistantDataEntry();
                    __instance.CreateInitialWares(__instance.MyPDE);
                    reseted = true;
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLShop_Sylvassi), "CreateInitialWares")]
    class InitialStore
    {
        static void Postfix(PLShop_Sylvassi __instance, TraderPersistantDataEntry inPDE)
        {
            if (PLServer.GetCurrentSector().Name == "The Core(MOD)" || (PLEncounterManager.Instance.PlayerShip.WarpTargetID != -1 && PLGlobal.Instance.Galaxy.AllSectorInfos.GetValueSafe(PLEncounterManager.Instance.PlayerShip.WarpTargetID).Name == "The Core(MOD)"))
            {
                int currentID = inPDE.ServerWareIDCounter;
                foreach (PLWare ware in inPDE.Wares.Values)
                {
                    currentID--;
                }
                inPDE.ServerWareIDCounter = currentID;
                inPDE.Wares.Clear();
                foreach (PulsarModLoader.Content.Components.Hull.HullMod hull in PulsarModLoader.Content.Components.Hull.HullModManager.Instance.HullTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(6, PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName(hull.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.HullPlating.HullPlatingMod hullPlating in PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.HullPlatingTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(16, PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName(hullPlating.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.NuclearDevice.NuclearDeviceMod NuclearDevice in PulsarModLoader.Content.Components.NuclearDevice.NuclearDeviceModManager.Instance.NuclearDeviceTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(19, PulsarModLoader.Content.Components.NuclearDevice.NuclearDeviceModManager.Instance.GetNuclearDeviceIDFromName(NuclearDevice.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.CaptainsChair.CaptainsChairMod CaptainsChair in PulsarModLoader.Content.Components.CaptainsChair.CaptainsChairModManager.Instance.CaptainsChairTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(27, PulsarModLoader.Content.Components.CaptainsChair.CaptainsChairModManager.Instance.GetCaptainsChairIDFromName(CaptainsChair.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.FBRecipeModule.FBRecipeModuleMod FBRecipeModule in PulsarModLoader.Content.Components.FBRecipeModule.FBRecipeModuleModManager.Instance.FBRecipeModuleTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(30, PulsarModLoader.Content.Components.FBRecipeModule.FBRecipeModuleModManager.Instance.GetFBRecipeModuleIDFromName(FBRecipeModule.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.Shield.ShieldMod Shield in PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.ShieldTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(1, PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName(Shield.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                for (int i = 0; i < 4; i++)
                {
                    foreach (PulsarModLoader.Content.Components.Thruster.ThrusterMod Thruster in PulsarModLoader.Content.Components.Thruster.ThrusterModManager.Instance.ThrusterTypes)
                    {
                        PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(9, PulsarModLoader.Content.Components.Thruster.ThrusterModManager.Instance.GetThrusterIDFromName(Thruster.Name), 0, 0, 12), null);
                        component.NetID = inPDE.ServerWareIDCounter;
                        inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                        inPDE.ServerWareIDCounter++;
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    foreach (PulsarModLoader.Content.Components.InertiaThruster.InertiaThrusterMod InertiaThruster in PulsarModLoader.Content.Components.InertiaThruster.InertiaThrusterModManager.Instance.InertiaThrusterTypes)
                    {
                        PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(25, PulsarModLoader.Content.Components.InertiaThruster.InertiaThrusterModManager.Instance.GetInertiaThrusterIDFromName(InertiaThruster.Name), 0, 0, 12), null);
                        component.NetID = inPDE.ServerWareIDCounter;
                        inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                        inPDE.ServerWareIDCounter++;
                    }
                }
                foreach (PulsarModLoader.Content.Components.ManeuverThruster.ManeuverThrusterMod ManeuverThruster in PulsarModLoader.Content.Components.ManeuverThruster.ManeuverThrusterModManager.Instance.ManeuverThrusterTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(26, PulsarModLoader.Content.Components.ManeuverThruster.ManeuverThrusterModManager.Instance.GetManeuverThrusterIDFromName(ManeuverThruster.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.Reactor.ReactorMod Reactor in PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.ReactorTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(3, PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName(Reactor.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.CPU.CPUMod CPU in PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.CPUTypes)
                {
                    if (CPU is CPUS.Researcher || CPU is CPUS.CreditLaundering || CPU is CPUS.Upgrader) continue;
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(7, PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName(CPU.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.MegaTurret.MegaTurretMod MainTurret in PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.MegaTurretTypes)
                {
                    if (MainTurret is Main_Turrets.TweakedMachineTurretMod) continue;
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(11, PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName(MainTurret.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.Missile.MissileMod Missile in PulsarModLoader.Content.Components.Missile.MissileModManager.Instance.MissileTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(20, PulsarModLoader.Content.Components.Missile.MissileModManager.Instance.GetMissileIDFromName(Missile.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramMod Program in PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramModManager.Instance.WarpDriveProgramTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(17, PulsarModLoader.Content.Components.WarpDriveProgram.WarpDriveProgramModManager.Instance.GetWarpDriveProgramIDFromName(Program.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.Extractor.ExtractorMod Extractor in PulsarModLoader.Content.Components.Extractor.ExtractorModManager.Instance.ExtractorTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(28, PulsarModLoader.Content.Components.Extractor.ExtractorModManager.Instance.GetExtractorIDFromName(Extractor.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                if (PLEncounterManager.Instance.PlayerShip.MyStats.GetSlot(ESlotType.E_COMP_AUTO_TURRET).MaxItems > 0)
                {
                    for (int i = 0; i < PLEncounterManager.Instance.PlayerShip.MyStats.GetSlot(ESlotType.E_COMP_AUTO_TURRET).MaxItems; i++)
                    {
                        foreach (PulsarModLoader.Content.Components.AutoTurret.AutoTurretMod AutoTurret in PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.AutoTurretTypes)
                        {
                            PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(24, PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName(AutoTurret.Name), 0, 0, 12), null);
                            component.NetID = inPDE.ServerWareIDCounter;
                            inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                            inPDE.ServerWareIDCounter++;
                        }
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    foreach (PulsarModLoader.Content.Components.Turret.TurretMod Turret in PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.TurretTypes)
                    {
                        if (Turret is Turrets.TweakedAntiShieldMod || Turret is Turrets.RNG) continue;
                        PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(10, PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName(Turret.Name), 0, 0, 12), null);
                        component.NetID = inPDE.ServerWareIDCounter;
                        inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                        inPDE.ServerWareIDCounter++;
                    }
                }
                foreach (PulsarModLoader.Content.Components.WarpDrive.WarpDriveMod WarpDrive in PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.WarpDriveTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(2, PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName(WarpDrive.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                if (PLServer.Instance.CrewFactionID == 5)
                {
                    foreach (PulsarModLoader.Content.Components.PolytechModule.PolytechModuleMod polytechModule in PulsarModLoader.Content.Components.PolytechModule.PolytechModuleModManager.Instance.PolytechModuleTypes)
                    {
                        PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(34, PulsarModLoader.Content.Components.PolytechModule.PolytechModuleModManager.Instance.GetPolytechModuleIDFromName(polytechModule.Name), 0, 0, 12), null);
                        component.NetID = inPDE.ServerWareIDCounter;
                        inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                        inPDE.ServerWareIDCounter++;
                    }
                }
            }
        }
        public static void UpdateCore()
        {
            TheCoreComms comms;
            PLHailTarget_ExoticShop old = Object.FindObjectOfType<PLHailTarget_ExoticShop>();
            if (old == null) return;
            if (!(old is TheCoreComms))
            {
                if (Object.FindObjectOfType<TheCoreComms>() == null)
                {
                    Object.FindObjectOfType<PLShop_Sylvassi>().gameObject.AddComponent<TheCoreComms>();
                }
                comms = Object.FindObjectOfType<TheCoreComms>();
                comms.MyShop = old.MyShop;
                comms.MyShop.Name = "The Core";
                comms.MyShop.Desc = "A very special vendor";
                comms.MyShop.ContrabandDealer = true;
                Object.Destroy(old);
            }
            PLShopkeeper keep = Object.FindObjectOfType<PLShopkeeper>();
            keep.Name = "Davey";
            keep.ActorInstance.DisplayName = "Davey";
            List<PLTeleportationLocationInstance> teleporters = PLEncounterManager.Instance.PlayerShip.GetAllTeleporterLocationInstances();
            foreach (PLTeleportationLocationInstance teleporter in teleporters)
            {
                if (teleporter.TeleporterLocationName == "Sylvassi Shop")
                {
                    teleporter.TeleporterLocationName = "Davey's Shop";
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLDialogueActorInstance), "BeginDialogue")]
    class DavySpeak
    {
        static void Postfix(PLDialogueActorInstance __instance)
        {
            if (__instance.DisplayName == "Davey" && __instance.GetCurrentLine() != null)
            {
                LineData data = __instance.GetCurrentLine();
                data.TextOptions = new List<string>() { "How you doing? Please buy stuff, I need food :(." };
                __instance.SetCurrentLine(data);
            }
        }
    }

    class TheCoreComms : PLHailTarget_ExoticShop
    {
        private float LastMissionUpdate = Time.time;
        private bool showingDeliver = false;
        private int missionID = -1;
        private float LastAdded = Time.time;
        public static bool soldIntergalatic = false;
        private string defaultText = "Welcome to the core, not sure how you found me in here, but doesn't matter. I have the most exotic components of all the galaxy, the other shops have no chance against me. Also ignore the big shiny center of the galaxy and buy something!";
        private string currentText;
        public override void Start()
        {
            base.Start();
            this.m_AllChoices.Clear();
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Browse Exotic Goods", false), new PLHailChoiceDelegate(this.OnSelectBrowseGoods)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Install Ship Components", false), new PLHailChoiceDelegate(this.OnSelectInstallComp)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Tell me more about you", new PLHailChoiceDelegate(this.OnTalkWith)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Missions", new PLHailChoiceDelegate(this.Mission)));
            if (PLServer.Instance.HasActiveMissionWithID(700))
            {
                PLMissionBase mission = PLServer.Instance.GetActiveMissionWithID(700);
                if (mission.Objectives[0].IsCompleted)
                {
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Deliver Jump Processor", new PLHailChoiceDelegate(this.DeliverCPU)));
                    showingDeliver = true;
                }
            }
        }
        private void UpdateText()
        {
            PLShipInfo player = PLEncounterManager.Instance.PlayerShip;
            if (player != null && player.CurrentHailTargetSelection != null)
            {
                string currentDialogueLeft = player.lastDisplayedDialogueLeftHash;
                currentDialogueLeft = player.CurrentHailTargetSelection.GetCurrentDialogueLeft();
                string currentDialogueRight = player.CurrentHailTargetSelection.GetCurrentDialogueRight();
                if (currentDialogueLeft != player.lastDisplayedDialogueLeftHash || currentDialogueRight != player.lastDisplayedDialogueRightHash || UnityEngine.Random.Range(0, 1250) == 0)
                {
                    currentDialogueLeft = player.CurrentHailTargetSelection.GetCurrentDialogueLeft();
                    string[] array = currentDialogueLeft.Replace("[newline]", "\n").Split(new char[]
                    {
                        '\n'
                    });
                    string[] array2 = currentDialogueRight.Split(new char[]
                    {
                        '\n'
                    });
                    if (player.newDialogueTextLeft.Length > 0)
                    {
                        player.newDialogueTextLeft.Remove(0, player.newDialogueTextLeft.Length);
                    }
                    if (player.newDialogueTextRight.Length > 0)
                    {
                        player.newDialogueTextRight.Remove(0, player.newDialogueTextRight.Length);
                    }
                    int num8 = 19;
                    for (int m = Mathf.Max(0, array.Length - num8); m < array.Length; m++)
                    {
                        if (m < array.Length - 1)
                        {
                            player.newDialogueTextLeft.Append("\n");
                        }
                        player.newDialogueTextLeft.Append(array[m]);
                    }
                    for (int n = Mathf.Max(0, array2.Length - num8); n < array2.Length; n++)
                    {
                        if (n < array2.Length - 1)
                        {
                            player.newDialogueTextRight.Append("\n");
                        }
                        player.newDialogueTextRight.Append(array2[n]);
                    }
                    player.lastDisplayedDialogueLeftCache = player.newDialogueTextLeft.ToString();
                    player.lastDisplayedDialogueRightCache = player.newDialogueTextRight.ToString();
                    player.lastDisplayedDialogueLeftHash = currentDialogueLeft;
                    player.lastDisplayedDialogueRightHash = currentDialogueRight;
                    player.FormatNewCacheStrings();
                }
            }
        }
        private void OnSelectInstallComp(bool authority, bool local)
        {
            if (PLEncounterManager.Instance.PlayerShip != null && local)
            {
                PLTabMenu.Instance.TabMenuActive = true;
                PLTabMenu.Instance.CurrentTabIndex = 1;
            }
        }
        private void OnSelectBrowseGoods(bool authority, bool local)
        {
            if (PLTabMenu.Instance != null && PLTabMenu.Instance.ItemShopMenu != null && local)
            {
                PLTabMenu.Instance.ItemShopMenu.SetTraderInfo(this.MyShop);
            }
        }
        private void OnTalkWith(bool authority, bool local)
        {
            if (local)
            {
                showingDeliver = false;
                m_AllChoices.Clear();
                currentText = "More about me? Sure, ask away";
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Why did you come to this galaxy?", new PLHailChoiceDelegate(this.WhyMoveHere)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("How did you get all these components?", new PLHailChoiceDelegate(this.HowGotThing)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Why did you set your shop in the core of the galaxy?", new PLHailChoiceDelegate(this.WhyCore)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("What is that store I can teleport to?", new PLHailChoiceDelegate(this.WhyStore)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Will you search the Lost Colony?", new PLHailChoiceDelegate(this.Colony)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Nevermind, go back to buying", new PLHailChoiceDelegate(this.BackToBuy)));
            }
        }
        public override void Update()
        {
            base.Update();
            if (showingDeliver && PLServer.Instance.HasCompletedMissionWithID(700))
            {
                this.m_AllChoices.RemoveAt(m_AllChoices.Count - 1);
                showingDeliver = false;
            }
            if (Time.time - LastMissionUpdate > 1)
            {
                bool onMission = false;
                int count = 0;
                foreach (PLHailChoice choise in this.m_AllChoices)
                {
                    count++;
                    if (choise.GetText() == "Nevermind, just want to go back to buying")
                    {
                        onMission = true;
                    }
                }
                LastMissionUpdate = Time.time;
                if (onMission)
                {
                    int actualCount = 1;
                    if (!PLServer.Instance.HasMissionWithID(700))
                    {
                        actualCount++;
                    }
                    if (!PLServer.Instance.HasMissionWithID(701))
                    {
                        actualCount++;
                    }
                    if (!PLServer.Instance.HasMissionWithID(702))
                    {
                        actualCount++;
                    }
                    if (!PLServer.Instance.HasMissionWithID(703))
                    {
                        actualCount++;
                    }
                    if (actualCount != count)
                    {
                        Mission(false, true);
                    }
                }
            }
        }
        private void Mission(bool authority, bool local)
        {
            showingDeliver = false;
            missionID = -1;
            m_AllChoices.Clear();
            currentText = "Want some money? I guess I have some jobs for you.                              ";
            if (!PLServer.Instance.HasMissionWithID(700))
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Recover a jump processor", new PLHailChoiceDelegate(this.RetriveCPU)));
            }
            if (!PLServer.Instance.HasMissionWithID(701))
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Kill a thief", new PLHailChoiceDelegate(this.KillThief)));
            }
            if (!PLServer.Instance.HasMissionWithID(702))
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Protect my judge", new PLHailChoiceDelegate(this.ProtectJudge)));
            }
            if (!PLServer.Instance.HasMissionWithID(703))
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Protect Biscuit Delivery", new PLHailChoiceDelegate(this.DeliverBiscuit)));
            }
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Nevermind, just want to go back to buying", new PLHailChoiceDelegate(this.BackToBuy)));
            UpdateText();
        }
        private void RetriveCPU(bool authority, bool local)
        {
            missionID = 700;
            m_AllChoices.Clear();
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nThis is a very simple job. A crew was supposed to bring me a special jump processor core for a... project. But I lost communication with them while they were in a black hole sector. I don't know if they are alive, but what I know is the ship probably won't survive for much time, so hurry with collecting the processor core. As a reward I will give you a special processor of mine.";
                LastAdded = Time.time;
            }
            UpdateText();
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Accept", new PLHailChoiceDelegate(this.AcceptMission)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Decline", new PLHailChoiceDelegate(this.Mission)));
        }
        private void KillThief(bool authority, bool local)
        {
            missionID = 701;
            m_AllChoices.Clear();
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nThere is this crew who dared to steal some of my components, and now think they are the most powerful crew in the galaxy. Show them wrong by killing them. I will pay you a good amount of money, and you can keep any component that survives the destruction of their ship. I also heared that they modified the anti-shield turret and the machine gun they stole from me.";
                LastAdded = Time.time;
            }
            UpdateText();
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Accept", new PLHailChoiceDelegate(this.AcceptMission)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Decline", new PLHailChoiceDelegate(this.Mission)));

        }
        private void ProtectJudge(bool authority, bool local)
        {
            missionID = 702;
            m_AllChoices.Clear();
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nI am not a political guy, but you may remember that my relations with the C.U. government is not the best one, but I found this Matthew judge that maybe can make things a lot easier to me. Only problem is he has enemies, and I got a quick SOS before the assassins were able to block long range comms, they are not far away, so I need you to quickly go there and protect him. I will give you some money and a special pair of turrets (it would all be easier if he got that witness protection).";
                LastAdded = Time.time;
            }
            UpdateText();
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Accept", new PLHailChoiceDelegate(this.AcceptMission)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Decline", new PLHailChoiceDelegate(this.Mission)));

        }
        private void DeliverBiscuit(bool authority, bool local)
        {
            missionID = 703;
            m_AllChoices.Clear();
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nI am a really big fan of the Fluffy Biscuits, mostly the funky biscuit. Sadly with the intergalatic warp network deactivated, the number of funky biscuits has being falling really fast, but I got notice of a ship currently near some kind of ancient warpgate and they have a lot of funky recipes. They are, however, being hunted, so please make sure they arrive in the Fluffy Factory 1, and I will pay you a good amount of cash.";
            }
            if (PLServer.Instance.m_ActiveBountyHunter_TypeID < 0)
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Accept", new PLHailChoiceDelegate(this.AcceptMission)));
            }
            else if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nI just noted that you guys have a bounty hunter after you, please kill them before starting this mission!";
            }
            LastAdded = Time.time;
            UpdateText();
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Decline", new PLHailChoiceDelegate(this.Mission)));
        }
        private void AcceptMission(bool authority, bool local)
        {
            if (local)
            {
                ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.StartExoticMission", PhotonTargets.MasterClient, new object[] { missionID });
                Mission(authority, local);
            }
        }
        private void WhyMoveHere(bool authority, bool local)
        {
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nWhy did I move here? I guess it was the weather, I heard Karattis has good weather or something like that. Also I did cross almost all galaxies on the known universe searching for some components for my fine collection and I heard about these Infected" +
                    " that were spreading through your sectors and thought maybe there is something useful coming from them, and I wasn't wrong! I got this cool infected turret. And with the intergalactic warp network disabled I am now stuck here (maybe I should have bought that ultimate explorer MK3).";
                LastAdded = Time.time;
            }
            UpdateText();
        }
        private void HowGotThing(bool authority, bool local)
        {
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nMy components? I found them during the years talking with exotic dealers from multiple galaxies, still didn't have time to buy from the dealers from this galaxy, I have heard about this ThermoCore and the Corbin's Wall, maybe I will buy them before the intergalatic warp network is activated again. If you bring them I will buy... And that's it. " +
                        "If you think I don't sell enough components, maybe visit me in another timeline, at least that is what Skarg said.";
                LastAdded = Time.time;
            }
            UpdateText();
        }
        private void WhyCore(bool authority, bool local)
        {
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nWhy the Core? I came here to the center of the galaxy mostly to avoid the C.U actually. Even if the outpost is not far, my low EM is hidden in the galaxy's core (still not sure how you found me here). ";
                LastAdded = Time.time;
                if (PLServer.Instance.CrewFactionID == 0)
                {
                    currentText += "And I know you guys are a C.U crew, but I have a felling you won't report me. ";
                }
                currentText += "Anyway, talking about hiding, I heard the Estate is actually in this galaxy, apperently with the warp drive malfunctioning or something.";
                if (!soldIntergalatic) currentText += " But they could be my ticket to getting out of here, just need to find them first. If you find the schematics for a flagship drive, you could bring it to me, I would pay good for it *laughs*.";
                else
                {
                    currentText += " But since you got me the schematics somehow, I will be building my own intergalactic warpdrive soon to get out of this galaxy.";
                }
            }
            UpdateText();
        }
        private void WhyStore(bool authority, bool local)
        {
            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nThe physical shop? I don't sell exotic items here, only components. That sylvassi in there is just Davey, he probably will sell you things normally found around. And don't worry he is not a slave, I pay him.... with food. I also like the sylvassi sense of decoration so I let him decorate the shop.";
                LastAdded = Time.time;
            }
            UpdateText();
        }
        private void Colony(bool authority, bool local)
        {

            if (Time.time - LastAdded > 1)
            {
                currentText += "\n\nThe Lost Colony? I didn't receive anything due to the Core interference....\nOh, some kind of hidden treasure and big power that could change the course of history forever? I am no treasure hunter, whatever is there, if it is something interesting I might buy it later. This galaxy of yours" + " has a lot of random shit happening, at this point I wouldn't be surprised if some alien robot army was trying to kill us all.";
                LastAdded = Time.time;
            }
            UpdateText();
        }
        private void BackToBuy(bool authority, bool local)
        {
            m_AllChoices.Clear();
            currentText = defaultText;
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Browse Exotic Goods", false), new PLHailChoiceDelegate(this.OnSelectBrowseGoods)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Install Ship Components", false), new PLHailChoiceDelegate(this.OnSelectInstallComp)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Tell me more about you", new PLHailChoiceDelegate(this.OnTalkWith)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Missions", new PLHailChoiceDelegate(this.Mission)));
            if (PLServer.Instance.HasActiveMissionWithID(700))
            {
                PLMissionBase mission = PLServer.Instance.GetActiveMissionWithID(700);
                if (mission.Objectives[0].IsCompleted)
                {
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Deliver Jump Processor", new PLHailChoiceDelegate(this.DeliverCPU)));
                    showingDeliver = true;
                }
            }
            UpdateText();
        }
        private void DeliverCPU(bool authority, bool local)
        {
            showingDeliver = false;
            currentText = "Nicely done, you have no idea how important this processor is for me, so you have my deepest gratitude, and for your success you will recieve the promised processor, this one will make research every time you jump to a new sector, isn't that nifty?";
            ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.DeliverProcessor", PhotonTargets.MasterClient, new object[0]);
            UpdateText();
            this.m_AllChoices.RemoveAt(m_AllChoices.Count - 1);
        }

        public override string GetName()
        {
            return "The Core";
        }
        public override string GetCurrentDialogueText()
        {
            if (currentText == defaultText && soldIntergalatic && !currentText.Contains("Flagship"))
            {
                defaultText = "Welcome to the core, you... sold me the Flagship Intergalactic Warp Schematics? Thanks, how did you get that? Doesn't matter, I should be able to build this in the next month with Davey's help. In the time being, I have the most exotic components of all the galaxy, the other shops have no chance against me." +
                    " Also ignore the big shiny center of the galaxy and buy something!";
                currentText = defaultText;
            }
            if (currentText == defaultText && PLServer.Instance.IsFragmentCollected(1) && !currentText.Contains("lower price"))
            {
                defaultText += " I have a strange felling, that I should sell you some things for a lower price, I am not sure why.";
                currentText = defaultText;
            }
            if (currentText == defaultText && PLServer.Instance.CrewFactionID == 5 && !currentText.Contains("polytech"))
            {
                defaultText += " I noticed you guys are part of the polytechnic federation, I actually got some components from another crew if you are interested.";
                currentText = defaultText;
            }
            if (currentText == null) currentText = defaultText;
            return PLDialogueActorInstance.AddNewLinesToText(currentText, false, 70, true);
        }
    }
}
