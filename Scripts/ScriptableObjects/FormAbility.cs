using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class FormAbility : Forms
{


    public override void Activate(GameObject parent)
    {
        EventManager.OnPlayerWeaponHolstered(false);
        parent.GetComponent<Animator>().SetTrigger("Slash");
        Time.timeScale = 0.3f;
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, parent.transform.position + positionOffset, parent.transform.rotation * Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z));
            /*obj.transform.position = parent.transform.position + positionOffset;
            obj.transform.rotation = parent.transform.rotation * Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);*/
        }
        Attack(parent);
        Collider[] hits = Physics.OverlapSphere(parent.transform.position, range, enemyMask);
        foreach (Collider c in hits)
        {
            if (c.TryGetComponent(out IDamagable damagable))
            {
                damagable.ApplyDamage(damage, DamageType.Fire, parent);
            }
        }
    }

    public override void BeginCooldown(GameObject parent)
    {
        Time.timeScale = 1f;
    }


    IEnumerator Attack(GameObject parent)
    {
        float pauseTime = activeTime / 3;
        while (activeTime > 0) 
        {
            Collider[] hits = Physics.OverlapSphere(parent.transform.position, range, enemyMask);
            foreach (Collider c in hits)
            {
                if (c.TryGetComponent(out IDamagable damagable))
                {
                    damagable.ApplyDamage(damage, DamageType.Fire, parent);
                }
            }
            activeTime -= activeTime / 3;
            yield return new WaitForSeconds(pauseTime);
        }
        
    }
}
