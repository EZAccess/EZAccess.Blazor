using System.ComponentModel;

namespace EZAccess.Blazor.Forms;

public class EZInPutHasFocus : INotifyPropertyChanged
{
    private ElementReference _input;

    public ElementReference Input
    {
        get { return _input; }
        set { 
            if (_input.GetHashCode != value.GetHashCode)
            {
                _input = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("input"));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
