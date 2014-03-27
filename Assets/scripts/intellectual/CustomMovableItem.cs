using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent Boxcollider2D]
public class CustomMovableItem: MonoBehaviour
{
    public bool moveX;
    public bool moveY;
    
    private float _width  = 0f;
    private float _height = 0f;
    
    void Awake()
    {
        if (moveX && moveY) {
            moveY = false;
        }
    }
    
    void Start()
    {
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();

        if (collider != null) {
            GameObject uiRoot = GameObject.FindGameObjectWithTag("UIRoot2D");

            if (uiRoot != null) {
                Vector3 scale = uiRoot.transform.localScale;

                if (Mathf.Abs(gameObject.transform.localRotation.eulerAngles.z) > 0) {
                    _height = collider.size.x * transform.localScale.x * scale.x;
                    _width  = collider.size.y * transform.localScale.y * scale.y;
                } else {
                    _width  = collider.size.x * transform.localScale.x * scale.x;
                    _height = collider.size.y * transform.localScale.y * scale.y;
                }
            } else {
                Debug.LogError("uiRoot = null");
            }
        } else {
            Debug.LogError("collider = null");
        }
    }
    
    public float getWidth()
    {
        return _width;
    }
    
    public float getHeight()
    {
        return _height;
    }
}
