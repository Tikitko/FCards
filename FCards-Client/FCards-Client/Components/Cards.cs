using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCards_Client
{
    class Cards
    {
        public static string GetImageName(int i)
        {
            switch (i)
            {
                case 1:
                    return "T6_clubs-min.png";
                case 2:
                    return "T7_clubs-min.png";
                case 3:
                    return "T8_clubs-min.png";
                case 4:
                    return "T9_clubs-min.png";
                case 5:
                    return "T10_clubs-min.png";
                case 6:
                    return "Jack_clubs-min.png";
                case 7:
                    return "Queen_clubs-min.png";
                case 8:
                    return "King_clubs-min.png";
                case 9:
                    return "Ace_clubs-min.png";
                case 10:
                    return "T6_diamonds-min.png";
                case 11:
                    return "T7_diamonds-min.png";
                case 12:
                    return "T8_diamonds-min.png";
                case 13:
                    return "T9_diamonds-min.png";
                case 14:
                    return "T10_diamonds-min.png";
                case 15:
                    return "Jack_diamonds-min.png";
                case 16:
                    return "Queen_diamonds-min.png";
                case 17:
                    return "King_diamonds-min.png";
                case 18:
                    return "Ace_diamonds-min.png";
                case 19:
                    return "T6_hearts-min.png";
                case 20:
                    return "T7_hearts-min.png";
                case 21:
                    return "T8_hearts-min.png";
                case 22:
                    return "T9_hearts-min.png";
                case 23:
                    return "T10_hearts-min.png";
                case 24:
                    return "Jack_hearts-min.png";
                case 25:
                    return "Queen_hearts-min.png";
                case 26:
                    return "King_hearts-min.png";
                case 27:
                    return "Ace_hearts-min.png";
                case 28:
                    return "T6_spades-min.png";
                case 29:
                    return "T7_spades-min.png";
                case 30:
                    return "T8_spades-min.png";
                case 31:
                    return "T9_spades-min.png";
                case 32:
                    return "T10_spades-min.png";
                case 33:
                    return "Jack_spades-min.png";
                case 34:
                    return "Queen_spades-min.png";
                case 35:
                    return "King_spades-min.png";
                case 36:
                    return "Ace_spades-min.png";
                default:
                    return "backside-min.png";
            }
        }
    }
}
