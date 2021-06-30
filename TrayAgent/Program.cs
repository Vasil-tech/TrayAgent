using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices; //Для использования DLLImport
using System.Security.Cryptography;
using System.Data.SqlClient; //Библиотека для работы с MS SQL-сервером
using ManagedClient; //Библиотека Миронова А. для ИРБИС64

namespace TrayAgent
{
	static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{

			#region Comment for parameters App.config
			//  Все параметры в App.config
			//      сигла
			//       сигла адрес_сервера
			//       и включение записи в журнал
			//    <appSettings>
			//    <add key="bibsigla" value="06"/>
			//    <add key="esbo-server" value="s00.libs.spb.ru"/>
			//    <add key="esbo-port" value="6666"/>
			//    <add key="stat-server" value="stat.libs.spn.ru"/>
			//    <add key="stat-port" value="1433"/>
			//    <add key="loc-server" value="10.168.33.34"/>
			//    <add key="loc-port" value="6666"/>
			//    <add key="write-log" value="True"/>
			//    <add key="write-sql" value="True"/>
			//    </appSettings>
			//    В командной строке:
			//       u имя_файла
			//       d имя_файла
			#endregion

			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				
				bool onlyInstance; //Обеспечиваем однократный запуск приложения

				//Mutex mtx = new Mutex(true, "TrayAgent", out onlyInstance);

				Process[] Ps0 = Process.GetProcessesByName("TrayAgent");
				int n0 = Ps0.Length;

				onlyInstance = (n0 <= 1);
				try
				{
					if (onlyInstance)
					{
						// Инициализация и запуск модуля обновления
						try
						{
							FormUpdater.Check(args);
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.Message);
						}

						if (FormUpdater.is_skipped)
						{
							//Обновление выполнено либо не требовалось
							// … запускаем основную программу
							try
							{    
								CNST.loc_serv = CM.GetString("loc-server", "");

								CNST.SIGLA = CM.GetInt32("bibsigla", 0);
								CNST.WRITELOG = CM.GetBoolean("write-log", false);
								CNST.WRITESQL = CM.GetBoolean("write-sql", false);

								if (!(CNST.SIGLA > 0 && CNST.SIGLA < 22))
								{
									MessageBox.Show("Не указана или указана не верно сигла/код библиотечной системы! Параметр \"bibsigla\" в файле TrayAgent.exe.config");
									return;
								}

								//Первыми заполняются серверы из config
								//loc_server уже заполнен выше
								CNST.esbo_serv = CM.GetString("esbo-server", "irbis.libs.spb.ru");
								CNST.stat_serv = CM.GetString("stat-server", "stat.libs.spb.ru");

								CNST.loc_port = CM.GetInt32("loc-port", 6666);
								CNST.esbo_port = CM.GetInt32("esbo-port", 6666);
								CNST.stat_port = CM.GetInt32("stat-port", 1433);

								if (CNST.loc_serv.Length == 0 || CNST.loc_port <= 0)
								{
									CNST.NS = 8;
									CNST.LOCSERVER = false;
								}
								else
								{
									CNST.NS = 9;
									CNST.LOCSERVER = true;
								}

								//Все TextBox в виде массива, кроме специальных 0, 1 и 11
								//На каждый сервер 4 элемента

								CNST.servers = new TcpServer[CNST.NS];

								CNST.servers[0] = new TcpServer(CNST.esbo_serv, CNST.esbo_port, false, "ИРБИС ЕБДЧ");
								CNST.servers[1] = new TcpServer(CNST.stat_serv, CNST.stat_port, false, "СТАТИСТИКА");

								//Распределяем оставшиеся

								switch (CNST.esbo_serv)
								{
									case "irbis.libs.spb.ru":
										CNST.servers[2] = new TcpServer("172.29.67.70", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[3] = new TcpServer("libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[4] = new TcpServer("10.168.33.29", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[5] = new TcpServer("s00.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										break;
									case "172.29.67.70":
										CNST.servers[2] = new TcpServer("irbis.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[3] = new TcpServer("libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[4] = new TcpServer("10.168.33.29", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[5] = new TcpServer("s00.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										break;
									case "libs.spb.ru":
										CNST.servers[2] = new TcpServer("172.29.67.70", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[3] = new TcpServer("irbis.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[4] = new TcpServer("10.168.33.29", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[5] = new TcpServer("s00.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										break;
									case "10.168.33.29":
										CNST.servers[2] = new TcpServer("172.29.67.70", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[3] = new TcpServer("libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[4] = new TcpServer("irbis.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[5] = new TcpServer("s00.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										break;
									case "s00.libs.spb.ru":
										CNST.servers[2] = new TcpServer("172.29.67.70", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[3] = new TcpServer("libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[4] = new TcpServer("10.168.33.29", 6666, false, "ИРБИС ЕБДЧ");
										CNST.servers[5] = new TcpServer("irbis.libs.spb.ru", 6666, false, "ИРБИС ЕБДЧ");
										break;
								}
								switch (CNST.stat_serv)
								{
									case "stat.libs.spb.ru":
										CNST.servers[6] = new TcpServer("172.29.67.69", 1433, false, "СТАТИСТИКА");
										CNST.servers[7] = new TcpServer("194.186.155.14", 1433, false, "СТАТИСТИКА");
										break;
									case "172.29.67.69":
										CNST.servers[6] = new TcpServer("stat.libs.spb.ru", 1433, false, "СТАТИСТИКА");
										CNST.servers[7] = new TcpServer("194.186.155.14", 1433, false, "СТАТИСТИКА");
										break;
									case "194.186.155.14":
										CNST.servers[6] = new TcpServer("172.29.67.69", 1433, false, "СТАТИСТИКА");
										CNST.servers[7] = new TcpServer("stat.libs.spb.ru", 1433, false, "СТАТИСТИКА");
										break;
								}

								//Сервер локального ЭК
								if (CNST.LOCSERVER)
									CNST.servers[CNST.NS - 1] = new TcpServer(CNST.loc_serv, CNST.loc_port, false, "СЕРВЕР ЭК");

								Application.Run(new Form1());
							}
							catch (Exception ex)
							{
								MessageBox.Show(ex.Message);
								return;
							}
						}
					}
					else
					{
						MessageBox.Show("Приложение уже запущено", CNST.APPTITLE(), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
					return;
				}
			}
			catch
			{ }
		}
	}
}