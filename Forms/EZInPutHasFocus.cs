using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EZAccess.Blazor.Forms;

[INotifyPropertyChanged]
partial class EZInputHasFocus //: INotifyPropertyChanged
{
    [ObservableProperty]
    private ElementReference _input;

}
