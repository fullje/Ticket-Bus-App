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
            Bus bus = new Bus();

            InitializeComponent();

            dataGrid1.ItemsSource = bus.readDB().DefaultView;
            checkGrid.ItemsSource = bus.readDB().DefaultView;

        }

        private void zatwierdzClick(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            bus.insertDB(Convert.ToInt32(idBox.Text), Convert.ToInt32(busNumber.Text), Convert.ToInt32(maxSeats.Text), description.Text);
            dataGrid1.ItemsSource = bus.readDB().DefaultView;

        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            dataGrid1.ItemsSource = bus.readDB().DefaultView;
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();

            //string rowID = dataGrid1.SelectedItem.ToString();
            DataRowView rowView = dataGrid1.SelectedItem as DataRowView;
            string busID = rowView.Row[1].ToString();

            bus.deleteDB(Convert.ToInt32(busID));
            dataGrid1.ItemsSource = bus.readDB().DefaultView;

            //MessageBox.Show(rowID.ToString());
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            checkGrid.ItemsSource = bus.readDB().DefaultView;
        }


        private void rowSelected(object sender, SelectionChangedEventArgs e)
        {
            Ticket takeData = new Ticket();
            string busNumber = null;
            DataGrid dg = (DataGrid)sender;
            DataRowView row_selected = dg.SelectedItem as DataRowView;

            if(row_selected != null)
            {
                busNumber = row_selected["busNumber"].ToString();
                displayGrid.ItemsSource = takeData.choosenDB(Convert.ToInt32(busNumber)).DefaultView;

                Bus busData = new Bus();
                DataGrid b_dg = (DataGrid)sender;
                DataRowView rowBus_selected = b_dg.SelectedItem as DataRowView;

                seatsNumberBox.Text = rowBus_selected["maxSeats"].ToString();
                freeSeatsBox.Text = rowBus_selected["freeSeats"].ToString();
                descriptionBox.Text = rowBus_selected["description"].ToString();
            }
   
            
        }

        public class Ticket
        {
            static string setConnection = "SERVER=localhost;DATABASE=ticketdb;User ID=root;Password=;SSLmode=none";
            MySqlConnection ticketDB = new MySqlConnection(setConnection);

            //Read whole 'ticket' db
            public DataTable readDB()
            {
                string Query = "SELECT * FROM ticket";
                MySqlCommand commandSelect = new MySqlCommand(Query, ticketDB);
                MySqlDataAdapter adp = new MySqlDataAdapter(commandSelect);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                ticketDB.Open();

                ticketDB.Close();

                return dt;
            }

            public DataTable choosenDB(int bN)
            {
                string Query = "SELECT * FROM ticket WHERE (busNumber=" + bN + ");";

                MySqlCommand commandSelect = new MySqlCommand(Query, ticketDB);
                MySqlDataAdapter adp = new MySqlDataAdapter(commandSelect);

                DataTable dt = new DataTable();
                adp.Fill(dt);

                ticketDB.Open();

                ticketDB.Close();

                return dt;
            }
        }

        public class Bus
        {
            static string setConnection = "SERVER=localhost;DATABASE=ticketdb;User ID=root;Password=;SSLmode=none";
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
            public void insertDB(int id, int busNumber, int maxSeats, string description)
            {
                //MessageBox.Show(id + ", " + busNumber + ", " + maxSeats + ", " + des);
                int freeSeats = maxSeats;
                string Query = "INSERT INTO bus (id, busNumber, maxSeats, freeSeats, description) VALUES (" + id + "," + busNumber + "," + maxSeats + "," + freeSeats + ",'" + description + "');";
                MySqlCommand commandInsert = new MySqlCommand(Query, busDB);
                MySqlDataReader reader;
                busDB.Open();

                reader = commandInsert.ExecuteReader();

                reader.Close();
                busDB.Close();
            }

            public void deleteDB(int busNumber)
            {
                string Query = "DELETE FROM bus WHERE busNumber=" + busNumber + ";";
                MySqlCommand commandDelete = new MySqlCommand(Query, busDB);

               
                busDB.Open();
                MySqlDataReader reader;
                reader = commandDelete.ExecuteReader();
                while (reader.Read())
                {

                }
                
                busDB.Close();

            }


        }

        
    }
}