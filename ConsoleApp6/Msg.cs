using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    class Msg
    {
        public int UserId { get;  set; }
        public string MsgString { get;  set; }
        public DateTime Date { get;  set; }

        public Msg(int uId,string msg)
        {
            UserId = uId;
            MsgString = msg;
            Date = DateTime.Now;
        }

    }
}
