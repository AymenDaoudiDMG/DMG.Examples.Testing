Integration tests.

# Introduction

Integration tests are important to ensure that applications behave as expected. They aim to test the entire system as a whole in a local controller environment without faking some of its components.
Not only they help you gain confidence in the good behavior of your application on your machine, but they also help you protect the shared source code against not enough tested contributions. This can be included during continuous integration.

## Docker compose

One of the challenges that people face when writing integration tests is how to replace the underlying tools and systems that the application uses, such as databases, queues, file storage, third party APIs or other applications and APIs that internally developed. Some solutions exist to overcome this is using in-memory alternatives, such as in memory databases. Although they can solve this issue and can work well in many situations, they tend to have some limitations:

- They are in many cases not easy to set up.
- If they are available, they have sometimes some limitations and do not mimic the complete behavior of the original tool. Think about a database that uses advanced tools such change data capture, triggers... some of the in-memory don't offer these tools.
- They often don't represent the real tool, they are a different tool that simulates the functioning of the original tool that we want to replace. This may cause an unexpected behavior, and hence probably erroneous integration testing results. In an integration test, we aim to test the system as it is with its real components.

To overcome this, Docker compose can be very helpful to create the same resources that are used by your application, but in a containerized environment. The containers will be the same tools that your application uses and they will behave exactly in the same way. Docker compose not only helps you have a local integration testing environment, but can also be included in your CI pipeline, allowing you to run integration tests on pull requests and different contributions to the source code. Another benefit of using docker, is that the images used can be shared across all the team or the company using the image registry (docker hub, azure ACR, aws ECR ...) which helps with consistency and facilitates the work of the different teams. 

## To not be confused with E2E tests

E2E tests are larger tests, that target an entire deployed system that mirrors production. They are high level, encompass all the components of the system, such as services, databases, logging, authentication as well as network-communication. They are a deployed system, and not like integration tests that run in a controller environment.

# Integration tests for an ASP.Net api

## Frameworks and tools

Testing in .Net in general is achieved using one of these tree famous frameworks: `MSTest`, `XUnit` and `NUnit`. During this article I will be using `XUnit`, but you are free to adopt any other one, as they all converge to the same output at the end.

In addition to the test framework, some libraries are really useful to write your integration tests, these are one of the most useful ones:

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

- **Flurl**: as its name indicates (Fluent Url), it is an http client library (around .Net's HttpClient) that helps you send http requests in a fluent elegant way.

When configuring Flurl, it will set behind the scenes the api url (for ex. ```"http://localhost"```), then you will fluently write your request by appending routes segments, query or path parameters, headers or body payloads, as in this example:

```C#
//_sut (System under test) refers to the api factory that launch our api that we are testing
// Here we configure Flurl globally
FlurlHttp.Configure(settings =>
{
    settings.FlurlClientFactory = new TestClientFactory(_sut.CreateClient());
});

// Act
var response = await "/management"
    .AppendPathSegment("/users")
    .PostJsonAsync(user);
```
You can still use other http client tools if you prefer, such as RestSharp, or just use the plain HttpClient object.

## Terminology

Understanding and using the right terminology is a key element to understand how testing works and to write effective easy to read tests. Several terms and concepts are used in the testing world, I discuss many of them in the unit testing article [here] but for this article I will explain only the `Sut` as it's generally the only one you will encounter.

**Sut** stands for **System Under Test**, which is simply the object that you will test. You will call functions of this object and you will assert the results that it returns to verify if they are as expected or not.

## Setting up the integration test project

To write an integration test I prefer to organize the test project as follows:

```
- Project
    |_ .\Common
    |     |__ .\AssemblyFixture.cs
    |     |__ .\WebApplicationFactory.cs
    |     |__ .\DataFixture.cs
    |     |__ .\TestBase.cs
    |_ .\Tests
    |     |__ .\Controller1Tests
    |     |      |___ .\Controller1TestBase.cs
    |     |      |___ .\Endpoint1Tests.cs
    |     |      |___ .\Endpoint2Tests.cs
    |     |      |___ ...
    |     |__ .\Controller2Tests
    |     |      |___ .\Controller2TestBase.cs
    |     |      |___ .\Endpoint1Tests.cs
    |     |      |___ .\Endpoint2Tests.cs
    |     |      |___ ...
    ...
```

### AssemblyFixture

This is a class that helps XUnit define the objects that you want to instantiate once for the entire assembly (The test project).

```C#
[CollectionDefinition(nameof(AssemblyFixture))]
public class AssemblyFixture : ICollectionFixture<DataFixture>
{
}
```

This basically tells XUnit that every class that is decorated with the attribute `[Collection(nameof(AssemblyFixture))]` will have `DataFixture` object injected into its constructor, such as in the following class for example:

```C#
[Collection(nameof(AssemblyFixture))]
public class TestBase
{
    protected readonly DataFixture _dataFixture;

    public TestBase(DataFixture dataFixture)
    {
        _dataFixture = dataFixture;
    }

```

Assembly fixtures are needed to share objects that are reused across all test classes of the test project.

### WebApplicationFactory

To run integration tests against an Api we obviously need that Api top be running. To launch the Api during the tests we use the `WebApplicationFactory<TProgram>` class, where `TProgram` is the entry point of the Api, `Program.cs` in the cas of ASP.Net 8.

We can also customize this class to add some settings and configurations that proper to the test scenario. This example shows a `CustomWebApplicationFactory` that uses the `appsettings.Test.json` variables and that "Replaces" the `DbContext` object created during `Program.cs` by a one that is suitable for our test scenario.

```C#
public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public IConfigurationRoot Configuration { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            Configuration = config.AddJsonFile("appsettings.Test.json")
                  .AddEnvironmentVariables()
                  .Build();
        });

        var mongodbSettings = new MongoDbSettings(
            Configuration["Database:MongoDb:Url"],
            Configuration["Database:MongoDb:DataBaseUser"],
            Configuration["Database:MongoDb:DataBasePassword"],
            Configuration["Database:MongoDb:DatabaseName"]
        );

        builder.ConfigureServices(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<DbContext>(_ => new DbContext(mongodbSettings)));
        });

        builder.UseEnvironment("Test");
    }
}
```

### TestBase

`TestBase.cs` is the base class for all your test classes. It defines objects that are shared by all test classes in order to avoid redundancy.

```C#
[Collection(nameof(AssemblyFixture))]
public class TestBase : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly IFixture _fixture;
    protected readonly ITestOutputHelper _output;
    protected readonly DataFixture _dataFixture;
    protected readonly CustomWebApplicationFactory<Program> _sut; //can be named also _apiFactory

    public TestBase(
        ITestOutputHelper output,
        DataFixture dataFixture,
        CustomWebApplicationFactory<Program> sut
    )
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _output = output;
        _dataFixture = dataFixture;
        _sut = sut;
        
        FlurlHttp.Configure(settings =>
        {
            settings.FlurlClientFactory = new TestClientFactory(_sut.CreateClient());
        });
    }
```

The `IClassFixture` interface helps setting a fixture, that is shared across all the test cases of the current test class, in contrary to an Assembly Fixture that we saw before that shares an object across the entire project.

Notice that XUnit automatically injects an object of type `ITestOutputHelper` when you ask for it. This object is simply a logger helper that you can use to output messages to the testing log, to explain some behavior, it can be achieved in your test cases for example by doing:

```C#
output.WriteLine("This passed because ...");
```

You can see that our _sut (System Under Test), is our `CustomWebApplicationFactory<Program>`, which means the Api that we are targeting. WebApplicationFactory exposes by default an `HttpClient` object that you can use to send http request. We wrap it in a Flurl configuration so we can construct http requests fluently from route segments later in our test cases.

### TestCases

Test cases are the core of our work, it's there where the job is done. It is a good practice to follow these guidelines in order to have simple, easily understood test cases:

- **Naming**: the test cases should be named exactly with what they do, they should be as explicit as possible, even if they may look long and not beautiful. Remember this is testing, not code for an actual software. In order to make the long test cases method names look readable we may:
    - Use underscores between words.
    - Use display name that your test framework provides, so that the IDE can display them in a more natural way.

```C#
[Fact(DisplayName = "When GetAll is called then response status code is ok and all users are returned")]
public async Task When_GetAll_Is_Called_Then_Response_Status_Code_Is_Ok_And_All_Users_Are_Returned()
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
public async void When_Get_Async_Is_Called_Then_No_Exception_Is_Thrown(string id)
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

## Using docker for external resources

If your Api connects to an external tool or calls an external Api, your integration test should mimic the same real scenario. Docker and/or Docker compose helps you achieve this.
If your Api connects for instance to a mongo database, you can write a docker file that configures the same database that is used by the Api, run it in a docker compose and set your WebApplicationFactory to connect to it instead of connecting to the real database. This can also be run in the CI pipeline to ensure that your integration tests are run when other contribute to the code base.