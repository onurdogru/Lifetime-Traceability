using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using IAS_MSL_Traceability.Printer;
using System.Printing;

namespace IAS_MSL_Traceability
{
	public partial class Admin : Form
	{
		Form1 form1;
		MySqlCommand mySqlCommandUpdate;
		MySqlDataReader mySqlDataReader;
		Boolean isInsert;
		public Admin(Form1 form1)
		{
			InitializeComponent();
			this.form1 = form1;
		}
		private void Admin_Load(object sender, EventArgs e)
		{
			checkBox1.Checked = Setting.Default.page1Visible;
			checkBox2.Checked = Setting.Default.page2Visible;
			checkBox3.Checked = Setting.Default.page3Visible;
			checkBox4.Checked = Setting.Default.buttonSIL; 

			this.printerName.Text = Setting.Default.printerName;
			this.printerPos.Text = Setting.Default.printerPos;

			foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
			{
				printerName.Items.Add(printer);
			}
		}
		private void button1_Click(object sender, EventArgs e)
		{
		/*	tabPageTemp = form1.tabControl1.TabPages[0];

			form1.tabControl1.TabPages.Remove(form1.tabControl1.TabPages[0]);*/
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//form1.tabControl1.TabPages.Add(tabPageTemp);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Setting.Default.page1Visible = checkBox1.Checked;
			Setting.Default.page2Visible = checkBox2.Checked;
			Setting.Default.page3Visible = checkBox3.Checked;
			Setting.Default.buttonSIL = checkBox4.Checked;


			Setting.Default.printerName = this.printerName.Text;
			Setting.Default.printerPos = this.printerPos.Text;
			if (txtPass.Text!="")
			Setting.Default.password = txtPass.Text;
			Setting.Default.Save();
			CustomMessageBox.ShowMessage("Kaydetme İşlemi Başarılı !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Information, Color.Green);
			Application.Restart();
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			if (isInsert)
			{
				try
				{
					if (txt5.Text == "")
					{
						mySqlCommandUpdate = new MySqlCommand("UPDATE `IAS_MSL_Tb` SET MSL='"+(txt9.Text)+"', BarcodeCol4='U" + txt1.Text + "', BarcodeCol6='X" + txt2.Text + "',ReleaseDate='" + txt3.Text + "',UsageStartDate='" + txt4.Text + "', `UsageEndDate`= NULL ," +
						" `MSLDate`='" + txt6.Text + "',BarcodeCol5='Q" + txt7.Text + ".0' , `Status`='" + (txt8.SelectedIndex + 1) + "' WHERE BarcodeCol1='" + txtBarcode.Text.Split(',')[0] + "'", this.form1.mySqlConnection);
					}
					else
					{
						mySqlCommandUpdate = new MySqlCommand("UPDATE `IAS_MSL_Tb` SET" + " MSL='" + txt9.Text + "',BarcodeCol4='U" + txt1.Text + "', BarcodeCol6='X" + txt2.Text + "',ReleaseDate='" + DateTime.Parse(txt3.Text).ToString("dd.MM.yyyy") + "',`UsageStartDate`= '" + DateTime.Parse(txt4.Text).ToString("yyyy-MM-dd HH:mm:ss") + "', `UsageEndDate`= '" + DateTime.Parse(txt5.Text).ToString("yyyy-MM-dd HH:mm:ss") + "', `MSLDate`='" + txt6.Text + "',BarcodeCol5='Q" + txt7.Text + ".0' , `Status`='" + (txt8.SelectedIndex+1)  + "' WHERE BarcodeCol1='" + txtBarcode.Text.Split(',')[0] + "'", this.form1.mySqlConnection);
					}

					mySqlCommandUpdate.ExecuteNonQuery();
					printerFunction2(
								txtBarcode.Text,
								txtBarcode.Text.Split(',')[1].Replace('M', ' ').Trim(),
								txt1.Text.Replace('U', ' ').Trim(),
								txt2.Text,
								DateTime.Parse(txt3.Text).ToString("dd.MM.yyyy"),
								txt4.Text,
								txt5.Text,
								txt9.Text,
								txt6.Text,
								txt7.Text);

					lblResult.Text = "Güncelleme Başarılı !";
				}
				catch (Exception)
				{
					//throw;

                    lblResult.Text = "Güncelleme Başarısız"; ;
                }
			}
			else
			{
				try
				{
					mySqlCommandUpdate = new MySqlCommand("INSERT INTO `IAS_MSL_Tb`(`ID`, `Barcode`, `BarcodeCol1`, `BarcodeCol2`, `BarcodeCol3`, `BarcodeCol4`, `BarcodeCol5`, `BarcodeCol6`," +
						" `MSL`,`ReleaseDate`, `UsageStartDate`, `UsageEndDate`,`MSLDate`,`Status`) VALUES (" + 0 + ",'" + txtBarcode.Text.Trim() + "','" + txtBarcode.Text.Split(',')[0] + "'," +
						"'" + txtBarcode.Text.Split(',')[1].Replace('M',' ').Trim() + "','" + txtBarcode.Text.Split(',')[2] + "','" + txtBarcode.Text.Split(',')[3] + "','" + txtBarcode.Text.Split(',')[4] + "'," +
						"'" + txtBarcode.Text.Split(',')[5] + "','" + txt9.Text + "','" + DateTime.Parse(txt3.Text).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Parse(txt4.Text).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Parse(txt5.Text).ToString("yyyy-MM-dd HH:mm:ss") + "','"+txt6.Text+"'," + (txt8.SelectedIndex+1) + ")", this.form1.mySqlConnection);;
					mySqlCommandUpdate.ExecuteNonQuery();
					lblResult.Text = "Kaydetme Başarılı !";

					printerFunction2(
							txtBarcode.Text,
							txtBarcode.Text.Split(',')[1].Replace('M', ' ').Trim(),
							txt1.Text.Replace('U', ' ').Trim(),
							txt2.Text,
							DateTime.Parse(txt3.Text).ToString("dd.MM.yyyy"),
							txt4.Text,
							txt5.Text,
							txt9.Text,
							txt6.Text,
							txt7.Text);
				}
				catch (Exception)
				{
					//throw;
					lblResult.Text = "Kaydetme Başarısız"; ;
				}
			}



			clear();
		}

		private void btnGet_Click(object sender, EventArgs e)
		{
			if (txtBarcode.Text != "" && txtBarcode.Text.Split(',').Length == 6)
			{
				mySqlCommandUpdate = new MySqlCommand("Select * from IAS_MSL_Tb where BarcodeCol1= '" + txtBarcode.Text.Split(',')[0] + "'", this.form1.mySqlConnection);
				mySqlDataReader = mySqlCommandUpdate.ExecuteReader();
				if (mySqlDataReader.Read())
				{
					txt1.Text = (string)mySqlDataReader["BarcodeCol4"].ToString().Replace('U', ' ').Trim();
					txt2.Text = (string)mySqlDataReader["BarcodeCol6"].ToString().Replace('X', ' ').Trim();
					txt3.Text = (string)mySqlDataReader["ReleaseDate"].ToString().Trim();
					txt4.Text = (string)mySqlDataReader["UsageStartDate"].ToString();
					txt5.Text = (string)mySqlDataReader["UsageEndDate"].ToString();
					txt6.Text = (string)mySqlDataReader["MSLDate"].ToString();
					txt7.Text = (string)mySqlDataReader["BarcodeCol5"].ToString().Replace('Q', ' ').Replace(".0", " ").Trim();
					txt8.Text = status((string)mySqlDataReader["Status"].ToString());
					txt9.Text = (string)mySqlDataReader["MSL"].ToString();
					isInsert = true;
				}
	
				else
				{
					txt1.Text = txtBarcode.Text.Split(',')[3].Replace('U', ' ').Trim();
					txt2.Text = txtBarcode.Text.Split(',')[5].Replace('X', ' ').Trim();
					txt3.Text = DateTime.Now.ToString("dd.MM.yyyy");
					txt4.Text = DateTime.Now.ToString();
					txt5.Text = DateTime.Now.ToString();
					txt6.Text = "0:0";
					txt7.Text = "0";
					txt8.Text = txt8.Items[0].ToString();
					txt9.Text = "1";
					isInsert = false;
				}
				mySqlDataReader.Close();

			}
		}
		private string status(string status)
		{
			if (status=="1")
			{
				return "İlk Kullanım";

			}else if (status == "2")
			{
				return "Dolap Giriş";
			}
			else
			{
				return "Dolaptan Cıkış";
			}
		}
		private void clear()
		{
			txt1.Text = "";
			txt2.Text = "";
			txt3.Text = "";
			txt4.Text = "";
			txt5.Text = "";
			txt6.Text = "";
			txt7.Text = "";
			txt8.Text = "";
			txt9.Text = "";
			txtBarcode.Text = "";
		}
		public bool printerFunction2(string barcode, string eps, string product_date, string expiration_date, string package_date, string cabine_out_date, string cabine_expiration_date, string msl, string life_time, string quantity)  //PRINTER AKSİYON
		{
			try
			{
				string start1 = "^XA";
				string start2 = "^LH" + Setting.Default.printerPos + "^FX";
				string font1 = "^CF0,20";
				string veri0 = "^FO130,10" + "^FD" + "NEM DOLABI TAKIP ETIKETI" + "^FS";
				string line1 = "^FO5,40^GB300,1,1^FS" + "^FO5,160^GB300,1,1^FS" + "^FO5,40^GB1,120,1^FS" + "^FO305,40^GB1,121,1^FS";
				string font2 = "CF0,15";
				string veri1 = "^FO10,50" + "^FD" + eps + "^FS";
				string veri2 = "^FO10,80" + "^FDURETIM TARIHI:" + product_date + "^FS";
				string veri3 = "^FO10,110" + "^FDSON KULLANMA TARIHI:" + expiration_date + "^FS";
				string veri4 = "^FO10,140" + "^FDPAKET ACILMA TARIHI:" + package_date + "^FS";
				//font1 tekrar
				string line2 = "^FO5,180^GB300,2,1^FS" + "^FO5,240^GB300,2,1^FS" + "^FO5,180^GB2,60,1^FS" + "^FO305,180^GB2,61,1^FS";
				string veri5 = "^FO50,190" + "^FDDOLAPTAN CIKIS TARIHI" + "^FS";
				string veri6 = "^FO60,215" + "^FD" + cabine_out_date + "^FS";
				string line3 = "^FO5,250^GB300,2,1^FS" + "^FO5,310^GB300,2,1^FS" + "^FO5,250^GB2,60,1^FS" + "^FO305,250^GB2,61,1^FS";
				string veri7 = "^FO12,260" + "^FDDOLAP DISI SON KULLANIM TARIHI" + "^FS";
				string veri8 = "^FO60,285" + "^FD" + cabine_expiration_date + "^FS";

				string line4 = "^FO312,40^GB160,1,1^FS" + "^FO312,160^GB160,1,1^FS" + "^FO312,40^GB1,120,1^FS" + "^FO472,40^GB1,121,1^FS";
				string veri9 = "^FO317,50" + "^FDMSL:" + msl + "^FS";
				string veri10 = "^FO317,90" + "^FDKALAN SURE:" + life_time + "^FS";
				string veri11 = "^FO317,130" + "^FDADET:" + quantity + "^FS";

				string qr = "^FO330,170" + "^BQN,3,3" + "^FDQA," + barcode + "^FS";  //QR
				string end = "^XZ";
				string test = start1 + start2 + font1 + veri0 + line1 + font2 + veri1 + veri2 + veri3 + veri4 + font1 + line2 + veri5 + veri6 + line3 + veri7 + veri8 + line4 + veri9 + veri10 + veri11 + qr + end;

				//Get local print server
				var server = new LocalPrintServer();

				//Load queue for correct printer
				PrintQueue pq = server.GetPrintQueue(Setting.Default.printerName, new string[0] { });
				PrintJobInfoCollection jobs = pq.GetPrintJobInfoCollection();
				foreach (PrintSystemJobInfo job in jobs)
				{
					job.Cancel();
				}

				if (!RawPrinterHelper.SendStringToPrinter(Setting.Default.printerName, test))
				{
					//otherConsoleAppendLine("Printer Error1: ", Color.Red);
					CustomMessageBox.ShowMessage("Printer Error1 !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
				}
				return true;
			}
			catch (Exception ex)
			{
				//otherConsoleAppendLine("Printer Error2: " + ex.Message, Color.Red);
				CustomMessageBox.ShowMessage("Printer Error2 !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
				return false;
			}
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{

		}
	}
}
