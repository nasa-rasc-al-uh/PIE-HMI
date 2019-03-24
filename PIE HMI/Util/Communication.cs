using ACS.SPiiPlusNET;
using PIE_HMI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIE_HMI.Util
{
    class Communication
    {
        private static Api ch;

        private static bool connected;
        
        public void Init()
        { 
            if ( ch == null )
                ch = new Api();
        }

        public Api getAPI()
        {
            return ch;
        }

        public bool OpenCommNetwork(string ip_address, int protocol)
        {
            try
            {
                ch.OpenCommEthernet(ip_address, protocol);
                connected = true;
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }

            return connected;
        }

        public bool OpenCommSerial(int channel, int rate)
        {
            try
            {
                ch.OpenCommSerial(channel, rate);
                connected = true;
            }
            catch (Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }

            return connected;
        }

        public bool OpenCommPCI(int slotNumber)
        {
            try
            {
                ch.OpenCommPCI(slotNumber);
                connected = true;
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }
            return connected;
        }

        public bool OpenCommSimulator()
        {
            try
            {
                ch.OpenCommSimulator();
                connected = true;
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }
            return connected;
        }

        public bool Disconnect()
        {
            if (connected)
            {
                try
                {
                    ch.CloseComm();
                    connected = false;
                }
                catch(Exception ex)
                {
                    LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return connected;
        }

        public bool Connected()
        {
            connected = ch.IsConnected;
            return connected;
        }

        public void WriteVariable(object value, string variable, ProgramBuffer buffer = ProgramBuffer.ACSC_NONE)
        {
            try
            {
                ch.WriteVariable(value, variable, buffer);
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public object ReadVariable(string variable, ProgramBuffer buffer = ProgramBuffer.ACSC_NONE)
        {
            object data = (int)0;
            try
            {
                 data = ch.ReadVariable(variable, buffer);
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }
            return data;
        }

    }
}
