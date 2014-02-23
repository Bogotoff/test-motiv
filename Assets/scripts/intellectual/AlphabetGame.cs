using UnityEngine;
using System.Collections;

public delegate void OnDragDropCallback(DragDropContainer container, 
                                        DragDropItem item);

public class AlphabetGame: IntellectualGame
{
    public GameObject dropContainersRoot;

    private bool _completed = false;
    private DragDropContainer[] _dropContainers;
    private int _dropedCount;
    private int _maxDropCount;
    
    public override bool checkEndOfGame()
    {
        return _dropedCount >= _maxDropCount;
    }

    void Start()
    {
        _maxDropCount = 0;
        _dropedCount  = 0;
        _completed    = true;

        if (dropContainersRoot != null) {
            _dropContainers = dropContainersRoot.GetComponentsInChildren<DragDropContainer>();

            for (int i = 0; i < _dropContainers.Length; i++) {
                _dropContainers[i].dragDropCallback = _dragDropEvent;
            }

            if (_dropContainers.Length > 0) {
                _completed    = false;
                _maxDropCount = _dropContainers.Length;
            }
        }
    }

    private void _dragDropEvent(DragDropContainer container, DragDropItem item)
    {
        _dropedCount++;

        if (!_completed && checkEndOfGame()) {
            _completed = true;
            Debug.Log("Game completed");
        }
    }
}
