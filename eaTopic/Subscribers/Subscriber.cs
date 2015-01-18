//
//  Subscriber.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2015 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using EaTopic.Participants.Builtin;
using EaTopic.Topics;
using EaTopic.Transports;

namespace EaTopic.Subscribers
{
	public delegate void ReceivedInstanceHandleEvent<T>(T instance) 
		where T : TopicData, new();

	public class Subscriber<T> : Entity
		where T : TopicData, new()
	{
		TransportReceiver receiver;

		internal Subscriber(Topic<T> topic, string metadata)
		{
			Topic = topic;
			Metadata  = metadata;
			IpAddress = GetLocalIpAddress();

			CreateReceiver();
			StartToReceive();

			Port = receiver.Port;
			Filter = new Filter();
			Info = new SubscriberInfo(this);
		}

		public string Metadata {
			get;
			private set;
		}

		public Topic<T> Topic {
			get;
			private set;
		}

		public Filter Filter {
			get;
			private set;
		}

		public override EntityInfo Info {
			get;
			set;
		}

		internal string IpAddress {
			get;
			private set;
		}

		internal int Port {
			get;
			private set;
		}

		public override void Dispose()
		{
			receiver.Close();
		}

		public event ReceivedInstanceHandleEvent<T> ReceivedInstance;

		public void SetLocalFilter(Filter filter)
		{
			Filter = filter;
			((SubscriberInfo)Info).Filter = new Filter();
		}

		public void SetRemoteFilter(Filter filter)
		{
			Filter = new Filter();
			((SubscriberInfo)Info).Filter = filter;
		}

		public void RemoveLocalFilter()
		{
			Filter = new Filter();
			((SubscriberInfo)Info).Filter = Filter;
		}

		void OnReceivedData(DataFormatter formatter)
		{
			var instance = TopicData.DeserializeData<T>(formatter);

			if (!Filter.IsValid(instance))
				return;

			if (ReceivedInstance != null)
				ReceivedInstance(instance);
		}

		void CreateReceiver()
		{
			if (Topic.IsBuiltin) {
				receiver = new UdpMulticastReceiver(
					BuiltinTopic.MulticastAddress, BuiltinTopic.MulticastPort);
			} else
				receiver = new TcpUnicastReceiver(IpAddress);

			receiver.ReceivedData += OnReceivedData;
		}

		void StartToReceive()
		{
			var formatter = new DataFormatter(Topic.DataType);
			receiver.StartReceive(formatter);
		}

		static string GetLocalIpAddress()
		{
			return Dns.GetHostEntry(Dns.GetHostName()).AddressList
				.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
				.ToString();
		}
	}
}

