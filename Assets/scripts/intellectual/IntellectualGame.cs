using UnityEngine;
using System.Collections;

public delegate void OnDragDropCallback(DragDropContainer container, 
                                        DragDropItem item);

abstract public class IntellectualGame : MonoBehaviour
{
    abstract public void startGame();
    abstract public void finishGame(bool gameCompleted);
    abstract public bool checkEndOfGame();
}
