using System.Net;
using System.Runtime.CompilerServices;
using eppo_sdk;
using eppo_sdk.dto;
using eppo_sdk.logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpYaml.Tokens;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace eppo_sdk_test;
public class BanditClientTest
{
    private const string BANDIT_CONFIG_FILE = "files/ufc/bandit-flags-v1.json";
    private const string BANDIT_MODEL_FILE = "files/ufc/bandit-models-v1.json";
    private const string FLAG_CONFIG_FILE = "files/ufc/flags-v1.json";
    private WireMockServer? _mockServer;
    private ContextAttributes? _subject;

    [OneTimeSetUp]
    public void Setup()
    {
        SetupMockServer();
        SetupSubjectMocks();
        var config = new EppoClientConfig("mock-api-key", new TestAssignmentLogger())
        {
            BaseUrl = _mockServer?.Urls[0]!
        };
        EppoClient.Init(config);
    }

    private void SetupSubjectMocks()
    {
        _subject = new ContextAttributes("userID")
            {
                {"account_age", 3},
                {"favourite_colour","red"}
            };
        _subject["timeofday"] = "night";
        _subject["loyalty_tier"] = "gold";
    }

    private void SetupMockServer()
    {
        _mockServer = WireMockServer.Start();
        Console.WriteLine($"MockServer started at: {_mockServer.Urls[0]}");
        this._mockServer
            .Given(Request.Create().UsingGet().WithPath(new RegexMatcher("flag-config/v1/config")))
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBody(GetMockBanditConfig()).WithHeader("Content-Type", "application/json"));
        this._mockServer
            .Given(Request.Create().UsingGet().WithPath(new RegexMatcher("flag-config/v1/bandits")))
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBody(GetMockBanditModelConfig()).WithHeader("Content-Type", "application/json"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _mockServer?.Stop();
    }

    private static string GetMockFlagConfig() => GetMockConfig(FLAG_CONFIG_FILE);

    private static string GetMockBanditConfig() => GetMockConfig(BANDIT_CONFIG_FILE);
    private static string GetMockBanditModelConfig() => GetMockConfig(BANDIT_MODEL_FILE);

    private static string GetMockConfig(string resourceFile)
    {
        var filePath = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.FullName,
            resourceFile);
        using var sr = new StreamReader(filePath);
        return sr.ReadToEnd();
    }


    // internal class TestAssignmentLogger : IAssignmentLogger
    // {
    //     public void LogAssignment(AssignmentLogData assignmentLogData)
    //     {
    //         // Do nothing
    //     }
    // }


    // [Test, TestCaseSource(nameof(GetTestAssignmentData))]
    //     public void ShouldValidateAssignments(AssignmentTestCase assignmentTestCase)
    //     {
    //         var client = EppoClient.GetInstance();


    //         switch (assignmentTestCase.VariationType)
    //         {
    //             case (EppoValueType.BOOLEAN):
    //                 var boolExpectations = assignmentTestCase.Subjects.ConvertAll(x => (bool?)x.Assignment);
    //                 var assignments = assignmentTestCase.Subjects.ConvertAll(subject =>
    //                     client.GetBooleanAssignment(assignmentTestCase.Flag, subject.SubjectKey, subject.SubjectAttributes, (bool)assignmentTestCase.DefaultValue));

    //                 Assert.That(assignments, Is.EqualTo(boolExpectations), $"Unexpected values for test file: {assignmentTestCase.TestCaseFile}");
    //                 break;
    //             case (EppoValueType.INTEGER):
    //                 var longExpectations = assignmentTestCase.Subjects.ConvertAll(x => (long?)x.Assignment);
    //                 var longAssignments = assignmentTestCase.Subjects.ConvertAll(subject =>
    //                     client.GetIntegerAssignment(assignmentTestCase.Flag, subject.SubjectKey, subject.SubjectAttributes, (long)assignmentTestCase.DefaultValue));

    //                 Assert.That(longAssignments, Is.EqualTo(longExpectations), $"Unexpected values for test file: {assignmentTestCase.TestCaseFile}");
    //                 break;
    //             case (EppoValueType.JSON):
    //                 var jsonExpectations = assignmentTestCase.Subjects.ConvertAll(x => (JObject)x.Assignment);
    //                 var jsonAssignments = assignmentTestCase.Subjects.ConvertAll(subject =>
    //                     client.GetJsonAssignment(assignmentTestCase.Flag, subject.SubjectKey, subject.SubjectAttributes, (JObject)assignmentTestCase.DefaultValue));

    //                 Assert.That(jsonAssignments, Is.EqualTo(jsonExpectations), $"Unexpected values for test file: {assignmentTestCase.TestCaseFile}");


    //                 // Also verify that the GetJsonStringAssignment method is working.
    //                 var jsonStringExpectations = assignmentTestCase.Subjects.ConvertAll(x => ((JObject)x.Assignment).ToString());
    //                 var jsonStringAssignments = assignmentTestCase.Subjects.ConvertAll(subject =>
    //                     client.GetJsonStringAssignment(assignmentTestCase.Flag, subject.SubjectKey, subject.SubjectAttributes, ((JObject)assignmentTestCase.DefaultValue).ToString()));

    //                 Assert.That(jsonStringAssignments, Is.EqualTo(jsonStringExpectations), $"Unexpected values for test file: {assignmentTestCase.TestCaseFile}");
    //                 break;
    //             case (EppoValueType.NUMERIC):
    //                 var numExpectations = assignmentTestCase.Subjects.ConvertAll(x => (double?)x.Assignment);
    //                 var numAssignments = assignmentTestCase.Subjects.ConvertAll(subject =>
    //                     client.GetNumericAssignment(assignmentTestCase.Flag, subject.SubjectKey, subject.SubjectAttributes, (double)assignmentTestCase.DefaultValue));

    //                 Assert.That(numAssignments, Is.EqualTo(numExpectations), $"Unexpected values for test file: {assignmentTestCase.TestCaseFile}");
    //                 break;
    //             case (EppoValueType.STRING):
    //                 var stringExpectations = assignmentTestCase.Subjects.ConvertAll(x => (string)x.Assignment);
    //                 var stringAssignments = assignmentTestCase.Subjects.ConvertAll(subject =>
    //                     client.GetStringAssignment(assignmentTestCase.Flag, subject.SubjectKey, subject.SubjectAttributes, (string)assignmentTestCase.DefaultValue));

    //                 Assert.That(stringAssignments, Is.EqualTo(stringExpectations), $"Unexpected values for test file: {assignmentTestCase.TestCaseFile}");
    //                 break;
    //         }
    //     }


    //     static List<AssignmentTestCase> GetTestAssignmentData()
    //     {
    //         var dir = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.FullName;
    //         var files = Directory.EnumerateFiles($"{dir}/files/ufc/tests", "*.json");
    //         var testCases = new List<AssignmentTestCase>(){};
    //         foreach (var file in files) {
    //             var atc = JsonConvert.DeserializeObject<AssignmentTestCase>(File.ReadAllText(file))!;
    //             atc.TestCaseFile = file;
    //             testCases.Add(atc);
    //         }
    //         return testCases;
    //     }
}

// internal class TestAssignmentLogger : IAssignmentLogger
// {
//     public void LogAssignment(AssignmentLogData assignmentLogData)
//     {
//         // Do nothing
//     }
// }
