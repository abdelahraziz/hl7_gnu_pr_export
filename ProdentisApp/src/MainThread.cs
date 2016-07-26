using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace ProdentisApp
{
    class MainThread : HL7Util.HL7Cb
    {
        private MainThreadCb ui;
        private string host;
        private int port;
        private string dbname;
        private string user;
        private string password;
        private int rowIndex;
        private int rowCount;

        public MainThread(MainThreadCb _ui, string _host, int _port, string _dbname, string _user, string _password)
        {
            ui = _ui;
            host = _host;
            port = _port;
            dbname = _dbname;
            user = _user;
            password = _password;
        }

        public interface MainThreadCb : DBUtil.ConnectCb, HL7Util.HL7Cb, NetUtil.ScanPortsCb
        {
            void onError(string msg);
        }

        public void processProdentis()
        {
            try
            {
                DBUtil.MSSqlDBConnection db = DBUtil.MSSqlDBConnection.Instance();
                if (db.connect(ui, host, port, dbname, user, password))
                {
                    int loop = 1;
                    string tmpFile = System.IO.Path.GetTempPath().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "hl7dbexport.hl7";
                    DBUtil.DBResult countResult = db.query("Select count(*) from Prodentis500.dbo.pacjenci");
                    rowCount = (Int32.Parse(countResult[0][0]) / 1000) + 1;
                    while (true)
                    {
                        string q = "Select nr_kartywew, imie, imie2, nazwisko, plec, ulica, num_domu, num_mieszkania, kod_pocztowy, miasto, g.nazwa, kod_miasta, w.wojewodztwo, c.nazwa, nip, pesel, data_urodzenia, miejsce_ur, email, telefonypraca, telefonydom, komorka, wys_sms, wys_email, paszport, nip, symbol_kch from Prodentis500.dbo.pacjenci p left join Prodentis500.dbo.s_oddzialy_NFZ nfzo on p.symbol_kch = nfzo.symbol_k left join Prodentis500.dbo.s_kraje c on c.id_kraju = p.id_kraju left join Prodentis500.dbo.s_wojewodztwa w on  p.kod_wojew = w.kod left join Prodentis500.dbo.s_gminy g on p.kod_gminy = g.kod order by pesel offset " + ((loop - 1) * 1000).ToString() + " rows FETCH NEXT " + (loop * 1000).ToString() + " ROWS ONLY";
                        DBUtil.DBResult result = db.query(q);
                        Patients patients = AppDataUtil.processProdentis(result);
                        if (patients.Size() == 0)
                        {
                            ui.onHL7Done(tmpFile);
                            db.Close();
                            return;
                        }
                        string stream = "";
                        HL7Util.processPatients(this, ref stream, patients, tmpFile);
                        loop++;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void onHL7Progress(double progress)
        {
            double res = progress;
            res = 100 * ((res / 100) + rowIndex) / rowCount;
            ui.onHL7Progress(res);
        }

        public void onHL7Done(string tmpFile)
        {
            rowIndex++;
        }
    }
}
