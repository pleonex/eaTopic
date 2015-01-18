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
	public class SubscriberInfo : EntityInfo
	{
		public SubscriberInfo()
		{
		}

		internal SubscriberInfo(dynamic subscriber)
		{
			TopicName  = subscriber.Topic.Name;
			Metadadata = subscriber.Metadata;
			IpAddress  = subscriber.IpAddress;
			Port = subscriber.Port;
			ParticipantUuid = subscriber.Topic.Participant.Info.Uuid;
		}

		public override TopicDataType Type {
			get { return TopicDataType.FromGeneric<byte[], string, string, string, int, DateTime>(); }
		}

		public string Metadadata { get; set; }

		public string TopicName { get ; set; }

		public string IpAddress { get; set; }

		public int Port { get; set; }

		public DateTime InfoCreationDate { get; set; }

		public byte[] ParticipantUuid { get; set; }

		public override void SerializeData(DataFormatter formatter)
		{
			InfoCreationDate = DateTime.UtcNow;
			formatter.Set(0, Uuid);
			formatter.Set(1, TopicName);
			formatter.Set(2, Metadadata);
			formatter.Set(3, IpAddress);
			formatter.Set(4, Port);
			formatter.Set(5, InfoCreationDate);
		}

		public override void DeserializeData(DataFormatter formatter)
		{
			Uuid = formatter.Get(0);
			TopicName  = formatter.Get(1);
			Metadadata = formatter.Get(2);
			IpAddress  = formatter.Get(3);
			Port = formatter.Get(4);
			InfoCreationDate = formatter.Get(5);
		}
	}
}

