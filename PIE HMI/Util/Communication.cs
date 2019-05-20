using ACS.SPiiPlusNET;
using PIE_HMI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PIE_HMI.Util
{
    public struct Global
    {
        // System publics
        public int running;
        public int advanceState;
        public int machineState;

        // public State Machine vars
        public int opControlState;
        public int drillState;
        public int coreState;
        public int extractState;
        public int filterState;

        // Telemetry
        public double drillWOB;
        public double drillRPM;
        public int heatTemp;

        public double framePwr;
        public double drillPwr;
        public double waterExPwr;
        public double filterPwr;
        public double totalPwr;

        public int holeCount;

        // Coring vars
        public int coreSamples;
        public double coreRPM;

        // Drilling Vars
        public double touchRMS;
        public double peckDistance;
        public double retractDistance;
        public double holeSpacing;
        public int numHoles;

        // Water Extract Vars
        public double xOffset;
        public double heaterSetpoint;

        // Filtration vars
        public int backFlush;
        public double ecAmpTarget;
        public double ecTime;
        public int enableSloshing;
        public double sloshAmplitude;
        public double sloshFrequency;
        public double sloshTime;

        // Heater control vars
        public int pellet1_h1_SetTemp;
        public double PELLET1_TC1_OFFSET,PELLET1_TC1_STATUS_OFFSET;
        public double PELLET1_TC1, PELLET1_TC1_STATUS;   //This is the temperature variable in c

        // Digital and Analog IO
        public int ECAT_OFFSET;
        public int ECAT_OUT1; // Digital Out
        public int ECAT_OUT2; // Analog Out 1
        public int ECAT_OUT3; // Analog Out 2
        public int ECAT_IN1;  // Digital In 1

        // Jog Control vars
        public double jogX;
        public double jogZ1;
        public double jogZ2;
        public double jogDrill;
        public int jogBoreholePump;
        public int homeX;
        public int homeZ1;
        public int homeZ2;

        // Homing vars
        public int currentHomingAxis;
        public int homeAxis;

    };

    public struct Persist
    {
        public double drillVel;
        public double drillAccel;
        public double drillJerk;

        public double Z1TravelVel;
        public double Z1DrillVel;
        public double Z1Accel;
        public double Z1Jerk;

        public double Z2TravelVel;
        public double Z2ProbeVel;
        public double Z2Accel;
        public double Z2Jerk;

        public double XVel;
        public double XAccel;
        public double XJerk;

        public int persistentChanged;
    };

    public sealed class Communication
    {

        //Thread-safe lazy, singleton pattern
        private static readonly Lazy<Communication> lazy = new Lazy<Communication>(() => new Communication());

        public static Communication Instance { get { return lazy.Value; } }

        private Communication()
        {

        }

        private Api ch = new Api();

        private bool connected;

        public Global global = new Global();
        public Persist persist = new Persist();

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
                if(Connected())
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
                if(Connected())
                    data = ch.ReadVariable(variable, buffer);
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }
            return data;
        }

        public void WriteGlobal()
        {
            try
            {
                foreach (var field in typeof(Global).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    WriteVariable((object)field.GetValue(global), field.Name);
                }
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
            }
        }

        public void WritePersist()
        {
            try
            {
                persist.persistentChanged = 1;
                foreach (var field in typeof(Persist).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    WriteVariable((object)field.GetValue(persist), field.Name);
                }
            }
            catch (Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
            }
        }

        public void ReadGlobal()
        {
            try
            {
                foreach (var field in typeof(Global).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    object boxed = global;
                    field.SetValue(boxed, ReadVariable(field.Name));
                    global = (Global)boxed;
                }
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
            }
        }

        public void ReadPersist()
        {
            try
            {
                foreach (var field in typeof(Persist).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    object boxed = persist;
                    field.SetValue(boxed, ReadVariable(field.Name));
                    persist = (Persist)boxed;
                }
            }
            catch (Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
            }
        }

    }
}
