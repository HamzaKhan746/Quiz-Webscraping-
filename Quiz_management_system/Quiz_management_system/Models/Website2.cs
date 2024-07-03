using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

public static class Website2
{
        public static void run(string course_id)    
    {
        var url = "https://byjus.com/maths/maths-quiz-questions/"; // Replace with the actual URL
        int questionsCount = 0; // Counter for the number of questions processed

        try
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);


            var articleNode = doc.DocumentNode.SelectSingleNode("//article[@id='post-1308890']");
            if (articleNode != null)
            {
                var paragraphs = articleNode.SelectNodes("p");
                if (paragraphs != null)
                {
                    for (int i = 0; i < paragraphs.Count && questionsCount < 11; i++)
                    {
                        var paragraph = paragraphs[i];
                        if (paragraph.InnerHtml.StartsWith("<strong>Q"))
                        {
                            var questionText = paragraph.InnerText.Trim();
                            questionText = questionText.Replace("&#8211;", "–").Replace("&#215;", "×"); // Replace HTML entities

                            if (i + 5 < paragraphs.Count)
                            {
                                List<string> optionsList = new List<string>();
                                for (int j = 1; j <= 4; j++)
                                {
                                    var option = paragraphs[i + j].InnerText.Trim();
                                    optionsList.Add(option);
                                }

                                var answerParagraph = paragraphs[i + 5];
                                if (answerParagraph.InnerText.Contains(":"))
                                {
                                    var answerText = answerParagraph.InnerText.Split(':')[1].Trim();
                                    StoreQuestionInDatabase(questionText, optionsList, answerText,course_id);
                                }
                                else
                                {
                                }


                                i += 5;
                                questionsCount++;
                            }
                            else
                            {
                                break;
                            }
                        }
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

            Console.WriteLine("Data stored in SQL table.");
        }
    }
}
