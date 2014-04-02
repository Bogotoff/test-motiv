using UnityEngine;
using System.Collections;

public class FailTrigger: MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("Player") == 0) {
            other.gameObject.GetComponent<Player>().onFail();
        }
    }
}
