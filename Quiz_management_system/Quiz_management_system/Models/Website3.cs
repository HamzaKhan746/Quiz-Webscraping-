using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
public static class Website3
{
public static void run(string course_id)    
    {
        var url = "https://www.classpoint.io/blog/multiple-choice-quiz-questions"; // Replace with the actual URL

        try
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var mainQuestions = doc.DocumentNode.SelectNodes("//ol/li");
            if (mainQuestions != null)
            {
                foreach (var mainQuestion in mainQuestions)
                {
                    var options = mainQuestion.SelectNodes(".//ul/li");
                    if (options != null && options.Count > 0)
                    {
                        var questionText = mainQuestion.SelectSingleNode("text()")?.InnerText.Trim();
                        Console.WriteLine($"Question: {questionText}");

                        var optionsList = new List<string>();
                        string correctAnswer = null;

                        var optionLabels = new[] { "A", "B", "C", "D" };

                        for (int i = 0; i < options.Count; i++)
                        {
                            var option = options[i];
                            var optionText = option.InnerText.Trim();

                            optionText = optionText.Substring(2).Trim();

                            var isCorrect = option.SelectSingleNode("strong") != null;

                            optionsList.Add(optionText);

                            if (isCorrect)
                            {
                                correctAnswer = optionLabels[i]; // Map to A, B, C, or D
                            }
                        }

                        for (int i = 0; i < optionsList.Count; i++)
                        {
                            Console.WriteLine($"Option {optionLabels[i]}: {optionsList[i]}");
                        }

                        if (correctAnswer != null)
                        {
                            Console.WriteLine($"Answer: {correctAnswer}");
                        }
                        StoreQuestionInDatabase(questionText, optionsList, correctAnswer,course_id);

                        Console.WriteLine();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.ReadLine();
    }
    static void StoreQuestionInDatabase(string question, List<string> options, string correctAnswer, string course_id)
    {
        string connectionString = "Data Source=DESKTOP-ABS14OI;Initial Catalog=databasequiz5;Integrated Security=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string insertQuery = "INSERT INTO question (course_id, question, option1, option2, option3, option4, answer) " +
                "VALUES (@course_id,@Question, @Option1, @Option2, @Option3, @Option4, @Answer)";

            SqlCommand command = new SqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@course_id", 2);
            command.Parameters.AddWithValue("@Question", question);
            command.Parameters.AddWithValue("@Option1", options[0]);
            command.Parameters.AddWithValue("@Option2", options[1]);
            command.Parameters.AddWithValue("@Option3", options[2]);
            command.Parameters.AddWithValue("@Option4", options[3]);
            command.Parameters.AddWithValue("@Answer", correctAnswer);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            Console.WriteLine("Data stored in SQL table.");
        }
    }
}
