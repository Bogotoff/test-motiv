using UnityEngine;
using System.Collections;

public class GroundTrigger: MonoBehaviour
{
    public bool isGrounded = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("Ground") == 0) {
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.CompareTo("Ground") == 0) {
            isGrounded = false;
        }
    }
}
