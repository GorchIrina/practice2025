public class Cruiser: ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;

    public double Position { get; private set; } = 0;
    public int CurrAngle { get; private set; } = 0;
    public int Shots { get; private set; } = 0;

    public void MoveForward()
    {
        Position+= Speed;
    }
    public void Rotate(int angle)
    {
        CurrAngle = (CurrAngle + angle) % 360;
        if(CurrAngle < 0)
        {
            CurrAngle+=360;
        }
    }
    public void Fire()
    {
        Shots++;
    }
}
