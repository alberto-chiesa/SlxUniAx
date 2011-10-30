using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gianos.UniLib;

namespace Gianos.SlxUniAxTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var dbHandler = new DbHandler(".\\ACA2008", "BVSLX_PROD", "sa", "masterkey");
            var dbHandler = new DbHandler(".\\ACA2008", "BVSLX_PROD", "sysdba", "masterkey");

        }
    }
}
