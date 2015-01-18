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
	public delegate void TopicDiscoveredEventHandler(TopicInfo topicInfo, BuiltinEventArgs e);
	public delegate void PublisherDiscoveredEventHandler(PublisherInfo pubInfo, BuiltinEventArgs e);
	public delegate void SubscriberDiscoveredEventHandler(SubscriberInfo subInfo, BuiltinEventArgs e);

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

		public event PublisherDiscoveredEventHandler  PublisherDiscovered;
		public event SubscriberDiscoveredEventHandler SubscriberDiscovered;
		public event TopicDiscoveredEventHandler TopicDiscovered;

		public byte Domain {
			get;
			private set;
		}

		public IEnumerable<TopicInfo> GetTopics()
		{
			return cache.Values.Select(v => v.Topic);
		}

		public IEnumerable<PublisherInfo> GetPublishers(TopicInfo topic)
		{
			var key = FindCacheKey(topic);
			if (key == null)
				return new PublisherInfo[0];

			return cache[key].Publishers;
		}

		public IEnumerable<SubscriberInfo> GetSubscribers(TopicInfo topic)
		{
			var key = FindCacheKey(topic);
			if (key == null)
				return new SubscriberInfo[0];

			return cache[key].Subscribers;
		}

		TopicInfo FindCacheKey(TopicInfo topic)
		{
			return cache.Keys.FirstOrDefault(t => t.TopicName == topic.TopicName);
		}

		void OnReceivedInfo(ParticipantInfo instance)
		{
			if (instance.Domain != Domain)
				return;

			foreach (var topic in GetValidTopicKeys(instance.Topics)) {
				UpdatePublishers(topic, instance.Publishers, instance.Uuid);
				UpdateSubscribers(topic, instance.Subscribers, instance.Uuid);
			}
		}

		IEnumerable<TopicInfo> GetValidTopicKeys(TopicInfo[] infos)
		{
			List<TopicInfo> valid = new List<TopicInfo>();

			foreach (var topicInfo in infos) {
				if (InitializeTopic(topicInfo))
					valid.Add(FindCacheKey(topicInfo));
			}

			return valid;
		}

		bool InitializeTopic(TopicInfo info)
		{
			bool noError = true;

			// Search if exists
			var topicKey = FindCacheKey(info);

			// Does not exists, add
			if (topicKey == null)
				AddTopic(info);
			// Exists, check if the type is the same
			else
				noError = (info.TopicType == topicKey.TopicType);

			return noError;
		}

		void AddTopic(TopicInfo info)
		{
			cache.Add(info, new TopicEntitiesList(info));
			if (TopicDiscovered != null)
				TopicDiscovered(info, new BuiltinEventArgs(info, BuiltinEntityChange.Added));
		}

		void UpdatePublishers(TopicInfo topic, PublisherInfo[] publishers, byte[] partUuid)
		{
			var topicPublishers = publishers.Where(pub => pub.TopicName == topic.TopicName);
			foreach (var pub in topicPublishers) {
				pub.ParticipantUuid = partUuid;
				var change = cache[topic].SetPublisher(pub);
				if (PublisherDiscovered != null)
					PublisherDiscovered(pub, new BuiltinEventArgs(topic, change));
			}

			var pubToRemove = cache[topic].Publishers
				.Where(pub => !publishers.Any(newPub => newPub.Uuid.SequenceEqual(pub.Uuid)))
				.ToArray();
			foreach (var pub in pubToRemove) {
				cache[topic].RemovePublisher(pub);
				if (PublisherDiscovered != null)
					PublisherDiscovered(pub, new BuiltinEventArgs(topic, BuiltinEntityChange.Removed));
			}
		}

		void UpdateSubscribers(TopicInfo topic, SubscriberInfo[] subscribers, byte[] partUuid)
		{
			var topicSubscribers = subscribers.Where(sub => sub.TopicName == topic.TopicName);
			foreach (var sub in topicSubscribers) {
				sub.ParticipantUuid = partUuid;
				var change = cache[topic].SetSubscriber(sub);
				if (SubscriberDiscovered != null)
					SubscriberDiscovered(sub, new BuiltinEventArgs(topic, change));
			}

			var subToRemove = cache[topic].Subscribers
				.Where(sub => !subscribers.Any(newSub => newSub.Uuid.SequenceEqual(sub.Uuid)))
				.ToArray();
			foreach (var sub in subToRemove) {
				cache[topic].RemoveSubscriber(sub);
				if (SubscriberDiscovered != null)
					SubscriberDiscovered(sub, new BuiltinEventArgs(topic, BuiltinEntityChange.Removed));
			}
		}

		Subscriber<ParticipantInfo> Subscriber {
			get;
			set;
		}

		class TopicEntitiesList
		{
			List<PublisherInfo> publishers;
			List<SubscriberInfo> subscribers;

			public TopicEntitiesList(TopicInfo topic)
			{
				Topic = topic;
				publishers  = new List<PublisherInfo>();
				subscribers = new List<SubscriberInfo>();
			}

			public TopicInfo Topic {
				get;
				private set;
			}

			public ReadOnlyCollection<PublisherInfo> Publishers { 
				get { return new ReadOnlyCollection<PublisherInfo>(publishers); }
			}
			public ReadOnlyCollection<SubscriberInfo> Subscribers {
				get { return new ReadOnlyCollection<SubscriberInfo>(subscribers); }
			}

			public BuiltinEntityChange SetPublisher(PublisherInfo info)
			{
				int idx = publishers.FindIndex(i => i.Uuid.SequenceEqual(info.Uuid));
				if (idx == -1) {
					publishers.Add(info);
					return BuiltinEntityChange.Added;
				} else {
					publishers[idx] = info;
					return BuiltinEntityChange.Modified;
				}
			}

			public void RemovePublisher(PublisherInfo info)
			{
				int idx = publishers.FindIndex(i => i.Uuid.SequenceEqual(info.Uuid));
				publishers.RemoveAt(idx);
			}

			public BuiltinEntityChange SetSubscriber(SubscriberInfo info)
			{
				int idx = subscribers.FindIndex(i => i.Uuid.SequenceEqual(info.Uuid));
				if (idx == -1) {
					subscribers.Add(info);
					return BuiltinEntityChange.Added;
				} else {
					subscribers[idx] = info;
					return BuiltinEntityChange.Modified;
				}
			}

			public void RemoveSubscriber(SubscriberInfo info)
			{
				int idx = subscribers.FindIndex(i => i.Uuid.SequenceEqual(info.Uuid));
				subscribers.RemoveAt(idx);
			}
		}
	}
}

