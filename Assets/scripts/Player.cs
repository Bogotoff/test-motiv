using UnityEngine;
using System.Collections;

public class Player: MonoBehaviour
{
    private bool _isFailTrigger;
    void Start()
    {
        _isFailTrigger = false;
    }
    
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        if (other.gameObject.GetComponent<FailTrigger>() != null) {
            _isFailTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: " + other.gameObject.name);
        if (other.gameObject.GetComponent<FailTrigger>() != null) {
            _isFailTrigger = false;
        }
    }

    public void onFail()
    {
        Debug.Log("FAIL!");
    }

    public void onBonus()
    {
        if (_isFailTrigger) {
            return;
        }

        Debug.Log("Bonus!");
    }

    public void onFinish()
    {
        Debug.Log("Finish!");
        GetComponent<PlayerController>().forceStop();
    }
}
