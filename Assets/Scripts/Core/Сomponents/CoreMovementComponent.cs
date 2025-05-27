using UnityEngine;

public class CoreMovementComponent : CoreComponent
{
    [Header("Just in case")]
    [SerializeField] float forwardSpeed = 70f;    
    [SerializeField] float turnSpeed = 150;

    Vector2 moveInput;
    Vector2 turnInput;

    private Transform movable;

    private float _forwardSpeed;
    private float _turnSpeed;

    private float ForwardSpeed => (_forwardSpeed>0) ? _forwardSpeed : forwardSpeed;
    private float TurnSpeed => (_turnSpeed > 0) ? _turnSpeed : turnSpeed;

    public void Init(Transform newMovable, float forwardSpeed, float turnSpeed)
    {
        SetMovable(newMovable);
        _forwardSpeed = forwardSpeed;
        _turnSpeed = turnSpeed;
    }

    public void SetMovable(Transform newMovable)
    {
        movable = newMovable;
    }



    public void SetMovementInput(Vector2 input)
    {
        moveInput = input; 
    }

    public void SetTurnInput(Vector2 input)
    {
        turnInput = input;
    }


    override public void LogicUpdate()
    {
        if (movable == null)
        {
            return;
        }

        Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            movable.rotation = Quaternion.RotateTowards(
                movable.rotation,
                targetRot,
                TurnSpeed * Time.deltaTime
            );
        }
        else if (Mathf.Abs(turnInput.x) > 0.01f)
        {
            movable.Rotate(0, turnInput.x * TurnSpeed * Time.deltaTime,0);
        }

        movable.position += movable.forward * ForwardSpeed * Time.deltaTime;
    }

}