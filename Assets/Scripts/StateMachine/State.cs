using System;
using UnityEngine;

namespace StateMachine
{
    [Serializable]
    public  class State<T> where T : StateMachine<T>
    {
        protected T stateMachine;

        public State(T stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() {}
        public virtual void Update() {}
        public virtual void Exit() {}
    }
}