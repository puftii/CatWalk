using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem.XR;


[CreateAssetMenu]
public class TPToEnemy : Ability
{
    public override void Activate(GameObject parent)
    {
        Collider[] enemies = Physics.OverlapSphere(parent.transform.position, 15f, enemyLayers);

        if (enemies.Length == 0 ) 
        {
            Transform enemy = enemies[0].transform;
            GameObject poof = Instantiate(AbilityPrefab, parent.transform.position, Quaternion.Euler(Vector3.zero));
            parent.transform.position = enemy.position + (enemy.position - parent.transform.position) / Vector3.Distance(enemy.position, parent.transform.position);
            parent.transform.position = new Vector3(parent.transform.position.x, enemy.position.y, parent.transform.position.z);
            parent.transform.LookAt(enemy.position);
            parent.transform.rotation = Quaternion.Euler(0, parent.transform.rotation.y, 0);
        }
        
        //parent.transform.position = Vector3.zero;

    }

    public override void BeginCooldown(GameObject parent)
    {
        
    }
}
