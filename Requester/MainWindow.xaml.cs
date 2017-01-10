using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Requester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Collection<Test> _tests = new Collection<Test>();
        private RestClient _testsClient = new RestClient();
        private string _jsonFiles;
        private string _methodValue;
        private TempFile _fileToUpload;
        public Config _config;
        FileViewModelList _files = new FileViewModelList();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TestInit()
        {
            _testsClient.BaseUrl = new Uri(_config.BaseURL);
            _tests.Add(new Test(_testsClient, new RestRequest("/app/?organizationKey=techrunning&workspaceKey=skiing", Method.GET), "1.1 Спискок приложений в рабочей области", "Проверка на отработку запроса \"/app/?organizationKey={organizationKey}&workspaceKey={workspaceKey}\", \nа так же проверка наличия тестовой рабочей области"));
            _tests.Add(new Test(_testsClient, new RestRequest("/app/trainings/", Method.GET), "1.2 Информация о приложении", "Проверка на отработку запроса \"/app/appKey/\", а так же проверка наличия тестового приложения"));
            _tests.Add(new Test(_testsClient, new RestRequest("/app/trainings/fields/", Method.GET), "2.1 Список полей приложения", "Проверка на отработку запроса \"/app/appKey/fields/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/app/trainings/items/", Method.GET), "3.1 Список элементов приложения", "Проверка на отработку запроса \"/app/appKey/items/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/app/items/51761/", Method.GET), "3.1 Элемент приложения", "Проверка на отработку запроса \"/app/items/itemKey/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/organizations/", Method.GET), "4.1 Список организаций", "Проверка на отработку запроса \"/organizations/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/workspaces/?organizationKey=techrunning", Method.GET), "4.2 Список рабочих областей в организации", "Проверка на отработку запроса \"/workspaces/?organizationKey=orgKey/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/?targetType=ApplicationPartRecord&targetKey=51761", Method.GET), "4.3.1 Комментарии элемента приложения", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"ApplicationPartRecord\"(Элемент приложения)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/?contextType=Organization&contextKey=techrunning", Method.GET), "4.5.1 Список активностей организации", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"Organization\"(Организация)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/251925360948376|ef4ebebe-1227-4bc3-95f9-992784934dc0/", Method.GET), "4.6 Контекст полученный по ключу", "Проверка на отработку запроса \"/activities/contextKey/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/userStream/", Method.GET), "5.1 Лента активности текущего пользователя", "Проверка на отработку запроса \"/activities/userStream/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/users/2/", Method.GET), "5.2 Получение информации о пользователе", "Подогреть админчику)0)))))/n Проверка запроса /users/userKey/"));
            _tests.Add(new Test(_testsClient, new RestRequest("/users/current/", Method.GET), "5.4 Получение информации о текущем пользователе", "Азаз, биссмюслица)0))))/nP.S. Запрос выполняется для отображения информации о текущем пользователе на форме"));
            _tests.Add(new Test(_testsClient, new RestRequest("/discussions/", Method.GET), "5.6 Получение списка диалогов", "Проверка запроса \"/discussions/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/discussions/20983229-35c8-4c15-8ee7-97fed3c529bf", Method.GET), "5.7 Получение сообщений диалога", "Проверка запроса \"/discussions/discussionTargetKey\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/contacts/", Method.GET), "6.1 Получение контактов пользователя", "Проверка запроса \"/contacts/\""));


            ListBoxItem item = new ListBoxItem();
            foreach (Test test in _tests)
            {
                item = new ListBoxItem();
                item.Content = test.Name;
                item.ToolTip = test.Description;
                listBoxTests.Items.Add(item);
            }
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs ea)
        {
            if (textBoxBody.Text.Contains("app")&& textBoxBody.Text.Contains("items") && _methodValue == "PUT")
            {
                var request = (HttpWebRequest)WebRequest.Create(_config.BaseURL + textBoxRequest.Text);
                request.Method = _methodValue;
                request.Headers.Add("oauth_token", _config.Token);
                request.ContentType = "application/octet-stream";

                byte[] content = Encoding.UTF8.GetBytes(textBoxBody.Text);
                request.ContentLength = content.Length;

                using (Stream dataStream = request.GetRequestStream())
                using (MemoryStream memoryStream = new MemoryStream(content))
                {
                    memoryStream.CopyTo(dataStream);
                    dataStream.Close();
                }

                string result;
                try
                {
                    using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                catch (SocketException e)
                {
                    result = e.Message;
                }
                catch (WebException e)
                {
                    result = e.Message;
                }
                catch (Exception e)
                {
                    result = e.Message;
                }
            }
            else {
                var client = new RestClient(_config.BaseURL);
                var request = new RestRequest(textBoxRequest.Text);
                switch (_methodValue)
                {
                    case "GET":
                        {
                            request.Method = Method.GET;
                        }
                        break;
                    case "POST":
                        {
                            request.Method = Method.POST;
                        }
                        break;
                    case "PUT":
                        {
                            request.Method = Method.PUT;
                        }
                        break;
                }
                request.AddHeader("oauth_token", _config.Token);
                request.RequestFormat = RestSharp.DataFormat.Json;
                if (textBoxBody.IsEnabled && textBoxBody.Text.Length > 0)
                {
                    request.AddParameter("application/json", textBoxBody.Text, ParameterType.RequestBody);
                }
                IRestResponse responce;
                responce = client.Execute(request);
                if(responce.StatusCode == HttpStatusCode.OK)
                    lblEventStatus.Foreground = Brushes.LimeGreen;
                else
                    lblEventStatus.Foreground = Brushes.DarkRed;
                lblEventStatus.Content = responce.StatusCode;
                textBoxResponce.Text = responce.StatusDescription + "\r\n";
                textBoxResponce.Text += JsonHelper.FormatJson(responce.Content);
            }
        }

        private void listBoxMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = listBoxMethod.SelectedIndex;
            switch(selected)
            {
                case 0: 
                    {
                        btnLaunch.IsEnabled = true;
                        _methodValue = "GET";
                        if (textBoxBody != null)
                            textBoxBody.IsEnabled = false;
                    }
                    break;
                case 1:
                    {
                        btnLaunch.IsEnabled = true;
                        _methodValue = "POST";
                        if (textBoxBody != null)
                            textBoxBody.IsEnabled = true;
                    }
                    break;
                case 2:
                    {
                        btnLaunch.IsEnabled = true;
                        _methodValue = "PUT";
                        if (textBoxBody != null)
                            textBoxBody.IsEnabled = true;
                    }
                    break;
                default:
                    {
                        btnLaunch.IsEnabled = false;
                    }
                    break;
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog selectFile = new Microsoft.Win32.OpenFileDialog();
            selectFile.CheckFileExists = true;
            if(selectFile.ShowDialog() == true)
            {
                _fileToUpload = new TempFile(selectFile.FileName);
            }
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(_config.BaseURL);
            var request = new RestRequest("/files/?storageType=Secured", Method.PUT);
            request.AddHeader("oauth_token", _config.Token);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;
            IRestResponse responce;
            request.AddFile("file1", _fileToUpload.LocalPath, _fileToUpload.MimeType);
            responce = client.Execute(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                FileViewModel file = SimpleJson.DeserializeObject<FileViewModel>(responce.Content);
                _files.Files.Add(file);
                listBoxFiles.Items.Add(file.FileName);
                File.WriteAllText("files.txt", SimpleJson.SerializeObject(_files));
            }
            textBoxResponce.Text = responce.StatusCode+ "\r\n";
            textBoxResponce.Text += responce.StatusDescription + "\r\n";
            textBoxResponce.Text += JsonHelper.FormatJson(responce.Content);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("files.txt"))
                File.Create("files.txt");
            if (!File.Exists("Config.txt"))
                File.Create("Config.txt");
            _jsonFiles = File.ReadAllText("files.txt");
            if (_jsonFiles.Length > 0)
            {   
                _files = SimpleJson.DeserializeObject<FileViewModelList>(_jsonFiles); 
                foreach(FileViewModel file in _files.Files)
                {
                    listBoxFiles.Items.Add(file.FileName);
                }
            }
            _jsonFiles = File.ReadAllText("Config.txt");
            if (_jsonFiles.Length > 0)
            {
                _config = SimpleJson.DeserializeObject<Config>(_jsonFiles);
                SetConfig();
            }
            else
                _config = new Config();
            TestInit();
            lblCurrentUser.Content = new CurrentUser(_config).UserName;
        }

        private void listBoxFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBoxFiles.SelectedItem != null)
            {
                foreach(FileViewModel file in _files.Files)
                {
                    if(listBoxFiles.SelectedItem.ToString().Contains(file.FileName))
                    {
                        Clipboard.SetText(string.Format("\"FileName\":\"{0}\",\n\"Url\":\"{2}\",\n\"Size\":{3},\n\"MimeType\":\"{1}\"", file.FileName, file.MimeType, file.Url, file.Size));
                        lblEventStatus.Content = string.Format("\"{0}\" filedata was copied", file.FileName);
                        lblEventStatus.Foreground = Brushes.LimeGreen;
                        break;
                    }
                }
            }
        }

        private void listBoxFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if (listBoxFiles.SelectedItem != null)
                {
                    foreach (FileViewModel file in _files.Files)
                    {
                        if (listBoxFiles.SelectedItem.ToString().Contains(file.FileName))
                        {
                            _files.Files.Remove(file);
                            listBoxFiles.Items.Remove(listBoxFiles.SelectedItem);
                            break;
                        }
                    }
                    File.WriteAllText("files.txt", SimpleJson.SerializeObject(_files));
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _config.Request = textBoxRequest.Text;
            _config.Responce = textBoxResponce.Text;
            _config.Body = textBoxBody.Text;
            _config.Method = _methodValue;
            if (rBtnBiz.IsChecked == true)
                _config.IsBiz = true;
            else
                _config.IsBiz = false;
            File.WriteAllText("Config.txt", SimpleJson.SerializeObject(_config));
        }
        private void SetConfig()
        {
            textBoxRequest.Text = _config.Request;
            textBoxResponce.Text = _config.Responce;
            textBoxBody.Text = _config.Body;
            _methodValue = _config.Method;
            if (_config.IsBiz)
                rBtnBiz.IsChecked = true;
            else
                rBtnCom.IsChecked = true;
        }

        private void btnEditAuthData_Click(object sender, RoutedEventArgs e)
        {
            Window cfgWindow = new AuthInfo(_config);
            cfgWindow.Owner = this;
            cfgWindow.ShowDialog();
        }

        private void rBtnBiz_Checked(object sender, RoutedEventArgs e)
        {
            if (_config != null)
            {
                _config.IsBiz = true;
                _testsClient.BaseUrl = new Uri(_config.BaseURL);
                lblCurrentUser.Content = new CurrentUser(_config).UserName;
            }
        }

        private void rBtnCom_Checked(object sender, RoutedEventArgs e)
        {
            if (_config != null)
            {
                _config.IsBiz = false;
                _testsClient.BaseUrl = new Uri(_config.BaseURL);
                lblCurrentUser.Content = new CurrentUser(_config).UserName;
            }
        }

        private void listBoxTests_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Collection<string> items = new Collection<string>();
            Collection<Test> testsList = new Collection<Test>();
            Collection<Result> testsResult;
            if (listBoxTests.SelectedItems.Count == 1)
            {
                textBoxLog.Text += "Тест \"" + _tests[listBoxTests.SelectedIndex].Name + "\" пошел\n";
                Result testResult = _tests[listBoxTests.SelectedIndex].Start(_config.Token);
                textBoxLog.Text += "Статус " + testResult.State + "\n";
                if (testResult.Body != "")
                    textBoxLog.Text += "Тело ответа: " + JsonHelper.FormatJson(testResult.Body) + "\n";
                else
                    textBoxLog.Text += "В теле ответа пришла пустота... \n";
            }
            else if (listBoxTests.SelectedItems.Count > 1)
            {
                foreach (ListBoxItem item in listBoxTests.SelectedItems)
                {
                    items.Add(item.Content.ToString());
                }
                foreach(string item in items)
                {
                    foreach(Test test in _tests)
                    {
                        if(item.Contains(test.Name))
                        {
                            testsList.Add(test);
                            break;
                        }
                    }
                }
                textBoxLog.Text += "Запускаем несколько тестов. Придется долго ждать пока я не закончу. Вахахах" + "\n";
                TestSuite suite = new TestSuite(testsList);
                testsResult = suite.Start(_config.Token);
                textBoxLog.Text += "Информация о прошедших тестах..." + "\n";
                textBoxLog.Text += "Всего тестов: "+ suite.TotalCount + "\n";
                textBoxLog.Text += "Успешных: " + suite.PassedCount + "\n";
                textBoxLog.Text += "Неудачников: " + suite.FailedCount + "\n";
                if(testsResult!= null)
                {
                    textBoxLog.Text += "О неудачниках: " + "\n";
                    foreach(Result failedResult in testsResult)
                    {
                        textBoxLog.Text += "-----------------------------------------------\n";
                        textBoxLog.Text += "Запрос: " + failedResult.URL + "\n";
                        textBoxLog.Text += "Код ответа: " + failedResult.State + "\n";
                        textBoxLog.Text += "Подробней: " + failedResult.Description + "\n";
                        if(failedResult.Body != "")
                            textBoxLog.Text += "Тело ответа: " + JsonHelper.FormatJson(failedResult.Body) + "\n";
                        else
                            textBoxLog.Text += "В теле ответа пришла пустота... \n";
                    }
                }
            }
        }
    }
}
