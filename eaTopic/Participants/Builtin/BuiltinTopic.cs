//
//  BuiltinTopic.cs
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
using EaTopic.Publishers;
using EaTopic.Subscribers;
using EaTopic.Topics;

namespace EaTopic.Participants.Builtin
{
	public class BuiltinTopic
	{
		const string Name = "BuiltinTopic";

		Topic<ParticipantInfo> topic;
		Subscriber<ParticipantInfo> subscriber;
		Publisher<ParticipantInfo> publisher;

		internal BuiltinTopic(Participant participant)
		{
			this.Participant = participant;
			topic = new Topic<ParticipantInfo>(participant, Name, true);
			subscriber = topic.CreateSubscriber();
			publisher  = topic.CreatePublisher();
		}

		internal string MulticastAddress {
			get { return "239.0.0.225"; }
		}

		internal int MulticastPort {
			get { return 14322 + Participant.Domain; }
		}

		public Participant Participant { get; private set; }
	}
}

