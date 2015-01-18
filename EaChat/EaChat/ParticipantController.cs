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
		BuiltinTopic builtinTopic;
		Dictionary<string, TopicEntities> topics;

		public ParticipantController(string userName, MainWindow window)
		{
			topics = new Dictionary<string, TopicEntities>();
			participant = new Participant(0);

			builtinTopic = participant.BuiltinTopic;
		    builtinTopic.PublisherDiscovered  += HandlePublisherDiscovered;
			builtinTopic.SubscriberDiscovered += HandleSubscriberDiscovered;
			builtinTopic.TopicDiscovered += HandleTopicDiscovered;

			UserName   = userName;
			MainWindow = window;
		}

		public string UserName {
			get;
			private set;
		}

		public MainWindow MainWindow {
			get;
			private set;
		}

		public event SubscriberDiscoveredEventHandler SubscriberDiscovered {
			add    { builtinTopic.SubscriberDiscovered += value; }
			remove { builtinTopic.SubscriberDiscovered -= value; }
		}

		public void CreateTopic(string topicName, ReceivedInstanceHandleEvent<ChatMessage> handle)
		{
			var topic = participant.CreateTopic<ChatMessage>(topicName);

			var publisher  = topic.CreatePublisher(UserName);
			var subscriber = topic.CreateSubscriber(UserName);
			subscriber.ReceivedInstance += handle;

			topics.Add(topicName, new TopicEntities(topic, publisher, subscriber));
		}

		public void CloseTopic(string topicName)
		{
			if (!topics.ContainsKey(topicName))
				return;

			topics[topicName].Topic.Dispose();
			topics.Remove(topicName);
		}

		public bool IsTopicOpened(string topicName)
		{
			return topics.ContainsKey(topicName);
		}

		public void Send(string text, string topicName)
		{
			if (string.IsNullOrEmpty(topicName) || !topics.ContainsKey(topicName))
				return;

			var message = new ChatMessage(UserName, text, DateTime.Now);
			topics[topicName].Publisher.Write(message);
		}

		public void SetTextFilter(string text, string topicName)
		{
			Filter filter = new Filter(topics[topicName].Topic.DataType);
			filter.AddCondition(1, FilterCondition.Contains, text);
			topics[topicName].Subscriber.SetLocalFilter(filter);
		}

		public void SetUserFilter(string name, string topicName)
		{
			Filter filter = new Filter(topics[topicName].Topic.DataType);
			filter.AddCondition(0, FilterCondition.Equals, name);
			topics[topicName].Subscriber.SetLocalFilter(filter);
		}

		public void UnsetFilter(string topicName)
		{
			topics[topicName].Subscriber.RemoveLocalFilter();
		}

		public IEnumerable<SubscriberInfo> GetSubscribers(string topicName)
		{
			var topicInfo = new TopicInfo { 
				TopicName = topicName,
				TopicType = TopicDataType.FromGeneric<ChatMessage>()
			};

			return builtinTopic.GetSubscribers(topicInfo);
		}

		public IEnumerable<PublisherInfo> GetPublishers(string topicName)
		{
			var topicInfo = new TopicInfo {
				TopicName = topicName,
				TopicType = TopicDataType.FromGeneric<ChatMessage>()
			};

			return builtinTopic.GetPublishers(topicInfo);
		}

		void HandleTopicDiscovered(TopicInfo topicInfo, BuiltinEventArgs e)
		{
			UpdateWindowChatInfo(topicInfo);
		}

		void HandlePublisherDiscovered(PublisherInfo pubInfo, BuiltinEventArgs e)
		{
			UpdateWindowChatInfo(e.Topic);
		}

		void HandleSubscriberDiscovered (SubscriberInfo subInfo, BuiltinEventArgs e)
		{
			UpdateWindowChatInfo(e.Topic);
		}
			
		void UpdateWindowChatInfo(TopicInfo topicInfo)
		{
			MainWindow.UpdateChatInfo(
				topicInfo.TopicName,
				builtinTopic.GetPublishers(topicInfo).Count(),
				builtinTopic.GetSubscribers(topicInfo).Count()
			);
		}

		struct TopicEntities
		{
			public TopicEntities(Topic<ChatMessage> topic, Publisher<ChatMessage> publisher,
				Subscriber<ChatMessage> subscriber) : this()
			{
				this.Topic = topic;
				this.Publisher  = publisher;
				this.Subscriber = subscriber;
			}

			public Topic<ChatMessage> Topic { get; private set; }
			public Publisher<ChatMessage> Publisher { get; private set; }
			public Subscriber<ChatMessage> Subscriber { get; private set; }
		}
	}
}
