using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using TMPro;
public class EnemyStats : MonoBehaviour
{

    public float Health = 100f;
    public float _currentHealth;
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
    public TextMeshProUGUI healthText;
    public GameObject healthSlider;
    private Slider _healthSlider;
    public GameObject BloodSample;
    public GameObject AttackParticle;
    private NavMeshAgent _agent;
    private Animator _animator;
    public float _currentAttackCooldown = 0f;
    private bool dying = false;
    private Rigidbody[] _bodies;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _healthSlider = healthSlider.GetComponent<Slider>();
        _animator = GetComponent<Animator>();
        _currentHealth = Health;
        healthText.text = _currentHealth.ToString();
        _healthSlider.maxValue = Health;
        _healthSlider.value = _currentHealth;
        _bodies = GetComponentsInChildren<Rigidbody>();
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
    }

    public void GetHit(float damage)
    {
        _currentHealth -= damage;
        if (healthText != null) 
        {
            healthText.text = _currentHealth.ToString();
        }
        _healthSlider.value = _currentHealth;
        Vector3 position = new Vector3(this.transform.position.x, this.transform.position.y + 1.6f, this.transform.position.z); //other.transform.position;
        Quaternion rotation = this.transform.rotation;
        Vector3 test = new Vector3(Random.Range(0,360), 0, Random.Range(0, 360));
        rotation.eulerAngles = test;//other.transform.rotation;
        GameObject blood = Instantiate(BloodSample, position, rotation);
        Debug.Log("Gets Hit");
        if (_currentHealth <= 0f)
        {
            Die();
            return;
        }
        if (damage > _currentHealth / 10)
        {
            _animator.SetTrigger("Stun");
        }
    }

    public void Die()
    {
        //this.GetComponent<Rigidbody>().isKinematic = false;
        Destroy(this.GetComponent<CapsuleCollider>());
        Destroy(healthSlider);

        _animator.enabled = !_animator.enabled;
        _animator.SetTrigger("Die");
        foreach (Rigidbody body in _bodies)
        {
            body.isKinematic = false;
        }
        dying = true;
    }

    public void LookForPlayer()
    {
        if (_currentAttackCooldown >= 0f)
        {
            _currentAttackCooldown -= Time.deltaTime;
            //return;
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
                    return;
                } else if(playerDistance <= AttackDistance && _currentAttackCooldown <= 0f) //если игрок в зоне поражения и кд нет - атакует
                {
                    _animator.SetTrigger("Attack");
                    _agent.ResetPath();
                    transform.LookAt(enemy.transform.position);
                }
            }
        }
        
        //Debug.Log(_currentAttackCooldown);
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
            if(hit.GetComponent<PlayerCombat>() != null)
            {
                hit.GetComponent<PlayerCombat>().GetHit(Mathf.Round(Damage * multiplier));
            }
        }
        Vector3 slashPosition = new Vector3(HitPosition.position.x, HitPosition.position.y - 1f, HitPosition.position.z + 1f);
        GameObject groundSlash = Instantiate(AttackParticle, slashPosition, HitPosition.rotation);
        _currentAttackCooldown = AttackCooldown;
    }
}
