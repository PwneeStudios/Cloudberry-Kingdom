using System;

using Microsoft.Xna.Framework;

#if PC
#if !MONO && !SDL2
#endif
#endif

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
		private static GamepadBase I;

		public static void Initialize(GameServiceContainer Container, GameComponentCollection ComponentCollection, IntPtr WindowHandle)
		{
			#if MONO || SDL2
			I = new XnaInput();
			I.Initialize(Container, ComponentCollection, WindowHandle);
			#else
			//try
			//{
			//    I = new NuclexGamepadInput();
			//    I.Initialize(Container, ComponentCollection, WindowHandle);
			//}
			//catch (Exception e)
			//{
			//    CloudberryKingdom.Tools.Log("NuclexGamepadInput initialization failed. Exception is:");
			//    CloudberryKingdom.Tools.Log(e.ToString());
			//    CloudberryKingdom.Tools.Log("Trying standard XnaInput instead.");

			//    // We don't wrap the following in a try-catch. If this fails the game is not playable,
			//    // so let the top level exception-catcher-logger handle any failure.
			//    I = new XnaInput();
			//    I.Initialize(Container, ComponentCollection, WindowHandle);
			//}

			// Normally we try out Nuclex input first, then fall back to Xna if needed.
			// Some people are experiencing unresponsive input though, suggesting Nuclex is broken without crashing.
			// Why did we need Nuclex to begin with? ARRRGH, where is the documentation.
			try
			{
				I = new XnaInput();
				I.Initialize(Container, ComponentCollection, WindowHandle);
			}
			catch (Exception e)
			{
				CloudberryKingdom.Tools.Log("XnaInput initialization failed. Exception is:");
				CloudberryKingdom.Tools.Log(e.ToString());
				CloudberryKingdom.Tools.Log("Trying standard NuclexGamepadInput instead.");

				// We don't wrap the following in a try-catch. If this fails the game is not playable,
				// so let the top level exception-catcher-logger handle any failure.
				I = new NuclexGamepadInput();
				I.Initialize(Container, ComponentCollection, WindowHandle);
			}
			#endif
		}
		
		public static void OnLoad() { I.OnLoad(); }
		
		public static void Update_EndOfStep()	{ I.Update_EndOfStep(); }
		public static void Update()			
		{
			try
			{
				I.Update();
			}
			catch (Exception e)
			{
				CloudberryKingdom.Tools.Log("XnaInput initialization failed. Exception is:");
				CloudberryKingdom.Tools.Log(e.ToString());
				CloudberryKingdom.Tools.Log("Trying standard NuclexGamepadInput instead.");
			}
		}
		
		public static void Clear()				{ I.Clear(); }

		public static bool IsConnected(PlayerIndex Index) { return I.IsConnected((int)Index); }
		public static bool IsConnected(int PlayerNumber)  { return I.IsConnected(PlayerNumber); }

		public static bool IsPressed(PlayerIndex Index, ControllerButtons Button)						{ return I.IsPressed((int)Index, Button, false); }
		public static bool IsPreviousPressed(PlayerIndex Index, ControllerButtons Button)				{ return I.IsPressed((int)Index, Button, true); }
		public static bool IsPressed(PlayerIndex Index, ControllerButtons Button, bool Previous)		{ return I.IsPressed((int)Index, Button, Previous); }
		
		public static bool IsPressed		(int PlayerNumber, ControllerButtons Button)				{ return I.IsPressed(PlayerNumber, Button, false); }
		public static bool IsPreviousPressed(int PlayerNumber, ControllerButtons Button)				{ return I.IsPressed(PlayerNumber, Button, true); }
		public static bool IsPressed		(int PlayerNumber, ControllerButtons Button, bool Previous) { return I.IsPressed(PlayerNumber, Button, Previous); }

		public static Vector2 LeftJoystick(PlayerIndex Index)	{ return I.LeftJoystick((int)Index); }
		public static Vector2 RightJoystick(PlayerIndex Index)	{ return I.RightJoystick((int)Index); }

		public static Vector2 LeftJoystick(int PlayerNumber)	{ return I.LeftJoystick(PlayerNumber); }
		public static Vector2 RightJoystick(int PlayerNumber)	{ return I.RightJoystick(PlayerNumber); }

		public static Vector2 DPad(PlayerIndex Index)	{ return I.DPad((int)Index); }
		public static Vector2 DPad(int PlayerNumber)	{ return I.DPad(PlayerNumber); }

		public static float LeftTrigger(PlayerIndex Index) { return I.LeftTrigger((int)Index); }
		public static float RightTrigger(PlayerIndex Index) { return I.RightTrigger((int)Index); }

		public static float LeftTrigger (int PlayerNumber)	{ return I.LeftTrigger (PlayerNumber); }
		public static float RightTrigger(int PlayerNumber)	{ return I.RightTrigger(PlayerNumber); }
	}
}