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
using System.Threading;

namespace EaTopic.Participants.Builtin
{
	public class BuiltinTopic
	{
		const string Name = "BuiltinTopic";
		const int PublishPeriod = 1000;

		Topic<ParticipantInfo> topic;
		Subscriber<ParticipantInfo> subscriber;
		BuiltinCache cache;

		Publisher<ParticipantInfo> publisher;
		Timer publishingTimer;

		internal BuiltinTopic(Participant participant)
		{
			this.Participant = participant;
			topic = new Topic<ParticipantInfo>(participant, Name, true);
			subscriber = topic.CreateSubscriber();
			publisher = topic.CreatePublisher();

			cache = new BuiltinCache(participant.Domain, subscriber);

			var state = new PublishTimeState(participant, publisher);
			publishingTimer = new Timer(PublishTick, state, 0, PublishPeriod);
		}

		internal string MulticastAddress {
			get { return "239.0.0.225"; }
		}

		internal int MulticastPort {
			get { return 57152 + Participant.Domain; }
		}

		public Participant Participant { get; private set; }

		public void Dispose()
		{
			publishingTimer.Change(Timeout.Infinite, Timeout.Infinite);
			publishingTimer.Dispose();
			topic.Dispose();
		}

		void PublishTick(object state)
		{
			var publishState = (PublishTimeState)state;

			publishState.Participant.UpdateInfo();
			publishState.Publisher.Write((ParticipantInfo)publishState.Participant.Info);
		}

		class PublishTimeState
		{
			public PublishTimeState(Participant participant, Publisher<ParticipantInfo> publisher)
			{
				this.Participant = participant;
				this.Publisher = publisher;
			}

			public Participant Participant { get; private set; }

			public Publisher<ParticipantInfo> Publisher { get; private set; }
		}
	}
}

