using PulsarModLoader.Content.Components.Missile;
using HarmonyLib;

namespace Exotic_Components
{
    class Missiles
    {
        class AntiArmorMissile : MissileMod
        {
            public override string Name => "Armor Denial";

            public override string Description => "This special missiles are designed to completely ignore the target hull armor while doing significant damage, but it is extremely slow";

            public override int MarketPrice => 7800;

            public override bool Experimental => true;

            public override float Damage => 450f;

            public override float Speed => 1.3f;

            public override EDamageType DamageType => EDamageType.E_PHYSICAL;

            public override int MissileRefillPrice => 600;

            public override int AmmoCapacity => 21;

            public override int PrefabID => 1;
        }
        public static bool WasMissileDamage(float damage, PLTurret turret, EDamageType damageType)
        {
            if (turret != null && turret.ShipStats != null && turret.ShipStats.Ship != null && turret.GetHasTrackingMissileCapability()) 
            {
                if(damage != turret.ShipStats.Ship.SelectedMissileLauncher.Damage) 
                {
                    return false;
                }
                if(turret is PLMegaTurret) 
                {
                    if ((turret as PLMegaTurret).DamageType != damageType) return true;
                }
                if(turret is PLLaserTurret) 
                {
                    if ((turret as PLLaserTurret).LaserDamageType!= damageType) return true;
                }
                if(damage != turret.m_Damage * turret.LevelMultiplier(0.15f, 1f) * turret.ShipStats.TurretDamageFactor) 
                {
                    return true;
                }
            }
            return false;
        }
        [HarmonyPatch(typeof(PLShipInfoBase),"TakeDamage")]
        class TakeDamage
        {
            static void Prefix(float dmg, bool bottomHit, EDamageType dmgType, float randomNum, int SystemTargetID, PLShipInfoBase attackingShip, int turretID, PLShipInfoBase __instance) 
            {
                PLTurret turret = null;
                if(attackingShip != null) { turret = attackingShip.GetTurretAtID(turretID); }
                if (WasMissileDamage(dmg, turret, dmgType) && attackingShip.SelectedMissileLauncher.SubType == MissileModManager.Instance.GetMissileIDFromName("Armor Denial"))
                {
                    __instance.MyStats.HullArmor = 0;
                }
            }
    }
    }
    
}
