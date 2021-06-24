using ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrayAgent
{
    public class ST
    {
        //Все, что связано с приемом книг в месте хранения
        //Соединение, которое остается активным (connected) после авторизации
        //в форме FormLogins
        //Закрытие соединения происходит либо при нажатии кнопки Выход
        //в форме FormStatus, либо при выходе из приложения
        //Или при каких-либо исключениях, связанных с этим соединением
        public static ManagedClient64 clientEK;
    }
}
