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
        public ObservableCollection<Reward> Rewards { get; set; } = [];

        private Student _currentStudent;

        private Reward _currentReward;

        public Student SelectedStudent
        {
            get => _currentStudent;
            set
            {
                _currentStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }

        public Reward SelectedReward
        {
            get => _currentReward;
            set
            {
                _currentReward = value;
                OnPropertyChanged(nameof(SelectedReward));
            }
        }
        public ICommand UpdateCustomCreditCommand { get; }
        public ICommand UpdateRewardCommand { get; }
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

        private string _updateRewardMessage;
        public string UpdateRewardMessage
        {
            get => _updateRewardMessage;
            set
            {
                _updateRewardMessage = value;
                OnPropertyChanged(nameof(UpdateRewardMessage));
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

            UpdateRewardCommand = new Command(async () => await UpdateReward());


            LoadStudents();
            LoadRewards();
        }

        private async void LoadStudents()
        {
            _showErrorMessage = string.Empty;
            OnPropertyChanged(nameof(ShowErrorMessage));

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
                _showErrorMessage = "Failed finding students.";
                OnPropertyChanged(nameof(ShowErrorMessage));
            }

            Waiting = false;
        }

        private void Refresh()
        {
            LoadStudents();
            LoadRewards();
        }

        private async Task GetStudentById(string studentId)
        {
            _showErrorMessage = string.Empty;
            OnPropertyChanged(nameof(ShowErrorMessage));

            Waiting = true;

            var response = await _studentService.GetStudentAsync(studentId);

            if (string.Equals(response.PlayerId, "NOTFOUND"))
            {
                _showErrorMessage = "Failed finding student.";
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
                    message = "Something failed while trying to update credits.";
                    _updateCreditsMessage = message;
                    Application.Current.MainPage.Dispatcher.Dispatch(() => UpdateCreditsMessage = _updateCreditsMessage);
                    OnPropertyChanged(nameof(CustomCreditsAmount));
                }

                else
                {
                    if (_customCreditsAmount < 0)
                    {
                        message = "Subtracted " + (_customCreditsAmount * -1) + " credits to student " + SelectedStudent.PlayerId + " aka " + SelectedStudent.StudentNick;
                    }

                    else
                    {
                        message = "Added " + _customCreditsAmount + " credits to student " + SelectedStudent.PlayerId + " aka " + SelectedStudent.StudentNick;
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

        private async void LoadRewards()
        {
            _updateRewardMessage = string.Empty;
            OnPropertyChanged(nameof(ShowErrorMessage));

            Waiting = true;

            var response = await _studentService.GetRewardsAsync();

            if (response.Count > 0)
            {
                _updateRewardMessage = string.Empty;
                OnPropertyChanged(nameof(ShowErrorMessage));
                Rewards.Clear();
                foreach (var reward in response)
                {
                    Rewards.Add(reward);
                }
            }

            else
            {
                Students.Clear();
                _updateRewardMessage = "Failed finding rewards.";
                OnPropertyChanged(nameof(ShowErrorMessage));
            }

            Waiting = false;
        }

        private async Task UpdateReward()
        {

            Waiting = true;

            string message = string.Empty;

            var result = await _studentService.UpdateReward(SelectedReward.Id, SelectedReward.Name, SelectedReward.Price);
            if (result != HttpStatusCode.OK)
            {
                message = "Something failed while trying to update rewards.";
                _updateRewardMessage = message;
                Application.Current.MainPage.Dispatcher.Dispatch(() => UpdateRewardMessage = _updateRewardMessage);
                OnPropertyChanged(nameof(CustomCreditsAmount));
            }

            else
            {
                message = "Reward Updated!";

                _updateRewardMessage = message;
                OnPropertyChanged(nameof(SelectedReward));
                Refresh();
                Application.Current.MainPage.Dispatcher.Dispatch(() => UpdateRewardMessage = _updateRewardMessage);
            }

            Waiting = false;

        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}