using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HolsterPlayerWeapon : MonoBehaviour
{
    public Rig Rig;
    private Animator m_Animator;

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.PlayerWeaponHolstered += HolsterWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HolsterWeapon(bool holster)
    {
        if (holster)
        {
            Rig.weight = 1.0f;
        }
        else
        {
            Rig.weight = 0f;
        }
    }
}
