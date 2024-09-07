using PulsarModLoader;
using PulsarModLoader.MPModChecks;

[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo("Assembly-CSharp")]
namespace Exotic_Components
{
    public class Mod : PulsarMod
    {
        public override string Version => "4.3";

        public override string Author => "pokegustavo (with suggestions from EngBot, TheShribe and Kani)";

        public override string ShortDescription => "Adds new components to the game, new shop and some missions";

        public override string Name => "Exotic Components";

        public override int MPRequirements => 3;

        public override string HarmonyIdentifier()
        {
            return "Pokegustavo.ExoticComponents";
        }
    }
}
