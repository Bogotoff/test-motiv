using UnityEngine;
using System.Collections;

abstract public class IntellectualGame : MonoBehaviour
{
    abstract public void startGame();
    abstract public void finishGame(bool gameCompleted);
    abstract public bool checkEndOfGame();
}
