using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices; //Для использования DLLImport
using System.Threading.Tasks;

namespace TrayAgent
{
	public static class FormUpdater
	{
		public static string url_version = "http://libs.spb.ru/TrayAgent/version.txt"; //Ссылка на txt-файл версии
		public static string url_program = "http://libs.spb.ru/TrayAgent/TrayAgent.exe"; //Ссылка на exe-файл программы
		public static string url_md5 = "http://libs.spb.ru/TrayAgent/TrayAgent.exe.md5"; //Ссылка на сайт

		public static string my_filename;   // Имя файла запущенной программы 
		public static string up_filename;  // Имя временного файла для загрузки обновления
		public static bool is_skipped;   // Признак, что обновление не требуется или закончено

        //public static async Task CheckAsync(string[] keys)
        //{
        //    await Task.Run(() => Check(keys));
        //}

        public static void Check(string[] keys)
		{
			try
			{
				my_filename = GetExecFilename(); // Получаем имя запущенной программы (без полного пути)
				up_filename = "new." + my_filename; // Формируем имя временного файла

				//Специальные виды запуска для выполнения обновления
				//третьим будет исходный параметр запуска, который сохраняется
				//при работе обновления:
				// d <filename> <сигла> <пусто либо сервер ЭК>
				// u <filename> <сигла> <пусто либо сервер ЭК>

				if (string.Concat(keys).IndexOf("TrayAgent") < 0)
					DoCheckUpdate(keys); //Аргументов для обновления нет – проверим версию на сервере
				else
				{
					if (keys[0] == "u")  // Запущена новая версия из временного файла
						DoCopyDownloadedProgram(keys[1], keys);

					if (keys[0] == "d")  //Осталось удалить временный файл.
						DoDeleteOldProgram(keys[1]);
				}
			}
			catch (Exception ex) 
			{ 
				MessageBox.Show("Check err " + ex.Message + " " + string.Concat(keys)); 
			}
		}

		//static async Task<string> GetExecFilenameAsync()
		//{
		//	await Task.Run(() => GetExecFilename());
		//}

		public static string GetExecFilename()
		{
			string filename = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

			if (filename.Length > 0)
				return filename;
			else
				return "";
		}

		//static async Task DoCheckUpdateAsync(string[] keys)
		//{
		//	await Task.Run(() => DoCheckUpdate(keys));
		//}

		public static void DoCheckUpdate(string[] keys)
		{
			string up_version = GetServerVersion(); // получаем номер версии программы на сервере

			if (CNST.my_version == up_version) // Если обновление не нужно
				is_skipped = true;   // Пропускаем модуль обновления
			else
				DownloadFile(keys); // Запускаем скачивание новой версии		
		}

		public static string GetServerVersion()
		{
			try
			{
				WebClient webClient = new WebClient();
				return webClient.DownloadString(url_version).Trim();
			}
			catch
			{   // Если номер версии не можем получить,
				return CNST.my_version;  // то программу даже и не будем пытаться.
			}
		}

		//static async Task DownloadFileAsync(string[] keys)
		//{
		//	await Task.Run(() => DownloadFile(keys));
		//}

		public static void DownloadFile(string[] keys)
		{
			try
			{
				WebClient webClient = new WebClient();
				string hash = "";
				string hash0 = webClient.DownloadString(url_md5).Trim();

				if (hash0.Length == 32)
				{
					int K = 5; //Число попыток чтения файла                    

					do
					{
						webClient.DownloadFile(new Uri(url_program), up_filename); // Начинаем скачивание
						hash = HashFunction.MD5File(up_filename);
						K--;
					} while (hash != hash0 && K > 0);

					if (hash == hash0)
					{
						is_skipped = false; // Основную программу не нужно запускать

						RunProgram(up_filename, "u \"" + my_filename + "\""); //Два варианта: задана только сигла либо сигла и сервер ЭК
					}
					else //скачивание не удалось, используем старую версию
						is_skipped = true;
				}
				else //скачивание не удалось, либо некорректаня md5, используем старую версию
					is_skipped = true;
			}
			catch (Exception ex)
			{  // В случае ошибки выводим сообщение и предлагаем скачать вручную
				MessageBox.Show("download_file" + ex.Message + " " + up_filename);
			}
		}

        //static async Task RunProgramAsync(string filename, string keys)
        //{
        //    await Task.Run(() => RunProgram(filename, keys));
        //}

        public static void RunProgram(string filename, string keys)
		{
			try
			{   
				System.Diagnostics.Process proc = new System.Diagnostics.Process(); // Использование системных методов для запуска программы
				proc.StartInfo.WorkingDirectory = Application.StartupPath;
				proc.StartInfo.FileName = filename;
				proc.StartInfo.Arguments = keys; // Аргументы командной строки
				proc.Start(); // Запускаем!
			}
			catch (Exception ex)
			{
				MessageBox.Show("run_program" + ex.Message + " " + filename);
			}
		}

		//static async Task DoCopyDownloadedProgramAsync(string filename, string[] keys)
		//{
		//	await Task.Run(() => DoCopyDownloadedProgram(filename, keys));
		//}

		public static void DoCopyDownloadedProgram(string filename, string[] keys)
		{
			TryToDeleteFile(filename); // Удаляем файл со старой версией программы
			try
			{   
				File.Copy(my_filename, filename); // Копируем скачанный файл в оригинальное имя файла
				RunProgram(filename, "d \"" + my_filename + "\""); // Запускаем этап «Ц»
				is_skipped = false;  // Обновление ещё не закончено
			}
			catch (Exception ex)
			{
				MessageBox.Show("do_copy_downloaded_program  " + ex.Message + " " + filename);
			}
		}

		public static void TryToDeleteFile(string filename)
		{
			int loop = 10; // Количество попыток 
			while (--loop > 0 && File.Exists(filename))
				try
				{
					File.Delete(filename);
				}
				catch
				{
					Thread.Sleep(200); // Небольшая задержка
				}
		}

		//static async Task DoDeleteOldProgramAsync(string filename)
		//{
		//	await Task.Run(() => DoDeleteOldProgram(filename));
		//}

		public static void DoDeleteOldProgram(string filename)
		{
			TryToDeleteFile(filename);
			is_skipped = true; // Обновление отработало, запускайте!
		}
	}
}
