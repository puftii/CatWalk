using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using TMPro;
using UnityEditor;
using SmallHedge.SoundManager;
using System.Linq;
public class EnemyStats : MonoBehaviour, IDamagable
{
    [Header("Health Settings")]
    [Tooltip("Sets Max Enemy Health")]public float MaxHealth = 100f;
    public float _currentHealth;

    [Header("Combat Settings")]
    public float Damage = 10f;
    public float range = 3f;
    public float AttackDistance = 1f;
    public float AttackRange = 0.5f;
    public float AttackCooldown = 1f;
    public float spawnTimeout = 3f;
    public float dyingTime = 5f;
    public Transform HitPosition;
    public LayerMask playerLayer;
    public LayerMask attackLayers;
    [Header("UI")]
    public TextMeshProUGUI healthText;
    public GameObject healthSlider;
    [Header("FX")]
    public GameObject BloodSample;
    public GameObject AttackParticle;
    public GameObject DieParticle;
    public Renderer Mesh;
    private Material _material;
    [Header("Audio Clips")]

    public AudioClip[] BloodClips;
    public AudioClip[] DyingClips;
    [Range(0, 1)] public float HitAudioVolume = 0.5f;

    private float _currentStun = 0f;
    private float _maxStun;
    private Slider _healthSlider;
    private NavMeshAgent _agent;
    private Animator _animator;
    public float _currentAttackCooldown = 0f;
    private bool dying = false;
    private Rigidbody[] _bodies;
    private Rigidbody _rigidbody;
    private int _totalBodies;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _maxStun = MaxHealth / 2;
        _agent = GetComponent<NavMeshAgent>();
        _material = Mesh.material;
        _healthSlider = healthSlider.GetComponent<Slider>();
        _animator = GetComponent<Animator>();
        _currentHealth = MaxHealth;
        healthText.text = _currentHealth.ToString();
        _healthSlider.maxValue = MaxHealth;
        _healthSlider.value = _currentHealth;
        _bodies = GetComponentsInChildren<Rigidbody>();
        //_bodies = _bodies.Skip(1).ToArray();
        _totalBodies = _bodies.Length;
        foreach (Rigidbody body in _bodies)
        {
            body.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimeout <= 0f && !dying)
        {
            LookForPlayer();
        } else if(dying)
        {
            if(dyingTime <= 0f)
            {
                Destroy(this.gameObject);
            } else
            {
                dyingTime -= Time.deltaTime;
            }
        } else
        {
            spawnTimeout -= Time.deltaTime;
        }
        _animator.SetFloat("Speed", _agent.velocity.magnitude);

        if(_currentStun >= 0f)
        {
            _currentStun -= Time.deltaTime;
        }
    }

    public void ApplyDamage(float damage, DamageType damageType, GameObject damager)
    {
        
        _currentHealth -= damage;
        _currentStun += damage;
        if (_currentStun >= _maxStun)
        {
            GetStunned();
        }

        if (damager != null) 
        {
            StartCoroutine(PushAway(0.3f, (transform.position - damager.transform.position) * 0.7f));
            //transform.position += (transform.position - damager.transform.position) * 0.7f;     
        }
        if (healthText != null)
        {
            healthText.text = _currentHealth.ToString();
        }
        _healthSlider.value = _currentHealth;
        Vector3 position = this.transform.position; //other.transform.position;
        Quaternion rotation = this.transform.rotation;
        Vector3 test = new Vector3(0, Random.Range(0, 360), 0);
        rotation.eulerAngles = test;//other.transform.rotation;
        Transform bodyPart = this.transform;
        if (_bodies.Length > 0)
        {
            bodyPart = _bodies[Random.Range(0, _bodies.Length)].transform;
        }
        if (BloodSample != null)
        {
            Instantiate(BloodSample, bodyPart.position, rotation, bodyPart);
        }
        AudioManager.PlaySoundFromClipArray(BloodClips, transform.position, HitAudioVolume);

        if (_currentHealth <= 0f)
        {
            Die(damager);
            return;
        }
    }

    public void Die(GameObject damager = null)
    {
        //this.GetComponent<Rigidbody>().isKinematic = false;
        Destroy(this.GetComponent<CapsuleCollider>());
        Destroy(healthSlider);
        StartCoroutine(StartDissolve());
        EventManager.OnDemonDied();
        //_material.SetFloat("Dissolve", 0.5f);
        _animator.enabled = !_animator.enabled;
        //_animator.SetTrigger("Die");
        AudioManager.PlaySoundFromClipArray(DyingClips, transform.position, HitAudioVolume);
        foreach (Rigidbody body in _bodies)
        {
            body.isKinematic = false;
            body.linearVelocity = (transform.position - damager.transform.position + Vector3.up * 2f) * 3f;

        }

        //_bodies[0].linearVelocity = (transform.position - damager.transform.position + Vector3.up * 2f) * 5f;
        dying = true;
        Instantiate(DieParticle, _bodies[1].transform);
    }

    public void LookForPlayer()
    {
        if (_currentAttackCooldown >= 0f)
        {
            _currentAttackCooldown -= Time.deltaTime;
            return;
        }
        Collider[] nearEnemies = Physics.OverlapSphere(transform.position, range, playerLayer);
        foreach (Collider enemy in nearEnemies)
        {
            if (enemy.GetComponent<PlayerCombat>() != null)
            {
                float playerDistance = Vector3.Distance(enemy.transform.position, transform.position);
                //если игрок в зоне поиска, но не в зоне атаки - идет к игроку
                if(playerDistance >= AttackDistance)
                {
                    _agent.SetDestination(enemy.transform.position);
                } else if(playerDistance <= AttackDistance && _currentAttackCooldown <= 0f) //если игрок в зоне поражения и кд нет - атакует
                {
                    transform.rotation = Quaternion.LookRotation(enemy.transform.position);
                    transform.LookAt(enemy.transform.position);
                    _animator.SetTrigger("Attack");
                    _currentAttackCooldown = AttackCooldown;
                    _agent.ResetPath();
                }
            }
        }
        
        //Debug.Log(_currentAttackCooldown);
    }


    public void GetStunned()
    {
        _animator.SetTrigger("Stun");
        SoundManager.PlaySound(SoundType.STUN);

    }

    void OnDrawGizmosSelected()
    {
        
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.DrawWireSphere(HitPosition.position, AttackRange);
    }

    public void Attack(float multiplier)
    {
        Collider[] hits = Physics.OverlapSphere(HitPosition.position, AttackRange);
        _currentAttackCooldown = AttackCooldown;
        foreach(Collider hit in hits)
        {
            if(hit.TryGetComponent(out IDamagable damagable) && hit.gameObject != this.gameObject)
            {
                
                damagable.ApplyDamage(Mathf.Round(Damage * multiplier), DamageType.Physical, this.gameObject);
                //GameObject groundSlash = Instantiate(AttackParticle, HitPosition.position, HitPosition.rotation);
            }
        }
        
        _currentAttackCooldown = AttackCooldown;
    }

    IEnumerator PushAway(float time, Vector3 force)
    {
        while(time > 0f) 
        {
            transform.position += force * 0.3f * time;
            yield return new WaitForSeconds(Time.deltaTime);
            time -= Time.deltaTime;
        }
        
    }


    IEnumerator StartDissolve()
    {
        float _dyingTime = dyingTime;
        while (_material.GetFloat("Dissolve") < 1f) {
            _material.SetFloat("Dissolve", _material.GetFloat("Dissolve") + (0.013f / _dyingTime));
            yield return new WaitForSeconds(.01f);
        }
    }
}
