<img src="assets/xwellbehaved_dotnet_256x256.png" width="128" />

# xWellBehaved.net

[![Xwellbehaved NuGet package](https://img.shields.io/nuget/v/Xwellbehaved.svg?label=Xwellbehaved%20NuGet%20Package)](https://nuget.org/packages/Xwellbehaved)

[![Xwellbehaved.Core NuGet package](https://img.shields.io/nuget/v/Xwellbehaved.Core.svg?label=Xwellbehaved.Core%20NuGet%20Package)](https://nuget.org/packages/Xwellbehaved.Core)

[![Find your community at https://gitter.im/xwellbehaved/xwellbehaved.net](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/xwellbehaved/xwellbehaved.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![TL;DR read the documentation https://github.com/mwpowellhtx/xwellbehaved.net/wiki](https://img.shields.io/badge/wiki-documentation-forestgreen)](https://github.com/mwpowellhtx/xwellbehaved.net/wiki)

_xWellBehaved.net_ is an [_xUnit.net_](https://github.com/xunit/xunit) extension, available [in full](https://nuget.org/packages/Xwellbehaved) or [minimal](https://nuget.org/packages/Xwellbehaved.Core) form, for [_Behavior Driven Development (BDD)_](https://en.wikipedia.org/wiki/Behavior-driven_development) or [_Test Driven Development (TDD)_](https://en.wikipedia.org/wiki/Test-driven_development) using natural language.

Platform support: [`.NET Standard 2.0` and upwards](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

## Introducing `TearDown`, `Background`, etc

_xWellBehaved.net_ has supported `BackgroundAttribute` for some time now. _Background_ affords developers a predictable _Background_ initialization opportunity, from the base class forward to the class whose _Scenarios_ are being invoked.

Conversely, _xWellBehaved.net_ now also supports `TearDownAttribute`, which does a similar thing, except treats _Background_ something like a stack, and unwinds _tear down_ evaluation in reverse order. Similar to `IDispose`, it is up developers to do the right thing unwinding their _BDD_ assets and resources in the correct order.

This is unlike _xUnit.net_ support for `IDispose` in the sense that unit test resources such as `ITestOutputHelper` ought still to be available for use during the _tear down_ invocation.

## Packages

The [full `Xwellbehaved` package](https://nuget.org/packages/Xwellbehaved) depends on the [`xunit` package](https://nuget.org/packages/xunit). That means you get the full suite of _xUnit.net_ dependencies such as [`xunit.assert`](https://nuget.org/packages/xunit.assert) and [`xunit.analyzers`](https://nuget.org/packages/xunit.analyzers).

The [minimal `Xwellbehaved.Core` package](https://nuget.org/packages/Xwellbehaved.Core) depends on the [`xunit.core` package](https://nuget.org/packages/xunit.core). That means you get only the minimum dependencies required to write and execute _xWellBehaved.net_ scenarios.

## Versioning

We are of the opinion that, _xWellBehaved.net_ is not providing feature symmetry with _xUnit.net_, per se, nor does it pretend to stand on the same `netstandard` API surface area. In fact, key areas that facilitate this being &quot;well behaved&quot;, so to speak, do depend on `netstandard2.0`. Along similar lines, we are also unconstrained by historical use cases for the API, so this frees us up a little to make these decisions.

Out of respect, however, we will begin by versioning based on the existing [_xBehave.net_](https://nuget.org/packages/xbehave) versions, but will most certainly branch out from there.

As a matter of protocol, we do make an effort to provide [_Semantic Versioning 2.0.0_](https://semver.org/spec/v2.0.0.html) compatible package versions, and will bump major, minor, etc, fields according to the severity of the changes.

A given _xWellBehaved.net_ patch version may introduce new features, fix bugs, or both.

---
<sub>_xWellBehaved.net_ logo designed by [_Michael W. Powell_](https://github.com/mwpowellhtx).</sub>
