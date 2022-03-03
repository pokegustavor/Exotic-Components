using PulsarModLoader.Content.Components.MegaTurret;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Exotic_Components
{
	public class Main_Turrets
	{
		class MachineTurretMod : MegaTurretMod
		{
			public override string Name => "MachineGunMainTurret";

			public override PLShipComponent PLMegaTurret => new MachineGunTurret();
		}

		class SilentDeathMod : MegaTurretMod
		{
			public override string Name => "SilentDeath";

			public override PLShipComponent PLMegaTurret => new SilentDeath();
		}

		public class TweakedMachineTurretMod : MegaTurretMod
		{
			public override string Name => "TweakedMachineGunMainTurret";

			public override PLShipComponent PLMegaTurret => new TweakedMachineGunTurret();
		}
		class FlagShipMainTurretMod : MegaTurretMod 
		{
			public override string Name => "FlagShipMainTurret";

			public override PLShipComponent PLMegaTurret => new FlagShipMainTurret();
		}
		class InfectedBeamMod : MegaTurretMod
		{
			public override string Name => "InfectedBeamMainTurret";

			public override PLShipComponent PLMegaTurret => new InfectedBeam();
		}
		class FakeKeeperBeamMod : MegaTurretMod
		{
			public override string Name => "FakeKeeperBeamTurret";

			public override PLShipComponent PLMegaTurret => new FakeKeeper();
		}
		class InstabilityTurretMod : MegaTurretMod
		{
			public override string Name => "InstabilityTurret";

			public override PLShipComponent PLMegaTurret => new InstabilityTurret();
		}
	}

	class MachineGunTurret : PLMegaTurret_Proj
	{
		public MachineGunTurret(int inLevel = 0, int inSubTypeData = 1) : base(inLevel, inSubTypeData)
		{
			this.Name = "Machine Gun Turret";
			this.Desc = "This old turret is designed for extreme firerate but with extremely low damage, time to melt some enemies";
			this.m_Damage = 25f;
			this.MinFireDelay = 0.009f;
			this.FireDelay = 0.009f;
			this.HeatGeneratedOnFire = 0.024f;
			this.m_MaxPowerUsage_Watts = 7500f;
			this.m_ProjSpeed = 5000f;
			base.SubType = MegaTurretModManager.Instance.GetMegaTurretIDFromName("MachineGunMainTurret"); ;
			this.m_MarketPrice = 24600;
			base.CargoVisualPrefabID = 5;
			this.m_SlotType = ESlotType.E_COMP_MAINTURRET;
			this.m_KickbackForceMultiplier = 0.002f;
			this.OnFire_CameraShakeAmt = 0.002f;
			this.m_AutoAimMinDotPrd = 0.99f;
			this.AutoAimEnabled = true;
			this.IsMainTurret = true;
			this.HasTrackingMissileCapability = true;
			this.HasPulseLaser = true;
		}
	}
	class TweakedMachineGunTurret : PLMegaTurret_Proj
	{
		public TweakedMachineGunTurret(int inLevel = 0, int inSubTypeData = 1) : base(inLevel, inSubTypeData)
		{
			this.Name = "Tweaked Machine Gun Turret";
			this.Desc = "This old turret is designed for extreme firerate but with extremely low damage, time to melt some enemies. This version was upgraded for more damage and less heat generated";
			this.m_Damage = 75f;
			this.MinFireDelay = 0.009f;
			this.FireDelay = 0.009f;
			this.HeatGeneratedOnFire = 0.008f;
			this.m_MaxPowerUsage_Watts = 18000f;
			this.m_ProjSpeed = 5000f;
			base.SubType = MegaTurretModManager.Instance.GetMegaTurretIDFromName("TweakedMachineGunMainTurret"); ;
			this.m_MarketPrice = 24600;
			base.CargoVisualPrefabID = 5;
			this.m_SlotType = ESlotType.E_COMP_MAINTURRET;
			this.m_KickbackForceMultiplier = 0.002f;
			this.OnFire_CameraShakeAmt = 0.002f;
			this.m_AutoAimMinDotPrd = 0.99f;
			this.AutoAimEnabled = true;
			this.IsMainTurret = true;
			this.HasTrackingMissileCapability = true;
			this.HasPulseLaser = true;
		}
	}
	class SilentDeath : PLMegaTurret_RapidFire
	{
		public SilentDeath(int inLevel = 0, int inSubTypeData = 1) : base(0)
		{
			this.Name = "Silent Death";
			this.Desc = "A special Sylvassi turret made to fire without breaking the cloak of a ship. Due to the possible malicious uses, it has being marked as a contraband";
			this.m_Damage = 280f;
			base.SubType = MegaTurretModManager.Instance.GetMegaTurretIDFromName("SilentDeath");
			this.Contraband = true;
			this.m_MarketPrice = 42200;
			this.FireDelay = 13f;
			this.m_MaxPowerUsage_Watts = 15000f;
			base.CargoVisualPrefabID = 5;
			this.TurretRange = 7000f;
			this.BeamColor = new Color(0.1f, 0.1f, 1.5f);
			this.MegaTurretExplosionID = 0;
			this.m_KickbackForceMultiplier = 0.8f;
			this.m_AutoAimMinDotPrd = 0.98f;
			this.HeatGeneratedOnFire = 0.7f;
			this.AutoAimEnabled = true;
			this.IsMainTurret = true;
			this.HasTrackingMissileCapability = true;
		}
		private int m_StoredProjID;
		private IEnumerator DelayedMegaTurretDamage(PLShipInfoBase hitShip, bool operatedByBot, RaycastHit hitInfo, float visibleChargeLevel)
		{
			yield return new WaitForSeconds(0.33f);
			if (hitShip != null)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				float damageDoneWithTiming = this.GetDamageDoneWithTiming(visibleChargeLevel, operatedByBot);
				PLServer.Instance.photonView.RPC("MegaTurretDamage", PhotonTargets.Others, new object[]
				{
				hitShip.ShipID,
				damageDoneWithTiming,
				num,
				hitShip.Exterior.transform.InverseTransformPoint(hitInfo.point),
				base.ShipStats.Ship.ShipID,
				this.TurretID,
				this.m_StoredProjID
				});
				PLServer.Instance.MegaTurretDamage(hitShip.ShipID, damageDoneWithTiming, num, hitShip.Exterior.transform.InverseTransformPoint(hitInfo.point), base.ShipStats.Ship.ShipID, this.TurretID, this.m_StoredProjID);
			}
			yield break;
		}
		private float GetDamageDoneWithTiming(float visibleChargeLevel, bool operatedByBot)
		{
			float num = Mathf.Clamp01(Mathf.Abs(visibleChargeLevel - 0.569565f));
			if (PhotonNetwork.isMasterClient && (operatedByBot || base.ShipStats.Ship.IsDrone))
			{
				num = UnityEngine.Random.Range(0.01f, 0.1f);
			}
			return (150f + (1f - Mathf.Clamp01(Mathf.Abs(Mathf.Pow(num * 100f, 2f) / 10000f))) * (this.m_Damage - 150f)) * base.LevelMultiplier(0.15f, 1f) * base.ShipStats.TurretDamageFactor;
		}
		private float m_VisibleChargeLevel;
		protected override void ChargeComplete(int inProjID, Vector3 dir)
		{
			this.LastFireTime = Time.time;
			if (this.TurretInstance == null)
			{
				return;
			}
			base.ShipStats.Ship.Exterior.GetComponent<Rigidbody>().AddForceAtPosition(-1200f * dir * this.m_KickbackForceMultiplier, this.TurretInstance.transform.position, ForceMode.Impulse);
			this.CurrentCameraShake += 2f;
			this.Heat += this.HeatGeneratedOnFire;
			PLMusic.PostEvent("play_ship_generic_external_weapon_maingun_shoot", this.TurretInstance.gameObject);
			PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID));
			bool flag = false;
			if (playerFromPlayerID != null)
			{
				flag = playerFromPlayerID.IsBot;
			}
			Ray ray = new Ray(this.TurretInstance.FiringLoc.position, this.TurretInstance.FiringLoc.forward);
			RaycastHit hitInfo = default(RaycastHit);
			int layerMask = 524289;
			int layer = 0;
			if (base.ShipStats.Ship.GetExteriorMeshCollider() != null)
			{
				layer = base.ShipStats.Ship.GetExteriorMeshCollider().gameObject.layer;
				base.ShipStats.Ship.GetExteriorMeshCollider().gameObject.layer = 31;
			}
			try
			{
				if (Physics.SphereCast(ray, 4f, out hitInfo, 10600f, layerMask))
				{
					PLShipInfoBase plshipInfoBase = null;
					PLDamageableSpaceObject_Collider pldamageableSpaceObject_Collider = null;
					bool flag2 = false;
					if (hitInfo.collider != null)
					{
						plshipInfoBase = hitInfo.collider.GetComponentInParent<PLShipInfoBase>();
						pldamageableSpaceObject_Collider = hitInfo.collider.GetComponent<PLDamageableSpaceObject_Collider>();
					}
					PLProximityMine plproximityMine = null;
					if (plshipInfoBase == null)
					{
						plproximityMine = hitInfo.collider.GetComponentInParent<PLProximityMine>();
					}
					PLSpaceTarget plspaceTarget = null;
					if (plproximityMine == null)
					{
						plspaceTarget = hitInfo.collider.GetComponentInParent<PLSpaceTarget>();
					}
					if (pldamageableSpaceObject_Collider != null)
					{
						pldamageableSpaceObject_Collider.MyDSO.TakeDamage_Location(this.GetDamageDoneWithTiming(this.m_VisibleChargeLevel, flag), hitInfo.point, hitInfo.normal);
					}
					else if (plproximityMine != null)
					{
						PLServer.Instance.photonView.RPC("ProximityMineExplode", PhotonTargets.All, new object[]
						{
						plproximityMine.EncounterNetID
						});
					}
					else if (plshipInfoBase != null)
					{
						if (plshipInfoBase != base.ShipStats.Ship)
						{
							base.ShipStats.Ship.StartCoroutine(this.DelayedMegaTurretDamage(plshipInfoBase, flag, hitInfo, this.m_VisibleChargeLevel));
							PLServer.Instance.photonView.RPC("MegaTurretExplosion", PhotonTargets.Others, new object[]
							{
							hitInfo.point,
							plshipInfoBase.ShipID,
							this.MegaTurretExplosionID
							});
							PLServer.Instance.MegaTurretExplosion(hitInfo.point, plshipInfoBase.ShipID, this.MegaTurretExplosionID);
						}
					}
					else if (hitInfo.collider != null && hitInfo.collider.CompareTag("Projectile"))
					{
						PLProjectile component = hitInfo.collider.gameObject.GetComponent<PLProjectile>();
						if (component != null && component.LaserCanCauseExplosion && component.OwnerShipID != -1 && component.OwnerShipID != base.ShipStats.Ship.ShipID)
						{
							component.TakeDamage(this.GetDamageDoneWithTiming(this.m_VisibleChargeLevel, flag), hitInfo.point, hitInfo.normal, playerFromPlayerID);
							flag2 = true;
						}
					}
					else if (plspaceTarget != null)
					{
						plspaceTarget.photonView.RPC("NetTakeDamage", PhotonTargets.All, new object[]
						{
						this.GetDamageDoneWithTiming(this.m_VisibleChargeLevel, flag)
						});
						PLServer.Instance.photonView.RPC("MegaTurretExplosion", PhotonTargets.Others, new object[]
						{
						hitInfo.point,
						-1,
						this.MegaTurretExplosionID
						});
						PLServer.Instance.MegaTurretExplosion(hitInfo.point, -1, this.MegaTurretExplosionID);
					}
					else
					{
						PLServer.Instance.photonView.RPC("MegaTurretExplosion", PhotonTargets.Others, new object[]
						{
						hitInfo.point,
						-1,
						this.MegaTurretExplosionID
						});
						PLServer.Instance.MegaTurretExplosion(hitInfo.point, -1, this.MegaTurretExplosionID);
					}
					if (!flag2)
					{
						this.LaserDist = (hitInfo.point - this.TurretInstance.FiringLoc.position).magnitude;
					}
					else
					{
						this.LaserDist = 20000f;
					}
				}
				else
				{
					this.LaserDist = 20000f;
				}
				Vector3 vector;
				int num;
				PLSwarmCollider hitSwarmCollider = this.GetHitSwarmCollider(this.LaserDist, out vector, out num);
				if (hitSwarmCollider != null)
				{
					float num2 = UnityEngine.Random.Range(0f, 1f);
					hitSwarmCollider.MyShipInfo.HandleSwarmColliderHitVisuals(hitSwarmCollider, vector);
					float num3 = Mathf.Clamp01(Mathf.Abs(this.m_VisibleChargeLevel - 0.569565f));
					if (PhotonNetwork.isMasterClient && (flag || base.ShipStats.Ship.IsDrone))
					{
						num3 = UnityEngine.Random.Range(0.01f, 0.1f);
					}
					float num4 = (150f + (1f - Mathf.Clamp01(Mathf.Abs(Mathf.Pow(num3 * 100f, 2f) / 10000f))) * (this.m_Damage - 150f)) * base.LevelMultiplier(0.15f, 1f) * base.ShipStats.TurretDamageFactor;
					num4 *= (float)num * 0.25f;
					PLServer.Instance.photonView.RPC("MegaTurretDamage", PhotonTargets.Others, new object[]
					{
					hitSwarmCollider.MyShipInfo.ShipID,
					num4,
					num2,
					hitSwarmCollider.MyShipInfo.Exterior.transform.InverseTransformPoint(vector),
					base.ShipStats.Ship.ShipID,
					this.TurretID,
					this.m_StoredProjID
					});
					PLServer.Instance.MegaTurretDamage(hitSwarmCollider.MyShipInfo.ShipID, num4, num2, hitSwarmCollider.MyShipInfo.Exterior.transform.InverseTransformPoint(vector), base.ShipStats.Ship.ShipID, this.TurretID, this.m_StoredProjID);
				}
			}
			catch
			{
			}
			if (base.ShipStats.Ship.GetExteriorMeshCollider() != null)
			{
				base.ShipStats.Ship.GetExteriorMeshCollider().gameObject.layer = layer;
			}
			this.m_IsCharging = false;
			this.m_IsVisiblyCharging = false;
			this.m_ChargeLevel = 0f;
			this.m_VisibleChargeLevel = 0f;
		}
		private PLSwarmCollider GetHitSwarmCollider(float laserDist, out Vector3 hitLoc, out int numHit)
		{
			PLSwarmCollider plswarmCollider = null;
			Vector3 vector = Vector3.zero;
			float num = float.MaxValue;
			int num2 = 0;
			foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
			{
				if (plshipInfoBase != null)
				{
					foreach (PLSwarmCollider plswarmCollider2 in plshipInfoBase.OptionalSwarmColliders)
					{
						bool flag = false;
						foreach (Vector3 vector2 in PLSwarmCollider.FindLineSphereIntersections(this.TurretInstance.FiringLoc.transform.position + this.TurretInstance.FiringLoc.transform.forward * 5f, this.TurretInstance.FiringLoc.transform.position + this.TurretInstance.FiringLoc.transform.forward * laserDist, plswarmCollider2.transform.position, plswarmCollider2.ColliderRadius))
						{
							flag = true;
							float num3 = Vector3.SqrMagnitude(this.TurretInstance.FiringLoc.transform.position - vector2);
							if (num3 < num)
							{
								plswarmCollider = plswarmCollider2;
								num = num3;
								vector = vector2;
							}
						}
						if (flag)
						{
							num2++;
						}
					}
				}
			}
			if (plswarmCollider != null)
			{
				hitLoc = vector;
				numHit = num2;
				return plswarmCollider;
			}
			hitLoc = Vector3.zero;
			numHit = 0;
			return null;
		}
		private bool m_IsCharging;
		private bool m_IsVisiblyCharging;
		private float m_ChargeLevel;
	}
    class FlagShipMainTurret : PLMegaTurret
    {
        public FlagShipMainTurret(int inLevel = 0) : base(inLevel)
        {
			Level = inLevel;
			SubType = MegaTurretModManager.Instance.GetMegaTurretIDFromName("FlagShipMainTurret");
			Name = "Flagship MainTurret";
			Desc = "This monster of a turret, that was going to be used in a flagship, will obliterate any ship that faces it, but it does use a lot of power and has a big knockback. Also it will be expensive to equip, because is quite... big for your ship, but if you think you can handle the size.";
			BeamColor = Color.green;
			m_Damage = 4900f;
			TurretRange = 50000f;
			TrackerMissileReloadTime = 5f;
			m_MaxPowerUsage_Watts = 190000f;
			FireDelay = 45f;
			m_MarketPrice = 1450000;
			HeatGeneratedOnFire *= 3;
			m_KickbackForceMultiplier *= 7;
			CoolingRateModifier *= 0.3f;
			HasPulseLaser = false;
		}
		protected override string GetTurretPrefabPath()
		{
			return "NetworkPrefabs/Component_Prefabs/CorruptedLaserTurret";
		}

        public override void Tick()
        {
            base.Tick();
			if (TurretInstance != null && TurretInstance.OptionalGameObjects[1] != null)
			{
				bool flag = ChargeAmount > 0.7f;
				GameObject gameObject = TurretInstance.OptionalGameObjects[1];
				if (!IsFiring && flag)
				{
					Ray ray = new Ray(TurretInstance.FiringLoc.position, TurretInstance.FiringLoc.forward);
					RaycastHit raycastHit = default(RaycastHit);
					int layerMask = 524289;
					if (Physics.SphereCast(ray, 1f, out raycastHit, 20000f, layerMask))
					{
						LaserDist = (raycastHit.point - TurretInstance.FiringLoc.position).magnitude * (1f / TurretInstance.transform.parent.lossyScale.x);
					}
					else
					{
						LaserDist = 20000f;
					}
				}
				if (gameObject != null)
				{
					float num = Mathf.Min(50000f, LaserDist);
					gameObject.transform.localPosition = new Vector3(0f, 0f, num * 0.5f);
					Mathf.Abs(0.2f);
					gameObject.transform.localScale = new Vector3(1f, num * 0.5f, 1f);
					if (gameObject.activeSelf != flag)
					{
						gameObject.SetActive(flag);
					}
				}
			}
		}
        public override void FinalLateAddStats(PLShipStats inStats)
        {
            base.FinalLateAddStats(inStats);
			inStats.Mass += 2400f;
        }
    }
    class InfectedBeam : PLMegaTurretCU_2
    {
        public InfectedBeam(int inLevel = 0, int inSubTypeData = 1) : base(inLevel, inSubTypeData)
        {
			Name = "Infected Beam";
			Desc = "Using some biological materials from the infected, this turret will cause a massive amount of damage to shields and hull, just hope you don't mind using biological warfare.";
			Contraband = true;
			m_MarketPrice = 23000;
			FireDelay = 14f;
			m_Damage = 650;
			DamageType = EDamageType.E_INFECTED;
			SubType = MegaTurretModManager.Instance.GetMegaTurretIDFromName("InfectedBeamMainTurret");
			BeamColor = new Color(204f, 88f, 6f, 0.8f);
			m_MaxPowerUsage_Watts = 18700f;
			Level = inLevel;
		}
		protected override string GetDamageTypeString()
		{
			return "INFECTED (BEAM)";
		}
	}
    class FakeKeeper : PLMegaTurret_KeeperBeam
    {
        public FakeKeeper(int inLevel = 0, int inSubTypeData = 1) : base(inLevel, inSubTypeData)
        {
			Name = "Keeper Beam Prototype";
			Desc = "This special turret was made with data collected from an alien machine called \"Keeper\", it is quite powerfull, but this prototype doesn't have an... off button. So be carefull with the power you will use, it will only stop when it overheat. Look, copying alien tech from some scans is not easy, ok.";
			m_Damage = 100f;
			Experimental = true;
			Level = inLevel;
			FireDelay = 30f;
			m_MarketPrice = 39000;
			HeatGeneratedOnFire = 0.05f;
			CoolingRateModifier *= 0.5f;
			SubType = MegaTurretModManager.Instance.GetMegaTurretIDFromName("FakeKeeperBeamTurret");
			BeamActiveTime = 50000f;
		}
        public override void Tick()
        {
			if (IsOverheated || ShipStats.Ship.IsReactorOverheated()) IsBeamActive = false;
			base.Tick();
			if (IsBeamActive)
			{
				ChargeAmount = 0f;
			}
		}
		public override void UpdateMaxPowerUsageWatts()
		{
			base.CalculatedMaxPowerUsage_Watts = 25600f * base.LevelMultiplier(0.1f, 1f);
		}
        protected override void UpdatePowerUsage(PLPlayer currentOperator)
        {
            base.UpdatePowerUsage(currentOperator);
			if (IsBeamActive) RequestPowerUsage_Percent = ShipStats.Ship.WeaponsSystem.GetHealthRatio();
		}
		protected override bool ShouldAIFire(bool operatedByBot, float heatOffset, float heatGeneratedOnFire)
		{
			float num = this.Heat + heatOffset;
			return num < 1f - heatGeneratedOnFire;
		}
	}

	class InstabilityTurret : PLTurret
	{
		public InstabilityTurret(int inLevel = 0) : base(ESlotType.E_COMP_MAINTURRET)
		{
			this.Name = "Anti-Matter Lance";
			this.Desc = "Banned for tampering with core safety protocols, this highly unstable prototype harnesses the high energy particles ejected by ship cores during overheating. Damage has been observed to scale with cores closer to meltdown. It also may periodically damage the weapons system, and explode screens";
			this.m_IconTexture = (Texture2D)Resources.Load("Icons/8_Weapons");
			this.m_Damage = 80f;
			this.baseDamage = 80f;
			this.SubType = MegaTurretModManager.Instance.GetMegaTurretIDFromName("InstabilityTurret");
			this.m_MarketPrice = 6200;
			this.FireDelay = 12f;
			this.m_MaxPowerUsage_Watts = 1f;
			base.CargoVisualPrefabID = 5;
			this.TurretRange = 12000f;
			this.BeamColor = new Color(92f, 3f, 121f,0.78f);
			this.MegaTurretExplosionID = 0;
			this.Contraband = true;
			base.Level = inLevel;
			this.CoolingRateModifier = 0.6f;
			this.m_AutoAimMinDotPrd = 0.93f;
			this.HeatGeneratedOnFire = 0.7f;
			this.AutoAimEnabled = true;
			this.IsMainTurret = true;
			base.SysInstConduit = 10;
			this.CanHitMissiles = true;
			this.DamageType = EDamageType.E_PHYSICAL;
		}

		protected override bool ShouldLookForMissilesToShoot()
		{
			return !this.m_IsVisiblyCharging;
		}
		protected override string GetTurretPrefabPath()
		{
			return "NetworkPrefabs/Component_Prefabs/MegaTurret";
		}
		public override void Fire(int inProjID, Vector3 dir)
		{
			if (!this.m_IsCharging)
			{
				this.ChargeAmount = 0f;
				this.m_StoredProjID = inProjID;
				PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID));
				bool flag = false;
				if (playerFromPlayerID != null)
				{
					flag = playerFromPlayerID.IsBot;
				}
				if (((PhotonNetwork.isMasterClient && (flag || base.ShipStats.Ship.IsDrone)) || PLNetworkManager.Instance.LocalPlayerID == base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID)) && PLNetworkManager.Instance.LocalPlayerID != -1)
				{
					int num = Mathf.RoundToInt(1f / this.LastCalculated_ChargeAmountDeltaBeforePower * 1000f * this.turretChargeSpeed_ToVisualChargeSpeed);
					PLServer.Instance.photonView.RPC("ClientMainTurretChargeBegin", PhotonTargets.Others, new object[]
					{
					base.ShipStats.Ship.ShipID,
					PLServer.Instance.GetEstimatedServerMs() + num
					});
					PLServer.Instance.ClientMainTurretChargeBegin(base.ShipStats.Ship.ShipID, PLServer.Instance.GetEstimatedServerMs() + num);
				}
			}
		}
		protected override bool AutoAimTrackingIsEnabled()
		{
			PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID));
			bool flag = false;
			if (playerFromPlayerID != null && playerFromPlayerID.IsBot)
			{
				flag = true;
			}
			return flag || base.ShipStats.Ship.IsDrone;
		}
		public override void AddStats(PLShipStats inStats)
		{
			base.AddStats(inStats);
			if (Time.time - this.LastFireTime < 1f)
			{
				inStats.EMSignature += 1f - Mathf.Clamp01(Time.time - this.LastFireTime);
			}
		}
		protected override string GetDamageTypeString()
		{
			return "PHYSICAL (BEAM)";
		}
		private PLSwarmCollider GetHitSwarmCollider(float laserDist, out Vector3 hitLoc, out int numHit)
		{
			PLSwarmCollider plswarmCollider = null;
			Vector3 vector = Vector3.zero;
			float num = float.MaxValue;
			int num2 = 0;
			foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
			{
				if (plshipInfoBase != null)
				{
					foreach (PLSwarmCollider plswarmCollider2 in plshipInfoBase.OptionalSwarmColliders)
					{
						bool flag = false;
						foreach (Vector3 vector2 in PLSwarmCollider.FindLineSphereIntersections(this.TurretInstance.FiringLoc.transform.position + this.TurretInstance.FiringLoc.transform.forward * 5f, this.TurretInstance.FiringLoc.transform.position + this.TurretInstance.FiringLoc.transform.forward * laserDist, plswarmCollider2.transform.position, plswarmCollider2.ColliderRadius))
						{
							flag = true;
							float num3 = Vector3.SqrMagnitude(this.TurretInstance.FiringLoc.transform.position - vector2);
							if (num3 < num)
							{
								plswarmCollider = plswarmCollider2;
								num = num3;
								vector = vector2;
							}
						}
						if (flag)
						{
							num2++;
						}
					}
				}
			}
			if (plswarmCollider != null)
			{
				hitLoc = vector;
				numHit = num2;
				return plswarmCollider;
			}
			hitLoc = Vector3.zero;
			numHit = 0;
			return null;
		}

		public bool ShouldProcessProj(int ProjID)
		{
			if (!this.ProcessedProjs.Contains(ProjID))
			{
				this.ProcessedProjs.Add(ProjID);
				return true;
			}
			return false;
		}
		protected virtual void ChargeComplete(int inProjID, Vector3 dir)
		{
			if (base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) != -1)
			{
				PulsarModLoader.Utilities.Messaging.Notification("Damage before math: " + this.m_Damage,PLNetworkManager.Instance.LocalPlayer,default,20000,default);
				PulsarModLoader.Utilities.Messaging.Notification("Reactor maxt temp: " + (0.01f * this.ShipStats.ReactorTempMax), PLNetworkManager.Instance.LocalPlayer, default, 20000, default);
				PulsarModLoader.Utilities.Messaging.Notification("Reactor maxt power: " + (0.001f * this.ShipStats.ReactorBoostedOutputMax), PLNetworkManager.Instance.LocalPlayer, default, 20000, default);
				PulsarModLoader.Utilities.Messaging.Notification("Reactor current instability: " + (this.ShipStats.Ship.CoreInstability), PLNetworkManager.Instance.LocalPlayer, default, 20000, default);
				PulsarModLoader.Utilities.Messaging.Notification("Reactor initial instability: " + (this.initialInstability), PLNetworkManager.Instance.LocalPlayer, default, 20000, default);
				PulsarModLoader.Utilities.Messaging.Notification("Damage multiplication: " + ((0.01f * this.ShipStats.ReactorTempMax + 0.001f * this.ShipStats.ReactorBoostedOutputMax) / (((1.07f + this.initialInstability) - (this.ShipStats.Ship.CoreInstability)))), PLNetworkManager.Instance.LocalPlayer, default, 20000, default);
				PulsarModLoader.Utilities.Logger.Info("Damage before math: " + this.m_Damage);
				PulsarModLoader.Utilities.Logger.Info("Reactor maxt temp: " + (0.01f * this.ShipStats.ReactorTempMax));
				PulsarModLoader.Utilities.Logger.Info("Reactor maxt power: " + (0.001f * this.ShipStats.ReactorBoostedOutputMax));
				PulsarModLoader.Utilities.Logger.Info("Reactor current instability: " + (this.ShipStats.Ship.CoreInstability));
				PulsarModLoader.Utilities.Logger.Info("Reactor initial instability: " + (this.initialInstability));
				PulsarModLoader.Utilities.Logger.Info("Damage multiplication: " + ((0.01f * this.ShipStats.ReactorTempMax + 0.001f * this.ShipStats.ReactorBoostedOutputMax) / (((1.07f + this.initialInstability) - (this.ShipStats.Ship.CoreInstability)))));
				
			}
			this.m_Damage = this.m_Damage * ((0.01f * this.ShipStats.ReactorTempMax + 0.001f * this.ShipStats.ReactorBoostedOutputMax) / (((1.07f + this.initialInstability) - (this.ShipStats.Ship.CoreInstability))));
			this.LastFireTime = Time.time;
			if (this.TurretInstance == null)
			{
				this.m_Damage = this.baseDamage;
				return;
			}
			if (base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) != -1)
			{
				PulsarModLoader.Utilities.Messaging.Notification("Damage value1: " + this.m_Damage, PLNetworkManager.Instance.LocalPlayer, default, 9000, default);
			}
			base.ShipStats.Ship.Exterior.GetComponent<Rigidbody>().AddForceAtPosition(-1200f * dir * this.m_KickbackForceMultiplier, this.TurretInstance.transform.position, ForceMode.Impulse);
			this.CurrentCameraShake += 2f;
			this.Heat += this.HeatGeneratedOnFire;
			PLMusic.PostEvent("play_ship_generic_external_weapon_maingun_shoot", this.TurretInstance.gameObject);
			PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID));
			bool flag = false;
			if (playerFromPlayerID != null)
			{
				flag = playerFromPlayerID.IsBot;
			}
			Ray ray = new Ray(this.TurretInstance.FiringLoc.position, this.TurretInstance.FiringLoc.forward);
			RaycastHit hitInfo = default(RaycastHit);
			int layerMask = 524289;
			int layer = 0;
			if (base.ShipStats.Ship.GetExteriorMeshCollider() != null)
			{
				layer = base.ShipStats.Ship.GetExteriorMeshCollider().gameObject.layer;
				base.ShipStats.Ship.GetExteriorMeshCollider().gameObject.layer = 31;
			}
			try
			{
				if (Physics.SphereCast(ray, 4f, out hitInfo, 10600f, layerMask))
				{
					PLShipInfoBase plshipInfoBase = null;
					PLDamageableSpaceObject_Collider pldamageableSpaceObject_Collider = null;
					bool flag2 = false;
					if (hitInfo.collider != null)
					{
						plshipInfoBase = hitInfo.collider.GetComponentInParent<PLShipInfoBase>();
						pldamageableSpaceObject_Collider = hitInfo.collider.GetComponent<PLDamageableSpaceObject_Collider>();
					}
					PLProximityMine plproximityMine = null;
					if (plshipInfoBase == null)
					{
						plproximityMine = hitInfo.collider.GetComponentInParent<PLProximityMine>();
					}
					PLSpaceTarget plspaceTarget = null;
					if (plproximityMine == null)
					{
						plspaceTarget = hitInfo.collider.GetComponentInParent<PLSpaceTarget>();
					}
					if (pldamageableSpaceObject_Collider != null)
					{
						pldamageableSpaceObject_Collider.MyDSO.TakeDamage_Location(this.m_Damage, hitInfo.point, hitInfo.normal);
					}
					else if (plproximityMine != null)
					{
						PLServer.Instance.photonView.RPC("ProximityMineExplode", PhotonTargets.All, new object[]
						{
						plproximityMine.EncounterNetID
						});
					}
					else if (plshipInfoBase != null)
					{
						if (plshipInfoBase != base.ShipStats.Ship)
						{
							base.ShipStats.Ship.StartCoroutine(this.DelayedMegaTurretDamage(plshipInfoBase, flag, hitInfo, this.m_VisibleChargeLevel,this.m_Damage));
							PLServer.Instance.photonView.RPC("MegaTurretExplosion", PhotonTargets.Others, new object[]
							{
							hitInfo.point,
							plshipInfoBase.ShipID,
							this.MegaTurretExplosionID
							});
							PLServer.Instance.MegaTurretExplosion(hitInfo.point, plshipInfoBase.ShipID, this.MegaTurretExplosionID);
						}
					}
					else if (hitInfo.collider != null && hitInfo.collider.CompareTag("Projectile"))
					{
						PLProjectile component = hitInfo.collider.gameObject.GetComponent<PLProjectile>();
						if (component != null && component.LaserCanCauseExplosion && component.OwnerShipID != -1 && component.OwnerShipID != base.ShipStats.Ship.ShipID)
						{
							component.TakeDamage(this.m_Damage, hitInfo.point, hitInfo.normal, playerFromPlayerID);
							flag2 = true;
						}
					}
					else if (plspaceTarget != null)
					{
						plspaceTarget.photonView.RPC("NetTakeDamage", PhotonTargets.All, new object[]
						{
						m_Damage
						});
						PLServer.Instance.photonView.RPC("MegaTurretExplosion", PhotonTargets.Others, new object[]
						{
						hitInfo.point,
						-1,
						this.MegaTurretExplosionID
						});
						PLServer.Instance.MegaTurretExplosion(hitInfo.point, -1, this.MegaTurretExplosionID);
					}
					else
					{
						PLServer.Instance.photonView.RPC("MegaTurretExplosion", PhotonTargets.Others, new object[]
						{
						hitInfo.point,
						-1,
						this.MegaTurretExplosionID
						});
						PLServer.Instance.MegaTurretExplosion(hitInfo.point, -1, this.MegaTurretExplosionID);
					}
					if (!flag2)
					{
						this.LaserDist = (hitInfo.point - this.TurretInstance.FiringLoc.position).magnitude;
					}
					else
					{
						this.LaserDist = 20000f;
					}
				}
				else
				{
					this.LaserDist = 20000f;
				}
				Vector3 vector;
				int num;
				PLSwarmCollider hitSwarmCollider = this.GetHitSwarmCollider(this.LaserDist, out vector, out num);
				if (hitSwarmCollider != null)
				{
					float num2 = UnityEngine.Random.Range(0f, 1f);
					hitSwarmCollider.MyShipInfo.HandleSwarmColliderHitVisuals(hitSwarmCollider, vector);
					float timeDiff = this.GetBaseTimeDiff();
					if (PhotonNetwork.isMasterClient && (flag || base.ShipStats.Ship.IsDrone))
					{
						timeDiff = UnityEngine.Random.Range(0.01f, 0.1f);
					}
					float num3 = (150f + this.m_Damage * base.ShipStats.TurretDamageFactor);
					num3 *= (float)num * 0.25f;
					PulsarModLoader.ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.Auto_Turrets.LaserAutoTurretDamage", PhotonTargets.Others, new object[]
					{
					hitSwarmCollider.MyShipInfo.ShipID,
					num3,
					num2,
					hitSwarmCollider.MyShipInfo.Exterior.transform.InverseTransformPoint(vector),
					base.ShipStats.Ship.ShipID,
					this.TurretID,
					this.m_StoredProjID
					});
					InstabilityTurretDamage.InstabilityTurretDamager(hitSwarmCollider.MyShipInfo.ShipID, num3, num2, hitSwarmCollider.MyShipInfo.Exterior.transform.InverseTransformPoint(vector), base.ShipStats.Ship.ShipID, this.TurretID, this.m_StoredProjID);
				}
			}
			catch
			{
			}
			if (base.ShipStats.Ship.GetExteriorMeshCollider() != null)
			{
				base.ShipStats.Ship.GetExteriorMeshCollider().gameObject.layer = layer;
			}
			if (Time.time - base.ShipStats.Ship.LastCloakingSystemActivatedTime > 2f)
			{
				base.ShipStats.Ship.SetIsCloakingSystemActive(false);
			}
			this.m_IsCharging = false;
			this.m_IsVisiblyCharging = false;
			this.m_ChargeLevel = 0f;
			this.m_VisibleChargeLevel = 0f;

			if (base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) != -1)
			{
				PulsarModLoader.Utilities.Messaging.Notification("Damage value2: " + this.m_Damage, PLNetworkManager.Instance.LocalPlayer, default, 9000, default);
			}
			this.m_Damage = this.baseDamage;
			/*
			if (base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) != -1)
			{
				PulsarModLoader.Utilities.Messaging.Notification("Damage before reset: " + this.m_Damage, PLNetworkManager.Instance.LocalPlayer, default, 20000, default);
				PulsarModLoader.Utilities.Logger.Info("Damage before reset: " + this.m_Damage);
			}
			this.m_Damage = this.baseDamage;
			if (base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) != -1)
			{
				PulsarModLoader.Utilities.Messaging.Notification("Damage after reset: " + this.m_Damage, PLNetworkManager.Instance.LocalPlayer, default, 20000, default);
				PulsarModLoader.Utilities.Logger.Info("Damage after reset: " + this.m_Damage);
			}
			*/

		}

		public float GetBaseTimeDiff()
		{
			float num = 0.97f;
			return Mathf.Clamp01(Mathf.Abs(this.m_VisibleChargeLevel - num));
		}

		public float GetBaseTimeDiff(float visibleChargeLevel)
		{
			float num = 0.97f;
			return Mathf.Clamp01(Mathf.Abs(visibleChargeLevel - num));
		}

		public float GetDmgPercentBasedOnTiming(float timeDiff)
		{
			return 1f - Mathf.Clamp01(Mathf.Abs(Mathf.Pow(timeDiff * 200f, 2f) / 10000f));
		}
		private IEnumerator DelayedMegaTurretDamage(PLShipInfoBase hitShip, bool operatedByBot, RaycastHit hitInfo, float visibleChargeLevel, float damage)
		{
			if (hitShip != null)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				PulsarModLoader.ModMessage.SendRPC("Pokegustavo.ExoticComponents", "Exotic_Components.Auto_Turrets.LaserAutoTurretDamage", PhotonTargets.Others, new object[]
				{
				hitShip.ShipID,
				damage,
				num,
				hitShip.Exterior.transform.InverseTransformPoint(hitInfo.point),
				base.ShipStats.Ship.ShipID,
				this.TurretID,
				this.m_StoredProjID
				});
				InstabilityTurretDamage.InstabilityTurretDamager(hitShip.ShipID, damage, num, hitShip.Exterior.transform.InverseTransformPoint(hitInfo.point), base.ShipStats.Ship.ShipID, this.TurretID, this.m_StoredProjID);
				this.m_StoredProjID++;
			}
			yield break;
		}

		public override bool ApplyLeadingToAutoAimShot()
		{
			return false;
		}
		protected override bool ShouldAIFire(bool operatedByBot, float heatOffset, float heatGeneratedOnFire)
		{
			float num = this.Heat + heatOffset;
			return operatedByBot && num < 0.85f - heatGeneratedOnFire;
		}
		public override void Tick()
		{
			if (base.ShipStats == null || base.ShipStats.isPreview || !this.IsEquipped)
			{
				return;
			}
			if (this.ShipStats.Ship.MyReactor != null)
			{
				this.ChargeAmount = 1f;
				if (!this.m_IsCharging && PLNetworkManager.Instance.LocalPlayerID == base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) && PLInput.Instance.GetButton(PLInputBase.EInputActionName.fire) && !this.IsOverheated)
				{
					this.m_IsCharging = true;
					this.initialInstability = base.ShipStats.Ship.CoreInstability;
				}
				else if ((this.m_IsCharging && PLNetworkManager.Instance.LocalPlayerID == base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) && !PLInput.Instance.GetButton(PLInputBase.EInputActionName.fire)) || (base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) == -1 && this.m_IsCharging))
				{
					this.m_IsCharging = false;
					this.m_IsVisiblyCharging = false;
					this.m_VisibleChargeLevel = 0.1f;
					this.ChargeComplete(this.m_StoredProjID, this.TurretInstance.RefJoint.transform.forward);
				}
				else if (this.m_IsCharging)
				{
					this.ShipStats.Ship.CoreInstability += Time.deltaTime * 0.1f;
				}
			}
            else 
			{
				this.ChargeAmount = 0f;
				this.IsCharging = false;
			}
			base.Tick();
			PLPlayer plplayer = null;
			if (base.SlotType != ESlotType.E_COMP_AUTO_TURRET && base.ShipStats.Ship != null && PLServer.Instance != null)
			{
				plplayer = PLServer.Instance.GetPlayerFromPlayerID(base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID));
			}
			bool flag = false;
			if (base.SlotType != ESlotType.E_COMP_AUTO_TURRET && plplayer != null)
			{
				flag = plplayer.IsBot;
			}
			if (((PhotonNetwork.isMasterClient && (flag || base.ShipStats.Ship.IsDrone)) || base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) == PLNetworkManager.Instance.LocalPlayerID) && PLNetworkManager.Instance.LocalPlayerID != -1)
			{
				float num = this.LastCalculated_ChargeAmountDeltaBeforePower * 3f * (1f / this.turretChargeSpeed_ToVisualChargeSpeed);
				if (this.m_IsCharging)
				{
					this.m_ChargeLevel += Time.deltaTime * num;
				}
				if (this.m_IsVisiblyCharging)
				{
					this.m_VisibleChargeLevel += Time.deltaTime * num;
				}
				if (PLNetworkManager.Instance.LocalPlayerID == base.ShipStats.Ship.GetCurrentTurretControllerPlayerID(this.TurretID) && !PLInput.Instance.GetButton(PLInputBase.EInputActionName.fire))
				{
					this.m_IsVisiblyCharging = false;
				}
			}
			if (this.TurretInstance != null && this.TurretInstance.BeamObject != null)
			{
				if (this.TurretInstance.BeamObjectRenderer == null)
				{
					this.TurretInstance.BeamObjectRenderer = this.TurretInstance.BeamObject.GetComponent<Renderer>();
				}
				if (this.m_IsCharging)
				{
					this.LaserDist = 20000f;
					this.LaserBaseRadius = 0.2f;
					this.TurretInstance.BeamObject.transform.localPosition = new Vector3(0f, 0f, this.LaserDist * 0.5f) * (1f / this.TurretInstance.transform.lossyScale.x);
					Mathf.Abs(Mathf.Sin(Time.time * 50f % 3.1415927f * 2f));
					this.TurretInstance.BeamObject.transform.localScale = Vector3.Lerp(this.TurretInstance.BeamObject.transform.localScale, new Vector3(this.LaserBaseRadius, this.LaserDist * 0.5f, this.LaserBaseRadius) * (1f / this.TurretInstance.transform.lossyScale.x), Mathf.Clamp01(30f * Time.deltaTime));
					this.TurretInstance.BeamObject.transform.localScale = new Vector3(this.TurretInstance.BeamObject.transform.localScale.x, this.LaserDist * 0.5f * (1f / this.TurretInstance.transform.lossyScale.x), this.TurretInstance.BeamObject.transform.localScale.z);
					if (this.TurretInstance.OptionalGameObjects.Length != 0 && this.TurretInstance.OptionalGameObjects[0] != null && !this.TurretInstance.OptionalGameObjects[0].activeSelf)
					{
						this.TurretInstance.OptionalGameObjects[0].SetActive(true);
					}
					if (this.TurretInstance.OptionalGameObjects.Length > 1 && this.TurretInstance.OptionalGameObjects[1] != null && this.TurretInstance.OptionalGameObjects[1].activeSelf)
					{
						this.TurretInstance.OptionalGameObjects[1].SetActive(false);
					}
					this.TurretInstance.BeamObjectRenderer.material.EnableKeyword("_EMISSION");
					this.TurretInstance.BeamObjectRenderer.material.SetColor("_Color", this.BeamColor * 0.1f);
					this.TurretInstance.BeamObjectRenderer.material.SetColor("_EmissionColor", this.BeamColor);
					this.TurretInstance.BeamObjectRenderer.enabled = true;
					return;
				}
				if (Time.time - this.LastFireTime > 0f && Time.time - this.LastFireTime < 0.66f)
				{
					this.LaserBaseRadius = 0.4f;
					this.TurretInstance.BeamObject.transform.localPosition = new Vector3(0f, 0f, this.LaserDist * 0.5f) * (1f / this.TurretInstance.transform.lossyScale.x);
					float num2 = Mathf.Abs(Mathf.Sin(Time.time * 500f % 3.1415927f * 2f));
					this.TurretInstance.BeamObject.transform.localScale = Vector3.Lerp(this.TurretInstance.BeamObject.transform.localScale, new Vector3(this.LaserBaseRadius + 0.05f * num2, this.LaserDist * 0.5f, this.LaserBaseRadius + 0.05f * num2) * (1f / this.TurretInstance.transform.lossyScale.x), Mathf.Clamp01(Time.deltaTime));
					this.TurretInstance.BeamObject.transform.localScale = new Vector3(this.TurretInstance.BeamObject.transform.localScale.x, this.LaserDist * 0.5f * (1f / this.TurretInstance.transform.lossyScale.x), this.TurretInstance.BeamObject.transform.localScale.z);
					if (this.TurretInstance.OptionalGameObjects.Length != 0 && this.TurretInstance.OptionalGameObjects[0] != null && !this.TurretInstance.OptionalGameObjects[0].activeSelf)
					{
						this.TurretInstance.OptionalGameObjects[0].SetActive(false);
					}
					if (this.TurretInstance.OptionalGameObjects.Length > 1 && this.TurretInstance.OptionalGameObjects[1] != null && !this.TurretInstance.OptionalGameObjects[1].activeSelf)
					{
						this.TurretInstance.OptionalGameObjects[1].SetActive(true);
					}
					this.TurretInstance.BeamObjectRenderer.material.EnableKeyword("_EMISSION");
					this.TurretInstance.BeamObjectRenderer.material.SetColor("_Color", this.BeamColor * 14f);
					this.TurretInstance.BeamObjectRenderer.material.SetColor("_EmissionColor", this.BeamColor * 14f);
					this.TurretInstance.BeamObjectRenderer.enabled = true;
					return;
				}
				if (this.TurretInstance.OptionalGameObjects.Length != 0 && this.TurretInstance.OptionalGameObjects[0] != null && this.TurretInstance.OptionalGameObjects[0].activeSelf)
				{
					this.TurretInstance.OptionalGameObjects[0].SetActive(false);
				}
				if (this.TurretInstance.OptionalGameObjects.Length > 1 && this.TurretInstance.OptionalGameObjects[1] != null && this.TurretInstance.OptionalGameObjects[1].activeSelf)
				{
					this.TurretInstance.OptionalGameObjects[1].SetActive(false);
				}
				this.TurretInstance.BeamObjectRenderer.enabled = false;
			}
		}

		public float ChargeLevel
		{
			get
			{
				return this.m_ChargeLevel;
			}
			set
			{
				this.m_ChargeLevel = value;
			}
		}
		public bool IsCharging
		{
			get
			{
				return this.m_IsCharging;
			}
			set
			{
				this.m_IsCharging = value;
			}
		}
		public float VisibleChargeLevel
		{
			get
			{
				return this.m_VisibleChargeLevel;
			}
			set
			{
				this.m_VisibleChargeLevel = value;
			}
		}

		public bool IsVisiblyCharging
		{
			get
			{
				return this.m_IsVisiblyCharging;
			}
			set
			{
				this.m_IsVisiblyCharging = value;
			}
		}

		public float baseDamage;
		private float m_ChargeLevel;
		private float m_VisibleChargeLevel;
		private bool m_IsCharging;
		private bool m_IsVisiblyCharging;
		private float initialInstability = 0;
		private List<int> ProcessedProjs = new List<int>();
		protected float LaserDist = 20000f;
		protected float LaserBaseRadius = 1f;
		private int m_StoredProjID = -1;
		protected Color BeamColor;
		public EDamageType DamageType = EDamageType.E_BEAM;
		private PLBottomRightMenuSubItemTwoLines EBombCooldownSubMenuItem;
		protected int MegaTurretExplosionID;
		private float turretChargeSpeed_ToVisualChargeSpeed = 0.75f;
	}

	[HarmonyLib.HarmonyPatch(typeof(PLUITurretUI), "Update")]
	class CustomTurretUI 
	{
		static void Postfix(PLUITurretUI __instance) 
		{
			PLTurret plturret = null;
			PLShipInfoBase plshipInfoBase = null;
			if (PLNetworkManager.Instance.MyLocalPawn != null)
			{
				plshipInfoBase = PLNetworkManager.Instance.MyLocalPawn.CurrentShip;
			}
			if (plshipInfoBase == null && PLCameraSystem.Instance.GetModeString() == "Turret")
			{
				PLCameraMode_Turret plcameraMode_Turret = PLCameraSystem.Instance.CurrentCameraMode as PLCameraMode_Turret;
				if (plcameraMode_Turret != null)
				{
					plshipInfoBase = plcameraMode_Turret.ShipInfo;
				}
			}
			if (PLNetworkManager.Instance.MyLocalPawn != null && plshipInfoBase != null)
			{
				for (int i = 0; i < plshipInfoBase.GetCurrentTurretControllerMaxTurretIndex(); i++)
				{
					if (plshipInfoBase.GetCurrentTurretControllerPlayerID(i) == PLNetworkManager.Instance.LocalPlayerID)
					{
						plturret = plshipInfoBase.GetTurretAtID(i);
						break;
					}
				}
			}
			if(plturret != null && plturret.ShipStats != null && plturret is InstabilityTurret) 
			{
				if (plturret.ShipStats.Ship.MyReactor != null)
				{
					__instance.LeftUI_Label.text = "Reactor stability: " + Mathf.RoundToInt((1 - plturret.ShipStats.Ship.CoreInstability) * 100f).ToString() + "%";
				}
                else 
				{
					__instance.LeftUI_Label.text = "NO REACTOR INSTALLED!";
				}
			}
		}
	}

	class InstabilityTurretDamage : PulsarModLoader.ModMessage
	{
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
		{
			InstabilityTurretDamager((int)arguments[0], (float)arguments[1], (float)arguments[2], (Vector3)arguments[3], (int)arguments[4], (int)arguments[5], (int)arguments[6]);
		}
		public static void InstabilityTurretDamager(int shipID, float damage, float randomNum, Vector3 localPosition, int attackingShipID, int turretID, int projID)
		{
			PLShipInfoBase shipFromID = PLEncounterManager.Instance.GetShipFromID(shipID);
			PLShipInfoBase shipFromID2 = PLEncounterManager.Instance.GetShipFromID(attackingShipID);
			if (shipFromID != null && shipFromID2 != null)
			{
				InstabilityTurret plmegaTurret = shipFromID2.GetTurretAtID(turretID) as InstabilityTurret;
				if (plmegaTurret != null && plmegaTurret.ShouldProcessProj(projID))
				{
					Vector3 a = shipFromID.Exterior.transform.TransformPoint(localPosition);
					bool bottomHit = false;
					Vector3 normalized = (a - shipFromID.Exterior.transform.position).normalized;
					Vector3 rhs = -shipFromID.Exterior.transform.up;
					if (Vector3.Dot(normalized, rhs) > -0.1f)
					{
						bottomHit = true;
					}
					shipFromID.TakeDamage(damage, bottomHit, plmegaTurret.DamageType, randomNum, -1, shipFromID2, turretID);
				}
			}
		}
	}

	[HarmonyLib.HarmonyPatch(typeof(PLIntrepidInfo), "SetupShipStats")]
	class IntrepidTestingTurret 
	{
		static void Postfix(PLIntrepidInfo __instance, bool startingPlayerShip) 
		{
            if (__instance.ShouldCreateDefaultComponents && startingPlayerShip) 
			{
				PLMegaTurret turret = __instance.MyStats.GetShipComponent<PLMegaTurret>(ESlotType.E_COMP_MAINTURRET, false);
				__instance.MyStats.AllComponents.Remove(turret);
				__instance.MyStats.AddShipComponent(new InstabilityTurret(0), -1, ESlotType.E_COMP_NONE);
			}
		}
	}
	/*
	 * Tool for checking main turret not doing damage
	[HarmonyLib.HarmonyPatch(typeof(PLServer), "MegaTurretDamage")]
	class turretlasertest 
	{
		static void Postfix(int shipID, float damage, float randomNum, Vector3 localPosition, int attackingShipID, int turretID, int projID) 
		{
			PulsarModLoader.Utilities.Messaging.Notification("Target Ship ID: " + shipID);
			PulsarModLoader.Utilities.Messaging.Notification("Damage: " + damage);
			PulsarModLoader.Utilities.Messaging.Notification("Random num: " + randomNum);
			PulsarModLoader.Utilities.Messaging.Notification("local pos: " + localPosition.ToString());
			PulsarModLoader.Utilities.Messaging.Notification("Attacking ship ID: " + attackingShipID);
			PulsarModLoader.Utilities.Messaging.Notification("Attacking turret ID: " + turretID);
			PulsarModLoader.Utilities.Messaging.Notification("Projectile ID: " + projID);
			PLShipInfoBase shipFromID = PLEncounterManager.Instance.GetShipFromID(shipID);
			PLShipInfoBase shipFromID2 = PLEncounterManager.Instance.GetShipFromID(attackingShipID);
			PulsarModLoader.Utilities.Messaging.Notification("Target ship null: " + (shipFromID == null));
			PulsarModLoader.Utilities.Messaging.Notification("Attack ship null: " + (shipFromID2 == null));
            if (shipFromID2 != null) 
			{
				PLMegaTurret plmegaTurret = shipFromID2.GetTurretAtID(turretID) as PLMegaTurret;
				PulsarModLoader.Utilities.Messaging.Notification("Mega Turret null: " + (plmegaTurret == null));
				if(plmegaTurret != null) 
				{
					PulsarModLoader.Utilities.Messaging.Notification("Should process: " + (plmegaTurret.ShouldProcessProj(projID)));
				}
			}
        }
    }
	*/
}
