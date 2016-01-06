namespace Crazyflie2DotNet.Crazyflie.TransferProtocol
{
	public interface ICrazyflieMessenger
	{
		IAckPacket SendMessage(IPacket packet);
	}
}