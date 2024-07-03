using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

public static class Website4
{
public static void run(string course_id)    
    {
        var url = "https://www.linkedin.com/pulse/30-general-knowledge-questions-answers-2023-edition"; // Replace with the actual URL

        try
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var quizContentBlocks = doc.DocumentNode.SelectNodes("//div[@data-test-id='article-content-blocks']/div[@class='article-main__content']");

            if (quizContentBlocks != null)
            {
                int i = 0;
                while (i < quizContentBlocks.Count)
                {
                    var questionNode = quizContentBlocks[i].SelectSingleNode(".//span[@class='font-[700]']");
                    if (questionNode != null)
                    {
                        var options = new string[4];
                        string correctAnswer = null;

                        i += 2;

                        bool validOptions = true;
                        for (int j = 0; j < 4; j++, i++)
                        {
                            if (i >= quizContentBlocks.Count)
                            {
                                validOptions = false;
                                break;
                            }
                            var optionNode = quizContentBlocks[i].SelectSingleNode(".//span");
                            if (optionNode != null && optionNode.InnerText.Trim().StartsWith((char)('a' + j) + ")"))
                            {
                                options[j] = optionNode.InnerText.Trim().Substring(2).Trim();
                            }
                            else
                            {
                                validOptions = false;
                                break;
                            }
                        }

                        // Skip the next div
                        i++;

                        if (i < quizContentBlocks.Count)
                        {
                            var answerNode = quizContentBlocks[i].SelectSingleNode(".//span[@class='font-[700] italic underline']");
                            if (answerNode != null)
                            {
                                correctAnswer = answerNode.InnerText.Trim().Replace("Correct answer:", "").Trim();
                                correctAnswer = correctAnswer[0].ToString().ToUpper(); // Extract only A, B, C, or D
                            }
                        }

                        if (validOptions && options.All(opt => opt != null) && correctAnswer != null)
                        {
                            Console.WriteLine($"Question: {questionNode.InnerText.Trim()}");
                            for (int j = 0; j < 4; j++)
                            {
                                Console.WriteLine($"Option {j + 1}: {options[j]}");
                            }
                            Console.WriteLine($"Answer: {correctAnswer}");
                            Console.WriteLine();
                            StoreQuestionInDatabase(questionNode.InnerText.Trim(), options.ToList(), correctAnswer,course_id);
                        }

                        i++;
                    }

                    i++;
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


            Console.WriteLine("Question stored in the database.");
        }
    }
}
