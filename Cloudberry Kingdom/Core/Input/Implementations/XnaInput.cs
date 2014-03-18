using System;

using CloudberryKingdom;

using Microsoft.Xna.Framework;
using input = Microsoft.Xna.Framework.Input;

namespace CoreEngine
{
	public class XnaInput : GamepadBase
	{
        protected input.GamePadState[] GamepadState, PrevGamepadState;

		public override void Initialize(GameServiceContainer Container, GameComponentCollection ComponentCollection, IntPtr WindowHandle)
		{
			// No initialization required for base XNA input.
		}

		public override void OnLoad()
		{
            GamepadState = new input.GamePadState[4];
            PrevGamepadState = new input.GamePadState[4];
		}

		public override void Update_EndOfStep()
		{
			// Store the previous states of the Xbox controllers.
			for (int i = 0; i < 4; i++)
                if (PrevGamepadState[i] != null)
					PrevGamepadState[i] = GamepadState[i];
		}

		public override void Update()
		{
            GamepadState[0] = input.GamePad.GetState(PlayerIndex.One);
            GamepadState[1] = input.GamePad.GetState(PlayerIndex.Two);
            GamepadState[2] = input.GamePad.GetState(PlayerIndex.Three);
            GamepadState[3] = input.GamePad.GetState(PlayerIndex.Four);
		}

		public override void Clear()
		{
			GamepadState[0] = new input.GamePadState();
            GamepadState[1] = new input.GamePadState();
            GamepadState[2] = new input.GamePadState();
            GamepadState[3] = new input.GamePadState();
		}

		public override bool IsConnected(int PlayerNumber)
		{
			return GamepadState[PlayerNumber].IsConnected;
		}

		public override bool IsPressed(int PlayerNumber, ControllerButtons Button, bool Previous)
		{
            input.GamePadState Pad;
			
			if (Previous)
				Pad = PrevGamepadState[PlayerNumber];
			else
				Pad = GamepadState[PlayerNumber];

			switch (Button)
			{
				case ControllerButtons.Start:		return (Pad.Buttons.Start == input.ButtonState.Pressed); break;
				case ControllerButtons.Back:		return (Pad.Buttons.Back == input.ButtonState.Pressed); break;
				
				case ControllerButtons.A:			return (Pad.Buttons.A == input.ButtonState.Pressed); break;
				case ControllerButtons.B:			return (Pad.Buttons.B == input.ButtonState.Pressed); break;
				case ControllerButtons.X:			return (Pad.Buttons.X == input.ButtonState.Pressed); break;
				case ControllerButtons.Y:			return (Pad.Buttons.Y == input.ButtonState.Pressed); break;
				
				case ControllerButtons.LJButton:	return (Pad.Buttons.LeftStick == input.ButtonState.Pressed); break;
				case ControllerButtons.RJButton:	return (Pad.Buttons.RightStick == input.ButtonState.Pressed); break;
				
				case ControllerButtons.LS:			return (Pad.Buttons.LeftShoulder == input.ButtonState.Pressed); break;
				case ControllerButtons.RS:			return (Pad.Buttons.RightShoulder == input.ButtonState.Pressed); break;
				
				case ControllerButtons.LT:			return (Pad.Triggers.Left > .5f);
				case ControllerButtons.RT:			return (Pad.Triggers.Right > .5f);
				
				case ControllerButtons.LJ:			return (Pad.Buttons.LeftStick == input.ButtonState.Pressed);
				case ControllerButtons.RJ:			return (Pad.Buttons.RightStick == input.ButtonState.Pressed);
				
				case ControllerButtons.DPad:		return false;

				default:							return false;
			}
		}

		public override Vector2 LeftJoystick(int PlayerNumber) 
        {
            Vector2 vec = GamepadState[PlayerNumber].ThumbSticks.Left;
            return new Vector2(vec.X, vec.Y);
        }
		public override Vector2 RightJoystick(int PlayerNumber)
        {
            Vector2 vec = GamepadState[PlayerNumber].ThumbSticks.Right;
            return new Vector2(vec.X, vec.Y);
        }

		public override Vector2 DPad(int PlayerNumber)
		{
			Vector2 Dir = Vector2.Zero;

			var Pad = GamepadState[PlayerNumber].DPad;

			if (Pad.Right	== input.ButtonState.Pressed) Dir.X = 1;
			if (Pad.Up		== input.ButtonState.Pressed) Dir.Y = 1;
			if (Pad.Left	== input.ButtonState.Pressed) Dir.X = -1;
			if (Pad.Down	== input.ButtonState.Pressed) Dir.Y = -1;

			return Dir;
		}

		public override float LeftTrigger(int PlayerNumber)  { return GamepadState[PlayerNumber].Triggers.Left;  }
		public override float RightTrigger(int PlayerNumber) { return GamepadState[PlayerNumber].Triggers.Right; }
	}
}