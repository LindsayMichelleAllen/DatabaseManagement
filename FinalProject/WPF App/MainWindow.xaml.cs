using System;
using System.IO;
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
using Npgsql;
using pagetest;


namespace WPF_App
{
    using pagetest;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> checkedItems = new List<string>();
        public class Business
        {
            public string id { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string city { get; set; }            
        }
        public class Review
        {
            public string date { get; set; }
            public string text { get; set; }
            public string stars { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            addColumns();
            addStates();
            addBusiness();
            revAddColumns();
            int i = 1;
            while (i < 6)
            {
                star_input.Items.Add(i.ToString());
                i++;
            }
        }

        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=password; Database=yelpdb";
        }

        public void addStates()
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT business_state FROM business ORDER BY business_state";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StateList.Items.Add(reader.GetString(0)); //will grab from index 0 each time
                        }
                    }
                }
                conn.Close();
            }
        }

        public void addColumns()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business Name";
            col1.Binding = new Binding("name");
            col1.Width = 255;
            businessGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "State";
            col2.Binding = new Binding("state");
            businessGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            businessGrid.Columns.Add(col3);
        }

        public void revAddColumns()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Date";
            col1.Width = 255;
            col1.Binding = new Binding("date");
            dataGrid1.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Stars";
            col2.Binding = new Binding("stars");
            dataGrid1.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Text";
            col3.Binding = new Binding("text");
            dataGrid1.Columns.Add(col3);
        }

        public void addBusiness()
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM business";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            businessGrid.Items.Add(new Business() { id = reader.GetString(0), name = reader.GetString(1), state = reader.GetString(2), city = reader.GetString(3) }); //will grab from index 0 each time
                        }
                    }
                }
                conn.Close();
            }
        }

        private void StateList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            checkedItems.Clear();
            //refresh query when new state selection is made
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                if(CityList1.SelectedItem != null)
                {
                    CityList1.Items.Clear();
                }
                if(ZipList.SelectedItem != null)
                {
                    ZipList.Items.Clear();
                }
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct city FROM business WHERE business_state=\'" + StateList.SelectedItem.ToString() + "\' ORDER BY city"; 
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CityList1.Items.Add(reader.GetString(0)); 
                        }
                    }
                }
                conn.Close();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ZipList.SelectedItem != null)
            {
                ZipList.Items.Clear();
            }
            if (CityList1.SelectedItem != null)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT DISTINCT zipcode FROM business WHERE city='" + CityList1.SelectedItem.ToString() + "' AND business_state='" + StateList.SelectedItem.ToString() + "' ORDER BY zipcode;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ZipList.Items.Add(reader.GetString(0)); //will grab from index 0 each time
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        public string build_query(List<string> items)
        {
            string temp_query = "SELECT * FROM business";

            for (int i = 0; i < items.Count; i++)
            {
                if(i == 0)
                {
                    temp_query = temp_query + " WHERE EXISTS (SELECT * FROM categories WHERE categories.business_id=business.business_id AND cat_name='" + items[i] + "')";
                }
                else
                {
                    temp_query = temp_query + " AND EXISTS (SELECT * FROM categories WHERE categories.business_id=business.business_id AND cat_name='" + items[i] + "')";
                }
            }
            return temp_query;
        }

        void cat_box_unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkedinput = (sender as CheckBox);
            var cat = checkedinput.Content;
            checkedItems.Remove(cat.ToString());
            businessGrid.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    if(checkedItems.Count > 0)
                    {
                        cmd.CommandText = "SELECT * FROM (" + build_query(checkedItems) + ") as foo WHERE foo.zipcode=" + ZipList.SelectedItem.ToString() + "; ";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT * FROM business WHERE city='" + CityList1.SelectedItem.ToString() + "' AND business_state='" + StateList.SelectedItem.ToString() + "' AND zipcode='" + ZipList.SelectedItem.ToString() + "' ORDER BY business_name;";
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            businessGrid.Items.Add(new Business() { id = reader.GetString(0), name = reader.GetString(1), state = StateList.SelectedItem.ToString(), city = CityList1.SelectedItem.ToString() }); //will grab from index 0 each time
                        }
                    }
                }
                conn.Close();
            }
        }

        void cat_box_checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkedinput = (sender as CheckBox);
            var cat = checkedinput.Content;
            checkedItems.Add(cat.ToString());
            businessGrid.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM (" + build_query(checkedItems) + ") as foo INNER JOIN business ON foo.business_id = business.business_id AND business.zipcode=" + ZipList.SelectedItem.ToString() + "; ";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            businessGrid.Items.Add(new Business() { id = reader.GetString(0), name = reader.GetString(1), state = StateList.SelectedItem.ToString(), city = CityList1.SelectedItem.ToString() }); //will grab from index 0 each time
                        }
                    }
                }
                conn.Close();
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();
            if(ZipList.SelectedItem != null)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * FROM business WHERE city='" + CityList1.SelectedItem.ToString() + "' AND business_state='" + StateList.SelectedItem.ToString() + "' AND zipcode='" + ZipList.SelectedItem.ToString() + "' ORDER BY business_name;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                businessGrid.Items.Add(new Business() { id = reader.GetString(0), name = reader.GetString(1), state = StateList.SelectedItem.ToString(), city = CityList1.SelectedItem.ToString() }); //will grab from index 0 each time
                            }
                        }
                        cmd.CommandText = "SELECT distinct on(cat_name) cat_name, categories.business_id FROM categories INNER JOIN (SELECT business_id FROM business WHERE city='"
                            + CityList1.SelectedItem.ToString() + "' AND business_state='" + StateList.SelectedItem.ToString() + "' AND zipcode='"
                            + ZipList.SelectedItem.ToString() + "' ORDER BY business_name) as foo ON categories.business_id = foo.business_id;";

                        category_StackPanel.Children.Clear();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CheckBox newCat = new CheckBox();
                                newCat.Content = reader.GetString(0);
                                category_StackPanel.Children.Add(newCat);
                                newCat.Checked += new RoutedEventHandler(cat_box_checked);
                                newCat.Unchecked += new RoutedEventHandler(cat_box_unchecked);
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        private void BusinessGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Business currBusiness = (Business)businessGrid.SelectedItem;
            CurrentUser.Businst = (Business)businessGrid.SelectedItem;
            dataGrid1.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT rev_date, rev_stars, rev_text FROM review INNER JOIN business ON business.business_id=review.business_id WHERE business_name='" + currBusiness.name.ToString() + "';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            dataGrid1.Items.Add(new Review() {date =  reader.GetString(0), stars = reader.GetString(1), text = reader.GetString(2)});
                        }
                    }
                }
                conn.Close();
            }
        }
        
        private void Submit_button_Click(object sender, RoutedEventArgs e)
        {
            Business currBusiness = (Business)businessGrid.SelectedItem;
            string business_id = currBusiness.id;
            var charList = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
            Random rand = new Random();
            char[] temprev_id = new char[22];
            int i = 0;
            while(i < 22)
            {
                temprev_id[i] = charList[rand.Next(charList.Length)];
                i++;
            }
            string rev_id = new string(temprev_id);
            string body = review_content1.Text;
            //review_content1.Clear();
            string stars = star_input.Text;
            DateTime now = DateTime.Now;
            string[] tokens = now.ToString().Split('/'); // mm/dd/yyyy
            string[] year = tokens[2].Split(' ');
            string dateinput = year[0] + "-";
            if(tokens[0].Length > 1)
            {
                dateinput = dateinput + tokens[0] + "-";
            }
            else
            {
                dateinput = dateinput + "0" + tokens[0] + "-";
            }
            if(tokens[1].Length > 1)
            {
                dateinput = dateinput + tokens[1];
            }
            else
            {
                dateinput = dateinput + "0" + tokens[1];
            }

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO review " +
                                  "(rev_id, user_id, business_id, rev_stars, rev_date, rev_text, useful_count, funny_count, cool_count)" +
                                  " VALUES ('" +
                                  rev_id.ToString() + "', " + "'om5ZiponkpRqUNa3pVPiRg'" + ", '" + business_id + "', " + stars.ToString() + ", '" + dateinput + "', '" + body + "', 0, 0, 0);";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }

        }

        private void Checkin_button_Click(object sender, RoutedEventArgs e)
        {
            Business currBusiness = (Business)businessGrid.SelectedItem;
            string business_id = currBusiness.id;
            var ch_time = DateTime.Now.ToString("HH:mm");
            var ch_day = DateTime.Now.DayOfWeek.ToString();

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO checkins " +
                                  "(business_id, ch_day, ch_time)" +
                                  " VALUES ('" +
                                  business_id + "', '" + ch_day + "', '" + ch_time + "');";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        private void UserViewButton_Click(object sender, RoutedEventArgs e)
        {
            UserPage newPage = new UserPage();
            newPage.Show();
            this.Close();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Business currBusiness = (Business)businessGrid.SelectedItem;
            string business_id = currBusiness.id;
            string user_id = CurrentUser.Instance.id;

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO favorites " + "(business_id, user_id)" + "VALUES ('" + business_id +
                        "', '" + user_id + "');";

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //open CheckinChart window
            CheckinChart newWindow = new CheckinChart();
            newWindow.Show();
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();
            string selection = sortlist.SelectedItem.ToString();
            string sortby = "business_name";
            if (selection == "System.Windows.Controls.ComboBoxItem: Name")
            {
                sortby = "business.business_name";
            }
            else if (selection == "System.Windows.Controls.ComboBoxItem: Stars")
            {
                sortby = "business.stars";
            }
            else if (selection == "System.Windows.Controls.ComboBoxItem: Review Count")
            {
                sortby = "business.review_count";
            }
            else if (selection == "System.Windows.Controls.ComboBoxItem: # of Check-Ins")
            {
                sortby = "business.numCheckins";
            }

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM (" + build_query(checkedItems) + ") as foo INNER JOIN business ON foo.business_id = business.business_id AND business.zipcode=" + ZipList.SelectedItem.ToString() + " ORDER BY "+ sortby +"; ";
                    //cmd.CommandText = "SELECT * FROM business WHERE city='" + CityList1.SelectedItem.ToString() + "' AND business_state='" + StateList.SelectedItem.ToString() + "' AND zipcode='" + ZipList.SelectedItem.ToString() + "' ORDER BY " + sortby;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            businessGrid.Items.Add(new Business() { id = reader.GetString(0), name = reader.GetString(1), state = StateList.SelectedItem.ToString(), city = CityList1.SelectedItem.ToString() }); //will grab from index 0 each time
                        }
                    }
                }
                conn.Close();
            }
        }
    }
}
