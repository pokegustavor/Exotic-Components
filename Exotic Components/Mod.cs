using PulsarModLoader;
using PulsarModLoader.MPModChecks;
using PulsarModLoader.SaveData;
using System.IO;

[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo("Assembly-CSharp")]
namespace Exotic_Components
{
    public class Mod : PulsarMod
    {
        public override string Version => "5.1";

        public override string Author => "pokegustavo (with suggestions from EngBot, TheShribe and Kani)";

        public override string ShortDescription => "Adds new components to the game, new shop and some missions";

        public override string Name => "Exotic Components";

        public override int MPRequirements => 3;

        public override string HarmonyIdentifier()
        {
            return "Pokegustavo.ExoticComponents";
        }
    }
    internal class DataSaver : PMLSaveData
    {
        public override uint VersionID => 5;

        public override string Identifier()
        {
            return "Pokegustavo.ExoticComponents";
        }

        public override void LoadData(byte[] Data, uint VersionID)
        {
            using (MemoryStream dataStream = new MemoryStream(Data))
            {
                using (BinaryReader binaryReader = new BinaryReader(dataStream))
                {
                    TheCoreComms.soldIntergalatic = binaryReader.ReadBoolean();
                    Gauntlet.CurrentTrial = binaryReader.ReadInt32();
                    Gauntlet.WildTrial = binaryReader.ReadBoolean();
                    Gauntlet.CurrentWildTrial = binaryReader.ReadInt32();
                    Gauntlet.WildSeed = binaryReader.ReadInt32();
                }
            }
        }

        public override byte[] SaveData()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(stream))
                {
                    binaryWriter.Write(TheCoreComms.soldIntergalatic);
                    binaryWriter.Write(Gauntlet.CurrentTrial);
                    binaryWriter.Write(Gauntlet.WildTrial);
                    binaryWriter.Write(Gauntlet.CurrentWildTrial);
                    binaryWriter.Write(Gauntlet.WildSeed);
                }
                return stream.ToArray();
            }
        }
    }
}
