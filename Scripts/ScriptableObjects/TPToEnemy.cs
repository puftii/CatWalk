using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem.XR;


[CreateAssetMenu]
public class TPToEnemy : Ability
{
    public override void Activate(GameObject parent)
    {
        ThirdPersonController _controller = parent.GetComponent<ThirdPersonController>();
        _controller.TPToNearestEnemy();
        //parent.transform.position = Vector3.zero;

    }

    public override void BeginCooldown(GameObject parent)
    {
        
    }
}
