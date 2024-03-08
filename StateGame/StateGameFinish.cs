using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monsterfall_01.Engine.UI;
using Monsterfall_01.Engine.StateManager;
using Monsterfall_01.Input;
using System;
using System.Collections.Generic;

namespace Monsterfall_01.StateGame
{
    internal class StateGameFinish : State
    {
        Game Game;
        private SpriteBatch _spriteBatch;
        private ContentManager Content;

        // Input manager
        InputCommandManager inputCommandManager;

        private Texture2D diedBackground;
        private Texture2D successBackground;
        private List<Button> buttonList;
        private Texture2D panel;
        private int panelWidth;
        private int panelHeight;

        int currentButtonIndex;

        public event EventHandler GameStart;
        public StateGameFinish(Game game)
        {
            Name = "Finish";
            Game = game;
            Content = game.Content;
        }
        public override void Enter(object owner)
        {
            buttonList = new List<Button>();
            successBackground = new Texture2D(Game.GraphicsDevice, 1, 1);
            successBackground.SetData<Color>(new Color[] { Color.Green });
            //successBackground = Content.Load<Texture2D>();
        }
        public override void Exit(object owner)
        {
            Content.Unload();
        }
        public override void Execute(object owner, GameTime gameTime)
        {

        }

        #region INPUT HANDLERS
        private void OnExit(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
                Game.Exit();
        }

        private void OnKeyDown(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED) ;
                //UpdateCurrentButton(+1);
        }

        private void OnKeyUp(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED) ;
                //UpdateCurrentButton(-1);
        }
        private void OnSelect(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED) ;
                //HandleButtonSelection();
        }
        #endregion
    }
}
