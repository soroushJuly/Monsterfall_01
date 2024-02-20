﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace Monsterfall_01.State
{
    // We can have different state classes but no Pure state
    public abstract class State
    {
        public abstract void Enter(object owner);
        public abstract void Execute(object owner, GameTime gameTime);
        public abstract void Exit(object owner);
        // Name of the state to be set in the derived class and to make users able to search by the name
        public string Name
        {
            get;
            set;
        }

        private List<Transition> m_transitions = new List<Transition>();
        public List<Transition> GetTransitions() {  return m_transitions; }
        public void AddTransition(Transition transition) {  m_transitions.Add(transition); }
    }
}
