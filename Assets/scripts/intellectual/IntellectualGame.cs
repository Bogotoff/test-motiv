using UnityEngine;
using System.Collections;

public delegate void OnDragDropCallback(DragDropContainer container, 
                                        DragDropItem item);

public abstract class IntellectualGame : MonoBehaviour
{
    protected bool _completed = false;

    public abstract void startGame();
    public abstract bool checkEndOfGame();

    public virtual void finishGame(bool gameCompleted)
    {
        _completed = true;

        if (gameCompleted) {
            saveGameResults();
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

    public abstract void saveGameResults();
}
