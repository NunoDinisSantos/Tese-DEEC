using SuperUser.Models;
using SuperUser.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SuperUser
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }

    public partial class MainViewModel : INotifyPropertyChanged
    {
        private readonly StudentService _studentService;
        public ObservableCollection<Student> Students { get; set; } = [];
        private Student _currentStudent;

        public Student SelectedStudent
        {
            get => _currentStudent;
            set
            {
                _currentStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }
        public ICommand UpdateCustomCreditCommand { get; }
        public ICommand GetStudentByIdAsync { get; }
        public ICommand RefreshData { get; }

        private int _customCreditsAmount;
        public int CustomCreditsAmount
        {
            get => _customCreditsAmount;
            set
            {
                _customCreditsAmount = value;
                OnPropertyChanged(nameof(CustomCreditsAmount));
            }
        }

        private bool _waiting;
        public bool Waiting
        {
            get => _waiting;
            set
            {
                _waiting = value;
                OnPropertyChanged(nameof(Waiting));
            }
        }

        private string _updateCreditsMessage;
        public string UpdateCreditsMessage
        {
            get => _updateCreditsMessage;
            set
            {
                _updateCreditsMessage = value;
                OnPropertyChanged(nameof(UpdateCreditsMessage));
            }
        }

        private string _studentNumberToSearch;

        public string StudentNumberToSearch
        {
            get => _studentNumberToSearch;
            set
            {
                _studentNumberToSearch = value;
                OnPropertyChanged(nameof(StudentNumberToSearch));
            }
        }

        private string _showErrorMessage;

        public string ShowErrorMessage
        {
            get => _showErrorMessage;
            set
            {
                _showErrorMessage = value;
                OnPropertyChanged(nameof(ShowErrorMessage));
            }
        }

        public MainViewModel(StudentService studentService)
        {
            _studentService = studentService;
            UpdateCustomCreditCommand = new Command(async () => await UpdateCredits(CustomCreditsAmount));
            GetStudentByIdAsync = new Command(async () => await GetStudentById(StudentNumberToSearch));
            RefreshData = new Command(() => Refresh());

            LoadStudents();
        }

        private async void LoadStudents()
        {
            Waiting = true;

            var response = await _studentService.GetStudentsAsync();

            if (response.Count > 0)
            {
                _showErrorMessage = string.Empty;
                OnPropertyChanged(nameof(ShowErrorMessage));
                Students.Clear();
                foreach (var student in response)
                {
                    Students.Add(student);
                }
            }

            else
            {
                Students.Clear();
                _showErrorMessage = "Falha ao encontrar estudantes. A API poderá estar em baixo.";
                OnPropertyChanged(nameof(ShowErrorMessage));
            }

            Waiting = false;
        }

        private void Refresh()
        {
            LoadStudents();
        }

        private async Task GetStudentById(string studentId)
        {
            Waiting = true;

            var response = await _studentService.GetStudentAsync(studentId);

            if (string.Equals(response.PlayerId, "NOTFOUND"))
            {
                _showErrorMessage = "Falha ao encontrar estudante. A API poderá estar em baixo.";
                OnPropertyChanged(nameof(ShowErrorMessage));
            }
            else
            {
                _showErrorMessage = string.Empty;
                OnPropertyChanged(nameof(ShowErrorMessage));
                Students.Clear();
                Students.Add(response);
            }

            Waiting = false;
        }

        private async Task UpdateCredits(int sumCredits)
        {
            if (SelectedStudent != null)
            {
                Waiting = true;

                string message = string.Empty;
                var result = await _studentService.UpdateStudentCreditsAsync(SelectedStudent.PlayerId, sumCredits);
                if (result != HttpStatusCode.OK)
                {
                    message = "Algo falhou a tentar actualizar créditos.";
                    _updateCreditsMessage = message;
                    Application.Current.MainPage.Dispatcher.Dispatch(() => UpdateCreditsMessage = _updateCreditsMessage);
                    OnPropertyChanged(nameof(CustomCreditsAmount));
                }

                else
                {
                    if (_customCreditsAmount < 0)
                    {
                        message = "Foram subtraídos " + (_customCreditsAmount * -1) + " créditos ao estudante " + SelectedStudent.PlayerId;
                    }

                    else 
                    {
                        message = "Foram adicionados " + _customCreditsAmount + " créditos ao estudante " + SelectedStudent.PlayerId;
                    }

                    _customCreditsAmount = 0;
                    _updateCreditsMessage = message;
                    OnPropertyChanged(nameof(SelectedStudent));
                    OnPropertyChanged(nameof(CustomCreditsAmount));
                    Refresh();
                    Application.Current.MainPage.Dispatcher.Dispatch(() => UpdateCreditsMessage = _updateCreditsMessage);
                }

                Waiting = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}