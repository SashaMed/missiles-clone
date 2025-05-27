using UnityEngine;


/// <summary>
/// Отсчитывает заданный промежуток времени, и активируется на один тик. Потом опять, активируется регулярно через каждый промежуток времени. 
/// Требуется проверять состояние в Update() - функциях.
/// </summary>
public class UpdateTimeout
{
    private float _timeout;
    public float Timeout { get { return _timeout; } set { _timeout = value; } }
    public float LastTimeActivate { get; protected set; }

    public UpdateTimeout(float timeout = 1f)
    {
        Timeout = timeout;
    }

    /// <summary>
    /// Проверка состояния счётчика, и откат, в случае, если он срабатывает(т.е. возвращает null)
    /// </summary>
    public bool Check => checkAndActivate();

    /// <summary>
    /// Сбросить состояние счётчика на границу
    /// </summary>
    public virtual void Drop()
    {
        LastTimeActivate = Time.time;
    }

    /// <summary>
    /// Активировать в следующий раз
    /// </summary>
    public void End(float afterOffset = 0f)
    {
        Drop();
        LastTimeActivate -= Timeout - afterOffset;
    }

    /// <summary>
    /// Проверить и если окей то запусить
    /// </summary>
    /// <returns></returns>
    protected virtual bool checkAndActivate()
    {
        if (Time.time - LastTimeActivate >= Timeout)
        {
            activate();
            return true;
        }

        return false;
    }

    protected virtual void activate()
    {
        Drop();
    }

    public void Wait(float timeWaiting)
    {
        LastTimeActivate += timeWaiting;
    }

    /// <summary>
    /// Позволяет просто проверять счётчик. Если возвращенное значение True, счетчик автоматически откатывается.
    /// Null-безопасно. Если счётчика в переменной нет, он просто не будет срабатывать.
    /// </summary>
    public static bool operator true(UpdateTimeout t)
    {
        return t != null && t.Check;
    }

    public static bool operator false(UpdateTimeout t)
    {
        return t == null || !t.Check;
    }



    /// <summary>
    /// Позволяет создавать экземпляр, просто задавая необходимый Timeout.
    /// <example>UpdateTimeout refresh = 0.5f;</example>
    /// </summary>
    /// <param name="timeout">Время, через которое будет срабатывать счётчик</param>
    public static implicit operator UpdateTimeout(float timeout)
    {
        return new UpdateTimeout(timeout);
    }
}
