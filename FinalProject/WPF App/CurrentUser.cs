using pagetest;

namespace WPF_App
{
    class CurrentUser
    {
        private static UserPage.User instance = null;
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

        private static MainWindow.Business businst = null;

        public static MainWindow.Business Businst
        {
            get
            {
                return businst;
            }
            set
            {
                businst = value;
            }
        }
    }
}
