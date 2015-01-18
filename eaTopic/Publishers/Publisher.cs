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
using System.Linq;
using EaTopic.Participants.Builtin;
using EaTopic.Subscribers;
using EaTopic.Topics;
using EaTopic.Transports;

namespace EaTopic.Publishers
{
	public class Publisher<T> : Entity
		where T: TopicData, new()
	{
		readonly List<RemoteHost> hosts;
		readonly Topic<T> topic;

		internal Publisher(Topic<T> topic, string metadata)
		{
			hosts = new List<RemoteHost>();
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

		public override EntityInfo Info {
			get;
			set;
		}

		public override void Dispose()
		{
			foreach (var host in hosts)
				host.Sender.Close();

			hosts.Clear();
		}

		/// <summary>
		/// Write / Publish an instance of data.
		/// </summary>
		/// <param name="instance">Instance of data to write.</param>
		public void Write(T instance)
		{
			foreach (var host in hosts)
				Write(instance, host);
		}

		void Write(T instance, RemoteHost host)
		{
			if (host.Info != null && !host.Info.Filter.IsValid(instance))
				return;

			host.Sender.Write(instance.SerializeData());
		}

		void Initialize()
		{
			if (Topic.IsBuiltin) {
				hosts.Add(new RemoteHost(
					new UdpMulticastSender(
						BuiltinTopic.MulticastAddress, BuiltinTopic.MulticastPort),
					null));
			} else {
				var builtinTopic = Topic.Participant.BuiltinTopic;
				foreach (var subInfo in builtinTopic.GetSubscribers((TopicInfo)Topic.Info))
					hosts.Add(new RemoteHost(
						new TcpUnicastSender(subInfo.IpAddress, subInfo.Port),
						subInfo));

				builtinTopic.SubscriberDiscovered += OnSubscriberDiscovered;
			}
		}

		void OnSubscriberDiscovered(SubscriberInfo subInfo, BuiltinEventArgs e)
		{
			if (e.Topic.TopicName != Topic.Name)
				return;

			if (e.Change == BuiltinEntityChange.Added)
				hosts.Add(new RemoteHost(
					new TcpUnicastSender(subInfo.IpAddress, subInfo.Port),
					subInfo));
			else if (e.Change == BuiltinEntityChange.Modified) {
				int idx = hosts.FindIndex(
					(h) => h.Info != null && h.Info.Uuid.SequenceEqual(subInfo.Uuid));

				if (idx != -1)
					hosts[idx].Info = subInfo;
			}
		}

		class RemoteHost
		{
			public RemoteHost(TransportSender sender, SubscriberInfo info)
			{
				this.Sender = sender;
				this.Info = info;
			}

			public TransportSender Sender { get; private set; }
			public SubscriberInfo Info { get; set; }
		}
	}
}

