using Minesweeper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Minesweeper.ViewModel
{
    class CellViewModel : ViewModelBase
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int State { get; set; } // -1 = mina; 0 - 8 = počet min v okolí

        private string _displayContent;
        public string DisplayContent
        {
            get => _displayContent;
            set { _displayContent = value; OnPropertyChanged(); }
        }

        private Brush _backgroundColor = Brushes.LightGray;
        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set { _backgroundColor = value; OnPropertyChanged(); }
        }
    }
}
