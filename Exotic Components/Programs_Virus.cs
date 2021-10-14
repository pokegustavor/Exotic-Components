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
                if (InWarpDriveProgram.MaxLevelCharges < -3) return;
                foreach (PLPlayer player in PLServer.Instance.AllPlayers)
                {
                    if (player.GetPawn() != null && !player.GetPawn().IsDead && player.RaceID != 2)
                    {
                        player.GetPawn().TakeDamage(500000, false, -1);
                    }
                }
                foreach (PLShipInfoBase ship in PLEncounterManager.Instance.AllShips.Values) 
                {
                    if(ship is PLAlienTentacleCreatureInfo || ship.IsInfected) 
                    {
                        ship.DestroySelf(InWarpDriveProgram.ShipStats.Ship);
                    }
                }
                foreach(PLCreature creatur in PLGameStatic.Instance.AllCreatures) 
                {
                    if(!(creatur is PLRobotWalker) && !(creatur is PLRobotWalkerLarge)) 
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

            public override void Execute(PLWarpDriveProgram InWarpDriveProgram)
            {
                base.Execute(InWarpDriveProgram);
                /*
                PLServer.Instance.photonView.RPC("AddToSendQueue", PhotonTargets.All, new object[]
                {

                });
                */
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
}
