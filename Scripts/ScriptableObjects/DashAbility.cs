using StarterAssets;
using UnityEngine;


[CreateAssetMenu]
public class DashAbility : Ability
{

    public override void Activate(GameObject parent)
    {
        Animator controller = parent.GetComponent<Animator>();
        controller.SetTrigger("Dash");
    }

    public override void BeginCooldown(GameObject parent)
    {
       
    }
}
