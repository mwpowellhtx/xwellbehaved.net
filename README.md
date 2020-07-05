<img src="assets/xwellbehaved_dotnet_256x256.png" width="128" />

# xWellBehaved.net

_[![Xwellbehaved NuGet package](https://img.shields.io/nuget/v/Xwellbehaved.svg?label=Xwellbehaved%20NuGet%20Package)](https://nuget.org/packages/Xwellbehaved)_
_[![Xwellbehaved.Core NuGet package](https://img.shields.io/nuget/v/Xwellbehaved.Core.svg?label=Xwellbehaved.Core%20NuGet%20Package)](https://nuget.org/packages/Xwellbehaved.Core)_

_[![Find your community at https://gitter.im/xwellbehaved/xwellbehaved.net](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/xwellbehaved/xwellbehaved.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)_

Documentation coming soon. We are considering translating some of the docs in terms of *xWellBehaved.net*, but that is a future effort at the moment.

xWellBehaved.net is an [*xUnit.net*](https://github.com/xunit/xunit) extension, available [in full](https://www.nuget.org/packages/Xwellbehaved) or [minimal](https://www.nuget.org/packages/Xwellbehaved.Core) form, for [*Behavior Driven Development (BDD)*](https://en.wikipedia.org/wiki/Behavior-driven_development) or [*Test Driven Development (TDD)*](https://en.wikipedia.org/wiki/Test-driven_development) using natural language.

Platform support: [.NET Standard 2.0 and upwards](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

## Packages

The [full `Xwellbehaved` package](https://www.nuget.org/packages/Xwellbehaved) depends on the [`xunit` package](https://www.nuget.org/packages/xunit). That means you get the full suite of *xUnit.net* dependencies such as [`xunit.assert`](https://www.nuget.org/packages/xunit.assert) and [`xunit.analyzers`](https://www.nuget.org/packages/xunit.analyzers).

The [minimal `Xwellbehaved.Core` package](https://www.nuget.org/packages/Xwellbehaved.Core) depends on the [`xunit.core` package](https://www.nuget.org/packages/xunit.core). That means you get only the minimum dependencies required to write and execute *xWellBehaved.net* scenarios.

## Versioning

We are of the opinion that, *xWellBehaved.net* is not providing feature symmetry with *xUnit.net*, per se, nor does it pretend to stand on the same `netstandard` API surface area. In fact, key areas that facilitate this being "well behaved", so to speak, do depend on `netstandard2.0`. Along similar lines, we are also unconstrained by historical use cases for the API, so this frees us up a little to make these decisions.

Out of respect, however, we will begin by versioning based on the existing [*xBehave.net*](https://www.nuget.org/packages/xbehave) versions, but will most certainly branch out from there.

As a matter of protocol, we do make an effort to provide [*Semantic Versioning 2.0.0*](https://semver.org/spec/v2.0.0.html) compatible package versions, and will bump major, minor, etc, fields according to the severity of the changes.

A given *xWellBehaved.net* patch version may introduce new features, fix bugs, or both.

---

<sub>*xWellBehaved.net* logo designed by [Michael W. Powell](https://github.com/mwpowellhtx).</sub>
