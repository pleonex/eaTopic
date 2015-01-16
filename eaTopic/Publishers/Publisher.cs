//
//  Publisher.cs
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
using EaTopic.Participants.Builtin;
using EaTopic.Subscribers;
using EaTopic.Topics;
using EaTopic.Transports;

namespace EaTopic.Publishers
{
	public class Publisher<T> : Entity
		where T: TopicData, new()
	{
		readonly List<TransportSender> senders;
		readonly Topic<T> topic;

		internal Publisher(Topic<T> topic, string metadata)
		{
			this.senders = new List<TransportSender>();
			this.topic  = topic;
			Metadata    = metadata;

			Info = new PublisherInfo(this);
			Initialize();
		}

		/// <summary>
		/// Gets the metadata associated with this publisher
		/// </summary>
		/// <value>Metadata value.</value>
		public string Metadata {
			get;
			private set;
		}

		/// <summary>
		/// Gets the topic associated.
		/// </summary>
		/// <value>The topic where the publisher is.</value>
		public Topic<T> Topic {
			get { return topic; }
		}

		internal override EntityInfo Info {
			get;
			set;
		}

		public override void Dispose()
		{
			foreach (var sender in senders)
				sender.Close();

			senders.Clear();
		}

		/// <summary>
		/// Write / Publish an instance of data.
		/// </summary>
		/// <param name="instance">Instance of data to write.</param>
		public void Write(T instance)
		{
			foreach (var sender in senders)
				Write(instance, sender);
		}

		void Write(T instance, TransportSender sender)
		{
			sender.Write(instance.SerializeData());
		}

		void Initialize()
		{
			var builtinTopic = Topic.Participant.BuiltinTopic;

			if (Topic.IsBuiltin) {
				senders.Add(
					new UdpMulticastSender(
						builtinTopic.MulticastAddress, builtinTopic.MulticastPort
					));
			} else {
				foreach (var subInfo in builtinTopic.GetSubscribers(Topic))
					senders.Add(new TcpUnicastSender(subInfo.IpAddress, subInfo.Port));

				builtinTopic.SubscriberDiscovered += OnSubscriberDiscovered;
			}
		}

		void OnSubscriberDiscovered(SubscriberInfo subInfo, BuiltinEventArgs e)
		{
			if (e.Topic.TopicName != Topic.Name)
				return;

			if (e.Change == BuiltinEntityChange.Added)
				senders.Add(new TcpUnicastSender(subInfo.IpAddress, subInfo.Port));
		}
	}
}

