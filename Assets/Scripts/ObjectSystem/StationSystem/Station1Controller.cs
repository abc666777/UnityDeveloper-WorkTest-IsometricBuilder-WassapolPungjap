namespace ObjectSystem.StationSystem
{
    public class Station1Controller : ObjectController, IStation
    {
        public void ShowMenu()
        {
            Station1Ui.ShowMenu.Invoke();
        }
    }
}
