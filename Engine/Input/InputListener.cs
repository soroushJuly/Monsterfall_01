using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Monsterfall_01.Engine.Input
{
    // We check for any update in inputs here (polling)
    // and then sending events accordingly
    // This listens to Inputs BUT actualy fires events
    internal class InputListener
    {
        public event EventHandler<KeyboardEventArgs> OnKeyDown;
        public event EventHandler<KeyboardEventArgs> OnKeyUp;
        public event EventHandler<KeyboardEventArgs> OnKeyPressed;

        public event EventHandler<MouseState> OnLeftClick;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        MouseState currentMouseState;
        MouseState previousMouseState;

        public HashSet<Keys> keyList;

        public InputListener()
        {
            keyList = new HashSet<Keys>();

            currentKeyboardState = Keyboard.GetState();
            previousKeyboardState = currentKeyboardState;

            currentMouseState = Mouse.GetState();
            previousMouseState = currentMouseState;
        }

        // Add new key to the list of keys we check in each polling
        public void AddKey(Keys key)
        {
            keyList.Add(key);
        }

        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            FireKeyboardEvents();
            FireMouseEvents();
        }
        void FireMouseEvents()
        {

            // Left Click Pressed
            if (currentMouseState.LeftButton != ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
                OnLeftClick(this, previousMouseState);
        }
        public void FireKeyboardEvents()
        {
            // based on the state of the key fire proper event
            foreach (var key in keyList)
            {
                if (currentKeyboardState.IsKeyDown(key))
                {
                    if (OnKeyDown != null)
                        OnKeyDown(this, new KeyboardEventArgs(key, currentKeyboardState, previousKeyboardState));
                }

                // Key released
                if (previousKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key))
                {
                    if (OnKeyUp != null)
                        OnKeyUp(this, new KeyboardEventArgs(key, currentKeyboardState, previousKeyboardState));
                }

                // Key pressed (was up and is down now)
                if (previousKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key))
                {
                    if (OnKeyPressed != null)
                        OnKeyPressed(this, new KeyboardEventArgs(key, currentKeyboardState, previousKeyboardState));
                }
            }
        }
    }
}
