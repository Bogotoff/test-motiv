using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Виды игры.
 */
public enum GameType
{
    DRIVE = 0,   // Катание на сноуборде.
    INTELLECTUAL // Интеллектуальная игра.
}

public class GameInfoList: List<List<GameInfo>>
{
}

/**
 * Статический класс для хранение данных игры между сценами.
 */
public static class GameData
{
    /** Количество уровней за игру. */
    public const int MAX_LEVEL_IN_SERIES = 4;

    /** Параметры и результаты уровней. */
    private static GameInfoList _gameInfo = null;

    /** Возраст игрока, по которому были загружены уровни игры. */
    private static User _user = null;

    /** Номер текущей игры. */
    private static int _currentDriveGame;

    /** Номер текщего интеллектуального задания. */
    private static int _currentIntellectualGame;

    /** Флаг загрузки параметров игры. */
    private static bool _isInitialized = false;

    /**
     * Инициализация и загрузка данных игры для выбранного профиля.
     * 
     * @param user данные игрока
     * 
     * @throw Exception, DatabaseException
     */
    public static void initialize(User user)
    {
        if (user == null) {
            throw new System.Exception("Не заданы параметры игрока");
        }

        if (user.age <= 0) {
            throw new System.Exception("Возраст игрока не может быть <= 0");
        }

        Debug.Log("gameId: " + user.gameId);

        _isInitialized = false;

        _currentDriveGame        = -1;
        _currentIntellectualGame = -1;

        if (_gameInfo == null) {
            _gameInfo = new GameInfoList();
        }

        _gameInfo.Clear();

        QueryResult res = DataBase.getInstance().query(
            "SELECT id, sceneId, type, maxTime, maxScore " +
            "FROM mv_levels " +
            "WHERE (" + user.age.ToString() + " >= minAge) AND " +
            "      (" + user.age.ToString() + " <= maxAge) "
        );

        if (res.numRows() <= 0) {
            throw new DatabaseException("Levels data load error.");
        }

        List<GameInfo> driveInfo     = new List<GameInfo>();
        List<GameInfo> intellectInfo = new List<GameInfo>();

        int i;

        for (i = 0; i < res.numRows(); i++) {
            if (res[i].asInt("type") <= 0) {
                driveInfo.Add(new GameInfo(
                    res[i].asInt("id"),
                    res[i].asString("sceneId"),
                    res[i].asInt("maxTime"),
                    res[i].asInt("maxScore")
                ));
            } else {
                intellectInfo.Add(new GameInfo(
                    res[i].asInt("id"),
                    "i" + res[i].asString("sceneId"),
                    res[i].asInt("maxTime"),
                    res[i].asInt("maxScore")
                ));
            }
        }

        // Выбор случайных уровней из загруженных
        _gameInfo.Add(new List<GameInfo>());
        _gameInfo.Add(new List<GameInfo>());

        // Катание на доске.
        i = 0;

        while (i++ < MAX_LEVEL_IN_SERIES) {
            if (driveInfo.Count <= 0) {
                break;
            }

            int r = UnityEngine.Random.Range(0, driveInfo.Count);
            _gameInfo[0].Add(driveInfo[r]);
            driveInfo.RemoveAt(r);
        }

        // Интеллектуальные игры.
        i = 0;
        int intellectualGameCount = _gameInfo[0].Count - 1;

        while (i++ < intellectualGameCount) {
            if (intellectInfo.Count <= 0) {
                break;
            }
            
            int r = UnityEngine.Random.Range(0, intellectInfo.Count);
            _gameInfo[1].Add(intellectInfo[r]);
            intellectInfo.RemoveAt(r);
        }

        driveInfo.Clear();
        driveInfo = null;

        intellectInfo.Clear();
        intellectInfo = null;

        if (_gameInfo[1].Count != _gameInfo[0].Count - 1) {
            _gameInfo[0].RemoveRange(0, _gameInfo[0].Count - _gameInfo[1].Count - 1);

            if (_gameInfo[0].Count <= 0) {
                _gameInfo.Clear();
                throw new System.Exception("Wrong level count for games");
            }
        }

        _user          = user;
        _isInitialized = true;
    }

    /**
     * Перезапускает игру для того же возраста игрок, который играл в прошлый раз.
     * 
     * @throw Exception, DatabaseException
     */
    public static void restartGame()
    {
        if (!_isInitialized || _user == null) {
            throw new System.Exception(
                "Невозможно перезапустить игру. Попробуйте сначала запустить игру.");
        }

        initialize(_user);

        if (hasNextGame()) {
            GameInfo info = getNextGame();
            Application.LoadLevel(info.sceneName);
        }
    }

    /**
     * Возвращает параметры и результаты игры.
     * 
     * @return bool Возвращает true, если игра есть, иначе false
     * @throw Exception
     */
    public static GameInfoList getInfo()
    {
        if (!_isInitialized) {
            throw new System.Exception("GameData not initialized. First call: GameData.initialize()");
        }

        return _gameInfo;
    }

    /**
     * Возвращает объект с данными игрока.
     * 
     * @return User Возвращает объект с данными игрока
     * @throw Exception
     */
    public static User getUser()
    {
        if (!_isInitialized) {
            throw new System.Exception("GameData not initialized. First call: GameData.initialize()");
        }
        
        return _user;
    }

    /**
     * Возвращает параметры и результаты текущей игры.
     * 
     * @return bool Возвращает true, если игра есть, иначе false
     */
    public static GameInfo getCurrentGame()
    {
        if (!_isInitialized || _currentDriveGame < 0 || 
            _currentDriveGame >= _gameInfo[0].Count
        ) {
            return null;
        }

        return _gameInfo[0][_currentDriveGame];
    }

    /**
     * Возвращает параметры и результаты текущей интеллектуальной игры.
     * 
     * @return bool Возвращает true, если игра есть, иначе false
     */
    public static GameInfo getCurrentIntellectualGame()
    {
        if (!_isInitialized || _currentIntellectualGame < 0 
            || _currentIntellectualGame >= _gameInfo[1].Count
        ) {
            return null;
        }
        
        return _gameInfo[1][_currentIntellectualGame];
    }

    /**
     * Возвращает флаг существования следующей игры.
     * 
     * @return bool Возвращает true, если игра есть, иначе false
     */
    public static bool hasNextGame()
    {
        if (!_isInitialized) {
            return false;
        }

        return (_currentDriveGame < _gameInfo[0].Count - 1);
    }

    /**
     * Возвращает флаг существования следующей интеллектуальной игры.
     * 
     * @return bool Возвращает true, если игра есть, иначе false
     */
    public static bool hasNextIntellectualGame()
    {
        if (!_isInitialized) {
            return false;
        }
        
        return (_currentIntellectualGame < _gameInfo[1].Count - 1);
    }

    /**
     * Возвращает параметры следующей игры.
     * 
     * @return GameInfo
     * @throw Exception, IndexOutOfRangeException
     */
    public static GameInfo getNextGame()
    {
        if (!_isInitialized) {
            throw new System.Exception("GameData not initialized. First call: GameData.initialize()");
        }

        if (_currentDriveGame >= _gameInfo[0].Count - 1) {
            throw new System.IndexOutOfRangeException("Index of range for _currentDriveGame");
        }

        _currentDriveGame++;

        return _gameInfo[0][_currentDriveGame];
    }

    /**
     * Возвращает параметры следующей интеллектуальной игры.
     * 
     * @return GameInfo
     * @throw Exception, IndexOutOfRangeException
     */
    public static GameInfo getNextIntellectualGame()
    {
        if (!_isInitialized) {
            throw new System.Exception("GameData not initialized. First call: GameData.initialize()");
        }
        
        if (_currentIntellectualGame >= _gameInfo[1].Count - 1) {
            throw new System.IndexOutOfRangeException("Index of range for _currentIntellectualGame");
        }
        
        _currentIntellectualGame++;
        
        return _gameInfo[1][_currentIntellectualGame];
    }

    /**
     * Сохранение результатов игры.
     * 
     * @param time  время прохождения уровня
     * @param score очки за уровень
     */
    public static void saveCurrentGameResult(int time, int score)
    {
        GameInfo info = getCurrentGame();

        if (info != null) {
            info.totalTime  = time;
            info.totalScore = score;
        }
    }

    /**
     * Сохранение результатов игры.
     * 
     * @param time  время прохождения уровня
     * @param score очки за уровень
     */
    public static void saveCurrentIntellectualResult(int time, int score)
    {
        GameInfo info = getCurrentIntellectualGame();
        
        if (info != null) {
            info.totalTime  = time;
            info.totalScore = score;
        }
    }
}
