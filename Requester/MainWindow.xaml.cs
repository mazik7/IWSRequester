using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private ContextMenu _listTestsItemMenu;
        private TempFile _fileToUpload;
        public Config _config;
        public TestingData _testingData;
        MenuItem _copyTestItem;
        MenuItem _callTestingDataEdit;
        FileViewModelList _files = new FileViewModelList();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TestingDataInit()
        {
            if (_config.IsBiz)
            {
                _jsonFiles = File.ReadAllText("TestingDataBiz.txt");
                if (_jsonFiles.Length > 0)
                {
                    _testingData = SimpleJson.DeserializeObject<TestingData>(_jsonFiles);
                }
                else
                {
                    _testingData = new TestingData();
                    _testingData.Application = "a46";
                    _testingData.Workspace = "testws";
                    _testingData.Organisation = "tstorg";
                    _testingData.User = "2";
                    _testingData.Task = "54499";
                    _testingData.ApplicationItem = "51761";
                    File.WriteAllText("TestingDataBiz.txt", SimpleJson.SerializeObject(_testingData));
                }
            }
            else
            {
                _jsonFiles = File.ReadAllText("TestingDataCom.txt");
                if (_jsonFiles.Length > 0)
                {
                    _testingData = SimpleJson.DeserializeObject<TestingData>(_jsonFiles);
                }
                else
                {
                    _testingData = new TestingData();
                    _testingData.Application = "a46";
                    _testingData.Workspace = "testws";
                    _testingData.Organisation = "tstorg";
                    _testingData.User = "2";
                    _testingData.Task = "54499";
                    _testingData.ApplicationItem = "51761";
                    File.WriteAllText("TestingDataCom.txt", SimpleJson.SerializeObject(_testingData));
                }
            }
        }
        private void TestInit()
        {
            _listTestsItemMenu = new ContextMenu();
            _listTestsItemMenu.MaxWidth = 300;
            _listTestsItemMenu.MaxHeight = 100;
            _copyTestItem = new MenuItem();
            _copyTestItem.Header = "Copy to custom request tab";
            _copyTestItem.Click += new RoutedEventHandler(CopySelectedTest);
            _callTestingDataEdit = new MenuItem();
            _callTestingDataEdit.Header = "Edit testing data";
            _callTestingDataEdit.Click += new RoutedEventHandler(StartTestingDataEditWindow);
            _listTestsItemMenu.Items.Add(_copyTestItem);
            _listTestsItemMenu.Items.Add(_callTestingDataEdit);
            _testsClient.BaseUrl = new Uri(_config.BaseURL);
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/?organizationKey={0}&workspaceKey={1}", _testingData.Organisation, _testingData.Workspace), Method.GET), "1.1 Спискок приложений в рабочей области", "Проверка на отработку запроса \"/app/?organizationKey={organizationKey}&workspaceKey={workspaceKey}\", \nа так же проверка наличия тестовой рабочей области"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/", _testingData.Application), Method.GET), "1.2 Информация о приложении", "Проверка на отработку запроса \"/app/appKey/\", а так же проверка наличия тестового приложения"));
            _tests.Add(new Test(_testsClient, new RestRequest("/app/", Method.PUT), "1.3 Новое приложение", "Проверка на отработку запроса \"/app\", на создание приложения в рабочей области \"tstorg|testws\"", "{\"WorkspaceKey\":\""+ _testingData.Organisation + "|"+ _testingData.Workspace + "\",\"DisplayName\":\"TestApp\",\"ItemDisplayName\":\"Item\",\"IconName\":\"6x3\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/update", _testingData.Application), Method.POST), "1.4 Изменение приложения", "Проверка на отработку запроса \"/app/appKey/\", а так же проверка наличия тестового приложения", "{\"WorkspaceKey\":\""+ _testingData.Organisation + "|"+_testingData.Workspace+"\",\"DisplayName\":\"TestAppUpdate\",\"ItemDisplayName\":\"Item\",\"IconName\":\"6x3\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/fields/", _testingData.Application), Method.GET), "2.1 Список полей приложения", "Проверка на отработку запроса \"/app/appKey/fields/\""));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/fields/field8/delete", _testingData.Application), Method.POST), "2.4 Удаление поля из приложения", "Проверка на отработку POST запроса \"/app/appKey/fields/fieldKey/delete\""));
            //Нужно разобраться с полями ордер и InternalName
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/fields/", _testingData.Application), Method.PUT), "2.2 Добавление поля в приложение", "Проверка на отработку PUT запроса \"/app/appKey/fields/\", а так же проверка наличия тестового приложения", "{\"ApplicationMetadataKey\": \""+ _testingData.Application + "\",\"DisplayName\": \"Текст\",\"InternalName\": \"field8\",\"Order\": 5,\"Settings\": [{\"Key\": \"StringFieldSettings.Required\",\"Value\": \"False\"},{\"Key\": \"StringFieldSettings.Hint\",\"Value\": \"\"},{\"Key\": \"StringFieldSettings.Flavor\",\"Value\": \"Markdown\"},{\"Key\": \"StringFieldSettings.IsTitle\",\"Value\": \"\"}],\"TypeName\": \"StringField\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/fields/update", _testingData.Application), Method.POST), "2.3 Изменение полей приложения", "Проверка на отработку PUT запроса \"/app/appKey/fields/\", а так же проверка наличия тестового приложения", "{\"ApplicationMetadataKey\": \""+ _testingData.Application + "\",\"DisplayName\": \"Текст\",\"InternalName\": \"field\",\"Order\": 0,\"Settings\": [{\"Key\": \"StringFieldSettings.Required\",\"Value\": \"False\"},{\"Key\": \"StringFieldSettings.Hint\",\"Value\": \"\"},{\"Key\": \"StringFieldSettings.Flavor\",\"Value\": \"Markdown\"},{\"Key\": \"StringFieldSettings.IsTitle\",\"Value\": \"\"}],\"TypeName\": \"StringField\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/items/", _testingData.Application), Method.GET), "3.1 Список элементов приложения", "Проверка на отработку запроса \"/app/appKey/items/\""));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/items/{0}/", _testingData.ApplicationItem), Method.GET), "3.2 Элемент приложения", "Проверка на отработку запроса \"/app/items/itemKey/\""));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/{0}/items/", _testingData.Application), Method.PUT), "3.3 Добавление элемента в приложение", "Проверка на отработку PUT запроса \"/app/appKey/items/\"", "[{\"InternalName\": \"field\",\"Value\": \"ydr11tuyrtur\"},{\"InternalName\": \"field1\",\"Value\": \"ytur111turtu\"},{\"InternalName\": \"field2\",\"Value\": \"232\"}]"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/app/items/{0}/update", _testingData.ApplicationItem), Method.PUT), "3.4 Обновление элемента приложения", "Проверка на отработку запроса \"/app/items/itemKey/update\"", "[{\"InternalName\": \"field\",\"Value\": \"ydr11tuyrtur\"},{\"InternalName\": \"field1\",\"Value\": \"ytur111turtu\"},{\"InternalName\": \"field2\",\"Value\": \"232\"}]"));
            //Не работает. Не придумана схема
            _tests.Add(new Test(_testsClient, new RestRequest("/app/items/{itemKey}/delete", Method.POST), "3.5 Удаление элемента приложения", "Проверка на отработку запроса \"/app/items/itemKey/delete\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/organizations/", Method.GET), "4.1 Список организаций", "Проверка на отработку запроса \"/organizations/\""));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/workspaces/?organizationKey={0}", _testingData.Organisation), Method.GET), "4.2 Список рабочих областей в организации", "Проверка на отработку запроса \"/workspaces/?organizationKey=orgKey/\""));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/comments/?targetType=ApplicationPartRecord&targetKey={0}", _testingData.ApplicationItem), Method.GET), "4.3.1 Комментарии элемента приложения", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"ApplicationPartRecord\"(Элемент приложения)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.1 Новый комментарий элемента приложения", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"ApplicationPartRecord\"(Элемент приложения)", "{\"TargetType\": \"ApplicationPartRecord\",\"TargetKey\": \""+ _testingData.ApplicationItem + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/?targetType=StatusData&targetKey=251938924079240_1727685f-37c4-412e-9baa-25e489bf88d0", Method.GET), "4.3.2 Комментарии статуса", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"StatusData\"(Статус)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.2 Новый комментарий статуса", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"StatusData\"(Статус)", "{\"TargetType\": \"StatusData\",\"TargetKey\": \"251938924079240_1727685f-37c4-412e-9baa-25e489bf88d0\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/comments/?targetType=Organization&targetKey={0}", _testingData.Organisation), Method.GET), "4.3.3 Комментарии контекста организации", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"Organization\"(Организация)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.3 Новый комментарий контекста организации", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"Organization\"(Организация)", "{\"TargetType\": \"Organization\",\"TargetKey\": \""+ _testingData.Organisation + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/comments/?targetType=UserOrganization&targetKey=23658|{0}", _testingData.Organisation), Method.GET), "4.3.4 Комментарии контекста пользователя в организации", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"UserOrganization\"(Пользователь в организации)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.4 Новый комментарий контекста пользователя", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"UserOrganization\"(Пользователь в организации)", "{\"TargetType\": \"UserOrganization\",\"TargetKey\": \"23658|"+ _testingData.Organisation + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/comments/?targetType=Workspace&targetKey={0}|{1}", _testingData.Organisation, _testingData.Workspace), Method.GET), "4.3.5 Комментарии контекста рабочей области", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"Workspace\"(Рабочая область)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.5 Новый комментарий контекста рабочей области", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"Workspace\"(Рабочая область)", "{\"TargetType\": \"Workspace\",\"TargetKey\": \"" + _testingData.Organisation + "|" + _testingData.Workspace + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/comments/?targetType=UserWorkspace&targetKey=23658|{0}||{1}", _testingData.Organisation, _testingData.Workspace), Method.GET), "4.3.6 Комментарии контекста пользователя в рабочей области", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"UserWorkspace\"(Пользователь в рабочей области)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.6 Новый комментарий контекста пользователя в рабочей области", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"UserWorkspace\"(Пользователь в рабочей области)", "{\"TargetType\": \"UserWorkspace\",\"TargetKey\": \"23658|"+ _testingData.Organisation + "||"+ _testingData.Workspace + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/comments/?targetType=ApplicationMetadata&targetKey={0}", _testingData.Application), Method.GET), "4.3.7 Комментарии приложения", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"ApplicationMetadata\"(Приложение)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.7 Новый комментарий контекста приложения", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"ApplicationMetadata\"(Приложение)", "{\"TargetType\": \"ApplicationMetadata\",\"TargetKey\": \""+ _testingData.ApplicationItem + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/comments/?targetType=TaskPartRecord&targetKey={0}", _testingData.Task), Method.GET), "4.3.8 Комментарии задачи", "Проверка на отработку запроса \"/comments/?targetType=type&targetKey=key\" \n для типа \"TaskPartRecord\"(Задача)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/comments/", Method.PUT), "4.4.8 Новый комментарий контекста задачи", "Проверка на отработку PUT запроса  \"/comments/\" \n для типа \"TaskPartRecord\"(Задача)", "{\"TargetType\": \"TaskPartRecord\",\"TargetKey\": \""+ _testingData.Task + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/activities/?contextType=Organization&contextKey={0}", _testingData.Organisation), Method.GET), "4.5.1 Список активностей организации", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"Organization\"(Организация)"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/activities/?contextType=ApplicationPartRecord&contextKey={0}", _testingData.ApplicationItem), Method.GET), "4.5.2 Список активностей элемента приложения", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"ApplicationPartRecord\"(Элемент приложения)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/?contextType=StatusData&contextKey=251938924079240_1727685f-37c4-412e-9baa-25e489bf88d0", Method.GET), "4.5.3 Список активностей статуса", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"StatusData\"(Статус)"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/activities/?contextType=UserOrganization&contextKey=23658|{0}", _testingData.Organisation), Method.GET), "4.5.4 Список активностей контекста пользователя в организации", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"UserOrganization\"(Пользователь в организации)"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/activities/?contextType=Workspace&contextKey={0}|{1}", _testingData.Organisation, _testingData.Workspace), Method.GET), "4.5.5 Список активностей контекста рабочей области", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"Workspace\"(Рабочая область)"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/activities/?contextType=UserWorkspace&contextKey=23658|{0}||{1}", _testingData.Organisation, _testingData.Workspace), Method.GET), "4.5.6 Список активностей контекста пользователя в рабочей области", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"UserWorkspace\"(Пользователь в рабочей области)"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/activities/?contextType=ApplicationMetadata&contextKey={0}", _testingData.Application), Method.GET), "4.5.7 Список активностей приложения", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"ApplicationMetadata\"(Приложение)"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/activities/?contextType=TaskPartRecord&contextKey={0}", _testingData.Task), Method.GET), "4.5.8 Список активностей задачи", "Проверка на отработку запроса \"/activities/?contextType=type&contextKey=key\" \n для типа \"TaskPartRecord\"(Задача)"));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/251925360948376|ef4ebebe-1227-4bc3-95f9-992784934dc0/", Method.GET), "4.6 Контекст полученный по ключу", "Проверка на отработку запроса \"/activities/contextKey/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/userStream/", Method.GET), "5.1 Лента активности текущего пользователя", "Проверка на отработку запроса \"/activities/userStream/\""));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/users/{0}/", _testingData.User), Method.GET), "5.2 Получение информации о пользователе", "Подогреть админчику)0)))))/n Проверка запроса /users/userKey/"));
            _tests.Add(new Test(_testsClient, new RestRequest("/users/current/", Method.GET), "5.3 Получение информации о текущем пользователе", "Азаз, биссмюслица)0))))/nP.S. Запрос выполняется для отображения информации о текущем пользователе на форме"));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/status/", Method.PUT), "5.4.1 Новый статус в организации", "Проверка на отработку PUT запроса  \"/activities/status/\" \n для ленты типа \"Organization\"(Организации)", "{\"StreamType\": \"Organization\",\"StreamKey\": \""+ _testingData.Organisation + "\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest("/activities/status/", Method.PUT), "5.4.2 Новый статус в рабочей области", "Проверка на отработку PUT запроса  \"/activities/status/\" \n для ленты типа \"Workspace\"(Рабочая область)", "{\"StreamType\": \"Workspace\",\"StreamKey\": \""+_testingData.Organisation+"|"+_testingData.Workspace+"\",\"Text\": \"All hail!!!\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest("/StatusData/251938924079240_1727685f-37c4-412e-9baa-25e489bf88d0/likeorunlike", Method.POST), "5.5.1 Отметка \"Лукас\"(Статус)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа StatusData"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/Organization/{0}/likeorunlike", _testingData.Organisation), Method.POST), "5.5.2 Отметка \"Лукас\"(Организация)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа Organization"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/UserOrganization/23658|{0}/likeorunlike", _testingData.Organisation), Method.POST), "5.5.3 Отметка \"Лукас\"(Пользователь в организации)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа UserOrganization"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/Workspace/{0}|{1}/likeorunlike", _testingData.Organisation, _testingData.Workspace), Method.POST), "5.5.4 Отметка \"Лукас\"(Рабочая область)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа Workspace"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/UserWorkspace/23658|{0}||{1}/likeorunlike", _testingData.Organisation, _testingData.Workspace), Method.POST), "5.5.5 Отметка \"Лукас\"(Пользователь в рабочей области)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа UserWorkspace"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/ApplicationMetadata/{0}/likeorunlike", _testingData.Application), Method.POST), "5.5.6 Отметка \"Лукас\"(Приложение)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа ApplicationMetadata"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/ApplicationPartRecord/{0}/likeorunlike", _testingData.ApplicationItem), Method.POST), "5.5.7 Отметка \"Лукас\"(Элемент приложения)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа ApplicationPartRecord"));
            _tests.Add(new Test(_testsClient, new RestRequest(string.Format("/TaskPartRecord/{0}/likeorunlike", _testingData.Task), Method.POST), "5.5.8 Отметка \"Лукас\"(Задача)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа TaskPartRecord"));
            _tests.Add(new Test(_testsClient, new RestRequest("/Comment/2519102251072752420/likeorunlike", Method.POST), "5.5.9 Отметка \"Лукас\"(Комментарий)", "Проверка на отработку запроса \"/targetType/targetKey/likeorunlike\" для типа Comment"));
            _tests.Add(new Test(_testsClient, new RestRequest("/discussions/", Method.GET), "5.6 Получение списка диалогов", "Проверка запроса \"/discussions/\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/discussions/20983229-35c8-4c15-8ee7-97fed3c529bf", Method.GET), "5.7 Получение сообщений диалога", "Проверка запроса \"/discussions/discussionTargetKey\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/discussions/", Method.PUT), "5.8 Создание нового диалога", "Проверка на отработку PUT запроса  \"/discussions/\"", "{\"Body\": \"BodyBodyBody\",\"Subject\": \"SubjectSubject\",\"RecipientKeys\": ["+ _testingData.User + "]}"));
            _tests.Add(new Test(_testsClient, new RestRequest("/discussions/20983229-35c8-4c15-8ee7-97fed3c529bf", Method.PUT), "5.9 Создание сообщения в диалоге", "Проверка PUT запроса \"/discussions/discussionTargetKey\"", "{\"Body\": \"HOTBODY\"}"));
            _tests.Add(new Test(_testsClient, new RestRequest("/discussions/20983229-35c8-4c15-8ee7-97fed3c529bf/MarkAsViewed", Method.POST), "5.10 Пометка диалога прочитанным", "Проверка запроса \"/discussions/discussionTargetKey/MarkAsViewed\""));
            _tests.Add(new Test(_testsClient, new RestRequest("/contacts/", Method.GET), "6.1 Получение контактов пользователя", "Проверка запроса \"/contacts/\""));

            ListBoxItem item = new ListBoxItem();
            foreach (Test test in _tests)
            {
                item = new ListBoxItem();
                item.ContextMenu = _listTestsItemMenu;
                item.Content = test.Name;
                item.Cursor = Cursors.Hand;
                item.ToolTip = test.Description;
                item.MouseDoubleClick += new MouseButtonEventHandler(listBoxTestItem_MouseDoubleClick);
                listBoxTests.Items.Add(item);
            }
        }
        private void StartTestingDataEditWindow(object sender, RoutedEventArgs ea)
        {
            Window tstDataWindow = new TesingDataWindow(_testingData, _config.IsBiz);
            tstDataWindow.Owner = this;
            tstDataWindow.ShowDialog();
        }
        private void CopySelectedTest(object sender, RoutedEventArgs ea)
        {
            MenuItem contextMenuItem = (MenuItem)sender;
            ListBoxItem selectedTestMenuItem = null;
            if (contextMenuItem != null)
            {
                selectedTestMenuItem = ((ContextMenu)contextMenuItem.Parent).PlacementTarget as ListBoxItem;
                foreach (Test test in _tests)
                {
                    if ((string)selectedTestMenuItem.Content == test.Name)
                    {
                        textBoxRequest.Text = test.Request.Resource;
                        if (test.Request.Parameters.Count == 0)
                        {
                            textBoxBody.Text = "";
                        }
                        else
                        {
                            bool hasBody = false;
                            foreach (Parameter param in test.Request.Parameters)
                            {
                                if (param.Type == ParameterType.RequestBody)
                                {
                                    textBoxBody.Text = JsonHelper.FormatJson(param.Value.ToString());
                                    hasBody = true;
                                    break;
                                }
                            }
                            if(!hasBody)
                            {
                                textBoxBody.Text = "";
                            }
                        }
                        string method = test.Request.Method.ToString();
                        foreach (ListBoxItem item in listBoxMethod.Items)
                        {
                            if ((string)item.Content == method)
                            {
                                item.IsSelected = true;
                            }
                        }
                        requestTabItem.IsSelected = true;
                        break;
                    }
                }
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
            if (!File.Exists("TestingDataBiz.txt"))
                File.Create("TestingDataBiz.txt");
            if (!File.Exists("TestingDataCom.txt"))
                File.Create("TestingDataCom.txt");
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
            TestingDataInit();
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
            foreach (ListBoxItem item in listBoxMethod.Items)
            {
                if ((string)item.Content == _config.Method)
                {
                    item.IsSelected = true;
                }
            }
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
                TestingDataInit();
            }
        }

        private void rBtnCom_Checked(object sender, RoutedEventArgs e)
        {
            if (_config != null)
            {
                _config.IsBiz = false;
                _testsClient.BaseUrl = new Uri(_config.BaseURL);
                lblCurrentUser.Content = new CurrentUser(_config).UserName;
                TestingDataInit();
            }
        }

        private void StartSelected()
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
                foreach (string item in items)
                {
                    foreach (Test test in _tests)
                    {
                        if (item.Contains(test.Name))
                        {
                            testsList.Add(test);
                            break;
                        }
                    }
                }
                textBoxLog.Dispatcher.Invoke(new ThreadStart(delegate { textBoxLog.AppendText("Запускаем несколько тестов. Придется долго ждать пока я не закончу. Вахахах" + "\n"); }));
                //textBoxLog.AppendText("Запускаем несколько тестов. Придется долго ждать пока я не закончу. Вахахах" + "\n");
                TestSuite suite = new TestSuite(testsList);
                testsResult = suite.Start(_config.Token);
                textBoxLog.Text += "Информация о прошедших тестах..." + "\n";
                textBoxLog.Text += "Всего тестов: " + suite.TotalCount + "\n";
                textBoxLog.Text += "Успешных: " + suite.PassedCount + "\n";
                textBoxLog.Text += "Неудачников: " + suite.FailedCount + "\n";
                if (testsResult != null)
                {
                    textBoxLog.Text += "О неудачниках: " + "\n";
                    foreach (Result failedResult in testsResult)
                    {
                        textBoxLog.Text += "-----------------------------------------------\n";
                        textBoxLog.Text += "Запрос: " + failedResult.URL + "\n";
                        textBoxLog.Text += "Код ответа: " + failedResult.State + "\n";
                        textBoxLog.Text += "Подробней: " + failedResult.Description + "\n";
                        if (failedResult.Body != "")
                            textBoxLog.Text += "Тело ответа: " + JsonHelper.FormatJson(failedResult.Body) + "\n";
                        else
                            textBoxLog.Text += "В теле ответа пришла пустота... \n";
                    }
                }
            }
        }

        private void listBoxTestItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            Result testResult;
            foreach (Test test in _tests)
            {
                if ((string)item.Content == test.Name)
                {
                    textBoxLog.Text += "Тест \"" + test.Name + "\" пошел\n";
                    testResult = test.Start(_config.Token);
                    textBoxLog.Text += "Статус " + testResult.State + "\n";
                    if (testResult.Body != "")
                        textBoxLog.Text += "Тело ответа: " + JsonHelper.FormatJson(testResult.Body) + "\n";
                    else
                        textBoxLog.Text += "В теле ответа пришла пустота... \n";
                    break;
                }
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            StartSelected();
        }

        private void listBoxTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTests.SelectedItems.Count <= 0)
                buttonStart.IsEnabled = false;
            else
                buttonStart.IsEnabled = true;
        }

        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxTests.Items.Count != listBoxTests.SelectedItems.Count)
            {
                foreach (ListBoxItem item in listBoxTests.Items)
                {
                    item.IsSelected = true;
                }
            }
            else
            {
                foreach (ListBoxItem item in listBoxTests.Items)
                {
                    item.IsSelected = false;
                }
            }
        }

        private void buttonClearLog_Click(object sender, RoutedEventArgs e)
        {
            textBoxLog.Text = "";
        }
    }
}
