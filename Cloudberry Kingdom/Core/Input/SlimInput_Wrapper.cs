#if PC_VERSION && CUSTOM_INPUT
using SlimDX;
using SlimDX.DirectInput;
using di = SlimDX.DirectInput;
using System.Collections.Generic;

using SlimGamepad;

namespace Joystick
{
	public class StickInput
	{
		public static GamepadState[] State;

		public static List<di.Joystick> Sticks;

		public static List<di.Joystick> GetSticks()
		{
			State = new GamepadState[4];
			for (int i = 0; i < 4; i++)
			{
				State[i] = new GamepadState((SlimDX.XInput.UserIndex)i);
			}

			DirectInput dinput = new DirectInput();

			Sticks = new List<di.Joystick>();
			foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AllDevices))
			//foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
			{
				// create the device
				try
				{
					var stick = new di.Joystick(dinput, device.InstanceGuid);
					stick.Acquire();

					foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
					{
						if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
							stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
					}

					Sticks.Add(stick);

					//Console.WriteLine(stick.Information.InstanceName);
				}
				catch (DirectInputException)
				{
				}
			}

			return Sticks;
		}

		/*public static void CreateDevice(IntPtr handle)
		{
			// make sure that DirectInput has been initialized
			DirectInput dinput = new DirectInput();

			// search for devices
			foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
			{
				// create the device
				try
				{
					var joystick = new di.Joystick(dinput, device.InstanceGuid);
					joystick.SetCooperativeLevel(handle, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);

					foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
					{
						if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
							joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
					}

					joystick.Acquire();

					break;
				}
				catch (DirectInputException)
				{
				}
			}
		}*/

		public static void ReadData()
		{
			for (int i = 0; i < 4; i++)
			{
				State[i].Update();

				System.Console.Write(State[i].A);
				if (State[i].A)
				{
					System.Console.Write("oh hai!");
				}
			}

			foreach (var j in Sticks)
			{
				ReadImmediateData(j);
			}
		}

		public static void ReadImmediateData(di.Joystick joystick)
		{
			if (joystick.Acquire().IsFailure)
				return;

			if (joystick.Poll().IsFailure)
				return;

			var state = joystick.GetCurrentState();
			
			if (Result.Last.IsFailure)
				return;

			System.Console.Write(state.AccelerationX);
		}

		public static void Cleanup()
		{
			foreach (var j in Sticks)
			{
				ReleaseDevice(j);
			}
		}

		public static void ReleaseDevice(di.Joystick joystick)
		{
			if (joystick != null)
			{
				joystick.Unacquire();
				joystick.Dispose();
			}
			joystick = null;
		}
	}
}

#endif