//
//  ParticipantController.cs
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
using System.Linq;
using EaTopic.Participants;
using EaTopic.Participants.Builtin;
using EaTopic.Publishers;
using EaTopic.Subscribers;
using EaTopic.Topics;
using System.Collections.ObjectModel;

namespace EaChat
{
	public class ParticipantController
	{
		Participant participant;
		Dictionary<string, TopicEntities> topics;

		public ParticipantController(string userName, MainWindow window)
		{
			participant = new Participant(0);
			participant.BuiltinTopic.PublisherDiscovered  += HandlePublisherDiscovered;
			participant.BuiltinTopic.SubscriberDiscovered += HandleSubscriberDiscovered;
			topics = new Dictionary<string, TopicEntities>();

			UserName   = userName;
			MainWindow = window;
		}

		public string UserName {
			get;
			set;
		}

		public MainWindow MainWindow {
			get;
			private set;
		}

		public void CreateTopic(string topicName, ReceivedInstanceHandleEvent<ChatMessage> handle)
		{
			if (!topics.ContainsKey(topicName))
				topics.Add(topicName, new TopicEntities(topicName));

			var topic = participant.CreateTopic<ChatMessage>(topicName);
			topics[topicName].Topic = topic;

			topics[topicName].Publisher  = topic.CreatePublisher(UserName);
			topics[topicName].Subscriber = topic.CreateSubscriber(UserName);
			topics[topicName].Subscriber.ReceivedInstance += handle;
		}

		public bool IsTopicOpened(string topicName)
		{
			if (!topics.ContainsKey(topicName))
				return false;

			return topics[topicName].Publisher != null;
		}

		public void Send(string text, string topicName)
		{
			if (string.IsNullOrEmpty(topicName) || !topics.ContainsKey(topicName))
				return;

			var message = new ChatMessage(UserName, text, DateTime.Now);
			topics[topicName].Publisher.Write(message);
		}

		void HandlePublisherDiscovered(PublisherInfo pubInfo, BuiltinEventArgs e)
		{
			string topicName = pubInfo.TopicName;
			if (!topics.ContainsKey(topicName))
				topics.Add(topicName, new TopicEntities(e.Topic));

			if (e.Change == BuiltinEntityChange.Added)
				topics[topicName].AddPublisherInfo(pubInfo);
			else if (e.Change == BuiltinEntityChange.Removed)
				topics[topicName].RemovePublisherInfo(pubInfo);

			UpdateWindowChatInfo(topicName);
		}

		void HandleSubscriberDiscovered (SubscriberInfo subInfo, BuiltinEventArgs e)
		{
			string topicName = subInfo.TopicName;
			if (!topics.ContainsKey(topicName))
				topics.Add(topicName, new TopicEntities(e.Topic));

			if (e.Change == BuiltinEntityChange.Added)
				topics[topicName].AddSubscriberInfo(subInfo);
			else if (e.Change == BuiltinEntityChange.Removed)
				topics[topicName].RemoveSubscriberInfo(subInfo);

			UpdateWindowChatInfo(topicName);
		}

		void UpdateWindowChatInfo(string topicName)
		{
			MainWindow.UpdateChatInfo(
				topicName,
				topics[topicName].PublishersInfo.Count,
				topics[topicName].SubscribersInfo.Count
			);
		}

		class TopicEntities
		{
			readonly List<PublisherInfo> publishersInfo;
			readonly List<SubscriberInfo> subscribersInfo;

			public TopicEntities(string topicName)
				: this(new TopicInfo { TopicName = topicName, TopicType = TopicDataType.FromGeneric<ChatMessage>() })
			{

			}

			public TopicEntities(TopicInfo topicInfo)
			{
				TopicInfo = topicInfo;
				publishersInfo = new List<PublisherInfo>();
				subscribersInfo = new List<SubscriberInfo>();
			}

			public Topic<ChatMessage> Topic { get; set; }
			public Publisher<ChatMessage> Publisher { get; set; }
			public Subscriber<ChatMessage> Subscriber { get; set; }

			public TopicInfo TopicInfo { get; private set; }

			public ReadOnlyCollection<PublisherInfo> PublishersInfo { 
				get {
					return new ReadOnlyCollection<PublisherInfo>(publishersInfo);
				}
			}

			public ReadOnlyCollection<SubscriberInfo> SubscribersInfo { 
				get {
					return new ReadOnlyCollection<SubscriberInfo>(subscribersInfo);
				}
			}

			public void AddPublisherInfo(PublisherInfo info)
			{
				publishersInfo.Add(info);
			}

			public void RemovePublisherInfo(PublisherInfo info)
			{
				int idx = publishersInfo.FindIndex(p => p.Uuid.SequenceEqual(info.Uuid));
				if (idx != -1)
					publishersInfo.RemoveAt(idx);
			}

			public void AddSubscriberInfo(SubscriberInfo info)
			{
				subscribersInfo.Add(info);
			}

			public void RemoveSubscriberInfo(SubscriberInfo info)
			{
				int idx = subscribersInfo.FindIndex(s => s.Uuid.SequenceEqual(info.Uuid));
				if (idx != -1)
					subscribersInfo.RemoveAt(idx);
			}
		}
	}
}

