#if PC_VERSION

using System;
using System.Globalization;
using System.Windows.Forms;
using SlimDX;
using SlimDX.XInput;
using SlimDX.DirectInput;
using di = SlimDX.DirectInput;
using SlimDX.RawInput;
using ri = SlimDX.RawInput;
using System.Collections.Generic;

namespace Joystick
{
	public class Joystick
	{
		public static di.Joystick[] GetSticks()
		{
			DirectInput dinput = new DirectInput();

			var sticks = new List<SlimDX.DirectInput.Joystick>();
			foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
			{
				// create the device
				try
				{
					var stick = new SlimDX.DirectInput.Joystick(dinput, device.InstanceGuid);
					stick.Acquire();

					foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
					{
						if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
							stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
					}

					sticks.Add(stick);

					//Console.WriteLine(stick.Information.InstanceName);
				}
				catch (DirectInputException)
				{
				}
			}
			return sticks.ToArray();
		}

		public static void CreateDevice(IntPtr handle)
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
		}

		public static void ReadImmediateData(di.Joystick joystick)
		{
			var js = GetSticks();

			if (joystick.Acquire().IsFailure)
				return;

			if (joystick.Poll().IsFailure)
				return;

			var state = joystick.GetCurrentState();
			if (Result.Last.IsFailure)
				return;
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