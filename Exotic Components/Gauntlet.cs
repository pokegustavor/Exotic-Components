using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using PulsarModLoader;
using static Exotic_Components.Gauntlet.Patches;
using System.Collections;
using System.Web;
namespace Exotic_Components
{
    public class StartTrial : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            PLSectorInfo trial = Gauntlet.GauntletArena;
            if (trial != null)
            {
                PLPlayer player = PLServer.GetPlayerForPhotonPlayer(sender.sender);
                if (PhotonNetwork.isMasterClient)
                {
                    PLServer.Instance.CPEI_HandleActivateWarpDrive(PLEncounterManager.Instance.PlayerShip.ShipID, trial.ID, player.GetPlayerID());
                    PLEncounterManager.Instance.PlayerShip.NumberOfFuelCapsules += 1;
                    if(Gauntlet.CurrentWildTrial == 0)Gauntlet.WildSeed = Random.Range(-1000000, 1000001);
                    Gauntlet.Patches.ArenaHeart.currentSeed = int.MinValue;
                }
                Gauntlet.WildTrial = (bool)arguments[0];
                Gauntlet.Patches.ArenaHeart.currentWave = 0;
            }
        }
    }
    public class SyncTrial : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (sender.sender.IsMasterClient)
            {
                Gauntlet.CurrentTrial = (int)arguments[0];
                Gauntlet.CurrentWildTrial = (int)arguments[1];
                Gauntlet.WildTrial = (bool)arguments[2];
            }
        }
    }
    public class StopWild : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            Gauntlet.WildTrial = false;
            Gauntlet.CurrentWildTrial = 0;
        }
    }
    public class RecieveGauntlet : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            Gauntlet.Patches.EnsureGauntletComms.CreateGauntlet((int)arguments[0]);
        }
    }
    public class WaveTitle : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (sender.sender.IsMasterClient)
            {
                FixBossBar.Title = (string)arguments[0];
                FixBossBar.SubTitle = (string)arguments[1];
                FixBossBar.LastTitle = Time.time;
            }
        }
    }
    public class Gauntlet
    {
        public static int CurrentTrial = 0;
        public static bool WildTrial = false;
        public static int CurrentWildTrial = 0;
        public static int WildSeed = -1;
        static PLSectorInfo _GauntletArena;
        static PLSectorInfo _GauntletLobby;
        public static PLSectorInfo GauntletArena
        {
            get
            {
                if (_GauntletArena == null)
                {
                    foreach (PLSectorInfo sector in PLGlobal.Instance.Galaxy.AllSectorInfos.Values)
                    {
                        if (sector.Name == "Gauntlet_Arena()")
                        {
                            _GauntletArena = sector;
                            break;
                        }
                    }
                }
                return _GauntletArena;
            }
        }
        public static PLSectorInfo GauntletLobby
        {
            get
            {
                if (_GauntletLobby == null)
                {
                    foreach (PLSectorInfo sector in PLGlobal.Instance.Galaxy.AllSectorInfos.Values)
                    {
                        if (sector.Name == "Gauntlet_Lobby()")
                        {
                            _GauntletLobby = sector;
                            break;
                        }
                    }
                }
                return _GauntletLobby;
            }
        }
        internal static void CreateGauntlet()
        {
            int minimumFreeSectorNumber = PLGlobal.Instance.Galaxy.GetMinimumFreeSectorNumber();
            if (minimumFreeSectorNumber != -1) //Warp gate-Entrance
            {
                PLSectorInfo plsectorInfo = new PLSectorInfo();
                plsectorInfo.Discovered = false;
                plsectorInfo.Visited = false;
                PLGlobal.Instance.Galaxy.AllSectorInfos.Add(minimumFreeSectorNumber, plsectorInfo);
                plsectorInfo.ID = minimumFreeSectorNumber;
                plsectorInfo.MySPI = SectorProceduralInfo.Create(PLGlobal.Instance.Galaxy, ref plsectorInfo, plsectorInfo.ID);
                plsectorInfo.FactionStrength = 0.5f;
                plsectorInfo.VisualIndication = ESectorVisualIndication.PT_WARP_GATE;
                plsectorInfo.MySPI.Faction = 5;
                plsectorInfo.MissionSpecificID = -10;
                plsectorInfo.Position = PLServer.GetSectorPositionAtDistance(12);
                plsectorInfo.Name = "The Gauntlet";
                plsectorInfo.LockedToFaction = true;
            }
            minimumFreeSectorNumber = PLGlobal.Instance.Galaxy.GetMinimumFreeSectorNumber();
            if (minimumFreeSectorNumber != -1) //Main Hub
            {
                PLSectorInfo plsectorInfo = new PLSectorInfo();
                plsectorInfo.Discovered = false;
                plsectorInfo.Visited = false;
                PLGlobal.Instance.Galaxy.AllSectorInfos.Add(minimumFreeSectorNumber, plsectorInfo);
                plsectorInfo.ID = minimumFreeSectorNumber;
                plsectorInfo.MySPI = SectorProceduralInfo.Create(PLGlobal.Instance.Galaxy, ref plsectorInfo, plsectorInfo.ID);
                plsectorInfo.FactionStrength = 0.5f;
                plsectorInfo.VisualIndication = ESectorVisualIndication.GENERAL_STORE;
                plsectorInfo.MySPI.Faction = 5;
                plsectorInfo.Position = new Vector3(-12, 12);
                plsectorInfo.Name = "Gauntlet_Lobby()";
                _GauntletLobby = plsectorInfo;
            }
            minimumFreeSectorNumber = PLGlobal.Instance.Galaxy.GetMinimumFreeSectorNumber();
            if (minimumFreeSectorNumber != -1) //Arena
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
                plsectorInfo.Position = new Vector3(-18, 12);
                plsectorInfo.Name = "Gauntlet_Arena()";
                _GauntletArena = plsectorInfo;
            }
        }

        class GauntletComms : PLHailTarget_CustomGeneralShop
        {
            bool wildTrialText = false;
            bool shouldBeWild = false;
            float lastsync = Time.time;
            private string currentText;
            private readonly string defaultText = "Welcome to the gauntlet! Here you will face challanges for a chance of winning scrap, credits and maybe even a special reward, if you can survive... I have created an atmosphere similar to a \"general shop\" to keep you comfortable.";
            public override void Start()
            {
                base.Start();
                this.m_AllChoices.Clear();
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Browse Goods", false), new PLHailChoiceDelegate(this.OnSelectBrowseGoods)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Install Ship Components", false), new PLHailChoiceDelegate(this.OnSelectInstallComp)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Start Gauntlet", new PLHailChoiceDelegate(this.StartGauntlet)));
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Leave", new PLHailChoiceDelegate(this.OnLeave)));
            }
            public override void Update()
            {
                base.Update();
                if (PhotonNetwork.isMasterClient && Time.time - lastsync > 5)
                {
                    lastsync = Time.time;
                    ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.SyncTrial", PhotonTargets.Others, new object[]
                    {
                        CurrentTrial,
                        CurrentWildTrial,
                        WildTrial
                    });
                }
                if (WildTrial && CurrentWildTrial > 0 && !wildTrialText)
                {
                    WildTrialText(false, true);
                    wildTrialText = true;
                }
                else if (wildTrialText && (!WildTrial || CurrentWildTrial <= 0))
                {
                    Default(false, true);
                    wildTrialText = false;
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
            private void StartGauntlet(bool authority, bool local)
            {
                if (local)
                {
                    m_AllChoices.Clear();
                    shouldBeWild = false;
                    currentText = "In the gauntlet you will be required to defeat waves of enemies non stop and a boss, if you survive you will return here and recieve your rewards for that round. This is purely a ship to ship combat, so boarding is NOT allowed.";
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("\"The Trial\"", new PLHailChoiceDelegate(this.TheTrial)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("\"The Wild Trial\"", new PLHailChoiceDelegate(this.TheWildTrial)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate(this.Default)));
                }
            }
            private void TheTrial(bool authority, bool local)
            {
                if (local)
                {
                    m_AllChoices.Clear();
                    if (CurrentTrial < 10)
                    {
                        currentText = "The Trial is a premade sequence of rounds created with the help of Bolgath and Skarg, we want to see your combat ability tested in specific scenarios.\n" + GetTextForCurrentTrial();
                        this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Start", new PLHailChoiceDelegate(this.StartTrial)));
                    }
                    else
                    {
                        currentText = "You have already complete our trials, we had the fun we wanted and you have your fragment, credits and scrap, but if you still need more you can always participate in the wild trials instead.";
                    }
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate(this.StartGauntlet)));
                }
            }
            private void StartTrial(bool authority, bool local)
            {
                if (local)
                {
                    ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.StartTrial", PhotonTargets.All, new object[]
                        {
                            shouldBeWild
                        });
                }
            }
            private void TheWildTrial(bool authority, bool local)
            {
                if (local)
                {
                    m_AllChoices.Clear();
                    shouldBeWild = true;
                    currentText = "The wild trial is an endeless trial where the objective is to see how far you will go, each round will consist of 2 waves and a boss, in between each round you will return here for repairs and rearm. The difficulty and rewards will grow with each round, are you ready to start?";
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Start", new PLHailChoiceDelegate(this.StartTrial)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate(this.StartGauntlet)));
                }
            }
            private void WildTrialText(bool authority, bool local)
            {
                if (local)
                {
                    m_AllChoices.Clear();
                    shouldBeWild = true;
                    currentText = "Welcome back, feel free to prepare for the next wild round, or if you want you can cash out right now with whatever rewards you got.";
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Browse Goods", false), new PLHailChoiceDelegate(this.OnSelectBrowseGoods)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Install Ship Components", false), new PLHailChoiceDelegate(this.OnSelectInstallComp)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Continue Wild Trial", new PLHailChoiceDelegate(this.StartTrial)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cash out", new PLHailChoiceDelegate(this.CashOut)));
                }
            }
            private void CashOut(bool authority, bool local)
            {
                if (local)
                {
                    ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.StopWild", PhotonTargets.All, new object[0]);
                }
            }
            private void OnLeave(bool authority, bool local)
            {
                if (local)
                {
                    m_AllChoices.Clear();
                    currentText = "So you wish to leave the Gauntlet, are you sure?";
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Yes", new PLHailChoiceDelegate(this.WarpAway)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("No", new PLHailChoiceDelegate(this.Default)));
                }
            }
            private void WarpAway(bool authority, bool local)
            {
                if (local)
                {
                    PLSectorInfo gate = PLGlobal.Instance.Galaxy.GetSectorOfVisualIndication(ESectorVisualIndication.PT_WARP_GATE);
                    if (gate != null)
                    {
                        PLServer.Instance.photonView.RPC("CPEI_HandleActivateWarpDrive", PhotonTargets.MasterClient, new object[]
                        {
                            PLEncounterManager.Instance.PlayerShip.ShipID,
                            gate.ID,
                            PLNetworkManager.Instance.LocalPlayerID
                        });
                    }
                }
            }
            private void Default(bool authority, bool local)
            {
                if (local)
                {
                    currentText = defaultText;
                    this.m_AllChoices.Clear();
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Browse Goods", false), new PLHailChoiceDelegate(this.OnSelectBrowseGoods)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom(PLLocalize.Localize("Install Ship Components", false), new PLHailChoiceDelegate(this.OnSelectInstallComp)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Start Gauntlet", new PLHailChoiceDelegate(this.StartGauntlet)));
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Leave", new PLHailChoiceDelegate(this.OnLeave)));
                }
            }
            public override string GetName()
            {
                return "The Gauntlet";
            }
            public override string GetCurrentDialogueText()
            {
                if (currentText == null) currentText = defaultText;
                return PLDialogueActorInstance.AddNewLinesToText(currentText, false, 70, true);
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
            string GetTextForCurrentTrial()
            {
                switch (CurrentTrial)
                {
                    default:
                        return string.Empty;
                    case 0:
                        return "This is the first round, so lets start simple, all enemies will be using components normally found across your galaxy";
                    case 1:
                        return "For this next round lets start to spice things up, the enemies will now have some exotic and expermiental components, also you may see some \"uncommon\" foes.";
                    case 2:
                        return "You better prepare your repair guns, because this next round will \"phase\" through your ship.";
                    case 3:
                        return "I hope you have a strong attack, this next round will be composed of heavily armored ships.";
                    case 4:
                        return "Preprare your dodging skills, the ships here are specialized in ignoring most of your defenses.";
                    case 5:
                        return "Lets test your hull strengh, this next challange will make your shields \"non-existent\", and your ship may overcharge...";
                    case 6:
                        return "Did you remember to upgrade your EM sensors? Because you might come to regret not doing so...";
                    case 7:
                        return "The Infected have become a problem in your timeline right? So lets test your ability to kill bugs.";
                    case 8:
                        return "The challanges are coming to an end, before the final showdown lets have a final round of heavy hitters.";
                    case 9:
                        return "This is it, if you can come out of this alive you will recieve a very special reward that will aid you in your journey. But be warned, it will be a real battle of endurance.";
                }
            }
        }
        internal class Patches
        {
            [HarmonyPatch(typeof(PLPTWarpgate), "Update")]
            class FixGauntletWarp
            {
                static void Postfix(PLPTWarpgate __instance)
                {
                    if (PLServer.GetCurrentSector().Name.Contains("The Gauntlet") && __instance.TargetSector == null)
                    {
                        __instance.TargetSector = GauntletLobby;
                    }
                }
            }

            [HarmonyPatch(typeof(PLStarmap), "ShouldShowSector")]
            class ShowIcon
            {
                static void Postfix(PLSectorInfo sectorInfo, ref bool __result)
                {
                    if (sectorInfo.Name.Contains("The Gauntlet"))
                    {
                        __result = true;
                    }
                }
            }
            [HarmonyPatch(typeof(PLStarmap), "ShouldShowSectorBG")]
            class ShowIcon2
            {
                static void Postfix(PLSectorInfo sectorInfo, ref bool __result)
                {
                    if (sectorInfo.Name.Contains("The Gauntlet"))
                    {
                        __result = true;
                    }
                }
            }
            [HarmonyPatch(typeof(PLShipInfo), "UpdateAllHailTargetsList")]
            public class EnsureGauntletComms
            {
                static void Postfix()
                {
                    GauntletComms comms = Object.FindObjectOfType<GauntletComms>();
                    if (comms != null)
                    {
                        ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.RecieveGauntlet", PhotonTargets.Others, new object[]
                        {
                    comms.GetHailTargetID()
                        });
                    }
                    else if (PhotonNetwork.isMasterClient && PLServer.GetCurrentSector().Name == "Gauntlet_Lobby()") 
                    {
                        UpdateGauntlet();
                    }
                }

                public static void CreateGauntlet(int ID)
                {
                    foreach (PLHailTarget target in PLHailTarget.AllHailTargets)
                    {
                        if (target is GauntletComms) return;
                    }
                    UpdateGauntlet();
                    GauntletComms comms = Object.FindObjectOfType<GauntletComms>();
                    comms.HailTargetID = ID;
                }

                public static void UpdateGauntlet()
                {
                    GauntletComms comms;
                    PLHailTarget_CustomGeneralShop old = Object.FindObjectOfType<PLHailTarget_CustomGeneralShop>();
                    if (old == null) return;
                    if (!(old is GauntletComms))
                    {
                        if (Object.FindObjectOfType<GauntletComms>() == null)
                        {
                            Object.FindObjectOfType<PLHailTarget_CustomGeneralShop>().gameObject.AddComponent<GauntletComms>();
                        }
                        comms = Object.FindObjectOfType<GauntletComms>();
                        comms.MyShop = old.MyShop;
                        comms.MyShop.Name = "The Gauntlet";
                        comms.MyShop.Desc = "A mysterious place";
                        comms.MyShop.ContrabandDealer = true;
                        Object.Destroy(old);
                    }
                }
            }
            [HarmonyPatch(typeof(PLTraderInfo), "Update")]
            class CreateGauntletComms
            {
                static bool reseted = false;
                static void Postfix(PLTraderInfo __instance)
                {
                    if (PLEncounterManager.Instance.PlayerShip != null && PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().Name == "Gauntlet_Lobby()" && __instance is PLShop_General)
                    {
                        if (PLEncounterManager.Instance.PlayerShip.InWarp)
                        {
                            reseted = false;
                        }
                        else if (!reseted)
                        {
                            EnsureGauntletComms.UpdateGauntlet();
                            reseted = true;
                        }
                    }
                }
            }
            [HarmonyPatch(typeof(PLShipStats), "Update")]
            class EnsureQuantumDefense
            {
                static void Postfix(PLShipStats __instance)
                {
                    if (PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().Name == "Gauntlet_Arena()")
                    {
                        __instance.QuantumShieldDefensesActive = true;
                        __instance.QuantumShieldsCanBeEnabled = true;
                        __instance.Ship.IsQuantumShieldActive = true;
                    }
                }
            }
            [HarmonyPatch(typeof(PLWarpDriveScreen), "Update")]
            class WarpDisable
            {
                static void Postfix(UISprite ___WarpDrivePanel, PLWarpDriveScreen __instance, UISprite ___JumpComputerPanel, UISprite ___m_BlockingTargetOnboardPanel)
                {
                    if (PLServer.GetCurrentSector() != null && (PLServer.GetCurrentSector().Name == "Gauntlet_Arena()" || PLServer.GetCurrentSector().Name == "Gauntlet_Lobby()"))
                    {
                        PLGlobal.SafeGameObjectSetActive(___WarpDrivePanel.gameObject, false);
                        PLGlobal.SafeGameObjectSetActive(___JumpComputerPanel.gameObject, false);
                        PLGlobal.SafeGameObjectSetActive(___m_BlockingTargetOnboardPanel.gameObject, true);
                    }
                }
            }
            [HarmonyPatch(typeof(PLShipInfoBase), "Ship_WarpOutNow")]
            class DisableEnemyWarp 
            {
                static bool Prefix()
                {
                    if (PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().Name == "Gauntlet_Arena()")
                    {
                        return false;
                    }
                    return true;
                }
            }

            [HarmonyPatch(typeof(PLPersistantEncounterInstance), "PlayMusicBasedOnShipType")]
            class ArenaMusic 
            {
                static void Postfix() 
                {
                    if(PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().Name == "Gauntlet_Arena()" && PLEncounterManager.Instance.PlayerShip != null && !PLEncounterManager.Instance.PlayerShip.InWarp && PLMusic.Instance.CurrentPlayingMusicEventString != "mx_ivm_dangerousenv") 
                    {
                       PLMusic.Instance.PlayMusic("mx_ivm_dangerousenv", true, false, true, true);
                    }
                }
            }

            [HarmonyPatch(typeof(PLShipInfoBase),"Update")]
            class EnsureAggro 
            {
                static void Postfix(PLShipInfoBase __instance) 
                {
                    if(PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().Name == "Gauntlet_Arena()" && PLEncounterManager.Instance.PlayerShip != null && __instance.TargetShip != PLEncounterManager.Instance.PlayerShip) 
                    {
                        __instance.TargetShip = PLEncounterManager.Instance.PlayerShip;
                    }
                }
            }

            [HarmonyPatch(typeof(PLTabMenu), "GetFragmentInfo")]

            class GauntletFragmentText 
            {
                static void Postfix(int fragID, ref string fragName, ref string fragDesc) 
                {
                    if(fragID == 5) 
                    {
                        fragName = "Gauntlet Fragment";
                        fragDesc = "Decreases the upgrade cost by 50%";
                    }
                    /*
                    switch (fragID)
                    {
                        case 0:
                            fragName = "A.O.G. Fragment";
                            fragDesc = "Your ship will become unflagged each jump";
                            return;
                        case 1:
                            fragName = "Auction Fragment";
                            fragDesc = "Gain access to special deals in general stores across the galaxy";
                            return;
                        case 2:
                            fragName = "Colonial Union Fragment";
                            fragDesc = "+50% max shield integrity\n+15% warp range";
                            return;
                        case 3:
                            fragName = "High Roller Fragment";
                            fragDesc = "Random chance to either gain credits, processed scrap or coolant each jump";
                            return;
                        case 5:
                            fragName = "Gauntlet Fragment";
                            fragDesc = "Decreases the upgrade cost by 50%";
                            return;
                        default:
                            fragName = "Fragment";
                            fragDesc = "";
                            return;
                        case 6:
                            fragName = "Fluffy Biscuit Fragment";
                            fragDesc = "2x fluffy oven food supply\n2x credits earned for biscuit sales";
                            return;
                        case 7:
                            fragName = "Grey Huntsmen Fragment";
                            fragDesc = "No effect";
                            return;
                        case 8:
                            fragName = "Warger's Fragment";
                            fragDesc = "No effect";
                            return;
                        case 9:
                            fragName = "Cursed Fragment";
                            fragDesc = "A supposedly 'cursed' data fragment. If you believe in that sort of thing ...";
                            return;
                        case 10:
                            fragName = "Racing Fragment";
                            fragDesc = "+30% thrust";
                            return;
                        case 11:
                            fragName = "Commander Fragment";
                            fragDesc = "5% hull repair per jump";
                            return;
                        case 12:
                            fragName = "Cypher Fragment";
                            fragDesc = "Increases ship program slots to 12";
                            return;
                        case 13:
                            fragName = "W.D. Corp Fragment";
                            fragDesc = "Installed tracker missiles are refilled by 10% each jump";
                            return;
                    }
                    */
                }
            }

            [HarmonyPatch(typeof(PLShipInfo), "GetMatCostForComp")]
            class ShipComponentsFragment 
            {
                static void Postfix(ref int __result) 
                {
                    if (PLServer.Instance.IsFragmentCollected(5)) 
                    {
                        __result /= 2;
                    }
                }
            }
            [HarmonyPatch(typeof(PLShipInfo), "GetMatCostForItem")]
            class PawnItemsFragment
            {
                static void Postfix(ref int __result)
                {
                    if (PLServer.Instance.IsFragmentCollected(5))
                    {
                        __result /= 2;
                    }
                }
            }

            [HarmonyPatch(typeof(PLServer), "GetSectorPositionAtDistance")]
            class PreventMissionsOnVoid 
            {
                static void Postfix(ref Vector3 __result, float inDist) 
                {
                    PLSectorInfo plsectorInfo = PLServer.GetCurrentSector();
                    if(plsectorInfo.Name == "Gauntlet_Arena()" || plsectorInfo.Name == "Gauntlet_Lobby()") 
                    {
                        plsectorInfo = PLGlobal.Instance.Galaxy.GetSectorOfVisualIndication(ESectorVisualIndication.PT_WARP_GATE);
                        if (PLEncounterManager.Instance.PlayerShip != null && PLEncounterManager.Instance.PlayerShip.WarpTargetID != -1 && PLGlobal.Instance.Galaxy.AllSectorInfos.ContainsKey(PLEncounterManager.Instance.PlayerShip.WarpTargetID))
                        {
                            plsectorInfo = PLGlobal.Instance.Galaxy.AllSectorInfos[PLEncounterManager.Instance.PlayerShip.WarpTargetID];
                        }
                        if (plsectorInfo != null)
                        {
                            for (int i = 0; i < 1500; i++)
                            {
                                Vector3 vector = plsectorInfo.Position + new Vector3(UnityEngine.Random.Range(-inDist, inDist), UnityEngine.Random.Range(-inDist, inDist)) * PLServer.CampaignEditorDistanceToGalaxyDistance * PLGlobal.Instance.Galaxy.GenGalaxyScale;
                                if ((vector.sqrMagnitude < 0.5f * PLGlobal.Instance.Galaxy.GenGalaxyScale || vector.sqrMagnitude < plsectorInfo.Position.sqrMagnitude) && !PLGlobal.Instance.Galaxy.IsSectorWithinRange2D(new Vector2(vector.x, vector.y), 0.016f))
                                {
                                    __result = vector;
                                    return;
                                }
                            }
                        }
                        __result = Vector3.zero;
                    }
                }
            }

            [HarmonyPatch(typeof(PLEncounterManager), "Update")]
            internal class ArenaHeart
            {
                static PLRand RNG = null;
                internal static int currentSeed = -1;
                internal static int currentWave = 0;
                static float WaveDelay = Time.time;
                static void Postfix()
                {
                    if (!PhotonNetwork.isMasterClient || PLServer.GetCurrentSector() == null || PLServer.GetCurrentSector().Name != "Gauntlet_Arena()" || PLEncounterManager.Instance.PlayerShip == null || PLEncounterManager.Instance.PlayerShip.InWarp) return;
                    if(WildTrial && (currentSeed != WildSeed + CurrentWildTrial || RNG == null)) 
                    {
                        RNG = new PLRand(WildSeed + CurrentWildTrial);
                        currentSeed = WildSeed + CurrentWildTrial;
                    }
                    int shipCount = 0;
                    foreach (PLShipInfoBase ship in PLEncounterManager.Instance.AllShips.Values)
                    {
                        if (ship != null && !ship.GetIsPlayerShip())
                        {
                            shipCount++;
                        }
                    }
                    if (shipCount == 0)
                    {
                        if (currentWave < 3) NextWave();
                        else
                        {
                            WaveDelay = Time.time;
                            PLServer.Instance.CPEI_HandleActivateWarpDrive(PLEncounterManager.Instance.PlayerShip.ShipID, GauntletLobby.ID, 0);
                            PLEncounterManager.Instance.PlayerShip.NumberOfFuelCapsules += 1;
                            if (WildTrial)
                            {
                                CurrentWildTrial++;
                            }
                            else
                            {
                                CurrentTrial++;
                            }
                            GenerateRewards(WildTrial ? CurrentWildTrial : CurrentTrial, WildTrial);
                        }
                    }
                }
                static void NextWave()
                {
                    if (Time.time - WaveDelay < 5) return;
                    currentWave++;
                    if (currentWave <= 3)
                    {
                        ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.WaveTitle", PhotonTargets.All, new object[]
                        {
                            $"Wave {currentWave}/3",
                            SubTitleForWave(WildTrial,WildTrial? CurrentWildTrial : CurrentTrial,currentWave)
                        });
                        PLPersistantShipInfo plpersistantShipInfo;
                        PLShipInfoBase pLShipInfoBase;
                        List<ComponentOverrideData> overrides;
                        if (!WildTrial)
                        {
                            switch (CurrentTrial)
                            {
                                case 0: //"Tutorial"
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE3, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INTREPID, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            pLShipInfoBase.IsRelicHunter = true;
                                            pLShipInfoBase.ShipName = "The Tutor";
                                            break;
                                    }
                                    break;

                                case 1: //"Exotic Tutorial"
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                            new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Nano Active MK2"), ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                                            new ComponentOverrideData() { CompType = 3, CompSubType = 6, ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                            new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("PhaserTurret"), ReplaceExistingComp = true, CompLevel = 0, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                                            new ComponentOverrideData() { CompType = 3, CompSubType = 6, ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                            new ComponentOverrideData() { CompType = 1, CompSubType = 13, ReplaceExistingComp = true, CompLevel = 1, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                                            new ComponentOverrideData() { CompType = 3, CompSubType = 6, ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_ABYSS_FIGHTER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_UNSEEN_FIGHTER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_MATRIX_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            GenerateBoss(0, 1);
                                            break;
                                    }
                                    break;

                                case 2: // Phase damage
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PHASE_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PHASE_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PHASE_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PHASE_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PHASE_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_UNSEEN_FIGHTER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 10, CompSubType = 11, ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                                                new ComponentOverrideData() { CompType = 10, CompSubType = 11, ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_UNSEEN_FIGHTER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 10, CompSubType = 11, ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                                                new ComponentOverrideData() { CompType = 10, CompSubType = 11, ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            GenerateBoss(1, 2);
                                            break;
                                    }
                                    break;

                                case 3: // Armored
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 1, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 1, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 1, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 1, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE3, 1, PLServer.GetCurrentSector(), 1, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 1, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            GenerateBoss(2, 3);
                                            break;
                                    }
                                    break;

                                case 4: // Shield Hull phase damage
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_MATRIX_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 9, CompSubType = 3, ReplaceExistingComp = true, CompLevel = 6, IsCargo = false, CompTypeToReplace = 9, SlotNumberToReplace = 1},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_MATRIX_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 9, CompSubType = 3, ReplaceExistingComp = true, CompLevel = 6, IsCargo = false, CompTypeToReplace = 9, SlotNumberToReplace = 1},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_MATRIX_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 9, CompSubType = 3, ReplaceExistingComp = true, CompLevel = 6, IsCargo = false, CompTypeToReplace = 9, SlotNumberToReplace = 1},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_DEATHSEEKER_DRONE, 1, PLServer.GetCurrentSector(), 3, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_DEATHSEEKER_DRONE, 1, PLServer.GetCurrentSector(), 3, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("PhaseShieldTurret"), ReplaceExistingComp = true, CompLevel = 2, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                                                new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Anti-Shield"), ReplaceExistingComp = true, CompLevel = 3, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                                                new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("HullSmasher"), ReplaceExistingComp = true, CompLevel = 3, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_REPAIR_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            GenerateBoss(3, 4);
                                            break;
                                    }
                                    break;

                                case 5: // Lightning/ overcharge
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_SHOCK_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_SHOCK_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_SHOCK_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_SHOCK_DRONE, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 3, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = 0, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                                                new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = 0, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                                                new ComponentOverrideData() { CompType = 3, CompSubType = 6, ReplaceExistingComp = true, CompLevel = 4, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            int beaconID = 0;
                                            for (int i = 0; i < 3000; i++) 
                                            {
                                                PLRand random = new PLRand(i + PLServer.Instance.GalaxySeed + plpersistantShipInfo.MyCurrentSector.ID);
                                                if (random.Next() % 18 == 13 && random.NextFloat() >= 0.05f) 
                                                {
                                                    beaconID = i; 
                                                    break;
                                                }
                                            }
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_BEACON, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            plpersistantShipInfo.BeaconIDInSector = beaconID;
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            GenerateBoss(4, 5);
                                            break;
                                    }
                                    break;

                                case 6: //Stealth
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 3, CompSubType = 5, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 3, CompSubType = 5, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE1, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 3, CompSubType = 5, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PTDRONE, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 3, CompSubType = 5, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PTDRONE, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 3, CompSubType = 5, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_PTDRONE, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 3, CompSubType = 5, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            break;
                                        case 3:
                                            GenerateBoss(5, 6);
                                            break;
                                    }
                                    break;

                                case 7: //Infected
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INFECTED_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INFECTED_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INFECTED_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INFECTED_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INFECTED_FIGHTER_HEAVY, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INFECTED_FIGHTER_HEAVY, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INFECTED_FIGHTER_HEAVY, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            GenerateBoss(6, 8);
                                            break;
                                    }
                                    break;

                                case 8: //Heavy attack
                                    switch (currentWave)
                                    {
                                        case 1:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 11, CompSubType = 5, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 11, CompSubType = 3, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDDRONE2, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 11, CompSubType = 4, ReplaceExistingComp = true, CompLevel = 11, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            break;
                                        case 2:
                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_ABYSS_HEAVY_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 6, CompSubType = 2, ReplaceExistingComp = true, CompLevel = 4, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_ABYSS_HEAVY_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 6, CompSubType = 2, ReplaceExistingComp = true, CompLevel = 4, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_ABYSS_HEAVY_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 6, CompSubType = 2, ReplaceExistingComp = true, CompLevel = 4, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;

                                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_ABYSS_HEAVY_FIGHTER, 1, PLServer.GetCurrentSector(), 8, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                                            overrides = new List<ComponentOverrideData>
                                            {
                                                new ComponentOverrideData() { CompType = 6, CompSubType = 2, ReplaceExistingComp = true, CompLevel = 4, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                                            };
                                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                                            pLShipInfoBase.DropScrap = false;
                                            pLShipInfoBase.CreditsLeftBehind = 0;
                                            break;
                                        case 3:
                                            GenerateBoss(7, 8);
                                            break;
                                    }
                                    break;

                                case 9: //Final showdown
                                    switch (currentWave)
                                    {
                                        case 1:
                                            GenerateBoss(0, 5);

                                            GenerateBoss(1, 5);
                                            break;
                                        case 2:
                                            GenerateBoss(2, 5);

                                            GenerateBoss(5, 5);
                                            break;
                                        case 3:
                                            GenerateBoss(8, 9);
                                            break;
                                    }
                                    break;
                            }
                        }
                        else 
                        {
                            if(currentWave == 1 || currentWave == 2) 
                            {
                                int enemyCount = 6;
                                int bossOdds = 25;
                                if(CurrentWildTrial/10 == 0) 
                                {
                                    enemyCount = RNG.Next(2,5);
                                    bossOdds = 5;
                                }
                                else if (CurrentWildTrial / 10 == 1)
                                {
                                    enemyCount = RNG.Next(3, 5);
                                    bossOdds = 7;
                                }
                                else if (CurrentWildTrial / 10 == 2)
                                {
                                    enemyCount = RNG.Next(4, 6);
                                    bossOdds = 15;
                                }
                                if (currentWave == 2) enemyCount += RNG.Next(0, 2);
                                for(int i = 0; i < enemyCount; i++) 
                                {
                                    if(RNG.Next(0,100) <= bossOdds) 
                                    {
                                        GenerateBoss(Random.Range(0, 8), CurrentWildTrial);
                                    }
                                    else 
                                    {
                                        GenerateRandomDrone(CurrentWildTrial);
                                    }
                                }
                            }
                            else 
                            {
                                if(CurrentWildTrial % 10 != 0 || CurrentWildTrial == 0) 
                                {
                                    GenerateBoss(Random.Range(0, 8), CurrentWildTrial);
                                }
                                else 
                                {
                                    GenerateBoss(8, CurrentWildTrial);
                                }
                            }
                        }
                    }
                }

                static void GenerateRewards(int round, bool isWild)
                {
                    currentWave = 0;
                    if (!isWild)
                    {
                        switch (round)
                        {
                            case 1:
                                PLServer.Instance.CurrentCrewCredits += 20000;
                                PLServer.Instance.CurrentUpgradeMats += 15;
                                break;
                            case 2:
                                PLServer.Instance.CurrentCrewCredits += 30000;
                                PLServer.Instance.CurrentUpgradeMats += 25;
                                break;
                            case 3:
                                PLServer.Instance.CurrentCrewCredits += 45000;
                                PLServer.Instance.CurrentUpgradeMats += 35;
                                break;
                            case 4:
                                PLServer.Instance.CurrentCrewCredits += 60000;
                                PLServer.Instance.CurrentUpgradeMats += 45;
                                break;
                            case 5:
                                PLServer.Instance.CurrentCrewCredits += 80000;
                                PLServer.Instance.CurrentUpgradeMats += 55;
                                break;
                            case 6:
                                PLServer.Instance.CurrentCrewCredits += 100000;
                                PLServer.Instance.CurrentUpgradeMats += 65;
                                break;
                            case 7:
                                PLServer.Instance.CurrentCrewCredits += 110000;
                                PLServer.Instance.CurrentUpgradeMats += 75;
                                break;
                            case 8:
                                PLServer.Instance.CurrentCrewCredits += 120000;
                                PLServer.Instance.CurrentUpgradeMats += 85;
                                break;
                            case 9:
                                PLServer.Instance.CurrentCrewCredits += 150000;
                                PLServer.Instance.CurrentUpgradeMats += 95;
                                break;
                            case 10:
                                PLServer.Instance.CurrentCrewCredits += 200000;
                                PLServer.Instance.CurrentUpgradeMats += 120;
                                PLServer.Instance.photonView.RPC("CollectFragment", PhotonTargets.All, new object[]
                                {
                                  5
                                });
                                break;
                        }
                    }
                    else 
                    {
                        PLServer.Instance.CurrentCrewCredits += Mathf.RoundToInt(15000 * Mathf.FloorToInt(1 + round * 0.75f) * Random.Range(0.75f,1.25f));
                        PLServer.Instance.CurrentUpgradeMats += Mathf.RoundToInt(15 * Mathf.FloorToInt(1 + round * 0.75f) * Random.Range(0.75f, 1.25f));
                    }
                }

                static void GenerateBoss(int ID,int round) 
                {
                    PLPersistantShipInfo plpersistantShipInfo;
                    PLShipInfoBase pLShipInfoBase;
                    List<ComponentOverrideData> overrides;
                    switch (ID) 
                    {
                        case 0: //Temperature Critical
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_OUTRIDER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 1, CompSubType = 11, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Toxic Wall"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("CryoCore MK2"), ReplaceExistingComp = true, CompLevel = round + 1, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("MachineGunMainTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Supreme RailGun"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},

                            };
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "Temperature Critical!";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 1: //Phaser
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.OLDWARS_SYLVASSI, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 1, CompSubType = 11, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("The Phase Driver Hull"), ReplaceExistingComp = true, CompLevel = round , IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("ThermoPoint Reactor"), ReplaceExistingComp = true, CompLevel = round + 3, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("PhaserTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 11, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 7, CompSubType = 14, ReplaceExistingComp = true, CompLevel = round + 4, IsCargo = false, CompTypeToReplace = 7, SlotNumberToReplace = 7},
                               new ComponentOverrideData() { CompType = 20, CompSubType = 9, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 20, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "Phaser";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 2: //The Brick - Armor
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_DESTROYER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Extreme Particle Shield"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("\"The Wall\""), ReplaceExistingComp = true, CompLevel = round , IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 16, CompSubType = PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName("TurtleP"), ReplaceExistingComp = true, CompLevel = 0 , IsCargo = false, CompTypeToReplace = 16, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("Pipe Reactor"), ReplaceExistingComp = true, CompLevel = round + 2, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = 5, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 4, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 24, CompSubType = PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName("AutoLightningTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 24, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 24, CompSubType = PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName("AutoDefenderTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 24, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 7, CompSubType = PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName("Shield Master"), ReplaceExistingComp = true, CompLevel = round - 1, IsCargo = false, CompTypeToReplace = 7, SlotNumberToReplace = 3},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "The Brick";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 3: //Breached - Shield and hull breach
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_ANNIHILATOR, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Quantum Shield"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = 2, ReplaceExistingComp = true, CompLevel = round + 4, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 16, CompSubType = PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName("AntiBreach"), ReplaceExistingComp = true, CompLevel = 0 , IsCargo = false, CompTypeToReplace = 16, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("Modified Strongpoint Reactor"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("PhaseShieldTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Anti-Shield"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("HullSmasher"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 20, CompSubType = PulsarModLoader.Content.Components.Missile.MissileModManager.Instance.GetMissileIDFromName("Armor Denial"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 20, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "Breached";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 4: //The Shutdown - Lightning/overcharge
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_INTREPID, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Electric Wall"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = 2, ReplaceExistingComp = true, CompLevel = round , IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("The Overcharge"), ReplaceExistingComp = true, CompLevel = round+2, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("SuperchargeMainTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 7, CompSubType = 17, ReplaceExistingComp = true, CompLevel = 0, IsCargo = false, CompTypeToReplace = 7, SlotNumberToReplace = 5},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "The Shutdown";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 5: //Death's Whisper - stealth
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_STARGAZER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Layered Shield"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Nano Active MK2"), ReplaceExistingComp = true, CompLevel = round + 3, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 16, CompSubType = PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName("GlassP"), ReplaceExistingComp = true, CompLevel = 0 , IsCargo = false, CompTypeToReplace = 16, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("ZeroPoint Reactor"), ReplaceExistingComp = true, CompLevel = round+9, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("SilentDeath"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 14, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 15, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 5, CompSubType = 0, ReplaceExistingComp = true, CompLevel = 7, IsCargo = false, CompTypeToReplace = 5, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "Death's Whisper";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 6: //Uninfected Carrier - Infected
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_CARRIER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Anti-Infected Shield"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Anti-Infected Hull"), ReplaceExistingComp = true, CompLevel = round , IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("Infected Reactor"), ReplaceExistingComp = true, CompLevel = round+9, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("InfectedBeamMainTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Infected Turret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Infected Turret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "Uninfected Carrier";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 7: // The Beam Caster - Heavy attack
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_WDCRUISER, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                                new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("The Absortion Field"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Nano Active MK2"), ReplaceExistingComp = true, CompLevel = round + 3, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 16, CompSubType = PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName("MegaHullP"), ReplaceExistingComp = true, CompLevel = 0 , IsCargo = false, CompTypeToReplace = 16, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("Overclocked ZeroPoint Reactor"), ReplaceExistingComp = true, CompLevel = round+2, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("FakeKeeperBeamTurret"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 4, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 7, CompSubType = PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName("Turret Thermo Boost"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 7, SlotNumberToReplace = 3},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };

                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "The Beam Caster";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            break;
                        case 8: //The Executioner - Final Showdown
                            plpersistantShipInfo = new PLPersistantShipInfo(EShipType.E_ROLAND, 1, PLServer.GetCurrentSector(), 0, false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                            overrides = new List<ComponentOverrideData>
                            {
                               new ComponentOverrideData() { CompType = 3, CompSubType = PulsarModLoader.Content.Components.Reactor.ReactorModManager.Instance.GetReactorIDFromName("Flagship Reactor"), ReplaceExistingComp = true, CompLevel = round/10, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 11, CompSubType = PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("FlagShipMainTurret"), ReplaceExistingComp = true, CompLevel = round/10, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 4, ReplaceExistingComp = true, CompLevel = round/10 + 4, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = round/10 + 4, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 24, CompSubType = PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName("AutoLightningTurret"), ReplaceExistingComp = true, CompLevel = round/10 + 4, IsCargo = false, CompTypeToReplace = 24, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 24, CompSubType = PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName("AutoBurstTurret"), ReplaceExistingComp = true, CompLevel = round/10 + 4, IsCargo = false, CompTypeToReplace = 24, SlotNumberToReplace = 1},
                               new ComponentOverrideData() { CompType = 24, CompSubType = PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName("AutoDefenderTurret"), ReplaceExistingComp = true, CompLevel = round/10 + 4, IsCargo = false, CompTypeToReplace = 24, SlotNumberToReplace = 2},
                               new ComponentOverrideData() { CompType = 24, CompSubType = PulsarModLoader.Content.Components.AutoTurret.AutoTurretModManager.Instance.GetAutoTurretIDFromName("AutoSentryTurret"), ReplaceExistingComp = true, CompLevel = round/10, IsCargo = false, CompTypeToReplace = 24, SlotNumberToReplace = 3},
                               new ComponentOverrideData() { CompType = 25, CompSubType = 3, ReplaceExistingComp = true, CompLevel = round/10 + 1, IsCargo = false, CompTypeToReplace = 25, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 9, CompSubType = PulsarModLoader.Content.Components.Thruster.ThrusterModManager.Instance.GetThrusterIDFromName("Flagship Thruster"), ReplaceExistingComp = true, CompLevel = 0, IsCargo = false, CompTypeToReplace = 9, SlotNumberToReplace = 0},
                               new ComponentOverrideData() { CompType = 7, CompSubType = PulsarModLoader.Content.Components.CPU.CPUModManager.Instance.GetCPUIDFromName("Active Anti-Virus"), ReplaceExistingComp = true, CompLevel = 10, IsCargo = false, CompTypeToReplace = 7, SlotNumberToReplace = 4},
                               new ComponentOverrideData() { CompType = 2, CompSubType = PulsarModLoader.Content.Components.WarpDrive.WarpDriveModManager.Instance.GetWarpDriveIDFromName("The Recharger"), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 2, SlotNumberToReplace = 0},
                            };
                            if (round < 20)
                            {
                                overrides.Add(new ComponentOverrideData() { CompType = 6, CompSubType = 2, ReplaceExistingComp = true, CompLevel = round / 10 + 7, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0 });
                            }
                            else
                            {
                                overrides.Add(new ComponentOverrideData() { CompType = 6, CompSubType = PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Flagship Hull"), ReplaceExistingComp = true, CompLevel = round/10, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0 });
                            }
                            if (round < 30)
                            {
                                overrides.Add(new ComponentOverrideData() { CompType = 1, CompSubType = 12, ReplaceExistingComp = true, CompLevel = round / 10 + 4, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0 });
                            }
                            else
                            {
                                overrides.Add(new ComponentOverrideData() { CompType = 1, CompSubType = PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Flagship Shield"), ReplaceExistingComp = true, CompLevel = round / 10, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0 });
                            }
                            plpersistantShipInfo.CompOverrides.AddRange(overrides);
                            plpersistantShipInfo.ShipName = "The Executioner";
                            pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                            pLShipInfoBase.DropScrap = false;
                            pLShipInfoBase.CreditsLeftBehind = 0;
                            pLShipInfoBase.IsRelicHunter = true;
                            foreach(PLShipComponent component in pLShipInfoBase.MyStats.AllComponents) 
                            {
                                if(component.Name.Contains("Flagship") && component.Level > round / 10) 
                                {
                                    component.Level = round / 10;
                                }
                            }
                            break;
                    }
                }

                static void GenerateRandomDrone(int round) 
                {
                    PLPersistantShipInfo plpersistantShipInfo;
                    PLShipInfoBase pLShipInfoBase;
                    List<ComponentOverrideData> overrides = new List<ComponentOverrideData>();
                    List<EShipType> droneTypes = new List<EShipType> //Drone type
                    {
                        EShipType.E_WDDRONE1,
                        EShipType.E_WDDRONE2,
                        EShipType.E_WDDRONE3,
                        EShipType.E_PHASE_DRONE,
                        EShipType.E_SHOCK_DRONE,
                        EShipType.E_DEATHSEEKER_DRONE,
                        EShipType.E_ABYSS_FIGHTER,
                        EShipType.E_ABYSS_HEAVY_FIGHTER,
                        EShipType.E_UNSEEN_FIGHTER,
                        EShipType.E_INFECTED_FIGHTER,
                        EShipType.E_INFECTED_FIGHTER_HEAVY
                    };
                    EShipType drone = droneTypes[RNG.Next(0, droneTypes.Count)];
                    //Shields
                    List<int> compTypes = new List<int>
                    {
                        5,
                        4,
                        8,
                        9
                    };
                    List<int> exoticTypes = new List<int>
                    {
                        13,
                        18,
                        16,
                        PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Layered Shield"),
                        PulsarModLoader.Content.Components.Shield.ShieldModManager.Instance.GetShieldIDFromName("Electric Wall"),
                    };
                    overrides.Add(new ComponentOverrideData() { CompType = 1, CompSubType = CompFromLists(compTypes,exoticTypes,round), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 1, SlotNumberToReplace = 0 });

                    //Hulls
                    compTypes = new List<int>
                    {
                        2
                    };
                    exoticTypes = new List<int>
                    {
                        PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Nano Active MK2"),
                        PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("Toxic Wall"),
                        PulsarModLoader.Content.Components.Hull.HullModManager.Instance.GetHullIDFromName("\"The Wall\""),
                    };
                    overrides.Add(new ComponentOverrideData() { CompType = 6, CompSubType = CompFromLists(compTypes, exoticTypes, round), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 6, SlotNumberToReplace = 0 });

                    //HullPlating
                    compTypes = new List<int>
                    {
                        0
                    };
                    exoticTypes = new List<int>
                    {
                        0,
                        0,
                        0,
                        0,
                        0,
                        PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName("MegaHullP"),
                        PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName("TurtleP"),
                        PulsarModLoader.Content.Components.HullPlating.HullPlatingModManager.Instance.GetHullPlatingIDFromName("GlassP"),
                    };
                    overrides.Add(new ComponentOverrideData() { CompType = 16, CompSubType = CompFromLists(compTypes, exoticTypes, round), ReplaceExistingComp = true, CompLevel = 0, IsCargo = false, CompTypeToReplace = 16, SlotNumberToReplace = 0 });

                    //Turrets
                    if (drone != EShipType.E_DEATHSEEKER_DRONE && drone != EShipType.E_ABYSS_FIGHTER && drone != EShipType.E_ABYSS_HEAVY_FIGHTER && drone != EShipType.E_MATRIX_DRONE && drone != EShipType.E_SHOCK_DRONE)
                    {
                        compTypes = new List<int>
                    {
                        1,
                        2,
                        4,
                        0,
                        6,
                        3,
                    };
                        exoticTypes = new List<int>
                    {
                        4,
                        9,
                        13,
                        6,
                        3,
                        14,
                        15,
                        PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Supreme RailGun"),
                        PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Anti-Shield"),
                        PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("HullSmasher"),
                        PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Infected Turret"),
                        PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Respected Nullifier Gun"),
                        PulsarModLoader.Content.Components.Turret.TurretModManager.Instance.GetTurretIDFromName("Defender Turret mk2"),
                    };
                        overrides.Add(new ComponentOverrideData() { CompType = 10, CompSubType = CompFromLists(compTypes, exoticTypes, round), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0 });
                        overrides.Add(new ComponentOverrideData() { CompType = 10, CompSubType = CompFromLists(compTypes, exoticTypes, round), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1 });
                    }
                    else if(drone == EShipType.E_SHOCK_DRONE) 
                    {
                        overrides.Add(new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 0 });
                        overrides.Add(new ComponentOverrideData() { CompType = 10, CompSubType = 9, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 10, SlotNumberToReplace = 1 });
                    }

                    //MainTurret
                    compTypes = new List<int>
                    {
                        0,
                        1,
                        3
                    };
                    exoticTypes = new List<int>
                    {
                        2,
                        4,
                        5,
                        6,
                        PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("SuperchargeMainTurret"),
                        PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("MachineGunMainTurret"),
                        PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("InfectedBeamMainTurret"),
                        PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("FakeKeeperBeamTurret"),
                        PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("PhaseShieldTurret"),
                        PulsarModLoader.Content.Components.MegaTurret.MegaTurretModManager.Instance.GetMegaTurretIDFromName("PhaserTurret"),
                    };
                    overrides.Add(new ComponentOverrideData() { CompType = 11, CompSubType = CompFromLists(compTypes, exoticTypes, round), ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 11, SlotNumberToReplace = 0 });

                    plpersistantShipInfo = new PLPersistantShipInfo(drone, 1, PLServer.GetCurrentSector(), RNG.Next(0,(int)EShipModifierType.MAX), false, true, false, -1, PLEncounterManager.Instance.PlayerShip.ShipID);
                    overrides.Add(new ComponentOverrideData() { CompType = 3, CompSubType = 10, ReplaceExistingComp = true, CompLevel = round, IsCargo = false, CompTypeToReplace = 3, SlotNumberToReplace = 0 });
                    plpersistantShipInfo.CompOverrides.AddRange(overrides);
                    pLShipInfoBase = PLEncounterManager.Instance.GetCPEI().SpawnEnemyShip(plpersistantShipInfo.Type, plpersistantShipInfo);
                    pLShipInfoBase.DropScrap = false;
                    pLShipInfoBase.CreditsLeftBehind = 0;
                }

                static string SubTitleForWave(bool isWild, int round, int wave) 
                {
                    if (!isWild) 
                    {
                        switch (round) 
                        {
                            case 0:
                                switch (wave) 
                                {
                                    case 1:
                                        return "Where it all starts.";
                                    case 2:
                                        return "Now they got bigger guns!";
                                    case 3:
                                        return "Time to face your teacher";
                                }
                                break;

                            case 1:
                                switch (wave)
                                {
                                    case 1:
                                        return "Look at their fancy tech.";
                                    case 2:
                                        return "The return of the forgotten.";
                                    case 3:
                                        return "As long as we stay cold we gold.";
                                }
                                break;

                            case 2:
                                switch (wave)
                                {
                                    case 1:
                                        return "They jump now, so fun.";
                                    case 2:
                                        return "Those one look odd.";
                                    case 3:
                                        return "Phasing through your soul.";
                                }
                                break;

                            case 3:
                                switch (wave)
                                {
                                    case 1:
                                        return "Just a little bit thougher.";
                                    case 2:
                                        return "They don't look happy.";
                                    case 3:
                                        return "Time to meet a true imovable object.";
                                }
                                break;

                            case 4:
                                switch (wave)
                                {
                                    case 1:
                                        return "Are those footballs?.";
                                    case 2:
                                        return "RUN, and keep an eye on the little one.";
                                    case 3:
                                        return "Shields and armor? How cute.";
                                }
                                break;

                            case 5:
                                switch (wave)
                                {
                                    case 1:
                                        return "These little balls are quite shocking.";
                                    case 2:
                                        return "What is that thing in the distance?";
                                    case 3:
                                        return "Do you need some extra power?";
                                }
                                break;

                            case 6:
                                switch (wave)
                                {
                                    case 1:
                                        return "Seems awfully quiet here.";
                                    case 2:
                                        return "What even are those things?";
                                    case 3:
                                        return "Right behind you.";
                                }
                                break;

                            case 7:
                                switch (wave)
                                {
                                    case 1:
                                        return "Those drones seem to be alive?";
                                    case 2:
                                        return "Cool, they got bigger now.";
                                    case 3:
                                        return "Time to perish like all the infected in my way.";
                                }
                                break;

                            case 8:
                                switch (wave)
                                {
                                    case 1:
                                        return "Watch out for those main turrets.";
                                    case 2:
                                        return "Why are this things so strong?";
                                    case 3:
                                        return "Imma firin' mah lazer.";
                                }
                                break;

                            case 9:
                                switch (wave)
                                {
                                    case 1:
                                        return "Are you sure you want to be here?";
                                    case 2:
                                        return "This is your last chance";
                                    case 3:
                                        return "The end is nigh";
                                }
                                break;
                        }
                    }
                    else 
                    {
                        List<string> list = new List<string> 
                        {
                            "Everything will be ok",
                            "Your fear will consume you"
                        };
                        return list[RNG.Next(0, list.Count)];
                    }
                    return string.Empty;
                }

                static int CompFromLists(List<int> basicList, List<int> exoticList, int round) 
                {
                    if (round % 10 == 0)
                    {
                        if (RNG.Next(0, 100) < 25)
                        {
                            return exoticList[RNG.Next(0, exoticList.Count)];
                        }
                        return basicList[RNG.Next(0, basicList.Count)];
                    }
                    else if (round % 10 == 1)
                    {
                        if (RNG.Next(0, 100) < 50)
                        {
                            return exoticList[RNG.Next(0, exoticList.Count)];
                        }
                        return basicList[RNG.Next(0, basicList.Count)];
                    }
                    else if (round % 10 == 2)
                    {
                        if (RNG.Next(0, 100) < 75)
                        {
                            return exoticList[RNG.Next(0, exoticList.Count)];
                        }
                        return basicList[RNG.Next(0, basicList.Count)];
                    }
                    else
                    {
                        return exoticList[RNG.Next(0, exoticList.Count)];
                    }
                }
            }
            [HarmonyPatch(typeof(PLInGameUI), "Update")]
            internal class FixBossBar
            {
                internal static float LastTitle = Time.time;
                internal static string Title = string.Empty;
                internal static string SubTitle = string.Empty;
                static bool shouldSound = true;
                static void Postfix(PLInGameUI __instance)
                {
                    if (Time.time - LastTitle <= 7 && Title != string.Empty)
                    {
                        PLGlobal.SafeGameObjectSetActive(__instance.Boss_TitleRoot, true);
                        PLGlobal.SafeGameObjectSetActive(__instance.BossUI_TitleBottom.gameObject, true);
                        __instance.BossUI_TitleTop.text = Title;
                        __instance.BossUI_TitleBottom.text = SubTitle;
                        if (shouldSound)
                        {
                            PLMusic.PostEvent("play_sx_playermenu_creworder", __instance.gameObject);
                            shouldSound = false;
                        }
                    }
                    else if (!shouldSound)
                    {
                        shouldSound = true;
                    }
                }
            }
        }
    }
}


