using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Threading;

namespace FCards_Client
{
    class Game
    {
        private BackgroundWorker worker;
        private MainWindow _this;
        private Socket sender;
        private int Status;
        public Game(MainWindow t, Socket s)
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(workerF);
            _this = t;
            sender = s;
            Status = 0;
        }

        private List<List<string>> StrDecoder(string msg)
        {
            List<List<string>> ot = new List<List<string>>();
            string m = Regex.Match(msg, @"^\[\[(.*)\]\]$").Groups[1].Value;
            List<string> z = m.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string v in z)
                ot.Add(v.Split(',').ToList());
            return ot;
        }

        private void Click(object s, RoutedEventArgs e)
        {
            if(Convert.ToBoolean(Status))
            {
                SocketException ex = new SocketException();
                string _out = string.Empty;
                if (s == _this.action)
                    _out = "[[1||0]]";
                else
                    _out = "[[2||" + _this.cards.FindIndex(x => x == s) + "]]";
                SocketFunctions.Send(sender, _out, ref ex);
                SocketCheckError(ex);
            }
        }

        private void SocketCheckError(SocketException ex)
        {
            if (ex.ErrorCode != 0)
            {
                Status = 0;
                _this.Dispatcher.BeginInvoke((Action)delegate () {
                    _this.info.Content = "Ошибка: " + ex.Message;
                    _this.info.Visibility = Visibility.Visible;
                });
            }
        }

        private void UpdateForm(string str)
        {
            List<List<string>> ot = StrDecoder(str);
            _this.Dispatcher.BeginInvoke((Action)delegate () {
                for (int i = 1; i <= 50; i++)
                    _this.cards[i - 1].Visibility = Visibility.Hidden;
                switch (ot[0][0])
                {
                    case "1":
                        _this.info.Content = "Игра найдена! Идет подготовка к игре...";
                        break;
                    case "2":
                        _this.info.Content = "Ваш номер: " + ot[1][0];
                        if (Convert.ToInt32(ot[2][1]) >= 1)
                        {
                            int Trump = Convert.ToInt32(ot[2][0]);
                            _this.cards[Trump].Margin = new Thickness(-600, -100, 0, 0);
                            _this.cards[Trump].Visibility = Visibility.Visible;
                        }
                        if (Convert.ToInt32(ot[2][1]) >= 2)
                        {
                            _this.cards[37].Margin = new Thickness(-600, 0, 0, 0);
                            _this.cards[37].Visibility = Visibility.Visible;
                        }

                        int player = Convert.ToInt32(ot[1][0]) - 1;
                        int playerCardsCount = Convert.ToInt32(ot[4][player]);
                        int card;
                        for (int i = 0; i < playerCardsCount; i++)
                        {
                            card = Convert.ToInt32(ot[5][i]);
                            Panel.SetZIndex(_this.cards[card], i);
                            _this.cards[card].Margin = new Thickness(-100 + (i * 60), 300, 0, 0);
                            _this.cards[card].Visibility = Visibility.Visible;
                        }

                        player = player == 1 ? 0 : 1;
                        playerCardsCount = Convert.ToInt32(ot[4][player]);
                        for (int i = 0; i < playerCardsCount; i++)
                        {
                            Panel.SetZIndex(_this.cards[38+i], i);
                            _this.cards[38 + i].Margin = new Thickness(-100 + (i * 60), -400, 0, 0);
                            _this.cards[38 + i].Visibility = Visibility.Visible;
                        }

                        for(int i = 0;i < ot[3].Count && Convert.ToInt32(ot[3][0]) != -1; i++)
                        {
                            card = Convert.ToInt32(ot[3][i]);
                            Panel.SetZIndex(_this.cards[card], i);
                            _this.cards[card].Margin = new Thickness(-150 + (i * 60), -50, 0, 0);
                            _this.cards[card].Visibility = Visibility.Visible;
                        }
                        break;
                    case "3":
                        _this.info.Content = "Игра окончена! Победил игрок №" + ot[1][0] + "!";
                        break;
                    case "4":
                        _this.info.Content = "Один из клиентов потерял соединение! Игра окончена...";
                        break;
                }
            });
        }

        private void workerF(object s, DoWorkEventArgs e)
        {
            SocketException ex = new SocketException();
            string temp, r;
            temp = r = string.Empty;
            _this.Dispatcher.BeginInvoke((Action)delegate () {
                _this.action.Content = "Взять / Отбой";
                _this.action.Visibility = Visibility.Visible;
                _this.info.Content = "Поиск игры...";
                _this.info.Visibility = Visibility.Visible;
            });
            while (SocketFunctions.Receive(sender, ref r, ref ex) && Convert.ToBoolean(Status))
            {
                if (temp != r)
                {
                    UpdateForm(r);
                    temp = r;
                }
            }
            SocketCheckError(ex);
        }

        public void Start()
        {
            Status = 1;
            worker.RunWorkerAsync();
            _this.action.Click += Click;
            foreach (Button v in _this.cards)
                v.Click += Click;
        }
    }
}
