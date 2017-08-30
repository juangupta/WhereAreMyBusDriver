using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhereAreMyBusDriver.Models;

namespace WhereAreMyBusDriver.ViewModels
{
    public class MainViewModel
    {
        #region Properties
        public LoginViewModel Login { get; set; }
        public DriverViewModel myDriver { get; set; }
        public Driver Driver { get; set; }

        #endregion
        #region Constructors
        public MainViewModel()
        {
            instance = this;
            Login = new LoginViewModel();
        }
        #endregion

        #region Singleton
        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }

            return instance;
        }
        #endregion
    }
}
