using System;

using CloudberryKingdom;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CoreEngine
{
	public class XnaInput : GamepadBase
	{
		protected GamePadState[] GamepadState, PrevGamepadState;

		public override void Initialize(GameServiceContainer Container, GameComponentCollection ComponentCollection, IntPtr WindowHandle)
		{
			// No initialization required for base XNA input.
		}

		public override void OnLoad()
		{
			GamepadState = new GamePadState[4];
			PrevGamepadState = new GamePadState[4];
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
			GamepadState[0] = GamePad.GetState(PlayerIndex.One);
			GamepadState[1] = GamePad.GetState(PlayerIndex.Two);
			GamepadState[2] = GamePad.GetState(PlayerIndex.Three);
			GamepadState[3] = GamePad.GetState(PlayerIndex.Four);
		}

		public override void Clear()
		{
			GamepadState[0] = new GamePadState();
			GamepadState[1] = new GamePadState();
			GamepadState[2] = new GamePadState();
			GamepadState[3] = new GamePadState();
		}

		public override bool IsConnected(int PlayerNumber)
		{
			return GamepadState[PlayerNumber].IsConnected;
		}

		public override bool IsPressed(int PlayerNumber, ControllerButtons Button, bool Previous)
		{
			GamePadState Pad;
			
			if (Previous)
				Pad = PrevGamepadState[PlayerNumber];
			else
				Pad = GamepadState[PlayerNumber];

			switch (Button)
			{
				case ControllerButtons.Start:		return (Pad.Buttons.Start == ButtonState.Pressed); break;
				case ControllerButtons.Back:		return (Pad.Buttons.Back == ButtonState.Pressed); break;
				
				case ControllerButtons.A:			return (Pad.Buttons.A == ButtonState.Pressed); break;
				case ControllerButtons.B:			return (Pad.Buttons.B == ButtonState.Pressed); break;
				case ControllerButtons.X:			return (Pad.Buttons.X == ButtonState.Pressed); break;
				case ControllerButtons.Y:			return (Pad.Buttons.Y == ButtonState.Pressed); break;
				
				case ControllerButtons.LJButton:	return (Pad.Buttons.LeftStick == ButtonState.Pressed); break;
				case ControllerButtons.RJButton:	return (Pad.Buttons.RightStick == ButtonState.Pressed); break;
				
				case ControllerButtons.LS:			return (Pad.Buttons.LeftShoulder == ButtonState.Pressed); break;
				case ControllerButtons.RS:			return (Pad.Buttons.RightShoulder == ButtonState.Pressed); break;
				
				case ControllerButtons.LT:			return (Pad.Triggers.Left > .5f);
				case ControllerButtons.RT:			return (Pad.Triggers.Right > .5f);
				
				case ControllerButtons.LJ:			return (Pad.Buttons.LeftStick == ButtonState.Pressed);
				case ControllerButtons.RJ:			return (Pad.Buttons.RightStick == ButtonState.Pressed);
				
				case ControllerButtons.DPad:		return false;

				default:							return false;
			}
		}

		public override Vector2 LeftJoystick(int PlayerNumber)  { return GamepadState[PlayerNumber].ThumbSticks.Left; }
		public override Vector2 RightJoystick(int PlayerNumber) { return GamepadState[PlayerNumber].ThumbSticks.Right; }

		public override Vector2 DPad(int PlayerNumber)
		{
			Vector2 Dir = Vector2.Zero;

			var Pad = GamepadState[PlayerNumber].DPad;

			if (Pad.Right	== ButtonState.Pressed) Dir = new Vector2(1, 0);
			if (Pad.Up		== ButtonState.Pressed) Dir = new Vector2(0, 1);
			if (Pad.Left	== ButtonState.Pressed) Dir = new Vector2(-1, 0);
			if (Pad.Down	== ButtonState.Pressed) Dir = new Vector2(0, -1);

			return Dir;
		}

		public override float LeftTrigger(int PlayerNumber)  { return GamepadState[PlayerNumber].Triggers.Left;  }
		public override float RightTrigger(int PlayerNumber) { return GamepadState[PlayerNumber].Triggers.Right; }
	}
}