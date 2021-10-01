using HarmonyLib;
using PulsarModLoader;
namespace Exotic_Components
{
    [HarmonyPatch(typeof(PLServer), "StartPlayer")]
    class Patch
    {
        static void Postfix()
        {
            foreach(PLSectorInfo sectors in PLGlobal.Instance.Galaxy.AllSectorInfos.Values) 
            {
                if (sectors.Name == "Universal Shop(MODDED)") return;
            }
            int ID = PLGlobal.Instance.Galaxy.GetMinimumFreeSectorNumber();
            PLSectorInfo plsectorInfo3 = new PLSectorInfo();
            plsectorInfo3.Discovered = false;
            plsectorInfo3.Visited = false;
            PLGlobal.Instance.Galaxy.AllSectorInfos.Add(ID, plsectorInfo3);
            plsectorInfo3.ID = ID;
            plsectorInfo3.MySPI = SectorProceduralInfo.Create(PLGlobal.Instance.Galaxy, ref plsectorInfo3, plsectorInfo3.ID);
            plsectorInfo3.MySPI.Faction = 5;
            plsectorInfo3.VisualIndication = ESectorVisualIndication.GENERAL_STORE;
            plsectorInfo3.Position = PLServer.GetSectorPositionAtDistance(UnityEngine.Random.Range(1f, 6f));
            plsectorInfo3.FactionStrength = 0.5f;
            plsectorInfo3.Name = "Universal Shop(MODDED)";
        }
    }
    [HarmonyPatch(typeof(PLShop_General), "CreateInitialWares")]
    class InitialStore
    {
        static void Postfix(PLShop_General __instance, TraderPersistantDataEntry inPDE) 
        {
            if(PLServer.GetCurrentSector().Name == "Universal Shop(MODDED)") 
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
                foreach (PulsarModLoader.Content.Components.PolytechModule.PolytechModuleMod PolytechModule in PulsarModLoader.Content.Components.PolytechModule.PolytechModuleModManager.Instance.PolytechModuleTypes)
                {
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(34, PulsarModLoader.Content.Components.PolytechModule.PolytechModuleModManager.Instance.GetPolytechModuleIDFromName(PolytechModule.Name), 0, 0, 12), null);
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
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(2, PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName(Shield.Name), 0, 0, 12), null);
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
                    PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(7, PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName(CPU.Name), 0, 0, 12), null);
                    component.NetID = inPDE.ServerWareIDCounter;
                    inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                    inPDE.ServerWareIDCounter++;
                }
                foreach (PulsarModLoader.Content.Components.MegaTurret.MegaTurretMod MainTurret in PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.MegaTurretTypes)
                {
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
                for (int i = 0; i < 4; i++)
                {
                    foreach (PulsarModLoader.Content.Components.AutoTurret.AutoTurretMod AutoTurret in PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.AutoTurretTypes)
                    {
                        PLShipComponent component = PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo(24, PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName(AutoTurret.Name), 0, 0, 12), null);
                        component.NetID = inPDE.ServerWareIDCounter;
                        inPDE.Wares.Add(inPDE.ServerWareIDCounter, component);
                        inPDE.ServerWareIDCounter++;
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    foreach (PulsarModLoader.Content.Components.Turret.TurretMod Turret in PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.TurretTypes)
                    {
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

                __instance.ContrabandDealer = true;
            }
        }
    }
}
