using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupDifferentGame: IntellectualGame
{
    public int group1Count = 3;
    public int group2Count = 3;
    public int timeLimit   = 120;
    
    public GameObject initDropContainersRoot;
    public GameObject targetDropContainersRoot;

    public UILabel countdownLabel;
    public UILabel group1CountLabel;
    public UILabel group2CountLabel;
    
    private int _droppedCount1;
    private int _droppedCount2;
    private int _countDownTime = 0;
    private float _startTime = 0;
    
    void Start()
    {
        startGame();
    }
    
    void Update()
    {
        if (_completed) {
            return;
        }
        
        _countDownTime = timeLimit - Mathf.FloorToInt(Time.time - _startTime);
        
        _updateCountdownText(_countDownTime);
        
        if (_countDownTime <= 0) {
            finishGame(false);
        }
    }
    
    public override void startGame()
    {
        if (group1Count < 0) {
            group1Count = 0;
        }
        
        if (group2Count < 0) {
            group2Count = 0;
        }
        
        if (timeLimit < 0) {
            timeLimit = 0;
        }
        
        _droppedCount1 = 0;
        _droppedCount2 = 0;
        _countDownTime = timeLimit;
        _completed     = true;
        
        _initializeDropContainers();
        _initializeDragItems();
        
        _updateCountdownText(timeLimit);
        _startTime = Time.time;
    }

    public override bool checkEndOfGame()
    {
        return (_droppedCount1 >= group1Count && _droppedCount2 >= group2Count);
    }

    public override void saveGameResults()
    {
        if (_countDownTime < 0) {
            _countDownTime = 0;
        }
        
        int totalScore = _countDownTime * 3;
        
        GameData.saveCurrentIntellectualResult(_countDownTime, totalScore);
    }
    
    private void _updateCountdownText(int timeStamp)
    {
        if (countdownLabel == null) {
            return;
        }
        
        if (timeStamp > 3600 || timeStamp < 0) {
            countdownLabel.text = "--:--";
            return;
        }
        
        int m = Mathf.FloorToInt(timeStamp / 60);
        int s = timeStamp % 60;
        
        countdownLabel.text = "" + ((m < 10) ? "0" : "") + m.ToString() + ":" 
            + ((s < 10) ? "0" : "") + s.ToString();
    }
    
    private void _initializeDropContainers()
    {
        if (targetDropContainersRoot != null) {
            DragDropContainer[] items = targetDropContainersRoot.GetComponentsInChildren<DragDropContainer>();
            
            if (items.Length > 0) {
                for (int i = 0; i < items.Length; i++) {
                    items[i].dragDropCallback = _dragDropEvent;
                }
                
                _completed = false;
            }
        }
    }
    
    private void _initializeDragItems()
    {
        DragDropContainer[] items = null;
        
        if (initDropContainersRoot != null) {
            items = initDropContainersRoot.GetComponentsInChildren<DragDropContainer>();
        }
        
        if (items.Length <= 0) {
            return;
        }
        
        Vector2 halfItemSize = new Vector2(65, 65);
        Vector2 leftTopPos   = new Vector2(-650, 180);
        Vector2 sectorSize   = new Vector2(390, 160);
        Vector2 offset       = new Vector2(10, 10);
        
        int i, j;
        int colCount = 3;
        int rowCount = 3;
        
        List<Vector3> positions = new List<Vector3>();
        
        for (i = 0; i < rowCount; i++) {
            for (j = 0; j < colCount; j++) {
                Rect r = new Rect(leftTopPos.x + i * sectorSize.x + offset.x, 
                                  leftTopPos.y - j * sectorSize.y - offset.y, 
                                  sectorSize.x, 
                                  sectorSize.y);
                
                positions.Add(new Vector3(Random.Range(r.x + halfItemSize.x, r.x + r.width - halfItemSize.x), 
                                          Random.Range(r.y - halfItemSize.y, r.y - r.height + halfItemSize.y),
                                          0));
            }
        }
        
        for (i = 0; i < items.Length; i++) {
            if (positions.Count > 0) {
                int index = Random.Range(0, positions.Count);
                
                items[i].gameObject.transform.parent.transform.localPosition = positions[index];
                positions.RemoveAt(index);
            }
        }
    }
    
    private void _dragDropEvent(DragDropContainer container, DragDropItem item)
    {
        if (container.group == 1) {
            _droppedCount1++;

            if (group1CountLabel != null) {
                group1CountLabel.text = _droppedCount1.ToString();
            }
        } else {
            _droppedCount2++;

            if (group2CountLabel != null) {
                group2CountLabel.text = _droppedCount2.ToString();
            }
        }

        if (item != null) {
            Destroy(item.gameObject);
        }
        
        if (!_completed && checkEndOfGame()) {
            finishGame(true);
        }
    }
}
