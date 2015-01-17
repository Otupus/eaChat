//
//  MainWindow.Designer.cs
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
using Xwt.Drawing;

namespace EaChat
{
	public partial class MainWindow : Window
	{
		ListView chatList;
		Button addChatBtn;
		Button remChatBtn;

		Notebook chatTabs;

		void CreateComponents()
		{
			Title  = "eaChat - eaTopic IRC";
			Width  = 800;
			Height = 600;

			var hpaned = new HPaned();
			hpaned.BackgroundColor = Colors.LightSteelBlue;

			var listBox = new VBox();
			hpaned.Panel1.Content = listBox;

			chatList = new ListView();
			chatList.WidthRequest = 150;
			chatList.HeadersVisible = false;
			chatList.Margin = 5;
			listBox.PackStart(chatList, true, true);

			var btnBox = new HBox();
			btnBox.Margin = 5;
			btnBox.MarginTop = 0;
			listBox.PackStart(btnBox, false, hpos: WidgetPlacement.Fill);

			addChatBtn = new Button(StockIcons.Add);
			btnBox.PackStart(addChatBtn);

			remChatBtn = new Button(StockIcons.Remove);
			btnBox.PackStart(remChatBtn);

			chatTabs = new Notebook();
			chatTabs.WidthRequest = 650;
			hpaned.Panel2.Content = chatTabs;

			Padding = new WidgetSpacing();
			Content = hpaned;
		}

		string AskUserName()
		{
			return AskTextDialog("Type your user name", "User name:");
		}

		string AskChatName()
		{
			return AskTextDialog("Type the new chat name", "Chat name:");
		}

		string AskTextDialog(string message, string label)
		{
			Dialog dialog = new Dialog();

			dialog.Title = message;
			dialog.Buttons.Add(new DialogButton(Command.Ok));

			Table dialogTable = new Table();
			dialog.Content = dialogTable;

			var textEntry = new TextEntry();
			dialogTable.Add(textEntry, 1, 0);
			dialogTable.Add(new Label(label), 0, 0);

			dialog.Run(this);
			string text = textEntry.Text;

			dialog.Dispose();
			return text;
		}
	}
}

