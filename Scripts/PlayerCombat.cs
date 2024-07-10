using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using TMPro;
using System;


public class PlayerCombat : MonoBehaviour
{
    // Start is called before the first frame update
    public float Health;
    public float _currentHealth;
    public float Stamina;
    public float _currentStamina;
    public int Level = 0;
    public int Exp = 0;
    public int Strength = 0;
    public int Speed = 0;
    public int Endurance = 0;
    public int Skill = 0;
    public int SkillPoints = 0;

    public TextMeshProUGUI healthText;
    public float Damage;
    public Transform AttackPoint;
    public float AttackRange;
    public LayerMask enemyLayers;
    public GameObject KatanRig;
    private Rig _katanRig;
    private float katanWeight;
    private bool holster = false;
    void Start()
    {
        LoadPlayer();
        katanWeight = 0f;
        if (KatanRig != null)
        {
            _katanRig = KatanRig.GetComponent<Rig>();
        }
        _currentHealth = Health;
        healthText.text = _currentHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Tactical1"))
        {
            //SavePlayer();
        }
    }

    public void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(AttackPoint.position, AttackRange, enemyLayers);
        if(hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                if(enemy.GetComponent<EnemyStats>() != null)
                {
                    enemy.GetComponent<EnemyStats>().GetHit(Damage);
                }
                
            }
        }
        
    }

    public void GetHit(float damage)
    {
        //GetComponent<CharacterController>().Move(transform.position - hitDirection);
        _currentHealth -= damage;
        healthText.text = _currentHealth.ToString();
    }
    void OnDrawGizmosSelected()
    {
        if(AttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if(data != null)
        {
            _currentHealth = data.currentHealth;
            Level = data.level;
            Exp = data.exp;
            Strength = data.strength;
            Endurance = data.endurance;
            Speed = data.speed;
            Skill = data.skill;
            SkillPoints = data.skillPoints;
            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];
            transform.position = position;
        }
    }
}
