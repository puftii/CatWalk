using UnityEngine;

interface IDamagable
{
  void ApplyDamage(float damage, DamageType damageType = DamageType.Physical, GameObject damager = null);
}
