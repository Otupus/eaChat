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
		RichTextView chatView;
		TextEntry chatTextEntry;
		Button chatSendBtn;

		void CreateComponents()
		{
			Title  = "eaChat - eaTopic IRC";
			Width  = 800;
			Height = 600;

			var hpaned = new HPaned();
			hpaned.BackgroundColor = Colors.LightSteelBlue;

			chatList = new ListView();
			chatList.Margin = 5;
			hpaned.Panel1.Content = chatList;

			// Chat controls
			var chatBox = new VBox();
			hpaned.Panel2.Content= chatBox;

			chatView = new RichTextView();
			chatView.Margin = 5;
			chatBox.PackStart(chatView, true, true);

			var chatMsgBox = new HBox();
			chatMsgBox.Margin = 5;
			chatBox.PackStart(chatMsgBox);

			chatTextEntry = new TextEntry();
			chatMsgBox.PackStart(chatTextEntry, true);

			chatSendBtn = new Button("Send!");
			chatMsgBox.PackStart(chatSendBtn, vpos: WidgetPlacement.Center);

			Padding = new WidgetSpacing();
			Content = hpaned;
		}
	}
}

