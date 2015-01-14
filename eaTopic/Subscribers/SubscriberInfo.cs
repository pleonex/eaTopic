//
//  SubscriberInfo.cs
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

namespace EaTopic.Subscribers
{
	internal class SubscriberInfo<T> : TopicData
		where T : TopicData, new()
	{
		public SubscriberInfo()
		{
		}

		public SubscriberInfo(Subscriber<T> subscriber)
		{
			TopicName = subscriber.Topic.Name;
			Metadadata = subscriber.Metadata;
		}

		public override TopicDataType Type {
			get { return TopicDataType.FromGeneric<string, string, DateTime>(); }
		}

		public string Metadadata { get; set; }

		public string TopicName { get ; set; }

		public DateTime InfoCreationDate { get; set; }

		public override void SerializeData(DataFormatter formatter)
		{
			InfoCreationDate = DateTime.Now;
			formatter.Set(0, TopicName);
			formatter.Set(1, Metadadata);
			formatter.Set(2, InfoCreationDate);
		}

		public override void DeserializeData(DataFormatter formatter)
		{
			TopicName  = formatter.Get(0);
			Metadadata = formatter.Get(1);
			InfoCreationDate = formatter.Get(2);
		}
	}
}

