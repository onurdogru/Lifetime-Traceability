using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
	public partial class Form1 : Form
	{
		//IAS MSL CONNECTİON / 
		SqlConnection sqlConnection;
		SqlCommand sqlCommand;
		SqlDataReader sqlDataReader;



		//php kısmı / bizim kendi dbmiz
		public MySqlConnection mySqlConnection;
		MySqlCommand mySqlCommandUpdate;
		MySqlDataReader mySqlDataReader2;
		MySqlDataReader mySqlDataReader3;
		MySqlDataReader mySqlDataReader4;


		public string mslValue="";

		public Form1()
		{
			InitializeComponent();

		
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			
			//dateCalculate(DateTime.Now,13,5);
			connectionIASDatabase();
			connectionMySQLDatabase();
			//findMSLDate("6");
			//MessageBox.Show(mslValue);
			tabPageVisible();


			this.txtBarcodeInsert.TabIndex = 1;
			this.txtBarcodeInsert.Focus();

		}



		private bool connectionIASDatabase()
		{
			string server = @"192.168.10.22";
			string database = "ALP802";
			string user = "otomasyon";
			string pass = "123KUM*";
			String connection = @"Data Source=" + server + ";Initial Catalog=" + database + ";User ID=" + user + ";Password=" + pass;
			sqlConnection = new SqlConnection(connection);
			try
			{
				sqlConnection.Open();
				return true;
			}
			catch (Exception)
			{
				CustomMessageBox.ShowMessage("Veritabanı bağlantısı kurulamadı !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
				return false;
			}
		}
		private void connectionMySQLDatabase()
		{
			string conn = "server=192.168.0.41;uid=alparge;pwd=1q2w3e4R;database=IAS_Traceability_DB";
			mySqlConnection = new MySqlConnection(conn); //Örnek alınmış.
			mySqlConnection.Open();
			if (mySqlConnection.State == ConnectionState.Open)
			{
			}
			else
			{
				CustomMessageBox.ShowMessage("Veritabanı bağlantısı kurulamadı !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
			}

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			sqlConnection.Close();
			mySqlConnection.Close();
		}

		private string spliteBarcodeCode(TextBox textBox)
		{
			
				int epsLenght = (textBox.Text.Split(',')[1].Length) - 1;
				if (textBox.Text.Split(',')[1].Substring(0, 1) == "M")
				{
					return textBox.Text.Split(',')[1].Substring(1, epsLenght);
				}
				else
				{
					return textBox.Text.Split(',')[1];
				}
		}
		private bool barcodeControl(TextBox textBox)
		{
			if (textBox.Text.Split(',').Length==6)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		private string findMSL(string MSL)
		{
			if (MSL == "0")
				return "0";
			else if (MSL == "1")
				return "1";
			else if (MSL == "2")
				return "2";
			else if (MSL == "3")
				return "2A";
			else if (MSL == "4")
				return "3";
			else if (MSL == "5")
				return "4";
			else if (MSL == "6")
				return "5";
			else if (MSL == "7")
				return "5A";
			else if (MSL == "8")
				return "6";
			else
				return "0";
		}
		private string findMSLDate(string MSL)
		{
			if (MSL == "0")
				return "0:0";
			else if (MSL == "1")
				return "999999:00";
			else if (MSL == "2")
				return "8766:00";
			else if (MSL == "2A")
				return "672:00";
			else if (MSL == "3")
				return "168:00";
			else if (MSL == "4")
				return "72:00";
			else if (MSL == "5")
				return "48:00";
			else if (MSL == "5A")
				return "24:00";
			else if (MSL == "6")
			{
				MSLDateForm mSLDateForm = new MSLDateForm(this);
				mSLDateForm.ShowDialog();
				return mslValue+":00";
			}
				
			else
				return "0:0";
		}
		private bool entryCheck(string reelNum)
		{
			MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM `IAS_MSL_Tb` WHERE `BarcodeCol1`='"+reelNum+"'", mySqlConnection);
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			if (mySqlDataReader.Read())
			{
				mySqlDataReader.Close();
				mySqlCommand.Dispose();
				return true;
			}
			else
			{
				mySqlDataReader.Close();
				mySqlCommand.Dispose();
				return false;
			}

		}

		private string dateCalculate(DateTime enteredDate, double hour,double minute)
		{
			DateTime newDate= enteredDate.AddHours(hour);
			newDate = newDate.AddMinutes(minute);
			return newDate.ToString("dd.MM.yyyy HH:mm:ss");
		}
		private void btnEkle_Click(object sender, EventArgs e)
		{
			if (barcodeControl(txtBarcodeInsert))
			{
				if (txtBarcodeInsert.Text != "")
				{
					sqlCommand = new SqlCommand("Select MSL from IASMATBASIC where MATERIAL Like '%" + spliteBarcodeCode(txtBarcodeInsert) + "%'", sqlConnection);
					sqlDataReader = sqlCommand.ExecuteReader();
					if (sqlDataReader.Read())
					{
						if (!entryCheck(txtBarcodeInsert.Text.Split(',')[0]))
						{
							MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO `IAS_MSL_Tb`(`ID`, `Barcode`, `BarcodeCol1`, `BarcodeCol2`, `BarcodeCol3`, `BarcodeCol4`, `BarcodeCol5`, `BarcodeCol6`, `MSL`,`ReleaseDate`, `UsageStartDate`, `UsageEndDate`,`MSLDate`,`Status`) VALUES (" + 0 + ",'" + txtBarcodeInsert.Text.Trim() + "','" + txtBarcodeInsert.Text.Split(',')[0] + "','" + txtBarcodeInsert.Text.Split(',')[1].Replace('M', ' ').Trim() + "','" + txtBarcodeInsert.Text.Split(',')[2] + "','" + txtBarcodeInsert.Text.Split(',')[3] + "','" + txtBarcodeInsert.Text.Split(',')[4] + "','" + txtBarcodeInsert.Text.Split(',')[5] + "','" + findMSL((string)sqlDataReader["MSL"]) + "',now(),now(),NULL,'" + findMSLDate(findMSL((string)sqlDataReader["MSL"])) + "'," + 1 + ")", mySqlConnection);
							try
							{
								mySqlCommand.ExecuteNonQuery();
								lblResult.ForeColor = Color.Green;
								lblResult.Text = "Kayıt eklendi";

								string remainderDate = dateCalculate(DateTime.Now, double.Parse(findMSLDate(findMSL((string)sqlDataReader["MSL"])).Split(':')[0]), 0);
								printBarcode(txtBarcodeInsert.Text.Split(',')[3].Replace('U', ' ').Trim(), txtBarcodeInsert.Text.Split(',')[5].Replace('X', ' ').Trim(), DateTime.Now.ToString(), DateTime.Now.ToString(), findMSL((string)sqlDataReader["MSL"]), findMSLDate(findMSL((string)sqlDataReader["MSL"])) + " Saat", txtBarcodeInsert.Text.Split(',')[4].Replace('Q', ' ').Trim(), remainderDate, lbl1P1, lbl2P1, lbl3P1, lbl4P1, lbl5P1, lbl6P1, lbl7P1, lbl8P1);
								printerFunction2(
									txtBarcodeInsert.Text,
									txtBarcodeInsert.Text.Split(',')[1].Replace('M', ' ').Trim(),
									txtBarcodeInsert.Text.Split(',')[3].Replace('U', ' ').Trim(), 
									txtBarcodeInsert.Text.Split(',')[5].Replace('X', ' ').Trim(), 
									DateTime.Now.ToString("dd.MM.yyyy"),
									DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
									remainderDate, 
									findMSL((string)sqlDataReader["MSL"]), 
									findMSLDate(findMSL((string)sqlDataReader["MSL"])).Split(':')[0], 
									txtBarcodeInsert.Text.Split(',')[4].Replace('Q', ' ').Trim());
							}
				
							catch (Exception)
							{
								lblResult.ForeColor = Color.Red;
								lblResult.Text = "Kayıt ekleme işlemi başarısız";
							}
							txtBarcodeInsert.Text = "";
							mslValue = "";
						}
						else
						{
							CustomMessageBox.ShowMessage("Aynı ürün tekrar eklenemez !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
						}

					}
					else
					{
						CustomMessageBox.ShowMessage("EPS kodu mevcut değildir !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
					}
					sqlDataReader.Close();
					timer1.Enabled = true;
				}
			}
			else
			{
				CustomMessageBox.ShowMessage("Lütfen doğru barkot tanıtın !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
			}


		}
		private void btnGetMLS_Click(object sender, EventArgs e)
		{
			/*DATE_FORMAT(Date,'%d/%m/%Y %h:%i ') AS Date*/
			if (barcodeControl(txtEPSCode) && txtUnit.Text!="")
			{
				MySqlCommand mySqlCommand = new MySqlCommand("Select BarcodeCol1,MSL,UsageStartDate,now() as UsageEndDate,ReleaseDate,MSLDate,Status,BarcodeCol4 from IAS_MSL_Tb where BarcodeCol1= '" + txtEPSCode.Text.Split(',')[0] + "' ", mySqlConnection);
				mySqlDataReader2 = mySqlCommand.ExecuteReader();
				if (mySqlDataReader2.Read())
				{
					if (mySqlDataReader2["Status"].ToString() == "1" || mySqlDataReader2["Status"].ToString() == "3")
					{
						//DateTime release = (DateTime)mySqlDataReader2["ReleaseDate"];
						DateTime start = (DateTime)mySqlDataReader2["UsageStartDate"];
						DateTime end = (DateTime)mySqlDataReader2["UsageEndDate"];
						var diffTicks = (end - start);
						string day = diffTicks.Days.ToString();
						string hour = diffTicks.Hours.ToString();
						string minute = diffTicks.Minutes.ToString();

						string mslHourUsage = ((int.Parse(day) * 24) + int.Parse(hour)).ToString();
						string floorLife = (string)mySqlDataReader2["MSLDate"];
						int remainderFloorLifeHour = int.Parse(floorLife.Split(':')[0]) - int.Parse(mslHourUsage);
						int remainderFloorLifeMinute = int.Parse(floorLife.Split(':')[1]) - (int.Parse(minute));

						if (remainderFloorLifeMinute < 0)
						{
							remainderFloorLifeHour = remainderFloorLifeHour - 1;
							remainderFloorLifeMinute = 60 + remainderFloorLifeMinute;
						}


						if (remainderFloorLifeHour > 0)
						{

							mySqlCommandUpdate = new MySqlCommand("UPDATE `IAS_MSL_Tb` SET `UsageEndDate`= now() , `MSLDate`='" + remainderFloorLifeHour + ":" + remainderFloorLifeMinute + "',BarcodeCol5='Q"+txtUnit.Text+".0' , `Status`=2 WHERE BarcodeCol1='" + (string)mySqlDataReader2["BarcodeCol1"] + "'", mySqlConnection);
						//	lblDate.Text = day + " Gün " + hour + " Saat " + minute + " Dakika";
						//	lblFloorLife.Text = remainderFloorLifeHour + ":" + remainderFloorLifeMinute;
							lblResultP2.Text = "Dolaba giriş işlemi başarılı !";
							lblResultP2.ForeColor = Color.Green;

							string remainderDate = dateCalculate(start, remainderFloorLifeHour, remainderFloorLifeMinute);
							printBarcode(txtEPSCode.Text.Split(',')[3].Replace('U', ' ').Trim(), txtEPSCode.Text.Split(',')[5].Replace('X', ' ').Trim(), mySqlDataReader2["ReleaseDate"].ToString(), /*start.ToString()*/"-", (string)mySqlDataReader2["MSL"], remainderFloorLifeHour + ":" + remainderFloorLifeMinute + " Saat", txtUnit.Text + ".0", "-", lbl1P2, lbl2P2, lbl3P2, lbl4P2, lbl5P2, lbl6P2, lbl7P2, lbl8P2);
							printerFunction2(
									txtEPSCode.Text,
									txtEPSCode.Text.Split(',')[1].Replace('M', ' ').Trim(),
									txtEPSCode.Text.Split(',')[3].Replace('U', ' ').Trim(),
									txtEPSCode.Text.Split(',')[5].Replace('X', ' ').Trim(),
									DateTime.Parse((string)mySqlDataReader2["ReleaseDate"]).ToString("dd.MM.yyyy"),
									" ",
									" ",
									mySqlDataReader2["MSL"].ToString(),
									remainderFloorLifeHour.ToString(),
									txtUnit.Text + ".0");

							mySqlDataReader2.Close();
							try
							{
								mySqlCommandUpdate.ExecuteNonQuery();
							}
							catch (Exception ex)
							{
								CustomMessageBox.ShowMessage(ex.ToString(), "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
								lblResultP2.Text = "Dolaba giriş işlemi başarısız !";
								lbl6P2.Text = "-";
								//lblDate.Text = "-";
								lblResultP2.ForeColor = Color.Red;
							}
						}
						else
						{
							CustomMessageBox.ShowMessage("Zemin Ömrü Geçmiştir. Sorumluya Bilgi Veriniz!", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
							//mySqlCommandUpdate = new MySqlCommand("UPDATE `IAS_MSL_Tb` SET `UsageEndDate`= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' , `MSLDate`='0:0',`Status`=2 WHERE BarcodeCol1='" + (string)mySqlDataReader2["BarcodeCol1"] + "'", mySqlConnection);
							//lblDate.Text = day + " Gün " + hour + " Saat " + minute + " Dakika";
							//	lblFloorLife.Text = remainderFloorLifeHour + ":" + remainderFloorLifeMinute;
							lblResultP2.Text = "Zemin Ömrü Geçmiştir !";
							lblResultP2.ForeColor = Color.Red;
						}
					}
					else
					{
						lblResultP2.Text = "Ürün dolapta bulunmaktadır !";
						lbl6P2.Text = "-";
						//lblDate.Text = "-";
						lblResultP2.ForeColor = Color.Red;

					}
				}
				else
				{
					lblResultP2.Text = "-";
					//lblDate.Text = "-";
					CustomMessageBox.ShowMessage("EPS kodu mevcut değildir !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
				}
				txtEPSCode.Text = "";
				mySqlDataReader2.Close();
				timer1.Enabled = true;
			}
			else
			{
				CustomMessageBox.ShowMessage("Lütfen doğru barkot ve adet bilgisi giriniz !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
			}
		}
	
		private void btnExit_Click(object sender, EventArgs e)
		{
			if (barcodeControl(txtEPSCodeP3))
			{
				MySqlCommand mySqlCommand = new MySqlCommand("Select BarcodeCol1,MSL,BarcodeCol5,UsageStartDate,now() AS UsageEndDate,ReleaseDate,MSLDate,Status from IAS_MSL_Tb where BarcodeCol1= '" + txtEPSCodeP3.Text.Split(',')[0] + "'", mySqlConnection);
				mySqlDataReader3 = mySqlCommand.ExecuteReader();
				if (mySqlDataReader3.Read())
				{
					if (mySqlDataReader3["Status"].ToString() == "2")
					{
						string floorLife = (string)mySqlDataReader3["MSLDate"];
						DateTime endDate = DateTime.Now;
						string releaseDate = DateTime.Parse((string)mySqlDataReader3["ReleaseDate"]).ToString("dd.MM.yyyy");
						mySqlCommand = new MySqlCommand("UPDATE `IAS_MSL_Tb` SET `UsageStartDate`= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',`Status`=3 WHERE BarcodeCol1='" + (string)mySqlDataReader3["BarcodeCol1"] + "'", mySqlConnection);
						string remainderDate = dateCalculate(endDate, int.Parse(floorLife.Split(':')[0]), int.Parse(floorLife.Split(':')[1]));
						printBarcode(txtEPSCodeP3.Text.Split(',')[3].Replace('U', ' ').Trim(), txtEPSCodeP3.Text.Split(',')[5].Replace('X', ' ').Trim(), mySqlDataReader3["ReleaseDate"].ToString(), DateTime.Now.ToString(), (string)mySqlDataReader3["MSL"], floorLife.Split(':')[0] + ":" + floorLife.Split(':')[1] + " Saat", (string)mySqlDataReader3["BarcodeCol5"].ToString().Replace('Q', ' ').Trim(), remainderDate, lbl1P3, lbl2P3, lbl3P3, lbl4P3, lbl5P3, lbl6P3, lbl7P3, lbl8P3);
                        printerFunction2(
                        txtEPSCodeP3.Text,
                        txtEPSCodeP3.Text.Split(',')[1].Replace('M', ' ').Trim(),
						txtEPSCodeP3.Text.Split(',')[3].Replace('U', ' ').Trim(),
						txtEPSCodeP3.Text.Split(',')[5].Replace('X', ' ').Trim(),
						releaseDate,
						endDate.ToString("dd.MM.yyyy HH:mm:ss"),
                        remainderDate,
                        mySqlDataReader3["MSL"].ToString(),
                        floorLife.Split(':')[0],
						(string)mySqlDataReader3["BarcodeCol5"].ToString().Replace('Q', ' ').Trim());

						mySqlDataReader3.Close();
						try
						{
							mySqlCommand.ExecuteNonQuery();
							lblResultP3.Text = "Dolaptan çıkarma işlemi başarılı !";
							lblResultP3.ForeColor = Color.Green;
						}
						catch (Exception ex)
						{
							CustomMessageBox.ShowMessage(ex.ToString(), "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
							lblResultP3.Text = "Dolaptan çıkırma işlemi başarısız !";
							lblResultP3.ForeColor = Color.Red;
						}
					}
					else
					{
						lblResultP3.Text = "Ürün dolapta bulunmamaktadır !";
						lblResultP3.ForeColor = Color.Red;
					}

				}
				else
				{
					lblResultP3.Text = "-";
					CustomMessageBox.ShowMessage("EPS kodu mevcut değildir !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
				}
				txtEPSCodeP3.Text = "";
				timer1.Enabled = true;
			}
			else
			{
				CustomMessageBox.ShowMessage("Lütfen doğru barkot tanıtın !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
			}
			mySqlDataReader3.Close();
		}

		private void printBarcode(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8 , Label label1, Label label2, Label label3, Label label4, Label label5, Label label6 ,Label label7, Label label8)
		{
			label1.Text = "Üretim Tarihi : " + s1;
			label2.Text = "Son Kullanma Tarihi : " + s2;
			label3.Text = "Paket Açılma Tarihi : " + s3;
			label4.Text = "Dolaptan Çıkış Tarihi : " + s4;
			label5.Text = "MSL : " + s5;
			label6.Text = "Kalan Süre : " + s6;
			label7.Text = "Adet : " + s7;
			label8.Text = "Dolap Dışı Son K. Tarihi : " + s8;
		}
		private void clearBarcode(Label label1, Label label2, Label label3, Label label4, Label label5, Label label6, Label label7, Label label8)
		{
			label1.Text = "Üretim Tarihi : -";
			label2.Text = "Son Kullanma Tarihi : -";
			label3.Text = "Paket Açılma Tarihi : -";
			label4.Text = "Dolaptan Çıkış Tarihi : -";
			label5.Text = "MSL : -";
			label6.Text = "Kalan Süre : -";
			label7.Text = "Adet : -";
			label8.Text = "Dolap Dışı Son K. Tarihi : -";
			txtUnit.Text = "";
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
					CustomMessageBox.ShowMessage("Printer Error1", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
				}
				return true;
			}
			catch (Exception ex)
			{
				//otherConsoleAppendLine("Printer Error2: " + ex.Message, Color.Red);
				CustomMessageBox.ShowMessage("Printer Error2", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
				return false;
			}
				
		}

		int count = 0;
		private void timer1_Tick(object sender, EventArgs e)
		{
			count++;
			if (count==10)
			{
				timer1.Enabled = false;
				lblResult.Text = "-";
				lblResult.ForeColor = Color.White;
				lblResultP2.Text = "-";
				lblResultP2.ForeColor = Color.White;
				lblResultP3.Text = "-";
				lblResultP3.ForeColor = Color.White;
				clearBarcode(lbl1P1, lbl2P1, lbl3P1, lbl4P1, lbl5P1, lbl6P1, lbl7P1, lbl8P1);
				clearBarcode(lbl1P2, lbl2P2, lbl3P2, lbl4P2, lbl5P2, lbl6P2, lbl7P2, lbl8P2);
				clearBarcode(lbl1P3, lbl2P3, lbl3P3, lbl4P3, lbl5P3, lbl6P3, lbl7P3, lbl8P3);
				count = 0;
			}
		
		}

		private void btnGiris_Click(object sender, EventArgs e)
		{

			if (txtUser.Text == "admin" && txtPass.Text == Setting.Default.password)
			{
				Admin admin = new Admin(this);
				admin.ShowDialog();

			}
			else
			{
				CustomMessageBox.ShowMessage("Yanlış kullanıcı adı veya şifre !", "IAS_MSL_TRACIBILITY", MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
			}

		}
		private void tabPageVisible()
		{

			if (!Setting.Default.page1Visible)
				tabControl1.TabPages.Remove(tabPage1);
			if (!Setting.Default.page2Visible)
				tabControl1.TabPages.Remove(tabPage2);
			if (!Setting.Default.page3Visible)
				tabControl1.TabPages.Remove(tabPage3);

			//OD
			if (!Setting.Default.buttonSIL)
			{
				buttonSIL.Visible = false;
				btnRefresh.Location = new System.Drawing.Point(529, 437);
			}
			else
			{
				buttonSIL.Visible = true;
				btnRefresh.Location = new System.Drawing.Point(419, 437);
			}


		}

		private void label20_Click(object sender, EventArgs e)
		{

		}

		private void lblFloorLife_Click(object sender, EventArgs e)
		{

		}

		private void txtUnit_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}

		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			
			listele();
		}

		private void listele()
		{
			MySqlCommand mySqlCommand = new MySqlCommand("SELECT `ID` as ID, `BarcodeCol2` as MalzemeKodu,replace(BarcodeCol4, 'U', '') as ÜretimTarihi ,replace(BarcodeCol5, 'Q', '') as Adet,replace(BarcodeCol6, 'X', '') as SonKullanmaTarihi,`MSL`,`ReleaseDate` as PaketAçılmaTarihi ,`UsageStartDate`as DolabGiriş,`UsageEndDate` as DolapÇıkış,`MSLDate`as KalanSüre,CASE WHEN Status = 2 THEN 'Nem Alma Kabininde' END Durum  FROM `IAS_MSL_Tb` WHERE Status = 2 ORDER by ID DESC", mySqlConnection);
			//MySqlCommand mySqlCommand = new MySqlCommand("SELECT `BarcodeCol2` as EPS,replace(BarcodeCol4, 'U', '') as ÜretimTarihi , replace(BarcodeCol5, 'Q', '') as Adet,replace(BarcodeCol6, 'X', '') as SonKullanmaTarihi,`MSL`,`ReleaseDate` as PaketAçılmaTarihi ,`UsageStartDate`as DolabGiriş,`UsageEndDate` as DolapÇıkış,`MSLDate`as KalanSüre,CASE WHEN Status = 1 THEN 'Üretimde' WHEN Status = 3 THEN 'Üretimde' ELSE 'Nem Alma Kabininde' END Durum  FROM `IAS_MSL_Tb` ORDER by ID DESC", mySqlConnection);
			MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
			DataTable dataTable = new DataTable();
			mySqlDataAdapter.Fill(dataTable);
			dataGridView1.DataSource = dataTable;
		}

		
		public void verisil(int id)
		{
			string sqlQueryDelete = "DELETE FROM IAS_MSL_Tb" + " WHERE ID=" + id;
			MySqlCommand mySqlCommand = new MySqlCommand(sqlQueryDelete, mySqlConnection);
			mySqlDataReader4 = mySqlCommand.ExecuteReader();
			mySqlDataReader4.Close();

			listele();
		}

		private void tabPage1_Click(object sender, EventArgs e)
		{

		}

		private void txtEPSCode_TextChanged(object sender, EventArgs e)
		{

		}

		private void tabPage2_Click(object sender, EventArgs e)
		{

		}

		private void txtBarcodeInsert_TextChanged(object sender, EventArgs e)
		{

		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			
		}


		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtBarcodeInsert.Text = "";
			txtEPSCode.Text = "";
			txtEPSCodeP3.Text = "";
			txtUser.Text = "";
			if (tabControl1.SelectedIndex == 0) 
			{
				txtBarcodeInsert.Focus();
			}
			else if(this.tabControl1.SelectedIndex == 1)
			{
				txtEPSCode.Focus();
			}
			else if (this.tabControl1.SelectedIndex == 2)
			{
				txtEPSCodeP3.Focus();
			}
			else if (this.tabControl1.SelectedIndex == 4)
			{
				txtUser.Focus();
			}
		}

		private void buttonSIL_Click(object sender, EventArgs e)
		{

			
			foreach (DataGridViewRow drow in dataGridView1.SelectedRows)
			{
				int id = Convert.ToInt32(drow.Cells[0].Value);
				verisil(id);

			}
			
		}

		private void tabPage4_Click(object sender, EventArgs e)
		{

		}
	}
	
}
