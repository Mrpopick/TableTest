using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Data.Common;
using System.ComponentModel.Design;
using ScottPlot;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnClick1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new();
            bool? success = fileDialog.ShowDialog();
            int len = 0;
            string connectionString = "server=localhost;Trusted_Connection=Yes;DataBase=Oil;";
            
            if (success == true)
            {
                string filePath = fileDialog.FileName;
                string fileName = Path.GetFileName(filePath);
                string  result = Regex.Match(fileName, ".*?\\.{0,1}\\.").Value.Replace(".", "");
                //MessageBox.Show(result);
                // Открываем файл для чтения
                StreamReader reader = new StreamReader(filePath);

                //ЗАПРОС НА СОЗДАНИЕ ТАБЛИЦЫ
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "CREATE TABLE"  + "["   + result + "]" + "(x float, y1 float, y2 float);";
                    SqlCommand command1 = new SqlCommand(query, connection);
                    try
                    {
                        connection.Open();
                        command1.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Table dont create");
                    }
                }
  

                while (reader.Peek() >= 0)
                {
                    string input = reader.ReadLine();
                    input = Regex.Replace(input, @"\s+", " ");
                    input = Regex.Replace(input, @"^\s{1}", "");
                    input = input.TrimEnd();
                    float[] numbers = input.Replace(".", ",").Split(' ').Select(float.Parse).ToArray();
                    len = numbers.Length;
                    if (len == 2)
                    {
                        numbers[0] = numbers[0] < 0 ? 0 : numbers[0];
                        numbers[1] = numbers[1] < 0 ? 0 : numbers[1];
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand("INSERT INTO " + result + "(x, y1,y2) VALUES (@column1, @column2,@column3)", connection))
                            {
                                command.Parameters.AddWithValue("@column1", numbers[0]);
                                command.Parameters.AddWithValue("@column2", numbers[1]);
                                command.Parameters.AddWithValue("@column3", 0);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    else if (len == 3)
                    {
                        numbers[0] = numbers[0] < 0 ? 0 : numbers[0];
                        numbers[1] = numbers[1] < 0 ? 0 : numbers[1];
                        numbers[2] = numbers[2] < 0 ? 0 : numbers[2];
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand("INSERT INTO " + result + "(x, y1,y2) VALUES (@column1, @column2, @coloumn3)", connection))
                            {
                                command.Parameters.AddWithValue("@column1", numbers[0]);
                                command.Parameters.AddWithValue("@column2", numbers[1]);
                                command.Parameters.AddWithValue("@coloumn3", numbers[2]);
                                command.ExecuteNonQuery();
                            }
                        }
                    }



                }

                if (len == 3)
                {
                    string stmt = "SELECT COUNT(*) FROM " + result;
                    string sqlQuery = "UPDATE " + result + " SET x = round(x, 3)";
                    string sqlQuery1 = "UPDATE " + result + " SET y1 = round(y1, 3)";
                    string sqlQuery2 = "UPDATE " + result + " SET y2 = round(y2, 3)";
                    int count = 0;
                    //СЧЕТ КОЛ. ЗНАЧЕНИЙ
                    using (SqlConnection Connection = new SqlConnection(connectionString))
                    {

                        using (SqlCommand cmdCount = new SqlCommand(stmt, Connection))
                        {
                            Connection.Open();

                            count = (int)cmdCount.ExecuteScalar();

                        }
                        using (SqlCommand command = new SqlCommand(sqlQuery, Connection))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command1 = new SqlCommand(sqlQuery1, Connection))
                        {
                            command1.ExecuteNonQuery();
                        }
                        using (SqlCommand command2 = new SqlCommand(sqlQuery2, Connection))
                        {
                            command2.ExecuteNonQuery();
                        }

                    }
                    //MessageBox.Show($"{count}");

                    string sqlE = "SELECT * FROM " + result;
                    double[] asixX = new double[count];
                    double[] asixY1 = new double[count];
                    double[] asixY2 = new double[count];
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlE, connection);

                        SqlDataReader re = command.ExecuteReader();

                        if (re.HasRows)
                        {
                            int i = 0;
                            while (re.Read()) // построчно считываем данные
                            {
                                double x = re.GetDouble(0);
                                double y1 = re.GetDouble(1);
                                double y2 = re.GetDouble(2);
                                asixX[i] = x;
                                asixY1[i] = y1;
                                asixY2[i] = y2;
                                i++;

                            }
                        }
                    }


                    WpfPlot1.Plot.AddScatter(asixX, asixY1, null, 1, 0, MarkerShape.none, LineStyle.Solid, null);
                    WpfPlot1.Refresh();
                    WpfPlot2.Plot.AddScatter(asixX, asixY2, null, 1, 0, MarkerShape.none, LineStyle.Solid, null);
                    WpfPlot2.Refresh();
                }
                else if (len == 2) 
                {
                    string stmt = "SELECT COUNT(*) FROM " + result;
                    string sqlQuery = "UPDATE " + result + " SET x = round(x, 3)";
                    string sqlQuery1 = "UPDATE " + result + " SET y1 = round(y1, 3)";
                    
                    int count = 0;
                    //СЧЕТ КОЛ. ЗНАЧЕНИЙ
                    using (SqlConnection Connection = new SqlConnection(connectionString))
                    {

                        using (SqlCommand cmdCount = new SqlCommand(stmt, Connection))
                        {
                            Connection.Open();

                            count = (int)cmdCount.ExecuteScalar();

                        }
                        using (SqlCommand command = new SqlCommand(sqlQuery, Connection))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command1 = new SqlCommand(sqlQuery1, Connection))
                        {
                            command1.ExecuteNonQuery();
                        }
                        

                    }
                    //MessageBox.Show($"{count}");

                    string sqlE = "SELECT * FROM " + result;
                    double[] asixX = new double[count];
                    double[] asixY1 = new double[count];
                    
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlE, connection);

                        SqlDataReader re = command.ExecuteReader();

                        if (re.HasRows)
                        {
                            int i = 0;
                            while (re.Read()) // построчно считываем данные
                            {
                                double x = re.GetDouble(0);
                                double y1 = re.GetDouble(1);
                                
                                asixX[i] = x;
                                asixY1[i] = y1;
                                
                                i++;

                            }
                        }
                    }

                    WpfPlot2.Visibility = Visibility.Hidden;
                    WpfPlot1.Plot.AddScatter(asixX, asixY1, null, 1, 0, MarkerShape.none, LineStyle.Solid, null);
                    WpfPlot1.Refresh();
                    
                }



            }
            else 
            {
                Close();
            }
            

        }

        

        
    }
}
