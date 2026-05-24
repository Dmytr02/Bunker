using System;
using UnityEngine;

namespace StateMachine
{
    [Serializable]
    public class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
    {
        [SerializeField] private StateStack<T> _stack;
        public State<T> CurrentState { get; private set; }
        private State<T> _previousState;
        
        public void Begin(State<T> state)
        {
            _stack = new StateStack<T>();
            _stack.Push(state);
            CurrentState = state;
            CurrentState.Enter();
        }

        public void SetState(State<T> state) 
        {
            if (CurrentState != null) 
                CurrentState.Exit();

            CurrentState = state;
            _stack.Push(state);
            CurrentState.Enter();
        }

        public void Dispose()
        {
            if(_stack.Count() == 0)
                return;
            
            CurrentState.Exit();
            CurrentState = null;
            _stack.Pop();

            if (_stack.Count() == 0)
                return;
            
            CurrentState = _stack.Peek();
            CurrentState.Enter();
        }
        
        private void Update()
        {
            if(CurrentState == null)
                return;
            
            CurrentState.Update();
        }
    }
}