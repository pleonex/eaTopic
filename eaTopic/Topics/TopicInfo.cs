//
//  TopicInfo.cs
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

namespace EaTopic.Topics
{
	public class TopicInfo : EntityInfo
	{
		public TopicInfo()
		{
		}

		internal TopicInfo(dynamic topic)
		{
			TopicName = topic.Name;
			TopicType = topic.DataType;
		}

		public string TopicName { get; set; }

		public TopicDataType TopicType { get; set; }

		public override TopicDataType Type {
			get { return TopicDataType.FromGeneric<byte[], string, TopicDataType>(); }
		}

		public override void SerializeData(DataFormatter formatter)
		{
			formatter[0] = Uuid;
			formatter[1] = TopicName;
			formatter[2] = TopicType;
		}

		public override void DeserializeData(DataFormatter formatter)
		{
			Uuid = formatter[0];
			TopicName = formatter[1];
			TopicType = formatter[2];
		}
	}
}

