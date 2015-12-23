using System;

using Microsoft.Xna.Framework;

namespace CoreEngine
{
	public abstract class GamepadBase
	{
		public abstract void Initialize(GameServiceContainer Container, GameComponentCollection ComponentCollection, IntPtr WindowHandle);

		public abstract void OnLoad();

		public abstract void Update_EndOfStep();
		public abstract void Update();

		public abstract void Clear();

		public abstract bool IsConnected(int PlayerNumber);

		public abstract bool IsPressed(int PlayerNumber, ControllerButtons Button, bool Previous);

		public abstract Vector2 LeftJoystick(int PlayerNumber);
		public abstract Vector2 RightJoystick(int PlayerNumber);

		public abstract Vector2 DPad(int PlayerNumber);

		public abstract float LeftTrigger(int PlayerNumber);
		public abstract float RightTrigger(int PlayerNumber);
	}
}