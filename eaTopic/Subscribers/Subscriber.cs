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

namespace EaTopic.Subscribers
{
	public class Subscriber<T> : Entity
		where T : TopicData, new()
	{
		readonly List<TransportReceiver> receivers;

		internal Subscriber(Topic<T> topic)
		{
			receivers = new List<TransportReceiver>();
			Topic = topic;
		}

		public string Metadata {
			get;
			set;
		}

		public Topic<T> Topic {
			get;
			private set;
		}

		public void Dispose()
		{
			foreach (var recv in receivers)
				recv.Close();

			receivers.Clear();
		}

		public T Read()
		{
			var instance = new T();

			var formatter = instance.CreateFormatter();
			receivers[0].Read(formatter);	// TEMP: Do it async with events for all recv
			instance.DeserializeData(formatter);

			return instance;
		}
	}
}

