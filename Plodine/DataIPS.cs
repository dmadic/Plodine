using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using System.Data;
using System.Collections;
using System.IO;

namespace Plodine
{
    class DataIPS
    {
        ////test
        //private static string connectionString = Plodine.Properties.Resources.Test;

        //Produkcija
        //private static string connectionString = Plodine.Properties.Resources.Produkcija;
        private static string connectionString = "Data Source=franpro.world;User ID=franck;Password=franck;Unicode=True";

        /// <summary>
        /// Spaja sa na bazu i izvrsava upit te rezultat tog upita vraca u DataTable obliku
        /// --Za SELECT upite
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static System.Data.DataTable ConnectAndExecuteQuery(string query, string connString)
        {
            using (OracleConnection connection = new OracleConnection())
            {
                //    OracleConnection connection = new OracleConnection();            

                connection.ConnectionString = connString;
                connection.Open();
                OracleCommand command = connection.CreateCommand();
                command.CommandText = query;

                OracleDataReader reader = command.ExecuteReader();
                System.Data.DataTable table = new System.Data.DataTable();
                CustomAdapter adap = new CustomAdapter();
                adap.FillFromReader(table, reader);
                //redera obavezno zatvoriti
                reader.Close();
                connection.Close();
                return table;
            }
        }

        /// <summary>
        /// Spaja sa na bazu i izvrsava upit koji ne vraca rezultat... npr. poziv procedura ili brisanje podataka
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static void ConnectAndExecuteNONQuery(string query)
        {
            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                OracleCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        //public static bool SQLInsertEDI_glava(EdiGlava eg)
        //{
        //    bool usjesnost = true;
        //    try
        //    {
        //        //OPREZ!!!
        //        //izbacio sam iz inserta vrijednosti koje se nisu ubacivale ni kroz stari edi_parser (ref_dok, ref_broj, status_valid, status, datum_prijema)                                
        //        //string query = "INSERT INTO edi_glava(TRANSAKCIJA, DATUM_TRAN, ORDER_ID, ORDER_DATA, MSG_TIP, MSG_ID, TRAN_GLN_SENDER, TRAN_GLN_RECIPIENT, GLN_BUYER, GLN_SUPPLIER, GLN_DELIVERYTO, GLN_ORDERED, DATUM_ISPORUKE, DATUM_KREIRANJA, DATUM_PRIJEMA, STATUS, SKLADISTE, REF_DOK, REF_BROJ, STATUS_VALID, FILENAME, FUNKCIJA, BROJ_ARTIKALA, TEST, MULTI_GLN, PARTNER_TIP) VALUES ('"+eg.transakcija+"',to_date('"+eg.datum_transakcija.ToShortDateString()+"'),'"+eg.order_id+"','"+eg.order_data+"','"+eg.msg_tip+"','"+eg.msg_id+"','"+eg.tran_gln_sender+"','"+eg.tran_gln_recipient+"','"+eg.gln_buyer+"','"+eg.gln_supplier+"','"+eg.gln_deliveryto+"','"+eg.gln_orderd+"',to_date('"+eg.datum_ispruke.ToShortDateString()+"'),to_date('"+eg.datum_kreiranja.ToShortDateString()+"'),to_date('"+eg.datum_prijema.ToShortDateString()+"'),'"+eg.status+"','"+eg.skladiste+"','"+eg.ref_dok+"','"+eg.ref_broj+"','"+eg.status_valid+"','"+eg.filename+"','"+eg.funkcija+"',to_number('"+eg.broj_artikala+"'),'"+eg.test+"',to_number('"+eg.multi_gln+"'),'"+eg.partner_tip+"')";


        //        //LOKALNO
        //        //lokalno na mom računalu radi sa
        //        //string query = "INSERT INTO edi_glava(TRANSAKCIJA, DATUM_TRAN, ORDER_ID, ORDER_DATA, MSG_TIP, MSG_ID, TRAN_GLN_SENDER, TRAN_GLN_RECIPIENT, GLN_BUYER, GLN_SUPPLIER, GLN_DELIVERYTO, GLN_ORDERED, DATUM_ISPORUKE, DATUM_KREIRANJA, SKLADISTE, FILENAME, FUNKCIJA, BROJ_ARTIKALA, TEST, MULTI_GLN, PARTNER_TIP) VALUES ('" + eg.transakcija + "',to_date('" + eg.datum_transakcija.ToShortDateString().Substring(0, eg.datum_transakcija.ToShortDateString().Length - 1) + "'),'" + eg.order_id + "','" + eg.order_data + "','" + eg.msg_tip + "','" + eg.msg_id + "','" + eg.tran_gln_sender + "','" + eg.tran_gln_recipient + "','" + eg.gln_buyer + "','" + eg.gln_supplier + "','" + eg.gln_deliveryto + "','" + eg.gln_orderd + "',to_date('" + eg.datum_ispruke.ToShortDateString().Substring(0, eg.datum_ispruke.ToShortDateString().Length - 1) + "'),to_date('" + eg.datum_kreiranja.ToShortDateString().Substring(0, eg.datum_kreiranja.ToShortDateString().Length - 1) + "'),'" + eg.skladiste + "','" + eg.filename + "','" + eg.funkcija + "',to_number('" + eg.broj_artikala + "'),'" + eg.test + "',to_number('" + eg.multi_gln + "'),'" + eg.partner_tip + "')";

        //        //PRODUKCIJA - OVO JE ZA s-vm-fak-2k3
        //        //za produkciju Clijent 10 (mislim da je problem u Oracle Clientu ali isti je na s-vm.fak-2k3 i mom racunalu)
        //        string query = "INSERT INTO edi_glava(TRANSAKCIJA, DATUM_TRAN, ORDER_ID, ORDER_DATA, MSG_TIP, MSG_ID, TRAN_GLN_SENDER, TRAN_GLN_RECIPIENT, GLN_BUYER, GLN_SUPPLIER, GLN_DELIVERYTO, GLN_ORDERED, DATUM_ISPORUKE, DATUM_KREIRANJA, SKLADISTE, FILENAME, FUNKCIJA, BROJ_ARTIKALA, TEST, MULTI_GLN, PARTNER_TIP) VALUES ('" + eg.transakcija + "',to_date('" + eg.datum_transakcija.ToShortDateString() + "'),'" + eg.order_id + "','" + eg.order_data + "','" + eg.msg_tip + "','" + eg.msg_id + "','" + eg.tran_gln_sender + "','" + eg.tran_gln_recipient + "','" + eg.gln_buyer + "','" + eg.gln_supplier + "','" + eg.gln_deliveryto + "','" + eg.gln_orderd + "',to_date('" + eg.datum_ispruke + "','dd.mm.yyyy. hh24:mi:ss'),to_date('" + eg.datum_kreiranja.ToShortDateString() + "'),'" + eg.skladiste + "','" + eg.filename + "','" + eg.funkcija + "',to_number('" + eg.broj_artikala + "'),'" + eg.test + "',to_number('" + eg.multi_gln + "'),'" + eg.partner_tip + "')";

        //        ConnectAndExecuteNONQuery(query);
        //    }
        //    catch (Exception e)
        //    {
        //        usjesnost = false;
        //    }
        //    return usjesnost;
        //}

        //jos treba testirati
        //public static bool SQLInsertEDI_stavka(EdiStavka es)
        //{
        //    bool usjesnost = true;
        //    try
        //    {
        //        string query = "INSERT INTO edi_stavka(TRANSAKCIJA, ORDER_ID, MSG_ID, GTIN_ARTIKL, QTY, RBR) VALUES ('" + es.transakcija + "','" + es.order_id + "','" + es.msg_id + "','" + es.gtin_artikl + "','" + es.qty + "','" + es.rbr + "')";
        //        ConnectAndExecuteNONQuery(query);
        //    }
        //    catch (Exception)
        //    {
        //        usjesnost = false;
        //    }
        //    return usjesnost;
        //}

        //vraca sifre iz partner_korisnika gdje je gln jednak parametru
        public static DataTable SQLMultiGLN(string gln, string partner)
        {
            //!!! U upit obavezno dodati status provjeru statusa 'A' za produkciju
            string query = "SELECT sifra FROM partner_korisnik WHERE gln = '" + gln + "' and partner='" + partner + "'";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            return podaci;
        }


        //provjera transportnog koda
        public static bool SQLPostojiEan_tp(string kod)
        {
            //!!! U upit obavezno dodati status provjeru statusa 'A' za produkciju
            bool vrati = true;
            string query = "SELECT * FROM ARTIKL WHERE ean_tp = '" + kod + "'";

            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);

            if (podaci.Rows.Count == 0)
            {
                vrati = false;
            }
            return vrati;
        }

        //provjera transportnog koda
        public static bool SQLArtiklBrisan(string kod)
        {
            //
            bool vrati = false;
            string query = "SELECT status FROM ARTIKL WHERE ean_tp = '" + kod + "'";

            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);


            if (podaci.Rows.Count == 0)
            {
                //ovakav artikl uopce ne postoji pa ne mozemo reci da je oznacen za brisanje
                vrati = false;
            }
            else
            {
                string status = podaci.Rows[0][0].ToString();
                //samo ako je status = 'B' vrati true ako je "" vratit ce false
                if (status.Equals("B"))
                {
                    vrati = true;
                }
            }
            return vrati;
        }


        //GLN FRANCK-a
        public static string SQLFranckGLN()
        {
            //!!! U upit obavezno dodati status provjeru statusa 'A' za produkciju
            string query = "SELECT gln FROM CONFIG";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            string glnFranck;
            if (podaci.Rows.Count == 0)
            {
                glnFranck = "3858881080003";
            }
            else
            {
                glnFranck = podaci.Rows[0][0].ToString();
            }
            return glnFranck;
        }


        //Mail KAM-a partner
        public static string SQLMailKam(string partner)
        {
            //U upit svakako treba ubaciti mogucnost izmjene parametara
            string query = "select k.email from partner p, kadar k where P.KAM = k.sifra and p.sifra = '" + partner + "'";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            string mail = "";
            if (podaci.Rows.Count == 0)
            {
                mail = "domagoj.madic@franck.hr";
            }
            else
            {
                mail = podaci.Rows[0][0].ToString();
            }
            return mail;
        }

        //Mail Referenta-a partner
        public static string SQLMailReferent(string partner)
        {
            //U upit svakako treba ubaciti mogucnost izmjene parametara
            string query = "select k.email from partner p, kadar k where P.referent = k.sifra and p.sifra = '" + partner + "'";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            string mail = "";
            if (podaci.Rows.Count == 0)
            {
                mail = "domagoj.madic@franck.hr";
            }
            else
            {
                mail = podaci.Rows[0][0].ToString();
            }
            return mail;
        }

        //GLN PARTNERA-a
        public static string SQLPartnerGLN(string partner)
        {
            //!!! U upit obavezno dodati status provjeru statusa 'A' za produkciju
            string query = "SELECT gln FROM PARTNER WHERE sifra = '" + partner + "'";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            string gln = "";
            if (podaci.Rows.Count == 0)
            {
                //ovdije dodati i za druge partnere koje ce se obradivati
                if (partner.Equals("000765"))
                {
                    gln = "3859888798007";
                }
            }
            else
            {
                gln = podaci.Rows[0][0].ToString();
            }
            return gln;
        }


        //Sifra PARTNERA-a
        public static string SQLPartnerSifra(string gln)
        {
            //!!! U upit obavezno dodati status provjeru statusa 'A' za produkciju
            string query = "SELECT sifra FROM PARTNER WHERE gln = '" + gln + "'";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            string partner = "";
            if (podaci.Rows.Count == 0)
            {
                partner = "PartnerNEpoznat";
            }
            else
            {
                partner = podaci.Rows[0][0].ToString();
            }
            return partner;
        }

        //
        public static DataTable SQLSkladisteOtpreme(string partner, string sif_korisnik)
        {
            string query = "SELECT skladiste_otpreme FROM partner_korisnik pk WHERE  pk.sifra ='" + sif_korisnik + "' and partner='" + partner + "'";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            return podaci;
        }

        //
        public static DataTable SQLPartnerTip(string partner)
        {
            string query = "SELECT dom_ino FROM partner p WHERE  p.sifra ='" + partner + "'";
            DataTable podaci = new DataTable();
            podaci = ConnectAndExecuteQuery(query, connectionString);
            return podaci;
        }

        ////
        //public static void DeleteFromEDIGlava()
        //{
        //    string query = "DELETE FROM edi_stavka e WHERE  e. ='" + partner + "'";
        //    DataTable podaci = new DataTable();
        //    podaci = ConnectAndExecuteQuery(query, connectionString);
        //    return podaci;
        //}


        public static bool SQLInsertPOM_EDI(ArrayList ep)
        {
            bool usjesnost = true;
            //try
            //{

                foreach (object oo in ep)
                {
                    //za svaki CSV
                    DataTable dt = (DataTable)oo;

                    for (int br = 0; br < dt.Rows.Count; br++ )
                    {
                        string pom = dt.Rows[br][0].ToString();
                        string a, a1, b, c, d, e, f, g, i, j, k, l, m, n, o;

                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        o = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        n = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        m = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        l = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        k = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        j = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        i = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);                        
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        f = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        //g kasnije preskacem jer je u toj koloni GLN - to ćemo morati promjeniti
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        g = pom.Substring(pom.LastIndexOf(";") + 1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        e = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        d = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        c = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        b = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);
                        //A1 je isto GLN i za sada ga kasnije preskacem
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        a1 = pom.Substring(pom.LastIndexOf(";") + 1, pom.Length - pom.LastIndexOf(";") - 1);
                        pom = pom.Substring(0, pom.LastIndexOf(";"));
                        a = pom.Substring(pom.LastIndexOf(";")+1, pom.Length - pom.LastIndexOf(";") - 1);

                        b = Convert.ToDateTime(b).ToShortDateString();
                        c = Convert.ToDateTime(c).ToShortDateString();
                                                
                        string query = "INSERT INTO pom_edi(a,b,c,d,e,f,i,j,k,l,m,n,o)VALUES ('" + a + "','" + b + "','" + c + "','" + d + "','" + e + "','" + f + "','" + i + "','" + j + "','" + k + "','" + l + "','" + m + "','" + n + "','" + o + "')";

                        ConnectAndExecuteNONQuery(query);
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    usjesnost = false;
            //    //Report u File
            //    StreamWriter writeError = new StreamWriter(@"M:\EDI-PLODINE\error\greskePlodine.txt", true);
            //    writeError.WriteLine(DateTime.Now.ToString() + e.Message);
            //    writeError.Flush();                
            //    writeError.Close();
            //}
            return usjesnost;
        }

        public static bool SQLInsertPOM_EDI_Server(ArrayList ep)
        {
            bool usjesnost = true;
            //try
            //{
                
                foreach (object oo in ep)
                {
                    //za svaki CSV
                    DataTable dt = (DataTable)oo;                    
                    for (int br = 0; br < dt.Rows.Count; br++)
                    {   
                        string a, a1, b, c, d, e, g, f, i, j, k, l, m, n, o;

                        a = dt.Rows[br][0].ToString();
                        a1 = dt.Rows[br][1].ToString();
                        b = dt.Rows[br][2].ToString();
                        c = dt.Rows[br][3].ToString();
                        d = dt.Rows[br][4].ToString();
                        e = dt.Rows[br][5].ToString();
                        g = dt.Rows[br][6].ToString();
                        f = dt.Rows[br][7].ToString();                        
                        i = dt.Rows[br][8].ToString();
                        j = dt.Rows[br][9].ToString();
                        k = dt.Rows[br][10].ToString();
                        l = dt.Rows[br][11].ToString();
                        m = dt.Rows[br][12].ToString();
                        n = dt.Rows[br][13].ToString();
                        o = dt.Rows[br][14].ToString();                                               

                        //Dodajem točku na kraj jer se inače oracle zblesira
                        b = Convert.ToDateTime(b).ToShortDateString()+".";
                        c = Convert.ToDateTime(c).ToShortDateString()+".";

                        string query = "INSERT INTO pom_edi(a,b,c,d,e,f,i,j,k,l,m,n,o) VALUES ('" + a + "','" + b + "','" + c + "','" + d + "','" + e + "','" + f + "','" + i + "','" + j + "','" + k + "','" + l + "','" + m + "','" + n + "','" + o + "')";

                        ConnectAndExecuteNONQuery(query);
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    usjesnost = false;
            //    //Report u File
            //    //StreamWriter writeError = new StreamWriter(@"M:\EDI-PLODINE\error\greskePlodine.txt", true);
            //    StreamWriter writeError = new StreamWriter(@"greskePlodine.txt", true);
            //    writeError.WriteLine(DateTime.Now.ToString() + e.Message);
            //    writeError.Flush();
            //    writeError.Close();
            //}
            return usjesnost;
        }


        public static bool SExecutePROC_EDI_PLOD()
        {
            bool usjesnost = true;

            try
            {

                OracleConnection connection = new OracleConnection();

                connection.ConnectionString = connectionString;
                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PROC_EDI_PLOD()";

                //command.Parameters.Add("P_PID", "varchar2");
                //command.Parameters[0].Value = pid;
                //command.Parameters.AddWithValue("P_PID", "varchar2").Value = pid;

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                usjesnost = false;
            }
            return usjesnost;
        }

    }
}
