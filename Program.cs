using System;
using System.IO;
using System.Net;

namespace MiniCrawler
{
   internal class Program
   {
      static void Main()
      {
         
         MiniCrawler();

         //UsingWebClientClass();

         Console.ReadKey();
      }

      // MiniCrawler - скелетный вариант поискового робота
      static void MiniCrawler()
      {
         string[] arg = { "https://www.mheducation.com" };
         string link;
         string str;
         string answer;
         // Содержит текущее положение в ответе
         int curloc;
         if (arg.Length != 1)
         {
            Console.WriteLine("MiniCrawler: {0}", arg[0]);
            return;
         }

         // Содержит текущий URI
         string uristr = arg[0];
         HttpWebResponse resp = null;
         try
         {
            do
            {
               Console.WriteLine("Переход по ссылке " + uristr);
               // Создать объект запроса типа WebRequest по указанному URI
               HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uristr);
               // Запретить дальнейшее использование этого URI
               uristr = null;
               // Отправить сформированный запрос и получить на него ответ
               resp = (HttpWebResponse)req.GetResponse();
               // Получить поток ввода из принятого ответа
               Stream istrm = resp.GetResponseStream();
               // Заключить поток ввода в оболочку класса StreamReader
               StreamReader rdr = new StreamReader(istrm);
               // Прочитать всю страницу
               str = rdr.ReadToEnd();
               curloc = 0;
               do
               {
                  // Найти следующий URI для перехода по ссылке
                  link = FindLink(str, ref curloc);
                  if (link != null)
                  {
                     Console.WriteLine("Найдена ссылка: " + link);
                     Console.Write("Перейти по ссылке, Искать дальше, Выйти? ");
                     answer = Console.ReadLine();
                     if (string.Equals(answer, "П", StringComparison.OrdinalIgnoreCase))
                     {
                        uristr = string.Copy(link);
                        break;
                     }
                     if (string.Equals(answer, "B", StringComparison.OrdinalIgnoreCase))
                     {
                        break;
                     }
                     if (string.Equals(answer, "И", StringComparison.OrdinalIgnoreCase))
                     {
                        Console.WriteLine("Поиск следующей ссылки");
                     }
                     else
                     {
                        Console.WriteLine("Больше ссылок не найдено");
                        break;
                     }
                  }
               } while (link.Length > 0);
               // Закрыть ответный поток
               if (resp != null)
               {
                  resp.Close();
               }
            } while (uristr != null);
         }
         catch (WebException exc)
         {
            Console.WriteLine("Сетевая ошибка: " + exc.Message + "\nКод состояния: " + exc.Status);
         }
         catch (ProtocolViolationException exc)
         {
            Console.WriteLine("Протокольная ошибка: " + exc.Message);
         }
         catch (UriFormatException exc)
         {
            Console.WriteLine("Ошибка формата URI: " + exc.Message);
         }
         catch (NotSupportedException exc)
         {
            Console.WriteLine("Неизвестный протокол: " + exc.Message);
         }
         catch (IOException exc)
         {
            Console.WriteLine("Ошибка ввода-вывода: " + exc.Message);
         }
         finally
         {
            if (resp != null)
            {
               resp.Close();
            }
         }

         Console.WriteLine("Завершение программы MiniCrawler");
      }

      // Найти ссылку в строке содержимого
      static string FindLink(string htmlstr, ref int startloc)
      {
         int i;
         int start, end;
         string uri = null;
         i = htmlstr.IndexOf("href=\"http", startloc, StringComparison.OrdinalIgnoreCase);
         if (i != -1)
         {
            start = htmlstr.IndexOf('"', i) + 1;
            end = htmlstr.IndexOf('"', start);
            uri = htmlstr.Substring(start, end - start);
            startloc = end;
         }

         return uri;
      }

      // Свойство LastModified - время обновления содержимого се­тевого ресурса в последний раз
      static void DateLastWebsiteUpdate()
      {
         string uri = "https://example.com";
         Console.WriteLine("LastModified {0}", uri);
         HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
         HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
         Console.WriteLine("Последняя модификация: " + resp.LastModified);
         resp.Close();
      }

      // Доступ к cookie-наборам
      static void AccessCookieSets()
      {
         string uri = "https://ya.ru/";
         Console.WriteLine("Cookie {0}", uri);
         // Создать объект запроса типа WebRequest по указанному URI
         HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
         // Получить пустой контейнер
         req.CookieContainer = new CookieContainer();
         // Отправить сформированный запрос и получить на него ответ
         HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
         // Отобразить cookie-наборы
         Console.WriteLine("Количество cookie-наборов: " + resp.Cookies.Count);
         Console.WriteLine("{0,-20}{1}", "Имя", "Значение");
         for (int i = 0; i < resp.Cookies.Count; i++)
         {
            Console.WriteLine("{0,-20}{1}", resp.Cookies[i].Name, resp.Cookies[i].Value);
         }

         // Закрыть ответный поток
         resp.Close();
      }

      // Доступ к заголовку с информацией
      static void AccessHeaderInformation()
      {
         // Создать объект запроса типа WebRequest по указанному URI
         HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://example.com");
         // Отправить сформированный запрос и получить на него ответ
         HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
         // Получить список имен
         string[] names = resp.Headers.AllKeys;
         // Отобразить пары "имя-значение" из заголовка
         Console.WriteLine("{0,-20}{1}", "Имя", "Значение");
         for (int i = 0; i < names.Length; i++)
         {
            string n = names[i];
            Console.Write("{0,-20}", n);
            for (int j = 0; j < resp.Headers.GetValues(n).Length; j++)
            {
               string v = resp.Headers.GetValues(n)[j];
               Console.WriteLine(v);
            }
         }

         // Закрыть ответный поток
         resp.Close();
      }

      // Пример применения свойств из класса Uri
      static void ApplyingPropertiesUriClass()
      {
         Uri sample = new Uri("https://example.com/somefile.txt?SomeQuery");
         Console.WriteLine("Хост: " + sample.Host);
         Console.WriteLine("Порт: " + sample.Port);
         Console.WriteLine("Протокол: " + sample.Scheme);
         Console.WriteLine("Локальный путь: " + sample.LocalPath);
         Console.WriteLine("Запрос: " + sample.Query);
         Console.WriteLine("Путь и запрос: " + sample.PathAndQuery);
      }

      // Пример обработки сетевых исключений
      static void HandlingNetworkException()
      {
         int ch = 0;
         try
         {
            // Сначала создать объект запроса типа WebRequest по указанному URI
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://example.com");
            // Затем отправить сформированный запрос и получить на него ответ
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            // Получить из ответа поток ввода
            Stream istrm = resp.GetResponseStream();
            // А теперь прочитать и отобразить гипертекстовое содержимое, полученное по указанному URI
            // Это содержимое выводится на экран отдельными порциями по 400 символов
            // После каждой такой порции следует нажать клавишу <ENTER>, чтобы вывести на экран следующую порцию из 400 символов
            for (int i = 1; ; i++)
            {
               if (istrm != null) ch = istrm.ReadByte();
               if (ch == -1)
               {
                  break;
               }

               Console.Write((char)ch);
               if (i % 400 == 0)
               {
                  Console.Write("\nНажмите клавишу <Enter>");
                  Console.ReadLine();
               }
            }

            // Закрыть ответный поток
            // При этом закрывается также поток ввода istrm
            resp.Close();
         }
         catch (WebException exc)
         {
            Console.WriteLine("Сетевая ошибка: " + exc.Message + "\nКод состояния: " + exc.Status);
         }
         catch (ProtocolViolationException exc)
         {
            Console.WriteLine("Протокольная ошибка: " + exc.Message);
         }
         catch (UriFormatException exc)
         {
            Console.WriteLine("Ошибка формата URI: " + exc.Message);
         }
         catch (NotSupportedException exc)
         {
            Console.WriteLine("Неизвестный протокол: " + exc.Message);
         }
         catch (IOException exc)
         {
            Console.WriteLine("Ошибка ввода-вывода: " + exc.Message);
         }
         catch (System.Security.SecurityException exc)
         {
            Console.WriteLine("Исключение в связи с нарушением безопасности: " + exc.Message);
         }
         catch (InvalidOperationException exc)
         {
            Console.WriteLine("Недопустимая операция: " + exc.Message);
         }
      }

      // Пример доступа к сайту на основе классов WebRequest и WebResponse
      static void ExampleAccessingSite()
      {
         int ch = 0;
         // Сначала создать объект запроса типа WebRequest по указанному URI
         WebRequest req = WebRequest.Create("https://example.com");
         // Затем отправить сформированный запрос и получить на него ответ
         WebResponse resp = req.GetResponse();
         // Получить из ответа поток ввода
         Stream istrm = resp.GetResponseStream();
         // А теперь прочитать и отобразить гипертекстовое содержимое, полученное по указанному URI
         // Это содержимое выводится на экран отдельными порциями по 400 символов
         // После каждой такой порции следует нажать клавишу <ENTER>, чтобы вывести на экран следующую порцию из 400 символов
         for (int i = 1; ; i++)
         {
            if (istrm != null)
            {
               ch = istrm.ReadByte();
            }

            if (ch == -1)
            {
               break;
            }

            Console.Write((char)ch);
            if ((i % 400) == 0)
            {
               Console.Write("\nНажмите клавишу <Enter>");
               Console.ReadLine();
            }
         }

         // Закрыть ответный поток
         // При этом закрывается также поток ввода istrm
         resp.Close();
      }

      // Пример доступа к сайту на основе классов WebRequest и WebResponse
      static void ExampleAccessingWebsite()
      {
         int ch = 0;
         // Сначала создается объект запроса типа WebRequest по указанному URI
         HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://example.com");
         // Затем отправить сформированный запрос и получить на него ответ
         HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
         // Получить из ответа поток ввода
         Stream istrm = resp.GetResponseStream();
         // А теперь прочитать и отобразить гипертекстовое содержимое, полученное по указанному URI
         // Это содержимое выводится на экран отдельными порциями по 400 символов
         // После каждой такой порции следует нажать клавишу <ENTER>, чтобы вывести на экран следующую порцию из 400 символов
         for (int i = 1; ; i++)
         {
            if (istrm != null)
            {
               ch = istrm.ReadByte();
            }

            if (ch == -1)
            {
               break;
            }

            Console.Write((char)ch);
            if ((i % 400) == 0)
            {
               Console.Write("\nНажмите клавишу <Enter>");
               Console.ReadLine();
            }
         }

         // Закрыть ответный поток
         // При этом закрывается также поток ввода istrm
         resp.Close();
      }

      // Применение класса WebClient
      static void UsingWebClientClass()
      {
         WebClient user = new WebClient();
         string uri = "https://example.com";
         string fname = "data.txt";
         try
         {
            Console.WriteLine("Загрузка данных по адресу " + uri + " в файл " + fname);
            user.DownloadFile(uri, fname);
         }
         catch (WebException exc)
         {
            Console.WriteLine(exc);
         }

         Console.WriteLine("Загрузка завершена");
      }
   }
}