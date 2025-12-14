using System;
using System.IO;
using System.Net;


class MiniCrawler
{
   // Найти ссылку в строке содержимого.
   static string FindLink(string htmlstr, ref int startloc)
   {
      int i;
      int start, end;
      string uri = null;

      i = htmlstr.IndexOf("href=\"http", startloc,
          StringComparison.OrdinalIgnoreCase);
      if (i != -1)
      {
         start = htmlstr.IndexOf('"', i) + 1;
         end = htmlstr.IndexOf('"', start);
         uri = htmlstr.Substring(start, end - start);
         startloc = end;
      }
      return uri;
   }

   static void Main(string[] args)
   {
      string[] arg = { "http://McGraw-Hill.com" };
      string link;
      string str;
      string answer;

      int curloc; // содержит текущее положение в ответе
      if (arg.Length != 1)
      {
         Console.WriteLine("Применение: MiniCrawler <uri>");
         return;
      }

      string uristr = arg[0]; // содержит текущий URI

      HttpWebResponse resp = null;

      try
      {
         do
         {
            Console.WriteLine("Переход по ссылке " + uristr);

            // Создать объект запроса типа WebRequest по указанному URI.
            HttpWebRequest req = (HttpWebRequest)
            WebRequest.Create(uristr);
            uristr = null; // запретить дальнейшее использование этого URI

            // Отправить сформированный запрос и получить на него ответ.
            resp = (HttpWebResponse)req.GetResponse();

            // Получить поток ввода из принятого ответа.
            Stream istrm = resp.GetResponseStream();

            // Заключить поток ввода в оболочку класса StreamReader.
            StreamReader rdr = new StreamReader(istrm);

            // Прочитать всю страницу.
            str = rdr.ReadToEnd();

            curloc = 0;

            do
            {
               // Найти следующий URI для перехода по ссылке.
               link = FindLink(str, ref curloc);
               if (link != null)
               {
                  Console.WriteLine("Найдена ссылка: " + link);

                  Console.Write("Перейти по ссылке, Искать дальше, Выйти?");
                  answer = Console.ReadLine();

                  if (string.Equals(answer, "П",
                      StringComparison.OrdinalIgnoreCase))
                  {
                     uristr = string.Copy(link);
                     break;
                  }

                  if (string.Equals(answer, "B",
                         StringComparison.OrdinalIgnoreCase))
                  {
                     break;
                  }

                  if (string.Equals(answer, "И",
                         StringComparison.OrdinalIgnoreCase))
                  {
                     Console.WriteLine("Поиск следующей ссылки.");
                  }
                  else
                  {
                     Console.WriteLine("Больше ссылок не найдено.");
                     break;
                  }
               }
            } while (link.Length > 0);
            // Закрыть ответный поток.
            if (resp != null) resp.Close();
         } while (uristr != null);
      }
      catch (WebException exc)
      {
         Console.WriteLine("Сетевая ошибка: " + exc.Message +
                         "\nКод состояния: " + exc.Status);
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
         if (resp != null) resp.Close();
      }
      Console.WriteLine("Завершение программы MiniCrawler.");
   }
}