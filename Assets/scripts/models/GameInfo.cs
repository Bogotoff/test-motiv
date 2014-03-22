
/**
 * Хранит параметры игры и результаты прохождения.
 */
public class GameInfo
{
    /** Идентификатор уровня в БД. */
    public int id;
    
    /** Название сцены. */
    public string sceneName;
    
    /** Максимальное время прохождения уровня. */
    public int maxTime;
    
    /** Максимально возможное количество очков на уровне. */
    public int maxScore;
    
    /** Время прохождения уровня. */
    public int totalTime;
    
    /** Общее количество очков за уровень. */
    public int totalScore;
    
    /**
     * Конструктор.
     * 
     * @param id        идентификатор уровня в БД
     * @param sceneName название сцены
     * @param maxTime   макс. время прохождения уровня
     * @param maxScore  макс. возможное количество очков на уровне
     */
    public GameInfo(int id, string sceneName, int maxTime, int maxScore)
    {
        this.id         = id;
        this.sceneName  = sceneName;
        this.maxTime    = maxTime;
        this.maxScore   = maxScore;
        this.totalTime  = 0;
        this.totalScore = 0;
    }
}
