using System;
using System.Net.Sockets;
using System.Data.SqlClient; //Библиотека для работы с MS SQL-сервером
using ManagedClient; //Библиотека Миронова А. для ИРБИС64
using System.Threading.Tasks;

namespace TrayAgent
{
	public static class TESTSERVERS //Описание и работа с данными серверов
	{ 
		public const int SQLport = 1433;
		public const int IRBport = 6666;

		public static async Task RunAsync(TcpServer[] S, TcpServer[] ST)
		{
			await Task.Run(() => RUN(S, out ST));
		}

		public static void RUN(TcpServer[] S, out TcpServer[] ST)
		{ //Тест серверов на ДОСТУПНОСТЬ по IP и ПОРТУ.
			ST = new TcpServer[CNST.NS];
			for (int k = 0; k < CNST.NS; k++)
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					try
					{
						if (S[k].host.Length > 0 && S[k].port > 0)
							tcpClient.Connect(S[k].host, S[k].port);
					}
					catch
					{
						//LOG.DoAccessLog("4\t" + ex.Message);
					}

					if (tcpClient.Connected)
					{
						ST[k] = S[k];
						ST[k].succ = true;
					}
					else
					{
						ST[k] = S[k];
						ST[k].succ = false;
					}
				}
			}
			return;
		}

		//Проверка на наличие срочных оповещений
		public static void ALERT(TcpServer[] S, int sig, out string msg0, out string msgsig)
		{ 
			//Возвращает сообщение либо пустую строку
			//Если ни один из серверов НЕДОСТУПЕН,
			//возвращает сообщение об этом.
			//MessageBox.Show("AlERT","DEBUG", 0);
			//При доступности сервера статистики, берем оповещение с него
			//иначе с HOST00 (БД ALERT)
			msg0 = ""; msgsig = "";

			for (int k = 0; k < S.Length; k++)
			{
				if (S[k].succ && S[k].port == SQLport)
				{
					string cnStr = "User ID=trayagent;Password=trayagent_2018;" +
									"Data Source =" + S[k].host + "," + S[k].port + ";" +
									"Initial Catalog=NEWSTAT;Connection Timeout=300 ";
					try
					{
						using (SqlConnection con = new SqlConnection(cnStr))
						{
							con.Open();
							SqlCommand cmd = new SqlCommand("EXECUTE [dbo].[pALERT_GET] @S=0", con);
							msg0 = (string)cmd.ExecuteScalar();
							cmd.CommandText = "EXECUTE [dbo].[pALERT_GET] @S=" + sig;
							msgsig = (string)cmd.ExecuteScalar();
							break;
						}
					}
					catch
					{
						msg0 = ""; msgsig = "";
					}
				}
			}

			#region Commented out part of the method
			/*ЗДЕСЬ ДОЛЖНО БЫТЬ ОБРАЩЕНИЕ К http серверу за дублем данных
			if (!getmes)
				for (int k = 0; k < S.Length - 1; k++)
				{
					if (S[k].succ && S[k].port == IRBport && S[k].capt.IndexOf("ЕБДЧ") >= 0)
					{
						string cnIRBIS = "host=" + S[k].host + ";port=" + S[k].port + ";" +
							"user=trayagent;password=trayagent;DB=ALERT";
						try
						{
							using (ManagedClient64 client = new ManagedClient64())
							{
								client.ParseConnectionString(cnIRBIS);
								client.Connect();
								msg0 = client.FormatRecord("V1", 2); //MFN=2  !!!!!!!!!!
							}
							msgsig = "";
							getmes = true;
							break;
						}
						catch
						{
							//Console.WriteLine(ex.Message);
							getmes = false;
							msg0 = ""; msgsig = "";
						}
					}
				}
			
			
			if (!getmes) //нет доступа ник одному из важных серверов
				//книговыдачи и чтатичтики
				//Формируем сообщение.
				msg0 = "Нет доступа к серверам книговыдачи и статистики!";
		 */
			#endregion

		}
	}
}