using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

public static class Website1
{
     public static void run(string course_id)
    {
        string url = "https://www.proprofs.com/quiz-school/story.php?title=1dq-funny-trick-questions";
        var web = new HtmlWeb();
        var doc = web.Load(url);

        var questionNodes = doc.DocumentNode.SelectNodes("//h3[@class='question-text']");

        if (questionNodes != null)
        {
            foreach (var questionNode in questionNodes)
            {
                string question = DecodeHtmlEntities(questionNode.InnerText.Trim());

                // Select options list and extract options
                var optionsList = questionNode.ParentNode.SelectSingleNode(".//ul[@class='answers-list']");
                var optionNodes = optionsList.SelectNodes(".//li[@class='list_multichoice']");
                List<string> options = new List<string>();

                foreach (var optionNode in optionNodes)
                {
                    string optionText = DecodeHtmlEntities(optionNode.SelectSingleNode(".//div[@class='opt_text']/p").InnerText.Trim());
                    options.Add(optionText);
                }

                var correctAnswerNode = questionNode.ParentNode.SelectSingleNode(".//div[@class='correct_ans_list']");
                string correctAnswer = correctAnswerNode.SelectSingleNode(".//span[contains(text(),'Correct Answer')]/following-sibling::br").NextSibling.InnerText.Trim();

                string correctOption = Regex.Match(correctAnswer, @"[A-D]").Value;

                StoreQuestionInDatabase(question, options, correctOption, course_id);

                Console.WriteLine("Question: " + question);
                Console.WriteLine("Options:");
                foreach (var option in options)
                {
                    Console.WriteLine("- " + option);
                }
                Console.WriteLine("Correct Answer: " + correctOption);
                Console.WriteLine("----------------------------------");
            }
        }
        else
        {
            Console.WriteLine("No questions found on the page.");
        }
    }  

    static string DecodeHtmlEntities(string text)
    {
        text = text.Replace("&nbsp;", " ");
        text = Regex.Replace(text, "&#\\d+;", m => ((char)int.Parse(m.Value.Substring(2, m.Value.Length - 3))).ToString());
        return text;
    }

    static void StoreQuestionInDatabase(string question, List<string> options, string correctAnswer,string course_id)
    {
        string connectionString = "Data Source=DESKTOP-ABS14OI;Initial Catalog=databasequiz5;Integrated Security=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string insertQuery = "INSERT INTO question (course_id, question, option1, option2, option3, option4, answer) " +
                                 "VALUES (@course_id, @Question, @Option1, @Option2, @Option3, @Option4, @Answer)";

            SqlCommand command = new SqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@course_id", course_id);
            command.Parameters.AddWithValue("@Question", question);
            command.Parameters.AddWithValue("@Option1", options[0]);
            command.Parameters.AddWithValue("@Option2", options[1]);
            command.Parameters.AddWithValue("@Option3", options[2]);
            command.Parameters.AddWithValue("@Option4", options[3]);
            command.Parameters.AddWithValue("@Answer", correctAnswer);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            
        }
    }
}
