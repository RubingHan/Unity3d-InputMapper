//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Runtime.InteropServices;
using ws.winx.devices;
using System.Collections.Generic;
using System.Linq;

namespace ws.winx.platform.windows
{
    public class WinHIDInterface : IHIDInterface
    {

        #region Fields
        private List<IJoystickDriver> __drivers;// = new List<IJoystickDriver>();


        private IJoystickDriver __defaultJoystickDriver;

        JoystickDevicesCollection _joysticks;

        public readonly Dictionary<IJoystickDevice, IHIDDeviceInfo> DeviceHIDInfos;

        #endregion


        #region IHIDInterface implementation
        public IJoystickDriver defaultDriver
        {
            get { if (__defaultJoystickDriver == null) { __defaultJoystickDriver = new WinMMDriver(); } return __defaultJoystickDriver; }
            set { __defaultJoystickDriver = value; }

        }




        IDeviceCollection IHIDInterface.Devices
        {

            get { return _joysticks; }

        }






        public void Update()
        {
            Enumerate();
        }

        #endregion



        #region Constructor
        public WinHIDInterface(List<IJoystickDriver> drivers)
        {
            __drivers = drivers;
            _joysticks = new JoystickDevicesCollection();
            DeviceHIDInfos = new Dictionary<IJoystickDevice, IHIDDeviceInfo>();

            Enumerate();
        }
        #endregion









        void Enumerate()
        {


            uint deviceCount = 0;
            var deviceSize = (uint)Marshal.SizeOf(typeof(RawInputDeviceList));

            // first call retrieves the number of raw input devices
            var result = UnsafeNativeMethods.GetRawInputDeviceList(
                IntPtr.Zero,
                ref deviceCount,
                deviceSize);

            //_devices = new RawInputDevice[deviceCount];

            if ((int)result == -1 || deviceCount == 0)
            {
                // call failed, or no devices found
                return;
            }

            // allocates memory for an array of Win32.RawInputDeviceList
            IntPtr ptrDeviceList = Marshal.AllocHGlobal((int)(deviceSize * deviceCount));

            result = UnsafeNativeMethods.GetRawInputDeviceList(
                ptrDeviceList,
                ref deviceCount,
                deviceSize);

			int hidInx=1;

            if ((int)result != -1)
            {
              
				// enumerates array of Win32.RawInputDeviceList,
                // and populates array of managed RawInputDevice objects
                for (var index = 0; index < deviceCount; index++)
                {
                    var rawInputDeviceList =
                        (RawInputDeviceList)Marshal.PtrToStructure(
                            new IntPtr((ptrDeviceList.ToInt32() +
                                    (deviceSize * index))),
                            typeof(RawInputDeviceList));

                   

                    if (rawInputDeviceList.DeviceType == RawInputDeviceType.HumanInterfaceDevice)
						resolveDevice(getDeviceInfo(hidInx++,rawInputDeviceList));

                }
            }

            Marshal.FreeHGlobal(ptrDeviceList);

        }


		protected HIDDeviceInfo getDeviceInfo(int inx,RawInputDeviceList rawInputDeviceList)
        {



           DeviceInfo deviceInfo = GetDeviceInfo(rawInputDeviceList.DeviceHandle);
          
			return new HIDDeviceInfo(inx, Convert.ToInt32(deviceInfo.HIDInfo.VendorID), Convert.ToInt32(deviceInfo.HIDInfo.ProductID), rawInputDeviceList.DeviceHandle, this, GetDevicePath(rawInputDeviceList.DeviceHandle));

        }






        private static IntPtr GetDeviceData(
            IntPtr deviceHandle,
            RawInputDeviceInfoCommand command)
        {
            uint dataSize = 0;
            var ptrData = IntPtr.Zero;

            UnsafeNativeMethods.GetRawInputDeviceInfo(
                deviceHandle,
                command,
                ptrData,
                ref dataSize);

            if (dataSize == 0) return IntPtr.Zero;

            ptrData = Marshal.AllocHGlobal((int)dataSize);

            var result = UnsafeNativeMethods.GetRawInputDeviceInfo(
                deviceHandle,
                command,
                ptrData,
                ref dataSize);

            if (result == 0)
            {
                Marshal.FreeHGlobal(ptrData);
                return IntPtr.Zero;
            }

            return ptrData;
        }

        private static string GetDevicePath(IntPtr deviceHandle)
        {
            var ptrDeviceName = GetDeviceData(
                deviceHandle,
                RawInputDeviceInfoCommand.DeviceName);

            if (ptrDeviceName == IntPtr.Zero)
            {
                return string.Empty;
            }

            var deviceName = Marshal.PtrToStringAnsi(ptrDeviceName);
            Marshal.FreeHGlobal(ptrDeviceName);
            return deviceName;
        }

        private static DeviceInfo GetDeviceInfo(IntPtr deviceHandle)
        {
            var ptrDeviceInfo = GetDeviceData(
                deviceHandle,
                RawInputDeviceInfoCommand.DeviceInfo);

            if (ptrDeviceInfo == IntPtr.Zero)
            {
                return new DeviceInfo();
            }

            var deviceInfo = (DeviceInfo)Marshal.PtrToStructure(
                ptrDeviceInfo, typeof(DeviceInfo));

            Marshal.FreeHGlobal(ptrDeviceInfo);
            return deviceInfo;
        }


        protected void resolveDevice(HIDDeviceInfo deviceInfo)
        {
            //IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension> joyDevice = null;
            IJoystickDevice joyDevice = null;

            //loop thru drivers and attach the driver to device if compatible
            if (__drivers != null)
                foreach (var driver in __drivers)
                {
                    joyDevice = driver.ResolveDevice(deviceInfo);
                    if (joyDevice != null)
                    {
                        _joysticks[deviceInfo.device] = joyDevice;
                        joyDevice.driver = driver;
                        Debug.Log("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + driver.GetType().ToString());

                        break;
                    }
                }

            if (joyDevice == null)
            {//set default driver as resolver if no custom driver match device
                joyDevice = defaultDriver.ResolveDevice(deviceInfo);

               
                if (joyDevice != null)
                {
                    _joysticks[deviceInfo.device] = joyDevice;
                    joyDevice.driver = __defaultJoystickDriver;
					DeviceHIDInfos[joyDevice] = deviceInfo;
                    Debug.Log("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " attached to " + __defaultJoystickDriver.GetType().ToString());

                }
                else
                {
                    Debug.LogWarning("Device PID:" + deviceInfo.PID + " VID:" + deviceInfo.VID + " not found compatible driver on the system.Removed!");

                }

            }

		   
        }









        public enum RawInputDeviceType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            HumanInterfaceDevice = 2
        }

        public enum RawInputDeviceInfoCommand : uint
        {
            PreparsedData = 0x20000005,
            DeviceName = 0x20000007,
            DeviceInfo = 0x2000000b,
        }






        [StructLayout(LayoutKind.Explicit)]
        public struct DeviceInfo
        {
            [FieldOffset(0)]
            public int Size;
            [FieldOffset(4)]
            public int Type;
            [FieldOffset(8)]
            public DeviceInfoMouse MouseInfo;
            [FieldOffset(8)]
            public DeviceInfoKeyboard KeyboardInfo;
            [FieldOffset(8)]
            public DeviceInfoHID HIDInfo;
        }

        public struct DeviceInfoMouse
        {
            public uint ID;
            public uint NumberOfButtons;
            public uint SampleRate;
        }

        public struct DeviceInfoKeyboard
        {
            public uint Type;
            public uint SubType;
            public uint KeyboardMode;
            public uint NumberOfFunctionKeys;
            public uint NumberOfIndicators;
            public uint NumberOfKeysTotal;
        }

        public struct DeviceInfoHID
        {
            public uint VendorID;
            public uint ProductID;
            public uint VersionNumber;
            public ushort UsagePage;
            public ushort Usage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputDeviceList
        {
            public IntPtr DeviceHandle;
            public RawInputDeviceType DeviceType;
        }

        #region UnsafeNativeMethods

        public static class UnsafeNativeMethods
        {


            [DllImport("User32.dll", SetLastError = true)]
            public static extern uint GetRawInputDeviceList(
                IntPtr pRawInputDeviceList,
                ref uint uiNumDevices,
                uint cbSize);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetRawInputDeviceInfo(
                IntPtr hDevice,
                RawInputDeviceInfoCommand uiCommand,
                IntPtr data,
                ref uint size);

        }





        #endregion


        #region IntPtrEqualityComparer
        // Simple equality comparer to allow IntPtrs as keys in dictionaries
        // without causing boxing/garbage generation.
        // Seriously, Microsoft, shouldn't this have been in the BCL out of the box?
        class IntPtrEqualityComparer : IEqualityComparer<IntPtr>
        {
            public bool Equals(IntPtr x, IntPtr y)
            {
                return x == y;
            }

            public int GetHashCode(IntPtr obj)
            {
                return obj.GetHashCode();
            }
        }
        #endregion




        #region JoystickDevicesCollection

        /// <summary>
        /// Defines a collection of JoystickAxes.
        /// </summary>
        public sealed class JoystickDevicesCollection : IDeviceCollection
        {
            #region Fields
            readonly Dictionary<IntPtr, IJoystickDevice> JoystickDevices;
            // readonly Dictionary<IntPtr, IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension>> JoystickDevices;

			readonly Dictionary<int, IntPtr> JoystickIDToDevice;

            List<IJoystickDevice> _iterationCacheList;
            bool _isEnumeratorDirty = true;

            #endregion

            #region Constructors

            internal JoystickDevicesCollection()
            {
                JoystickDevices = new Dictionary<IntPtr, IJoystickDevice>(new IntPtrEqualityComparer());
                // JoystickDevices = new Dictionary<IntPtr, IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension>>(new IntPtrEqualityComparer());

                JoystickIDToDevice = new Dictionary<int, IntPtr>();

                
            }

            #endregion

            #region Public Members

            #region IDeviceCollection implementation

            public void Remove(IntPtr device)
            {
                JoystickIDToDevice.Remove(JoystickDevices[device].ID);
                JoystickDevices.Remove(device);

                _isEnumeratorDirty = true;
            }


            public void Remove(int inx)
            {
                IntPtr device = JoystickIDToDevice[inx];
                JoystickIDToDevice.Remove(inx);
                JoystickDevices.Remove(device);

                _isEnumeratorDirty = true;
            }




            public IJoystickDevice this[int ID]
            //public IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension> this[int index]
            {
                get { return JoystickDevices[JoystickIDToDevice[ID]]; }
                //				internal set { 
                //
                //							JoystickIndexToDevice [JoystickDevices.Count]=
                //							JoystickDevices[]
                //						}
            }


            // public IJoystickDevice<IAxisDetails, IButtonDetails, IDeviceExtension> this[IntPtr device]
            public IJoystickDevice this[IntPtr device]
            {
                get { return JoystickDevices[device]; }
                internal set
                {
					JoystickIDToDevice[value.ID] = device;
                    JoystickDevices[device] = value;

                    _isEnumeratorDirty = true;

                }
            }

            public bool ContainsKey(int key)
            {
                return JoystickIDToDevice.ContainsKey(key);
            }

            public bool ContainsKey(IntPtr key)
            {
                return JoystickDevices.ContainsKey(key);
            }

            public System.Collections.IEnumerator GetEnumerator()
            {
                if (_isEnumeratorDirty)
                {
                    _iterationCacheList = JoystickDevices.Values.ToList<IJoystickDevice>();
                    _isEnumeratorDirty = false;


                }

                return _iterationCacheList.GetEnumerator();
                   
            }


            /// <summary>
            /// Gets a System.Int32 indicating the available amount of JoystickDevices.
            /// </summary>
            public int Count
            {
                get { return JoystickDevices.Count; }
            }

            #endregion

            #endregion




           
        }
        #endregion;





    }

}















