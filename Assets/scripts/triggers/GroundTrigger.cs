using UnityEngine;
using System.Collections;

public class GroundTrigger: MonoBehaviour
{
    public bool isGrounded {
        get {
            return (_raycastGrounded || _triggerGrounded);
        }
    }
    public float distance = 1.2f;
    private int _layerMask;
    private Transform _cachedTransform;
    private bool _raycastGrounded = false;
    private bool _triggerGrounded = false;

    void Start()
    {
        _cachedTransform = transform;
        _layerMask = LayerMask.NameToLayer("Ground");
    }

    void Update()
    {
        _raycastGrounded = Physics.Raycast(_cachedTransform.position, Vector3.down, distance, _layerMask);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Ground")) {
            _triggerGrounded = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("Ground")) {
            _triggerGrounded = false;
        }
    }
}
