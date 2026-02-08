using Minesweeper.Model;
using Minesweeper.MVVM;
using Minesweeper.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Minesweeper.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly ScoreService _scoreService;
        private DispatcherTimer _timer;

        private GameScore _bestScore;
        private int _currentSeconds;

        public GameScore BestScore
        {
            get => _bestScore;
            set { _bestScore = value; OnPropertyChanged(); }
        }

        public int CurrentSeconds
        {
            get => _currentSeconds;
            set { _currentSeconds = value; OnPropertyChanged(); }
        }

        public RelayCommand StartGameCommand { get; set; }

        public MainWindowViewModel()
        {

            _scoreService = new ScoreService();
            BestScore = _scoreService.Load();
            CurrentSeconds = 0;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            StartGameCommand = new RelayCommand(o => StartGame());

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentSeconds++;
        }

        // začátek Tomíka

        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => _isGameRunning == false);
        public RelayCommand StopCommand => new RelayCommand(execute => StopGame(), canExecute => _isGameRunning == true);

        public RelayCommand RevealCellCommand => new RelayCommand(execute => Click(execute as CellViewModel), canExecute => _isGameRunning == true);

        private int _rows = 10;
        public int Rows {
            get => _rows;
            set {
                _rows = value;
                if (_isGameRunning == false)
                    OnPropertyChanged();
            }
        }
        private int _columns = 10;
        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                if (_isGameRunning == false)
                    OnPropertyChanged();
            }
        }
        private bool _isGameRunning = false;

        private int _mineCount = 20;
        public int MineCount
        {
            get => _mineCount;
            set
            {
                _mineCount = value;
                OnPropertyChanged();
            }
        }

        private bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value; 
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CellViewModel> Cells { get; set; } = new ObservableCollection<CellViewModel>();

        public int RevealedCells;

        public void StartGame()
        {
            // + časovač
            _timer.Stop();
            CurrentSeconds = 0;
            _timer.Start();

            RevealedCells = 0;


            _isGameRunning = true;
            IsReadOnly = true;
            Cells.Clear();
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Cells.Add(new CellViewModel { Row = r, Col = c, DisplayContent = "", State = 0});
                }
            }

            Random rnd = new Random();
            int placedMines = 0;
            while (placedMines < MineCount) // dořešit, když bude víc min než polí
            {
                int i = rnd.Next(Cells.Count);
                if (Cells[i].State != -1)
                {
                    Cells[i].State = -1;
                    placedMines++;
                }

            }

            foreach (var cell in Cells.Where(cell => cell.State != -1)) 
            {
                int count = 0;
                for (int r = cell.Row -1; r <= cell.Row +1; r++)
                {
                    for (int c = cell.Col -1; c <= cell.Col +1; c++)
                    {
                        var neighbor = Cells.FirstOrDefault(n => n.Row == r && n.Col == c);
                        if (neighbor != null && neighbor.State == -1)
                            count++;
                    }
                }
                cell.State = count;
            }

        }

        public void StopGame()
        {
            // + časovač
            _timer.Stop();

            _isGameRunning = false;
            IsReadOnly = false;
        }


        // kontrola rekordního času
        private void CheckNewRecord()
        {
            if (BestScore == null || CurrentSeconds < BestScore.Seconds)
            {
                BestScore = new GameScore
                {
                    Seconds = CurrentSeconds,
                    DateAchieved = DateTime.Now
                };
                _scoreService.Save(BestScore);
            }
        }

        public void Click(CellViewModel cell)
        {
            if (cell.Visible)
            {
                return;
            }

            CellReveal(cell);

            if (cell.State == -1)
            {

                MessageBox.Show("Prohrál jsi 😢");
                StopGame();
                return;
            }

            if (cell.State == 0)
            {
                RevealAround(cell);
            }

            WinCheck();
        }

        public void RevealAround(CellViewModel cell)
        {
            Queue<CellViewModel> zeros = new Queue<CellViewModel>();
            zeros.Enqueue(cell);
            while (zeros.Count > 0)
            {
                var zero = zeros.Dequeue();
                for (int r = zero.Row - 1; r <= zero.Row + 1; r++)
                {
                    for (int c = zero.Col - 1; c <= zero.Col + 1; c++)
                    {
                        var neighbor = Cells.FirstOrDefault(n => n.Row == r && n.Col == c);
                        if (neighbor == null || neighbor.Visible)
                        {
                            continue;
                        }

                        CellReveal(neighbor);

                        if (neighbor.State == 0)
                        {
                            zeros.Enqueue(neighbor);
                        }
                    }
                }

            }
        }

        public void CellReveal(CellViewModel cell)
        {
            RevealedCells++;
            cell.Visible = true;
        }

        public void WinCheck()
        {
            if (Rows * Columns == RevealedCells + MineCount)
            {
                _timer.Stop();
                CheckNewRecord();
                MessageBox.Show("Vyhrál jsi!");
            }
        }

    }
}
