using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms; 
using ManagedClient; //Библиотека Миронова А. для ИРБИС64


namespace TrayAgent
{
	public partial class FormEKZ : Form
	{
		private Timer timerekz;

	   public FormEKZ()
		{
			InitializeComponent();
			
		}
		private void FormEKZ_Load(object sender, EventArgs e)
		{
			btnOK.Hide();

			Text = Text + "  " + CNST.userlogin;

			//Отключаем мониторинг
			CNST.monitor = false;
			//Формируем список БД ЭК, доступных клиенту,
			//используя формат
			//&uf('+7W1#'&uf('+9C1,'&uf('IMAIN,DBNNAMECAT,'))), (if g1:'%' then g1'|' fi)
			//Пример результата:
			/*
			  MAYAK%SERV21%|PERI%SERV21%|PRSFT%SERV21%|NOTA%SERV21%|MUSIC%SERV21%|OESPB%HOST03%|MBA%HOST03%|
			 */
			string catformat = "&uf('+7W1#'&uf('+9C1,'&uf('IMAIN,DBNNAMECAT,'))), (if g1:'%' then g1'|' fi)";

			if (!ST.clientEK.Connected)
				ST.clientEK.Reconnect();

			ST.clientEK.PushDatabase("IBIS");

			IrbisRecord r = ST.clientEK.SearchReadOneRecord("I=$");
			
			string dblist = ST.clientEK.FormatRecord(catformat, r.Mfn);

			string[] DB = dblist.Split('|');

			//Полные имена хранятся в DB, а их индексы в DB для каждого
			//элемента списка cmbxDB - в DBU  
			//Полное имя для получаем как элемент DB с индексом равным значению элемента
			//DBU с индексом равнм номеру
			//выбранного элемента списка cmbxDB

			for (int k = 0; k < DB.Length; k++)
			{
				if (DB[k].Length > 0)
				{
					cmbxDB.Items.Add(DB[k]);
				}
			}
			//Показываем первый элемент
			
			cmbxDB.SelectedIndex = 0;
			timerekz.Start();
		}

		private void FormEKZ_FormClosing(object sender, EventArgs e)
		{
			//Включаем мониторинг
			CNST.monitor = true;
		}

		private void FormEKZ_Timer(object sender, EventArgs e)
		{
			if (ST.clientEK.Connected)
				ST.clientEK.NoOp();
			else
				ST.clientEK.Reconnect();
		}

		private void SELECT_INV()
		{
			txtbINV.Focus();
			txtbINV.SelectAll();
		}

		private void MSG( char status, string messtext)
		{   //Вывод диагностики с выделением текста цветом
			txtMSG.Text = messtext;
			switch (status)
			{ 
				case 'e':
					txtMSG.ForeColor = System.Drawing.Color.Red;
					//this.txtMSG.Font = new Font(txtMSG.Font, FontStyle.Bold);
					break;
				case 'w':
					txtMSG.ForeColor = System.Drawing.Color.Green;
					break;
				default:
					txtMSG.ForeColor = System.Drawing.Color.Black;
					//this.txtMSG.Font = new Font(txtMSG.Font, FontStyle.Regular);
					break;
			}
		}

		private void txtbINV_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				string nh = txtbINV.Text;

				if ((!(nh == "")) && cmbxDB.Text.Length > 0)
				{   //При необходимости переключаемся на нужную БД
					if ((!(ST.clientEK.Database == cmbxDB.Text)) && (!(cmbxDB.Text.Substring(0, 1) == "-")))
					{
						ST.clientEK.PushDatabase(cmbxDB.Text);
					}
					//Поиск по словарю Инв. №/штрих-код
					try
					{
						int[] rec = ST.clientEK.Search("\"IN={0}\"", nh).ToArray();
						if (rec.Length == 0)
							txtbBOOK.Text = "Экземпляр не найден!";
						else if (rec.Length > 1)
							txtbBOOK.Text = "Неоднозначность экземпляра. Найдено записей " + rec.Length;
						else //Найдена единственная запись.
							//Правда, там еще может оказаться больше одного экземпляра с одинаковым инв. номером или меткой
						{
							IrbisRecord recEK = ST.clientEK.ReadRecord(rec[0]);
							txtbBOOK.Text = ST.clientEK.FormatRecord("@brief", rec[0]); ;
							//Обязательно проверяем, нет ли двух экземпляров с одинаковым инв. номером или меткой
							//BA5 - если трактовать nh, как инв. номер
							//HA5 - если — как метку
							var BA5 = recEK.Fields.GetField("910").GetField('a', "5").GetField('b',nh).ToArray();
						
							int Lb = BA5.Length;

							var HA5 = recEK.Fields.GetField("910").GetField('a', "5").GetField('h', nh).ToArray();
							
							int Lh = HA5.Length;

							if ((Lb+Lh)==0)
							{
								MSG('e', "Статус экземпляра не 5!");
								return;
							}
							else if (Lb>1 && Lh==0)  //неоднозначность по инв. номеру
							{
								MSG('e', "Дубли ("+Lb+ ") инвентарного номера!");
								return;
							}
							else if (Lb==0 && Lh>1)  //неоднозначность по метке
							{
								MSG('e',"Дубли ("+Lh+ ") метки/штрих-кода!");
								return;
							}
							else if (Lb > 1 && Lh > 1)  //неоднозначность полная
							{
								MSG('e', "Дубли по инв. номеру (" + Lb + ") и метке (" + Lh + ")!");
								return;
							}
							else  //Здесь один экз. по метке либо инвентарю
							{
								//Демонстрируем место хранения и КСУ
								string mx = ""; string ksu = "";
								if (Lb == 1 && Lh == 0)
								{
									mx = BA5[0].GetSubFieldText('d', 0);
									ksu = BA5[0].GetSubFieldText('u', 0);
								}
								else
								{
									mx = HA5[0].GetSubFieldText('d', 0);
									ksu = HA5[0].GetSubFieldText('u', 0); 
								}
								MSG('s', mx+"    "+ksu+" (место хр. КСУ)");
								btnOK.Show();
								btnOK.Select();
							}
						}
					}
					catch
					{
						MSG('e', "Ошибка при запросе к серверу!");
						btnOK.Hide();
						return;
					}
				}
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{ //Повторяет (для гарантии) все действия, которые выполнялись после
		 //ввода Enter в поле txtbINV

			string nh = txtbINV.Text;
			string mx = ""; string ksu = "";

			if (!(nh == ""))
			{   //При необходимости переключаемся на нужную БД
				if (!(ST.clientEK.Database == cmbxDB.Text))
					ST.clientEK.PushDatabase(cmbxDB.Text);
				//Поиск по словарю Инв. №/штрих-код
				int[] rec = null;

				try
				{
					rec = ST.clientEK.Search("\"IN={0}\"", nh).ToArray();
				}
				catch (Exception ex)
				{
					if (ST.clientEK.Connected)
					{
						txtbBOOK.Text = ex.Message+"\n\rЭкземпляр не найден! Поробуйте еще раз.";
						btnOK.Hide();
						SELECT_INV();
						return;
					}
					else
					{
						txtbBOOK.Text = ex.Message + "\n\rЭкземпляр не найден! Возможно, проблема с подключением к серверу."
											+"\n\rПопытка снова соединиться с сервером.";
						ST.clientEK.Connect();
						SELECT_INV();
						btnOK.Hide();
						return;
					}
				}

				if (rec.Length == 0)
				{ 
					txtbBOOK.Text = "Экземпляр не найден!";
					btnOK.Hide();
				}
				else if (rec.Length > 1)
				{ 
					txtbBOOK.Text = "Неоднозначность экземпляра. Найдено записей " + rec.Length;
					btnOK.Hide();
				}
				else //Найдена единственная запись.
					//Правда, там еще может оказаться больше одного экземпляра с одинаковым инв. номером или меткой
				{
					IrbisRecord recEK = null;

					try 
					{
						recEK = ST.clientEK.ReadRecord(rec[0]);
					}
					catch (Exception ex)
					{
						if (ST.clientEK.Connected)
						{
							txtbBOOK.Text = ex.Message + "\n\rОшибка при чтении записи. Поробуйте еще раз.";
							btnOK.Select();
							return;
						}							
						else
						{
							txtbBOOK.Text = ex.Message + "\n\rОшибка при чтении записи! Возможно, проблема с подключением к серверу."
												+ "\n\rПопытка снова соединиться с сервером.";
							ST.clientEK.Connect();
							btnOK.Hide();
							SELECT_INV();								
							return;
							}
						}

					txtbBOOK.Text = ST.clientEK.FormatRecord("@brief", rec[0]); ;
					//Обязательно проверяем, нет ли двух экземпляров с одинаковым инв. номером или меткой
					//BA5 - если трактовать nh, как инв. номер
					//HA5 - если — как метку
					var BA5 = recEK.Fields.GetField("910").GetField('a', "5").GetField('b', nh).ToArray();
					int Lb = BA5.Length;
					var HA5 = recEK.Fields.GetField("910").GetField('a', "5").GetField('h', nh).ToArray();
					int Lh = HA5.Length;

					if ((Lb + Lh) == 0)
					{
						MSG('e', "Статус экземпляра не 5!");
						btnOK.Hide();
						return;
					}
					else if (Lb > 1 && Lh == 0)  //неоднозначность по инв. номеру
					{
						MSG('e', "Дубли (" + Lb + ") инвентарного номера!");
						btnOK.Hide();
						return;
					}
					else if (Lb == 0 && Lh > 1)  //неоднозначность по метке
					{
						MSG('e', "Дубли (" + Lh + ") метки/штрих-кода!");
						btnOK.Hide();
						return;
					}
					else if (Lb > 1 && Lh > 1)  //неоднозначность полная
					{
						MSG('e', "Дубли по инв. номеру (" + Lb + ") и метке (" + Lh + ")!");
						btnOK.Hide();
						return;
					}
					else  //Здесь один экз. по метке либо инвентарю
					{
						MSG('s', "");
						//Собственно попытка редактирования статуса
						//Алгоритм меняется только в зависимости
						//от того, по инв. номеру найден экземпляр
						//или по метке
						string dn = DateTime.Now.ToString("yyyyMMdd H:mm:ss");
						RecordField F910 = null;
						if (Lb == 1 && Lh == 0) //по инв. номеру
						{	
							F910 = BA5[0];
						}
						else
						{
							F910 = HA5[0]; //Дальше работаем с F910							
							F910.SetSubField('a', "0");
							mx = F910.GetSubFieldText('d', 0);
							ksu = F910.GetSubFieldText('u', 0);
						}
							//Дополняем подполями
							//^L-логин пользователя,
							//^N - текущая дата, время

						if (F910.HaveNotSubField('l'))							{ 
							F910.AddSubField('l', CNST.userlogin);
						}
						else
						{
							F910.SetSubField('l', CNST.userlogin);
						}
						if (F910.HaveNotSubField('n'))
						{
							F910.AddSubField('n', dn);
						}
						else
						{ 
							F910.SetSubField('n', dn);
						}
							
						try  //Сохраняем запись
						{
							ST.clientEK.WriteRecord(recEK, false, true);
						}
						catch (Exception ex)
						{
							if (ST.clientEK.Connected)
							{
								txtbBOOK.Text = ex.Message + "\n\rОшибка при сохранении записи. Поробуйте еще раз.";
								btnOK.Select();
								return;
							}
							else
							{
								txtbBOOK.Text = ex.Message + "\n\rОшибка при сохранении записи! Возможно, проблема с подключением к серверу."
																+ "\n\rПопытка снова соединиться с сервером.";
								ST.clientEK.Connect();
								btnOK.Hide();
								SELECT_INV();
								return;
							}
						}
					}
					
					try //Вновь перечитываем
					{
						recEK = ST.clientEK.ReadRecord(rec[0]);
					}
					catch (Exception ex)
					{
						if (ST.clientEK.Connected)
						{
							txtbBOOK.Text = ex.Message + "\n\rОшибка при проверке записи. Поробуйте еще раз.";
							btnOK.Select();
							return;
						}
						else
						{
							txtbBOOK.Text = ex.Message + "\n\rОшибка при проверке записи! Возможно, проблема с подключением к серверу."
												+ "\n\rПопытка снова соединиться с сервером.";
							ST.clientEK.Connect();
							btnOK.Hide();
							SELECT_INV();
							return;
						}
					}
					//Для проверки ищем повторение по тем же критериям
					//результат должен быть пустой
					Lb = Lh = 0;
					
					if (Lb == 1 && Lh == 0) //по инв. номеру
					{
						BA5 = recEK.Fields.GetField("910").GetField('a', "5").GetField('b', nh).ToArray();
						Lb = BA5.Length;
					}
					else
					{
						HA5 = recEK.Fields.GetField("910").GetField('a', "5").GetField('h', nh).ToArray();
						Lh = HA5.Length;
					}
					//Если оба поиска не дали результатов, то все нормально
					if ((Lb + Lh) == 0)
					{
						MSG('w', "Успешно: "+mx+"    "+ksu+" (место хр., КСУ)");
						btnOK.Hide();
						SELECT_INV();
						return;
					}
					else
					{
						MSG('e', "Не удалось обновить статус!");
						btnOK.Hide();
						SELECT_INV();
						return;
					}
				}
			}
		}

		private void cbmxDB_SelectedIndexChanged(object sender, System.EventArgs e)
		{ 
			//Служит для сброса поля txtINV, txtBOOK и txtMSG при смене БД
			txtbINV.Text = "";
			txtbBOOK.Text = "";
			txtMSG.Text = "";
			btnOK.Hide();
			txtbINV.Select();
		}
	}
}
