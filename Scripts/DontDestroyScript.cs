using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyScript : MonoBehaviour
{
    public GameObject[] objs;

    void Awake()
    {
        foreach (GameObject obj in objs)
        {
            DontDestroyOnLoad(obj);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}