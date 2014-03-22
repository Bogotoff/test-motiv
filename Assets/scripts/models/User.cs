
/**
 * Хранит данные пользователя.
 */
public class User
{
    /** Идентификатор игрока в БД. */
    public int id;

    /** Возраст игрока. */
    public int age;

    /** Имя игрока. */
    public string name;

    /** Фамилия игрока. */
    public string surname;

    /** Отчество игрока. */
    public string famillar;

    /** Номер игры по счету. */
    public int gameId;

    /**
     * Конструктор.
     * 
     * @param id       идентификатор игрока в БД
     * @param age      возраст игрока
     * @param name     имя игрока
     * @param surname  фамилия игрока
     * @param famillar отчество игрока
     * @param gameId   номер игры по счету
     */
    public User(int id, int age, string name, string surname, string famillar, int gameId)
    {
        this.id       = id;
        this.age      = age;
        this.name     = name;
        this.surname  = surname;
        this.famillar = famillar;
        this.gameId   = gameId;
    }
}
