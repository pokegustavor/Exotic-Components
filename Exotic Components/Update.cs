using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exotic_Components
{
    [HarmonyLib.HarmonyPatch(typeof(PLShipInfo),"Update")]
    internal class Update
    {
        static void Postfix()
        {
            if (PLServer.GetCurrentSector().Name == "The Core(MOD)")
            {
                InitialStore.UpdateCore();
            }
        }
}
}
