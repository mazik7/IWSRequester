using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
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
        private string _jsonFiles;
        private string _methodValue;
        private TempFile _fileToUpload;
        FileViewModelList _files = new FileViewModelList();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs ea)
        {
            if (textBoxBody.Text.Contains("app")&& textBoxBody.Text.Contains("items") && _methodValue == "PUT")
            {
                var request = (HttpWebRequest)WebRequest.Create("http://inworkspace.biz/api/" + textBoxRequest.Text);
                request.Method = _methodValue;
                request.Headers.Add("oauth_token", "70c431c2-89cc-41ea-9a1e-f4aa5af9624a");
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
                var client = new RestClient("http://inworkspace.biz/api");
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
                request.AddHeader("oauth_token", "70c431c2-89cc-41ea-9a1e-f4aa5af9624a");
                request.RequestFormat = RestSharp.DataFormat.Json;
                if (textBoxBody.IsEnabled && textBoxBody.Text.Length > 0)
                {
                    request.AddParameter("application/json", textBoxBody.Text, ParameterType.RequestBody);
                }
                IRestResponse responce;
                responce = client.Execute(request);
                textBoxResponce.Text = responce.StatusCode + "\r\n";
                textBoxResponce.Text += responce.StatusDescription + "\r\n";
                textBoxResponce.Text += responce.Content;
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
            var client = new RestClient("http://inworkspace.biz/api");
            var request = new RestRequest("/files/?storageType=Secured", Method.PUT);
            request.AddHeader("oauth_token", "70c431c2-89cc-41ea-9a1e-f4aa5af9624a");
            request.AddHeader("Content-Type", "multipart/form-data");
            //request.AddParameter("multipart/form-data", fileToUpload.UploadBody, ParameterType.RequestBody);
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
            textBoxResponce.Text += responce.Content;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("files.txt"))
                File.Create("files.txt");
            
            _jsonFiles = File.ReadAllText("files.txt");
            if (_jsonFiles.Length > 0)
            {   
                _files = SimpleJson.DeserializeObject<FileViewModelList>(_jsonFiles); 
                foreach(FileViewModel file in _files.Files)
                {
                    listBoxFiles.Items.Add(file.FileName);
                }
            }
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
    }
}
