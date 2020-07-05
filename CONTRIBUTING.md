# How to contribute

We really appreciate your consideration contributing to xWellBehaved.net! It is with this kind of fervent support that xWellBehaved.net and a host of other efforts are made successful. We want to make it as seamless as possible to enable contribution in order to support your working environment. We appreciate you observing these few guidelines as you do.

The following guidelines apply for code changes, but we are always happy to consider contributions in a variety of means, i.e. updates to [documentation](https://github.com/mwpowellhtx/xwellbehaved.net/wiki), answering [questions on StackOverflow](https://stackoverflow.com/search?q=xwellbehaved.net), providing help in the [chat room](https://gitter.im/mwpowellhtx/xwellbehaved.net/), blog posts and samples, [Twitter endorsements](https://twitter.com) mentions, etc.

## Preparation

Please do [raise an issue](https://github.com/mwpowellhtx/xwellbehaved.net/issues) before you embark on any *functional* changes, features or enhancements, bug fixes, etc. Clearly communicate the motivation, observation, etc, concerning the issue. This will help to avert pollution of the issue space, duplication, etc. When the issue is non-trivial, we encourage open discussion beforehand, especially before excessive effort is devoted to the patch.

It is *unnecessary** to raise any issues for non-functional changes, i.e. refactoring, adding tests, reformatting code, documentation, updating packages, and so on.

## Tests

All new features must be covered by feature tests in the [`Test.Xwellbehaved` project](https://github.com/mwpowellhtx/xwellbehaved.net/tree/master/src/Test.Xwellbehaved).

## Branches, tagging, etc

Let the owner do the tagging. This is non-negotiable. We try to keep a ready delivery pipeline, and part of that pipeline includes bumping assembly versions, tagging, etc.

Tagging will come in the form *major.minor.patch.build*, and we manage this bump policy as part of the build pipeline. If you do any work on the project, be sure to set the `BumpSpecSwitch` property to `init`, which will leave the version unchanged for you during your contribution.

Concerning branches, we try to keep the `master` and other branches relatively pristine. If we do any branching at all, we do so in our local working clones. We are open to suggestions on establishing a sane branching policy in terms of managing issues, releases, etc.

## Making changes

We are of the mindset, K.I.S.S. principle, *Keep It Simple Stupid*. How you maintain your local clones, branches, etc, is entirely up to you. We want to consider Pull Requests for acceptance through the official Github channels, always on the `master` branch. If it does not pass the *Pull Request* criteria, then we need to discuss why.

1. [Fork a repo](https://docs.github.com/en/github/getting-started-with-github/fork-a-repo) on GitHub
1. Fork the project into your Github
1. Make a local clone from your fork
1. You should have a defacto `origin` source directed to your fork on Github
1. Do your work, commit to your Github `master` branch, merge from your local branches, etc, freely
1. When you are ready to submit the changes, submit a PR request to our repository

## Handling updates from `upstream`

The purpose of this project is not to get side tracked or bogged down as a Git or Github how-to. This is really not the purpose of the project, whatsoever, and subtracts from the overall efficacy of the offering.

Personally, I like to maintain a handful of useful clones, and indicate them such as `workstream`, `protostream`, so on and so forth. I use the [Git Branching and Merging](https://git-scm.com/book/en/v2/Git-Branching-Basic-Branching-and-Merging) features far less than I do actual repository clones perhaps than I should.

Which is to say, however you maintain your local clones, remotes, so on and so forth, is entirely up to you. Our goal for contributing is to facilitate your path to the trunk as early as possible. Because, after all, no one likes been that far out on a branch, out on a limb, so to speak. So we aim to trim that tree as early as possible in order to keep a sane trunk.

## Sending a pull request

See the Github docs on [*Creating a pull request*](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request).

We want to facilitate your contribution on the `master` branch. That is the goal.

## What happens next?

The maintainers will review your pull request and provide any feedback required. If your pull request is accepted, your changes will be included in the next release. If we can determine your Twitter handle, we will mention you in the tweet which announces the release.

If you contributed a new feature or a change to an existing feature then we are always very grateful to receive updates to the [documentation](https://github.com/mwpowellhtx/xwellbehaved.net/wiki).
