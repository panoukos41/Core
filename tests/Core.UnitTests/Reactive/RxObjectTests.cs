namespace Core.Reactive;

public sealed class RxObjectTests
{
    // todo: Break below method to more tests.
    // todo: Move ViewModel tests to Core.UnitTests project

    private class TestObject : ObservableObject
    {
        public string firstName = string.Empty;
        public string lastName = string.Empty;
        public double funds;

        public string FirstName { get => firstName; set => SetAndRaise(ref firstName, value); }

        public string LastName { get => lastName; set => SetAndRaise(ref lastName, value); }

        public double Funds { get => funds; set => SetAndRaise(ref funds, value); }
    }

    //[Fact]
    //public void Should_Change_Product_Properties()
    //{
    //    var obj = new TestObject
    //    {
    //        FirstName = "Test",
    //        LastName = "Test",
    //        Funds = 10,
    //    };

    //    var propertyChangedFired = false;
    //    var propertyChangingFired = false;
    //    Problem? problem = null;

    //    ((INotifyPropertyChanging)obj).PropertyChanging += (s, e) => propertyChangingFired = true;
    //    ((INotifyPropertyChanged)obj).PropertyChanged += (s, e) => propertyChangedFired = true;

    //    obj.WhenValueChanged(x => x.Name).Subscribe(x => name = x);
    //    obj.WhenValueChanged(x => x.Total).Subscribe(x => total = x);
    //    obj.WhenPropertyChanged.Subscribe();
    //    obj.WhenProblem.Subscribe(x => problem = x);


    //    total.Should().Be(10);

    //    viewModel.Price = 20;
    //    total.Should().Be(20);

    //    viewModel.Quantity = 2;
    //    total.Should().Be(40);

    //    viewModel.Price = -1;
    //    viewModel.Price.Should().Be(20);
    //    problem.Should().NotBeNull();

    //    obj.Name.Should().Be("Test");
    //    obj.Price.Should().Be(10);
    //    obj.Quantity.Should().Be(1);

    //    propertyChangingFired.Should().BeTrue();
    //    propertyChangedFired.Should().BeTrue();
    //}
}
