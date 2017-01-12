using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace FCards_Server
{
    class Game
    {
        private List<ClientSandbox> ClientsInGame;
        private List<int> CardsDeck;
        private int Trump;
        private int TrumpCard;
        private List<int> CardsTable;
        private List<List<int>> CardsHand;
        private int ActivePlayer;
        private List<string> ActiveCommand;
        private List<int> Results;
        public Game(ref List<ClientSandbox> Clients, List<int> Lobby)
        {
            ClientsInGame = new List<ClientSandbox>();
            CardsDeck = new List<int>();
            CreateCardDeck();
            Trump = GetCardClass(CardsDeck[0]);
            TrumpCard = CardsDeck[0];
            CardsTable = new List<int>();
            CardsHand = new List<List<int>>();
            foreach (int v in Lobby)
            {
                ClientsInGame.Add(Clients[v]);
                ClientsInGame[ClientsInGame.Count - 1].ExternalStatus = 4;
                ClientsInGame[ClientsInGame.Count - 1].RSEH += new ClientSandbox.ReceiveStringEventHandler(MsgFromClient);
                CardsHand.Add(new List<int>());
                for (int i = 0; i < 6; i++)
                    TakeFromCardDeck(ClientsInGame.Count - 1);
            }
            new Thread(new ThreadStart(Start)).Start();
            ActivePlayer = -1;
            ActiveCommand = new List<String>();
            Results = new List<int>();
        }

        private void CreateCardDeck()
        {
            Random rand = new Random();
            for (int i = 1; i <= 36; i++)
                CardsDeck.Add(i);
            for (int i = 36 - 1; i >= 1 - 1; i--)
            {
                int j = rand.Next(0, i + 1);
                int tmp = CardsDeck[i];
                CardsDeck[i] = CardsDeck[j];
                CardsDeck[j] = tmp;
            }
        }

        private bool TakeFromCardDeck(int Hand)
        {
            if (CardsDeck.Any())
            {
                CardsHand[Hand].Add(CardsDeck.Last());
                CardsDeck.RemoveAt(CardsDeck.Count - 1);
                return true;
            }
            return false;
        }

        private bool TakeFromTable(int Hand)
        {
            if (CardsTable.Any())
            {
                CardsHand[Hand].Add(CardsTable.Last());
                CardsTable.RemoveAt(CardsTable.Count - 1);
                return true;
            }
            return false;
        }

        private bool CardToTable(int Hand, int c)
        {
            if (CardsHand[Hand].Remove(c))
            {
                CardsTable.Add(c);
                return true;
            }
            return false;
        }

        private bool CardsRetreat()
        {
            if (CardsTable.Any())
            {
                CardsTable.Clear();
                return true;
            }
            return false;
        }

        private void MsgFromClient(object sender, string msg)
        {
            string m = Regex.Match(msg, @"^\[\[(.*)\]\]$").Groups[1].Value;
            List<string> z = m.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (ActivePlayer != -1 && sender == ClientsInGame[ActivePlayer] && z.Any())
                foreach (string s in z)
                    ActiveCommand.Add(s);
        }

        private int GetCardClass(int card)
        {
            if (1 <= card && card <= 9)
                return 1;
            if (10 <= card && card <= 18)
                return 2;
            if (19 <= card && card <= 27)
                return 3;
            if (28 <= card && card <= 36)
                return 4;
            return 0;
        }

        private int GetCardRank(int card,int _class)
        {
            return _class != 0 ? card - (9 * (_class - 1)) : 0;
        }

        private bool MoveCardAbility(int card, bool attack)
        {
            bool status = false;
            int PlayerCardClass = GetCardClass(card);
            int PlayerCardRank = GetCardRank(card, PlayerCardClass);
            if (!CardsTable.Any())
                status = true;
            else {
                if (attack)
                {
                    foreach (int c in CardsTable)
                        if (GetCardRank(c, GetCardClass(c)) == PlayerCardRank)
                        {
                            status = true;
                            break;
                        }
                }
                else
                {
                    int lastCard = CardsTable.Last();
                    int lastCardClass = GetCardClass(lastCard);
                    int lastCardRank = GetCardRank(lastCard, lastCardClass);

                    if (PlayerCardClass == Trump)
                    {
                        if (lastCardClass == PlayerCardClass && lastCardRank < PlayerCardRank)
                            status = true;
                        else if (lastCardClass != PlayerCardClass)
                            status = true;
                    }
                    else if (PlayerCardClass == lastCardClass && lastCardRank < PlayerCardRank)
                        status = true;
                }
            }
            return status;
        }

        private void CheckHands()
        {
            int c = 0;
            foreach(List<int> v in CardsHand)
            {
                if (!v.Any())
                    Results.Add(c);
                c += 1;
            }
        }

        private void SenderInformationInstaller(int type)
        {
            List<List<int>> k;
            List<int> t;
            for (int i = 0; i < ClientsInGame.Count; i++)
            {
                k = new List<List<int>>();
                t = new List<int>();
                switch (type)
                {
                    case 1:
                    case 4:
                        k.Add(new List<int>() { type });
                        ClientsInGame[i].SetSendString(FormatList(k));
                        break;
                    case 2:
                        k.Add(new List<int>() { type });
                        k.Add(new List<int>() { i + 1 });
                        k.Add(new List<int>() { TrumpCard, CardsDeck.Count, CardsTable.Count });
                        if (CardsTable.Any())
                            k.Add(CardsTable);
                        else
                            k.Add(new List<int>() { -1 });
                        t.Clear();
                        for (int j = 0; j < ClientsInGame.Count; j++)
                            t.Add(CardsHand[j].Count);
                        k.Add(t);
                        if (CardsHand[i].Any())
                            k.Add(CardsHand[i]);
                        else
                            k.Add(new List<int>() { -1 });
                        ClientsInGame[i].SetSendString(FormatList(k));
                        break;
                    case 3:
                        k.Add(new List<int>() { type });
                        k.Add(Results);
                        ClientsInGame[i].SetSendString(FormatList(k));
                        break;
                }
            }
        }

        private string FormatList(List<List<int>> l)
        {
            string r = string.Empty;
            for (int i = 0; i < l.Count; i++)
            {
                for (int j = 0; j < l[i].Count; j++)
                {
                    r += l[i][j].ToString();
                    if (j != l[i].Count - 1)
                        r += ",";
                }
                if(i != l.Count - 1)
                    r += "||";
            }
            return "[[" + r + "]]";
        } 

        public void Start()
        {
            Console.Write("Game with clients ");
            foreach (ClientSandbox v in ClientsInGame)
                Console.Write(v.ClientName + " ");
            Console.WriteLine("started!");
            SenderInformationInstaller(1);
            Thread.Sleep(5000);
            int CardTo,t,n;
            bool los,start, end;
            los = false;
            for (int i = 0; true; i++)
            {
                if (los)
                {
                    los = false;
                    continue;
                }
                if (i == ClientsInGame.Count)
                    i = 0;
                else if (i == ClientsInGame.Count + 1)
                    i = 1;
                int j = i;
                while (CardsHand[j].Any())
                {
                    SenderInformationInstaller(2);
                    ActivePlayer = j;

                    while (ActiveCommand.Count != 2)
                    {
                        foreach(ClientSandbox v in ClientsInGame)
                            if(v.GetStatus() == 0)
                            {
                                Console.Write("Game with clients ");
                                foreach (ClientSandbox vi in ClientsInGame)
                                    Console.Write(vi.ClientName + " ");
                                Console.WriteLine("ended because of loss of connection.");
                                SenderInformationInstaller(4);
                                Thread.Sleep(5000);
                                goto gend;
                            }
                        Thread.Sleep(100);
                    }
                    start = false;
                    end = false;
                    switch (ActiveCommand[0])
                    {
                        case "1":
                            if (j != i)
                            {
                                while (CardsTable.Any())
                                    TakeFromTable(j);
                                los = true;
                            }
                            end = true;
                            break;
                        case "2":
                            CardTo = -1;
                            foreach (int c in CardsHand[j])
                                if (c.ToString() == ActiveCommand[1])
                                    CardTo = c;
                            if (CardTo != -1)
                            {
                                if (MoveCardAbility(CardTo, (j == i)))
                                    CardToTable(j, CardTo);
                                else
                                    start = true;
                            }
                            else
                                start = true;
                            break;
                        default:
                            start = true;
                            break;
                    }
                    ActiveCommand.Clear();
                    ActivePlayer = -1;
                    if (end)
                        break;
                    if (start)
                        continue;

                    if (j == i)
                        j++;
                    else
                        j = i;
                    if (j == ClientsInGame.Count)
                        j = 0;
                }
                t = i;
                n = i == ClientsInGame.Count - 1 ? 0 : i + 1;
                while(CardsDeck.Any() && (CardsHand[t].Count < 6 || CardsHand[n].Count < 6))
                {
                    if(CardsHand[t].Count < 6)
                        TakeFromCardDeck(t);
                    if (CardsHand[n].Count < 6)
                        TakeFromCardDeck(n);
                }
                CardsRetreat();
                CheckHands();
                if (Results.Count == ClientsInGame.Count)
                {
                    Console.Write("Game with clients ");
                    foreach (ClientSandbox v in ClientsInGame)
                        Console.Write(v.ClientName + " ");
                    Console.WriteLine("ended!");
                    SenderInformationInstaller(3);
                    Thread.Sleep(5000);
                    goto gend;
                }
            }
            gend:
            foreach(ClientSandbox v in ClientsInGame)
                v.ExternalStatus = 4;
        }
    }
}
