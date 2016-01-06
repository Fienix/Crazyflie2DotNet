using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Crazyflie2DotNet.Crazyflie.TransferProtocol;
using Crazyflie2DotNet.Crazyradio.Driver;
using System.ComponentModel;
using log4net;
using log4net.Config;

namespace Crazyflie2DotNet.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Parameters
        //private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));

        //params
        private ushort thrust = 10000;
        private float pitch = 0;
        private float yaw = 0;
        private float roll = 0;

        //limit params
        private ushort minThrust = 10000; //10500 motor min
        private ushort maxThrust = 45000; //60000 motor max
        private float minYaw = -15.0f;
        private float maxYaw = 15.0f;

        //increment params
        private ushort thrustIncrements = 500;
        private float pitchIncrements = 1;
        private float yawIncrements = 1;
        private float rollIncrements = 1;

        //mouse positions
        private float x = 0;
        private float y = 0;
        private float currentX = 0;
        private float currentY = 0;
        private float xSlop = 10;
        private float ySlop = 10;
        private float prModifier = 10;

        //Crazyradio Parameters
        ICrazyradioDriver crazyradioDriver;
        CrazyradioMessenger crazyRadioMessenger;

        //timers
        private System.Windows.Forms.Timer thrustYawTimer = new System.Windows.Forms.Timer { Interval = 25 };
        private System.Windows.Forms.Timer pitchRollTimer = new System.Windows.Forms.Timer { Interval = 50 };
        private System.Windows.Forms.Timer normalizeTimer = new System.Windows.Forms.Timer { Interval = 50 };

        //keys pressed
        private bool _W = false;
        private bool _S = false;
        private bool _A = false;
        private bool _D = false;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeCrazyflie();
        }

        private void InitializeCrazyflie()
        {
            try
            {
                WriteToConsole("Initializing Crazyflie...");
                //BasicConfigurator.Configure();
                crazyradioDriver = SetupCrazyflieDriver();
                crazyRadioMessenger = new CrazyradioMessenger(crazyradioDriver);

                thrustYawTimer.Tick += thrustYawTimer_Tick;
                pitchRollTimer.Tick += pitchRollTimer_Tick;
                normalizeTimer.Tick += normalizeTimer_Tick;

                pitchRollTimer.Start();

                WriteToConsole("Crazyflie intilization successful.");
            }
            catch (Exception ex)
            {
                WriteToConsole("Error controlling Crazyradio.");
            }
        }

        private ICrazyradioDriver SetupCrazyflieDriver()
        {
            IEnumerable<ICrazyradioDriver> crazyradioDrivers = null;

            try
            {
                // Scan for connected Crazyradio USB dongles
                crazyradioDrivers = CrazyradioDriver.GetCrazyradios();
            }
            catch (Exception ex)
            {
                WriteToConsole("Error getting Crazyradio USB dongle devices connected to computer.");
            }

            // If we found any
            if (crazyradioDrivers != null && crazyradioDrivers.Any())
            {
                // Use first available Crazyradio dongle
                var crazyradioDriver = crazyradioDrivers.First();

                try
                {
                    // Initialize driver
                    crazyradioDriver.Open();

                    // Scan for any Crazyflie quadcopters ready for communication
                    var scanResults = crazyradioDriver.ScanChannels();
                    if (scanResults.Any())
                    {
                        // Use first online Crazyflie quadcopter found
                        var firstScanResult = scanResults.First();

                        // Set CrazyradioDriver's DataRate and Channel to that of online Crazyflie
                        var dataRateWithCrazyflie = firstScanResult.DataRate;
                        var channelWithCrazyflie = firstScanResult.Channels.First();
                        crazyradioDriver.DataRate = dataRateWithCrazyflie;
                        crazyradioDriver.Channel = channelWithCrazyflie;

                        return crazyradioDriver;
                    }
                    else
                    {
                        WriteToConsole("No Crazyflie quadcopters available for communication.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    WriteToConsole("Error initializing Crazyradio USB dongle for communication with a Crazyflie quadcopter.");
                    throw;
                }
            }
            else
            {
                WriteToConsole("No Crazyradio USB dongles found!");
                return null;
            }
        }

        #region Timers
        private void thrustYawTimer_Tick(object sender, EventArgs e)
        {
            if (_W) { thrust += thrustIncrements; }
            if (_S) { thrust -= thrustIncrements; }
            if (_D) { yaw += yawIncrements; }
            if (_A) { yaw -= yawIncrements; }

            yaw = Utils.Clamp(yaw, minYaw, maxYaw);
            thrust = Utils.Clamp(thrust, minThrust, maxThrust);
            SendControl();
        }

        private void pitchRollTimer_Tick(object sender, EventArgs e)
        {
            currentX = System.Windows.Forms.Cursor.Position.X / prModifier;
            currentY = System.Windows.Forms.Cursor.Position.Y / prModifier;
            if (currentX > x + xSlop) { roll += rollIncrements; x = currentX; }
            else if (currentX < x - xSlop) { roll -= rollIncrements; x = currentX; }
            if (currentY > y + ySlop) { pitch += pitchIncrements; y = currentY; }
            else if (currentY < y - ySlop) { pitch -= pitchIncrements; y = currentY; }

            SendControl();
        }

        private void normalizeTimer_Tick(object sender, EventArgs e)
        {
            if (thrust > 15000) { thrust -= 300; }
            if (yaw > 0) { yaw -= 0.5f; }
            else if (yaw < 0) { yaw += 0.5f; }

            UpdateCommandValues();
        }
        #endregion Timers

        #region Key Events
        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            switch (e.Key)
            {
                // stop
                case Key.Space:
                case Key.Escape:
                    StopControl();
                    return;
                // thrust up
                case Key.W:
                    _W = true;
                    break;
                // thrust down
                case Key.S:
                    _S = true;
                    break;
                // yaw right
                case Key.D:
                    _D = true;
                    break;
                // yaw left
                case Key.A:
                    _A = true;
                    break;

                default:
                    WriteToConsole("Invalid key for action.");
                    break;
            }

            thrustYawTimer.Start();
            normalizeTimer.Stop();
        }

        public void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                case Key.Escape:
                    break;
                // thrust up
                case Key.W:
                    _W = false;
                    break;
                // thrust down
                case Key.S:
                    _S = false;
                    break;
                // yaw right
                case Key.D:
                    _D = false;
                    break;
                // yaw left
                case Key.A:
                    _A = false;
                    break;

                default:
                    WriteToConsole("Invalid key for action.");
                    break;
            }

            if (!(_W || _S || _D || _A))
            {
                thrustYawTimer.Stop();
                normalizeTimer.Start();
            }
        }
        #endregion

        #region Send Control
        private void StopControl()
        {
            roll = 0;
            pitch = 0;
            yaw = 0;
            thrust = 10000;

            WriteToConsole("Cut all motors.");
            SendControl();
        }

        private void SendControl()
        {
            CommanderPacket commanderPacket = new CommanderPacket(roll, pitch, yaw, thrust);
            IPacket ackPacket = crazyRadioMessenger.SendMessage(commanderPacket);
            UpdateCommandValues();
        }
        #endregion

        #region UI Updates
        private void WriteToConsole(string text)
        {
            console.Text += "\n" + text;
            console.ScrollToEnd();
        }

        private void UpdateCommandValues()
        {
            thrustValue.Text = thrust.ToString();
            yawValue.Text = yaw.ToString();
            pitchValue.Text = pitch.ToString();
            rollValue.Text = roll.ToString();
        }
        #endregion
    }
}
