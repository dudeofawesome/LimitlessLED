﻿using System;
using System.Configuration;
using System.Net.Sockets;
using System.Threading;

namespace LimitlessLED_Test
{
    class UserCommands
    {
        // Connect to bridge
        static readonly string BridgeIpAddress = ConfigurationManager.AppSettings["ip"];
        static readonly UdpClient UdpClient = new UdpClient(Properties.Settings.Default.IP, 8899);

        /// <summary>
        /// Shortcut to send UDP commands to the wifi bridge
        /// </summary>
        static void LedBridge(byte[] hexCommand)
        {
            UdpClient.Send(hexCommand, 3);
        }

        /// <summary>
        /// Used for testing individual commands
        /// Will be removed when debugging is complete
        /// </summary>
        public static void Test()
        {
            AllOff();
            Thread.Sleep(100);
            //RGBOff();

            WakeUpCall();
        }

        /// <summary>
        /// Strobe mode:  
        /// Flash the lights till a key is pressed 
        /// </summary>
        public static void StrobeMode()
        {   
            Console.WriteLine("Starting Strobe mode, press any key to stop");
            while (!Console.KeyAvailable)
            {
                LedBridge(BridgeCommands.Group1Off);
                Thread.Sleep(100);
                LedBridge(BridgeCommands.Group1On);
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Slow fade group one to minimum
        /// this will probably be renamed later due to potential naming conflict
        /// </summary>
        public static void FadeDown()
        {
            Console.WriteLine("Dimming Group One");

            // Ensure group one is on, and selected by the wifi bridge
            LedBridge(BridgeCommands.Group1On);
            Thread.Sleep(101);

            while (Properties.Settings.Default.currentBrightness > 2)
            {
                Console.WriteLine(Properties.Settings.Default.currentBrightness);
                LedBridge(new byte[] { 0x4E, (byte)Properties.Settings.Default.currentBrightness, 0x55 });
                Properties.Settings.Default.currentBrightness--;
                Thread.Sleep(101);
            }
        }

        /// <summary>
        /// Slow fade group one to maximum
        /// this will probably be renamed later due to potential naming conflict
        /// </summary>
        public static void FadeUp()
        {
             Console.WriteLine("Brightening Group One");
            
            // Ensure group one is on, and selected by the wifi bridge
            LedBridge(BridgeCommands.Group1On);
            Thread.Sleep(101);

            while (Properties.Settings.Default.currentBrightness < 27)
            {
                Console.WriteLine(Properties.Settings.Default.currentBrightness);
                LedBridge(new byte[] { 0x4E, (byte)Properties.Settings.Default.currentBrightness, 0x55 });
                Properties.Settings.Default.currentBrightness++;
                Thread.Sleep(101);
            }
        }

        /// <summary>
        /// Turn all white lights off
        /// </summary>
        public static void AllOff()
        {
            LedBridge(BridgeCommands.AllOff);
        }

        /// <summary>
        /// Turn all white lights on
        /// </summary>
        public static void AllOn()
        {
            LedBridge(BridgeCommands.AllOn);
        }

        public static void AllWhite()
        {
            LedBridge(BridgeCommands.AllWhite);
        }

        /// <summary>
        ///  Brighten the last selected group
        /// </summary>
        public static void Brighten()
        {
            Properties.Settings.Default.currentBrightness++;
            LedBridge(new byte[] { 0x4E, (byte)Properties.Settings.Default.currentBrightness, 0x55 });
        }

        /// <summary>
        /// Dim the last selected group
        /// </summary>
        public static void Dim()
        {
            Properties.Settings.Default.currentBrightness--;
            LedBridge(new byte[] { 0x4E, (byte)Properties.Settings.Default.currentBrightness, 0x55 });
        }

        /// <summary>
        /// Turn group 1 off
        /// </summary>
        public static void Group1Off()
        {
            LedBridge(BridgeCommands.Group1Off);
        }

        /// <summary>
        /// Turn group 2 off
        /// </summary>
        public static void Group2Off()
        {
            LedBridge(BridgeCommands.Group2Off);
        }

        /// <summary>
        /// Turn group 3 off
        /// </summary>
        public static void Group3Off()
        {
            LedBridge(BridgeCommands.Group3Off);
        }

        /// <summary>
        /// Turn group 4 off
        /// </summary>
        public static void Group4Off()
        {
            LedBridge(BridgeCommands.Group4On);
        }

        /// <summary>
        /// Turn group 1 off
        /// </summary>
        public static void Group1On()
        {
            LedBridge(BridgeCommands.Group1On);
        }

        /// <summary>
        /// Turn group 2 off
        /// </summary>
        public static void Group2On()
        {
            LedBridge(BridgeCommands.Group2On);
        }

        /// <summary>
        /// Turn group 3 off
        /// </summary>
        public static void Group3On()
        {
            LedBridge(BridgeCommands.Group3On);
        }

        /// <summary>
        /// Turn group 4 off
        /// </summary>
        public static void Group4On()
        {
            LedBridge(BridgeCommands.Group4On);
        }

        /// <summary>
        /// Turns on all lights to minimum brightness, then slowly increases the brightness over 10 min
        /// </summary>
        public static void WakeUpCall()
        {
            // Turn on White lights and then dim them to minimum as fast as possible
            AllOn();
            Thread.Sleep(101);         
            for (int i = 0; i < 10; i++)
            {
                Dim();
                Thread.Sleep(101);
            }

            // Turn on the RGB Lights, set them to white and then dim them to minimum as fast as possible
            AllOn();
            Thread.Sleep(101);
            Disco();
            Thread.Sleep(101);
            for (int i = 0; i < 10; i++)
            {
                Dim();
                Thread.Sleep(101);
            }
                    
            // Slowly increase brightness to max over the next 10 min
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(60000);
                AllOn();
                Thread.Sleep(101);
                Brighten();
                Thread.Sleep(101);
                AllOn();
                Thread.Sleep(101);
                Disco();
                Thread.Sleep(101);
                Brighten();
            }
        }

        /// <summary>
        /// Will set the RGB bulbs to the colour specified between 0 and 255
        /// </summary>
        /// <param name="colour"></param>
        public static void RGBColour(int colour)
        {
            // Set invalid colours to 0
            colour = (colour > 0 && colour < 256) ? colour : 0;
            
            LedBridge(new byte[] { 0x40, (byte)colour, 0x55 });
        }
        
        /// <summary>
        /// Set Disco Mode to next mode
        /// </summary>
        public static void Disco()
        {
            LedBridge(BridgeCommands.DiscoNext);
        }

        /// <summary>
        /// Increase the speed of the Colour LED's disco mode
        /// </summary>
        public static void DiscoSpeedUp()
        {
            LedBridge(BridgeCommands.DiscoSpeedUp);
        }

        /// <summary>
        /// Decrease the speed of the Colour LED's Disco mode
        /// </summary>
        public static void DiscoSpeedDown()
        {
            LedBridge(BridgeCommands.DiscoSpeedDown);
        }
    }
}
