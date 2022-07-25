using UnityEngine;


namespace Big2Game
{
    public class StateMachine : MonoBehaviour
    {
        BaseState currentState;

        void Start()
        {
            currentState = GetInitialState();
            if (currentState != null)
                currentState.Enter();
        }

        void Update()
        {
            if (currentState != null)
                currentState.UpdateLogic();
        }

        void LateUpdate()
        {
            if (currentState != null)
                currentState.UpdatePhysics();
        }

        public void ChangeState(BaseState newState)
        {
            currentState.Exit();

            currentState = newState;
            currentState.Enter();
        }

        protected virtual BaseState GetInitialState()
        {
            return null;
        }
    }
}