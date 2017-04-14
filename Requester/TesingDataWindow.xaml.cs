using System.Windows;
using RestSharp;
using System.IO;

namespace Requester
{
    /// <summary>
    /// Interaction logic for TesingDataWindow.xaml
    /// </summary>
    public partial class TesingDataWindow : Window
    {
        private TestingData _testData;
        private bool _isBiz;
        public TesingDataWindow()
        {
            InitializeComponent();
        }
        public TesingDataWindow(TestingData testingData, bool isBiz)
        {
            _testData = testingData;
            _isBiz = isBiz;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxApplication.Text = _testData.Application;
            textBoxApplicationItem.Text = _testData.ApplicationItem;
            textBoxOrganization.Text = _testData.Organisation;
            textBoxTask.Text = _testData.Task;
            textBoxUser.Text = _testData.User;
            textBoxWorkspace.Text = _testData.Workspace;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            _testData.Application = textBoxApplication.Text;
            _testData.ApplicationItem = textBoxApplicationItem.Text;
            _testData.Organisation = textBoxOrganization.Text;
            _testData.Task = textBoxTask.Text;
            _testData.User = textBoxUser.Text;
            _testData.Workspace = textBoxWorkspace.Text;
            if (_isBiz)
            {
                File.WriteAllText("TestingDataBiz.txt", SimpleJson.SerializeObject(_testData));
            }
            else
            {
                File.WriteAllText("TestingDataCom.txt", SimpleJson.SerializeObject(_testData));
            }
            MainWindow main = Owner as MainWindow;
            main._testingData = _testData;
            Close();
        }
    }
}
