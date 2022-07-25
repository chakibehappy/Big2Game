using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Big2Game
{
    public class GameEndState : BaseState
    {
        private readonly GameSM gsm;
        public GameEndState(GameSM stateMachine) : base("Game End", stateMachine)
        {
            gsm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            gsm.HideAllCards();

            gsm.GM.UI.ShowCoinResult(false);
            gsm.GM.UI.ShowTagline(false);
            gsm.GM.UI.SetCharacterSprite(gsm.charSpriteObj);

            stateMachine.ChangeState(gsm.start);
        }

    }

}