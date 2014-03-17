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

        GameInfoList infoList = null;

        try {
            infoList = GameData.getInfo();
        } catch(System.Exception e) {
            return;
        }

        if (infoList[0].Count <= 0) {
            return;
        }

        int i;
        UILabel label;
        int sumEvaluation = 0;
        int totalMaxScore = 0;
        int totalScore    = 0;

        for (i = 0; i < infoList[0].Count; i++) {
            // время прохождения уровня
            label = GameObject.Find("item" + (i + 1) + "_2").GetComponent<UILabel>();

            if (label != null) {
                label.text = _getFormattedTime(infoList[0][i].totalTime);
            }

            // очки за уровень
            label = GameObject.Find("item" + (i + 1) + "_3").GetComponent<UILabel>();
            
            if (label != null) {
                label.text = infoList[0][i].totalScore.ToString();
            }

            totalMaxScore += infoList[0][i].maxScore;
            totalScore    += infoList[0][i].totalScore;
            sumEvaluation += _getEvaluationFromScoreRatio(infoList[0][i].totalScore, 
                                                          infoList[0][i].maxScore);

            // очки за интеллектуальное задание
            if (i < infoList[1].Count) {
                label = GameObject.Find("item" + (i + 1) + "_4").GetComponent<UILabel>();
                
                if (label != null) {
                    label.text = infoList[1][i].totalScore.ToString();
                    totalScore += infoList[0][i].totalScore;
                }

                totalMaxScore += infoList[1][i].maxScore;
                totalScore    += infoList[1][i].totalScore;
                sumEvaluation += _getEvaluationFromScoreRatio(infoList[1][i].totalScore, 
                                                              infoList[1][i].maxScore);
            }
        }

        // итоговое количество очков
        label = GameObject.Find("totalScore").GetComponent<UILabel>();
        
        if (label != null) {
            label.text = totalScore.ToString() + " / " + totalMaxScore.ToString();
        }

        // итоговая оценка
        label = GameObject.Find("totalEvaluation").GetComponent<UILabel>();
        
        if (label != null) {
            sumEvaluation = (int)Math.Round((sumEvaluation + 0.000001) / (infoList[0].Count + infoList[1].Count));
            label.text    = sumEvaluation.ToString();
        }
    }

    /**
     * Вычисляет оценку уровня исходя из отношения полученных очков к максимальному.
     * 
     * @param int totalScore набранные очки за уровень
     * @param int maxScore   максимальное количество очков за уровень
     * 
     * @param int Возвращает оценку за прохождение уровня
     */
    private int _getEvaluationFromScoreRatio(int totalScore, int maxScore)
    {
        if (maxScore <= 0) {
            return 5;
        }

        float f = totalScore / maxScore;
        int v;

        if (f >= 0.85) {
            v = 5;
        } else if (f >= 0.65) {
            v = 4;
        } else if (f >= 45) {
            v = 3;
        } else if (f >= 20) {
            v = 2;
        } else {
            v = 1;
        }

        return v;
    }

    /**
     * Преобразовывает время в секундах к формату "мм:cc".
     * 
     * @param timeInSeconds время в секундах
     * @return string Вреобразованных формат времени
     */
    private string _getFormattedTime(int timeInSeconds)
    {
        if (timeInSeconds > 3600) {
            return "59:59";
        }

        if (timeInSeconds <= 0) {
            return "00:00";
        }
        
        int m = Mathf.FloorToInt(timeInSeconds / 60);
        int s = timeInSeconds % 60;
        
        return "" + ((m < 10) ? "0" : "") + m.ToString() + ":" 
                  + ((s < 10) ? "0" : "") + s.ToString();
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
