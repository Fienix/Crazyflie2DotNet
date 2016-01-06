﻿namespace Crazyflie2DotNet.Crazyflie.TransferProtocol
{
	public enum CommunicationPort
	{
		Console = 0x00,

		Parameters = 0x02,

		Commander = 0x03,

		Logging = 0x05,

		Debugging = 0x0E,

		LinkControl = 0x0F,

		All = 0xFF
	}
}