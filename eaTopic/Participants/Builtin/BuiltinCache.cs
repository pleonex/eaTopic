//
//  BuiltinCache.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using EaTopic.Publishers;
using EaTopic.Subscribers;
using EaTopic.Topics;

namespace EaTopic.Participants.Builtin
{
	internal class BuiltinCache
	{
		Dictionary<TopicInfo, TopicEntitiesList> cache;

		public BuiltinCache(byte domain, Subscriber<ParticipantInfo> subscriber)
		{
			cache = new Dictionary<TopicInfo, TopicEntitiesList>();

			Domain = domain;
			Subscriber = subscriber;
			Subscriber.ReceivedInstance += OnReceivedInfo;
		}

		public byte Domain {
			get;
			private set;
		}

		public PublisherInfo[] GetPublishers(TopicInfo topic)
		{
			var key = FindCacheKey(topic);
			if (key == null)
				return new PublisherInfo[0];

			return cache[key].Publishers.Cast<PublisherInfo>().ToArray();
		}

		public SubscriberInfo[] GetSubscribers(TopicInfo topic)
		{
			var key = FindCacheKey(topic);
			if (key == null)
				return new SubscriberInfo[0];

			return cache[key].Subscribers.Cast<SubscriberInfo>().ToArray();
		}

		TopicInfo FindCacheKey(TopicInfo topic)
		{
			return cache.Keys.FirstOrDefault(t => t.TopicName == topic.TopicName);
		}

		void OnReceivedInfo(ParticipantInfo instance)
		{
			if (instance.Domain != Domain)
				return;

			var topicInfos = GetValidTopics(instance.Topics);
			foreach (var topic in topicInfos) {
				cache[topic].AddPublishers(
					instance.Publishers
					.Where(pub => pub.TopicName == topic.TopicName)
					.ToArray());
				cache[topic].AddSubscribers(
					instance.Subscribers
					.Where(sub => sub.TopicName == topic.TopicName)
					.ToArray());
			}
		}

		TopicInfo[] GetValidTopics(TopicInfo[] infos)
		{
			List<TopicInfo> valid = new List<TopicInfo>();

			foreach (var topicInfo in infos) {
				if (InitializeTopic(topicInfo))
					valid.Add(topicInfo);
			}

			return valid.ToArray();
		}

		bool InitializeTopic(TopicInfo info)
		{
			bool noError = true;

			// Search if exists
			var topicKey = FindCacheKey(info);

			// Does not exists, add
			if (topicKey == null)
				cache.Add(info, new TopicEntitiesList());
			// Exists, check if the type is the same
			else
				noError = (info.TopicType == topicKey.TopicType);

			return noError;
		}

		Subscriber<ParticipantInfo> Subscriber {
			get;
			set;
		}

		class TopicEntitiesList
		{
			List<EntityInfo> publishers;
			List<EntityInfo> subscribers;

			public TopicEntitiesList()
			{
				publishers = new List<EntityInfo>();
				subscribers = new List<EntityInfo>();
			}

			public ReadOnlyCollection<EntityInfo> Publishers { 
				get { return new ReadOnlyCollection<EntityInfo>(publishers); }
			}
			public ReadOnlyCollection<EntityInfo> Subscribers {
				get { return new ReadOnlyCollection<EntityInfo>(subscribers); }
			}

			public void AddPublisher(EntityInfo info)
			{
				if (!publishers.Any(i => i.Uuid.SequenceEqual(info.Uuid)))
					publishers.Add(info);
			}

			public void AddPublishers(EntityInfo[] infos)
			{
				foreach (var pub in infos)
					AddPublisher(pub);
			}

			public void AddSubscriber(EntityInfo info)
			{
				if (!subscribers.Any(i => i.Uuid.SequenceEqual(info.Uuid)))
					subscribers.Add(info);
			}

			public void AddSubscribers(EntityInfo[] infos)
			{
				foreach (var sub in infos)
					AddSubscriber(sub);
			}
		}
	}
}

