using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputComponent : MonoBehaviour
{
    [Header("----Movement----")]
    public InputActionReference groundedMove;
    public InputActionReference sprint;
    public InputActionReference jump;
    public InputActionReference crouch;
    public InputActionReference slide;
    public InputActionReference climb;
    public InputActionReference wallRun;
    public InputActionReference wallJump;
    public InputActionReference ledgeGrab;

    [Header("----Combat----")]
    public InputActionReference weapon1;
    public InputActionReference weapon2;
    public InputActionReference weapon3;
    public InputActionReference reload;

    private void OnEnable()
    {
        groundedMove.action.Enable();
        sprint.action.Enable();
        jump.action.Enable();
        crouch.action.Enable();
        slide.action.Enable();
        climb.action.Enable();
        wallRun.action.Enable();
        wallJump.action.Enable();
        ledgeGrab.action.Enable();
        weapon1.action.Enable();
        weapon2.action.Enable();
        weapon3.action.Enable();
        reload.action.Enable();
    }

    private void OnDisable()
    {
        groundedMove.action.Disable();
        sprint.action.Disable();
        jump.action.Disable();
        crouch.action.Disable();
        slide.action.Disable();
        climb.action.Disable();
        wallRun.action.Disable();
        wallJump.action.Disable();
        ledgeGrab.action.Disable();
        weapon1.action.Disable();
        weapon2.action.Disable();
        weapon3.action.Disable();
        reload.action.Disable();
    }

}
