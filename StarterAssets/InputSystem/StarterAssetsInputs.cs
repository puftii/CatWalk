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
		public bool attack;
		public bool ragdoll;
		public bool tactical1;
		public float cameraZoom;

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
			if(cursorInputForLook)
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
		public void OnAttack(InputValue value)
		{
			AttackInput(value.isPressed);
		}
		public void OnRagdoll(InputValue value)
		{
			RagdollInput(value.isPressed);
		}
		public void OnCameraZoom(InputValue value)
		{
			CameraZoomInput(value.Get<float>());
		}
		public void OnTactical1(InputValue value)
		{
			Tactical1Input(value.isPressed);
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

        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        public void RagdollInput(bool newRagdollState)
        {
            ragdoll = newRagdollState;
        }

        public void CameraZoomInput(float newCameraZoomState)
        {
            cameraZoom = newCameraZoomState;
        }

        public void Tactical1Input(bool newTactical1State)
        {
            tactical1 = newTactical1State;
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