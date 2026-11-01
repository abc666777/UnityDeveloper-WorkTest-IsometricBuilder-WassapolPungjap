namespace ObjectSystem.StationSystem
{
    public class Station3Controller : ObjectController, IStation
    {
        public void ShowMenu()
        {
            Station3Ui.ShowMenu.Invoke();
        }
    }
}
