using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlphabetGame: IntellectualGame
{
    public GameObject initDropContainersRoot;
    public GameObject targetDropContainersRoot;
    public UILabel countdownLabel;

    private int _dropedCount;
    private int _maxDropCount;
    private int _timeLimit;
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

        _countDownTime = _timeLimit - Mathf.FloorToInt(Time.time - _startTime);

        _updateCountdownText(_countDownTime);

        if (_countDownTime <= 0) {
            finishGame(false);
        }
    }

    public override void startGame()
    {
        _timeLimit     = 30;
        _maxDropCount  = 0;
        _dropedCount   = 0;
        _countDownTime = _timeLimit;
        _completed     = true;
        
        _initializeDropContainers();
        _initializeDragItems();
        
        _updateCountdownText(_timeLimit);
        
        _startTime = Time.time;
    }

    public override bool checkEndOfGame()
    {
        return _dropedCount >= _maxDropCount;
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
                
                _completed    = false;
                _maxDropCount = items.Length;
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
        Vector2 sectorSize   = new Vector2(390, 215);
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

        string word = _generateRandomWord(items.Length);

        if (word.Length != items.Length) {
            Debug.LogError("Не удалось сгенерировать слово из необходимого количества букв");
        }

        for (i = 0; i < items.Length; i++) {
            if (positions.Count > 0) {
                int index = Random.Range(0, positions.Count);

                DragDropItem dragItem = items[i].gameObject.GetComponentInChildren<DragDropItem>();

                if (dragItem != null) {
                    UILabel label = dragItem.GetComponentInChildren<UILabel>();

                    if (label != null) {
                        label.text = "" + word[dragItem.group - 1];
                    }
                }

                items[i].gameObject.transform.parent.transform.localPosition = positions[index];
                positions.RemoveAt(index);
            }
        }
    }

    private string _generateRandomWord(int size)
    {
        string words = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЭЮЯ";

        if (size <= 0) {
            return "";
        }

        if (size >= 30) {
            return words;
        }

        List<string> groups = new List<string>();

        int chunk = (int)(words.Length / size);
        int other = words.Length % size;
        int i;

        for (i = 0; i < size; i++) {
            string s;

            if (i == size - 1) {
                s = words.Substring(i * chunk, chunk + other);
            } else {
                s = words.Substring(i * chunk, chunk);
            }

            groups.Add(s);
        }

        string result = "";

        for (i = 0; i < groups.Count; i++) {
            int r = Random.Range(0, groups[i].Length);
            result += groups[i][r];
        }

        return result;
    }

    private void _dragDropEvent(DragDropContainer container, DragDropItem item)
    {
        _dropedCount++;

        if (!_completed && checkEndOfGame()) {
            finishGame(true);
        }
    }
}
