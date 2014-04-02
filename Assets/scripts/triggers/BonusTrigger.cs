using UnityEngine;
using System.Collections;

public class BonusTrigger: MonoBehaviour
{
    public int score          = 10;
    public bool destroyOnTake = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("Player") == 0) {
            other.gameObject.GetComponent<Player>().onBonus(score);
        }

        if (destroyOnTake) {
            Destroy(gameObject);
        }
    }
}
