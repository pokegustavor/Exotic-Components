using PulsarModLoader.Content.Components.Virus;
using PulsarModLoader.Content.Components.WarpDriveProgram;
using UnityEngine;
using HarmonyLib;
using System.Linq;
using PulsarModLoader;

namespace Exotic_Components
{
    internal class Programs_Virus
    {
        class AntiLiveProgram : WarpDriveProgramMod
        {
            public override string Name => "Anti-Life Pulse";

            public override string Description => "This pulse system recovered from some aliens by a polytech crew will kill EVERY organic in the sector. It can only be used 3 times.";

            public override int MarketPrice => 70000;

            public override bool Contraband => true;

            public override string ShortName => "ALP";

            public override float ActiveTime => 0.1f;

            public override int MaxLevelCharges => -1;

            public override void Execute(PLWarpDriveProgram InWarpDriveProgram)
            {
                if (InWarpDriveProgram.MaxLevelCharges < -3) return;
                foreach (PLPlayer player in PLServer.Instance.AllPlayers)
                {
                    if (!PhotonNetwork.isMasterClient) break;
                    if (player != null && player.GetPawn() != null && !player.GetPawn().IsDead && player.RaceID != 2)
                    {
                        player.GetPawn().photonView.RPC("TakeDamage", PhotonTargets.All, new object[]
                        {
                            5000000f,
                            false,
                            -1
                        });
                    }
                }
                foreach (PLShipInfoBase ship in PLEncounterManager.Instance.AllShips.Values) 
                {
                    if(ship != null && (ship is PLAlienTentacleCreatureInfo || ship.IsInfected)) 
                    {
                        ship.DestroySelf(InWarpDriveProgram.ShipStats.Ship);
                    }
                }
                foreach(PLCreature creatur in PLGameStatic.Instance.AllCreatures) 
                {
                    if(creatur != null && (!(creatur is PLRobotWalker) && !(creatur is PLRobotWalkerLarge))) 
                    {
                        creatur.IsDead = true;
                        creatur.TakeDamage(500000, false, -1);
                    }
                }
                InWarpDriveProgram.MaxLevelCharges--;
                if (InWarpDriveProgram.MaxLevelCharges < -3) InWarpDriveProgram.FlagForSelfDestruction();
            }
        }
        class BlindFoldProgram : WarpDriveProgramMod
        {
            public override string Name => "BlindFold [VIRUS]";

            public override string Description => "Makes the target blind to any EMP signal for 60 seconds";

            public override int MaxLevelCharges => 4;

            public override bool IsVirus => true;

            public override int MarketPrice => 14000;

            public override int VirusSubtype => VirusModManager.Instance.GetVirusIDFromName("Blind Fold");

            public override string ShortName => "BF";

            public override float ActiveTime => 60f;

            public override Texture2D IconTexture => PLGlobal.Instance.VirusBGTexture;

        }
        class SelfDestructProgram : WarpDriveProgramMod
        {
            public override string Name => "Self Destruction [VIRUS]";

            public override string Description => "If this virus is not removed in 30 seconds, the ship will self-destruct";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override int MaxLevelCharges => 5;

            public override bool IsVirus => true;

            public override int VirusSubtype => VirusModManager.Instance.GetVirusIDFromName("Self Destruction");

            public override string ShortName => "SFD";

            public override float ActiveTime => 45f;

            public override Texture2D IconTexture => PLGlobal.Instance.VirusBGTexture;
        }
        class FriendlyDroneProgram : WarpDriveProgramMod
        {
            public override string Name => "Friendly Charm [VIRUS]";

            public override string Description => "This virus makes it so infected drones see you as friendly for 30 seconds.";

            public override int MarketPrice => 35000;

            public override int MaxLevelCharges => 5;

            public override bool IsVirus => true;

            public override int VirusSubtype => VirusModManager.Instance.GetVirusIDFromName("Friendly Charm");

            public override string ShortName => "FC";

            public override float ActiveTime => 50f;

            public override Texture2D IconTexture => PLGlobal.Instance.VirusBGTexture;
        }
        class DoorStuckProgram : WarpDriveProgramMod
        {
            public override string Name => "DoorStuck [VIRUS]";

            public override string Description => "Locks all doors for 15 seconds";

            public override int MaxLevelCharges => 1;

            public override bool IsVirus => true;

            public override int MarketPrice => 7000;

            public override int VirusSubtype => VirusModManager.Instance.GetVirusIDFromName("Door Stuck");

            public override string ShortName => "DS";

            public override float ActiveTime => 30f;

            public override Texture2D IconTexture => PLGlobal.Instance.VirusBGTexture;

        }
        class WarpNetwork : WarpDriveProgramMod 
        {
            public override string Name => "Warp Gate Jump";

            public override string Description => "This program uses the remnants of the Warp Network Warp Fields to engage an emergency connection between you and the nearest warp gate, imagine as a portable warp gate, and nothing will trap you in!";

            public override int MarketPrice => 25000;

            public override string ShortName => "WGJ";

            public override float ActiveTime => 0.1f;

            public override int MaxLevelCharges => 5;

            public override void Execute(PLWarpDriveProgram InWarpDriveProgram)
            {
                if(PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.UNSEEN_MS && PhotonNetwork.isMasterClient && PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.LCWBATTLE && PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.TOPSEC && !PLServer.GetCurrentSector().IsPartOfLongRangeWarpNetwork && !PLServer.GetCurrentSector().Name.Contains("Gauntlet_")) 
                {
                    PLSectorInfo target = null;
                    foreach(PLSectorInfo sector in PLGlobal.Instance.Galaxy.AllSectorInfos.Values) 
                    {
                        if (sector.IsPartOfLongRangeWarpNetwork && sector.VisualIndication != ESectorVisualIndication.GWG && sector.VisualIndication != ESectorVisualIndication.PT_WARP_GATE)
                        {
                            if (target == null) 
                            {
                                target = sector;
                            }
                            else if((sector.Position - PLServer.GetCurrentSector().Position).magnitude < (target.Position - PLServer.GetCurrentSector().Position).magnitude)
                            {
                                target = sector;
                            }
                        }
                    }
                    if(target != null) 
                    {
                        PLServer.Instance.photonView.RPC("NetworkBeginWarp", PhotonTargets.All, new object[]
                        {
                        PLEncounterManager.Instance.PlayerShip.ShipID,
                        target.ID,
                        PLServer.Instance.GetEstimatedServerMs(),
                        -1
                        });
                    }
                }
            }
        }
        class SuperShield : WarpDriveProgramMod 
        {
            public override string Name => "Super Shield";

            public override string Description => "This program will activate a shield overcharge after 1.5 seconds that will keep your ship invunerable for 10 seconds, but after that the shields will be depleated, to ensure your shields won't fry it can only be used once per sector.";

            public override int MarketPrice => 80000;

            public override string ShortName => "SS";

            public override float ActiveTime => 11.5f;

            public override int MaxLevelCharges => 6;

            public bool Used = false;

            public override void Execute(PLWarpDriveProgram InWarpDriveProgram)
            {
                if (InWarpDriveProgram.ShipStats != null && InWarpDriveProgram.ShipStats.Ship != null && !Used && InWarpDriveProgram.ShipStats.Ship.MyShieldGenerator != null)
                {
                    InWarpDriveProgram.ShipStats.Ship.SuperShieldActivateStartTime = PLServer.Instance.GetEstimatedServerMs();
                    Used = true;
                }
                else 
                {
                    InWarpDriveProgram.Level = 6;
                    InWarpDriveProgram.SubTypeData = 6;
                    InWarpDriveProgram.Last_Active_Time = 0;
                }
            }
            public override void OnWarp(PLShipComponent InComp)
            {
                Used = false;
            }
        }
        class Rebound : WarpDriveProgramMod
        {
            public override string Name => "The Rebound";

            public override string Description => "This program will create a copy of any virus infecting you and will send it back to their sender.";

            public override int MarketPrice => 25000;

            public override bool Experimental => true;

            public override int MaxLevelCharges => 5;

            public override string ShortName => "RB";

            public override float ActiveTime => 0f;

            public override void Execute(PLWarpDriveProgram InWarpDriveProgram)
            {
                if (!PhotonNetwork.isMasterClient) return;
                foreach(PLVirus virus in InWarpDriveProgram.ShipStats.GetComponentsOfType(ESlotType.E_COMP_VIRUS).Cast<PLVirus>()) 
                {
                    PLVirus plvirus = new PLVirus((EVirusType)virus.SubType, 0, 0)
                    {
                        Sender = InWarpDriveProgram.ShipStats.Ship,
                        InitialTime = PLServer.Instance.GetEstimatedServerMs()
                    };
                    plvirus.InfectionCompletedOnShips.Add(virus.Sender.ShipID);
                    PLShipComponent plshipComponent = virus.Sender.MyStats.AddShipComponent(plvirus, -1, ESlotType.E_COMP_NONE);
                    PLServer.Instance.photonView.RPC("SetupSelfInfectedVirus", PhotonTargets.All, new object[]
                    {
                            virus.Sender.ShipID,
                            plshipComponent.NetID,
                            PLServer.Instance.GetEstimatedServerMs()
                    });
                }
            }
        }



        class DoorStuckVirus : VirusMod
        {
            public override string Name => "Door Stuck";

            public override string Description => "Locks all doors for 15 seconds";

            public override int InfectionTimeLimitMs => 15000;
        }
        class FriendlyDroneVirus : VirusMod
        {
            public override string Name => "Friendly Charm";

            public override string Description => "This virus makes it so infected drones see you as friendly for 30 seconds.";

            public override int InfectionTimeLimitMs => 30000;
        }
        class SelfDestructVirus : VirusMod 
        {
            public override string Name => "Self Destruction";

            public override string Description => "Destroys the ship if not removed in 30 seconds";

            public override int InfectionTimeLimitMs => 45500;

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                PLVirus me = InComp as PLVirus;
                if (me.ShipStats.Ship.ShipTypeID == EShipType.E_GUARDIAN || me.ShipStats.Ship.ShipTypeID == EShipType.E_UNSEEN_EYE) return;
                if(PLServer.Instance.GetEstimatedServerMs() - me.InitialInfectionTime >= 30000f) 
                {
                    me.ShipStats.Ship.TakeDamage(500000, false, EDamageType.E_INFECTED, 0, 0, me.Sender, -1);
                }
            }
        }
        class BlindFoldVirus : VirusMod
        {
            public override string Name => "Blind Fold";

            public override string Description => "Disables EMP detection for 60 seconds";

            public override int InfectionTimeLimitMs => 60000;

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                PLShipStats mystats = InComp.ShipStats;
                mystats.EMDetection *= 0f;
            }
        }

    }

    [HarmonyPatch(typeof(PLShipInfoBase), "ShouldBeHostileToShip")]
    class FriendlyDrones 
    {
        static void Postfix(PLShipInfoBase __instance, PLShipInfoBase inShip, ref bool __result) 
        {
            if (__instance.IsDrone && !(__instance is PLWarpGuardian) && !(__instance is PLUnseenEye))
            {
                bool Friendly = false;
                foreach (PLShipComponent component in __instance.MyStats.GetSlot(ESlotType.E_COMP_VIRUS))
                {
                    if (component.SubType == VirusModManager.Instance.GetVirusIDFromName("Friendly Charm") && (component as PLVirus).Sender == inShip)
                    {
                        Friendly = true;
                        break;
                    }
                }
                if (Friendly)
                {
                    __result = false;
                    if (__instance.HostileShips.Contains(inShip.ShipID))
                    {
                        __instance.HostileShips.Remove(inShip.ShipID);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLShipInfoBase), "IsSupershieldActive")]
    class SuperShield 
    {
        static void Postfix(PLShipInfoBase __instance, ref bool __result) 
        {
            if (PLServer.Instance != null && __instance.MyShieldGenerator != null)
            {
                int num = PLServer.Instance.GetEstimatedServerMs() - __instance.SuperShieldActivateStartTime;
                if (num > 11500 && num < 12000) __instance.MyShieldGenerator.Current = 0;
                __result = num > 1500 && num < 11500;
                return;
            }
            __result = false;
        }
    }
    [HarmonyPatch(typeof(PLShipComponent), "OnWarp")]
    class OnWarpFix 
    {
        static bool Prefix(PLShipComponent __instance)
        {
            PLWarpDriveProgram pLReactor = __instance as PLWarpDriveProgram;
            if (pLReactor == null) return true;
            int subtypeformodded = pLReactor.SubType - WarpDriveProgramModManager.Instance.VanillaWarpDriveProgramMaxType;
            if (subtypeformodded > -1 && subtypeformodded < WarpDriveProgramModManager.Instance.WarpDriveProgramTypes.Count && pLReactor.ShipStats != null)
            {
                WarpDriveProgramModManager.Instance.WarpDriveProgramTypes[subtypeformodded].OnWarp(pLReactor);
                return false;
            }
            return true;
        }
    }
}
