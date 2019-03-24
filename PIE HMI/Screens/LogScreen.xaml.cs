using PIE_HMI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PIE_HMI.Screens
{
    /// <summary>
    /// Interaction logic for LogScreen.xaml
    /// </summary>
    /// 
    public partial class LogScreen : UserControl
    {
        private static List<Message> messages;
        private static Queue<Message> messageQueue;

        public static event Action<Message> PushMessageHandler;

        public LogScreen()
        {
            InitializeComponent();
            messages = new List<Message>();
            messageQueue = new Queue<Message>();
            PushMessageHandler += LogScreen_PushMessageHandler;
        }

        private void LogScreen_PushMessageHandler(Message obj)
        {
            while (messageQueue.Count > 0)
            {
                Message message = messageQueue.Dequeue();
                TextRange tr = new TextRange(messageBox.Document.ContentEnd, messageBox.Document.ContentEnd);
                tr.Text = message.message;

                switch (message.type)
                {
                    case MessageType.Note:
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                        break;
                    case MessageType.Warning:
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
                        break;
                    case MessageType.Error:
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                        break;
                }

                messageBox.AppendText(Environment.NewLine);
            }
        }

        public static void PushMessage(Message message)
        {
            messages.Add(message);
            messageQueue.Enqueue(message);
            PushMessageHandler(message);
        }

        public static void PushMessage(string message, MessageType messageType)
        {
            Message _message;
            _message.message = message;
            _message.type = messageType;

            PushMessage(_message);
        }

        public static List<Message> GetMessages()
        {
            return messages;
        }

        private void Logscreen_Loaded(object sender, RoutedEventArgs e)
        {

            messageBox.Document.Blocks.Clear();

            for (int i = 0; i < messages.Count; i++)
            {
                Message message = messages[i];
                TextRange tr = new TextRange(messageBox.Document.ContentEnd, messageBox.Document.ContentEnd);
                tr.Text = message.message;

                switch (message.type)
                {
                    case MessageType.Note:
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                        break;
                    case MessageType.Warning:
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
                        break;
                    case MessageType.Error:
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                        break;
                }

                messageBox.AppendText(Environment.NewLine);
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            messageBox.Document.Blocks.Clear();
        }
    }
}
