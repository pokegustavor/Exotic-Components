using PulsarModLoader.Content.Components.Virus;
using PulsarModLoader.Content.Components.WarpDriveProgram;
using UnityEngine;

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
                if (!PhotonNetwork.isMasterClient) return;
                if (InWarpDriveProgram.MaxLevelCharges < -3) return;
                foreach (PLPlayer player in PLServer.Instance.AllPlayers)
                {
                    if (player != null && player.GetPawn() != null && !player.GetPawn().IsDead && player.RaceID != 2)
                    {
                        player.GetPawn().photonView.RPC("TakeDamage", PhotonTargets.All, new object[]
                        {
                            500000,
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

            public override string Description => "If this virus is not removed in 30 seconds, the ship will selfdestruct";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override int MaxLevelCharges => 6;

            public override bool IsVirus => true;

            public override int VirusSubtype => VirusModManager.Instance.GetVirusIDFromName("Self Destruction");

            public override string ShortName => "SFD";

            public override float ActiveTime => 45f;

            public override Texture2D IconTexture => PLGlobal.Instance.VirusBGTexture;
        }
        class FriendlyDroneProgram : WarpDriveProgramMod
        {
            public override string Name => "Friendly Charm [VIRUS]";

            public override string Description => "This virus makes so infected drones see you as friendly for 30 seconds.";

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
        class DoorStuckVirus : VirusMod
        {
            public override string Name => "Door Stuck";

            public override string Description => "Locks all doors for 15 seconds";

            public override int InfectionTimeLimitMs => 15000;
        }
        class FriendlyDroneVirus : VirusMod
        {
            public override string Name => "Friendly Charm";

            public override string Description => "This virus makes so infected drones see you as friendly for 30 seconds.";

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
    [HarmonyLib.HarmonyPatch(typeof(VirusModManager), "CreateVirus")]
    class ManualFixVirus
    {
        static void Postfix(int Subtype, ref PLVirus __result)
        {
            if (Subtype >= VirusModManager.Instance.VanillaVirusMaxType)
            {
                int subtypeformodded = Subtype - VirusModManager.Instance.VanillaVirusMaxType;
                if (subtypeformodded <= VirusModManager.Instance.VirusTypes.Count && subtypeformodded > -1)
                {
                    VirusMod VirusType = VirusModManager.Instance.VirusTypes[Subtype - VirusModManager.Instance.VanillaVirusMaxType];
                    __result.InfectionTimeLimitMs = VirusType.InfectionTimeLimitMs;
                }
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PLShipInfoBase), "ShouldBeHostileToShip")]
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
}
