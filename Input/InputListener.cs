using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Monsterfall_01.Input
{
    // We check for any update in inputs here (polling)
    // and then sending events accordingly
    internal class InputListener
    {
        public event EventHandler<KeyboardEventArgs> OnKeyDown = delegate { };
        public event EventHandler<KeyboardEventArgs> OnKeyUp = delegate { };
        public event EventHandler<KeyboardEventArgs> OnKeyPressed = delegate { };

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        public HashSet<Keys> keyList;

        public InputListener()
        {
            keyList = new HashSet<Keys>();

            currentKeyboardState = Keyboard.GetState();
            previousKeyboardState = currentKeyboardState;
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

            FireKeyboardEvents();
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
