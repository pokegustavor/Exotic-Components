﻿using PulsarModLoader.Content.Components.Turret;
using UnityEngine;
using Pathfinding;
using System.Collections;
using System.ComponentModel;
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
        public class TweakedAntiShieldMod : TurretMod
        {
            public override string Name => "Tweaked Anti-Shield";

            public override PLShipComponent PLTurret => new TweakedAntiShield();
        }

        public class RNG : TurretMod
        {
            public override string Name => "Respected Nullifier Gun";

            public override PLShipComponent PLTurret => new RespectedNullifierGun();
        }
        public class Defender2 : TurretMod
        {
            public override string Name => "Defender Turret mk2";

            public override PLShipComponent PLTurret => new Defender2Turret();
        }

        public class InternalSpray : TurretMod
        {
            public override string Name => "Internal Corrosion Turret";

            public override PLShipComponent PLTurret => new InternalSprayGun();
        }

        public class OverheatPlasma : TurretMod
        {
            public override string Name => "Super Plasma Turret";

            public override PLShipComponent PLTurret => new OverheatPlasmaTurret();
        }

        public class TractorBeam : TurretMod
        {
            public override string Name => "Tractor Beam Turret";

            public override PLShipComponent PLTurret => new TractorBeamTurret();
        }

        public class MineLauncher : TurretMod
        {
            public override string Name => "Mine Launcher Turret";

            public override PLShipComponent PLTurret => new MineLauncherTurret();
        }
    }

    class SupremeRailGun : PLBasicTurret
    {
        public SupremeRailGun(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Supreme RailGun";
            this.Desc = "Recovered Ancient Human technology, this Railgun is the strongest of its kind, even if not as powerful as the original";
            this.m_Damage = 450f;
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
    class TweakedAntiShield : PLLaserTurret
    {
        public TweakedAntiShield(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Tweaked Anti-Shield Turret";
            this.Desc = "A weak but special turret that fires a laser at the exact frequency to ignore the target's shield if they are on static. This version was upgraded for more damage";
            this.m_Damage = 69f;
            this.FireDelay = 3.1f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Tweaked Anti-Shield");
            this.m_MarketPrice = 30000;
            base.Level = inLevel;
            base.SubTypeData = (short)inSubTypeData;
            this.TurretRange = 8000f;
            this.m_MaxPowerUsage_Watts = 6500f;
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
            this.Desc = "A dangerous and questionable experiment at using the infected as weapons. Just be careful of possible specimens escaping the ammo silo";
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
            if (!(ShipStats.Ship is PLShipInfo)) return;
            PLShipInfo motherShip = this.ShipStats.Ship as PLShipInfo;
            PLPathfinderGraphEntity pgeforShip = PLPathfinder.GetInstance().GetPGEforShip(motherShip);
            if (pgeforShip != null)
            {
                int counter = 0;
                foreach (PLCombatTarget plcombatTarget in PLGameStatic.Instance.AllCombatTargets)
                {
                    if (plcombatTarget != null && plcombatTarget.GetPlayer() == null && plcombatTarget.Lifetime > 4f && plcombatTarget.ShouldShowInHUD() && !plcombatTarget.GetIsFriendly() && plcombatTarget.CurrentShip == motherShip)
                    {
                        counter++;
                        if (counter >= 10) return;
                    }
                }
                int amount = 0;
                float random = Random.value;
                if (random < 0.5f)
                {
                    amount = 1;
                }
                else if (random < 0.75f)
                {
                    amount = 2;
                }
                for (int i = 0; i < amount; i++)
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
    class RespectedNullifierGun : PLLaserTurret
    {
        public RespectedNullifierGun(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Respected Nullifier Gun";
            this.Desc = "A more powerful laser turret that has some kind of portal to another universe (or something like that) and every shot will have a random damage between half and double the base damage, also it has a chance to insta-kill the enemy. You have no conception of how rare that is! IT IS MILLIONS TO ONE!";
            this.m_Damage = 65f;
            this.FireDelay = 5f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Respected Nullifier Gun");
            this.m_MarketPrice = 7500;
            base.Level = inLevel;
            base.SubTypeData = (short)inSubTypeData;
            this.TurretRange = 8000f;
            this.m_MaxPowerUsage_Watts = 9500f;
            base.CargoVisualPrefabID = 3;
            base.Experimental = true;
            this.PlayShootSFX = "play_ship_generic_external_weapon_laser_shoot";
            this.StopShootSFX = "";
            this.PlayProjSFX = "play_ship_generic_external_weapon_laser_projectile";
            this.StopProjSFX = "stop_ship_generic_external_weapon_laser_projectile";
            this.LaserDamageType = EDamageType.E_BEAM;
            this.UpdateMaxPowerUsageWatts();
        }

        public override void UpdateMaxPowerUsageWatts()
        {
            this.m_MaxPowerUsage_Watts = 9500f * base.LevelMultiplier(0.2f, 1f);
        }

        public override void Tick()
        {
            float original = this.m_Damage;
            if (Random.Range(1, 1000000) == 69420) this.m_Damage = 1000000;
            else this.m_Damage *= Random.Range(0.5f, 2f);
            base.Tick();
            m_Damage = original;
        }
    }
    public class Defender2Turret : PLDefenderTurret
    {
        private Vector3[] Offsets_ProjArray;
        public Defender2Turret(int inLevel = 0, int inSubTypeData = 0) : base(inLevel, inSubTypeData)
        {
            Name = "Paragon Defender Turret";
            Desc = "This special defender turret will cause damage based on the currently equiped missile silo, so go ahead with making some combos with this.";
            Level = inLevel;
            SubType = TurretModManager.Instance.GetTurretIDFromName("Defender Turret mk2");
            HasTrackingMissileCapability = true;
            Experimental = true;
            TrackerMissileReloadTime = 9999999f;
            m_MarketPrice = 31000;
            m_ProjSpeed = 190f;
            m_Damage = 60f;
            FireDelay = 9f;
            float x = 0.01f;
            float y = 0.01f;
            this.Offsets_ProjArray = new Vector3[8];
            this.Offsets_ProjArray[0] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(-3f, 0.5f, 0f));
            this.Offsets_ProjArray[1] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(-1f, 0.5f, 0f));
            this.Offsets_ProjArray[2] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(3f, 0.5f, 0f));
            this.Offsets_ProjArray[3] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(1f, 0.5f, 0f));
            this.Offsets_ProjArray[4] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(-3f, -0.5f, 0f));
            this.Offsets_ProjArray[5] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(-1f, -0.5f, 0f));
            this.Offsets_ProjArray[6] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(3f, -0.5f, 0f));
            this.Offsets_ProjArray[7] = Vector3.Scale(new Vector3(x, y, 0f), new Vector3(1f, -0.5f, 0f));
        }
        public override void Tick()
        {
            base.Tick();
            LastFireMissileTime = Time.time;
        }
        public override void Fire(int inProjID, Vector3 dir)
        {
            this.LastFireTime = Time.time;
            this.ChargeAmount = 0f;
            if (Time.time - base.ShipStats.Ship.LastCloakingSystemActivatedTime > 2f)
            {
                base.ShipStats.Ship.SetIsCloakingSystemActive(false);
            }
            PLMusic.PostEvent("play_ship_generic_external_weapon_defenderturret_shoot", this.TurretInstance.gameObject);
            this.TurretInstance.StartCoroutine(this.ControlledFireRoutine(inProjID, dir));
        }
        private IEnumerator ControlledFireRoutine(int inProjID, Vector3 dir)
        {
            PLRand spreadRandomness = new PLRand(inProjID);
            int index = 0;
            foreach (Vector3 b in this.Offsets_ProjArray)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.TurretInstance.Proj, this.TurretInstance.FiringLoc.transform.position + b, this.TurretInstance.FiringLoc.transform.rotation);
                PLMissle component = gameObject.GetComponent<PLMissle>();
                PLProjectile component2 = gameObject.GetComponent<PLProjectile>();
                PLTrackerMissile silo = ShipStats.Ship.SelectedMissileLauncher;
                gameObject.GetComponent<Rigidbody>().velocity = base.ShipStats.Ship.Exterior.GetComponent<Rigidbody>().velocity + dir * this.m_ProjSpeed * Mathf.Clamp((float)spreadRandomness.NextDouble(), 0.5f, 1f);
                component2.ProjID = inProjID + index;
                component.MaxDamage = (this.m_Damage * base.LevelMultiplier(0.15f, 1f) * base.ShipStats.TurretDamageFactor) + (silo != null ? silo.Damage / 30 : 0);
                component.Damage = (this.m_Damage * base.LevelMultiplier(0.15f, 1f) * base.ShipStats.TurretDamageFactor) + (silo != null ? silo.Damage / 30 : 0);
                component2.MaxLifetime = 7f;
                component2.OwnerShipID = base.ShipStats.Ship.ShipID;
                component.TurnFactor *= spreadRandomness.Next(0.33f, 0.66f);
                if (base.ShipStats.Ship.TargetShip != null)
                {
                    component.TargetShipID = base.ShipStats.Ship.TargetShip.ShipID;
                }
                component2.TurretID = this.TurretID;
                component.TargetShip = base.ShipStats.Ship.TargetShip;
                component2.ExplodeOnMaxLifetime = false;
                EDamageType eDamageType;
                if (silo != null)
                {
                    eDamageType = silo.DamageType;
                }
                else
                {
                    eDamageType = EDamageType.E_PHYSICAL;
                }
                switch (eDamageType)
                {
                    case EDamageType.E_SHIELD_PIERCE_PHYS:
                        component.SmokeSys.startColor = Color.yellow;
                        break;
                    case EDamageType.E_INFECTED:
                        component.SmokeSys.startColor = new Color(204f, 88f, 6f, 0.8f);
                        break;
                    case EDamageType.E_SYSTEM_DAMAGE:
                        component.SmokeSys.startColor = Color.red;
                        break;
                    case EDamageType.E_ARMOR_PIERCE_PHYS:
                        component.SmokeSys.startColor = Color.blue;
                        break;
                }
                component2.MyDamageType = eDamageType;
                component.TrackingDelay = spreadRandomness.Next(0.5f, 1.5f);
                component.Speed *= spreadRandomness.Next(0.5f, 1f);
                component.SetShouldLeadTargetShip(true);
                component.SetLerpedSpeed(base.ShipStats.Ship.ExteriorRigidbody.velocity.magnitude * 1.5f);
                Physics.IgnoreCollision(base.ShipStats.Ship.Exterior.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
                PLServer.Instance.m_ActiveProjectiles.Add(component2);
                int num = index;
                index = num + 1;
                yield return new WaitForSeconds(0.1f);
                this.Heat += this.HeatGeneratedOnFire;
            }
            Vector3[] array = null;
            yield break;
        }
    }

    class InternalSprayGun : PLBioHazardTurret
    {
        public InternalSprayGun(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Internal Corrosion Turret";
            this.Desc = "This bio-hazard turret has been modified with a special corrosive fuel that while unable to damage shields or hull, it will completely ignore the hull of the target and melt away their internal components.";
            this.m_Damage = 0.1f;
            this.FireDelay = 5f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Internal Corrosion Turret");
            this.m_MarketPrice = 27000;
            base.Level = inLevel;
            base.SubTypeData = (short)inSubTypeData;
            this.m_MaxPowerUsage_Watts = 5900f;
            base.CargoVisualPrefabID = 3;
            base.Experimental = true;
            MyDamageType = EDamageType.E_SYSTEM_DAMAGE;
        }
        protected override string GetDamageTypeString()
        {
            return "SYSTEM DAMAGE";
        }
    }

    public class OverheatPlasmaTurret : PLBasicTurret
    {
        public OverheatPlasmaTurret(int inLevel = 0, int inSubTypeData = 0) : base(inLevel, inSubTypeData)
        {
            this.Name = "Super Plasma Turret";
            this.Desc = "This plasma turret is not as dense as a normal plasma turret, but launches a way hotter plasma, resulting in less pyhisical damage to the hull, but a lot of heat damage to the target's reactor system.";
            this.m_Damage = 50f;
            this.FireDelay = 1.7f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Super Plasma Turret");
            this.m_MarketPrice = 13000;
            base.Level = inLevel;
            base.SubTypeData = (short)inSubTypeData;
            this.m_MaxPowerUsage_Watts = 4000f;
            base.CargoVisualPrefabID = 3;
            base.Experimental = true;
            ProjSpeed = 600f;
        }

        protected override string GetDamageTypeString()
        {
            return "PHYSICAL (FIRE)";
        }

        public override void Fire(int inProjID, Vector3 dir)
        {
            base.Fire(inProjID, dir);
            foreach (PLProjectile proj in PLServer.Instance.m_ActiveProjectiles)
            {
                if (proj.ProjID == inProjID)
                {
                    proj.MyDamageType = EDamageType.E_FIRE;
                    break;
                }
            }
        }
    }

    public class TractorBeamTurret : PLLaserTurret
    {
        public TractorBeamTurret(int inLevel = 0, int inSubTypeData = 0) : base(0, 0)
        {
            this.Name = "Tractor Beam Turret";
            this.Desc = "A weak but special turret that fires a laser that is capable of pulling the target object towards, great for you spreadshot enjoyers!";
            this.m_Damage = 30f;
            this.FireDelay = 3f;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Tractor Beam Turret");
            this.m_MarketPrice = 19000;
            base.Level = inLevel;
            base.SubTypeData = (short)inSubTypeData;
            this.TurretRange = 8000f;
            this.m_MaxPowerUsage_Watts = 6500f;
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

    public class MineLauncherTurret : PLTurret
    {
        public MineLauncherTurret(int inLevel = 0, int inSubTypeData = 0)
        {
            this.Name = "Mine Launcher Turret";
            this.Desc = "This launcher eqquiped into a turret slot is capable of slowly shooting proximity mines that will explode if any ship besides yours aproaches it. Upgrade it to increase firerate.";
            this.m_Damage = 1200f;
            this.FireDelay = 6;
            base.SubType = TurretModManager.Instance.GetTurretIDFromName("Mine Launcher Turret");
            this.m_MarketPrice = 20000;
            base.Level = inLevel;
            base.SubTypeData = (short)inSubTypeData;
            this.m_MaxPowerUsage_Watts = 4000f;
            base.CargoVisualPrefabID = 3;
            base.HeatGeneratedOnFire = 0.1f;
            base.Experimental = true;
            ProjSpeed = 500;
            FireTurretSoundSFX = "play_ship_generic_external_weapon_railgun_shoot";
        }

        public override string GetStatLineRight()
        {
            float num = this.m_Damage;
            float num2 = this.FireDelay / ((base.ShipStats != null) ? base.ShipStats.TurretChargeFactor : 1f);
            return string.Concat(new string[]
            {
            num.ToString("0"),
            "\n",
            num2.ToString("0.0"),
            "\n",
            this.GetDamageTypeString(),
            "\n"
            });
        }

        public override void Fire(int inProjID, Vector3 dir)
        {
            this.ChargeAmount = 0f;
            this.LastFireTime = Time.time;
            this.Heat += this.HeatGeneratedOnFire;
            if (this.TurretInstance == null || this.TurretInstance.FiringLoc == null || this.TurretInstance.Proj == null)
            {
                return;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PLGlobal.Instance.ProximityMinePrefab, this.TurretInstance.FiringLoc.transform.position + dir*5, this.TurretInstance.FiringLoc.transform.rotation);
            gameObject.name = $"TurretedMine({ShipStats.Ship.ShipID})";
            gameObject.transform.localScale = Vector3.one;
            PLSpecialEncounterNetObject.InitNewObject(gameObject.GetComponent<PLProximityMine>(), PLEncounterManager.Instance.GetCurrentPersistantEncounterInstance());
            if (gameObject == null)
            {
                return;
            }
            gameObject.GetComponent<Rigidbody>().velocity = base.ShipStats.Ship.ExteriorRigidbody.velocity + dir * this.m_ProjSpeed;
            if (base.ShipStats.Ship.GetExteriorMeshCollider() != null)
            {
                Physics.IgnoreCollision(base.ShipStats.Ship.GetExteriorMeshCollider(), gameObject.GetComponent<Collider>());
            }
            foreach (Collider collider in base.ShipStats.Ship.ExtraColliders)
            {
                if (collider != null)
                {
                    Physics.IgnoreCollision(collider, gameObject.GetComponent<Collider>());
                }
            }
            this.CurrentCameraShake += this.OnFire_CameraShakeAmt;
            if (this.FireTurretSoundSFX != "")
            {
                PLMusic.PostEvent(this.FireTurretSoundSFX, this.TurretInstance.gameObject);
            }
            if (Time.time - base.ShipStats.Ship.LastCloakingSystemActivatedTime > 2f)
            {
                base.ShipStats.Ship.SetIsCloakingSystemActive(false);
            }
            PLServer.Instance.m_ActiveProjectiles.Add(gameObject.GetComponent<PLProjectile>());
            if (this.TurretInstance != null && this.TurretInstance.GetComponent<Animation>() != null)
            {
                this.TurretInstance.GetComponent<Animation>().Play(this.TurretInstance.FireAnimationName);
            }
        }

        public override void Tick()
        {
            FireDelay = Mathf.Clamp(6f - (Level * 0.4f), 0.5f, 6);
            base.Tick();
        }

        protected override string GetTurretPrefabPath()
        {
            return "NetworkPrefabs/Component_Prefabs/RailgunTurret";
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PLInfectedSporeProj), "FixedUpdate")]
    internal class SporePatch
    {
        private static void Postfix(PLInfectedSporeProj __instance, ref Rigidbody ___MyRigidbody)
        {
            if (PLEncounterManager.Instance.PlayerShip == null || PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID) == null || PLEncounterManager.Instance.GetShipFromID(__instance.OwnerShipID).IsInfected) return;
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

    [HarmonyLib.HarmonyPatch(typeof(PLFlamelanceTurret), "PlayFireSFX")]
    internal class FixSpraySound
    {
        static void Postfix(int ID, GameObject target, PLFlamelanceTurret __instance)
        {
            if (ID == 1 && !__instance.playFireLoop)
            {
                if (__instance is InternalSprayGun)
                {
                    PLMusic.PostEvent("play_ship_generic_external_weapon_biohazardturret_fire", target);
                    __instance.playFireLoop = true;
                }
            }
            else if (ID == 2 && __instance.playFireLoop)
            {
                if (__instance is InternalSprayGun)
                {
                    PLMusic.PostEvent("stop_ship_generic_external_weapon_biohazardturret_fire", target);
                    __instance.playFireLoop = false;
                }
            }
        }
    }
    [HarmonyLib.HarmonyPatch(typeof(PLProximityMine), "Update")]
    class FixMines
    {
        static bool Prefix(PLProximityMine __instance)
        {
            if (__instance.name.Contains("TurretedMine"))
            {
                __instance.GetComponent<Renderer>().enabled = !__instance.Exploded;
                __instance.GetComponent<Collider>().enabled = !__instance.Exploded;
                __instance.m_MySpecialEncounterNetObjectPersistantData.PersistantState = __instance.Exploded;
                string shipIDStr = __instance.name.Split('(')[1];
                int shipID = int.Parse(shipIDStr.Remove(shipIDStr.IndexOf(')'), 1));
                bool nearbyShip = false;
                __instance.EmissiveMaterial.EnableKeyword("_EMISSION");
                foreach (PLShipInfoBase ship in PLEncounterManager.Instance.AllShips.Values)
                {
                    if (ship != null && ship.ShipID != shipID)
                    {
                        float magnitude = (ship.ExteriorTransformCached.position - __instance.transform.position).magnitude;
                        if (!__instance.Exploded && __instance.EmissiveMaterial != null)
                        {
                            if (magnitude < __instance.Range * 2f)
                            {
                                nearbyShip = true;
                            }
                            if (magnitude < __instance.Range && !__instance.Exploded && PhotonNetwork.isMasterClient)
                            {
                                PLServer.Instance.photonView.RPC("ProximityMineExplode", PhotonTargets.All, new object[]
                                {
                                    __instance.EncounterNetID
                                });
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }
    }
}
