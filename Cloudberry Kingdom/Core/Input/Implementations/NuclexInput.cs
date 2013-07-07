#if PC_VERSION
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nuclex.Input;

namespace CoreEngine
{
	public class NuclexGamepadInput : XnaInput
	{
		InputManager NuclexInput;
		
		public override void Initialize(GameServiceContainer Container, GameComponentCollection ComponentCollection, IntPtr WindowHandle)
		{
			NuclexInput = new InputManager(Container, WindowHandle);
			ComponentCollection.Add(NuclexInput);
		}

		public override void Update()
		{
			GamepadState[0] = NuclexInput.GetGamePad(PlayerIndex.One).GetState();
			GamepadState[1] = NuclexInput.GetGamePad(PlayerIndex.Two).GetState();
			GamepadState[2] = NuclexInput.GetGamePad(PlayerIndex.Three).GetState();
			GamepadState[3] = NuclexInput.GetGamePad(PlayerIndex.Four).GetState();
		}
	}
}
#endif