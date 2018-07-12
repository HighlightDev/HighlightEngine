using MassiveGame.API.Keyboard;
using MassiveGame.Core.GameCore.Entities.MoveEntities;
using System;
using System.Windows.Forms;

namespace MassiveGame.Core.GameCore
{
    public enum ActionTypeBinding
    {
        MoveForward,
        MoveBack,
        MoveLeft, 
        MoveRight,
        Jump
    }

    public class PlayerController
    {
        private ThirdPersonCamera playerCamera;
        private KeyboardHandler keyboard;

        private Tuple<Action, Keys> PlayerMoveForward = null;
        private Tuple<Action, Keys> PlayerMoveBack = null;
        private Tuple<Action, Keys> PlayerMoveLeft = null;
        private Tuple<Action, Keys> PlayerMoveRight = null;

        public void InvokeBindings()
        {
            if (PlayerMoveForward != null && keyboard.GetKeyState(PlayerMoveForward.Item2))
                PlayerMoveForward.Item1();
            if (PlayerMoveBack != null && keyboard.GetKeyState(PlayerMoveBack.Item2))
                PlayerMoveBack.Item1();
            if (PlayerMoveLeft != null && keyboard.GetKeyState(PlayerMoveLeft.Item2))
                PlayerMoveLeft.Item1();
            if (PlayerMoveRight != null && keyboard.GetKeyState(PlayerMoveRight.Item2))
                PlayerMoveRight.Item1();
        }

        public KeyboardHandler GetKeyboardHandler()
        {
            return keyboard;
        }

        private void Factory_BindActionToKey(Keys key, ActionTypeBinding actionBinding)
        {
            switch (actionBinding)
            {
                case ActionTypeBinding.MoveForward: PlayerMoveForward = new Tuple<Action, Keys>(new Action(playerCamera.GetThirdPersonTarget().MoveActorForward), key); keyboard.AllocateKey(key); break;
                case ActionTypeBinding.MoveBack: break;
                case ActionTypeBinding.MoveLeft: break;
                case ActionTypeBinding.MoveRight: break;
                case ActionTypeBinding.Jump: break;
            }
        }

        public void SetBindingKeyboardKey(Keys key, ActionTypeBinding actionBinding)
        {
            if (playerCamera.GetThirdPersonTarget() != null)
            {
                MovableEntity thirdPersonEntity = playerCamera.GetThirdPersonTarget();
                Factory_BindActionToKey(key, actionBinding);
            }
        }

        public PlayerController(ThirdPersonCamera camera)
        {
            keyboard = new KeyboardHandler();
            playerCamera = camera;
        }
    }
}
