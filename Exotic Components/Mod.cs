using PulsarModLoader;
[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo("Assembly-CSharp")]
namespace Exotic_Components
{
    public class Mod : PulsarMod
    {
        public override string Version => "2.10";

        public override string Author => "pokegustavo";

        public override string ShortDescription => "Adds new components to the game, new shop and some missions";

        public override string Name => "Exotic Components";

        public override int MPFunctionality => (int)MPFunction.All;

        public override string HarmonyIdentifier()
        {
            return "Pokegustavo.ExoticComponents";
        }
    }
}
