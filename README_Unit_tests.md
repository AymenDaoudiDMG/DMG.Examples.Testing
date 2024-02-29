Unit tests.

# Introduction

Unit tests are important to ensure that small isolated parts of the software behave as expected. They aim to test methods or functions in complete isolation. To achieve this isolation we generally mock (stub or fake) the underlying dependencies.

## Terminology

Understanding and using the right terminology is a key element to understand how testing works and how to write effective easy to read tests. Several terms and concepts are used in the testing world, here are the main terms that are widely used.

- **Sut** stands for **System Under Test**, which is simply the object that you will test. You will call functions of this object and you will assert the results that it returns to verify if they are as expected or not.
- **Mocking**: is the operation of imitating the behavior of a function by setting a predefined behavior, they don't necessarily have working implementations but a simplified one and they have pre-programmed expectations. They help you run tests against your `Sut`'s method by mocking the dependencies to achieve the isolation.
- **Stubbing**: Stubs are defined to return predefined values, they don't contain any implementation.
- **Faking**: Fakes on the other hand, have working implementation, they are used to imitate a call to an external application, service or API.

Mocking, Stubbing and Faking are generally easily used interchangeably, due to the similarity. I personally just use the term mocking for all situations, if you want to be more specific feel free to do it.

- **Setups**: a setup is a class that defines the mocking of the operations of entity being used by a `Sut`, in order to keep this one isolated.

# Unit testing an ASP.Net api

## Guidelines

Having a well written API is an important first step into writing unit tests. Tightly coupled Apis with no separation of concerns and poor respect of SOLID principals can be very hard, if not impossible, to test.
Unit tests can be run on many types of software, including frontend applications, libraries or Apis. In these latter, we can unit test any part, it can be the endpoint logic, the business logic, or the data access logic. All this is possible as long as these components are well isolated and not tightly coupled.

In most cases when we try to unit test an Api we tend to unit test the business logic, unless there is a clear requirement to test the data access logic or the endpoint logic (for example if both may contain some complex operations that we want to make sure they behave correctly). In order to unit test business logic correctly and easily, it is important to separate the methods that contain the business logic from any other type of logic. Like querying a database, pushing to queue, reading from a file system, sending notifications or emails...etc. All these operations should be delegated to another type of objects (such as Repositories). Doing so will keep your `Sut` clean, and you will be focused on testing your business logic in isolation, by mocking those repositories functions and not a DataBase reader, a file system, a cloud messaging service ...etc. Doing otherwise will make writing tests a very bad experience, and you will spend time mocking those complex systems instead of writing logic that test your business logic. 

## Frameworks and tools

Testing in .Net in general is achieved using one of these tree famous frameworks: `MSTest`, `XUnit` and `NUnit`. During this article I will be using `XUnit`, but you are free to adopt any other one, as they all converge to the same output at the end.

In addition to the test framework, some libraries are really useful to write your unit tests, these are one of the most useful ones:

- **AutoFixture**: a great tool to create fixtures of any type, with or without customization, it takes the burden of instantiating objects, services, dependencies. Which prevents you from writing imperative code which is generally error prone. Remember, we are here to write tests that detect bugs, we wouldn't want to introduce bugs in our tests.
For instance, this code tells auto fixture to create 20 `User` objects by making sure that the Ids remain null, and the age is strictly positive integer.
```C#
_fixture.Customize<User>(composer => composer
    .FromFactory((string name, uint age) => new User
    {
        Name = name,
        Age = (int)(age == 0 ? 1 : age)
    })
    .Without(u => u.Id)
    .OmitAutoProperties());

Users = _fixture.CreateMany<User>(20).ToList();
```

Imagine if you needed to instantiate 100 `User`s, and you were in a pre Github copilot era. Even with Github copilot, it would still be code, big code.

- **FluentAssertions**: an excellent tool that helps with writing assertions in a fluent, elegant, humanly readable fashion. To check if the expected value is correct, if an operation `Should()` or `Should()` `NotThrow()` an exception etc...

```C#
//Assert         
user.Should()
    .NotBeNull().And
    .BeEquivalentTo(expectedUser);
```

- **ExpectedObjects**: a helpful library that you can combine with FluentAssertions to assert expected objects values.

- **Moq**: Moq is a library that helps create mock objects, and setup the alternative behavior of its operations and functions. Moq can also perfectly be combined with Autofixture.

## Setting up the unit test project

To write unit tests I prefer to organize the test project as follows:


```
- Project
    |_ .\Common
    |     |__ .\AssemblyFixture.cs
    |     |__ .\MockData.cs
    |     |__ .\TestBase.cs
    |_ .\Setups
    |     |__ .\Setups
    |            |___ .\Repository1Setup.cs
    |_ .\Tests
    |     |__ .\SystemUnderTest1Tests
    |     |      |___ .\SystemUnderTest1TestBase.cs
    |     |      |___ .\Method1Tests.cs
    |     |      |___ .\Method2Tests.cs
    |     |      |___ ...
    |     |__ .\SystemUnderTest2Tests
    |     |      |___ .\SystemUnderTest2TestBase.cs
    |     |      |___ .\Method1Tests.cs
    |     |      |___ .\Method2Tests.cs
    |     |      |___ ...
    ...
```

### AssemblyFixture

This is a class that helps XUnit define the objects that you want to instantiate once for the entire assembly (The test project).

```C#
[CollectionDefinition(nameof(AssemblyFixture))]
public class AssemblyFixture :
    ICollectionFixture<MockData>,
    ICollectionFixture<UserRepositorySetup>
{
}
```

This basically tells XUnit that every class that is decorated with the attribute `[Collection(nameof(AssemblyFixture))]` will have `MockData` and `UserRepositorySetup` objects injected into its constructor if they are required, such as in the following class for example:

```C#
[Collection(nameof(AssemblyFixture))]
public class TestBase
{
    protected readonly DataFixture _dataFixture;

    public TestBase(MockData _mockData)
    {
        _dataFixture = dataFixture;
    }

```

Assembly fixtures are needed to share objects that are reused across all test classes of the test project.

### MockData

The `MockData.cs` is generally a class that contains your fake data source that imitates a data source, like a database or a message queue. It's the place where we create lists of our entities on which we will use as fake data in our tests, as shown in the Autofixture section, we create such fake data as follows:

```C#
public class MockData
{
    private readonly IFixture _fixture = new Fixture();
    public int UserIdCounter { get; set; } = 0;
    public List<User> Users { get; private set; }

    public MockData()
    {
        Init();
    }

    private void Init()
    {
        _fixture.Customize<User>(composer => composer
            .FromFactory((string name, uint age) => new User
            {
                Id = (++UserIdCounter).ToString(),
                Name = name,
                Age = (int)(age == 0 ? 1 : age)
            })
            .OmitAutoProperties());

        Users = _fixture.CreateMany<User>(20).ToList();
    }
}
```

Notice how we tell Autofixture to customize the creation of any instance of the User entity, by precising how the Id is created and by checking that the age is a strictly positive integer, we let however Autofixture to assign any value it desires to the name property.

### TestBase

`TestBase.cs` is the base class for all your test classes. It defines objects that are shared by all test classes in order to avoid redundancy.

Any test class in your project will inherit `TestBase`. It contains reference to the `Sut`, to the assembly shared `MockData` along with other fields.

```C#
[Collection(nameof(AssemblyFixture))]
public abstract class TestBase<TSut> where TSut : IService
{
    protected readonly ITestOutputHelper _output;
    protected readonly MockData _mockData;
    protected readonly IFixture _fixture;
    protected readonly TSut _sut;

    public TestBase(
        MockData mockData, 
        ITestOutputHelper output,
        Func<IFixture, TSut> sutFactory,
        params Action<IFixture>[] mockFactories
    )
    {
        _output = output;
        _mockData = mockData;
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        
        foreach (var mockFactory in mockFactories)
            mockFactory.Invoke(_fixture);
        
        _sut = sutFactory.Invoke(_fixture);
    }
}
```

Notice that XUnit automatically injects an object of type `ITestOutputHelper` when you ask for it. This object is simply a logger helper that you can use to output messages to the testing log, to explain some behavior, it can be achieved in your test cases for example by doing:

```C#
output.WriteLine("This passed because ...");
```
The `TestBase` constructor also takes two important parameters: `sutFactory` and `mockFactories`.

- **sutFactory** is a function that helps instantiating the right `Sut` on demand at the concrete test class level. Using a factory like this is less imperative, very abstract and highly flexible.
- **mockFactories** is a list of factories that help creating instances of the mock objects that our `Sut` need during it's creation and that we also need inside `Setup`s. The following examples will explain:

This following code is base TestBase for a `Sut` called `UserService`, it implements `TestBase`.

```C#
public class UserServiceTestBase : TestBase<IUserService>
{
    protected readonly UserRepositorySetup _userRepositorySetup;

    public UserServiceTestBase(MockData mockData, ITestOutputHelper output) 
    : base(
        mockData, 
        output,
        sutFactory: f => f.Create<UserService>(),
        mockFactories: f => f.Freeze<Mock<IUserRepository>>()
    )
    {
        _userRepositorySetup = _fixture.Create<UserRepositorySetup>();
    }
}
```

You can see that we use `f => f.Create<UserService>()` function to define how our `Sut` needs to be created. and we use the function `f => f.Freeze<Mock<IUserRepository>>()` to tell how we want the mocks to be created. Notice also the `Freeze` method from AutoFixture that we ware using. What it does is that it "Freezes" the type `Mock<IUserRepository>` to a common value, so that whenever `IRepository` is encountered when Autofixture is creating objects, it will automatically replaced by that "Freezed" `Mock<IUserRepository>`. `IUSerService` needs `IUserRepository` in its definition as you can see here:

```C#
public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    
    ...
}
```

Autofixture can collaborate with Moq to assign that mock object `Mock<IUserRepository>` during the creation of the `Sut` `UserService`, because it was configured to do so in the `TestBase` constructor, take a look at it again:

```C#
_fixture = new Fixture().Customize(new AutoMoqCustomization());
```

### Setups

Setups as we mentioned before help you define the fake behavior of the methods of your mocked objects that your `Sut` uses, in our previous example, we mocked `IUserRepository`, so we need to to define fake behavior for all of its methods:

```C#
public interface IUSerRepository<TModel> where TModel : ModelBase
{
    Task<IEnumerable<TModel>> GetAllAsync();

    Task<TModel?> GetAsync(string id);

    Task<TModel> CreateAsync(TModel user);
}
```

The setup class would look like this:

```C#
public class UserRepositorySetup
{
    public Mock<IUserRepository> UserRepositoryMock;

    public void GetAllAsyncSetup(Func<IEnumerable<User>> returnAction)
    {
        UserRepositoryMock
            .Setup(b => b.GetAllAsync())
            .ReturnsAsync(returnAction);
    }

    public void GetAsyncSetup(Func<string, User?> returnAction)
    {
        UserRepositoryMock
            .Setup(b => b.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(returnAction);
    }

    public void CreateAsyncSetup(Func<User, User> returnAction)
    {
        UserRepositoryMock
            .Setup(b => b.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(returnAction);
    }
}
```

When we instantiate `UserRepositorySetup` using Autofixture, it will automatically assign the "freezed" value of `Mock<IUserRepository>` to UserRepositoryMock variable.

### TestCases

Now that we have all the components ready, we can start defining our test cases, which are the core of our work, it's there where the job is done. It is a good practice to follow these guidelines in order to have simple, easily understood test cases:

- **Naming**: the test cases should be named exactly with what they do, they should be as explicit as possible, even if they may look long and not beautiful. Remember this is testing, not code for an actual software. In order to make the long test cases method names look readable we may:
    - Use underscores between words.
    - Use display name that your test framework provides, so that the IDE can display them in a more natural way.

```C#
[Fact(DisplayName = "When GetAllAsync is called then all users are returned")]
public async Task When_GetAllAsync_Is_Called_Then_All_Users_Are_Returned()
{
    //Test logic here
}
```
- **Test logic structure**: It is highly recommended and a common good practice to format the content of your test case as follows:
    - Arrange: is the step where you setup objects that you are going to need to perform the test operation, this may include mock objects and expected results.
    - Act: is where the call to the `sut` is performed.
    - Assert: is where the assertions are run to compare the Act result against the predefined expected results, which will tell the test framework if this test case passed or not.

- **You must** be as declarative and as imperative as possible in your code, everything that is directly related to the test case should be declared in the test case, your code must not call shared methods or encapsulated logic in other classes or methods, the test case reader should not need to go and search elsewhere.

```C#
[Theory(DisplayName = "When GetAsync is called then no exception is thrown")]
[InlineData(null)]
[InlineData("0")]
[InlineData("1")]
[InlineData("5")]
[InlineData("10")]
[InlineData("15")]
[InlineData("20")]
[InlineData("2147483647")]
public async void When_GetAsync_Is_Called_Then_No_Exception_Is_Thrown(string id)
{
    //Arrange
    _userRepositorySetup.GetAsyncSetup((id) => _mockData.Users.SingleOrDefault(u => u.Id == id));

    //Act
    Func<string, Task<User?>> func = _sut.GetAsync;

    //Assert             
    await func
        .Invoking(f => f(id))
        .Should()
        .NotThrowAsync<Exception>();
}
```

Notice in this example how we pass the fake behavior to the `GetAsyncSetup` of the `UserRepositorySetup`, like this it will just search in our fake data instead of performing the real code of querying the database.