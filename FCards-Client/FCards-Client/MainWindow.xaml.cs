using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace FCards_Client
{
    public partial class MainWindow : Window
    {
        public Label info;
        public Button action;
        public List<Button> cards;
        private Socket sender;
        public MainWindow()
        {
            InitializeComponent();
            info = new Label();
            info.Width = 400;
            info.Height = 30;
            info.Margin = new Thickness(-300, 500, 0, 0);
            info.Visibility = Visibility.Hidden;
            MAIN.Children.Add(info);
            action = new Button();
            action.Width = 100;
            action.Height = 30;
            action.Margin = new Thickness(600, 500, 0, 0);
            action.Visibility = Visibility.Hidden;
            MAIN.Children.Add(action);
            cards = new List<Button>();
            for (int i = 1;i <= 50;i++)
            {
                Image img = new Image();
                Button btn = new Button();
                img.Source = new BitmapImage(new Uri("Resources/" + Cards.GetImageName(i - 1), UriKind.Relative));
                btn.BorderBrush = Brushes.Transparent;
                btn.Background = Brushes.Transparent;
                btn.Width = img.Width = 100;
                btn.Height = img.Height = 150;
                btn.Content = img;
                btn.Visibility = Visibility.Hidden;
                cards.Add(btn);
                MAIN.Children.Add(cards[i - 1]);
            }
        }

        private async void button_Click(object s, RoutedEventArgs e)
        {
            try
            {
                START.IsEnabled = false;
                START.Content = "Подключение к серверу...";
                sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27015)); // 82.146.32.134
                START.Content = "Соединение установлено!";
                await Task.Delay(2000);
                START.Visibility = Visibility.Hidden;
                LOGO.Visibility = Visibility.Hidden;
                (new Game(this, sender)).Start();
            }
            catch (Exception ex)
            {
                START.Content = "Ошибка: " + ex.Message;
            }
        }
    }
}
