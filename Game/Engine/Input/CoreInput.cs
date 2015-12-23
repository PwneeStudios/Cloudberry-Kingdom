using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CoreEngine
{
    public enum ControllerButtons { A, B, X, Y, RS, LS, RT, LT, RJ, RJButton, LJ, LJButton, DPad, Start, Back, Left, Right, Up, Down, Enter, None, Any };

	public static class CoreKeyboard
	{
		public static PlayerIndex KeyboardPlayerIndex = PlayerIndex.One;
		public static int KeyboardPlayerNumber { get { return (int)KeyboardPlayerIndex; } }
	}

	public static class CoreGamepad
	{
        static GamePadState[] GamepadState, PrevGamepadState;

        public static void Initialize(GameServiceContainer Container, GameComponentCollection ComponentCollection, IntPtr WindowHandle)
		{
		}
		
		public static void OnLoad()
        {
            GamepadState = new GamePadState[4];
            PrevGamepadState = new GamePadState[4];
        }

        public static void Update_EndOfStep()
        {
            // Store the previous states of the Xbox controllers.
            for (int i = 0; i < 4; i++)
                if (PrevGamepadState[i] != null)
                    PrevGamepadState[i] = GamepadState[i];
        }

        public static void Update()
        {
            GamepadState[0] = GamePad.GetState(PlayerIndex.One);
            GamepadState[1] = GamePad.GetState(PlayerIndex.Two);
            GamepadState[2] = GamePad.GetState(PlayerIndex.Three);
            GamepadState[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public static void Clear()
        {
            GamepadState[0] = new GamePadState();
            GamepadState[1] = new GamePadState();
            GamepadState[2] = new GamePadState();
            GamepadState[3] = new GamePadState();
        }

        public static bool IsConnected(PlayerIndex Index) { return IsConnected((int)Index); }
        public static bool IsConnected(int PlayerNumber)
        {
            return GamepadState[PlayerNumber].IsConnected;
        }

        public static bool IsPressed(PlayerIndex Index, ControllerButtons Button) { return IsPressed((int)Index, Button, false); }
        public static bool IsPreviousPressed(PlayerIndex Index, ControllerButtons Button) { return IsPressed((int)Index, Button, true); }
        public static bool IsPressed(PlayerIndex Index, ControllerButtons Button, bool Previous) { return IsPressed((int)Index, Button, Previous); }
        public static bool IsPressed(int PlayerNumber, ControllerButtons Button) { return IsPressed(PlayerNumber, Button, false); }
        public static bool IsPreviousPressed(int PlayerNumber, ControllerButtons Button) { return IsPressed(PlayerNumber, Button, true); }
        public static bool IsPressed(int PlayerNumber, ControllerButtons Button, bool Previous)
        {
            GamePadState Pad;

            if (Previous)
                Pad = PrevGamepadState[PlayerNumber];
            else
                Pad = GamepadState[PlayerNumber];

            switch (Button)
            {
                case ControllerButtons.Start: return (Pad.Buttons.Start == ButtonState.Pressed); break;
                case ControllerButtons.Back: return (Pad.Buttons.Back == ButtonState.Pressed); break;

                case ControllerButtons.A: return (Pad.Buttons.A == ButtonState.Pressed); break;
                case ControllerButtons.B: return (Pad.Buttons.B == ButtonState.Pressed); break;
                case ControllerButtons.X: return (Pad.Buttons.X == ButtonState.Pressed); break;
                case ControllerButtons.Y: return (Pad.Buttons.Y == ButtonState.Pressed); break;

                case ControllerButtons.LJButton: return (Pad.Buttons.LeftStick == ButtonState.Pressed); break;
                case ControllerButtons.RJButton: return (Pad.Buttons.RightStick == ButtonState.Pressed); break;

                case ControllerButtons.LS: return (Pad.Buttons.LeftShoulder == ButtonState.Pressed); break;
                case ControllerButtons.RS: return (Pad.Buttons.RightShoulder == ButtonState.Pressed); break;

                case ControllerButtons.LT: return (Pad.Triggers.Left > .5f);
                case ControllerButtons.RT: return (Pad.Triggers.Right > .5f);

                case ControllerButtons.LJ: return (Pad.Buttons.LeftStick == ButtonState.Pressed);
                case ControllerButtons.RJ: return (Pad.Buttons.RightStick == ButtonState.Pressed);

                case ControllerButtons.DPad: return false;

                default: return false;
            }
        }

        public static Vector2 LeftJoystick(PlayerIndex Index) { return LeftJoystick((int)Index); }
        public static Vector2 LeftJoystick(int PlayerNumber)
        {
            Vector2 vec = GamepadState[PlayerNumber].ThumbSticks.Left;
            return new Vector2(vec.X, vec.Y);
        }

        public static Vector2 RightJoystick(PlayerIndex Index) { return RightJoystick((int)Index); }
        public static Vector2 RightJoystick(int PlayerNumber)
        {
            Vector2 vec = GamepadState[PlayerNumber].ThumbSticks.Right;
            return new Vector2(vec.X, vec.Y);
        }

        public static Vector2 DPad(PlayerIndex Index) { return DPad((int)Index); }
        public static Vector2 DPad(int PlayerNumber)
        {
            Vector2 Dir = Vector2.Zero;

            var Pad = GamepadState[PlayerNumber].DPad;

            if (Pad.Right == ButtonState.Pressed) Dir.X = 1;
            if (Pad.Up == ButtonState.Pressed) Dir.Y = 1;
            if (Pad.Left == ButtonState.Pressed) Dir.X = -1;
            if (Pad.Down == ButtonState.Pressed) Dir.Y = -1;

            return Dir;
        }

        public static float LeftTrigger(PlayerIndex Index) { return LeftTrigger((int)Index); }
        public static float LeftTrigger(int PlayerNumber) { return GamepadState[PlayerNumber].Triggers.Left; }

        public static float RightTrigger(PlayerIndex Index) { return RightTrigger((int)Index); }
        public static float RightTrigger(int PlayerNumber) { return GamepadState[PlayerNumber].Triggers.Right; }
    }
}