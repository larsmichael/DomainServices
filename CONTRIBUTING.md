# How to contribute to Domain Services

Domain Services is open for contributions.

The easiest way to contribute is to create an [issue](https://github.com/DomainServices/issues) and start a discussion. Once decided if and how an issue should be resolved, you must submit a [pull request](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests) with suggested code modifications.


## Filing issues
Filing an issue is the perfect starting point for a contribution - no matter whether it concerns a bug fix, a modification or a new feature. Filing an issue can sometimes be preceded by more informal communication such as direct messaging or email communication, but the sooner the discussion is moved into the issue tracker the better. The issue tracker is much better suited for involving other community members, labeling, references etc.

For tips and tricks regarding filing issues, see [Mastering Issues](https://guides.github.com/features/issues/).

## Creating pull requests
When code modifications are agreed upon, you must create a new branch for this development. When naming the branch, please follow this naming convention:

- `feature/{description}` - for example **feature/add-method-foo-in-type-bar**
- `bugfix/{description}` - for example **bugfix/fix-method-foo-in-type-bar**

Once the code modifications in this branch are ready, you create a pull request (PR). PRs are a clean way so submit, review and merge proposed contributions.

To make the review- and approval process as seamless as possible, you must follow some best practices for creating PRs. This is a minimum checklist of good practices:

- Write a good title and description.
- Use references - e.g "[closes #123](https://bloggie.io/@kinopyo/github-tip-closing-issue-automatically-when-the-pull-request-is-merged)" in the description will close issue #123 once the PR is approved and merged.
- Keep the PR small. The fewer modified files the better.
- Adhere to the code style of the project.
- All new functionality must be covered by new tests. Likewise, fixed bugs should  be verified with new tests.
- The code must build and all unit tests must pass before creating a PR.
- All merge conflicts must be resolved before or immediately after creating a PR (in this case, mark the PR as draft). When solving merge conflicts, you must always be 100% sure about what you are doing. Otherwise, please seek advice with a maintainer of the project.

For more detailed information on how to make good pull requests, please dive into the following guidelines:

- [10 tips for better Pull Requests](https://blog.ploeh.dk/2015/01/15/10-tips-for-better-pull-requests/)
- [How to write the perfect pull request](https://github.blog/2015-01-21-how-to-write-the-perfect-pull-request/)
- [The Contentious Art of Pull Requests](https://dev.to/bytebodger/the-contentious-art-of-pull-requests-f3)
- [An Open Source Etiquette Guidebook](https://css-tricks.com/open-source-etiquette-guidebook/#for-the-user)