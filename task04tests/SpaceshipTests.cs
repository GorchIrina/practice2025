using Xunit;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(40, fighter.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Cruiser_ShouldBeMorePowerfulThanFighter()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.FirePower < cruiser.FirePower);
    }

    [Fact]
    public void MoveForward_PositionBySpeed()
    {
        var cruiser = new Cruiser();
        cruiser.MoveForward();
        Assert.Equal(50, cruiser.Position);

        var fighter = new Fighter();
        fighter.MoveForward();
        fighter.MoveForward();
        Assert.Equal(200, fighter.Position);
    }

    [Fact]
    public void Fire_ShouldIncrementShots()
    {
        var cruiser = new Cruiser();
        cruiser.Fire();
        Assert.Equal(1, cruiser.Shots);

        var fighter = new Fighter();
        fighter.Fire();
        fighter.Fire();
        Assert.Equal(2, fighter.Shots);
    }

    [Fact]
    public void Rotate_ShouldUpdateAngle()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(300);
        cruiser.Rotate(100);
        Assert.Equal(40, cruiser.CurrAngle);

        var fighter = new Fighter();
        fighter.Rotate(-45);
        fighter.Rotate(45);
        Assert.Equal(0, fighter.CurrAngle);
    }
}
