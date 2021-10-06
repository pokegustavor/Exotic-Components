using PulsarModLoader.Content.Components.Virus;
using PulsarModLoader.Content.Components.WarpDriveProgram;
using UnityEngine;

namespace Exotic_Components
{
    internal class Programs_Virus
    {
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
