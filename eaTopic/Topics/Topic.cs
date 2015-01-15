//
//  Topic.cs
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
using System.Reflection;
using EaTopic.Participants;
using EaTopic.Publishers;
using EaTopic.Subscribers;
using EaTopic.Transports;
using System.Collections.ObjectModel;

namespace EaTopic.Topics
{
	public class Topic<T> : Entity
		where T : TopicData, new()
	{
		readonly List<Publisher<T>> publishers;
		readonly List<Subscriber<T>> subscribers;

		internal Topic(Participant participant, string name, bool isBuiltin)
		{
			Name = name;
			IsBuiltin = isBuiltin;
			Participant = participant;
			publishers  = new List<Publisher<T>>();
			subscribers = new List<Subscriber<T>>();

			DataType = new T().Type;
			Info = new TopicInfo(this);
		}

		public string Name {
			get;
			private set;
		}

		public TopicDataType DataType {
			get;
			private set;
		}

		public Participant Participant {
			get;
			private set;
		}

		public ReadOnlyCollection<Publisher<T>> Publishers {
			get { return new ReadOnlyCollection<Publisher<T>>(publishers); }
		}

		public ReadOnlyCollection<Subscriber<T>> Subscribers {
			get { return new ReadOnlyCollection<Subscriber<T>>(subscribers); }
		}

		internal bool IsBuiltin {
			get;
			private set;
		}

		internal override EntityInfo Info {
			get;
			set;
		}

		public override void Dispose()
		{
			foreach (var pub in publishers)
				pub.Dispose();

			foreach (var sub in subscribers)
				sub.Dispose();

			publishers.Clear();
			subscribers.Clear();
		}

		public Publisher<T> CreatePublisher(string metadata = "")
		{
			var pub = new Publisher<T>(this, metadata);
			publishers.Add(pub);
			return pub;
		}

		public Subscriber<T> CreateSubscriber(string metadata = "")
		{
			var sub = new Subscriber<T>(this, metadata);
			subscribers.Add(sub);
			return sub;
		}
	}
}

