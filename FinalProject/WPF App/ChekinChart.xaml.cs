using Npgsql;
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
using System.Windows.Shapes;

namespace WPF_App
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CheckinChart : Window
    {
        public CheckinChart()
        {
            InitializeComponent();
            columnChart();
        }

        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=password; Database=yelpdb";
        }

        private void columnChart()
        {
            string business_id = CurrentUser.Businst.id;
            List<KeyValuePair<string, int>> checkinData = new List<KeyValuePair<string, int>>();
            //fakedata ex from video
            //checkinData.Add(new KeyValuePair<string, int>("Monday", 60));

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT ch_day, SUM(ch_count) FROM checkins WHERE business_id='"+ business_id + "' GROUP BY ch_day ORDER BY ch_day;";

                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            checkinData.Add(new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1)));
                        }
                    }

                    //cmd.ExecuteNonQuery();
                }
                conn.Close();
            }

            Checkins.DataContext = checkinData;
        }
    }
}
