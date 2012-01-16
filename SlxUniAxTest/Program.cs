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
            //var dbHandler = new DbHandler(".\\ACA2008", "BVSLX_PROD", "sysdba", "masterkey");
            //var fields = dbHandler.ReadTableDataFromSLXDb();

            string testActions = @"
# This is a comment
ACCOUNT.accountname->Unicode(100)

Contact.FirstName->ansi(200)
# contact.lastname->UnIcode
  contact   . prefix
->  unicode

account.    accountmanagerid -> ansi(1)";
            var actions = FieldAction.Parse(testActions);


            return;
            var model = new SLXModelHandler(@"C:\Users\ACA.GIANOS\Documents\Dev\bvweb\Model");
            var fields = model.FindEntityModels();
            var dbHandler = new DbHandler(".\\ACA2008", "BVSLX_PROD", "sysdba", "masterkey");
            fields = dbHandler.ReadTableDataFromSLXDb(fields);
            var f = fields["BVSTORE"]["COD_COUNTRY"];

            var s = f.State;
            f = fields["ACCOUNT"]["ACCOUNT"];
            s = f.State;
            //model.SetUnicodeOnSlxField(f, true);
            //dbHandler.SetUnicodeOnDbField(f, true);
            //var s = dbHandler.GetCreateScriptForIndexes("C_BASE_TABLE", "NETSALEEUR");
            //Console.Write(s);
        }
    }
}
