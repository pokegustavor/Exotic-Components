﻿using PulsarModLoader.Content.Components.Turret;
using UnityEngine;
using Pathfinding;
namespace Exotic_Components
{
    internal class Turrets
    {
        class SupremeRailGunMod : TurretMod
        {
            public override string Name => "Supreme RailGun";

            public override PLShipComponent PLTurret => new SupremeRailGun();
        }
        class AntiShieldMod : TurretMod
        {
            public override string Name => "Anti-Shield";

            public override PLShipComponent PLTurret => new AntiShield();
        }
        class HullSmasherMod : TurretMod
        {
            public override string Name => "HullSmasher";

            public override PLShipComponent PLTurret => new HullSmasher();
        }
        class InfectedTurretMod : TurretMod
        {
            public override string Name => "Infected Turret";

            public override PLShipComponent PLTurret => new InfectedTurret();
        }
    }

    class SupremeRailGun : PLBasicTurret
    {
        public SupremeRailGun(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Supreme RailGun";
            this.Desc = "Recovered Acient Human technology, this Railgun is the strongest of its kind, even if not as powerfull as the original";
            this.m_Damage = 450f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Supreme RailGun");
            this.m_MarketPrice = 12000;
            this.m_MaxPowerUsage_Watts = 7000f;
            this.m_ProjSpeed = 230f;
            this.FireDelay = 4.5f;
            this.Experimental = true;
            this.TurretRange = 5400f;
            base.CargoVisualPrefabID = 3;
            base.Level = inLevel;
            this.OnFire_CameraShakeAmt = 0.8f;
            this.FireTurretSoundSFX = "play_ship_generic_external_weapon_railgun_shoot";
        }
        protected override string GetTurretPrefabPath()
        {
            return "NetworkPrefabs/Component_Prefabs/RailgunTurretAncient";
        }
        public override void Fire(int inProjID, Vector3 dir)
        {
            base.Fire(inProjID, dir);
            if (this.TurretInstance != null)
            {
                foreach (object obj in this.TurretInstance.GetComponent<Animation>())
                {
                    ((AnimationState)obj).speed = 8f;
                }
            }
            ParticleSystem component = this.TurretInstance.OptionalGameObjects[0].GetComponent<ParticleSystem>();
            if (component != null)
            {
                component.Emit(2);
            }
            ParticleSystem component2 = this.TurretInstance.OptionalGameObjects[1].GetComponent<ParticleSystem>();
            if (component2 != null)
            {
                component2.Emit(12);
            }
        }
    }

    class AntiShield : PLLaserTurret
    {
        public AntiShield(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Anti-Shield Turret";
            this.Desc = "A weak but special turret that fires a laser at the exact frequency to ignore the target's shield if they are on static.";
            this.m_Damage = 30f;
            this.FireDelay = 2.4f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Anti-Shield");
            this.m_MarketPrice = 15000;
            base.Level = inLevel;
            base.SubTypeData = (short)inSubTypeData;
            this.TurretRange = 8000f;
            this.m_MaxPowerUsage_Watts = 5500f;
            base.CargoVisualPrefabID = 3;
            base.Experimental = true;
            this.PlayShootSFX = "play_ship_generic_external_weapon_laser_shoot";
            this.StopShootSFX = "";
            this.PlayProjSFX = "play_ship_generic_external_weapon_laser_projectile";
            this.StopProjSFX = "stop_ship_generic_external_weapon_laser_projectile";
            this.LaserDamageType = EDamageType.E_BEAM;
            this.UpdateMaxPowerUsageWatts();
        }
    }

    class InfectedTurret : PLSporeTurret
    {
        public InfectedTurret(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Infected Turret";
            this.Desc = "A dangerous and questionable experiment at using the infected as weapons. Just be carreful with possible subjects escaping the ammo sile";
            this.m_Damage = 250f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Infected Turret");
            this.m_MarketPrice = 21000;
            this.Contraband = true;
            this.m_MaxPowerUsage_Watts = 1000f;
            this.m_ProjSpeed = 350f;
            this.FireDelay = 15f;
            this.TurretRange = 5000f;
            base.Level = inLevel;
        }

        protected override void InnerCheckFire()
        {
            base.InnerCheckFire();
            PLPathfinderGraphEntity pgeforShip = PLPathfinder.GetInstance().GetPGEforShip(this.ShipStats.Ship as PLShipInfo);
            if (pgeforShip != null)
            {
                NNConstraint nnconstraint = new NNConstraint();
                nnconstraint.constrainWalkability = true;
                nnconstraint.walkable = true;
                nnconstraint.graphMask = PLBot.GetContraintForPGE(ref nnconstraint, pgeforShip).graphMask;
                nnconstraint.area = (int)pgeforShip.LargestAreaIndex;
                nnconstraint.constrainArea = true;
                Vector3 position = new Vector3(UnityEngine.Random.Range(pgeforShip.Graph.forcedBounds.min.x, pgeforShip.Graph.forcedBounds.max.x), UnityEngine.Random.Range(pgeforShip.Graph.forcedBounds.min.y, pgeforShip.Graph.forcedBounds.max.y), UnityEngine.Random.Range(pgeforShip.Graph.forcedBounds.min.z, pgeforShip.Graph.forcedBounds.max.z));
                NNInfoInternal nearest = pgeforShip.Graph.GetNearest(position, nnconstraint);
                if (nearest.node != null && nearest.node.Area == pgeforShip.LargestAreaIndex)
                {
                    Ray ray = new Ray((Vector3)nearest.node.position, Vector3.up);
                    RaycastHit raycastHit = default(RaycastHit);
                    if (Physics.Raycast(ray, out raycastHit, 15f, 2048) && Vector3.Dot(raycastHit.normal, Vector3.up) < 0f)
                    {
                        PLInfectedSpider component = PhotonNetwork.Instantiate("NetworkPrefabs/Infected_Spider_01", raycastHit.point + Vector3.up, Quaternion.identity, 0, null).GetComponent<PLInfectedSpider>();
                        if (component != null)
                        {
                            component.MyCurrentTLI = this.ShipStats.Ship.MyTLI;
                        }
                    }
                }
            }

        }
    }
    class HullSmasher : PLBasicTurret
    {
        public HullSmasher(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Hull Smasher";
            this.Desc = "A special Railgun that deals extra damage to hulls and can ignore shields if on modulate, however it has almost no effect against static shields";
            this.m_Damage = 250f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("HullSmasher");
            this.m_MarketPrice = 12000;
            this.Experimental = true;
            this.TurretRange = 5000f;
            this.m_MaxPowerUsage_Watts = 6500f;
            this.FireDelay = 4f;
            base.CargoVisualPrefabID = 3;
            base.Level = inLevel;
            this.OnFire_CameraShakeAmt = 0.8f;
            this.FireTurretSoundSFX = "play_ship_generic_external_weapon_railgun_shoot";
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PLInfectedSporeProj), "FixedUpdate")]
    internal class SporePatch
    {
        private static void Postfix(PLInfectedSporeProj __instance, ref Rigidbody ___MyRigidbody)
        {
            float d = 1f;
            if (PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID).MyStats.Ship.TargetShip != null && PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID).GetTurretAtID(__instance.TurretID).Name == "Infected Turret")
            {
                Vector3 normalized = (PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID).MyStats.Ship.TargetShip.Exterior.transform.position - __instance.transform.position).normalized;
                Quaternion b = Quaternion.LookRotation(normalized);
                ___MyRigidbody.rotation = b;
                float num = (PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID).MyStats.Ship.TargetShip.Exterior.transform.position - __instance.transform.position).magnitude * 0.5f;
                float value = 0.5f;
                if (___MyRigidbody.velocity.sqrMagnitude > 1f)
                {
                    value = Vector3.Dot(___MyRigidbody.velocity.normalized, normalized);
                }
                if (PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID).MyStats.Ship.TargetShip.ExteriorRigidbody != null)
                {
                    float min = Mathf.Max(8f, PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID).MyStats.Ship.TargetShip.ExteriorRigidbody.velocity.magnitude * 1.1f);
                    d = Mathf.Clamp(num * 0.005f * Mathf.Clamp(value, 0.5f, 1f), min, 600f);
                }
                else
                {
                    d = Mathf.Clamp(num * 0.01f, 60f, 450f);
                }
            }
            ___MyRigidbody.AddRelativeForce(Vector3.forward * d);
        }
    }
}
