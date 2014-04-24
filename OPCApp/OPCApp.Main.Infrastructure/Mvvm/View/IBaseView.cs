namespace OPCApp.Infrastructure.Mvvm.View
{
    public interface IBaseView
    {
        object DataContext { set; get; }
        void CloseView();

        void Cancel();
        bool? ShowDialog();
    }
}