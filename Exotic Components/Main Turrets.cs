﻿using PulsarModLoader.Content.Components.MegaTurret;
using UnityEngine;
using System.Collections;
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
			this.Name = "Machine Gun Turret";
			this.Desc = "This old turret is designed for extreme firerate but with extremely low damage, time to melt some enemies. This version was upgraded for more damage and less heat generated";
			this.m_Damage = 75f;
			this.MinFireDelay = 0.009f;
			this.FireDelay = 0.009f;
			this.HeatGeneratedOnFire = 0.0024f;
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
	class SilentDeath : PLMegaTurret_RapidFire
	{
		public SilentDeath(int inLevel = 0, int inSubTypeData = 1) : base(0)
		{
			this.Name = "Silent Death";
			this.Desc = "A special Sylvassi turret made to fire without breaking the cloak of a ship. Due to the possible mallicious uses, it has being marked as a contraband";
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
}
