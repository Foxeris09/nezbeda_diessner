using Minesweeper.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => _isGameRunning == false);
        public RelayCommand StopCommand => new RelayCommand(execute => StopGame(), canExecute => _isGameRunning == true);

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


        public void StartGame()
        {
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
                    Cells[i].DisplayContent = "X";
                    Cells[i].BackgroundColor = Brushes.Red;
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
                cell.DisplayContent = count.ToString();
            }

        }

        public void StopGame()
        {
            _isGameRunning = false;
            IsReadOnly = false;
        }
    }
}
