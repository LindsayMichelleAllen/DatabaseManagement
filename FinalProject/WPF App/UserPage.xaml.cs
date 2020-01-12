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
using WPF_App;
using Npgsql;

namespace pagetest
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>

    public partial class UserPage : Window
    {
        List<string> friendsList = new List<string>();

        public class Favorite
        {
            public string name { get; set; }
            public string id { get; set; }
        }
        public class Friend
        {
            public string id { get; set; }
            public string name { get; set; }
            public string yelping_since { get; set; }
        }
        public class User
        {
            public string id { get; set; }
        }
        //current user is a singleton s.t. mainwindow also knows what user is selected
        public class currentUser
        {
            private static User instance = null;

            public currentUser()
            {
            }

            public static User Instance
            {
                get
                {
                    return instance;
                }
                set
                {
                    instance = value;
                }
            }
        }
        public class Review
        {
            public string date { get; set; }
            public string text { get; set; }
            public string stars { get; set; }
            public string name { get; set; }
        }
        public UserPage()
        {
            InitializeComponent();
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Friend ID";
            col1.Width = 200;
            col1.Binding = new Binding("id");
            userSearchOutput_datagrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Name";
            col2.Binding = new Binding("name");
            col2.Width = 100;
            friendsList_datagrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Yelping Since";
            col3.Binding = new Binding("yelping_since");
            col3.Width = 100;
            friendsList_datagrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Business Name";
            col4.Binding = new Binding("name");
            businessFavorites_datagrid.Columns.Add(col4);

            revAddColumns();

        }

        public void revAddColumns()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Date";
            col1.Width = 90;
            col1.Binding = new Binding("date");
            friendsReviews_datagrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Stars";
            col2.Binding = new Binding("stars");
            friendsReviews_datagrid.Columns.Add(col2);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Name";
            col4.Binding = new Binding("name");
            friendsReviews_datagrid.Columns.Add(col4);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Text";
            col3.Binding = new Binding("text");
            friendsReviews_datagrid.Columns.Add(col3);
        }

        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=password; Database=yelpdb";
        }

        private void BusinessPageButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }

        private void UserSearchInput_textbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            clearall();
            userSearchOutput_datagrid.Items.Clear();
            string userName = userSearchInput_textbox.Text;
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT user_id FROM customer WHERE cust_name = '" + userName + "';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userSearchOutput_datagrid.Items.Add(new User { id = reader.GetString(0) });
                        }
                    }
                }
                conn.Close();
            }
        }

        private void fillFriendsGrid(List<string> fList)
        {
            friendsReviews_datagrid.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                for (int i = 0; i < fList.Count; i++)
                {
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT rev_date, rev_stars, rev_text, cust_name FROM (SELECT rev_date, rev_stars, rev_text FROM review " +
                            "WHERE user_id= '" + fList[i] + "' " +
                            "AND rev_date=(SELECT MAX(rev_date) FROM review WHERE user_id='" + fList[i] + "')) as foo INNER JOIN customer ON customer.user_id='" + fList[i] + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                friendsReviews_datagrid.Items.Add(new Review { date = reader.GetString(0), stars = reader.GetString(1), text = reader.GetString(2), name = reader.GetString(3) });
                            }
                        }
                    }
                }
                conn.Close();
            }
        }

        private void clearall()
        {
            userSearchOutput_datagrid.Items.Clear();
            clearProfile();
            friendsList_datagrid.Items.Clear();
            friendsReviews_datagrid.Items.Clear();
            businessFavorites_datagrid.Items.Clear();
        }

        private void clearProfile()
        {
            name_textbox.Text = null;
            fans_textbox.Text = null;
            yelpingSince_textbox.Text = null;
            funny_textbox.Text = null;
            cool_textbox.Text = null;
            useful_textbox.Text = null;
            latitude_textbox.Text = null;
            longitude_textbox.Text = null;
        }

        private void UserSearchOutput_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(userSearchOutput_datagrid.SelectedItem == null) { return; }
            clearProfile();
            User selectedUser = (User)userSearchOutput_datagrid.SelectedItem;
            //currentUser current = new currentUser();
            //Application.Current.Properties("currentUserID") = userSearchOutput_datagrid.SelectedItem;
            CurrentUser.Instance = (User)userSearchOutput_datagrid.SelectedItem;
            string selectedUserID = selectedUser.id;

            // fill out user information
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT cust_name, fans, yelping_since, funny, cool, useful, " +
                                        "user_lat, user_longi FROM customer WHERE user_id='" + selectedUserID + "';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name_textbox.Text = reader.GetString(0);
                            fans_textbox.Text = reader.GetString(1);
                            yelpingSince_textbox.Text = reader.GetString(2);
                            funny_textbox.Text = reader.GetString(3);
                            cool_textbox.Text = reader.GetString(4);
                            useful_textbox.Text = reader.GetString(5);
                            if (!reader.IsDBNull(6))
                            {
                                latitude_textbox.Text = reader.GetDouble(6).ToString();
                            }
                            if (!reader.IsDBNull(7))
                            {
                                longitude_textbox.Text = reader.GetDouble(7).ToString();
                            }
                        }
                    }
                }
                using (var cmd = new NpgsqlCommand())
                {
                    friendsList_datagrid.Items.Clear();
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT user_id, cust_name, yelping_since FROM " + 
                                        "(SELECT friend_id FROM " + 
                                        "(SELECT * FROM customer WHERE user_id = '" + selectedUserID + "') as foo" + 
                                        " INNER JOIN friends ON foo.user_id = friends.user_id) as foo2" + 
                                        " INNER JOIN customer ON foo2.friend_id = customer.user_id; ";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            friendsList.Add(reader.GetString(0));
                            friendsList_datagrid.Items.Add(new Friend { id = reader.GetString(0), name = reader.GetString(1), yelping_since = reader.GetString(2) });
                        }
                    }
                }
                fillFriendsGrid(friendsList);
                friendsList.Clear();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT business_name, business.business_id FROM (SELECT business_id FROM Favorites WHERE user_id='" + selectedUserID +"') as foo INNER JOIN business ON business.business_id = foo.business_id";
                    using (var reader = cmd.ExecuteReader())
                    {
                        businessFavorites_datagrid.Items.Clear();
                        while (reader.Read())
                        {
                            businessFavorites_datagrid.Items.Add(new Favorite { name = reader.GetString(0), id = reader.GetString(1) });
                        }
                    }
                }
                conn.Close();
            }
        }    

        private void Latitude_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key != System.Windows.Input.Key.Enter) return;
            User selectedUser = (User)userSearchOutput_datagrid.SelectedItem;
            string user_id = null;
            if(userSearchOutput_datagrid.SelectedItem != null)
            {
                user_id = userSearchOutput_datagrid.SelectedItem.ToString();
            }
            if (user_id != null)
            {
                string latitude = latitude_textbox.Text;
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UPDATE customer " +
                                      "SET user_lat='" + latitude + "' " +
                                      "WHERE user_id='" + selectedUser.id + "'";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
        }

        private void Longitude_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            User selectedUser = (User)userSearchOutput_datagrid.SelectedItem;
            string user_id = null;
            if (userSearchOutput_datagrid.SelectedItem != null)
            {
                user_id = userSearchOutput_datagrid.SelectedItem.ToString();
            }
            if (user_id != null)
            {
                string longitude = longitude_textbox.Text;
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UPDATE customer " +
                                      "SET user_longi='" + longitude + "' " +
                                      "WHERE user_id='" + selectedUser.id + "'";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
        }

        private void FriendsList_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RemoveFromFavorites_button_Click(object sender, RoutedEventArgs e)
        {
            /*Favorite business = (Favorite)businessFavorites_datagrid.SelectedItem;
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM checkins WHERE business_id = '" + business.id + "' AND user_id = '" + currentUser.Instance.id + "';";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            */ 
        }

        private void BusinessFavorites_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Favorite curFav = (Favorite)businessFavorites_datagrid.SelectedItem;
            string busid = curFav.id;

            Console.WriteLine(busid);
        }
    }
    //current user is a singleton s.t. mainwindow also knows what user is selected
    /*public class currentUser
    {
        private static UserPage.User instance = null;

        public currentUser()
        {
        }

        public static UserPage.User Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }
    } */
}
