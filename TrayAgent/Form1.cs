using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices; //Для использования DLLImport
using System.Data.SqlClient; //Библиотека для работы с MS SQL-сервером
using ManagedClient; //Библиотека Миронова А. для ИРБИС64

namespace TrayAgent
{
	public class Form1 : Form
	{
		public int COUNTER = 0; //Счетчик запусков для управлением оповещениями
		
		private NotifyIcon notifyIcon1;
		private TextBox textBox0;
		private TextBox textBox1;
		private TextBox textBox11;
		private TextBox[,] TXTB;
		private System.Windows.Forms.Timer timer1;
		/// <summary>
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent()
		{
			TXTB = new TextBox[CNST.NS,4];
			for (int i = 0; i < CNST.NS; i++)
				for (int j=0; j < 4;j++ )
					TXTB[i,j] = new TextBox();
			  
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.notifyIcon1 = new NotifyIcon(this.components);
			
			this.textBox11 = new TextBox();
			this.textBox1 = new TextBox();
			this.textBox0 = new TextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			//             
			// textBox1
			// 
			this.textBox1.AcceptsTab = false;
			this.textBox1.BackColor = SystemColors.Control;
			this.textBox1.Cursor = Cursors.No;
			this.textBox1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
			this.textBox1.ForeColor = Color.Red;
			this.textBox1.Location = new Point(6, 7);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(536, 84);
			this.textBox1.TabIndex = 2;
			this.textBox1.BorderStyle = BorderStyle.None;
			this.textBox1.ReadOnly = true;


			for (int i = 0; i < CNST.NS; i++)
				for (int j=0; j < 4; j++)
				{
					this.TXTB[i,j].BackColor = SystemColors.Control;
					this.TXTB[i,j].Name = "textBox_"+((i+1)*4+j);
					this.TXTB[i,j].BorderStyle = BorderStyle.None;
					this.TXTB[i,j].TabIndex = (i+1)*4+j + 5;
					this.TXTB[i, j].ReadOnly = true;

					int K = 0, L = 0;

					switch (j)
					{
						case 0:
							K = 6; L=150; break;
						case 1:
							K = 156; L=150;  break;
						case 2:
							K = 306; L=86;  break;
						case 3:
							K = 392; L=150;  break;
					}

					this.TXTB[i,j].Location = new Point(K, 100 + i * 25);
					this.TXTB[i,j].Size = new Size(L, 22);
				
				}

			// 
			// textBox11
			// 
			this.textBox11.BackColor = SystemColors.Control;
			this.textBox11.Location = new System.Drawing.Point(6, 335);
			this.textBox11.Font = new Font(textBox11.Font, FontStyle.Bold);
			this.textBox11.Name = "textBox11";
			this.textBox11.Size = new System.Drawing.Size(536, 22);
			this.textBox11.TabIndex = 12;
			this.textBox11.BorderStyle = BorderStyle.None;
			this.textBox11.TextAlign = HorizontalAlignment.Left;
			this.textBox11.ReadOnly = true;
			// 
			// textBox0
			// 
			this.textBox0.BackColor = System.Drawing.SystemColors.Control;
			this.textBox0.Location = new System.Drawing.Point(6, 358);
			this.textBox0.Name = "textBox0";
			this.textBox0.Size = new System.Drawing.Size(536, 1);
			this.textBox0.TabIndex = 1;
			this.textBox0.BorderStyle = BorderStyle.None;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(553, 361);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ControlBox = true;

			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.textBox11);

			for (int i = 0; i < CNST.NS; i++)
				for(int j=0;j<4;j++)
				{
					this.Controls.Add(this.TXTB[i,j]);
				}

			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.ShowInTaskbar = false;
			this.Text = CNST.APPTITLE();
			this.TopMost = true;
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Text = CNST.APPTITLE();
			this.notifyIcon1.Visible = true;
			// 
			// timer1
			// 
			this.timer1.Interval = 60000;
			this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
			this.Load += new System.EventHandler(this.Form1_FormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

			this.Hide();

		}
		
		//Иконка встроена в exe-файл
		public Icon mico = Icon.ExtractAssociatedIcon("TrayAgent.exe ");
		public bool EXIT = false; //флаг безусловного закрытия приложения
		public string snotif;

		public int scSQL;
		public int scIRB;
		public int scLOC;

		void EXIT_APP() //Обеспечивает безусловный выход из приложения
		{
			EXIT = true;
			Application.Exit();
		}
 
		public Form1()
		{ //аргументы, используемые только для обновления
			InitializeComponent();

			this.Shown += new EventHandler(Form1_Shown);

			timer1.Tick += timer1_Tick;
			//предотвращаем закрытие формы при нажатии на крест
			this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
			notifyIcon1.Click += notifyIcon1_Click;
			notifyIcon1.Icon = mico;
			// задаем иконку всплывающей подсказки
			notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
			// задаем текст подсказки
			notifyIcon1.BalloonTipText = "Проверка доступа к серверам книговыдачи и статистики...";
			// устанавливаем зголовк
			notifyIcon1.BalloonTipTitle = CNST.APPTITLE();
			//notifyIcon1.Text = "ЕСБО СПБ. Монитор";
			// отображаем подсказку 6 секунд
			notifyIcon1.ShowBalloonTip(6);
			notifyIcon1.Visible = true;
			notifyIcon1.ContextMenu = new ContextMenu(
				new[]
					{   
						new MenuItem("Прием",(s,e) => this.EKZ50_Click()),
						new MenuItem("Показать", (s, e) => this.FormShow()),
						new MenuItem("Выход", (s, e) => this.EXIT_APP()),
					}
			);
		}

		void Form1_Shown(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void SetText(int K, TcpServer SRV)
		{
			TXTB[K, 0].Text = SRV.capt;
			TXTB[K, 1].Text = SRV.host;
			TXTB[K, 2].Text = SRV.port.ToString();

			if (SRV.succ)
			{
				TXTB[K, 3].Text = "ДОСТУПЕН";
				for (int j = 0; j < 4; j++)
					if (!TXTB[K, j].Font.Bold)
						TXTB[K, j].Font = new Font(TXTB[K, j].Font, TXTB[K, j].Font.Style ^ FontStyle.Bold);
			}
			else
			{
				TXTB[K, 3].Text = "НЕДОСТУПЕН";
				for (int j = 0; j < 4; j++)
					if (TXTB[K, j].Font.Bold)
						TXTB[K, j].Font = new Font(TXTB[K, j].Font, TXTB[K, j].Font.Style ^ FontStyle.Bold);
			}
		  
		}

		private void SetAllTexts(TcpServer[] S)
		{
			//Сначала доступные, затем остальные
			//По сути - сортируем по succ
			
			TcpServer buf=new TcpServer();
			
			//Сортировка: сначала доступные, затем- остальные
			//Это искажает результаты иногда, т. к. подцепляется не
			//основной ip для теста
			/*19.04.2018 1.2.2
			for (int k = CNST.NS - 1; k > 0; k--)
				for (int i = 0; i < k; i++)
					if ((!S[i].succ) && S[i + 1].succ)
					{
						buf = S[i];
						S[i] = S[i + 1];
						S[i + 1] = buf;
					}
			*/

			TcpServer[] H = new TcpServer[CNST.NS];
			for (int k = 0; k < CNST.NS; k++) 
				H[k] = S[k];

			//И сортируем только H
			for (int k = CNST.NS - 1; k > 0; k--)
			{
				for (int i = 0; i < k; i++)
					if ((!H[i].succ) && S[i + 1].succ)
					{
						buf = H[i];
						H[i] = H[i + 1];
						H[i + 1] = buf;
					}
			}

			for (int k = 0; k < CNST.NS; k++)
				SetText(k, H[k]);    
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			//Мониторинг временно выключен для удобства пользователя
			//формы FormEKZ
			if (!CNST.monitor)
			{
				//timer1.Interval = 86440000;
				return;
			}

			//timer1.Interval = 60000;
			COUNTER++;
			timer1.Stop();
			
			#region ОСНОВНАЯ ЧАСТЬ ПРИЛОЖЕНИЯ
			
			#region Проверка доступности

			TESTSERVERS.RUN(CNST.servers, out CNST.servers);
					  

			scSQL = 0;
			scIRB = 0;
			scLOC = 0;

			for (int k = 0; k < CNST.NS; k++)
			{
				if (CNST.servers[k].succ)
				{
					if (CNST.servers[k].port == TESTSERVERS.SQLport)
						scSQL++;
					else if (CNST.servers[k].capt.IndexOf("ЕБДЧ")>=0)
						scIRB++;
					else
						scLOC++;
				}
			}

			if (scSQL > 0 && scIRB > 0 && (scLOC > 0 || (scLOC == 0 && !CNST.LOCSERVER)))
			{
				if (COUNTER % 60 == 1)
					ShowNotification(0, "Все серверы доступны. Успешной работы.", 6);
			}
			else if (scSQL == 0 && scIRB == 0 && (scLOC == 0 && CNST.LOCSERVER))
			{
				if (CNST.AlertN != 1)
				{ 
					CNST.AlertN = 1;
					ShowMessage(2, "Серверы книговыдачи, статистики и ЭК не доступны!");
					this.FormShow();
				}
				if (COUNTER % 4 == 1)
					ShowNotification(2, "Серверы книговыдачи, статистики и ЭК не доступны!", 6);
			}
			else if (scSQL == 0 && scIRB == 0)
			{
				if (CNST.AlertN != 2)
				{
					CNST.AlertN = 2;
					ShowMessage(2, "Серверы книговыдачи и статистики не доступны!");
					this.FormShow();
				}
				if (COUNTER % 4 == 1)
					ShowNotification(2, "Серверы книговыдачи и статистики не доступны!", 6);
			}
			else if (scSQL == 0 && (scLOC == 0 && CNST.LOCSERVER))
			{
				if (CNST.AlertN != 3)
				{
					CNST.AlertN = 3;
					ShowMessage(2, "Серверы статистики и ЭК не доступны!");
					this.FormShow();
				}
				if (COUNTER % 4 == 1)
					ShowNotification(2, "Серверы статистики и ЭК не доступны!", 6);
			}
			else if (scIRB == 0 && (scLOC == 0 && CNST.LOCSERVER))
			{
				if (CNST.AlertN != 4)
				{
					CNST.AlertN = 4;
					ShowMessage(2, "Серверы книговыдачи и ЭК не доступны!");
					this.FormShow();
				}
				if (COUNTER % 4 == 1)
					ShowNotification(2, "Серверы книговыдачи и ЭК не доступны!", 6);
			}
			else if (scIRB == 0)
			{
				if (CNST.AlertN != 5)
				{
					CNST.AlertN = 5;
					ShowMessage(2, "Сервер книговыдачи НЕДОСТУПЕН!");
					this.FormShow();
				}
				if (COUNTER % 4 == 1)
					ShowNotification(2, "Сервер книговыдачи НЕДОСТУПЕН!", 6);
			}
			else if (scSQL == 0)
			{
				if (CNST.AlertN != 6)
				{
					CNST.AlertN = 6;
					ShowMessage(2, "Сервер статистики НЕДОСТУПЕН!");
				}                       
				if (COUNTER % 4 == 1)
					ShowNotification(2, "Сервер статистики НЕДОСТУПЕН!", 6);
			}
			else if (scLOC == 0 && CNST.LOCSERVER)
			{
				if (CNST.AlertN != 7)
				{
					CNST.AlertN = 7;
					ShowMessage(2, "Сервер ЭК НЕДОСТУПЕН!");
				}
				if (COUNTER % 4 == 1)
					ShowNotification(2, "Сервер ЭК НЕДОСТУПЕН!", 6);
										}

			#endregion Проверка доступности
			
			//Заполнение результатов в форме
			SetAllTexts(CNST.servers);
 
			#region Проверка оповещения

			string alert0 = "";
			string alert_sig = "";

			TESTSERVERS.ALERT(CNST.servers, CNST.SIGLA, out alert0, out alert_sig);

			if (alert0.Length > 0)
			{
				if (COUNTER % 4 == 1)
					ShowNotification(2, alert0, 4);

				if (alert0 != CNST.ALERTER0)
				{
					CNST.ALERTER0 = alert0;
					ShowMessage(3, alert0);
				}
			}
			
			if (alert_sig.Length > 0)
			{
				if (COUNTER % 4 == 1)
					ShowNotification(2, alert_sig, 4);

				if (alert_sig != CNST.ALERTER_SIG)
				{
					CNST.ALERTER_SIG = alert_sig;
					ShowMessage(3, alert_sig);
				}
			}
			
			if ((alert0.Length == 0) && (alert_sig.Length == 0)) 
				 textBox1.Text = "";

			#endregion Проверка оповещения
			
			#region Проверка времени отклика
			
			//Оцениваем как отклик с локального компьютера
			//так и данные сервера книговыдачи

			int Tmax, Tavg, Tmax0, Tavg0;
			string sTmax0, sTavg0;
			string host00 = ""; string statserver = ""; int statport = 0;
					   
			//Тестовая операция для сервера книговыдачи
			//по первому доступному значению host
			//MessageBox.Show("Проверка времени отклика", "DEBUG", 0);

			Tmax = 0; Tmax0 = 0; Tavg = 0; Tavg0 = 0;
			
			//Конечно, только в случае доступности сервера
			//Проводится каждые 5 интервалов таймера
			//для уменьшения нагрузки на сервер ЕБДЧ

			if ((scIRB > 0) && (scSQL>0) && (COUNTER % 4 == 1))
			{
				//Первый доступный хост
				for (int k = 0; k < CNST.NS; k++)
					if (CNST.servers[k].port == TESTSERVERS.IRBport && CNST.servers[k].succ && CNST.servers[k].capt.IndexOf("ЕБДЧ")>=0)
					{
						host00 = "host=" + CNST.servers[k].host + ";port=" + CNST.servers[k].port + ";" +
							"user=trayagent;password=trayagent_2018;DB=RDR";
						break;
					}
				//Аналогично - первый доступный сервер статистики
				for (int k = 0; k < CNST.NS; k++)
					if (CNST.servers[k].port == TESTSERVERS.SQLport && CNST.servers[k].succ)
					{
						statserver = CNST.servers[k].host;
						statport=CNST.servers[k].port;
						break;
					}

				try
				{
					using (ManagedClient64 client = new ManagedClient64())
					{
						client.ParseConnectionString(host00);
						client.Connect();
							
						//Данные сервера книговыдачи
						//в полях 2 и 3 первой записи БД ALERT
						string cnStr = "User ID=trayagent;Password=trayagent_2018;" +
								"Data Source =" + statserver + "," + statport + ";" +
								"Initial Catalog=NEWSTAT;Connection Timeout=300 ";

						try
						{
							using (SqlConnection con = new SqlConnection(cnStr))
							{
								con.Open(); 
								string sExec = "EXECUTE [dbo].[pTrayAgentGet] "
											   + "@TYPE=1";
								SqlCommand cmd = new SqlCommand(sExec, con);

								Tmax0 = (Int32)cmd.ExecuteScalar();

								sExec = "EXECUTE [dbo].[pTrayAgentGet] "
											  + "@TYPE=0";
								cmd.CommandText = sExec;

								Tavg0 = (Int32)cmd.ExecuteScalar();
								   
							}

						}
						catch (Exception ex)
						{ 
							LOG.DoAccessLog("2\t" + cnStr + ex.Message);  
						}
						//Данные с сервера получены (или 0 при неудаче)
						//Теперь мониторинг с удаленного кoпьютера

						Stopwatch sw = new Stopwatch();
							long MTmax = 0;
							long MTavr = 0;
							long ms;

						for (int i = 0; i < 2; i++)
						{
							sw.Reset();
							sw.Start();

							IrbisProcessInfo[] IPINFO = client.GetProcessList();
							IrbisVersion IRVER = client.GetVersion();
							int mn = client.GetMaxMfn();

							sw.Stop();
							ms = sw.ElapsedMilliseconds;

							MTmax = Math.Max(MTmax, ms);
							MTavr = MTavr + ms;

							Thread.Sleep(1000);
						}

						Tmax = (Int32)MTmax;
						Tavg = (Int32)(MTavr / 2);
														
						LOG.DoAccessLog( "\t" + Tmax + "\t" + Tavg + "\t" + Tmax0 + "\t" + Tavg0);
							
						//client.PopDatabase();

						if (CNST.WRITESQL)
						{
						LOG.WriteLog(statserver,
							statport, CNST.SIGLA, Tmax, Tavg, Tmax0, Tavg0);
						}
					}
				}
				catch
				{
						
				}

				//Все данные Tmax, Tmax0, Tavg , Tavg0 получены
				//Приступаем к анализу (сравнению с пороговыми цифрами)

				//Упрощенный анализ
				//Если Tmax или Tavg превосходят некоторый порог,
				//появляется уведомление

				if (Tmax >= 1000 || Tavg >= 250 || Tmax0 >= 800 || Tavg0 >= 200)
					ShowNotification(1, "Возможны задержки при работе с сервером книговыдачи!", 10);

				//Вывод в нижнюю строку Form1

				textBox11.Text = "Tavg/Tmax, ms.\t с этого ПК: " + Tavg + "/" + Tmax +
					", сервер ЕБДЧ: " + Tavg0 + "/" + Tmax0 + ".";

			}

			#endregion Проверка времени отклика
			
			#endregion ОСНОВНАЯ ЧАСТЬ ПРИЛОЖЕНИЯ

			timer1.Start();
		}

		//23.04.2021
		//РАСШИРЕНИЕ ВОЗМОЖНОСТЕЙ ПРИЛОЖЕНИЯ
		//Смена статусов экземпляров с 5 на 0 при вводе инвентарного номера/метки

		private void EKZ50_Click()
		{ //Открытие формы авторизации пользователя
			Form frm2 = new FormLogins();
			frm2.Show();
		}

		void notifyIcon1_Click(object sender, EventArgs e) { }


		//void notifyIcon1_BalloonTipClicked(object sender, EventArgs e) { }

		void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!EXIT)
			{
				e.Cancel = true; //Отменяем закрытие формы при нажатии на крест
				this.Hide();
			}
			else
				Application.Exit();
		}

		void Form1_FormLoad(object sender, EventArgs e)
		{
			timer1_Tick(sender, e);
		}

		void FormShow()
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
			this.textBox0.Select();
		}

		//Работа с Notification
		public void ShowNotification(int tip, string txt, int tm)
		{
			//Показ всплывающего сообщения
			//tip 0-инфо, 1 - предупр, 2 - ошибка
			//txt - текст сообщения (максимум 127 символов)
			//tm - время показа
			LOG.DoAccessLog( "\tУВЕДОМЛЕНИЕ\t" + txt);
						
			if (!notifyIcon1.Visible)
				notifyIcon1.Visible = true;

			// задаем иконку всплывающей подсказки
			switch (tip)
			{
				case 0:
					notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
					break;
				case 1:
					notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
					break;
				case 2:
					notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
					break;
				default:
					notifyIcon1.BalloonTipIcon = ToolTipIcon.None;
					break;
			}

			// задаем текст подсказки
			notifyIcon1.BalloonTipText = txt;
			// отображаем подсказку tm секунд
			notifyIcon1.ShowBalloonTip(tm);        
		}

		//Работа с сообщениями
	
		public void ShowMessage(int tip, string txt)
		{
			//Показ всплывающего сообщения
			//tip 0-инфо, 1 - предупр, 2 - ошибка, 3 - тоже, что и 2, но с отображением 
			// в textbox1
			//txt - текст сообщения (максимум 127 симвоволов)
			
			MessageBoxButtons buttonsOK = MessageBoxButtons.OK;
            DialogResult result;

            if (CNST.WRITELOG)
				LOG.DoAccessLog("СООБЩЕНИЕ:\t" + txt);

			switch (tip)
			{
				case 0:
					MessageBox.Show(txt, CNST.APPTITLE(), buttonsOK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
					break;
				case 1:
					MessageBox.Show(txt, CNST.APPTITLE(), buttonsOK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
					break;
				case 2:
					MessageBox.Show(txt, CNST.APPTITLE(), buttonsOK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
					break;
				case 3:
					MessageBox.Show(txt, CNST.APPTITLE(), buttonsOK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
					textBox1.Text = txt;
					break;
				default:
                    MessageBox.Show(txt, CNST.APPTITLE(), buttonsOK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
					break;
			}
		}     
	}
}
