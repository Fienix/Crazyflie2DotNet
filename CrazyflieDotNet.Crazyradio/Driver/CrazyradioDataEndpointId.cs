﻿#region Imports

using LibUsbDotNet.Main;

#endregion

namespace Crazyflie2DotNet.Crazyradio.Driver
{
	/// <summary>
	/// Internal static properties used by the CrazyradioDriver class.
	/// These two static properties define the endpoints for read and write usb comminication with the Crazyradio dongle USB device.
	/// </summary>
	internal static class CrazyradioDataEndpointId
	{
		/// <summary>
		/// The data READ endpoint for the Crazyradio USB dongle.
		/// </summary>
		public static readonly ReadEndpointID DataReadEndpointId = ReadEndpointID.Ep01;

		/// <summary>
		/// The data WRITE endpoint for the Crazyradio USB dongle.
		/// </summary>
		public static readonly WriteEndpointID DataWriteEndpointId = WriteEndpointID.Ep01;
	}
}