//
//  ParticipantInfo.cs
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
using System.Linq;
using EaTopic.Topics;
using EaTopic.Subscribers;
using EaTopic.Publishers;

namespace EaTopic.Participants
{
	public class ParticipantInfo : EntityInfo
	{
		public ParticipantInfo()
		{
		}

		public byte ProtocolVersion { get { return 0x01; } }

		public byte Domain { get; set; }

		public SubscriberInfo[] Subscribers { get; set; }

		public PublisherInfo[] Publishers { get; set; }

		public TopicInfo[] Topics { get; set; }

		public override TopicDataType Type {
			get { 
				return TopicDataType.FromGeneric<uint, byte[], DataFormatter[],
					DataFormatter[], DataFormatter[]>();
			}
		}

		public override void SerializeData(DataFormatter formatter)
		{
			uint header = (uint)(ProtocolVersion | (Domain << 8));

			formatter[0] = header;
			formatter[1] = Uuid;
			formatter[2] = SerializeInfoList(Subscribers);
			formatter[3] = SerializeInfoList(Publishers);
			formatter[4] = SerializeInfoList(Topics);
		}

		public override void DeserializeData(DataFormatter formatter)
		{
			uint header = formatter[0];
			Domain = (byte)((header >> 8) & 0xFF);
			Uuid = formatter[1];

			Subscribers = DeserializeInfoList<SubscriberInfo>(formatter[2]);
			Publishers  = DeserializeInfoList<PublisherInfo>(formatter[3]);
			Topics      = DeserializeInfoList<TopicInfo>(formatter[4]);
		}

		DataFormatter[] SerializeInfoList(TopicData[] infoList)
		{
			return infoList.Select(info => info.SerializeData()).ToArray();
		}

		T[] DeserializeInfoList<T>(dynamic formatterList)
			where T : TopicData, new()
		{
			return ((DataFormatter[])formatterList)
				.Select(f => TopicData.DeserializeData<T>(f))
				.ToArray();
		}
	}
}

