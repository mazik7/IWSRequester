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

namespace Requester
{
    /// <summary>
    /// Interaction logic for AuthInfo.xaml
    /// </summary>
    public partial class AuthInfo : Window
    {
        private Config _cconfig;
        public AuthInfo()
        {
            InitializeComponent();
        }
        public AuthInfo(Config config)
        {
            _cconfig = config;
            InitializeComponent();
        }

        private void btnConfigSave_Click(object sender, RoutedEventArgs e)
        {
            _cconfig.TokenBiz = tBoxBizToken.Text;
            _cconfig.UrlBiz = tBoxBizUrl.Text;
            _cconfig.TokenCom = tBoxComToken.Text;
            _cconfig.UrlCom = tBoxComUrl.Text;
            MainWindow main = this.Owner as MainWindow;
            main._config = _cconfig;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tBoxBizToken.Text = _cconfig.TokenBiz;
            tBoxBizUrl.Text = _cconfig.UrlBiz;
            tBoxComToken.Text = _cconfig.TokenCom;
            tBoxComUrl.Text = _cconfig.UrlCom;
        }
    }
}
