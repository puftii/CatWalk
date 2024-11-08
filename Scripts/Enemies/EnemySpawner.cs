using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public bool activated = false;
    public GameObject EnemyPrefab;
    public LayerMask GroundLayers;
    public Transform[] enemiesPositions;
    public Vector2 XRange = new Vector2(200, 800);
    public Vector2 YRange = new Vector2(200, 800);
    public int MaxEnemies = 100;
    public float SpawnCooldown = 1f;
    private GameObject[] _enemies;
    private Vector3 _position;
    private int _totalEnemies;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(SpawnEnemies(MaxEnemies));
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_position, Vector3.down * 500);
    }

    IEnumerator SpawnEnemies(int count)
    {
        
        while (true) 
        {
            RaycastHit hit;
            _position = new Vector3(Random.Range(XRange.x, XRange.y), 300, Random.Range(YRange.x, YRange.y));
            if(Physics.Raycast(_position, Vector3.down * 500, out hit, GroundLayers))
            {
                Debug.Log("Hitted Ground");
                Instantiate(EnemyPrefab, hit.transform.position, new Quaternion(0, Random.Range(0, 359), 0, 0));
                count--;
            }
            yield return new WaitForSeconds(SpawnCooldown);
        }
    }

    public void SpawnEnemiesByPosition(Transform[] enemies)
    {
        foreach (var enemy in enemies)
        {
            Instantiate(EnemyPrefab, enemy.transform.position, enemy.transform.rotation);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && !activated && EnemyPrefab != null)
        {
            SpawnEnemiesByPosition(enemiesPositions);
            activated = true;
        }
        
        
    }
}
