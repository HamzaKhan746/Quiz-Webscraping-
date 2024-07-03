using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

public static class Website5
{
public static void run(string course_id)    {
        string url = "https://www.funtrivia.com/trivia-quiz/Sports/The-Asian-Games-318239.html";

        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);

        string containerXPath = "//div[@class='container']";

        HtmlNodeCollection containerNodes = doc.DocumentNode.SelectNodes(containerXPath);
        HtmlNodeCollection answerNodes = doc.DocumentNode.SelectNodes("//div[@class='extrainfo']//span/b");

        if (containerNodes != null && answerNodes != null)
        {
            int questionNumber = 1; // Track question number

            foreach (HtmlNode containerNode in containerNodes)
            {
                HtmlNode questionNode = containerNode.SelectSingleNode(".//div[@class='playquiz_qntxtbox']/b");
                if (questionNode != null)
                {
                    string question = questionNode.InnerText.Trim();
                    question = CleanQuestionText(question);

                    Console.WriteLine("Question " + questionNumber + ": " + question);

                    HtmlNodeCollection optionNodes = containerNode.SelectNodes(".//div[@class='playquiz_anslist']//label");
                    if (optionNodes != null)
                    {
                        List<string> options = new List<string>();
                        char optionLabel = 'A';

                        foreach (HtmlNode optionNode in optionNodes)
                        {
                            string option = optionNode.InnerText.Trim();
                            options.Add(option);
                            Console.WriteLine(optionLabel + ": " + option);
                            optionLabel++;
                        }

                        string correctAnswerText = answerNodes[questionNumber - 1].InnerText.Trim();
                        string correctAnswer = MapCorrectAnswer(correctAnswerText, options);
                        Console.WriteLine("Correct Answer: " + correctAnswer);
                        StoreQuestionInDatabase(question, options, correctAnswer,course_id);

                        questionNumber++;

                    }

                    Console.WriteLine();
                }

            }


        }
        else
        {
            Console.WriteLine("No container divs or answers found on the page.");
        }

        Console.ReadLine();
    }

    static string CleanQuestionText(string text)
    {
        return Regex.Replace(text, @"^\d+\.\s*", "");
    }

    static string MapCorrectAnswer(string correctAnswerText, List<string> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].Contains(correctAnswerText))
            {
                return ((char)('A' + i)).ToString();
            }
        }
        return string.Empty;
    }
    static void StoreQuestionInDatabase(string question, List<string> options, string correctAnswer,string course_id)
    {
        string connectionString = "Data Source=DESKTOP-ABS14OI;Initial Catalog=databasequiz5;Integrated Security=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string insertQuery = "INSERT INTO question (course_id, question, option1, option2, option3, option4, answer) " +
                "VALUES (@course_id,@Question, @Option1, @Option2, @Option3, @Option4, @Answer)";

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
