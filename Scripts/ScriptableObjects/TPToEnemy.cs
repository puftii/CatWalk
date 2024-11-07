using StarterAssets;
using UnityEngine;


[CreateAssetMenu]
public class TPToEnemy : Forms
{
    public override void Activate(GameObject parent)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(parent.transform.position, range, enemyMask);

        if (hitEnemies.Length > 0)
        {
            parent.GetComponent<ThirdPersonController>().TPToNearestEnemy(hitEnemies[0].gameObject);
        }
    }

    public override void BeginCooldown(GameObject parent)
    {
        
    }
}
