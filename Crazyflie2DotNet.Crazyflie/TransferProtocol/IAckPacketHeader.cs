using Crazyflie2DotNet.Crazyradio.Driver;

namespace Crazyflie2DotNet.Crazyflie.TransferProtocol
{
	public interface IAckPacketHeader
		: IPacketHeader
	{
        bool AckRecieved { get; }

        bool PowerDetector { get; }

        int RetryCount { get; }
    }
}