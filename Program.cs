using System;
using System.IO;
using System.Net;

// MiniCrawler - скелетный вариант поискового робота

namespace MiniCrawler
{
   internal class Program
   {
      static void Main()
      {
         string[] reference = { "https://www.mheducation.com" };
         string link;
         string line;
         string answer;
         // Содержит текущее положение в ответе
         int curloc;
         if (reference.Length != 1)
         {
            Console.WriteLine("MiniCrawler: {0}", reference[0]);
            return;
         }

         // Содержит текущий URI
         string uri = reference[0];
         HttpWebResponse response = null;
         try
         {
            do
            {
               Console.WriteLine("Переход по ссылке " + uri);
               // Создать объект запроса типа WebRequest по указанному URI
               HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
               // Запретить дальнейшее использование этого URI
               uri = null;
               // Отправить сформированный запрос и получить на него ответ
               response = (HttpWebResponse)request.GetResponse();
               // Получить поток ввода из принятого ответа
               Stream istrm = response.GetResponseStream();
               // Заключить поток ввода в оболочку класса StreamReader
               StreamReader rdr = new StreamReader(istrm);
               // Прочитать всю страницу
               line = rdr.ReadToEnd();
               curloc = 0;
               do
               {
                  // Найти следующий URI для перехода по ссылке
                  link = FindLink(line, ref curloc);
                  if (link != null)
                  {
                     Console.WriteLine("Найдена ссылка: " + link);
                     Console.Write("Перейти по ссылке, Искать дальше, Выйти? ");
                     answer = Console.ReadLine();
                     if (string.Equals(answer, "П", StringComparison.OrdinalIgnoreCase))
                     {
                        uri = string.Copy(link);
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
               if (response != null)
               {
                  response.Close();
               }
            } while (uri != null);
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
            if (response != null)
            {
               response.Close();
            }
         }

         Console.WriteLine("Завершение программы MiniCrawler");

         Console.ReadKey();
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
   }
}