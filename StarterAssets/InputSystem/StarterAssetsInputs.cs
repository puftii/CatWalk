using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool dash;
        public bool attack;
        public bool block;
        public bool tactical1;
        public bool tactical2;
        public bool lockTarget;
        public bool interact;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        public void OnDash(InputValue value)
        {
            DashInput(value.isPressed);
        }
        public void OnAttack(InputValue value)
        {
            AttackInput(value.isPressed);
        }
        public void OnBlock(InputValue value)
        {
            BlockInput(value.isPressed);
        }
        public void OnTactical2(InputValue value)
        {
            Tactical2Input(value.isPressed);
        }
        public void OnLockTarget(InputValue value)
        {
            OnLockTargetInput(value.isPressed);
        }
        public void OnTactical1(InputValue value)
        {
            Tactical1Input(value.isPressed);
        }
        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void DashInput(bool newDashState)
        {
            dash = newDashState;
        }
        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        public void BlockInput(bool newBlockState)
        {
            block = newBlockState;
        }

        public void Tactical2Input(bool newTactical2State)
        {
            tactical2 = newTactical2State;
        }

        public void OnLockTargetInput(bool newLockTargetState)
        {
            lockTarget = newLockTargetState;
        }

        public void Tactical1Input(bool newTactical1State)
        {
            tactical1 = newTactical1State;
        }

        public void InteractInput(bool newInteractState)
        {
            interact = newInteractState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}