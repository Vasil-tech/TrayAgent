using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient; //Библиотека для работы с MS SQL-сервером
using ManagedClient; //Библиотека Миронова А. для ИРБИС64


namespace TrayAgent
{
	public partial class FormLogins : Form
	{
		public FormLogins()
		{
			InitializeComponent();
		}

		private void UserLogin_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
				this.UserPassword.Select();
		}

		private void UserPassword_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
				this.button1.Select();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		public void button1_Click(object sender, EventArgs e)
		{
			string un = "";
			string up = "";

			un = this.UserLogin.Text;
			up=this.UserPassword.Text;

			 ST.clientEK = new ManagedClient64();

			if (un.Length>0 && up.Length>0)
			{
				ST.clientEK.Host = CNST.esbo_serv;
				ST.clientEK.Port = CNST.esbo_port;
				ST.clientEK.Username =un ;
				ST.clientEK.Password = up;
				ST.clientEK.Database = "IBIS";


				try
				{
					ST.clientEK.Connect();
					this.txtStatus.ForeColor = System.Drawing.Color.Black;
					
					//Определяем параметр клиента CLIENT_TIME_LIVE
					//для последующего использования в NoOp в форме FormEKZ
					CNST.CLIENT_TIME_LIEVE=ST.clientEK.Settings.Get<int>
								  (
									"Main",
									"CLIENT_TIME_LIVE",
									2
								  );

				   
				   // this.txtStatus.Text = "Успешно." + CNST.CLIENT_TIME_LIEVE; ;

					CNST.userlogin = un;

					this.Close();

					//Открытие формы авторизации пользователя

					Form frmEKZ = new FormEKZ();
					frmEKZ.Show();
					
				}
				catch
				{
					this.txtStatus.ForeColor = System.Drawing.Color.Red;
					this.txtStatus.Text = "Не удается подключиться к серверу ЕБДЧ!";
				}
			}

		}

		private void FormLogins_Load(object sender, EventArgs e)
		{
			if (ST.clientEK == null) //Первое окрытие
			{
				return;
			}
			
			//Если соединение уже было установлено, нет смысла
			//второй раз авторизоваться.
			//Просто проверяем и, при необходимости, восстанавливаем соединение.

			bool frmIsOpen = false;
			Form frmEKZ_OP=null;
				
			foreach (Form f in Application.OpenForms) //Просто проверяем, не открыта ли ф орма
			{
				if (f.Name == "FormEKZ")
				{
					frmIsOpen = true;
					frmEKZ_OP = f;
					break;
				}
			}	
			
			try
			{
				ST.clientEK.Reconnect();
				txtStatus.ForeColor = System.Drawing.Color.Black;
			}
			catch
			{
				txtStatus.ForeColor = System.Drawing.Color.Red;
				txtStatus.Text = "Не удается подключиться к серверу ЕБДЧ!";
			}

			try
			{
				if (!frmIsOpen)
				{
					Form frmEKZ = new FormEKZ();
					Close();
					frmEKZ.Show();
				}
				else
				{
					Close();
					frmEKZ_OP.Show();
					frmEKZ_OP.WindowState = FormWindowState.Normal;
				}
			}
			catch
			{ }  

		}
	}
}
