using PulsarModLoader;
[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo("Assembly-CSharp")]
namespace Exotic_Components
{
    public class Mod : PulsarMod
    {
        public override string Version => "1.0";

        public override string Author => "pokegustavo";

        public override string ShortDescription => "Adds new components to the game";

        public override string Name => "Exotic Components";

        public override string HarmonyIdentifier()
        {
            return "Pokegustavo.ExoticComponents";
        }
    }
}
