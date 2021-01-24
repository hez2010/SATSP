using OxyPlot;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SATSP
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private string status = "";
        private bool dataLoaded;
        private int iteration = 1500;
        private bool computing;

        public string FileName { get; set; } = "";

        public string TourFileName { get; set; } = "";

        public bool DataLoaded
        {
            get => dataLoaded;
            set
            {
                dataLoaded = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public int Iteration
        {
            get => iteration;
            set
            {
                iteration = value;
                OnPropertyChanged();
            }
        }
        public double TemperatureMax { get; set; } = 100;
        public double TemperatureMin { get; set; } = 1;
        public double Alpha { get; set; } = 0.99;
        public bool SimulatedAnnealing { get; set; } = false;

        public bool Computing
        {
            get => computing;
            set
            {
                computing = value;
                OnPropertyChanged();
            }
        }

        public BulkObservableCollection<DataPoint> Result { get; } = new();
        public BulkObservableCollection<DataPoint> ResultPath { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? propertyName = default) => PropertyChanged?.Invoke(this, new(propertyName));
    }
}
