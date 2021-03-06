﻿/**
Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

using System.Collections.Generic;
using System.Linq;

namespace ccl
{
	/// <summary>
	/// Representation of a Cycles rendering device
	/// </summary>
	public class Device
	{
		/// <summary>
		/// Get the numerical ID for this device
		/// </summary>
		public uint Id { get; private set; }
		/// <summary>
		/// Get the Cycles description for the device
		/// </summary>
		public string Description { get; private set; }
		/// <summary>
		/// The name for the device
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Get the Cycles num for the device
		/// </summary>
		public uint Num { get; private set; }
		/// <summary>
		/// Get the device type
		/// </summary>
		public DeviceType Type { get; private set; }
		/// <summary>
		/// True if this device supports advanced shading
		/// </summary>
		public bool AdvancedShading { get; private set; }
		/// <summary>
		/// True if this device is used as a display device
		/// </summary>
		public bool DisplayDevice { get; private set; }
		/// <summary>
		/// True if this device supports packed images
		/// </summary>
		public bool PackImages { get; private set; }
		/// <summary>
		/// True if this is a CUDA device
		/// </summary>
		public bool IsCuda { get { return Type == DeviceType.CUDA;  } }

		/// <summary>
		/// Get the default device (CPU)
		/// </summary>
		/// <returns>The default device</returns>
		static public Device Default
		{
			get
			{
				return Device.GetDevice(0);
			}
		}

		/// <summary>
		/// Get a device by using GetDevice(int idx). Constructor is private.
		/// </summary>
		private Device() { }

		/// <summary>
		/// Get the number of available Cycles render devices
		/// </summary>
		/// <returns></returns>
		static public uint Count
		{
			get
			{
				return CSycles.number_devices();
			}
		}

		/// <summary>
		/// True if any of the available devices is a CUDA device
		/// </summary>
		/// <returns></returns>
		static public bool CudaAvailable()
		{
			return CSycles.number_cuda_devices() > 0;
		}

		/// <summary>
		/// Enumerate over available devices.
		/// </summary>
		static public IEnumerable<Device> Devices
		{
			get
			{
				for (var i = 0; i < Count; i++)
				{
					yield return Device.GetDevice(i);
				}
			}
		}

		/// <summary>
		/// Returns the first cuda device if it exists,
		/// the default rendering device if not.
		/// </summary>
		static public Device FirstCuda
		{
			get
			{
				foreach (var dev in Devices.Where(dev => dev.IsCuda))
				{
					return dev;
				}
				return Default;
			}
		}

		/// <summary>
		/// Get the device with specified index
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		static public Device GetDevice(int idx)
		{
			return new Device
			{
				Id = (uint)idx,
				Description = CSycles.device_decription(idx),
				Name = CSycles.DeviceId(idx),
				Num = CSycles.device_num(idx),
				Type = CSycles.device_type(idx),
				AdvancedShading = CSycles.device_advanced_shading(idx),
				DisplayDevice = CSycles.device_display_device(idx),
				PackImages = CSycles.device_pack_images(idx)
			};
		}
	}
}
