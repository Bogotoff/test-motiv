using System;
using UnityEngine;

[ExecuteInEditMode]
public class UIStretchMy : MonoBehaviour
{
    public Camera uiCamera = null;
    public Vector2 relativeSize = Vector2.one;
    
    private Transform mTrans;
    private UIRoot mRoot;
    
    void OnEnable ()
    {
        if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
        mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
    }
    
    void Update ()
    {
        if (uiCamera != null)
        {
            if (mTrans == null) mTrans = transform;

            Rect rect = uiCamera.pixelRect;
            float screenWidth  = rect.width;
            float screenHeight = rect.height;

            Vector3 localScale = mTrans.localScale;

            float scale;

            float relativeRatio = relativeSize.x / relativeSize.y;
            float screenRatio = screenWidth / screenHeight;

            if (relativeRatio < screenRatio) {
                scale = screenWidth / relativeSize.x;
            } else {
                scale = screenHeight / relativeSize.y;
            }

            float texWidth  = relativeSize.x * scale;
            float texHeight = relativeSize.y * scale;

            if (mRoot != null && !mRoot.automatic && screenHeight > 1f) {
                scale = mRoot.manualHeight / screenHeight;

                texWidth  *= scale;
                texHeight *= scale;
            }

            localScale.x = texWidth;
            localScale.y = texHeight;

            if (mTrans.localScale != localScale) mTrans.localScale = localScale;
        }
    }
}
