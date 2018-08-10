using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

using MySql.Data.MySqlClient;
using System.Data;

namespace TicketApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
         
        }

        private void zatwierdzClick(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            bus.insertDB(Convert.ToInt32(idBox.Text), Convert.ToInt32(busNumber.Text), Convert.ToInt32(maxSeats.Text), desc.Text);
            
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            dataGrid1.ItemsSource = bus.readDB().DefaultView;
        }
    }

    public class Bus
    {
        public static string setConnection = "SERVER=localhost;DATABASE=ticketdb;User ID=root;Password=;SSLmode=none";
        MySqlConnection busDB = new MySqlConnection(setConnection);
        

        //Read whole 'bus' db
        public DataTable readDB()
        {
            string Query = "SELECT * FROM bus";
            MySqlCommand commandSelect = new MySqlCommand(Query, busDB);
            MySqlDataAdapter adp = new MySqlDataAdapter(commandSelect);
            DataTable dt = new DataTable();
            adp.Fill(dt);
                        
            busDB.Open();
            
            busDB.Close();

            return dt;
        }

        //Insert into 'bus' db
        public void insertDB(int id, int busNumber, int maxSeats, string des)
        {
            //MessageBox.Show(id + ", " + busNumber + ", " + maxSeats + ", " + des);
            string Query = "INSERT INTO bus (id, busNumber, maxSeats, des) VALUES (" + id + "," + busNumber + "," + maxSeats + ",'" + des + "');";
            MySqlCommand commandInsert = new MySqlCommand(Query, busDB);
            MySqlDataReader reader;
            busDB.Open();

            reader = commandInsert.ExecuteReader();

            reader.Close();
            busDB.Close();
        }
            

    }


}
