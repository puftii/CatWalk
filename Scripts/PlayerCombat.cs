using UnityEngine;
using System;


public class PlayerCombat : MonoBehaviour, IDamagable
{
    // Start is called before the first frame update
    [Header("Health Settings")]
    public float MaxHealth;
    public float CurrentHealth;

    [Header("Stamina Settings")]
    public float MaxStamina;
    public float CurrentStamina;
    public float StaminaCooldown;
    public float StaminaRecoverSpeed = 30f;
    public float DashStaminaCost = 30f;
    private float _currentStaminaCooldown = 0f;

    [Header("RPG")]
    public int Level = 0;
    public int Exp = 0;
    public int Strength = 0;
    public int Speed = 0;
    public int Endurance = 0;
    public int Skill = 0;
    public int SkillPoints = 0;

    public float Damage;
    public Transform AttackPoint;
    public float AttackRange;
    public LayerMask enemyLayers;


    [Header("Impact Prefabs")]
    public GameObject[] HitImpacts;

    [Header("Audio Clips")]
    public AudioClip[] swordWooshs;

    void Start()
    {
        //LoadPlayer();
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Tactical1"))
        {
            //SavePlayer();
        }
        Stamina();

    }

    public void Attack()
    {
        //play sound
        if (swordWooshs.Length > 0)
        {
            int number = UnityEngine.Random.Range(0, swordWooshs.Length);
            AudioSource.PlayClipAtPoint(swordWooshs[number], transform.position);
        }

        //Get enemies in sphere
        Collider[] hitEnemies = Physics.OverlapSphere(AttackPoint.position, AttackRange, enemyLayers);

        //if Sphere have enemies, attacks them
        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.TryGetComponent(out IDamagable damageable))
                {

                    damageable.ApplyDamage(Damage);
                    if (HitImpacts.Length > 0)
                    {
                        int number = UnityEngine.Random.Range(0, HitImpacts.Length);
                        Instantiate(HitImpacts[number], AttackPoint.transform.position, AttackPoint.transform.rotation);
                    }
                }

            }
        }

    }

    private void Stamina()
    {
        if (CurrentStamina < MaxStamina && _currentStaminaCooldown <= 0f)
        {
            CurrentStamina += Time.deltaTime * StaminaRecoverSpeed;
            if (CurrentStamina > MaxStamina)
            {
                CurrentStamina = MaxStamina;
            }
        }
        else
        {
            _currentStaminaCooldown -= Time.deltaTime;
        }
    }

    public void ChangeStamina(float value)
    {
        CurrentStamina += value;

        if (value < 0)
        {
            if (CurrentStamina <= 0f)
            {
                CurrentStamina = 0f;
                return;
            }
            _currentStaminaCooldown = StaminaCooldown;

        }
    }

    public void ApplyDamage (float damage) 
    {
        //GetComponent<CharacterController>().Move(transform.position - hitDirection);
        CurrentHealth -= damage;

        EventManager.OnPlayerHealthChanged(CurrentHealth);
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            EventManager.OnPlayerHealthChanged(CurrentHealth);
            Die();
        }
    }

    private void Die()
    {
        EventManager.OnPlayerDied();
    }
    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
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
        if (data != null)
        {
            CurrentHealth = data.currentHealth;
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
