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

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly StudentService _studentService;
        public ObservableCollection<Estudante> Estudantes { get; set; } = new ObservableCollection<Estudante>();
        private Estudante _currentStudent;

        public Estudante EstudanteSeleccionado
        {
            get => _currentStudent;
            set
            {
                _currentStudent = value;
                OnPropertyChanged(nameof(EstudanteSeleccionado));
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

        private string _numeroEstudanteProcurar;

        public string NumeroEstudanteProcurar
        {
            get => _numeroEstudanteProcurar;
            set
            {
                _numeroEstudanteProcurar = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(StudentService studentService)
        {
            _studentService = studentService;
            UpdateCustomCreditCommand = new Command(async () => await UpdateCredits(CustomCreditsAmount));
            GetStudentByIdAsync = new Command(async () => await GetStudentById(NumeroEstudanteProcurar));
            RefreshData = new Command(async () => await Refresh());

            LoadStudents();
        }

        private async void LoadStudents()
        {
            var response = await _studentService.GetStudentsAsync();

            if (response.Count>0)
            {
                Estudantes.Clear();
                foreach (var student in response)
                {
                    Estudantes.Add(student);
                }
            }
        }
        private async Task Refresh()
        {
            LoadStudents();
        }

        private async Task GetStudentById(string studentId)
        {
            var response = await _studentService.GetStudentAsync(studentId);
            Estudantes.Clear();
            Estudantes.Add(response);
        }

        private async Task UpdateCredits(int sumCreditos)
        {
            if (EstudanteSeleccionado != null)
            {
                string message = string.Empty;
                var result = await _studentService.UpdateStudentCreditsAsync(EstudanteSeleccionado.PlayerId, sumCreditos);
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
                    OnPropertyChanged(nameof(EstudanteSeleccionado));
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