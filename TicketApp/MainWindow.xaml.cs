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
            loadAtStart();
        }

        private void loadAtStart()
        {
            Bus bus = new Bus();
            Ticket ticket = new Ticket();

            description.IsEnabled = false;
            maxSeats.IsEnabled = false;

            applyBtn.IsEnabled = false;
            zatwierdzBtn.IsEnabled = false;


            dataGrid1.ItemsSource = bus.readDB().DefaultView;
            checkGrid.ItemsSource = bus.readDB().DefaultView;
        }

        //===================================================
        // When dropDown = refresh view of comboBox content
        //===================================================
        private void comboBox_1_DropDownOpened(object sender, EventArgs e)
        {
            Bus bus = new Bus();

            comboBox_1.ItemsSource = bus.readDB().DefaultView;
            comboBox_1.DisplayMemberPath = "description";

            nameTxt.Text = "";
            seatTxt.Text = "";
            descriptionTxt.Text = "";

            applyBtn.IsEnabled = false;
            nameTxt.IsEnabled = false;

            comboSeat.Items.Clear();

        }

        //===================================================
        // Gets value from comboSeat (there are seats numbers)
        // And put them to the text box
        // And set the value of checkbox
        //===================================================
        private void comboSeat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboSeat.SelectedItem != null)
            {
                seatTxt.Text = comboSeat.SelectedItem.ToString();
                nameTxt.IsEnabled = true;

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

        //===================================================
        // When dropDown then fill the comboBox with the 
        // 'Description' values from DB 
        // Also check the free Seats 
        // 
        //===================================================
        private void comboBox_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Bus bus = new Bus();
            Ticket ticket = new Ticket();
            ComboBox cmb = (ComboBox)sender;
            DataRowView drv = cmb.SelectedItem as DataRowView;

            int holder = 0;
            int busNbr = 0;
            int seatNbr = 0;
            string freeSeat = null;


            if (drv != null)
            {
                descriptionTxt.Text = drv["description"].ToString(); // Set textbox with 'description'
                busNumberTxt.Text = drv["busNumber"].ToString(); // Set textbox with 'busNumber'

                holder = Convert.ToInt32(drv["maxSeats"].ToString()); // Set holder (int) with 'maxSeats'
                busNbr = Convert.ToInt32(drv["busNumber"].ToString()); // Set busNbr (int) with 'busNumber'

                // Creating array (only when user select from dropDown and only works for current bus
                string[] seats = new string[holder];

                DataTable dt = ticket.choosenDB(busNbr);

                for (int y = 0; y < dt.Rows.Count; y++)
                {
                    seatNbr = Convert.ToInt32(dt.Rows[y]["seatNumber"]);

                    for (int i = 1; i < seats.Length + 1; i++)
                    {
                        if (i == seatNbr)
                        {
                            seats[i - 1] = seatNbr.ToString();
                        }
                    }
                }

                for (int z = 0; z < seats.Length; z++)
                {
                    if (seats[z] == null)
                    {
                        freeSeat = (z + 1).ToString();
                        comboSeat.Items.Add(freeSeat);
                    }
                }
            }
        }

        //===================================================
        // Apply button - Sends values to tickettDB
        //===================================================
        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            Ticket ticket = new Ticket();
            ticket.insertDB(nameTxt.Text.ToString(), Convert.ToInt32(busNumberTxt.Text.ToString()), Convert.ToInt32(seatTxt.Text.ToString()), _window.IsChecked.Value, _hallway.IsChecked.Value);

            nameTxt.Text = "";
            seatTxt.Text = "";

            //++++++++++++++++++++
            //Bug found - Should be reloaded not cleared! But it works...
            //++++++++++++++++++++
            comboSeat.Items.Clear(); 

            applyBtn.IsEnabled = false;
        }

        //===================================================
        // Sends values from textbox to busDB
        // And reload view of dataGrid
        //===================================================
        private void zatwierdzClick(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            bus.insertDB(Convert.ToInt32(busNumber.Text), Convert.ToInt32(maxSeats.Text), description.Text);
            dataGrid1.ItemsSource = bus.readDB().DefaultView;

            busNumber.Text = "";
            maxSeats.Text = "";
            description.Text = "";

        }

        //===================================================
        // Manual reload function
        //===================================================
        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            dataGrid1.ItemsSource = bus.readDB().DefaultView;
        }

        //===================================================
        // Delete selected row from DB 
        // Then reload view of dataGrid
        //===================================================
        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            DataRowView rowView = dataGrid1.SelectedItem as DataRowView;
            
            if(rowView != null)
            {
                string busID = rowView.Row[1].ToString();
                bus.deleteDB(Convert.ToInt32(busID));
                dataGrid1.ItemsSource = bus.readDB().DefaultView;
            }
        }

        //===================================================
        // Manual reload for dataGrid
        //===================================================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = new Bus();
            checkGrid.ItemsSource = bus.readDB().DefaultView;
        }

        //===================================================
        // Display selected busNumber, how many seats are free
        // and description
        //===================================================
        private void rowSelected(object sender, SelectionChangedEventArgs e)
        {
            Ticket takeData = new Ticket();
            DataGrid dg = (DataGrid)sender;
            DataRowView row_selected = dg.SelectedItem as DataRowView;
            
            string busNumber = null;

            if (row_selected != null)
            {
                busNumber = row_selected["busNumber"].ToString();
                //++++++++++++++++++++
                //Bug found - when clicked on empty datagrid
                //++++++++++++++++++++
                displayGrid.ItemsSource = takeData.choosenDB(Convert.ToInt32(busNumber)).DefaultView; 

                Bus busData = new Bus();
                DataGrid b_dg = (DataGrid)sender;
                DataRowView rowBus_selected = b_dg.SelectedItem as DataRowView;

                seatsNumberBox.Text = rowBus_selected["maxSeats"].ToString();
                
                descriptionBox.Text = rowBus_selected["description"].ToString();
            }            
        }

        

        //===================================================
        // if name TextBox is empty
        //===================================================
        private void nameTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (nameTxt.Text == "")
            {
                applyBtn.IsEnabled = false;
                zatwierdzBtn.IsEnabled = false;
            }
            else
            {
                applyBtn.IsEnabled = true;
            }
        }

        private void busTextChanged(object sender, TextChangedEventArgs e)
        {
            if (busNumber.Text == "")
            {
                maxSeats.IsEnabled = false;
                zatwierdzBtn.IsEnabled = false;
            }
            else
            {
                maxSeats.IsEnabled = true;
            }
        }

        private void maxSeats_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (maxSeats.Text == "")
            {
                description.IsEnabled = false;
                zatwierdzBtn.IsEnabled = false;
            }
            else
            {
                description.IsEnabled = true;
            }
        }

        private void description_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (description.Text == "")
            {
                zatwierdzBtn.IsEnabled = false;
            }
            else
            {
                zatwierdzBtn.IsEnabled = true;
            }
        }
        //=================================================

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

            //Read only selected = busNumber
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