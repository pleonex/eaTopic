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
using EaTopic.Topics;
using System.Collections.Generic;
using EaTopic.Transports;
using EaTopic.Participants.Builtin;

namespace EaTopic.Subscribers
{
	public delegate void ReceivedInstanceHandleEvent<T>(T instance) 
		where T : TopicData, new();

	public class Subscriber<T> : Entity
		where T : TopicData, new()
	{
		readonly List<TransportReceiver> receivers;

		internal Subscriber(Topic<T> topic, string metadata)
		{
			receivers = new List<TransportReceiver>();
			Topic = topic;
			Metadata = metadata;
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

		internal SubscriberInfo Info {
			get;
			private set;
		}

		public void Dispose()
		{
			foreach (var recv in receivers)
				recv.Close();

			receivers.Clear();
		}

		public event ReceivedInstanceHandleEvent<T> ReceivedInstance;

		public void OnReceivedData(DataFormatter formatter)
		{
			var instance = TopicData.DeserializeData<T>(formatter);

			if (ReceivedInstance != null)
				ReceivedInstance(instance);
		}

		void CreateReceiver(string ip, int port)
		{
			TransportReceiver recv;

			if (Topic.IsBuiltin) {
				var topic = Topic.Participant.BuiltinTopic;
				recv = new UdpMulticastReceiver(topic.MulticastAddress, topic.MulticastPort);
			} else
				recv = null;	// TODO

			receivers.Add(recv);

			var formatter = new DataFormatter(Topic.DataType);
			recv.StartReceive(formatter);
		}
	}
}

