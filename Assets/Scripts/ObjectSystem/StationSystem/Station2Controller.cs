namespace ObjectSystem.StationSystem
{
    public class Station2Controller : ObjectController, IStation
    {
        public void ShowMenu()
        {
            Station2Ui.ShowMenu.Invoke();
        }
    }
}
