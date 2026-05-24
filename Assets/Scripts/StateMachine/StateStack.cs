using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [Serializable]
    public class StateStack<T> where T : StateMachine<T>
    {
        [SerializeField] private List<State<T>> _stack = new();

        public void Push(State<T> state) => _stack.Add(state);

        public State<T> Pop()
        {
            State<T> lastState = Peek();
            _stack.RemoveAt(_stack.Count - 1);
            return lastState;
        }
        
        public State<T> Peek()
        {
            if (_stack.Count == 0)
                return null;
            
            return _stack[^1];
        }

        public int Count() => _stack.Count;
        public List<State<T>> GetStack() => _stack;
    }
}