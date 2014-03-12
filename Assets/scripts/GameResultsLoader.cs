using UnityEngine;
using System;

/**
 * Загрузчик данных результатов прохождения серии игр.
 */
public class GameResultsLoader: MonoBehaviour
{
    void Start()
    {
        GameObject.Find("mainMenuButton").GetComponent<ButtonEvents>().addClickListener(onMainMenuButtonClick);
        GameObject.Find("restartGameButton").GetComponent<ButtonEvents>().addClickListener(onRestartGameButtonClick);
    }

    /**
     * Событие по нажатию на кнопку перехода в гланое меню.
     * 
     * @param target объект иницировавший событие
     */
    private void onMainMenuButtonClick(GameObject target)
    {
        Application.LoadLevel("mainMenu");
    }

    /**
     * Событие по нажатию на кнопку перезапуска серии игр.
     * 
     * @param target объект иницировавший событие
     */
    private void onRestartGameButtonClick(GameObject target)
    {
        try {
            GameData.restartGame();
        } catch(System.Exception e) {
            Debug.LogError(e.Message);
        }
    }
}
