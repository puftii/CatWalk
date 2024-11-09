using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public new string name;
    public float cooldownTime;
    public float activeTime;
    public float staminaCost;
    public LayerMask enemyLayers;
    public GameObject AbilityPrefab;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;


    public virtual void Activate(GameObject parent) {}
    public virtual void BeginCooldown(GameObject parent) { }
}
