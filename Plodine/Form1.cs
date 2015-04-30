using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Globalization;
using System.Net.Mail;
using System.Collections;

namespace Plodine
{
    public partial class Form1 : Form
    {
        private string direktorij = "M:\\EDI-PLODINE\\";
        //private string errorFile = @"M:\EDI-PLODINE\error\greskePlodine.txt";
        private string errorFile = @"greskePlodine.txt";

        public Form1()
        {
            InitializeComponent();
        }

        public void AutoImport()
        {
            try
            {
                MoveFilesToWorking();
                ArrayList lista = ReadWorkingFiles();
                //lokalno i 2k12
                bool ishod = DataIPS.SQLInsertPOM_EDI(lista);
                
                //Server 2k3
                //bool ishod = DataIPS.SQLInsertPOM_EDI_Server(lista);

                ReportStatus("InsertPOM_EDI"+ishod.ToString());
                bool ishod2 = DataIPS.SExecutePROC_EDI_PLOD();
                ReportStatus("PROC_EDI_PLOD"+ishod2.ToString());
            }
            catch (Exception izn)
            {
                ErrorReport(izn.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MoveFilesToWorking();
                ArrayList lista = ReadWorkingFiles();
                //na serveru se mora pokretati druga procedura POM_EDI_Server
                bool ishod = DataIPS.SQLInsertPOM_EDI_Server(lista);
                ReportStatus("InsertPOM_EDI" + ishod.ToString());
                bool ishod2 = DataIPS.SExecutePROC_EDI_PLOD();
                ReportStatus("PROC_EDI_PLOD" + ishod2.ToString());
            }
            catch (Exception izn)
            {
                ErrorReport(izn.Message);
            }
        }


        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                MoveFilesToWorking();
                ArrayList lista = ReadWorkingFiles();
                //lokalno se poziva ova precedura a na severu 2k3 pokrece se POM_EDI_Server
                bool ishod = DataIPS.SQLInsertPOM_EDI(lista);
                ReportStatus("InsertPOM_EDI" + ishod.ToString());
                bool ishod2 = DataIPS.SExecutePROC_EDI_PLOD();
                ReportStatus("PROC_EDI_PLOD" + ishod2.ToString());
            }
            catch (Exception izn)
            {
                ErrorReport(izn.Message);
            }
        }


        private DataTable GetDataTableFromCsv(string dName, string fName, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            //string pathOnly = Path.GetDirectoryName(path);
            //string fileName = Path.GetFileName(path);
            string pathOnly = dName;
            string fileName = fName;

            string sql = @"SELECT * FROM [" + fileName + "]";

            ReportStatus("GetDataTableFromCsv");

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        /// <summary>
        /// Nova procedura za load podataka iz CSV datoteke u dataTable objekt
        /// </summary>
        /// <param name="dName"></param>
        /// <param name="fName"></param>
        /// <param name="isFirstRowHeader"></param>
        /// <returns></returns>
        private DataTable GetDataTableFromCsvNEW(string dName, string fName)
        {
            //string CSVFilePathName = @"C:\test.csv";
            string CSVFilePathName = dName+"\\"+fName;
            string[] Lines = File.ReadAllLines(CSVFilePathName);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }
            return dt;
        }


        private DataTable CSVDT(string dName, string fName)
        {
            string CSVFilePathName = dName+"\\"+fName;
            string[] Lines = File.ReadAllLines(CSVFilePathName);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }
            return dt;
        }

        private void MoveFilesToWorking()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(direktorij);
            FileInfo[] fileovi = dirInfo.GetFiles("*.csv");

            string workingDir = direktorij + "working\\";
            string notValidDir = direktorij + "not valid\\";
            string errorDir = direktorij + "error\\";

            ReportStatus("MoveFilesToWorking");

            foreach (FileInfo file in fileovi)
            {
                
                if (File.Exists(workingDir + file.Name))
                {
                    File.Delete(workingDir + file.Name);
                    File.Move(direktorij + file.Name, workingDir + file.Name);
                }
                else
                {
                    File.Move(direktorij + file.Name, workingDir + file.Name);
                }
                
                
            }
        }
        private ArrayList ReadWorkingFiles()
        {   
            string workingDir = direktorij + "working\\";
            ArrayList excel_load = new ArrayList();
            DirectoryInfo dirInf = new DirectoryInfo(workingDir);
            FileInfo[] fileInf = dirInf.GetFiles();
            

            //preuzeti podatke iz excel-a
            //string dat = @"M:\AOP\EDI\EDI-Plodine\nar-001-12517.csv";

            ReportStatus("ReadWorkingFiles");

            foreach (FileInfo file in fileInf)
            {
                try
                {
                    
                    //DataTable podaci = GetDataTableFromCsv(file.DirectoryName,file.Name, true);
                    DataTable podaci = GetDataTableFromCsvNEW(file.DirectoryName, file.Name);

                    
                    excel_load.Add(podaci);
                    //obradene datoteke premjestam u backup direktorij
                    //obrisi datoteku koja je tamo ako ona postoji

                    //stvaranje novog direktorija
                    string sad = DateTime.Now.ToString();
                    string folderName = sad.Replace(":", "_");
                    string newDirPath = direktorij + "backup\\" + folderName;
                    Directory.CreateDirectory(newDirPath);

                    if (File.Exists(newDirPath+"\\" + file.Name))
                    {
                        File.Delete(newDirPath+ "\\" + file.Name);
                        File.Move(workingDir + file.Name, newDirPath + "\\" + file.Name);
                    }
                    else
                    {
                        File.Move(workingDir + file.Name, newDirPath + "\\" + file.Name);
                    }
                }
                catch (Exception izn)
                {
                    //U slučaju da se dogodi pogreška pri obradi odredene datoteke
                    ErrorReport(file.Name + " - Greška pri deserijalizaciji" + izn.Message);

                    if (File.Exists(direktorij + "error\\" + file.Name))
                    {
                        File.Delete(direktorij + "error\\" + file.Name);
                        File.Move(workingDir + file.Name, direktorij + "error\\" + file.Name);
                    }
                    else
                    {
                        File.Move(workingDir + file.Name, direktorij + "error\\" + file.Name);
                    }

                    continue;
                }
            }
            return excel_load;
        }

        private void PosaljiMailAdmin(string sadrzajPoruke)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add("domagoj.madic@franck.hr");
            mail.CC.Add("sonja.novacic-nadu@franck.hr");                       
            mail.Subject = "EDI narudžbe - problem";
            mail.From = new MailAddress("EDI@franck.hr");
            mail.Body = sadrzajPoruke;
            SmtpClient client = new SmtpClient("mail.franck.hr");
            client.Send(mail);
        }

        private void PosaljiMailSvima(string sadrzajPoruke, string partner)
        {
            MailMessage mail = new MailMessage();
            string mailKAM = DataIPS.SQLMailKam(partner);
            string mailREFERENT = DataIPS.SQLMailReferent(partner);


            mail.To.Add(mailKAM);
            mail.CC.Add(mailREFERENT);


            mail.Bcc.Add("domagoj.madic@franck.hr");
            //mail.Bcc.Add("darko.jegjud@franck.hr");
            //mail.Bcc.Add("tinka.hrastinski@franck.hr");
            mail.Bcc.Add("sonja.novacic-nadu@franck.hr");
            //mail.Bcc.Add("zelimir.kozjak@franck.hr");
            mail.Subject = "EDI narudžbe - problem";
            mail.From = new MailAddress("EDI@franck.hr");
            mail.Body = sadrzajPoruke;
            SmtpClient client = new SmtpClient("mail.franck.hr");
            client.Send(mail);
        }
        
        //zapisuje poruku u datoteku greske.txt i salje mail
        public void ErrorReport(string poruka)
        {
            try
            {
                StreamWriter writeError = new StreamWriter(errorFile, true);
                writeError.WriteLine(DateTime.Now.ToString());
                writeError.WriteLine("EDI PLODINE");
                writeError.WriteLine(poruka);
                writeError.WriteLine("--------------------------------------------------");
                writeError.Flush();
                writeError.Close();
                PosaljiMailAdmin(poruka);
            }
            catch (Exception e)
            {
                ReportStatus(e.Message);
            }
        }

        //zapisuje poruku u datoteku greske.txt i salje mail
        public void ErrorReportSvima(string poruka, string partner)
        {
            try
            {
                StreamWriter writeError = new StreamWriter(errorFile, true);
                writeError.WriteLine(DateTime.Now.ToString());
                writeError.WriteLine(poruka);
                writeError.WriteLine("--------------------------------------------------");
                writeError.Flush();
                writeError.Close();
                PosaljiMailSvima("EDI PLODINE" + poruka, partner);
            }
            catch (Exception e)
            {
                ReportStatus(e.Message);
            }
        }

        //zapisuje poruku u datoteku greske.txt
        public void ReportStatus(string poruka)
        {
            StreamWriter writeError = new StreamWriter(errorFile, true);
            writeError.WriteLine(DateTime.Now.ToString());
            writeError.WriteLine("EDI PLODINE");
            writeError.WriteLine(poruka);
            writeError.Flush();
            writeError.Close();
        }

       
    
    }


}
