using System;
using UnityEngine;
using System.Collections;

public class Game: MonoBehaviour
{
    public GameObject player;
    public UILabel userNameLabel;
    public UILabel timeLabel;
    public UILabel scoreLabel;
    public UILabel countdownLabel;

    private float _time = 0;
    private int _score  = 0;

    private bool _finished = false;
    private bool _started  = false;

    /**
     * 
     */
    void Awake()
    {
        if (userNameLabel == null || timeLabel == null || scoreLabel == null || countdownLabel == null) {
            Debug.LogError("userNameLabel == null || timeLabel == null || pointsLabel == null");
            return;
        }

        Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("music_volume", 0.8f);
    }

    void Start()
    {
        userNameLabel.text = GameData.getUser().surname + (GameData.getUser().id > 0 ? " " : "")
                           + GameData.getUser().name + " " 
                           + GameData.getUser().famillar;

        _updateScoreText(0);
        _updateTimeText(0);

        player.GetComponent<PlayerController>().enabled = false;
        StartCoroutine(_start());
    }

    void Update()
    {
        if (_started && !_finished) {
            _time += Time.deltaTime;
            _updateTimeText(Mathf.FloorToInt(_time));
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel("mainMenu");
        }
    }

    public void onBonus(int bonusScore)
    {
        _score += bonusScore;
        
        if (_score < 0) {
            _score = 0;
        }
        
        _updateScoreText(_score);
    }

    public void onFail()
    {
        //
    }
    
    public void onFinish()
    {
        Debug.Log("Finish!");
        _finished = true;
        player.GetComponent<PlayerController>().forceStop();

        //TODO расчет очков 

        try {
            GameData.saveCurrentGameResult(Mathf.FloorToInt(_time), _score);
            
            if (GameData.hasNextIntellectualGame()) {
                GameInfo info = GameData.getNextIntellectualGame();
                Application.LoadLevelAdditive(info.sceneName);
            } else {
                // окно результатов игры
                Application.LoadLevelAdditive("gameResults");
            }
        } catch (System.Exception e) {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerator _start()
    {
        countdownLabel.text = "3";
        yield return new WaitForSeconds(1);
        countdownLabel.text = "2";
        yield return new WaitForSeconds(1);
        countdownLabel.text = "1";
        yield return new WaitForSeconds(1);
        countdownLabel.text = "";

        _started = true;
        player.GetComponent<PlayerController>().enabled = true;
    }

    private void _updateTimeText(int timeStamp)
    {
        if (timeStamp > 3600 || timeStamp < 0) {
            timeStamp = 3600;
        }
        
        int m = Mathf.FloorToInt(timeStamp / 60);
        int s = timeStamp % 60;

        timeLabel.text = "" + ((m < 10) ? "0" : "") + m.ToString() + ":" 
                       + ((s < 10) ? "0" : "") + s.ToString();
    }

    private void _updateScoreText(int score)
    {
        scoreLabel.text = score.ToString();
    }
}
