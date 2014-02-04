using UnityEngine;

[AddComponentMenu("NGUI/Motiv/Drag & Drop Item")]
public class DragDropItem : MonoBehaviour
{
    public int group = 0;
    public GameObject prefab;
    
    private Transform _cachedTransform;
    private bool _IsDragging = false;
    private Transform _lastParent;
    
    void Awake()
    {
        _cachedTransform = transform;
        _lastParent = _cachedTransform.parent;
        
        if (prefab == null) {
            prefab = gameObject;
        }
    }

    void updateTable()
    {
        UITable table = NGUITools.FindInParents<UITable>(_lastParent.gameObject);

        if (table != null) {
            table.repositionNow = true;
        }
    }

    void drop()
    {
        // Is there a droppable container?
        Collider col = UICamera.lastHit.collider;
        DragDropContainer container = (col != null) ? col.gameObject.GetComponent<DragDropContainer>() : null;

        if (container != null && container.putDragDropItem(this)) {
            //updateTable();
        } else {
            _cachedTransform.parent = _lastParent;
        }

        updateTable();

        // Make all widgets update their parents
        BroadcastMessage("CheckParent", SendMessageOptions.DontRequireReceiver);
    }
    
    void OnDrag(Vector2 delta)
    {
        if (UICamera.currentTouchID == -1) {
            if (!_IsDragging) {
                _IsDragging = true;
                _lastParent = _cachedTransform.parent;
                _cachedTransform.parent = DragDropRoot.root;
                
                Vector3 pos = _cachedTransform.localPosition;
                pos.z = 0f;
                _cachedTransform.localPosition = pos;
                
                _cachedTransform.BroadcastMessage("CheckParent", SendMessageOptions.DontRequireReceiver);
            } else {
                _cachedTransform.localPosition += (Vector3)delta;
            }
        }
    }
    
    void OnPress(bool isPressed)
    {
        _IsDragging = false;
        Collider col = collider;
        
        if (col != null) {
            col.enabled = !isPressed;
        }
        
        if (!isPressed) {
            drop();
        }
    }
}