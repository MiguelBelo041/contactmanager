using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ContactManager.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject source)
            return;

        var current = source;
        while (current != null)
        {
            if (current is DataGrid || current is DataGridRow || current is DataGridCell
                || current is TextBox || current is ButtonBase)
                return;

            current = VisualTreeHelper.GetParent(current);
        }

        ContactsGrid.SelectedItem = null;
    }

    private void Telefone_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsDigit);
    }

    private void Telefone_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
            e.Handled = true;
    }

    private void Telefone_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var pasted = (string)e.DataObject.GetData(typeof(string));
            if (!pasted.All(char.IsDigit))
                e.CancelCommand();
        }
        else
        {
            e.CancelCommand();
        }
    }
}
