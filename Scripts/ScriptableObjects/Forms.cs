using Unity.VisualScripting;
using UnityEngine;

public class Forms : ScriptableObject
{
    public new string name;
    public float damage;
    public float range = 3f;
    public DamageType damageType;
    public LayerMask enemyMask;
    public float staminaCost;
    public float activeTime;
    public float cooldownTime;
    public AnimationClip animation;
    public float animationSpeed = 1f;
    public GameObject prefab;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    public virtual void Activate(GameObject parent) { }
    public virtual void BeginCooldown(GameObject parent) { }
}
