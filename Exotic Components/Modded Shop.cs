using HarmonyLib;
using PulsarModLoader;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
namespace Exotic_Components
{
    [HarmonyPatch(typeof(PLTraderInfo), "SellPawnItem")]
    class SellIntergalatic
    {
        static void Prefix(int inNetID, int inPlayer)
        {
            if (!PhotonNetwork.isMasterClient || TheCoreComms.soldIntergalatic) return;
            if (PLServer.Instance.GetPlayerFromPlayerID(inPlayer).MyInventory.GetItemAtNetID(inNetID).GetItemName(true) == "Flagship Intergalactic Warp Schematics" && PLServer.GetCurrentSector().Name == "The Core(MOD)")
            {
                TheCoreComms.soldIntergalatic = true;
                PLServer.Instance.photonView.RPC("CaptainBuy_Fuel", PhotonTargets.All, new object[]
                             {
                         PLEncounterManager.Instance.PlayerShip.ShipID,
                        -500000
                             });
            }
        }
    }
    [HarmonyPatch(typeof(PLTraderInfo), "ServerAddWare")]
    class DeleteIntergalatic
    {
        static void Postfix(PLTraderInfo __instance, int wareType, int wareHash)
        {
            PLWare maybeFlag = PLWare.CreateFromHash(wareType, wareHash);
            if (maybeFlag.GetItemName(true) == "Flagship Intergalactic Warp Schematics" && PLServer.GetCurrentSector().Name == "The Core(MOD)")
            {
                foreach (KeyValuePair<int,PLWare> items in __instance.MyPDE.Wares)
                {
                    if (items.Value.GetItemName(true) == "Flagship Intergalactic Warp Schematics")
                    {
                        //PulsarModLoader.Utilities.Messaging.Notification("Removing");
                        __instance.photonView.RPC("RemoveWare", PhotonTargets.All, new object[]
                        {
                        items.Key
                        });
                        return;
                    }
                }
                //PulsarModLoader.Utilities.Messaging.Notification("Nothing Removed");
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "StartPlayer")]
    class Patch
    {
        static void Postfix()
        {
            foreach (PLSectorInfo sectors in PLGlobal.Instance.Galaxy.AllSectorInfos.Values)
            {
                if (sectors.Name == "The Core(MOD)") return;
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
            plsectorInfo3.Position = new Vector3(0, 0, 0.0001f);
            plsectorInfo3.FactionStrength = 0.5f;
            plsectorInfo3.Name = "The Core(MOD)";
        }
    }
    [HarmonyPatch(typeof(PLShop_General), "CreateInitialWares")]
    class InitialStore
    {
        static void Postfix(PLShop_General __instance, TraderPersistantDataEntry inPDE)
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
                UpdateCore();
            }
        }
        public static void UpdateCore()
        {
            TheCoreComms comms;
            PLHailTarget_CustomGeneralShop old = Object.FindObjectOfType<PLHailTarget_CustomGeneralShop>();
            if (old == null) return;
            if (!(old is TheCoreComms))
            {
                if (Object.FindObjectOfType<TheCoreComms>() == null)
                {
                    Object.FindObjectOfType<PLShop_General>().gameObject.AddComponent<TheCoreComms>();
                }
                comms = Object.FindObjectOfType<TheCoreComms>();
                comms.MyShop = old.MyShop;
                comms.MyShop.Name = "The Core";
                comms.MyShop.Desc = "A very special vendor";
                comms.MyShop.ContrabandDealer = true;
                Object.Destroy(old);
            }
            PLShopkeeper keep = Object.FindObjectOfType<PLShopkeeper>();
            keep.Name = "Davy";
            keep.ActorInstance.DisplayName = "Davy";
        }
    }
    [HarmonyPatch(typeof(PLDialogueActorInstance), "BeginDialogue")]
    class DavySpeak
    {
        static void Postfix(PLDialogueActorInstance __instance)
        {
            if (__instance.DisplayName == "Davy" && __instance.GetCurrentLine() != null)
            {
                LineData data = __instance.GetCurrentLine();
                data.TextOptions = new List<string>() { "How you doing? Please buy stuff, I need energy :(." };
                __instance.SetCurrentLine(data);
            }
        }
    }

    class TheCoreComms : PLHailTarget_CustomGeneralShop
    {
        public static bool soldIntergalatic = false;
        private string defaultText = "Welcome to the core, not sure how you found me in here, but doesn't matter. I have the most exotic components of all the galaxy, the other shops have no chance against me. Also Ignore the big shiny center of the galaxy and buy something!";
        public override void Start()
        {
            base.Start();
            this.m_AllChoices.Clear();
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Browse Goods", false), new PLHailChoiceDelegate(this.OnSelectBrowseGoods)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Install Ship Components", false), new PLHailChoiceDelegate(this.OnSelectInstallComp)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Tell me more about you", new PLHailChoiceDelegate(this.OnTalkWith)));
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
                m_AllChoices.Clear();
                currentText += "\n\nMore about me? Sure, ask away";
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Why did you come to this galaxy?", new PLHailChoiceDelegate(this.WhyMoveHere)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("How did you get all this components?", new PLHailChoiceDelegate(this.HowGotThing)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Why did you set your shop in the core of the galaxy?", new PLHailChoiceDelegate(this.WhyCore)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("What is that general store I can teleport?", new PLHailChoiceDelegate(this.WhyStore)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Will you search the Lost Colony?", new PLHailChoiceDelegate(this.Colony)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Nevermind, just want to go back to buying", new PLHailChoiceDelegate(this.BackToBuy)));
            }
        }
        private void WhyMoveHere(bool authority, bool local)
        {
            if (local)
            {
                currentText = "Why did I move here? I guess it was the wheater, I hearded Karattis has a good wheater or something like that. Also I did cross almost all galaxies on the know universe serching for some components for my fine collection and I heard about this Infected" +
                    " that where spreading trought your sectors and thought maybe there is something useful coming from them, and I wasn't wrong! I got this cool infected turret. And with the intergalatic warp network disabled I am now stuck here (maybe I should have bought that ultimate explorer MK3)";
            }
        }
        private void HowGotThing(bool authority, bool local)
        {
            if (local)
            {
                currentText = "My components? I found them during the years talking with exotic dealers from multiple galaxies, still didn't have time to buy from the dealers from this galaxy, I have hearded about this Thermocore and the Corbin's Wall, maybe I will buy them before the intergalatic warp network is activated again. If you bring them I will buy... And that's it." +
                    "If you think I don't sell enough components, maybe visit me in another timeline, at least that is what Skarg said.";
            }
        }
        private void WhyCore(bool authority, bool local)
        {
            if (local)
            {
                currentText = "Why the Core? I came here to the center of the galaxy mostly to avoid the C.U actually. Even if the outpost is not far, my low EM is hidden in the galaxy's core (still not sure how you found me here). ";
                if (PLServer.Instance.CrewFactionID == 0)
                {
                    currentText += "And I know you guys are a C.U crew, but I have a felling you won't report me. ";
                }
                currentText += "Anyway, talking about hidding, I hearded the Estate is actually in this galaxy, apperently with the warp drive malfunctioning or something.";
                if(!soldIntergalatic)   currentText += " But they could be my ticket to getting out of here, just need to find them first. If you find the schematics for a flagship drive you could bring it to me, I would pay good for it *laughs*";
                else 
                {
                    currentText += " But since you got me the schematics somehow, I will be building my own intergalatic warpdrive soon to get out of this galaxy";
                }
            }
        }
        private void WhyStore(bool authority, bool local)
        {
            if (local)
            {
                currentText = "The general shop? I don't sell exotic items here, only components. That robot that came with is just Davy, he probably will sell you things normally found around. And don't worry he is not a slave, I pay him.... with energy.";
            }
        }
        private void Colony(bool authority, bool local)
        {
            if (local)
            {
                currentText = "The Lost Colony? I didn't recive anything due to the Core interference....\nOh, some kind of hidden treasure and big power that could change the course of history forever? I am no treasure hunter, whatever is there, if it is something interesting I will buy later. This galaxy of yours" +
                    " has a lot of random shit happening, at this point I wouldn't be surprised if some alien robot army was trying to kill us all.";
            }
        }
        private void BackToBuy(bool authority, bool local)
        {
            if (local)
            {
                m_AllChoices.Clear();
                currentText = defaultText;
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Browse Goods", false), new PLHailChoiceDelegate(this.OnSelectBrowseGoods)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Install Ship Components", false), new PLHailChoiceDelegate(this.OnSelectInstallComp)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Tell me more about you", new PLHailChoiceDelegate(this.OnTalkWith)));
            }
        }
        public override string GetName()
        {
            return "The Core";
        }
        public override string GetCurrentDialogueText()
        {
            if (currentText == defaultText && soldIntergalatic && !currentText.Contains("Flagship"))
            {
                defaultText = "Welcome to the core, you... sold me the Flagship Intergalactic Warp Schematics? Thanks, how did you get that? Doesn't matter, I should be able to build this in the next month with Davy's help. In the time being, I have the most exotic components of all the galaxy, the other shops have no chance against me." +
                    " Also Ignore the big shiny center of the galaxy and buy something!";
                currentText = defaultText;
            }
            if (currentText == null) currentText = defaultText;
            return currentText;
        }
        private string currentText;
    }
}
