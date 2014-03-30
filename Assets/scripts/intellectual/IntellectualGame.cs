using UnityEngine;
using System.Collections;

public delegate void OnDragDropCallback(DragDropContainer container, 
                                        DragDropItem item);

public abstract class IntellectualGame : MonoBehaviour
{
    public int timeLimit = 60;
    public UILabel countdownLabel;

    public Transform objectsRoot = null;
    public GameObject winDialogPrefab   = null;
    public GameObject looseDialogPrefab = null;

    protected bool _completed    = false;
    protected int _countDownTime = 0;
    protected float _startTime   = 0;

    public abstract void startGame();
    public abstract bool checkEndOfGame();
    public abstract void saveGameResults();

    void Start()
    {
        if (timeLimit < 0) {
            timeLimit = 0;
        }

        _completed = false;
        startGame();

        _countDownTime = timeLimit;
        _startTime     = Time.time;
        _updateCountdownText(_countDownTime);
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

        step();
    }

    private IEnumerator _finish(bool gameCompleted)
    {
        GameObject dlg = null;
        
        if (gameCompleted) {
            if (winDialogPrefab != null) {
                dlg = GameObject.Instantiate(winDialogPrefab) as GameObject;
            }
        } else if (looseDialogPrefab != null) {
            dlg = GameObject.Instantiate(looseDialogPrefab) as GameObject;
        }
        
        if (dlg != null) {
            if (objectsRoot != null) {
                dlg.transform.parent = objectsRoot;
            } else {
                dlg.transform.parent = objectsRoot;
            }
            
            dlg.transform.localScale    = Vector3.one;
            dlg.transform.localPosition = new Vector3(0f, 0f, -1f);
        }

        yield return new WaitForSeconds(3);

        if (dlg != null) {
            Destroy(dlg);
        }

        try {
            if (GameData.hasNextGame()) {
                GameInfo info = GameData.getNextGame();
                
                Application.LoadLevel(info.sceneName);
            } else {
                Debug.LogError("Ошибка! После интеллектуальной игры должна быть гонка.");
                Application.LoadLevel("mainMenu");
            }
        } catch (System.Exception e) {
            Debug.LogError(e.Message);
        }
    }

    public virtual void finishGame(bool gameCompleted)
    {
        _completed = true;

        if (gameCompleted) {
            saveGameResults();
        }

        StartCoroutine(_finish(gameCompleted));
    }

    protected void _updateCountdownText(int timeStamp)
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

    public virtual void step()
    {

    }
}
