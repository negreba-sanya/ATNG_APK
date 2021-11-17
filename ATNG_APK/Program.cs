using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using iTextSharp.text.pdf.parser;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace ATNG_APK
{
    class Program
    {
        

        public static string url_ATNG;
        public static string save_path_ATNG;
        public static string name_ATNG;

        public static string url_APK;
        public static string save_path_APK;
        public static string name_APK;

        private static TelegramBotClient client_ATNG;
        private static TelegramBotClient client_APK;

        public static string mode = "none";

        static void Main(string[] args)
        {
            client_ATNG = new TelegramBotClient(token_ANTG);
            client_ATNG.StartReceiving();
            client_ATNG.OnMessage += MessageATNG;

            client_APK = new TelegramBotClient(token_APK);
            client_APK.StartReceiving();
            client_APK.OnMessage += MessageAPK;

            Console.ReadLine();
            client_APK.StopReceiving();
            client_ATNG.StopReceiving();
        }

        private async static void MessageAPK(object sender, MessageEventArgs e)
        {
            url_APK = "";
            var msg = e.Message;
            if (msg != null)
            {

                switch (msg.Text)
                {
                    case "Сегодня":
                        try
                        {
                            string answer = "";
                            string table = "";
                            HtmlWeb ws = new HtmlWeb();
                            ws.OverrideEncoding = Encoding.UTF8;
                            HtmlDocument docum = ws.Load("https://cross-apk.ru/index.php?option=com_content&view=article&id=3004&Itemid=1802");
                            ArrayList list = new ArrayList();
                            foreach (HtmlNode node in docum.DocumentNode.SelectNodes("//div[contains(@class, 'item-page')]//a[@href]"))
                            {
                                if (node.InnerText == "Расписание занятий на " + GetFirstDayOfWeek(DateTime.Now).ToString("dd.MM") + "-" + GetLastDayOfWeek(DateTime.Now).ToString("dd.MM.yyyy"))
                                {
                                    url_APK = "https://cross-apk.ru/" + node.GetAttributeValue("href", null);
                                }
                            }
                            save_path_APK = @"";
                            WebClient wc = new WebClient();
                            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                            save_path_APK = @"";
                            name_APK = DateTime.Now.ToString("yyyy_MM_dd") + ".pdf";
                            wc.DownloadFile(url_APK, save_path_APK + name_APK);
                            try
                            {
                                string text_lessons;
                                PDDocument doc = null;
                                try
                                {
                                    doc = PDDocument.load(save_path_APK + name_APK);
                                    PDFTextStripper stripper = new PDFTextStripper();
                                    text_lessons = stripper.getText(doc).Replace("\r", "");
                                }
                                finally
                                {
                                    if (doc != null)
                                    {
                                        doc.close();
                                    }
                                }


                                string day_one = "";
                                string day_two = "";


                                switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                                {
                                    case 1:
                                        day_one = "Понедельник";
                                        day_two = "Вторник";
                                        break;
                                    case 2:
                                        day_one = "Вторник";
                                        day_two = "Среда";
                                        break;
                                    case 3:
                                        day_one = "Среда";
                                        day_two = "Четверг";
                                        break;
                                    case 4:
                                        day_one = "Четверг";
                                        day_two = "Пятница";
                                        break;
                                    case 5:
                                        day_one = "Пятница";
                                        day_two = "Суббота";
                                        break;
                                    case 6:
                                        day_one = "Суббота";
                                        break;
                                }
                                if (day_two != "")
                                {
                                    Regex regex_5 = new Regex(@"" + day_one + "(.+\n)+" + day_two + "");
                                    MatchCollection matches_3 = regex_5.Matches(text_lessons);

                                    if (matches_3.Count > 0)
                                    {
                                        foreach (Match match in matches_3)
                                        {
                                            answer += match.Value;
                                        }

                                    }
                                    else
                                    {
                                        answer = "Совпадений не найдено";
                                    }
                                }
                                else
                                {
                                    Regex regex = new Regex(@"" + day_one + "(.+\n)+");
                                    MatchCollection matches_1 = regex.Matches(text_lessons);

                                    if (matches_1.Count > 0)
                                    {
                                        foreach (Match match in matches_1)
                                        {
                                            answer += match.Value;
                                        }

                                    }
                                }
                                Regex regex_1 = new Regex(@"1н(\n)а(\n)(.+\n)+1н(\n)б");
                                MatchCollection matches_2 = regex_1.Matches(answer);
                                if (matches_2.Count > 0)
                                {
                                    table += DateTime.Now.ToString("dd MMMM yyyy ") + "\n";
                                    foreach (Match match in matches_2)
                                    {
                                        table += match.Value.Replace("1н\nа", "").Replace("1н\nб", "");
                                    }

                                }
                                System.IO.File.Delete(name_APK);
                            }
                            catch
                            {

                            }
                            await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: table, replyMarkup: ButtonsAPK());
                        }
                        catch
                        {
                            await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: DateTime.Now.ToString("dd.MM.yyyy") + "\n" + "\n" + "На этот день нет расписания", replyMarkup: ButtonsAPK());
                        }
                        break;
                    case "Завтра":
                        DateTime data = DateTime.Today.AddDays(+1);
                        try
                        {

                            string answer = "";
                            string table = "";
                            HtmlWeb ws = new HtmlWeb();
                            ws.OverrideEncoding = Encoding.UTF8;
                            HtmlDocument docum = ws.Load("https://cross-apk.ru/index.php?option=com_content&view=article&id=3004&Itemid=1802");
                            ArrayList list = new ArrayList();
                            foreach (HtmlNode node in docum.DocumentNode.SelectNodes("//div[contains(@class, 'item-page')]//a[@href]"))
                            {
                                if (node.InnerText == "Расписание занятий на " + GetFirstDayOfWeek(data).ToString("dd.MM") + "-" + GetLastDayOfWeek(data).ToString("dd.MM.yyyy"))
                                {
                                    url_APK = "https://cross-apk.ru/" + node.GetAttributeValue("href", null);
                                }
                            }
                            save_path_APK = @"";
                            WebClient wc = new WebClient();
                            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                            save_path_APK = @"";
                            name_APK = data.ToString("yyyy_MM_dd") + ".pdf";
                            wc.DownloadFile(url_APK, save_path_APK + name_APK);
                            try
                            {
                                string text_lessons;
                                PDDocument doc = null;
                                try
                                {
                                    doc = PDDocument.load(save_path_APK + name_APK);
                                    PDFTextStripper stripper = new PDFTextStripper();
                                    text_lessons = stripper.getText(doc).Replace("\r", "");
                                }
                                finally
                                {
                                    if (doc != null)
                                    {
                                        doc.close();
                                    }
                                }


                                string day_one = "";
                                string day_two = "";


                                switch (Convert.ToInt32(data.DayOfWeek))
                                {
                                    case 1:
                                        day_one = "Понедельник";
                                        day_two = "Вторник";
                                        break;
                                    case 2:
                                        day_one = "Вторник";
                                        day_two = "Среда";
                                        break;
                                    case 3:
                                        day_one = "Среда";
                                        day_two = "Четверг";
                                        break;
                                    case 4:
                                        day_one = "Четверг";
                                        day_two = "Пятница";
                                        break;
                                    case 5:
                                        day_one = "Пятница";
                                        day_two = "Суббота";
                                        break;
                                    case 6:
                                        day_one = "Суббота";
                                        break;
                                }
                                if (day_two != "")
                                {
                                    Regex regex_5 = new Regex(@"" + day_one + "(.+\n)+" + day_two + "");
                                    MatchCollection matches_3 = regex_5.Matches(text_lessons);

                                    if (matches_3.Count > 0)
                                    {
                                        foreach (Match match in matches_3)
                                        {
                                            answer += match.Value;
                                        }

                                    }
                                    else
                                    {
                                        answer = "Совпадений не найдено";
                                    }
                                }
                                else
                                {
                                    Regex regex = new Regex(@"" + day_one + "(.+\n)+");
                                    MatchCollection matches_1 = regex.Matches(text_lessons);

                                    if (matches_1.Count > 0)
                                    {
                                        foreach (Match match in matches_1)
                                        {
                                            answer += match.Value;
                                        }

                                    }
                                }
                                Regex regex_1 = new Regex(@"1н(\n)а(\n)(.+\n)+1н(\n)б");
                                MatchCollection matches_2 = regex_1.Matches(answer);
                                if (matches_2.Count > 0)
                                {
                                    table += data.ToString("dd MMMM yyyy ") + "\n";
                                    foreach (Match match in matches_2)
                                    {
                                        table += match.Value.Replace("1н\nа", "").Replace("1н\nб", "");
                                    }

                                }
                                System.IO.File.Delete(name_APK);
                            }
                            catch
                            {

                            }
                            await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: table, replyMarkup: ButtonsAPK());
                        }
                        catch
                        {
                            await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: data.ToString("dd.MM.yyyy") + "\n" + "\n" + "На этот день нет расписания", replyMarkup: ButtonsAPK());
                        }

                        break;
                    case "Послезавтра":
                        DateTime data_3 = DateTime.Today.AddDays(+2);
                        try
                        {

                            string answer = "";
                            string table = "";
                            HtmlWeb ws = new HtmlWeb();
                            ws.OverrideEncoding = Encoding.UTF8;
                            HtmlDocument docum = ws.Load("https://cross-apk.ru/index.php?option=com_content&view=article&id=3004&Itemid=1802");
                            ArrayList list = new ArrayList();
                            foreach (HtmlNode node in docum.DocumentNode.SelectNodes("//div[contains(@class, 'item-page')]//a[@href]"))
                            {
                                if (node.InnerText == "Расписание занятий на " + GetFirstDayOfWeek(data_3).ToString("dd.MM") + "-" + GetLastDayOfWeek(data_3).ToString("dd.MM.yyyy"))
                                {
                                    url_APK = "https://cross-apk.ru/" + node.GetAttributeValue("href", null);
                                }
                            }
                            save_path_APK = @"";
                            WebClient wc = new WebClient();
                            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                            save_path_APK = @"";
                            name_APK = data_3.ToString("yyyy_MM_dd") + ".pdf";
                            wc.DownloadFile(url_APK, save_path_APK + name_APK);
                            try
                            {
                                string text_lessons;
                                PDDocument doc = null;
                                try
                                {
                                    doc = PDDocument.load(save_path_APK + name_APK);
                                    PDFTextStripper stripper = new PDFTextStripper();
                                    text_lessons = stripper.getText(doc).Replace("\r", "");
                                }
                                finally
                                {
                                    if (doc != null)
                                    {
                                        doc.close();
                                    }
                                }


                                string day_one = "";
                                string day_two = "";


                                switch (Convert.ToInt32(data_3.DayOfWeek))
                                {
                                    case 1:
                                        day_one = "Понедельник";
                                        day_two = "Вторник";
                                        break;
                                    case 2:
                                        day_one = "Вторник";
                                        day_two = "Среда";
                                        break;
                                    case 3:
                                        day_one = "Среда";
                                        day_two = "Четверг";
                                        break;
                                    case 4:
                                        day_one = "Четверг";
                                        day_two = "Пятница";
                                        break;
                                    case 5:
                                        day_one = "Пятница";
                                        day_two = "Суббота";
                                        break;
                                    case 6:
                                        day_one = "Суббота";
                                        break;
                                }
                                if (day_two != "")
                                {
                                    Regex regex_5 = new Regex(@"" + day_one + "(.+\n)+" + day_two + "");
                                    MatchCollection matches_3 = regex_5.Matches(text_lessons);

                                    if (matches_3.Count > 0)
                                    {
                                        foreach (Match match in matches_3)
                                        {
                                            answer += match.Value;
                                        }

                                    }
                                    else
                                    {
                                        answer = "Совпадений не найдено";
                                    }
                                }
                                else
                                {
                                    Regex regex = new Regex(@"" + day_one + "(.+\n)+");
                                    MatchCollection matches_1 = regex.Matches(text_lessons);

                                    if (matches_1.Count > 0)
                                    {
                                        foreach (Match match in matches_1)
                                        {
                                            answer += match.Value;
                                        }

                                    }
                                }
                                Regex regex_1 = new Regex(@"1н(\n)а(\n)(.+\n)+1н(\n)б");
                                MatchCollection matches_2 = regex_1.Matches(answer);
                                if (matches_2.Count > 0)
                                {
                                    table += data_3.ToString("dd MMMM yyyy ") + "\n";
                                    foreach (Match match in matches_2)
                                    {
                                        table += match.Value.Replace("1н\nа", "").Replace("1н\nб", "");
                                    }

                                }
                                System.IO.File.Delete(name_APK);
                            }
                            catch
                            {

                            }
                            await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: table, replyMarkup: ButtonsAPK());
                        }
                        catch
                        {
                            await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: data_3.ToString("dd.MM.yyyy") + "\n" + "\n" + "На этот день нет расписания", replyMarkup: ButtonsAPK());
                        }
                        break;
                    default:
                        try
                        {
                            DateTime data_1 = Convert.ToDateTime(msg.Text);
                            string answer = "";
                            string table = "";
                            try
                            {
                                HtmlWeb ws = new HtmlWeb();
                                ws.OverrideEncoding = Encoding.UTF8;
                                HtmlDocument docum = ws.Load("https://cross-apk.ru/index.php?option=com_content&view=article&id=3004&Itemid=1802");
                                ArrayList list = new ArrayList();
                                foreach (HtmlNode node in docum.DocumentNode.SelectNodes("//div[contains(@class, 'item-page')]//a[@href]"))
                                {
                                    if (node.InnerText == "Расписание занятий на " + GetFirstDayOfWeek(data_1).ToString("dd.MM") + "-" + GetLastDayOfWeek(data_1).ToString("dd.MM.yyyy"))
                                    {
                                        url_APK = "https://cross-apk.ru/" + node.GetAttributeValue("href", null);
                                    }
                                }
                                save_path_APK = @"";
                                WebClient wc = new WebClient();
                                Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                                save_path_APK = @"";
                                name_APK = data_1.ToString("yyyy_MM_dd") + ".pdf";
                                wc.DownloadFile(url_APK, save_path_APK + name_APK);
                                try
                                {
                                    string text_lessons;
                                    PDDocument doc = null;
                                    try
                                    {
                                        doc = PDDocument.load(save_path_APK + name_APK);
                                        PDFTextStripper stripper = new PDFTextStripper();
                                        text_lessons = stripper.getText(doc).Replace("\r", "");
                                    }
                                    finally
                                    {
                                        if (doc != null)
                                        {
                                            doc.close();
                                        }
                                    }


                                    string day_one = "";
                                    string day_two = "";


                                    switch (Convert.ToInt32(data_1.DayOfWeek))
                                    {
                                        case 1:
                                            day_one = "Понедельник";
                                            day_two = "Вторник";
                                            break;
                                        case 2:
                                            day_one = "Вторник";
                                            day_two = "Среда";
                                            break;
                                        case 3:
                                            day_one = "Среда";
                                            day_two = "Четверг";
                                            break;
                                        case 4:
                                            day_one = "Четверг";
                                            day_two = "Пятница";
                                            break;
                                        case 5:
                                            day_one = "Пятница";
                                            day_two = "Суббота";
                                            break;
                                        case 6:
                                            day_one = "Суббота";
                                            break;
                                    }
                                    if (day_two != "")
                                    {
                                        Regex regex_5 = new Regex(@"" + day_one + "(.+\n)+" + day_two + "");
                                        MatchCollection matches_3 = regex_5.Matches(text_lessons);

                                        if (matches_3.Count > 0)
                                        {
                                            foreach (Match match in matches_3)
                                            {
                                                answer += match.Value;
                                            }

                                        }
                                        else
                                        {
                                            answer = "Совпадений не найдено";
                                        }
                                    }
                                    else
                                    {
                                        Regex regex = new Regex(@"" + day_one + "(.+\n)+");
                                        MatchCollection matches_1 = regex.Matches(text_lessons);

                                        if (matches_1.Count > 0)
                                        {
                                            foreach (Match match in matches_1)
                                            {
                                                answer += match.Value;
                                            }

                                        }
                                    }
                                    Regex regex_1 = new Regex(@"1н(\n)а(\n)(.+\n)+1н(\n)б");
                                    MatchCollection matches_2 = regex_1.Matches(answer);
                                    if (matches_2.Count > 0)
                                    {
                                        table += data_1.ToString("dd MMMM yyyy ") + "\n";
                                        foreach (Match match in matches_2)
                                        {
                                            table += match.Value.Replace("1н\nа", "").Replace("1н\nб", "");
                                        }

                                    }
                                    System.IO.File.Delete(name_APK);
                                }
                                catch
                                {

                                }
                                await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: table, replyMarkup: ButtonsAPK());
                            }
                            catch
                            {
                                await client_APK.SendTextMessageAsync(chatId: msg.Chat.Id, text: data_1.ToString("dd.MM.yyyy") + "\n" + "\n" + "На этот день нет расписания", replyMarkup: ButtonsAPK());
                            }
                        }
                        catch
                        {
                            await client_APK.SendTextMessageAsync(
                                                chatId: msg.Chat.Id,
                                                text: "Введите дату в формате \"01.01.2000\"",
                                                replyMarkup: ButtonsAPK()
                                                );
                        }
                        break;
                }


            }
        }

        private async static void MessageATNG(object sender, MessageEventArgs e)
        {
            string answer;
            var msg = e.Message;

            if (msg != null)
            {
                if (mode == "none")
                    switch (msg.Text)
                    {
                        case "Расписание":
                            mode = "time";
                            await client_ATNG.SendTextMessageAsync(
                                        chatId: msg.Chat.Id,
                                        text: "Введите дату в формате \"01.01.2000\"",
                                        replyMarkup: ButtonsATNG_three()
                                        );
                            break;
                        case "Преподаватели":
                            mode = "teachers";
                            await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                        text: "Введите фамилию преподавателя в формате \"Иванов\"",
                                        replyMarkup: ButtonsATNG_two());
                            break;
                        default:
                            mode = "none";
                            await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                        text: "Выберите команду:",
                                        replyMarkup: ButtonsATNG_one());
                            break;
                    }
                else
                {
                    switch (mode)
                    {
                        case "time":
                            if (msg.Text == "Назад")
                            {
                                mode = "none";
                                await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                        text: "Выберите команду:",
                                        replyMarkup: ButtonsATNG_one());
                            }
                            else
                            {
                                switch (msg.Text)
                                {
                                    case "Сегодня":
                                        try
                                        { // формирование ссылки и скачивание файла с дальнейшим распознаванием и парсингом
                                            WebClient wc = new WebClient();
                                            url_ATNG = "https://www.achtng.ru/atng/rasp/" + DateTime.Now.ToString("yyyy_MM") + "/" + DateTime.Now.ToString("yyyy_MM_dd") + ".pdf";
                                            save_path_ATNG = @"";
                                            name_ATNG = DateTime.Now.ToString("yyyy_MM_dd") + ".pdf";
                                            wc.DownloadFile(url_ATNG, save_path_ATNG + name_ATNG);
                                            answer = DateTime.Now.ToString("dd MMMM yyyy ") + "\n" + "\n";
                                            try
                                            {
                                                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(save_path_ATNG + name_ATNG);
                                                StringBuilder stringBuil = new StringBuilder();
                                                for (int i = 1; i <= reader.NumberOfPages; i++)
                                                {
                                                    stringBuil.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                                                }
                                                string text_lessons = stringBuil.ToString();

                                                Regex regex_1 = new Regex(@"ИСП-19/9(.+\n)+№ ИСП-20/11 Ауд.");
                                                MatchCollection matches = regex_1.Matches(text_lessons);
                                                string day = "";
                                                switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                                                {
                                                    case 1:
                                                        day = "понедельник";
                                                        break;
                                                    case 2:
                                                        day = "вторник";
                                                        break;
                                                    case 3:
                                                        day = "среда";
                                                        break;
                                                    case 4:
                                                        day = "четверг";
                                                        break;
                                                    case 5:
                                                        day = "пятница";
                                                        break;
                                                    case 6:
                                                        day = "суббота";
                                                        break;
                                                }


                                                if (matches.Count > 0)
                                                {
                                                    foreach (Match match in matches)
                                                    {
                                                        answer += match.Value.Replace("\nРасписание создано в 1С:Колледж с помощью обработки 'Мастер создания расписания'(Автор: Денис Буторин http://butorin.org)Расписание на " + DateTime.Now.ToString("dd MMMM yyyy") + " (" + day + ")   - продолжение стр.2", "");
                                                    }

                                                }
                                                else
                                                {
                                                    answer = "Совпадений не найдено";
                                                }

                                                Regex regex_2 = new Regex(@"№ ИСП-20/11 Ауд.");
                                                answer = regex_2.Replace(answer, "");
                                                reader.Close();
                                                System.IO.File.Delete(name_ATNG);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        catch
                                        { // вызывается при отсутсвии расписания на сайте по сформированной ссылке и вносит текст в переменную answer
                                            answer = DateTime.Now.ToString("dd MMMM yyyy ") + "\n" + "\n" + "На этот день нет расписания";
                                        }
                                        // вывод переменной в чат с пользователем 
                                        await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                            text: answer,
                                            replyMarkup: ButtonsATNG_three()
                                            );
                                        break;
                                    case "Завтра":
                                        DateTime date_2 = DateTime.Today.AddDays(+1);
                                        try
                                        { // формирование ссылки и скачивание файла с дальнейшим распознаванием и парсингом

                                            WebClient wc = new WebClient();
                                            url_ATNG = "https://www.achtng.ru/atng/rasp/" + date_2.ToString("yyyy_MM") + "/" + date_2.ToString("yyyy_MM_dd") + ".pdf";
                                            save_path_ATNG = @"";
                                            name_ATNG = date_2.ToString("yyyy_MM_dd") + ".pdf";
                                            wc.DownloadFile(url_ATNG, save_path_ATNG + name_ATNG);
                                            answer = date_2.ToString("dd MMMM yyyy ") + "\n" + "\n";
                                            try
                                            {
                                                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(save_path_ATNG + name_ATNG);
                                                StringBuilder stringBuil = new StringBuilder();
                                                for (int i = 1; i <= reader.NumberOfPages; i++)
                                                {
                                                    stringBuil.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                                                }
                                                string text_lessons = stringBuil.ToString();

                                                Regex regex_1 = new Regex(@"ИСП-19/9(.+\n)+№ ИСП-20/11 Ауд.");
                                                MatchCollection matches = regex_1.Matches(text_lessons);

                                                string day = "";
                                                switch (Convert.ToInt32(date_2.DayOfWeek))
                                                {
                                                    case 1:
                                                        day = "понедельник";
                                                        break;
                                                    case 2:
                                                        day = "вторник";
                                                        break;
                                                    case 3:
                                                        day = "среда";
                                                        break;
                                                    case 4:
                                                        day = "четверг";
                                                        break;
                                                    case 5:
                                                        day = "пятница";
                                                        break;
                                                    case 6:
                                                        day = "суббота";
                                                        break;
                                                }

                                                if (matches.Count > 0)
                                                {
                                                    foreach (Match match in matches)
                                                    {
                                                        answer += match.Value.Replace("\nРасписание создано в 1С:Колледж с помощью обработки 'Мастер создания расписания'(Автор: Денис Буторин http://butorin.org)Расписание на " + date_2.ToString("dd MMMM yyyy") + " (" + day + ")   - продолжение стр.2", "");
                                                    }

                                                }
                                                else
                                                {
                                                    answer = "Совпадений не найдено";
                                                }

                                                Regex regex_2 = new Regex(@"№ ИСП-20/11 Ауд.");
                                                answer = regex_2.Replace(answer, "");
                                                reader.Close();
                                                System.IO.File.Delete(name_ATNG);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        catch
                                        { // вызывается при отсутсвии расписания на сайте по сформированной ссылке и вносит текст в переменную answer
                                            answer = date_2.ToString("dd MMMM yyyy ") + "\n" + "\n" + "На этот день нет расписания";
                                        }
                                        // вывод переменной в чат с пользователем 
                                        await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                            text: answer,
                                            replyMarkup: ButtonsATNG_three()
                                            );
                                        break;
                                    case "Послезавтра":
                                        DateTime date_3 = DateTime.Today.AddDays(+2);
                                        try
                                        { // формирование ссылки и скачивание файла с дальнейшим распознаванием и парсингом

                                            WebClient wc = new WebClient();
                                            url_ATNG = "https://www.achtng.ru/atng/rasp/" + date_3.ToString("yyyy_MM") + "/" + date_3.ToString("yyyy_MM_dd") + ".pdf";
                                            save_path_ATNG = @"";
                                            name_ATNG = date_3.ToString("yyyy_MM_dd") + ".pdf";
                                            wc.DownloadFile(url_ATNG, save_path_ATNG + name_ATNG);
                                            answer = date_3.ToString("dd MMMM yyyy ") + "\n" + "\n";
                                            try
                                            {
                                                string day = "";
                                                switch (Convert.ToInt32(date_3.DayOfWeek))
                                                {
                                                    case 1:
                                                        day = "понедельник";
                                                        break;
                                                    case 2:
                                                        day = "вторник";
                                                        break;
                                                    case 3:
                                                        day = "среда";
                                                        break;
                                                    case 4:
                                                        day = "четверг";
                                                        break;
                                                    case 5:
                                                        day = "пятница";
                                                        break;
                                                    case 6:
                                                        day = "суббота";
                                                        break;
                                                }
                                                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(save_path_ATNG + name_ATNG);
                                                StringBuilder stringBuil = new StringBuilder();
                                                for (int i = 1; i <= reader.NumberOfPages; i++)
                                                {
                                                    stringBuil.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                                                }
                                                string text_lessons = stringBuil.ToString();

                                                Regex regex_1 = new Regex(@"ИСП-19/9(.+\n)+№ ИСП-20/11 Ауд.");
                                                MatchCollection matches = regex_1.Matches(text_lessons);

                                                if (matches.Count > 0)
                                                {
                                                    foreach (Match match in matches)
                                                    {
                                                        answer += match.Value.Replace("\nРасписание создано в 1С:Колледж с помощью обработки 'Мастер создания расписания'(Автор: Денис Буторин http://butorin.org)Расписание на " + date_3.ToString("dd MMMM yyyy") + " (" + day + ")   - продолжение стр.2", "");
                                                    }

                                                }
                                                else
                                                {
                                                    answer = "Совпадений не найдено";
                                                }

                                                Regex regex_2 = new Regex(@"№ ИСП-20/11 Ауд.");
                                                answer = regex_2.Replace(answer, "");
                                                reader.Close();
                                                System.IO.File.Delete(name_ATNG);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        catch
                                        { // вызывается при отсутсвии расписания на сайте по сформированной ссылке и вносит текст в переменную answer
                                            answer = date_3.ToString("dd MMMM yyyy ") + "\n" + "\n" + "На этот день нет расписания";
                                        }
                                        // вывод переменной в чат с пользователем 
                                        await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                            text: answer,
                                            replyMarkup: ButtonsATNG_three()
                                            );
                                        break;
                                    default:
                                        try
                                        {// попытка преобразовать текст сообщения в дату
                                            DateTime date_4 = Convert.ToDateTime(msg.Text);
                                            try
                                            { // формирование ссылки и скачивание файла с дальнейшим распознаванием и парсингом
                                                WebClient wc = new WebClient();
                                                url_ATNG = "https://www.achtng.ru/atng/rasp/" + date_4.ToString("yyyy_MM") + "/" + date_4.ToString("yyyy_MM_dd") + ".pdf";
                                                save_path_ATNG = @"";
                                                name_ATNG = date_4.ToString("yyyy_MM_dd") + ".pdf";
                                                wc.DownloadFile(url_ATNG, save_path_ATNG + name_ATNG);
                                                answer = date_4.ToString("dd MMMM yyyy ") + "\n" + "\n";
                                                try
                                                {
                                                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(save_path_ATNG + name_ATNG);
                                                    StringBuilder stringBuil = new StringBuilder();
                                                    for (int i = 1; i <= reader.NumberOfPages; i++)
                                                    {
                                                        stringBuil.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                                                    }
                                                    string text_lessons = stringBuil.ToString();

                                                    Regex regex_1 = new Regex(@"ИСП-19/9(.+\n)+№ ИСП-20/11 Ауд.");
                                                    MatchCollection matches = regex_1.Matches(text_lessons);

                                                    string day = "";
                                                    switch (Convert.ToInt32(date_4.DayOfWeek))
                                                    {
                                                        case 1:
                                                            day = "понедельник";
                                                            break;
                                                        case 2:
                                                            day = "вторник";
                                                            break;
                                                        case 3:
                                                            day = "среда";
                                                            break;
                                                        case 4:
                                                            day = "четверг";
                                                            break;
                                                        case 5:
                                                            day = "пятница";
                                                            break;
                                                        case 6:
                                                            day = "суббота";
                                                            break;
                                                    }

                                                    if (matches.Count > 0)
                                                    {
                                                        foreach (Match match in matches)
                                                        {
                                                            answer += match.Value.Replace("\nРасписание создано в 1С:Колледж с помощью обработки 'Мастер создания расписания'(Автор: Денис Буторин http://butorin.org)Расписание на " + date_4.ToString("dd MMMM yyyy") + " (" + day + ")   - продолжение стр.2", "");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        answer = "Совпадений не найдено";
                                                    }

                                                    Regex regex_2 = new Regex(@"№ ИСП-20/11 Ауд.");
                                                    answer = regex_2.Replace(answer, "");
                                                    reader.Close();
                                                    System.IO.File.Delete(name_ATNG);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            catch
                                            { // вызывается при отсутсвии расписания на сайте по сформированной ссылке и вносит текст в переменную answer
                                                answer = date_4.ToString("dd MMMM yyyy ") + "\n" + "\n" + "На этот день нет расписания";
                                            }
                                            // вывод переменной в чат с пользователем 
                                            await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                                text: answer,
                                                replyMarkup: ButtonsATNG_three()
                                                );

                                        }
                                        catch
                                        {
                                            await client_ATNG.SendTextMessageAsync(
                                                chatId: msg.Chat.Id,
                                                text: "Введите дату в формате \"01.01.2000\"",
                                                replyMarkup: ButtonsATNG_two()
                                                );
                                        }
                                        break;
                                }

                            }
                            break;
                        case "teachers":
                            if (msg.Text == "Назад")
                            {
                                mode = "none";
                                await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                        text: "Выберите команду:",
                                        replyMarkup: ButtonsATNG_one());
                            }
                            else
                            {
                                try
                                {// попытка сравнить текст сообщения с фамилией преподавателя из списка
                                    await client_ATNG.SendTextMessageAsync(
                                        chatId: msg.Chat.Id,
                                        text: Teachers(msg.Text),
                                        replyMarkup: ButtonsATNG_two());
                                }
                                catch
                                {// при отрицательном результате предыдущих условий
                                    await client_ATNG.SendTextMessageAsync(chatId: msg.Chat.Id,
                                        text: "Введите фамилию преподавателя в формате \"Иванов\"",
                                        replyMarkup: ButtonsATNG_two());
                                }
                            }
                            break;
                    }
                }
            }
        }

        private static IReplyMarkup ButtonsATNG_one()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Расписание" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Преподаватели" } }
                }
            };
        }
        private static IReplyMarkup ButtonsATNG_two()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Назад" } }
                }
            };
        }
        private static IReplyMarkup ButtonsATNG_three()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Сегодня" }, new KeyboardButton { Text = "Завтра" }, new KeyboardButton { Text = "Послезавтра" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Назад" } }
                }
            };
        }

        private static IReplyMarkup ButtonsAPK()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Сегодня" }, new KeyboardButton { Text = "Завтра" }, new KeyboardButton { Text = "Послезавтра" } }
                }
            };
        }

        public static string Teachers(string msg)
        {
            string answ = "";
            List<string[]> hrefTags = new List<string[]>();
            var client_APK = new WebClient();
            string html = client_APK.DownloadString("https://www.achtng.ru/staff");
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            foreach (IElement element in document.GetElementsByClassName("staffer-staff-title"))
            {
                string[] a = Convert.ToString(element.TextContent).Replace("\t", "").Replace("\n", "").Split(' ');
                hrefTags.Add(a);
            }
            foreach (string[] i in hrefTags)
            {
                if (i[0] == msg)
                {
                    answ = i[0] + " " + i[1] + " " + i[2];
                }
            }
            return answ;
        }

        static DateTime GetFirstDayOfWeek(DateTime date)
        {
            DateTime time = date;
            switch (Convert.ToInt32(date.DayOfWeek))
            {
                case 1:
                    time = date;
                    break;
                case 2:
                    time = date.AddDays(-1);
                    break;
                case 3:
                    time = date.AddDays(-2);
                    break;
                case 4:
                    time = date.AddDays(-3);
                    break;
                case 5:
                    time = date.AddDays(-4);
                    break;
                case 6:
                    time = date.AddDays(-5);
                    break;
            }
            return time;
        }
        static DateTime GetLastDayOfWeek(DateTime date)
        {
            DateTime time = date;
            switch (Convert.ToInt32(date.DayOfWeek))
            {
                case 1:
                    time = date.AddDays(+5);
                    break;
                case 2:
                    time = date.AddDays(+4);
                    break;
                case 3:
                    time = date.AddDays(+3);
                    break;
                case 4:
                    time = date.AddDays(+2);
                    break;
                case 5:
                    time = date.AddDays(+1);
                    break;
                case 6:
                    time = date;
                    break;
            }
            return time;
        }
    }
}
