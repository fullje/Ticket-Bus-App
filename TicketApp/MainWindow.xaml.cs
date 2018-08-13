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
            Ticket ticket = new Ticket();

            InitializeComponent();

            dataGrid1.ItemsSource = bus.readDB().DefaultView;
            checkGrid.ItemsSource = bus.readDB().DefaultView;
            //TODO : FUNCITON TO LOAD ALL OF THE THINGS
     


        }

        private void comboBox_1_DropDownOpened(object sender, EventArgs e)
        {
            Bus bus = new Bus();


            comboBox_1.ItemsSource = bus.readDB().DefaultView;
            comboBox_1.DisplayMemberPath = "description";

            comboSeat.Items.Clear();

        }

        private void comboSeat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboSeat.SelectedItem != null)
            {
                seatTxt.Text = comboSeat.SelectedItem.ToString();

                if (Convert.ToInt32(comboSeat.SelectedItem.ToString()) % 2 == 0)
                {
                    _window.IsChecked = true;
                    _hallway.IsChecked = false;
                } else
                {
                    _hallway.IsChecked = true;
                    _window.IsChecked = false;
                }
                

            }
        }

        private void comboBox_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bus bus = new Bus();
            Ticket ticket = new Ticket();
            int holder = 0;
            int seatNbr = 0;
            int busNbr = 0;
            string freeSeat = null;
            ComboBox cmb = (ComboBox)sender;


            DataRowView drv = cmb.SelectedItem as DataRowView;
            if(drv != null)
            {
                descriptionTxt.Text = drv["description"].ToString();
                busNumberTxt.Text = drv["busNumber"].ToString();

                holder = Convert.ToInt32(drv["maxSeats"].ToString()); //How many seats are set
                busNbr = Convert.ToInt32(drv["busNumber"].ToString());

                //MessageBox.Show("Bus number: " + busNbr.ToString());

                string[] seats = new string[holder];

                DataTable dt = ticket.seats(busNbr); // Here has to be busNumber from selected ComboBox

                for (int y = 0; y < dt.Rows.Count; y++)
                {
                    //MessageBox.Show(dt.Rows[y]["seatNumber"].ToString());
                    seatNbr = Convert.ToInt32(dt.Rows[y]["seatNumber"]); // Less in loop :v
                    
                    for (int i = 1; i < seats.Length+1; i++)
                    {
                        //MessageBox.Show((i) + "========" + seatNbr.ToString());
                        if (i == seatNbr)
                        {
                            seats[i-1] = seatNbr.ToString();
                        }
                    }
                }
                
                for (int z = 0; z < seats.Length; z++)
                {
                    //MessageBox.Show("Siedzenie numer: " + (z+1) + " jest " + seats[z]);

                    if(seats[z] == null)
                    {
                        //MessageBox.Show("Siedzenie numer: " + (z + 1) + " jest wolne");
                        freeSeat = (z + 1).ToString();

                        comboSeat.Items.Add(freeSeat);
                    }
                }
            }


            

            //string[] seats = new string[]; // Here has to be array of maxSeats from bus

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
                //MessageBox.Show(dt.Rows[i]["seatNumber"].ToString());
                //comboSeat.Items.Add(dt.Rows[i]["seatNumber"].ToString());

            //}


        }

        private void zatwierdzClick(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            bus.insertDB(Convert.ToInt32(busNumber.Text), Convert.ToInt32(maxSeats.Text), description.Text);
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


        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            Ticket ticket = new Ticket();
            //MessageBox.Show(_window.IsChecked.Value.ToString(), _hallway.IsChecked.Value.ToString());
            //(string name, int busNumber, int seatNumber, bool isFree, bool isWindow)
            //MessageBox.Show(nameTxt.Text.ToString() + " " + busNumberTxt.Text.ToString() + " " + seatTxt.Text.ToString() + " " + _window.IsChecked.Value + " " +  _hallway.IsChecked.Value);
            ticket.insertDB(nameTxt.Text.ToString(), Convert.ToInt32(busNumberTxt.Text.ToString()), Convert.ToInt32(seatTxt.Text.ToString()), _window.IsChecked.Value, _hallway.IsChecked.Value);
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


            public DataTable seats(int busNumber)
            {
                string Query = "SELECT * FROM ticket WHERE busNumber=" + busNumber + ";";
                
                MySqlCommand commandSelect = new MySqlCommand(Query, ticketDB);

                MySqlDataAdapter adp = new MySqlDataAdapter(commandSelect);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                ticketDB.Open();

                ticketDB.Close();

                return dt;
            }

            public void insertDB(string name, int busNumber, int seatNumber, bool isFree, bool isWindow)
            {
                string Query = "INSERT INTO ticket (busNumber, name, seatNumber, isFree, isWindow) VALUES (" + busNumber + ",'" + name + "'," + seatNumber + "," + isFree + "," + isWindow + ");";
                MySqlCommand commandInsert = new MySqlCommand(Query, ticketDB);
                MySqlDataReader reader;
                ticketDB.Open();

                reader = commandInsert.ExecuteReader();

                reader.Close();
                ticketDB.Close();
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
            public void insertDB(int busNumber, int maxSeats, string description)
            {
                //MessageBox.Show(id + ", " + busNumber + ", " + maxSeats + ", " + des);
                int freeSeats = maxSeats;
                string Query = "INSERT INTO bus (busNumber, maxSeats, freeSeats, description) VALUES (" + busNumber + "," + maxSeats + "," + freeSeats + ",'" + description + "');";
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