using UnityEngine;
using System.Collections;

public class Player: MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
    }

    public void onFail()
    {
        Debug.Log("FAIL!");
    }

    public void onBonus()
    {
        Debug.Log("Bonus!");
    }
}
