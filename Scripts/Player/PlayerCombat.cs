using UnityEngine;
using System;
using SmallHedge.SoundManager;


public class PlayerCombat : MonoBehaviour, IDamagable
{
    // Start is called before the first frame update
    [Header("Health Settings")]
    public float MaxHealth;
    public float CurrentHealth;
    public float HealthRecoverCooldown;
    public float HealthRecoverSpeed;
    private float _currentHealthRecoverCooldown;

    [Header("Stamina Settings")]
    public float MaxStamina;
    public float CurrentStamina;
    public float StaminaCooldown;
    public float StaminaRecoverSpeed = 30f;
    public float DashStaminaCost = 30f;
    private float _currentStaminaCooldown = 0f;

    [Header("RPG")]
    [Range(0, 50)] public int Level = 0;
    public int Exp = 0;
    [Range(0, 20)] public int Strength = 0;
    [Range(0, 20)] public int Speed = 0;
    [Range(0, 20)] public int Endurance = 0;
    [Range(0, 20)] public int Skill = 0;
    public int SkillPoints = 0;

    public float Damage;
    public Transform AttackPoint;
    public float AttackRange;
    public LayerMask enemyLayers;


    [Header("Impact Prefabs")]
    public GameObject[] HitImpacts;

    public GameObject[] HurtImpacts;

    [Header("Audio Clips")]
    public AudioClip[] swordWooshs;

    [Range(0, 1)] public float SwordWhooshVolume = 0.5f;
    public AudioClip[] HurtClips;
    [Range(0,1)] public float HurtClipsVolume = 0.5f;

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
        Health();
    }

    public void Attack()
    {
        //play sound
        SoundManager.PlaySound(SoundType.SWORDWHOOSH);

        //Get enemies in sphere
        Collider[] hitEnemies = Physics.OverlapSphere(AttackPoint.position, AttackRange, enemyLayers);

        //if Sphere have enemies, attacks them
        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.TryGetComponent(out IDamagable damageable))
                {

                    damageable.ApplyDamage(Damage, DamageType.Physical, this.gameObject);

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
            EventManager.OnPlayerStaminaChanged(CurrentStamina);

            if (CurrentStamina > MaxStamina)
            {
                CurrentStamina = MaxStamina;
                EventManager.OnPlayerStaminaChanged(CurrentStamina);
            }
        }
        else
        {
            _currentStaminaCooldown -= Time.deltaTime;
        }
    }

    private void Health()
    {
        if (CurrentHealth < MaxHealth && _currentHealthRecoverCooldown <= 0f)
        {
            CurrentHealth += HealthRecoverSpeed * Time.deltaTime;
            if (CurrentHealth > MaxHealth) 
            { 
                CurrentHealth = MaxHealth;
            }
            EventManager.OnPlayerHealthChanged(CurrentHealth);
        }
        if (_currentHealthRecoverCooldown > 0f) 
        {
            _currentHealthRecoverCooldown -= Time.deltaTime;
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
                //return;
            }
            _currentStaminaCooldown = StaminaCooldown;

        }
        EventManager.OnPlayerStaminaChanged(CurrentStamina);
    }

    public void ApplyDamage (float damage, DamageType damageType, GameObject damager) 
    {
        //GetComponent<CharacterController>().Move(transform.position - hitDirection);
        CurrentHealth -= damage;
        _currentHealthRecoverCooldown = HealthRecoverCooldown;
        EventManager.OnPlayerHealthChanged(CurrentHealth);

        if(HurtImpacts.Length > 0)
        {
            int number = UnityEngine.Random.Range(0, HurtImpacts.Length);
            Vector3 position = transform.position;
            position.y += 1f;
            Instantiate(HurtImpacts[number], position, transform.rotation);
        }

        SoundManager.PlaySound(SoundType.PLAYERHURT);
        /*if (HurtClips.Length > 0)
        {
            int number = UnityEngine.Random.Range(0, HurtClips.Length);
            AudioSource.PlayClipAtPoint(HurtClips[number], transform.position, HurtClipsVolume);
        }*/

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

    public void AddSkillPoint(int AttributeIndex)
    {
        if (AttributeIndex == 1) {
            if (Strength > 20)
            {
                Debug.Log("Unable to improve that skill");
            }
            else 
            {
                Strength += 1;
            }
        }
    }

    public void RecalculateStats()
    {

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
