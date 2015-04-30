using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Plodine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length.Equals(0))
            {
                //klasicno pokretanje aplikacije
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                //proslijeden je parametar
                //Za windows scheduler
                Form1 forma = new Form1();

                switch (args[0])
                {
                    case "AUTO":
                        //Kada je paramater kod pokretanja AUTO onda se odmah ivrši sve
                        forma.AutoImport();
                        break;
                    //case "ECOD":
                        //Ukoliko je parametar ECOD pokrecem samo automatizirani proces parsiranja ECOD narudžbi
                        //NE prosljeđujem partnera jer se partner pokupi iz XML-a
                        //forma.AutomatskoPokretanjeECOD();
                        //break;
                }//end switch                   

            }


        }
    }
}
