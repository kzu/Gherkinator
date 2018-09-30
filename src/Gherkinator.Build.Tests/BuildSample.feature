Feature: Sample
  A sample feature showing how to run the file from xunit

  Scenario: SDK project should restore
    Given Foo.csproj =
"""
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Newtonsoft.Json" Version="11.0.2" />
    </ItemGroup>
</Project>
"""
    And Class1.cs = 
"""
using System;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello");
        Console.ReadLine();
    }
}
"""
    When restoring packages
    Then restore succeeds

  Scenario: Can access MSBuild state
    Given Foo.csproj =
"""
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Newtonsoft.Json" Version="11.0.2" />
    </ItemGroup>
</Project>
"""
    When restoring packages
    And building project
    Then build result is successful

  Scenario: Should invoke target by name
    Given Foo.csproj =
"""
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Newtonsoft.Json" Version="11.0.2" />
    </ItemGroup>
</Project>
"""
    When invoking restore programmatically
    Then build result is successful

  Scenario: Should invoke target by name with properties
    Given Foo.csproj =
"""
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Newtonsoft.Json" Version="11.0.2" />
    </ItemGroup>
    <ItemGroup Condition="'$(xunit)' == 'true'">
        <PackageReference Include="xunit" Version="2.4.0" />
    </ItemGroup>
</Project>
"""
    When invoking restore with properties
    Then restore assets contain conditional package reference

  Scenario: After restore the build log can be opened
    Given Foo.csproj =
"""
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Newtonsoft.Json" Version="11.0.2" />
    </ItemGroup>
</Project>
"""
    When restoring packages
    Then can open build log