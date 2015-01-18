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
using Xwt.Formats;
using Xwt.Drawing;

namespace EaChat
{
	public partial class MainWindow
	{
		ListStore chatStore;
		DataField<int>    publishersCol;
		DataField<Image>  publisherImgCol;
		DataField<int>    subscribersCol;
		DataField<Image>  subscriberImgCol;
		DataField<string> chatNameCol;

		ParticipantController controller;

		public MainWindow()
		{
			CreateComponents();

			publishersCol    = new DataField<int>();
			publisherImgCol  = new DataField<Image>();
			subscribersCol   = new DataField<int>();
			subscriberImgCol = new DataField<Image>();
			chatNameCol      = new DataField<string>();

			chatStore = new ListStore(publishersCol, publisherImgCol, subscribersCol, 
				subscriberImgCol, chatNameCol);
			chatList.DataSource = chatStore;
			chatList.Columns.Add("Publishers", publisherImgCol, publishersCol);
			chatList.Columns.Add("Subscribers", subscriberImgCol, subscribersCol);
			chatList.Columns.Add("Name", chatNameCol);
			chatList.SelectionChanged += HandleChatSelected;

			string userName = AskUserName();
			if (string.IsNullOrEmpty(userName))
				Application.Exit();

			controller = new ParticipantController(userName, this);

			addChatBtn.Clicked += HandleAddChat;
			remChatBtn.Clicked += HandleRemChat;

			CloseRequested += HandleCloseRequested;
		}

		void HandleAddChat(object sender, EventArgs e)
		{
			var chatName = AskChatName();

			if (!string.IsNullOrEmpty(chatName))
				RequestChat(chatName);
		}

		void HandleRemChat(object sender, EventArgs e)
		{
			chatTabs.CurrentTab.Child.Dispose();
			chatTabs.Tabs.Remove(chatTabs.CurrentTab);
			if (chatTabs.Tabs.Count == 0)
				remChatBtn.Sensitive = false;
		}

		public void UpdateChatInfo(string chatName, int numPub, int numSub)
		{
			int row = FindRow(chatName);
			if (row == -1)
				AddChatInfo(chatName, numPub, numSub);
			else
				SetChatInfo(row, numPub, numSub);
		}

		int FindRow(string chatName)
		{
			for (int i = 0; i < chatStore.RowCount; i++)
				if (chatStore.GetValue(i, chatNameCol) == chatName)
					return i;

			return -1;
		}

		void AddChatInfo(string chatName, int numPub, int numSub)
		{
			remChatBtn.Sensitive = true;
			int row = chatStore.AddRow();
			chatStore.SetValues(row,
				publishersCol,    numPub,
				publisherImgCol,  Image.FromResource("EaChat.res.arrow_up.png"),
				subscribersCol,   numSub,
				subscriberImgCol, Image.FromResource("EaChat.res.arrow_down.png"),
				chatNameCol,      chatName
			);
		}

		void SetChatInfo(int row, int numPub, int numSub)
		{
			chatStore.SetValue(row, publishersCol, numPub);
			chatStore.SetValue(row, subscribersCol, numSub);
		}

		void HandleChatSelected(object sender, EventArgs e)
		{
			string chatName = chatStore.GetValue(chatList.SelectedRow, chatNameCol);
			RequestChat(chatName);
		}

		void RequestChat(string chatName)
		{
			if (!controller.IsTopicOpened(chatName))
				CreateChat(chatName);
			else
				SelectChat(chatName);
		}

		void CreateChat(string chatName)
		{
			chatTabs.Add(new ChatBoxView(controller, chatName), chatName);
		}

		void SelectChat(string chatName)
		{
			foreach (var tab in chatTabs.Tabs)
				if (tab.Label == chatName)
					chatTabs.CurrentTab = tab;
		}

		void HandleCloseRequested(object sender, CloseRequestedEventArgs args)
		{
			Application.Exit();
		}
	}
}

