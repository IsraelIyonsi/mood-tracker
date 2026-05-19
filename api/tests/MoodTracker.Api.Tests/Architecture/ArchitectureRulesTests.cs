namespace MoodTracker.Api.Tests.Architecture;

using System.Reflection;
using MoodTracker.Api.Common.Abstractions;
using NetArchTest.Rules;

public class ArchitectureRulesTests
{
    private static readonly Assembly ApiAssembly = typeof(IRequestHandler<,>).Assembly;

    [Fact]
    public void Handlers_AreSealed()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That().ImplementInterface(typeof(IRequestHandler<,>))
            .Should().BeSealed()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Failing types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void Handlers_AreInternal()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That().ImplementInterface(typeof(IRequestHandler<,>))
            .Should().NotBePublic()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Failing types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void Validators_AreSealedAndInternal()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That().HaveNameEndingWith("Validator")
            .Should().BeSealed().And().NotBePublic()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Failing types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void Endpoints_AreStaticClasses()
    {
        var endpointTypes = ApiAssembly.GetTypes()
            .Where(type => type.Name.EndsWith("Endpoint", StringComparison.Ordinal))
            .ToList();

        endpointTypes.ShouldNotBeEmpty();
        endpointTypes.ShouldAllBe(type => type.IsAbstract && type.IsSealed,
            "Endpoints must be static classes (abstract+sealed in IL)");
    }

    [Fact]
    public void Dtos_HaveRequestOrResponseOrQuerySuffix_AreSealed()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .HaveNameEndingWith("Request").Or()
            .HaveNameEndingWith("Response").Or()
            .HaveNameEndingWith("Query").Or()
            .HaveNameEndingWith("View")
            .And().ResideInNamespaceContaining("Features")
            .Should().BeSealed()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Failing types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }

    [Fact]
    public void Features_DoNotReferenceOtherFeatures()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespace("MoodTracker.Api.Features.Moods.LogMood")
            .Should().NotHaveDependencyOn("MoodTracker.Api.Features.Moods.GetRecentMoods")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Failing types: {string.Join(", ", result.FailingTypes?.Select(t => t.FullName) ?? [])}");
    }
}
