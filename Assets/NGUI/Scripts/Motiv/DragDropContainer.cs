using UnityEngine;

[AddComponentMenu("NGUI/Motiv/Drop Container")]
public class DragDropContainer : MonoBehaviour
{
    public int group = 0;
    public int maxItemCount = 0;
    public OnDragDropCallback dragDropCallback = null;

    private Transform _cachedTransform;
    private UITable _table;

    void Awake()
    {
        _cachedTransform = transform;
        _table = gameObject.GetComponent<UITable>();
    }

    public bool putDragDropItem(DragDropItem item)
    {
        if (group > 0 && group != item.group) {
            return false;
        }

        if (maxItemCount > 0 && gameObject.GetComponentsInChildren<DragDropItem>().Length >= maxItemCount) {
            return false;
        }

        item.transform.parent        = _cachedTransform;
        item.transform.localPosition = Vector3.zero;

        if (_table != null) {
            _table.repositionNow = true;
        }

        if (dragDropCallback != null) {
            dragDropCallback(this, item);
        }

        return true;
    }


}