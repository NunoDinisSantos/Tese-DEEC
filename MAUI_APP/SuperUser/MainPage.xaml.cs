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
                OnPropertyChanged();
            }
        }

        private string _updateCreditsMessage;
        public string UpdateCreditsMessage
        {
            get => _updateCreditsMessage;
            set
            {
                _updateCreditsMessage = value;
                OnPropertyChanged();
            }
        }

        private string _studentNumberToSearch;

        public string StudentNumberToSearch
        {
            get => _studentNumberToSearch;
            set
            {
                _studentNumberToSearch = value;
                OnPropertyChanged();
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
            var response = await _studentService.GetStudentsAsync();

            if (response.Count>0)
            {
                Students.Clear();
                foreach (var student in response)
                {
                    Students.Add(student);
                }
            }
        }
        private void Refresh()
        {
            LoadStudents();
        }

        private async Task GetStudentById(string studentId)
        {
            var response = await _studentService.GetStudentAsync(studentId);
            Students.Clear();
            Students.Add(response);
        }

        private async Task UpdateCredits(int sumCredits)
        {
            if (SelectedStudent != null)
            {
                string message = string.Empty;
                var result = await _studentService.UpdateStudentCreditsAsync(SelectedStudent.PlayerId, sumCredits);
                if (result != HttpStatusCode.OK)
                {
                    message = "Algo falhou a tentar actualizar créditos.";
                    _updateCreditsMessage = message;
                    OnPropertyChanged(UpdateCreditsMessage);
                }

                else
                {
                    _customCreditsAmount = 0;
                    message = "Créditos actualizados com sucesso.";
                    OnPropertyChanged(nameof(SelectedStudent));
                    OnPropertyChanged(UpdateCreditsMessage);

                    _updateCreditsMessage = message;
                    Refresh();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}