using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Monsterfall_01.Engine.Input
{
    // Game action must have this structure to be called-back by the event
    public delegate void GameAction(eButtonState buttonState, Vector2 amount);
    //public delegate void GameAction(eButtonState buttonState, Vector2 amount, KeyboardState prevState);
    // Here we have toolset to bind actual keys to actual jobs (game actions)
    internal class InputCommandManager
    {
        InputListener m_Input;

        // Just to map every key to an action
        Dictionary<Keys, GameAction> m_KeyBindings = new Dictionary<Keys, GameAction>();
        Dictionary<String, GameAction> m_mouseBindings = new Dictionary<String, GameAction>();

        public InputCommandManager()
        {
            m_Input = new InputListener();

            // Register events with the input listener 
            m_Input.OnKeyDown += OnKeyDown;
            m_Input.OnKeyPressed += OnKeyPressed;
            m_Input.OnKeyUp += OnKeyUp;

            m_Input.OnLeftClick += OnLeftClick;
        }

        // Add new keys to listen for
        public void AddKeyboardBinding(Keys key, GameAction action)
        {
            // Add key to listen for when polling 
            m_Input.AddKey(key);

            // Add the binding to the command map 
            m_KeyBindings.Add(key, action);
        }
        // Add new mouse keys to listen for
        public void AddMouseBinding(String key, GameAction action)
        {
            // Add the binding to the command map 
            m_mouseBindings.Add(key, action);
        }

        public void Update()
        {
            m_Input.Update();
        }

        public void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            // Checks if there is an action for that key
            if (action != null)
            {
                // If so, fire the action related to that key
                action(eButtonState.DOWN, new Vector2(1.0f));
            }
        }

        public void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
                action(eButtonState.UP, new Vector2(1.0f));
        }

        public void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
                action(eButtonState.PRESSED, new Vector2(1.0f));

        }

        public void OnLeftClick(object sender, MouseState e)
        {
            GameAction action = m_mouseBindings["Left"];

            if (action != null)
                action(eButtonState.UP, new Vector2(e.X, e.Y));
        }
    }
}
