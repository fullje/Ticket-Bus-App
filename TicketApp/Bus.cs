using System;
using MySql.Data.MySqlClient;
using System.Data;

public class Bus
{
    public int id;
    public int busNumber;
    public int maxSeats;
    public string desc;

    public string setConnection = "SERVER=localhost;DATABASE=ticketdb;User ID=root;Password=;SSLmode=none";
    MySqlConnection busDB = new MySqlConnection(setConnection);

    //Read whole 'bus' db
    public void readDB()
    {

    }

    //Insert into 'bus' db
    public void insertDB()
    {

    }


}