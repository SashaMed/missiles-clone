using UnityEngine;


/// <summary>
/// ����������� �������� ���������� �������, � ������������ �� ���� ���. ����� �����, ������������ ��������� ����� ������ ���������� �������. 
/// ��������� ��������� ��������� � Update() - ��������.
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
    /// �������� ��������� ��������, � �����, � ������, ���� �� �����������(�.�. ���������� null)
    /// </summary>
    public bool Check => checkAndActivate();

    /// <summary>
    /// �������� ��������� �������� �� �������
    /// </summary>
    public virtual void Drop()
    {
        LastTimeActivate = Time.time;
    }

    /// <summary>
    /// ������������ � ��������� ���
    /// </summary>
    public void End(float afterOffset = 0f)
    {
        Drop();
        LastTimeActivate -= Timeout - afterOffset;
    }

    /// <summary>
    /// ��������� � ���� ���� �� ��������
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
    /// ��������� ������ ��������� �������. ���� ������������ �������� True, ������� ������������� ������������.
    /// Null-���������. ���� �������� � ���������� ���, �� ������ �� ����� �����������.
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
    /// ��������� ��������� ���������, ������ ������� ����������� Timeout.
    /// <example>UpdateTimeout refresh = 0.5f;</example>
    /// </summary>
    /// <param name="timeout">�����, ����� ������� ����� ����������� �������</param>
    public static implicit operator UpdateTimeout(float timeout)
    {
        return new UpdateTimeout(timeout);
    }
}
