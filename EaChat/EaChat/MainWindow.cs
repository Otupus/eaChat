//
//  MainWindow.cs
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
using Xwt;
using EaTopic.Topics;
using EaTopic.Participants;
using EaTopic.Publishers;
using EaTopic.Subscribers;
using EaTopic.Participants.Builtin;
using Xwt.Formats;

namespace EaChat
{
	public partial class MainWindow
	{
		ListStore chatStore;
		DataField<int> publishersCol;
		DataField<int> subscribersCol;
		DataField<string> chatNameCol;
		DataField<TopicInfo> topicCol;

		string chatText;

		Participant participant;
		Publisher<ChatMessage> publisher;
		Subscriber<ChatMessage> subscriber;

		public MainWindow()
		{
			CreateComponents();

			publishersCol  = new DataField<int>();
			subscribersCol = new DataField<int>();
			chatNameCol    = new DataField<string>();
			topicCol  = new DataField<TopicInfo>();

			chatStore = new ListStore(publishersCol, subscribersCol, chatNameCol, topicCol);
			chatList.DataSource = chatStore;
			chatList.Columns.Add("Publishers", publishersCol);
			chatList.Columns.Add("Subscribers", subscribersCol);
			chatList.Columns.Add("Name", chatNameCol);

			participant = new Participant(0);
			participant.BuiltinTopic.PublisherDiscovered += HandlePublisherDiscovered;
			participant.BuiltinTopic.SubscriberDiscovered += HandleSubscriberDiscovered;

			chatText  = "";
			var topic = participant.CreateTopic<ChatMessage>("DevChat");
			publisher  = topic.CreatePublisher();
			subscriber = topic.CreateSubscriber();
			subscriber.ReceivedInstance += HandleReceivedInstance;

			chatTextEntry.KeyPressed += HandleKeySendPressed;
			chatSendBtn.Clicked += HandleSend;
			CloseRequested += HandleCloseRequested;
		}

		void HandleKeySendPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
				Send();
		}

		void HandleSend(object sender, EventArgs e)
		{
			Send();
		}

		void Send()
		{
			var message = new ChatMessage("pleonex", chatTextEntry.Text, DateTime.Now);
			publisher.Write(message);
			chatTextEntry.Text = "";
		}

		void HandleReceivedInstance(ChatMessage instance)
		{
			chatText += string.Format("[{0}] {1}: {2}\n", 
				instance.UserName, instance.Date, instance.Message);
			chatView.LoadText(chatText, TextFormat.Plain);
		}

		void UpdateChatList(TopicInfo topic)
		{
			int row = chatStore.AddRow();
			chatStore.SetValues(row,
				publishersCol, participant.BuiltinTopic.GetPublishers(topic).Length,
				subscribersCol, participant.BuiltinTopic.GetSubscribers(topic).Length,
				chatNameCol, topic.TopicName,
				topicCol, topic
			);
		}

		int FindRow(string topicName)
		{
			for (int i = 0; i < chatStore.RowCount; i++)
				if (chatStore.GetValue(i, topicCol).TopicName == topicName)
					return i;

			return -1;
		}

		void HandlePublisherDiscovered(PublisherInfo pubInfo, BuiltinEventArgs e)
		{
			if (e.Change == BuiltinEntityChange.Added) {
				int row = FindRow(pubInfo.TopicName);
				if (row != -1) {
					int currValue = chatStore.GetValue(row, publishersCol);
					chatStore.SetValue(row, publishersCol, currValue + 1);
				} else {
					UpdateChatList(e.Topic);
				}
			}
		}

		void HandleSubscriberDiscovered (SubscriberInfo subInfo, BuiltinEventArgs e)
		{
			if (e.Change == BuiltinEntityChange.Added) {
				int row = FindRow(subInfo.TopicName);
				if (row != -1) {
					int currValue = chatStore.GetValue(row, subscribersCol);
					chatStore.SetValue(row, subscribersCol, currValue + 1);
				} else {
					UpdateChatList(e.Topic);
				}
			}
		}

		void HandleCloseRequested(object sender, CloseRequestedEventArgs args)
		{
			Application.Exit();
		}
	}
}

