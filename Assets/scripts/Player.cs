using UnityEngine;
using System.Collections;

public class Player: MonoBehaviour
{
    public Game game;
    private bool _isFailTrigger;

    void Start()
    {
        _isFailTrigger = false;

        if (game == null) {
            Debug.LogError("game == null");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<FailTrigger>() != null) {
            _isFailTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<FailTrigger>() != null) {
            _isFailTrigger = false;
        }
    }

    public void onFail()
    {
        game.onFail();
    }

    public void onBonus(int bonusScore)
    {
        if (_isFailTrigger) {
            return;
        }

        game.onBonus(bonusScore);
    }

    public void onFinish()
    {
        game.onFinish();
    }
}
