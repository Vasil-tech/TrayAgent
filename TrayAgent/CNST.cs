using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrayAgent
{
    public class CNST
    {
        public const string my_version = "1.3.2"; // Текущая версия проекта, доступная для всего проекта
        //=========================================================        
        //28.04.2021
        //Ключ включения/отключения мониторинга
        //для работы с установкой статуса отключается, чтобы не мешал

        public static bool monitor = true;

        public static int CLIENT_TIME_LIEVE = 2; //Интервал CLIENT_TIME_LIEVE для клиента установки статуса 2 минуты  
        public static string userlogin = ""; //Логин пользователя
        public static int AlertN = 0;
        public static int SIGLA = 0; //Сигла/код библиотечной системы

        public static string ALERTER0 = "";    //"Наблюдатель за текстом сообщений"
        public static string ALERTER_SIG = ""; //"Наблюдатель за текстом сообщений"

        public static string loc_serv;
        public static string esbo_serv;
        public static string stat_serv;

        public static int loc_port;
        public static int esbo_port;
        public static int stat_port;

        public static bool WRITELOG; //Пишет в текстовый лог
        public static bool WRITESQL; //Пишет жупнал на сервере статистики
        public static int NS = 9; //Максимальное число опрашиваемых серверов
        public static bool LOCSERVER = false; //Задан сервер ЭК

        public static TcpServer[] servers;

        public static string APPTITLE() { return "ЕСБО СПБ Монитор. v. " + my_version; }
    }
}
