using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent Boxcollider2D]
public class CustomMovableItem: MonoBehaviour
{
    public bool moveX;
    public bool moveY;
    
    private BoxCollider _collider = null;
    private GameObject _uiRoot    = null;
    private bool _isRotated       = false;
    private Transform _cachedTransform = null;

    void Awake()
    {
        if (moveX && moveY) {
            moveY = false;
        }
    }
    
    void Start()
    {
        _cachedTransform = gameObject.transform;
        _collider = gameObject.GetComponent<BoxCollider>();

        if (collider == null) {
            Debug.LogError("collider = null");
        }

        _uiRoot = GameObject.FindGameObjectWithTag("UIRoot2D");

        if (_uiRoot == null) {
            Debug.LogError("Ui root with tag \"UIRoot2D\" not found");
        }

        _isRotated = (Mathf.Abs(_cachedTransform.localRotation.eulerAngles.z) > 0);
    }
    
    public float getWidth()
    {
        Vector3 scale = _uiRoot.transform.localScale;
        float width;

        if (_isRotated) {
            width = _collider.size.y * _cachedTransform.localScale.y * _uiRoot.transform.localScale.y;
        } else {
            width = _collider.size.x * _cachedTransform.localScale.x * _uiRoot.transform.localScale.x;
        }

        return width;
    }
    
    public float getHeight()
    {
        float height;

        if (_isRotated) {
            height = _collider.size.x * _cachedTransform.localScale.x * _uiRoot.transform.localScale.x;
        } else {
            height = _collider.size.y * _cachedTransform.localScale.y * _uiRoot.transform.localScale.y;
        }

        return height;
    }
}
